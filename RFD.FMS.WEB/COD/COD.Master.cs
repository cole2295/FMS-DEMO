using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util.ControlHelper;
using System.Data;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEB.COD
{
	public partial class COD : System.Web.UI.MasterPage
	{
        ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                BindMerchantSource();
                BindAccountType();
                txtBeginTime.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
			}
			pager.PagerPageChanged += new EventHandler(pager_PageChanged);
			BindCheckBoxListColumns(new List<string>());
		}

		public GridView gvListControl
		{
			get { return gvAccountList; }
		}

		public int gvListRowCount
		{
			get { return gvAccountList.Rows.Count; }
		}

		/// <summary>
		/// gridview 列集合
		/// </summary>
		public DataControlFieldCollection gvListColumns
		{
			get { return gvListControl.Columns; }
		}

		/// <summary>
		/// griview 数组列
		/// </summary>
		public string[] gvListColumnArray
		{
			get
			{
				List<string> l = new List<string>();
				foreach (DataControlField column in gvListColumns)
				{
					l.Add(column.HeaderText);
				}
				return l.ToArray();
			}
		}

		public DataTable gvListDataTable
		{
			get { return GridViewHelper.GridView2DataTable(gvAccountList); }
		}

		public IList<KeyValuePair<string, string>> gvListChecked
		{
			get { return GridViewHelper.GetSelectedRows<string>(gvAccountList, "cbCheck", 8); }
		}

		protected void btSearch_Click(object sender, EventArgs e)
		{
			BingGridViewList(1);
		}

		public PageColumns PageColumn
		{
			get { return ViewState["PageColumns"] == null ? null : ViewState["PageColumns"] as PageColumns; }
			set { ViewState.Add("PageColumns", value); }
		}

		public void BingGridViewList(int currentPageIndex)
		{
			if (currentPageIndex == -1)
			{
				currentPageIndex = pager.CurrentPageIndex;
			}

			gvAccountList.DataSource = null;
			gvAccountList.DataBind();
			PageInfo pi = new PageInfo(20);
			pager.PageSize = 20;
			pi.CurrentPageIndex = currentPageIndex;

			string auditStatus = ddlAccountStatus.SelectedValue == "-1" ? "" : ddlAccountStatus.SelectedValue;
            string expressCompanyId = !string.IsNullOrEmpty(hidRfdChecked.Value) ? hidRfdChecked.Value : !string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID) ? UCExpressCompanyTV.SelectExpressID : "";
			string accountDateS = string.IsNullOrEmpty(txtBeginTime.Text) ? "" : txtBeginTime.Text.Trim();
			string accountDateE = string.IsNullOrEmpty(txtEndTime.Text) ? "" : DateTime.Parse(txtEndTime.Text.Trim()).AddDays(1).ToString("yyyy-MM-dd");
			string accountNo = string.IsNullOrEmpty(tbAccountNO.Text) ? "" : tbAccountNO.Text.Trim();
            string merchantId = "";// ddlMerchant.SelectedValue == "-1" ? "" : ddlMerchant.SelectedValue;

			DataTable dt = cODAccountService.SearchAccount(auditStatus, expressCompanyId, accountDateS, accountDateE, accountNo, merchantId, ref pi, true);

			if (dt == null || dt.Rows.Count <= 0)
			{
				pColumns.Visible = false;
				noData.Visible = true;
				noData.Style.Add("text-align", "center");
				noData.Text = "查询无数据";
				return;
			}
			noData.Visible = false;
			pager.RecordCount = pi.ItemCount;
			//全部置为true 避免有个小BUG
			foreach (DataControlField d in gvAccountList.Columns)
			{
				d.Visible = true;
			}
			gvAccountList.DataSource = dt;
			gvAccountList.DataBind();
			IList<string> columns = new List<string>();
			foreach (DataColumn dcf in dt.Columns)
			{
				columns.Add(dcf.ColumnName);
			}
			BindCheckBoxListColumns(columns);
		}

		private void CreateGridViewColumns(DataColumnCollection dcc)
		{
			for (int i = gvAccountList.Columns.Count - 1; i > 0; i--)
			{
				gvAccountList.Columns.RemoveAt(i);//把除第一列以外的其他列移除  
			}  
			foreach (DataColumn dc in dcc)
			{
				BoundField bc = new BoundField();
				bc.DataField = dc.ColumnName;
				bc.HeaderText = dc.ColumnName;// dc.Caption.ToString();
				bc.ItemStyle.HorizontalAlign = HorizontalAlign.Left;//居中对齐
				gvAccountList.Columns.Add(bc);
			}
		}

		protected void pager_PageChanged(object sender, EventArgs e)
		{
			BingGridViewList(pager.CurrentPageIndex);
		}

		private void BindCheckBoxListColumns(IList<string> columns)
		{
			pColumns.Visible = true;
			if (PageColumn == null || PageColumn.ColumnsShow.Count == 0 || PageColumn.DataColumns.Count == 0)//区别首次
			{
				CreatePageColumnsModel(columns);
			}
			pColumns.Controls.Clear();//每次查询清空
			CreateCheckBoxs();
			foreach (KeyValuePair<string, bool> k in PageColumn.ColumnsShow)
			{
				GridViewColumnVisible(k.Key, k.Value);
			}
		}

		private void CreateCheckBoxs()
		{
			int i = 0;
			foreach (KeyValuePair<string, bool> k in PageColumn.ColumnsShow)
			{
				CheckBox cb = new CheckBox();
				cb.AutoPostBack = true;
				cb.ID = "cb" + i;
				cb.Text = k.Key;
				cb.Checked = k.Value;
				if (k.Key == "结算单号" || k.Key == "结算状态" || k.Key == "选择")
				{
					cb.Enabled = false;
				}
                if (k.Key == "ROW_NO")
                {
                    cb.Visible = false;
                }
				cb.CheckedChanged += cb_CheckedChanged;
				pColumns.Controls.Add(cb);
				i++;
			}
		}

		protected void cb_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)sender;
			GridViewColumnVisible(cb.Text, cb.Checked);
			PageColumns pc = new PageColumns();
			IDictionary<string, bool> columns = new Dictionary<string, bool>();
			for (int i = 0; i < pColumns.Controls.Count; i++)
			{
				CheckBox ckf = (CheckBox)pColumns.FindControl("cb" + i);
				columns.Add(new KeyValuePair<string, bool>(ckf.Text, ckf.Checked));
			}
			pc.ColumnsShow = columns;
			pc.DataColumns = PageColumn.DataColumns;
			PageColumn = pc;
			BindCheckBoxListColumns(PageColumn.DataColumns);
		}

		private void GridViewColumnVisible(string headerText, bool flag)
		{
			for (int i = 0; i < gvAccountList.Columns.Count; i++)
			{
				if (gvAccountList.Columns[i].HeaderText == headerText)
				{
					gvAccountList.Columns[i].Visible = flag;
					break;
				}
			}
		}

		private void CreatePageColumnsModel(IList<string> columnNames)
		{
			PageColumns pc = new PageColumns();
			IDictionary<string, bool> columns = new Dictionary<string, bool>();
			foreach (string s in columnNames)
			{
				columns.Add(new KeyValuePair<string, bool>(s, true));
			}
			pc.ColumnsShow = columns;
			pc.DataColumns = columnNames;
			PageColumn = pc;
		}

		public DataTable GetExportData()
		{
			string auditStatus = ddlAccountStatus.SelectedValue == "-1" ? "" : ddlAccountStatus.SelectedValue;
            string expressCompanyId = !string.IsNullOrEmpty(hidRfdChecked.Value) ? hidRfdChecked.Value : !string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID) ? UCExpressCompanyTV.SelectExpressID : "";
			string accountDateS = string.IsNullOrEmpty(txtBeginTime.Text) ? "" : txtBeginTime.Text.Trim();
			string accountDateE = string.IsNullOrEmpty(txtEndTime.Text) ? "" : txtEndTime.Text.Trim();
			string accountNo = string.IsNullOrEmpty(tbAccountNO.Text) ? "" : tbAccountNO.Text.Trim();
            string merchantId = "";// ddlMerchant.SelectedValue == "-1" ? "" : ddlMerchant.SelectedValue;
            ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();
			PageInfo pi = new PageInfo(1);
            DataTable dt = cODAccountService.SearchAccount(auditStatus, expressCompanyId, accountDateS, accountDateE, accountNo, merchantId, ref pi, false);
			return dt;
		}

		public DataTable GetPrintData()
		{
			DataTable dt = GetExportData();
			string[] removeColumns = { "序号", "创建人", "创建时间", "最后修改人", "最后修改时间", "审核人", "审核时间" };
			foreach (string s in removeColumns)
			{
				if (dt.Columns.Contains(s))
					dt.Columns.Remove(s);
			}

			return dt;
		}

		private int[] PrintColumnWidth = { 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80 };
		public int[] gvListPrintColumnWidth
		{
			get { return PrintColumnWidth; }
		}

		public string[] gvListPrintColumn
		{
			get
			{
				List<string> l = new List<string>();
				foreach (DataColumn column in GetPrintData().Columns)
				{
					l.Add(column.ColumnName);
				}
				return l.ToArray();
			}
		}

        public void BindMerchantSource()
        {
            //var service = ServiceLocator.GetService<IMerchantService>();
            //DataTable data = service.GetAllMerchants(DistributionCode);
            //ddlMerchant.BindListData(data, "MerchantName", "ID", "所有", "-1");
        }

        public void BindAccountType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(ddlAccountStatus, "所有", "-1", "CODAccount", DistributionCode);
        }

        private string _distributionCode;
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
	}
}
