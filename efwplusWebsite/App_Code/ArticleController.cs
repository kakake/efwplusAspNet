using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace efwplusWebsite.App_Code
{
    public class ArticleController: BaseController
    {

        public object GetArticle()
        {
            int ID = Convert.ToInt32(Request.QueryString["articleID"]);
            string strsql = @"SELECT ID articleID
                                ,title articleTitle
                                ,intro articleIntro
                                ,linkurl articleLinkUrl
                                ,createdate
                                ,author articleAuthor
                                ,classid articleClassID
                                ,labelcontext articleLabelTag
                                ,toplevel articleTopLevel
                                ,isshow articleIsShow
                                 FROM ews_articleList
                                WHERE ID={0}";
            strsql = string.Format(strsql, ID);
            return DbHelper.GetDataTable(strsql);
        }

        public object SaveArticle()
        {
            string strsql = "";
            if (Request.Form["articleID"].Trim() == ""
                || Request.Form["articleID"].Trim() == "0")//新增
            {
                strsql = "INSERT INTO ews_articleList VALUES('{0}','{1}','{2}',GETDATE(),'{3}',{4},'{5}',{6},{7})";
                strsql = string.Format(strsql, Request.Form["articleTitle"]
                    , Request.Form["articleIntro"]
                    , Request.Form["articleLinkUrl"]
                    , Request.Form["articleAuthor"]
                    , Request.Form["articleClassID"]
                    , Request.Form["articleLabelTag"]
                    , Request.Form["articleTopLevel"]
                    , Request.Form["articleIsShow"]);

                return DbHelper.InsertRecord(strsql);
            }
            else//更新
            {
                strsql = @"UPDATE ews_articleList SET Title='{0}',intro='{1}',linkurl='{2}',createdate=GETDATE(),author='{3}',classid={4},labelcontext='{5}',toplevel={6},isshow={7}
                            WHERE ID={8}";
                strsql = string.Format(strsql, Request.Form["articleTitle"]
                    , Request.Form["articleIntro"]
                    , Request.Form["articleLinkUrl"]
                    , Request.Form["articleAuthor"]
                    , Request.Form["articleClassID"]
                    , Request.Form["articleLabelTag"]
                    , Request.Form["articleTopLevel"]
                    , Request.Form["articleIsShow"]
                    , Request.Form["articleID"]);

                DbHelper.DoCommand(strsql);
                return Convert.ToInt32(Request.Form["articleID"]);
            }
        }

        public object ShowArticle()
        {
            string classID = Request.QueryString["classID"];
            string searchKey = Request.QueryString["searchKey"];

            ArticleViewModel view = new ArticleViewModel();

            string strsql = @"SELECT ID,name
                                ,(SELECT COUNT(*) FROM ews_ArticleList a WHERE a.classID=ews_Class.ID) num
                                FROM ews_Class WHERE type=1 order by sort";
            DataTable dtAC = DbHelper.GetDataTable(strsql);

            strsql = @"SELECT top 50 a.*,b.NAME classname FROM ews_ArticleList a
                        LEFT JOIN ews_class b ON a.classid=b.id AND b.TYPE=1";
            if (string.IsNullOrEmpty(classID) && string.IsNullOrEmpty(searchKey))
            {
                strsql += @" WHERE isshow=1 order by toplevel DESC, createdate desc";
            }
            else if (string.IsNullOrEmpty(classID) == false)
            {
                strsql += @" WHERE isshow=1 and classid=" + classID + " order by toplevel DESC, createdate desc";
            }
            else if (string.IsNullOrEmpty(searchKey) == false)
            {
                strsql += @" WHERE isshow=1 and (title like '%" + searchKey + "%' or intro like '%" + searchKey + "%') order by toplevel DESC, createdate desc";
            }
            DataTable dtAL = DbHelper.GetDataTable(strsql);

            //获取总数量
            strsql = @"SELECT COUNT(*) FROM ews_ArticleList WHERE isshow=1";
            view.totalNum = Convert.ToInt32(DbHelper.GetDataResult(strsql));

            view.ArticleClass = new List<ClassModel>();
            for (int i = 0; i < dtAC.Rows.Count; i++)
            {
                ClassModel cm = new ClassModel();
                cm.ID = Convert.ToInt32(dtAC.Rows[i]["ID"]);
                cm.name = dtAC.Rows[i]["name"].ToString() + "(" + dtAC.Rows[i]["num"].ToString() + ")";
                view.ArticleClass.Add(cm);
            }

            view.ArticleList = new List<ArticleModel>();
            for (int i = 0; i < dtAL.Rows.Count; i++)
            {
                ArticleModel pm = new ArticleModel();
                pm.ID = dtAL.Rows[i]["ID"].ToString();
                pm.Title = dtAL.Rows[i]["title"].ToString();
                pm.Intro = dtAL.Rows[i]["intro"].ToString();
                pm.linkurl = dtAL.Rows[i]["linkurl"].ToString();
                pm.className = dtAL.Rows[i]["classname"].ToString();
                pm.author = dtAL.Rows[i]["author"].ToString();
                pm.createdate = dtAL.Rows[i]["createdate"].ToString();
                view.ArticleList.Add(pm);
            }

            return view;
        }

        public object GetWxText()
        {
            string wxarticleID = Request.QueryString["wxarticleID"];
            string strsql = @"SELECT a.ID,a.title,a.linkurl FROM ews_ArticleList a
                                WHERE isshow=1 AND ID in ({0}) order by createdate";
            strsql = string.Format(strsql, wxarticleID);
            DataTable dtAL = DbHelper.GetDataTable(strsql);

            StringBuilder context = new StringBuilder();
            context.Append("最新文章：");
            context.Append(";");
            for(int i=0;i<dtAL.Rows.Count;i++)
            {
                context.Append("• " + dtAL.Rows[i]["title"].ToString());
                context.Append(";");
                context.Append(dtAL.Rows[i]["linkurl"].ToString());
                context.Append(";");
            }
            context.Append(";");
            context.Append("efwplus官网:http://www.efwplus.cn/index.html");

            return context.ToString();
        }
    }

    public class ArticleViewModel
    {
        public int totalNum { get; set; }
        public List<ClassModel> ArticleClass { get; set; }
        public List<ArticleModel> ArticleList { get; set; }
    }

    public class ArticleModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Intro { get; set; }
        public string linkurl { get; set; }
        public string className { get; set; }
        public string author { get; set; }
        public string createdate { get; set; }
    }
}