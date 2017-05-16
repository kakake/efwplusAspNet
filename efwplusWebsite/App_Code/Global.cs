using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace efwplusWebsite.App_Code
{
    public class GlobalAPP
    {
        public static string RunState = "Stop";
        public static void AppInitialize()
        {
            //初始化
            //File.Create("AppInitialize.txt");
            GlobalAPP.RunState = "Start";
        }
    }
}