using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class Account : System.Web.UI.MasterPage
	{
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                BindMerchantSource();
                BindAccountType();
				txtBeginTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")+" 00:00:00";
				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd")+" 23:59:59";
			}
		}

		protected void btSearch_Click(object sender, EventArgs e)
		{
			DateTime de = DateTime.Parse(txtEndTime.Text.Trim());
			DateTime ds = DateTime.Parse(txtBeginTime.Text.Trim());
			TimeSpan ts = de - ds;
			if (ts.TotalDays <= 0)
			{
				lbMsg.Visible = true;
				lbMsg.Text = "结束时间不能小于开始时间";
				return;
			}
			if (ts.TotalDays > 31)
			{
				lbMsg.Visible = true;
				lbMsg.Text = "时间范围不能大于31天";
				return;
			}
			lbMsg.Visible = false;
			BindingGridView();
		}

		public void BindingGridView()
		{
            DataTable dt = incomeAccountService.GetAccountList(ddlAccountStatus.SelectedValue, ddlMerchant.SelectedValue,
																	txtBeginTime.Text.Trim(), txtEndTime.Text.Trim(), tbAccountNO.Text.Trim());
			gvList.DataSource = dt;
			gvList.DataBind();
			if (gvList.Rows.Count <= 0)
			{
				noData.Visible = true;
				return;
			}

			noData.Visible = false;
		}

		public IList<KeyValuePair<string,string>> gvListCheckList
		{
			get { return GridViewHelper.GetSelectedRows<string>(gvList, "cbCheck", 3); }
		}

        public void BindMerchantSource()
        {
            var service = ServiceLocator.GetService<IMerchantService>();
            DataTable data = service.GetMerchants(DistributionCode);
            ddlMerchant.BindListData(data, "MerchantName", "ID", "所有", "-1");
        }

        public void BindAccountType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(ddlAccountStatus, "所有", "-1", "CODAccount", DistributionCode);
        }

        private string _distributionCode;
        /// <summary>
        /// 配送商编码
        /// </summary>
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
	}
}
