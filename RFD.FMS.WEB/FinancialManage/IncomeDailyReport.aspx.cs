using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Service.AudiMgmt;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class IncomeDailyReport : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if(!IsPostBack)
            {
                txtBeginTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            UCPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
		}

        private DataTable DataSource
        {
            get { return ViewState["SearchData"] as DataTable; }
            set { ViewState["SearchData"] = value; }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GetSearchData()) return;

                if (DataSource == null || DataSource.Rows.Count <= 0)
                {
                    gv.DataSource = null;
                    gv.DataBind();
                    lblWaybillCount.Text = "0";
                    lblAccountFare.Text = "0";
                    lblProtectedFee.Text = "0";
                    lblReceiveFee.Text = "0";
                    lblPOSReceiveServiceFee.Text = "0";
                    lblSumFee.Text = "0";
                    lblAvgFee.Text = "0";
                    Alert("没有查询到数据");
                    return;
                }
                IIncomeBaseInfoService service = ServiceLocator.GetService<IIncomeBaseInfoService>();
                IDictionary<string, string> dicValue = service.GetIncomeDailyReportSum(DataSource);
                lblWaybillCount.Text = dicValue["WaybillCount"];
                lblAccountFare.Text = dicValue["AccountFare"];
                lblProtectedFee.Text = dicValue["ProtectedFee"];
                lblReceiveFee.Text = dicValue["ReceiveFee"];
                lblPOSReceiveServiceFee.Text = dicValue["POSReceiveServiceFee"];
                lblSumFee.Text = dicValue["SumFee"];
                lblAvgFee.Text = dicValue["AvgFee"];

                BindDataWithBuildPage(DataSource, UCPager, gv);
            }
            catch (Exception ex)
            {
                Alert("查询错误<br>"+ex.Message);
            }
        }

        private bool GetSearchData()
        {
            if (txtBeginTime.Text.Trim().Length == 0)
            {
                Alert("请选择开始时间!");

                return false;
            }

            if (txtEndTime.Text.Trim().Length == 0)
            {
                Alert("请选择结束时间!");

                return false;
            }

            DateTime? beginDate = DataConvert.ToDateTime(txtBeginTime.Text.Trim());
            DateTime? endDate = DataConvert.ToDateTime(txtEndTime.Text.Trim()).Value.AddDays(1);

            if ((endDate.Value - beginDate.Value).Days > 32)
            {
                Alert("最多只能统计一个月的数据，请选择统计时间区间为一个月以内，谢谢！");

                return false;
            }

            IIncomeBaseInfoService service = ServiceLocator.GetService<IIncomeBaseInfoService>();

            DataTable table = service.GetIncomeDailyReport(beginDate.ToString(), endDate.ToString(), ucMerchant.SelectMerchantID, DistributionCode);

            DataSource = table;
            return true;
        }

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            BindDataWithBuildPage(DataSource, UCPager, gv);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GetSearchData()) return;
                
                CSVExport.ExportFileToClient(DataSource, "收入日报-" + DateTime.Parse(txtBeginTime.Text.Trim()).ToString("yyyyMMdd")+"-"
                    + DateTime.Parse(txtEndTime.Text.Trim()).ToString("yyyyMMdd")
                    , false, download_token_name.Value, download_token_value.Value);
            }
            catch (Exception ex)
            {
                Alert("导出错误<br>"+ex.Message);
            }
        }

        protected void pager_PageChanged(object sender, EventArgs e)
        {
            btnSearch_Click(null,null);
        }
    }
}
