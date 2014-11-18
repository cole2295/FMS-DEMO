using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using System.Data;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class OperateLogView : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindOperateLog();
            }
        }

        public string PK_NO
        {
            get { return string.IsNullOrEmpty(Request.QueryString["PK_NO"]) ? "" : Request.QueryString["PK_NO"]; }
        }

        public int LogType
        {
            get { return string.IsNullOrEmpty(Request.QueryString["LogType"]) ? 0 : int.Parse(Request.QueryString["LogType"]); }
        }

        private void BindOperateLog()
        {
            if (string.IsNullOrEmpty(PK_NO))
            {
                RunJS("alert('绑定日志编号错误');window.close();");
                return;
            }

            if (LogType==0)
            {
                RunJS("alert('绑定日志类型错误');window.close();");
                return;
            }

            IAccountOperatorLogService accountOperatorLogService=ServiceLocator.GetService<IAccountOperatorLogService>();
            DataTable dt = accountOperatorLogService.GetOperatorLogLog(PK_NO, LogType);

            if (dt == null || dt.Rows.Count <= 0)
            {
                RunJS("alert('查询无操作日志');window.close();");
                return;
            }

            gvList.DataSource = dt;
            gvList.DataBind();
        }
    }
}