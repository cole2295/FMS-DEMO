using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;
using RFD.FMS.WEBLOGIC;
using System.Data;
using RFD.FMS.Util.ControlHelper;
using System.Drawing;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.COD
{
	public partial class CODDetailView : BasePage
	{
        ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			pager.PagerPageChanged += new EventHandler(pager_PageChanged);
			if (!IsPostBack)
				LoadSearchCondition();
		}

		private string AccountNO
		{
			get { return string.IsNullOrEmpty(Request.QueryString["accountNo"]) ? null : Request.QueryString["accountNo"].ToString(); }
		}

		private CODSearchCondition SearchCondition
		{
			get { return ViewState["CODSearchCondition"] == null ? null : ViewState["CODSearchCondition"] as CODSearchCondition; }
			set { ViewState.Add("CODSearchCondition", value); }
		}

		private string SearchType
		{
			get { return ViewState["SearchType"] == null ? null : ViewState["SearchType"].ToString(); }
			set { ViewState["SearchType"] = value; }
		}

		private void LoadSearchCondition()
		{
			if (string.IsNullOrEmpty(AccountNO))
			{
				RunJS("alert('获取结算单号失败');fnCloseNewPage('" + MenuLibrary.AccountDetailMenu("").MenuID + "')");
			}

			SearchCondition = cODAccountService.GetSearchConditionByNo(AccountNO, true);
			if (SearchCondition == null)
			{
				RunJS("alert('获取查询条件失败');fnCloseNewPage('" + MenuLibrary.AccountDetailMenu("").MenuID + "')");
			}
		}

		protected void btSearch_D_1_Click(object sender, EventArgs e)
		{
            MarkShowEventButton(sender as Button);
            SearchType = "D";
			SearchData(false);
		}

		protected void btSearch_D_2_Click(object sender, EventArgs e)
		{
            MarkShowEventButton(sender as Button);
            SearchType = "DV";
			SearchData(false);
		}

		protected void btSearch_R_1_Click(object sender, EventArgs e)
		{
            MarkShowEventButton(sender as Button);
            SearchType = "R";
			SearchData(true);
		}

		protected void btSearch_R_2_Click(object sender, EventArgs e)
		{
            MarkShowEventButton(sender as Button);
            SearchType = "RV";
			SearchData(true);
		}

		protected void btSearch_V_Click(object sender, EventArgs e)
		{
            MarkShowEventButton(sender as Button);
            SearchType = "V";
			SearchData(true);
		}

		private void SearchData(bool flag)
		{
			gvList.Columns[5].Visible = flag;
			gvList.Columns[6].Visible = flag;
			SearchData(1);
		}

		private void SearchData(int currentPageIndex)
		{
			gvList.DataSource = null;
			gvList.DataBind();
			PageInfo pi = new PageInfo(100);
			pager.PageSize = 100;
			pi.CurrentPageIndex = currentPageIndex;

			DataTable dt = cODAccountService.SearchDetail(SearchType, SearchCondition, ref pi);

			if (dt == null || dt.Rows.Count <= 0)
			{
				noData.Visible = true;
				noData.Style.Add("text-align", "center");
				noData.Text = "查询无数据";
				return;
			}
			noData.Visible = false;
			pager.RecordCount = pi.ItemCount;
			gvList.DataSource = dt;
			gvList.DataBind();
		}

		protected void pager_PageChanged(object sender, EventArgs e)
		{
			SearchData(pager.CurrentPageIndex);
		}

		protected void btExport_D_1_Click(object sender, EventArgs e)
		{
            ExportData("D", sender);
            MarkShowEventButton(sender as Button);
		}

		protected void btExport_D_2_Click(object sender, EventArgs e)
		{
            ExportData("DV", sender);
            MarkShowEventButton(sender as Button);
		}

		protected void btExport_R_1_Click(object sender, EventArgs e)
		{
            ExportData("R", sender);
            MarkShowEventButton(sender as Button);
		}

		protected void btExport_R_2_Click(object sender, EventArgs e)
		{
            ExportData("RV", sender);
            MarkShowEventButton(sender as Button);
		}

		protected void btExport_V_Click(object sender, EventArgs e)
		{
			ExportData("V", sender);
            MarkShowEventButton(sender as Button);
		}

		private void ExportData(string exportType, object sender)
		{
			try
			{
				DataTable dt = cODAccountService.SearchExportDetail(exportType, SearchCondition);
				if (dt == null || dt.Rows.Count <= 0)
				{
					Alert("未找到导出数据");
					return;
				}

				List<string> l = new List<string>();
				foreach (DataColumn column in dt.Columns)
				{
					l.Add(column.ColumnName);
				}
				Button b = sender as Button;
				ExportExcel(dt, l.ToArray(), AccountNO + b.Text.Substring(2) + "明细");
			}
			catch (Exception ex)
			{
				Alert("导出失败");
			}
		}

        private void MarkShowEventButton(Button b)
        {
            string[] buttonNames = { "btSearch_D_1", "btSearch_D_2", "btSearch_R_1", "btSearch_R_2", "btSearch_V", "btExport_D_1", "btExport_D_2", "btExport_R_1", "btExport_R_2", "btExport_V" };
            foreach (string s in buttonNames)
            {
                if (s == b.ID)
                {
                    b.ForeColor = Color.Red;
                }
                else
                {
                    Button bt = pOperator.FindControl(s) as Button;
                    bt.ForeColor = Color.White;
                }
            }
        }
	}
}
