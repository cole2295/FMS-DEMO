using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.COD;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class OperrateLogViewByNo : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindOperateLog();
            }      
        }   
        public string wayBillNo
        {
            get { return string.IsNullOrEmpty(Request.QueryString["wayBillNo"]) ? "" : Request.QueryString["wayBillNo"]; } 
        }
        public int LogType
        {
            get { return string.IsNullOrEmpty(Request.QueryString["LogType"]) ? 0 : int.Parse(Request.QueryString["LogType"]); }
        }
        private  void BindOperateLog()
        {
            if (LogType == 0)
            {
                RunJS("alert('绑定日志类型错误');window.close();");
                return;
            }
            if(string.IsNullOrEmpty(wayBillNo))
            {
                RunJS("alert('绑定日志编号不可为空');window.close();");
                return;
            }
            var service = ServiceLocator.GetService<IIncomeBaseInfoService>();
            long waybillNo=0;
            try
            {
                 waybillNo = DataConvert.ToLong(wayBillNo);
            }
            catch (Exception ex)
            {

                RunJS("alert('绑定日志编号错误');window.close();");
                return;
            }
            
            DataTable dtID = service.ExsitIncomeFeeInfoByNo(waybillNo);
            string PK_NO = (dtID.Rows[0]["IncomeFeeID"].ToString());
            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
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