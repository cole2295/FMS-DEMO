using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util.ControlHelper;
using System.Data;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.MODEL;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class MerchantDeliverFee : System.Web.UI.MasterPage
	{
        IMerchantDeliverFee merchantDeliverFee = ServiceLocator.GetService<IMerchantDeliverFee>();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                BindAreaType();
                BindAuditType();
                UCWareHouseCheckBox.DistributionCode = DistributionCode;
				InitForm();
                UCGoodsCategoryCheckBox.MerchantID = -1;
                UCGoodsCategoryCheckBox.DistributionCode = DistributionCode;
                UCGoodsCategoryCheckBox.LoadData();
			}
            UCPager.PagerPageChanged += new EventHandler(pager_PageChanged);
		}

        protected void pager_PageChanged(object sender,EventArgs e)
        {
            BindGridView(UCPager.CurrentPageIndex);
        }

		private void InitForm()
		{
			if (MerchantID == "")
				return;

            UCMerchantSourceTV.SelectMerchantID = MerchantID.ToString();
			BindGridView(1);
		}

        public void BindAreaType()
        {
            IStatusCodeService service = ServiceLocator.GetService <IStatusCodeService>();
            service.BindDropDownListByCodeType(ddlAreaType, "所有", "", "AreaType", DistributionCode);
        }

        public void BindAuditType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(ddlAuditStatus, "所有", "", "AreaTypeAudit", DistributionCode);
        }

		public string MerchantID
		{
			get { return string.IsNullOrEmpty(Request.QueryString["mid"]) ? "" : Request.QueryString["mid"]; }
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			BindGridView(1);
		}

		public void BindGridView(int n)
		{
            string merchantId = string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID) ? MerchantID : UCMerchantSourceTV.SelectMerchantID;
            string expressCompanyId = UCExpressCompanyTV.SelectExpressID == "UCSelectStationCommon" ? "" : UCExpressCompanyTV.SelectExpressID;

            string areaType = ddlAreaType.SelectedValue;
            string sortCenterId = UCWareHouseCheckBox.SelectWareHouseIds;
            string auditStatus = ddlAuditStatus.SelectedValue;
            string distributionCode = DistributionCode;
            bool isWait = cbWaitEffect.Checked;
            string categoryCode = UCGoodsCategoryCheckBox.SelectCategoryID;

            PageInfo pi = new PageInfo(UCPager.PageSize);
            pi.CurrentPageIndex = n;
            DataTable dt = merchantDeliverFee.GetDeliverFeeList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, isWait, categoryCode, ref pi);
            UCPager.RecordCount = pi.ItemCount;

            gvList.Columns[1].Visible = cbWaitEffect.Checked;
            if (OperatorArea.Controls.Count>1)
                gvList.Columns[18].Visible = false;
            else
                gvList.Columns[18].Visible = true;

            JudgeConrrolExists("btnAddFee", !cbWaitEffect.Checked);
            JudgeConrrolExists("btnEditFee", !cbWaitEffect.Checked);
            JudgeConrrolExists("btnAddFeeWait", !cbWaitEffect.Checked);
            JudgeConrrolExists("btnUpdateFeeWait", cbWaitEffect.Checked);
            JudgeConrrolExists("btnDelFee", !cbWaitEffect.Checked);
            JudgeConrrolExists("btDownTemplet", !cbWaitEffect.Checked);
            JudgeConrrolExists("btImport", !cbWaitEffect.Checked);

            JudgeConrrolExists("btnAuditFee", !cbWaitEffect.Checked);
            JudgeConrrolExists("btnResetFee", !cbWaitEffect.Checked);
            JudgeConrrolExists("btnAuditFeeWait", cbWaitEffect.Checked);
            JudgeConrrolExists("btnResetFeeWait", cbWaitEffect.Checked);

            gvList.DataSource = dt;
			gvList.DataBind();
		}

        private void JudgeConrrolExists(string btName, bool enableFlag)
        {
            Button b = OperatorArea.FindControl(btName) as Button;
            if (b != null)
                b.Enabled = enableFlag;
        }

		public IList<KeyValuePair<string, string>> GridViewCheckList
		{
			get { return GridViewHelper.GetSelectedRows<string>(gvList, "cbCheck", 15); }
		}

        private string _distributionCode;
        public string DistributionCode
        {
            get
            {
                return _distributionCode;
            }
            set
            {
                _distributionCode = value;
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string merchantId = string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID) ? MerchantID : UCMerchantSourceTV.SelectMerchantID;
            string expressCompanyId = UCExpressCompanyTV.SelectExpressID == "UCSelectStationCommon" ? "" : UCExpressCompanyTV.SelectExpressID;

            string areaType = ddlAreaType.SelectedValue;
            string sortCenterId = UCWareHouseCheckBox.SelectWareHouseIds;
            string auditStatus = ddlAuditStatus.SelectedValue;
            string distributionCode = DistributionCode;
            bool isWait = cbWaitEffect.Checked;
            string categoryCode = UCGoodsCategoryCheckBox.SelectCategoryID;

 

            try
            {

                DataTable dt = merchantDeliverFee.GetExportDeliverFeeList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, isWait, categoryCode);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    return;
                }
                CSVExport.ExportFileToClient(dt, "商家配送价格导出", false, download_token_name.Value, download_token_value.Value);
            }
            catch (Exception ex)
            {
             
            }
           
        }
	}
}
