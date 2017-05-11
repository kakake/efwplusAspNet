using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace efwplusWebsite.App_Code
{
    /// <summary>
    /// 字典管理
    /// </summary>
    public class DictionaryController : BaseController
    {
        public object ShowAdminData()
        {
            AdminViewModel view = new AdminViewModel();
            string strsql = @"SELECT ID,name FROM ews_Class WHERE type=0";
            DataTable dtPC = DbHelper.GetDataTable(strsql);
            strsql = @"SELECT ID,name FROM ews_Class WHERE type=1";
            DataTable dtAC = DbHelper.GetDataTable(strsql);

            view.projectClassDict = new List<ClassModel>();
            for (int i = 0; i < dtPC.Rows.Count; i++)
            {
                ClassModel cm = new ClassModel();
                cm.ID = Convert.ToInt32(dtPC.Rows[i]["ID"]);
                cm.name = dtPC.Rows[i]["name"].ToString();
                view.projectClassDict.Add(cm);
            }

            view.articleClassDict = new List<ClassModel>();
            for (int i = 0; i < dtAC.Rows.Count; i++)
            {
                ClassModel cm = new ClassModel();
                cm.ID = Convert.ToInt32(dtAC.Rows[i]["ID"]);
                cm.name = dtAC.Rows[i]["name"].ToString();
                view.articleClassDict.Add(cm);
            }

            return view;
        }

        public object GetClassData()
        {
            string strsql = @"SELECT * FROM ews_Class order by type,sort";
            return DbHelper.GetDataTable(strsql);
        }

        public object GetClass()
        {
            int ID = Convert.ToInt32(Request.QueryString["classID"]);
            string strsql = @"SELECT ID classID
                                ,name className
                                ,sort classSort
                                ,type classType
                                 FROM ews_Class
                                WHERE ID={0}";
            strsql = string.Format(strsql, ID);
            return DbHelper.GetDataTable(strsql);
        }

        public object SaveClass()
        {
            string strsql = "";
            if (Request.Form["classID"].Trim() == ""
                || Request.Form["classID"].Trim() == "0")//新增
            {
                strsql = "INSERT INTO ews_Class VALUES({0},'{1}',{2})";
                strsql = string.Format(strsql, Request.Form["classType"]
                    , Request.Form["className"]
                    , Request.Form["classSort"]);

                return DbHelper.InsertRecord(strsql);
            }
            else//更新
            {
                strsql = @"UPDATE ews_Class SET type={0},name='{1}',sort={2}
                            WHERE ID={3}";
                strsql = string.Format(strsql, Request.Form["classType"]
                   , Request.Form["className"]
                   , Request.Form["classSort"]
                   , Request.Form["classID"]);

                DbHelper.DoCommand(strsql);
                return Convert.ToInt32(Request.Form["classID"]);
            }
        }
    }

    public class AdminViewModel
    {
        public List<ClassModel> projectClassDict { get; set; }
        public List<ClassModel> articleClassDict { get; set; }
    }

    public class ClassModel
    {
        public int ID { get; set; }
        public string name { get; set; }
    }
}