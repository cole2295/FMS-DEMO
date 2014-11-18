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

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class ExpressFinanceSearch : FMSBasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if(!IsPostBack)
            {
                BindPayType(ddlPayType);
                //BindExportFormat();
            }
		}

        private bool IsClickV2
        {
            get { return DataConvert.ToBoolean(ViewState["IsClickV2"]); }
            set { ViewState["IsClickV2"] = value; }
        }

        public void BindExportFormat()
        {
            var data = EnumHelper.GetEnumValueAndDescriptions<ExportFileFormat>();
            //rblExportFormat.BindListData(data, "");
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            IsClickV2 = false;
            if (!JudgeInput())
                return;
            bindGV();
            this.gvDetails.DataSource = null;
            this.gvDetails.DataBind();
        }

        private bool JudgeInput()
        {
            if (string.IsNullOrEmpty(txtBeginTime.Text) || string.IsNullOrEmpty(txtEndTime.Text))
            {
                Alert("汇总日期不能为空");
                return false;
            }

            return true;
        }

        private void bindGV()
        {
            DataTable dt=DtSource(true);

            BindSum(dt);

            gvSummary.DataSource = dt;
            gvSummary.DataBind();
        }

        private void BindSum(DataTable dt)
        {
            if(dt!=null)
            {
                int CashCount = 0;
                int POSCount = 0;
                int SumCount = 0;
                decimal TransferSum = 0;
                decimal ProtectedPriceSum = 0;
                decimal SumPrice = 0;
                int ProtectedPriceCount = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    CashCount += StringUtil.ConvertToInt(dr["CashWaybillCount"]);
                    POSCount += StringUtil.ConvertToInt(dr["POSWaybillCount"]);
                    TransferSum += StringUtil.ConvertToDecimal(dr["TransferFeeSum"]);
                    ProtectedPriceSum += StringUtil.ConvertToDecimal(dr["ProtectedPriceSum"]);
                    SumPrice += StringUtil.ConvertToDecimal(dr["SaveAmount"]);
                    if(StringUtil.ConvertToDecimal(dr["ProtectedPriceSum"])>0)
                    {
                        ProtectedPriceCount++;
                    }
                }
                SumCount = CashCount + POSCount;

                lblCashCount.Text = CashCount.ToString();
                lblPOSCount.Text = POSCount.ToString();
                lblSumCount.Text = SumCount.ToString();
                lblTransferSum.Text = TransferSum.ToString();
                lblProtectedPriceSum.Text = ProtectedPriceSum.ToString();
                lblSumPrice.Text = SumPrice.ToString();
                lblProtectedPriceCount.Text = ProtectedPriceCount.ToString();
            }
        }
        private void bindDetailGV(DateTime dateTime, int DeliverStationID)
        {
            DataTable dt = DtDetailSource(dateTime, DeliverStationID);
            gvDetails.DataSource = dt;
            gvDetails.DataBind();
        }
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        private DataTable DtSource(bool PageOrNo)
        {
            var condition = new SearchCondition();
            condition.DeliverStation = UCSelectStation.Name.IsNullData() ? 0 : UCSelectStation.ID.ConvertToInt();
            condition.StationName = UCSelectStation.Name.Trim();
            condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
            condition.BeginTime = StringUtil.FormatDateTime(txtBeginTime.Text,DateTime.Now);
            condition.EndTime =
                Convert.ToDateTime(
                    StringUtil.FormatDateTime(txtEndTime.Text, DateTime.Now).ToString("yyyy-MM-dd 23:59:59"));
            condition.TransferPayType = Convert.ToInt32(ddlTransferPayType.SelectedValue);
            condition.DistributionCode = DistributionCode;
            condition.MerchantIDs = UCMerchantSourceTV.SelectMerchantID;
            IFinanceService service = ServiceLocator.GetService<IFinanceService>();
            return service.GetTransferFinanceSumData(condition);
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        private DataTable DtDetailSource(DateTime dateTime, int DeliverStationID)
        {
            var condition = new SearchCondition();
            condition.DeliverStation = DeliverStationID;
            condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
            condition.BeginTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 00:00:00"));
            condition.EndTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 23:59:59"));
            condition.TransferPayType = Convert.ToInt32(ddlTransferPayType.SelectedValue);
            condition.MerchantIDs = UCMerchantSourceTV.SelectMerchantID;
            condition.DistributionCode = DistributionCode;

            IFinanceService service = ServiceLocator.GetService<IFinanceService>();
            return service.GetTransferFinanceDetailData(condition);
        }

        protected void gvSummary_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            //Alert("ok");
        }

        protected void gvSummary_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            HiddenField hdPara = (HiddenField) gvSummary.Rows[e.NewSelectedIndex].Cells[8].FindControl("hidQueryParams");
            Label lblCreateTime = (Label)gvSummary.Rows[e.NewSelectedIndex].Cells[8].FindControl("lblCreateTime");

            if (IsClickV2 == true)
            {
                bindDetailData(Convert.ToDateTime(lblCreateTime.Text), Convert.ToInt32(hdPara.Value));
            }
            else
            {
                bindDetailGV(Convert.ToDateTime(lblCreateTime.Text), Convert.ToInt32(hdPara.Value));
            }
            //Alert(hdPara.Value.ToString() + "," + lblCreateTime.Text);
        }

        protected void btnSearchDetails_Click(object sender, EventArgs e)
        {

        }

        protected void gvSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    e.Row.Attributes["style"] = "cursor:hand";
                    #region 点击行触发SelectedIndexChanged事件
                    PostBackOptions myPostBackOptions = new PostBackOptions(this);
                    myPostBackOptions.AutoPostBack = false;
                    myPostBackOptions.PerformValidation = false;
                    myPostBackOptions.RequiresJavaScriptProtocol = true; //加入javascript:头
                    String evt = Page.ClientScript.GetPostBackClientHyperlink(sender as GridView, "Select$" + e.Row.RowIndex.ToString());
                    e.Row.Attributes.Add("onclick", evt);
                    #endregion
                    break;
            }
        }

        protected void btnSummary_Click(object sender, EventArgs e)
        {
            var dt = GridViewHelper.GridView2DataTable(gvSummary);
            CSVExport.DataTable2Excel(dt, "快递收款汇总数据");
        }

        protected void btnDetails_Click(object sender, EventArgs e)
        {
            var dt = GridViewHelper.GridView2DataTable(gvDetails);
            CSVExport.DataTable2Excel(dt, "快递收款明细数据");
        }

        protected void btnAddDetails_Click(object sender, EventArgs e)
        {
            if (!JudgeInput())
                return;
            var dt = DtAllDetailSource();
            CSVExport.DataTable2Excel(dt, "快递收款所有明细数据");
        }

        /// <summary>
        /// 获取所有数据源
        /// </summary>
        /// <returns></returns>
        private DataTable DtAllDetailSource()
        {
            var condition = new SearchCondition();
            condition.DeliverStation = 0;
            condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
            condition.BeginTime = Convert.ToDateTime(Convert.ToDateTime(txtBeginTime.Text).ToString("yyyy-MM-dd 00:00:00"));
            condition.EndTime = Convert.ToDateTime(Convert.ToDateTime(txtEndTime.Text).ToString("yyyy-MM-dd 23:59:59"));
            condition.TransferPayType = Convert.ToInt32(ddlTransferPayType.SelectedValue);
            condition.MerchantIDs = UCMerchantSourceTV.SelectMerchantID;
            condition.DistributionCode = DistributionCode;
            IFinanceService service = ServiceLocator.GetService<IFinanceService>();

            return service.GetTransferFinanceDetailData(condition);
        }

        #region 通过中间表实现

        protected void btnSearchV2_Click(object sender, EventArgs e)
        {
            IsClickV2 = true;
            if (!JudgeInput())
                return;
            GetAndBind();

            this.gvDetails.DataSource = null;

            this.gvDetails.DataBind();
        }

        protected void btnSummaryV2_Click(object sender, EventArgs e)
        {
            var dt = GridViewHelper.GridView2DataTable(gvSummary);

            CSVExport.DataTable2Excel(dt, "快递收款汇总数据");
        }

        protected void btnDetailsV2_Click(object sender, EventArgs e)
        {
            var dt = GridViewHelper.GridView2DataTable(gvDetails);

            CSVExport.DataTable2Excel(dt, "快递收款明细数据");
        }

        protected void btnAddDetailsV2_Click(object sender, EventArgs e)
        {
            if (!JudgeInput())
                return;
            var dt = GetDetailSource();

            CSVExport.DataTable2Excel(dt, "快递收款所有明细数据");
        }

        private void GetAndBind()
        {
            DataTable dt = GetDataSource(true);
            BindSum(dt);
            gvSummary.DataSource = dt;
            gvSummary.DataBind();
        }

        private DataTable GetDataSource(bool PageOrNo)
        {
            var condition = new SearchCondition();
            condition.DeliverStation = UCSelectStation.Name.IsNullData() ? 0 : UCSelectStation.ID.ConvertToInt();
            condition.StationName = UCSelectStation.Name.Trim();
            condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
            condition.BeginTime = StringUtil.FormatDateTime(txtBeginTime.Text, DateTime.Now);
            condition.EndTime =
                Convert.ToDateTime(
                    StringUtil.FormatDateTime(txtEndTime.Text, DateTime.Now).ToString("yyyy-MM-dd 23:59:59"));
            condition.TransferPayType = Convert.ToInt32(ddlTransferPayType.SelectedValue);
            condition.DistributionCode = DistributionCode;
            condition.MerchantIDs = UCMerchantSourceTV.SelectMerchantID;
            IFinanceService service = ServiceLocator.GetService<IFinanceService>();

            return service.GetTransferFinanceSumDataV2(condition);
        }

        private DataTable GetDetailSource()
        {
            var condition = new SearchCondition();
            condition.DeliverStation = 0;
            condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
            condition.BeginTime = Convert.ToDateTime(Convert.ToDateTime(txtBeginTime.Text).ToString("yyyy-MM-dd 00:00:00"));
            condition.EndTime = Convert.ToDateTime(Convert.ToDateTime(txtEndTime.Text).ToString("yyyy-MM-dd 23:59:59"));
            condition.TransferPayType = Convert.ToInt32(ddlTransferPayType.SelectedValue);
            condition.MerchantIDs = UCMerchantSourceTV.SelectMerchantID;
            condition.DistributionCode = DistributionCode;

            IFinanceService service = ServiceLocator.GetService<IFinanceService>();

            return service.GetTransferFinanceDetailDataV2(condition);
        }

        private void bindDetailData(DateTime dateTime, int DeliverStationID)
        {
            DataTable dt = GetDetailSource(dateTime, DeliverStationID);
            gvDetails.DataSource = dt;
            gvDetails.DataBind();
        }

        private DataTable GetDetailSource(DateTime dateTime, int DeliverStationID)
        {
            var condition = new SearchCondition();
            condition.DeliverStation = DeliverStationID;
            condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
            condition.BeginTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 00:00:00"));
            condition.EndTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 23:59:59"));
            condition.TransferPayType = Convert.ToInt32(ddlTransferPayType.SelectedValue);
            condition.DistributionCode = DistributionCode;

            IFinanceService service = ServiceLocator.GetService<IFinanceService>();

            return service.GetTransferFinanceDetailDataV2(condition);
        }

        #endregion
    }
}
