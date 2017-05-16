using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace efwplusWebsite
{
    /// <summary>
    /// testhandler 的摘要说明
    /// </summary>
    public class testhandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            //测试微信群发
            if (context.Request.QueryString["type"] == "wxsendall")
            {
                wxhandler.SendAll();
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}