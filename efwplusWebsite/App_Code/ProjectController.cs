using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace efwplusWebsite.App_Code
{
    public class ProjectController : BaseController
    {
        public object Test()
        {
            return DbHelper.GetDataTable("select * from Jokul_Forum_User");
        }

        public object GetProject()
        {
            int ID = Convert.ToInt32(Request.QueryString["projectID"]);
            string strsql = @"SELECT ID projectID
                                ,NAME projectName
                                ,intro projectIntro
                                ,linkurl projectLinkUrl
                                ,createdate
                                ,author projectAuthor
                                ,classid projectClassID
                                ,labelcontext projectLabelTag
                                ,toplevel projectTopLevel
                                ,isshow projectIsShow
                                 FROM ews_ProjectList
                                WHERE ID={0}";
            strsql = string.Format(strsql, ID);
            return DbHelper.GetDataTable(strsql);
        }

        public object SaveProject()
        {
            string strsql = "";
            if (Request.Form["projectID"].Trim() == ""
                || Request.Form["projectID"].Trim() == "0")//新增
            {
                strsql = "INSERT INTO ews_ProjectList VALUES('{0}','{1}','{2}',GETDATE(),'{3}',{4},'{5}',{6},{7})";
                strsql = string.Format(strsql, Request.Form["projectName"]
                    , Request.Form["projectIntro"]
                    , Request.Form["projectLinkUrl"]
                    , Request.Form["projectAuthor"]
                    , Request.Form["projectClassID"]
                    , Request.Form["projectLabelTag"]
                    , Request.Form["projectTopLevel"]
                    , Request.Form["projectIsShow"]);

                return DbHelper.InsertRecord(strsql);
            }
            else//更新
            {
                strsql = @"UPDATE ews_ProjectList SET NAME='{0}',intro='{1}',linkurl='{2}',createdate=GETDATE(),author='{3}',classid={4},labelcontext='{5}',toplevel={6},isshow={7}
                            WHERE ID={8}";
                strsql = string.Format(strsql, Request.Form["projectName"]
                    , Request.Form["projectIntro"]
                    , Request.Form["projectLinkUrl"]
                    , Request.Form["projectAuthor"]
                    , Request.Form["projectClassID"]
                    , Request.Form["projectLabelTag"]
                    , Request.Form["projectTopLevel"]
                    , Request.Form["projectIsShow"]
                    , Request.Form["projectID"]);

                DbHelper.DoCommand(strsql);
                return Convert.ToInt32(Request.Form["projectID"]);
            }
        }

        public object ShowProject()
        {
            string classID = Request.QueryString["classID"];

            ProjectViewModel view = new ProjectViewModel();
            //获取分类
            string strsql = @"SELECT ID,name
                            ,(SELECT COUNT(*) FROM ews_ProjectList a WHERE a.classID=ews_Class.ID) num
                            FROM ews_Class WHERE type=0 order by sort";
            DataTable dtPC = DbHelper.GetDataTable(strsql);
            //获取项目列表
            strsql = @"SELECT top 50 a.*,b.NAME classname FROM ews_ProjectList a
                    LEFT JOIN ews_class b ON a.classid=b.id AND b.TYPE=0";

            if (string.IsNullOrEmpty(classID))
            {
                strsql += @" WHERE isshow=1 order by toplevel DESC, createdate desc";
            }
            else
            {
                strsql += @" WHERE isshow=1 and classid=" + classID+ " order by toplevel DESC, createdate desc";
            }
            DataTable dtPL= DbHelper.GetDataTable(strsql);
            //获取总数量
            strsql = @"SELECT COUNT(*) FROM ews_ProjectList WHERE isshow=1";
            view.totalNum = Convert.ToInt32(DbHelper.GetDataResult(strsql));

            view.projectClass = new List<ClassModel>();
            for (int i = 0; i < dtPC.Rows.Count; i++)
            {
                ClassModel cm = new ClassModel();
                cm.ID = Convert.ToInt32(dtPC.Rows[i]["ID"]);
                cm.name = dtPC.Rows[i]["name"].ToString() + "(" + dtPC.Rows[i]["num"].ToString() + ")";
                view.projectClass.Add(cm);
            }

            view.ProjectList = new List<ProjectModel>();
            for (int i = 0; i < dtPL.Rows.Count; i++)
            {
                ProjectModel pm = new ProjectModel();
                pm.ID = dtPL.Rows[i]["ID"].ToString();
                pm.Name = dtPL.Rows[i]["name"].ToString();
                pm.Intro = dtPL.Rows[i]["intro"].ToString();
                pm.linkurl = dtPL.Rows[i]["linkurl"].ToString();
                pm.className= dtPL.Rows[i]["classname"].ToString();
                pm.author = dtPL.Rows[i]["author"].ToString();
                pm.createdate = dtPL.Rows[i]["createdate"].ToString();
                view.ProjectList.Add(pm);
            }

            return view;
        }
    }

    public class ProjectViewModel
    {
        public int totalNum { get; set; }
        public List<ClassModel> projectClass { get; set; }
        public List<ProjectModel> ProjectList { get; set; }
    }

    public class ProjectModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
        public string linkurl { get; set; }

        public string className { get; set; }
        public string author { get; set; }
        public string createdate { get; set; }
    }
}