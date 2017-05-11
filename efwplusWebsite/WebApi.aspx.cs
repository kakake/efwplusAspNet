using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using efwplusWebsite.App_Code;
using miniCoreFrame.DbProvider;

namespace efwplusWebsite
{
    public partial class WebApi : System.Web.UI.Page
    {
        public static List<Type> ControllerTypes;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string controller = Request.QueryString["controller"].ToString();
                string method = Request.QueryString["method"].ToString();

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

                baseObj.Request = Request;
                object data = minfo.Invoke(baseObj, null);
                string Json = Newtonsoft.Json.JsonConvert.SerializeObject(data);//序列化为Json
                Response.Write(Json);
            }
            catch (Exception err)
            {
                Response.Write(err.Message);
            }
        }

        private AbstractDbHelper CreateDb()
        {
            ConnectionStringSettings aSett = System.Configuration.ConfigurationManager.ConnectionStrings["efwplusWebSite"];
            AbstractDbHelper  DbHelper = miniCoreFrame.DbProvider.CreateDatabase.GetDatabase("SqlServer", aSett.ConnectionString);
            miniCoreFrame.Common.Log.Info("连接数据库成功！");

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
    }
}