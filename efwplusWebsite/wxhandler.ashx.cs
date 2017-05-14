using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using miniCoreFrame.DbProvider;
using Tencent;

namespace efwplusWebsite
{
    /// <summary>
    /// wxhandler 的摘要说明
    /// </summary>
    public class wxhandler : IHttpHandler
    {
        string token = "kakake";//从配置文件获取Token
        string appId = "wx95ff01dd30aac611";//从配置文件获取appId
        string appSecret = "f6e05ef0b365d8db7c889d345c7aee34";//
        string encodingAESKey = "Pl3eZn0SYdQXbFoRSXbQd48tMzac2MzR1DSmzXW85lm";//从配置文件获取EncodingAESKey

        public void ProcessRequest(HttpContext context)
        {
            string postString = string.Empty;
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "GET")
            {
                Auth();
            }
            else if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                MsgHandle();
            }
        }

        /// <summary>
        /// 成为开发者的第一步，验证并相应服务器的数据
        /// </summary>
        private void Auth()
        {
            try
            {
                string signature = HttpContext.Current.Request.QueryString["signature"];
                string timestamp = HttpContext.Current.Request.QueryString["timestamp"];
                string nonce = HttpContext.Current.Request.QueryString["nonce"];
                string echostr = HttpContext.Current.Request.QueryString["echostr"];

                //get method - 仅在微信后台填写URL验证时触发
                if (CheckSignature(signature, timestamp, nonce, token))
                {
                    WriteContent(echostr); //返回随机字符串则表示验证通过
                }
                else
                {
                    WriteContent("failed:" + signature + "," + GetSignature(timestamp, nonce, token) + "。" +
                                "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                }
            }
            catch (Exception e)
            {
                WriteContent(e.Message);
            }
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 消息处理
        /// </summary>
        private void MsgHandle()
        {
            try
            {
                //miniCoreFrame.Common.Log.Info("MsgHandle");

                string signature = HttpContext.Current.Request.QueryString["signature"];
                string timestamp = HttpContext.Current.Request.QueryString["timestamp"];
                string nonce = HttpContext.Current.Request.QueryString["nonce"];
                //miniCoreFrame.Common.Log.Info(HttpContext.Current.Request.QueryString);

                if (CheckSignature(signature, timestamp, nonce, token))//验证通过处理消息
                {
                    using (Stream stream = HttpContext.Current.Request.InputStream)
                    {
                        Byte[] postBytes = new Byte[stream.Length];
                        stream.Read(postBytes, 0, (Int32)stream.Length);
                        string postString = Encoding.UTF8.GetString(postBytes);
                        string MsgType = ParserPostXML(postString, "MsgType");
                        if (MsgType == "text")
                        {
                            TextHandle(postString);//回复文本
                        }
                        else if (MsgType == "event")
                        {
                            EventHandle(postString);//事件处理
                        }
                        else
                        {
                            string tpl = ContextTemplate(contextType.其他, null);
                            string text = BuildOutText(postString, tpl);
                            WriteContent(text);
                        }
                    }
                }
                else
                {
                    WriteContent("failed:" + signature + "," + GetSignature(timestamp, nonce, token) + "。" +
                                "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                }
            }
            catch (Exception e)
            {
                WriteContent(e.Message);
            }

            HttpContext.Current.Response.End();
        }
        //文本处理
        private void TextHandle(string postString)
        {
            string searchKey = ParserPostXML(postString, "Content");
            AbstractDbHelper DbHelper = CreateDb();
            string strsql = @"SELECT top 20 a.title,a.linkurl FROM ews_ArticleList a";
            strsql += @" WHERE isshow=1 and (title like '%" + searchKey + "%' or intro like '%" + searchKey + "%') order by toplevel DESC, createdate desc";
            DataTable dtAL = DbHelper.GetDataTable(strsql);
            Dictionary<string, string> articleList = new Dictionary<string, string>();
            for (int i = 0; i < dtAL.Rows.Count; i++)
            {
                articleList.Add(dtAL.Rows[i]["title"].ToString(), dtAL.Rows[i]["linkurl"].ToString());
            }
            string tpl = ContextTemplate(contextType.搜索, articleList);
            string text = BuildOutText(postString, tpl);
            WriteContent(text);
        }
        //事件处理
        private void EventHandle(string postString)
        {
            string Event = ParserPostXML(postString, "Event");
            if(Event== "subscribe")
            {
                string tpl= ContextTemplate(contextType.关注, null);
                string text= BuildOutText(postString, tpl);
                WriteContent(text);
            }
        }

        private string ParserPostXML(string postString,string nodename)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root;
            doc.LoadXml(postString);
            root = doc.FirstChild;
            return root[nodename].InnerText;
        }

        private string BuildOutText(string postString,string context)
        {
            string ToUserName = ParserPostXML(postString, "ToUserName");
            string FromUserName = ParserPostXML(postString, "FromUserName");
            string CreateTime = ParserPostXML(postString, "CreateTime");
            string outText = @"<xml> 
                                  <ToUserName><![CDATA[{0}]]></ToUserName>  
                                  <FromUserName><![CDATA[{1}]]></FromUserName>  
                                  <CreateTime>{2}</CreateTime>  
                                  <MsgType><![CDATA[{3}]]></MsgType>  
                                  <Content><![CDATA[{4}]]></Content> 
                                </xml>";
            outText = string.Format(outText, FromUserName, ToUserName, CreateTime, "text", context);
            return outText;
        }

        private string ContextTemplate(contextType type,Dictionary<string,string> articleList)
        {
            StringBuilder context = new StringBuilder();
            switch (type)
            {
                case contextType.关注:
                    context.AppendLine("Hello!欢迎您关注efwplus的官方微信！回复文字将自动搜索文章。");
                    break;
                case contextType.搜索:
                    context.AppendLine("文章搜索结果：");
                    if (articleList==null || articleList.Count == 0)
                        context.AppendLine("没有搜索到相关文章！");
                    else
                    {
                        foreach(var item in articleList)
                        {
                            context.AppendLine("• "+item.Key);
                            context.AppendLine(item.Value);
                        }
                    }
                    break;
                case contextType.群发:
                    context.AppendLine("最新文章：");
                    foreach (var item in articleList)
                    {
                        context.AppendLine("• " + item.Key);
                        context.AppendLine(item.Value);
                    }
                    break;
                case contextType.其他:
                    break;
            }
            context.AppendLine("efwplus官网:http://www.efwplus.cn/index.html");
            context.AppendLine("项目汇总:http://www.efwplus.cn/index.html#project");
            context.AppendLine("文章汇总:http://www.efwplus.cn/index.html#article");
            context.AppendLine("论坛:http://bbs.efwplus.cn/Default.aspx");
            return context.ToString();
        }

        private bool CheckSignature(string signature, string timestamp, string nonce, string token)
        {
            return signature == GetSignature(timestamp, nonce, token);
        }
        private string GetSignature(string timestamp, string nonce, string token)
        {
            string[] arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
            string arrString = string.Join("", arr);
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
            StringBuilder enText = new StringBuilder();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }
            return enText.ToString();
        }

        private void WriteContent(string str)
        {
            miniCoreFrame.Common.Log.Info(str);
            HttpContext.Current.Response.Output.Write(str);
        }

        private AbstractDbHelper CreateDb()
        {
            ConnectionStringSettings aSett = System.Configuration.ConfigurationManager.ConnectionStrings["efwplusWebSite"];
            AbstractDbHelper DbHelper = miniCoreFrame.DbProvider.CreateDatabase.GetDatabase("SqlServer", aSett.ConnectionString);
            miniCoreFrame.Common.Log.Info("连接数据库成功！");

            return DbHelper;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private enum contextType
        {
            关注,搜索,群发,其他
        }
    }
}