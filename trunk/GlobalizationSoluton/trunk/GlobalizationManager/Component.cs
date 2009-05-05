/****************************************************************************
 * Copyright (C) Disappearwind. All rights reserved.                        *
 *                                                                          *
 * Author: disapearwind                                                     *
 * Created: 2009-4-27                                                       *
 *                                                                          *
 * Description:                                                             *
 *  The Componet contain two properties.                                    *
 *  when use componentName sign the unique component,                       *
 *  it will be destroied by different cultureInfo                           *
 *                                                                          *
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Disappearwind.GlobalizationSolution.GlobalizationManager
{
    /// <summary>
    /// The Componet contain two properties.
    /// when use componentName sign the unique component,it will be destroied by different cultureInfo
    /// </summary>
    public class Component
    {
        public string ComponentName { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public Component(string componentName, CultureInfo cultureInfo)
        {
            ComponentName = componentName;
            CultureInfo = cultureInfo;
        }
    }
}
