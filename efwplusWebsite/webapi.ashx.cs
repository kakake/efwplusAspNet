using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using efwplusWebsite.App_Code;
using miniCoreFrame.DbProvider;

namespace efwplusWebsite
{
    /// <summary>
    /// webapi 的摘要说明
    /// </summary>
    public class webapi : IHttpHandler
    {
        public static List<Type> ControllerTypes;

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");

            try
            {
                string controller = context.Request.QueryString["controller"].ToString();
                string method = context.Request.QueryString["method"].ToString();

                //string args = String.Empty;
                //if (Request.QueryString.AllKeys.Contains("args"))
                //    args = Request.QueryString["args"].ToString();

                ControllerTypes = GetTypes();
                Type controllerObj = ControllerTypes.Find(x => x.Name.ToUpper() == controller.Trim().ToUpper());
                if (controllerObj == null)
                    throw new Exception("请求控制器名" + controller + "不存在");

                BaseController baseObj = Activator.CreateInstance(controllerObj) as BaseController;
                baseObj.DbHelper = CreateDb();//创建数据库连接
                MethodInfo minfo = controllerObj.GetMethods().ToList().Find(x => x.IsPublic == true && x.Name.ToUpper() == method.Trim().ToUpper());
                if (minfo == null)
                    throw new Exception("请求方法名" + method + "不存在");

                baseObj.Request = context.Request;
                object data = minfo.Invoke(baseObj, null);
                string Json = Newtonsoft.Json.JsonConvert.SerializeObject(data);//序列化为Json
                context.Response.Write(Json);
            }
            catch (Exception err)
            {
                context.Response.Write(err.Message);
            }
        }

        private AbstractDbHelper CreateDb()
        {
            ConnectionStringSettings aSett = System.Configuration.ConfigurationManager.ConnectionStrings["efwplusWebSite"];
            AbstractDbHelper DbHelper = miniCoreFrame.DbProvider.CreateDatabase.GetDatabase("SqlServer", aSett.ConnectionString);
            miniCoreFrame.Common.Log.Info("连接数据库成功！");
            //miniCoreFrame.Common.Log.Info(GlobalAPP.RunState); 
            return DbHelper;
        }

        private List<Type> GetTypes()
        {
            if (ControllerTypes == null)
            {
                ControllerTypes = new List<Type>();
                try
                {
                    foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        if (item.Namespace == "efwplusWebsite.App_Code")
                        {
                            ControllerTypes.Add(item);
                        }
                    }
                }
                catch { }
            }

            return ControllerTypes;
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