/****************************************************************************
 * Copyright (C) Disappearwind. All rights reserved.                        *
 *                                                                          *
 * Author: disapearwind                                                     *
 * Created: 2009-4-27                                                       *
 *                                                                          *
 * Description:                                                             *
 *  The class for globalization                                             *
 *  carefully,the key of a resouceInfo must be unique                       *
 *  it can use comonentName_resourceInfo                                    *
 *                                                                          *
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//we must add these namespace for globalization
using System.Globalization;
using System.IO;    // for resource file
using System.Threading; //for locker
using System.Xml;       //for read resouce file
using System.Xml.XPath; //for read resouce file
using System.Reflection;   //for get currentpath

namespace Disappearwind.GlobalizationSolution.GlobalizationManager
{
    /// <summary>
    /// The class for globalization
    /// carefully,the key of a resouceInfo must be unique
    /// it can use comonentName_resourceInfo
    /// </summary>
    public class GlobalizationAdmin
    {
        /// <summary>
        /// The directory for store resouce files
        /// client can evaluate heself or the path will be set a default path in the constructor
        /// </summary>
        public static string ResourceFilePath { get; set; }
        /// <summary>
        /// The list of component in GA
        /// </summary>
        private static Dictionary<Component, GlobalizationAdmin> ComponentList = new Dictionary<Component, GlobalizationAdmin>();
        /// <summary>
        /// Container to store the resouce of globalization configuration
        /// </summary>
        private static Dictionary<string, string> ResouceList = new Dictionary<string, string>();
        /// <summary>
        /// The current resouce file path 
        /// </summary>
        private static string m_ResouceFilePath = string.Empty;
        /// <summary>
        /// The lock for avoid recursion when read the data from list
        /// </summary>
        private static ReaderWriterLockSlim m_locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The constructor
        /// initialize some Properties
        /// </summary>
        public GlobalizationAdmin()
        {

        }
        /// <summary>
        /// use comonentName and culturesInfo to create a GlobalizationAdmin
        /// the comonentName and culturesInfo will be assemble as a comonent to be unique 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static GlobalizationAdmin Creat(string componentName, CultureInfo cultureInfo)
        {
            //when arguments are null,it will not instance a Component,so throw exception here.
            if (string.IsNullOrEmpty(componentName) || cultureInfo == null)
            {
                throw new ArgumentNullException();
            }
            //if ResourceFilePath is "",set a default value,which is current assembly path.
            if (string.IsNullOrEmpty(ResourceFilePath))
            {
                ResourceFilePath = Assembly.GetExecutingAssembly().Location;
            }
            GlobalizationAdmin GA;
            Component component = new Component(componentName, cultureInfo);
            //when read list add a locker
            m_locker.EnterReadLock();
            bool isExit = ComponentList.TryGetValue(component, out GA);
            m_locker.ExitReadLock();
            //because the ComponentList static property,so it will in the momory all the time.
            //we can get it without compute it again
            if (!isExit)
            {
                //instance a new GlobalizationAdmin avoid wrong ponit
                GA = new GlobalizationAdmin();
                GA.LoadResource(componentName, cultureInfo);
                try
                {
                    //add write locker when add a item to a list
                    //before add,check out the item whether exist,
                    //if it already exist in the list,return the exist one
                    //when use the locker do unlock in the finally,
                    //so this is why use "try"
                    m_locker.EnterWriteLock();
                    GlobalizationAdmin tryGA;
                    if (ComponentList.TryGetValue(component, out tryGA))
                    {
                        return tryGA;
                    }
                    else
                    {
                        ComponentList.Add(component, GA);
                    }
                }
                finally
                {
                    m_locker.ExitWriteLock();
                }
            }
            return GA;
        }
        /// <summary>
        /// Get globalization info by the key.
        /// when can not find value return key.
        /// </summary>
        /// <param name="key">the key of resource</param>
        /// <returns></returns>
        public string GetResource(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            else
            {
                string value = string.Empty;
                //it's a great way to use List.TryGetValue method for get value
                if (ResouceList.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    //when the key does not exist in the Resource
                    //return the key for client check out if he call right way
                    return key;
                }
            }
        }
        /// <summary>
        /// Load resouce from config file 
        /// </summary>
        /// <param name="componentName">compnent name</param>
        /// <param name="cultureInfo">cultures info</param>
        private void LoadResource(string componentName, CultureInfo cultureInfo)
        {
            //use a relative path like "\Resouce\en-us\index.xaml" to store the resource file
            m_ResouceFilePath = string.Format(@"{0}\Resource\{1}\{2}.xaml", ResourceFilePath, cultureInfo.Name.ToLower(), componentName);
            //when resource file can't find throw exception
            if (!File.Exists(m_ResouceFilePath))
            {
                throw new FileNotFoundException();
            }

            //begin read text from xaml file
            XPathDocument doc = new XPathDocument(m_ResouceFilePath);
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("//sys:String");
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nav.NameTable);
            nsManager.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            nsManager.AddNamespace("sys", "clr-namespace:System;assembly=mscorlib");
            expr.SetContext(nsManager);

            //Begin read
            XPathNodeIterator iterator = nav.Select(expr);
            string key = string.Empty;
            while (iterator.MoveNext())
            {
                key = iterator.Current.GetAttribute("Key", "http://schemas.microsoft.com/winfx/2006/xaml");
                if (!ResouceList.Keys.Contains(key))
                {
                    ResouceList.Add(key, iterator.Current.Value);
                }
            }
        }
    }
}
