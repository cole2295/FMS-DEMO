using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.Main;
using System.Data;
using RFD.FMS.MODEL;
using System.Drawing;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AccountBuild : BasePage
	{
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                var service = ServiceLocator.GetService<IMerchantService>();
                DataTable data = service.GetMerchants(base.DistributionCode);
                ddlMerchant.BindListData(data, "MerchantName", "ID", "所有", "-1");
				InitForm();
			}
		}

		public string AccountNO
		{
			get
			{
				return ViewState["AccountNO"] == null ?
						string.IsNullOrEmpty(Request.QueryString["accountNo"]) ? null :
						Request.QueryString["accountNo"].ToString() : ViewState["AccountNO"].ToString();
			}
			set
			{
				ViewState.Add("AccountNO", value);
				if (!string.IsNullOrEmpty(value))
				{
					lbAccountNo.Text = value;
					lbAccountMsg.Visible = true;
				}
				else
				{
					lbAccountMsg.Visible = false;
				}
			}
		}

		public List<IncomeAccountDetail> IncomeSearchResult
		{
			get { return ViewState["IncomeSearchResult"] == null ? null : ViewState["IncomeSearchResult"] as List<IncomeAccountDetail>; }
			set { ViewState.Add("IncomeSearchResult", value); }
		}

		public IncomeSearchCondition IncomeSearchCondition
		{
			get { return ViewState["IncomeSearchCondition"] == null ? null : ViewState["IncomeSearchCondition"] as IncomeSearchCondition; }
			set { ViewState.Add("IncomeSearchCondition", value); }
		}

		private void InitForm()
		{
			if (string.IsNullOrEmpty(AccountNO))
			{
                //txtBeginTime.Text = "2011-09-01";
                //txtEndTime.Text = "2011-09-30";
                txtBeginTime.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
				EnableControls(false);
			}
			else
			{
				LoadEditData();
			}
		}

		private void LoadEditData()
		{
			EnableControls(true);
			if (string.IsNullOrEmpty(AccountNO))
			{
				RunJS("alert('读取数据错误，结算状态可能已被更改，或被删除');fnCloseNewPage('" + MenuLibrary.IncomeAccountEditMenu("").MenuID + "');");
				return;
			}
            IncomeSearchResult = incomeAccountService.GetAccountDetailsNew(AccountNO);
            IncomeSearchCondition = incomeAccountService.GetAccountSearchCondition(AccountNO);
			txtBeginTime.Text = IncomeSearchCondition.DateStr.ToString("yyyy-MM-dd");
			txtEndTime.Text = IncomeSearchCondition.DateEnd.ToString("yyyy-MM-dd");
			ddlMerchant.SelectedValue = IncomeSearchCondition.MerchantID.ToString();
			BindingGridView();
		}

		private void EnableControls(bool flag)
		{
			btDeleteAccount.Enabled = flag;
			btEditAccount.Enabled = flag;
			btSubmitAccount.Enabled = flag;
			btAccountAreaSummary.Enabled = flag;
			btExprotDetail.Enabled = flag;

			txtBeginTime.Enabled = !flag;
			txtEndTime.Enabled = !flag;
			ddlMerchant.Enabled = !flag;
			btSearch.Enabled = !flag;

			btBuildAccount.Enabled = !flag;
		}

		protected void btSearch_Click(object sender, EventArgs e)
		{
			if (!JudgeInput())
				return;

			IncomeSearchCondition = new IncomeSearchCondition()
			{
				DateStr = DateTime.Parse(txtBeginTime.Text.Trim()),
				DateEnd = DateTime.Parse(txtEndTime.Text.Trim()),
				MerchantID = ddlMerchant.SelectedValue,
				CreateBy=Userid,
			};

            IncomeSearchResult = incomeAccountService.GetUniteAccount(IncomeSearchCondition);
			BindingGridView();
		}

		private bool JudgeInput()
		{
			DateTime de = DateTime.Parse(txtEndTime.Text.Trim());
			DateTime ds = DateTime.Parse(txtBeginTime.Text.Trim());
			TimeSpan ts = de - ds;
			if (ts.TotalDays <= 0)
			{
				Alert("结束时间不能小于开始时间");
				return false;
			}
            if (ts.TotalDays > 31)
            {
                Alert("时间范围不能大于31天");
                return false;
            }

			if (string.IsNullOrEmpty(ddlMerchant.SelectedValue) || ddlMerchant.SelectedValue == "-1")
			{
				Alert("商家必选");
				return false;
			}

			return true;
		}

		private void BindingGridView()
		{
			gvList.DataSource = null;
			gvList.DataBind();
			if (IncomeSearchResult == null || IncomeSearchResult.Count <= 0)
			{
				noData.Visible = true;
				return;
			}

			noData.Visible = false;
			gvList.DataSource = IncomeSearchResult;
			gvList.DataBind();
			
		}

		protected void btBuildAccount_Click(object sender, EventArgs e)
		{
            if (IncomeSearchResult == null || IncomeSearchResult.Count<=0)
		    {
		        Alert("没有要生成结算单的数据");  
                return;
		    }
			string accountNo=string.Empty;
            if (incomeAccountService.CreateIncomeAccount(IncomeSearchCondition, IncomeSearchResult, Userid, out accountNo))
			{
				AccountNO = accountNo;
				LoadEditData();
				Alert("创建成功");
			}
			else
			{
				Alert("创建失败");
			}
		}

		protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				//CheckBox cb = e.Row.FindControl("cbCheck") as CheckBox;
				int n = int.Parse(gvList.DataKeys[e.Row.RowIndex].Values[3].ToString());
				switch (n)
				{
					case 1:
						e.Row.BackColor = ColorTranslator.FromHtml("#CCCCCC");
						//cb.Enabled = true;
						break;
					case 2:
						e.Row.BackColor = ColorTranslator.FromHtml("#A7DEF5");
						e.Row.ForeColor = Color.Red;
						//cb.Enabled = false;
						break;
					default:
						e.Row.ForeColor = Color.Black;
						e.Row.BackColor = Color.White;
						//cb.Enabled = false;
						break;
				}
			}
		}

		protected void btDeleteAccount_Click(object sender, EventArgs e)
		{
            if (incomeAccountService.DeleteAccount(BuildList(), Userid))
			{
				AccountNO = "";
				EnableControls(false);
				gvList.DataSource = null;
				gvList.DataBind();

				Alert("删除成功");
			}
			else
			{
				Alert("删除失败");
			}
		}

		protected void btEditAccount_Click(object sender, EventArgs e)
		{
			try
			{
                //IList<KeyValuePair<string, string>> checkRows = new List<KeyValuePair<string, string>>();
                //foreach (GridViewRow row in gvList.Rows)
                //{
                //    if (row.RowType == DataControlRowType.DataRow)
                //    {
                //        if (((CheckBox)row.FindControl("cbCheck")).Checked)
                //        {
                //            DataKey dataKey = gvList.DataKeys[row.RowIndex];
                //            checkRows.Add(new KeyValuePair<string, string>(dataKey.Values[0].ToString(), dataKey.Values[3].ToString()));
                //        }
                //    }
                //}
                //if (checkRows.Count <= 0)
                //{
                //    Alert("没有选择需要修改分拣中心");
                //    return;
                //}
                //if (checkRows.Count > 1)
                //{
                //    Alert("只能同时修改一个仓库的费用");
                //    return;
                //}
                //int dataType = int.Parse(checkRows[0].Value);
                //if (dataType != 1)
                //{
                //    Alert("请选择灰色记录修改");
                //    return;
                //}

                
                
                string url = string.Format("'AccountFeeEdit.aspx?AccountNo={0}'", AccountNO);
				RunJS(string.Format("fnShowLayer({0});", url));

			}
			catch (Exception ex)
			{
				Alert("打开更新失败<br>" + ex.Message);
			}
		}

		protected void btSubmitAccount_Click(object sender, EventArgs e)
		{
            if (incomeAccountService.UpdateAccountStatus(BuildList(), Userid, (int)EnumAccountAudit.A2))
			{
				RunJS("alert('提交成功');fnCloseNewPage('" + MenuLibrary.IncomeAccountEditMenu("").MenuID + "');");
			}
			else
			{
				Alert("提交失败");
			}
		}

		private IList<KeyValuePair<string, string>> BuildList()
		{
			return new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>(AccountNO,""),
			};
		}

		protected void btAccountAreaSummary_Click(object sender, EventArgs e)
		{
			RunJS(MenuLibrary.IncomeAccountAreaSummaryMenu(AccountNO).JsString);
		}

		protected void btExprotDetail_Click(object sender, EventArgs e)
		{
			try
			{
				if (gvList.Rows.Count <= 0)
				{
					Alert("未找到导出数据");
					return;
				}
				List<string> l = new List<string>();
				foreach (DataControlField column in gvList.Columns)
				{
					l.Add(column.HeaderText);
				}
				ExportExcel(GridViewHelper.GridView2DataTable(gvList), l.ToArray(), AccountNO+"收入结算明细");
			}
			catch (Exception ex)
			{
				Alert("导出失败");
			}
		}
	}
}
