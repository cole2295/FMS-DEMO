using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AccountDetailView : BasePage
	{
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (string.IsNullOrEmpty(AccountNO))
				{
					RunJS("alert('读取数据错误，结算状态可能已被更改，或被删除');winodw.close();");
					return;
				}
                List<IncomeAccountDetail> detail = incomeAccountService.GetAccountDetails(AccountNO);
				gvList.DataSource = detail;
				gvList.DataBind();
                IncomeSearchCondition condition = incomeAccountService.GetAccountSearchCondition(AccountNO);
				lbAccountNO.Text = condition.AccountNO;
				lbDateStr.Text = condition.DateStr.ToString("yyyy-MM-dd");
				lbDateEnd.Text = condition.DateEnd.ToString("yyyy-MM-dd");
				lbMerchant.Text = condition.MerchantName.ToString();
			}
		}

		public string AccountNO
		{
			get
			{
				return string.IsNullOrEmpty(Request.QueryString["accountNo"]) ? null :
						Request.QueryString["accountNo"].ToString();
			}
		}

		protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				int n = int.Parse(gvList.DataKeys[e.Row.RowIndex].Values[3].ToString());
				switch (n)
				{
					case 1:
						e.Row.BackColor = ColorTranslator.FromHtml("#CCCCCC");
						break;
					case 2:
						e.Row.BackColor = ColorTranslator.FromHtml("#A7DEF5");
						e.Row.ForeColor = Color.Red;
						break;
					default:
						e.Row.ForeColor = Color.Black;
						e.Row.BackColor = Color.White;
						break;
				}
			}
		}
	}
}
