using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.Main
{
    public partial class Menu : BasePage
    {
        protected string JSList = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BuidMenu();
            }
        }

        /// <summary>
        /// 可变部分
        /// </summary>
        private void BuidMenu()
        {
            //实例化业务层
            var menu = ServiceLocator.GetService<IRoleservice>();
            DataTable dataTable = menu.GetMenuListByUserID(Userid.ToString()).Tables[0];
            //DataRow[] dataRows = dataTable.Select(" MenuLevel=0");
            DataRow[] dataRows = dataTable.Select(" MenuLevel=0 and systemId=" + SystemType);
            var id = default(int);
            for (var i = 0; i < dataRows.Length; i++)
            {
                var dataRow = dataRows[i];
                LoadMenuTitle(dataRow, i);
                DataRow[] rows = dataTable.Select(" MenuLevel=1 and MenuGroup=" + dataRow["MenuGroup"]);
                LoadMenuItem(i, rows, ref id);
                LoadMenuFoot();
            }
        }
        private void LoadMenuItem(int i, IEnumerable<DataRow> rows, ref int id)
        {
            foreach (var r in rows)
            {
                string sign = "&";
                if (r["URL"].ToString().IndexOf("?") == -1)
                {
                    sign = "?";
                }
                var url = r["URL"].ToString().Replace("..", "") + sign + "sysname=" + HttpUtility.UrlEncode(r["SystemName"].ToString()) + "&menuname=" + HttpUtility.UrlEncode(r["MenuName"].ToString());
                JSList += string.Format(@"
                <TreeNode Name='id_{2}' Text='{0}' value='{1}' Image='/ScriptsNew/Images/16/mark.gif' href='javascript:void(0)' Target='main'></TreeNode>"
                    , r["MenuName"], url, id++);

//                JSList += string.Format(@"
//                <TreeNode Name='id_{2}' Text='{0}' value='{1}' Image='/ScriptsNew/Images/16/mark.gif' href='javascript:void(0)' Target='main'></TreeNode>"
//                    , r["MenuName"], GetURL(r), id++);
            }
        }

        private string GetURL(DataRow row)
        {
            string url = row["URL"].ToString();

            if (url.IndexOf("@LMS") != -1)
            {
                url = url.Replace("@LMS", "http://lms.wuliusys.com");
            }
            else
            { 
                url = url.Replace("..", "");
            }

            return url;
        }

        private void LoadMenuTitle(DataRow dataRow, int i)
        {
            JSList += string.Format(@"
            	<TreeNode id='ParentNode{0}' Text='{1}' Image='/ScriptsNew/Images/vista/gif/folder.gif'>",
                i, dataRow["MenuName"]);
        }

        private void LoadMenuFoot()
        {
            JSList += "</TreeNode>";
        }

        public string SystemType
        {
            get
            {
                var st = ConfigurationManager.AppSettings["SystemType"];
                return !st.IsNullData() && st == "LMS_RFD_FMS_FLOW" ?
                    ((int)(RFD.FMS.MODEL.BizEnums.SystemType.LMS_RFD_FMS_FLOW)).ToString() :
                    ((int)(RFD.FMS.MODEL.BizEnums.SystemType.LMS_RFD_FMS)).ToString();
            }
        }
    }
}

