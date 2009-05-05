using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Disappearwind.GlobalizationSolution.GlobalizationManager;

public partial class CulturesTest : System.Web.UI.Page
{
    public string PublishStr = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalizationAdmin.ResourceFilePath = AppDomain.CurrentDomain.BaseDirectory;
        //Test fro GlobalizationAdmin
        //China:zh-cn
        //US:en-us
        GlobalizationAdmin GA = GlobalizationAdmin.Creat("CulturesTest", new CultureInfo("en-us"));
        PublishStr = GA.GetResource("CulturesTest_Lable1");
    }
}
