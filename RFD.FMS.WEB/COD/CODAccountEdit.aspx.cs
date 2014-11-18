using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL;
using RFD.FMS.WEBLOGIC;
using System.Data;
using System.Drawing;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEB.COD
{
	public partial class CODAccountEdit : BasePage
	{
        ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                var service = ServiceLocator.GetService<IMerchantService>();
                DataTable data = service.GetAllMerchants(base.DistributionCode);

				if (string.IsNullOrEmpty(AccountNO))
					InitCreate();
				else
					InitEdit(true);
			}
            ucDeliveryHouse.DistributionCode = base.DistributionCode;
            ucReturnHouse.DistributionCode = base.DistributionCode;
            ucVisitReturnHouse.DistributionCode = base.DistributionCode;
            UCPager.PagerPageChanged += new EventHandler(UCPager_PageChanged);
			BindCheckBoxListColumns(new List<string>());
		}

        protected void UCPager_PageChanged(object sender, EventArgs e)
        {
            BindDataWithBuildPage(CODSearchResult, UCPager, gvList);
        }

		protected string AccountNO
		{
			get
			{
				return ViewState["AccountNO"]==null ?
					    string.IsNullOrEmpty(Request.QueryString["accountNo"])? null :
						Request.QueryString["accountNo"].ToString() : ViewState["AccountNO"].ToString();
			}
			set
			{
				ViewState.Add("AccountNO", value);
				tbAccountNO.Text = value;
			}
		}

		public CODSearchCondition CODSearchCondition
		{
			get { return ViewState["CODSearchCondition"] == null ? null : ViewState["CODSearchCondition"] as CODSearchCondition; }
			set { ViewState.Add("CODSearchCondition", value); }
		}

		public DataTable CODSearchResult
		{
			get { return ViewState["CODSearchResult"] == null ? null : ViewState["CODSearchResult"] as DataTable; }
			set { ViewState.Add("CODSearchResult", value); }
		}

		private void InitCreate()
		{
			DateTime dt = DateTime.Now.AddMonths(-1);
			DateTime dt1 = DateTime.Now;
			DateTime dtS = new DateTime(dt.Year, dt.Month, 1);
			DateTime dtE = new DateTime(dt1.Year, dt1.Month, 1).AddDays(-1);
            tbDate_D_S.Text = dtS.ToString("yyyy-MM-dd");
            tbDate_D_E.Text = dtE.ToString("yyyy-MM-dd");
            tbDate_R_S.Text = dtS.ToString("yyyy-MM-dd");
            tbDate_R_E.Text = dtE.ToString("yyyy-MM-dd");
            tbDate_V_S.Text = dtS.ToString("yyyy-MM-dd");
            tbDate_V_E.Text = dtE.ToString("yyyy-MM-dd");
            ExportDifference.Visible = false;

			EnableControls(false);
			tbAccountNO.Text = "";
			gvList.DataSource = null;
			gvList.DataBind();
		}

		private void InitEdit(bool flag)
		{
			CODSearchCondition searchCondition;
			CODSearchResult = cODAccountService.AccountSearchByNo(AccountNO, flag, out searchCondition);
			if (CODSearchResult == null || CODSearchResult.Rows.Count <= 0 || searchCondition==null)
			{
				RunJS("alert('读取数据错误，结算状态可能已被更改，或被删除');fnCloseNewPage('" + MenuLibrary.AccountEditMenu("").MenuID + "');");
				return;
			}
			CODSearchCondition = searchCondition;
            EnableControls(true);
			GridViewBind();
			AccountNO = CODSearchCondition.AccountNO;

			if (flag)
			{
				tbDate_D_S.Text = CODSearchCondition.Date_D_S.ToString("yyyy-MM-dd");
				tbDate_D_E.Text = CODSearchCondition.Date_D_E.ToString("yyyy-MM-dd");
				tbDate_R_S.Text = CODSearchCondition.Date_R_S.ToString("yyyy-MM-dd");
				tbDate_R_E.Text = CODSearchCondition.Date_R_E.ToString("yyyy-MM-dd");
				tbDate_V_S.Text = CODSearchCondition.Date_V_S.ToString("yyyy-MM-dd");
				tbDate_V_E.Text = CODSearchCondition.Date_V_E.ToString("yyyy-MM-dd");
				rbGeneral.Checked = CODSearchCondition.AccountType == 1 ? true : false;
				rbEMS.Checked = CODSearchCondition.AccountType == 2 ? true : false;
				rbZJS.Checked = CODSearchCondition.AccountType == 3 ? true : false;
                UCExpressCompanyTV.SelectExpressID = CODSearchCondition.ExpressCompanyID;
                UCExpressCompanyTV.SelectExpressName = CODSearchCondition.CompanyName;
				ucDeliveryHouse.SelectWareHouseIds = CODSearchCondition.HouseD;
				ucReturnHouse.SelectWareHouseIds = CODSearchCondition.HouseR;
				ucVisitReturnHouse.SelectWareHouseIds = CODSearchCondition.HouseV;
                UCMerchantSourceTV.SelectMerchantID = CODSearchCondition.MerchantID.ToString();
			}
            ExportDifference.Visible = CODSearchCondition.IsDifference > 0;
		}

		private void GridViewBind()
		{
            gvList.DataSource = null;
            gvList.DataBind();
			//全部置为true 避免有个小BUG
			foreach (DataControlField d in gvList.Columns)
			{
				if(d.HeaderText!="DataType" && d.HeaderText!="AccountDetailID")
					d.Visible = true;
				else
					d.Visible = false;
			}
			
			if (CODSearchResult == null || CODSearchResult.Rows.Count <= 0)
			{
				noData.Style.Add("display", "block");
				noData.Style.Add("text-align", "center");
				noData.Text = "查询无数据";
                pColumns.Visible = false;
                UCPager.Visible = false;
			}
			else
			{
                BindDataWithBuildPage(CODSearchResult, UCPager, gvList);
				noData.Style.Add("display", "none");
                pColumns.Visible = true;
                UCPager.Visible = true;
				CreateVisibleColumns();
			}
		}

		private void EnableControls(bool flag)
		{
			ViewDetail.Disabled = !flag;
			UpdateMsg.Enabled = flag;
			SubmitAccount.Enabled = flag;
			DeleteAccountNO.Enabled = flag;
			AreaFareMsg.Disabled = !flag;

			ucDeliveryHouse.SelectEnable = !flag;
			ucReturnHouse.SelectEnable = !flag;
			ucVisitReturnHouse.SelectEnable = !flag;
			SaveAccount.Enabled = !flag;
			tbDate_D_S.Enabled = !flag;
			tbDate_D_E.Enabled = !flag;
			tbDate_R_S.Enabled = !flag;
			tbDate_R_E.Enabled = !flag;
			tbDate_V_S.Enabled = !flag;
			tbDate_V_E.Enabled = !flag;
            UCExpressCompanyTV.Editable = !flag;
			btSearch.Enabled = !flag;
			rbGeneral.Enabled = !flag;
			rbEMS.Enabled = !flag;
			rbZJS.Enabled = !flag;
            UCMerchantSourceTV.Editable = !flag;

            if (CODSearchCondition != null && CODSearchCondition.IsDifference.ToString().TryGetInt() > 0)
            {
                SubmitAccount.OnClientClick = "return window.confirm('实际结算量与创建结算量 存在差异，确定提交此结算？');";
            }
            else
            {
                SubmitAccount.OnClientClick = "return window.confirm('确定提交此结算？');";
            }
		}

		protected void btSearch_Click(object sender, EventArgs e)
		{
            try
            {
                if (!SearchData()) return;

                GridViewBind();
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
		}

        private bool SearchData()
        {
            if (!JudgeInput())
                return false;

            CODSearchCondition csc = new CODSearchCondition();
            csc.HouseD = ucDeliveryHouse.SelectWareHouseIds;
            csc.HouseR = ucReturnHouse.SelectWareHouseIds;
            csc.HouseV = ucVisitReturnHouse.SelectWareHouseIds;
            csc.Date_D_S = DateTime.Parse(tbDate_D_S.Text.Trim());
            csc.Date_D_E = DateTime.Parse(tbDate_D_E.Text.Trim());
            csc.Date_R_S = DateTime.Parse(tbDate_R_S.Text.Trim());
            csc.Date_R_E = DateTime.Parse(tbDate_R_E.Text.Trim());
            csc.Date_V_S = DateTime.Parse(tbDate_V_S.Text.Trim());
            csc.Date_V_E = DateTime.Parse(tbDate_V_E.Text.Trim());
            csc.ExpressCompanyID = !string.IsNullOrEmpty(hidRfdChecked.Value) ? hidRfdChecked.Value : !string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID) ? UCExpressCompanyTV.SelectExpressID : "";
            csc.AccountType = rbGeneral.Checked ? 1 : rbEMS.Checked ? 2 : rbZJS.Checked ? 3 : 0;
            csc.MerchantID = string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID) ? "" : UCMerchantSourceTV.SelectMerchantID;
            CODSearchCondition = csc;

            CODSearchResult = cODAccountService.SearchUniteAccount(CODSearchCondition);
            return true;
        }

		private bool JudgeInput()
		{
			if (string.IsNullOrEmpty(ucDeliveryHouse.SelectWareHouseIds) ||
				string.IsNullOrEmpty(ucReturnHouse.SelectWareHouseIds) ||
				string.IsNullOrEmpty(ucVisitReturnHouse.SelectWareHouseIds))
			{
				Alert("仓库必选");
				return false;
			}

            if (string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID) && string.IsNullOrEmpty(hidRfdChecked.Value))
			{
				Alert("配送商必选");
				return false;
			}

			if (string.IsNullOrEmpty(tbDate_D_S.Text) ||
				string.IsNullOrEmpty( tbDate_D_E.Text) ||
				string.IsNullOrEmpty(tbDate_R_S.Text) ||
				string.IsNullOrEmpty(tbDate_R_E.Text) ||
				string.IsNullOrEmpty(tbDate_V_S.Text) ||
				string.IsNullOrEmpty(tbDate_V_E.Text)
				)
			{
				Alert("时间必填");
				return false;
			}

			if (string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
			{
				Alert("商家必填");
				return false;
			}

            string selectMerchantId = "," + UCMerchantSourceTV.SelectMerchantID + ",";
            if (selectMerchantId.Contains(",8,") || selectMerchantId.Contains(",9,"))
            {
                string[] selectMerchantIds = selectMerchantId.TrimStart(',').TrimEnd(',').Split(',');
                if (selectMerchantIds.Length > 1)
                {
                    Alert("vancl、vjia需要单独创建");
                    return false;
                }
            }

            if (UCMerchantSourceTV.SelectMerchantID == "8")
			{
                if (!cODAccountService.JudgeWareHouseContains(ucDeliveryHouse.SelectWareHouseIds))
				{
                    Alert("Vancl上海仓库、上海低耗品库、上海调检仓、上海化妆品库独立结算，请选择商家vancl后，选择了发货仓库上海仓库就不要选择其他仓库，否则反之");
					return false;
				}
                if (!cODAccountService.JudgeWareHouseContains(ucReturnHouse.SelectWareHouseIds))
				{
                    Alert("Vancl上海仓库、上海低耗品库、上海调检仓、上海化妆品库独立结算，请选择商家vancl后，选择了拒收入库仓库上海仓库就不要选择其他仓库，否则反之");
					return false;
				}
				if (!cODAccountService.JudgeWareHouseContains(ucVisitReturnHouse.SelectWareHouseIds))
				{
                    Alert("Vancl上海仓库、上海低耗品库、上海调检仓、上海化妆品库独立结算，请选择商家vancl后，选择了上门退入库仓库上海仓库就不要选择其他仓库，否则反之");
					return false;
				}
			}

			if ((RFD.FMS.Util.Common.JudgeDateAreaByDay(DateTime.Parse(tbDate_D_S.Text), DateTime.Parse(tbDate_D_E.Text)) ||
				RFD.FMS.Util.Common.JudgeDateAreaByDay(DateTime.Parse(tbDate_R_S.Text), DateTime.Parse(tbDate_R_E.Text)) ||
				RFD.FMS.Util.Common.JudgeDateAreaByDay(DateTime.Parse(tbDate_V_S.Text), DateTime.Parse(tbDate_V_E.Text))))
			{
				Alert("结束时间不能早于开始时间");
				return false;
			}

			//仓库一致
			if ((ucDeliveryHouse.SelectWareHouseIds != ucReturnHouse.SelectWareHouseIds ||
				ucDeliveryHouse.SelectWareHouseIds != ucVisitReturnHouse.SelectWareHouseIds ||
				ucReturnHouse.SelectWareHouseIds != ucVisitReturnHouse.SelectWareHouseIds))
			{
				Alert("发货、拒收、上门退仓库选择必须一致");
				return false;
			}

			int maxSearchDay = 31;
			//时间限制31天内
			TimeSpan dayD = DateTime.Parse(tbDate_D_E.Text) - DateTime.Parse(tbDate_D_S.Text);
			TimeSpan dayR = DateTime.Parse(tbDate_R_E.Text) - DateTime.Parse(tbDate_R_S.Text);
			TimeSpan dayV = DateTime.Parse(tbDate_V_E.Text) - DateTime.Parse(tbDate_V_S.Text);
			if (dayD.TotalDays > maxSearchDay ||
				dayR.TotalDays > maxSearchDay ||
				dayV.TotalDays > maxSearchDay)
			{
				Alert(string.Format("时间区域不能大于{0}天", maxSearchDay.ToString()));
				return false;
			}

			return true;
		}

		protected void SaveAccount_Click(object sender, EventArgs e)
		{
            try
            {
                if (!SearchData()) throw new Exception("保存出错");

                if (gvList.Rows.Count <= 0)
                {
                    Alert("没有列表");
                    return;
                }

                //验证日统计日志中是否存在错误日志
                DataTable dtErrorLog = cODAccountService.GetErrorLog(CODSearchCondition);
                if (dtErrorLog != null && dtErrorLog.Rows.Count > 0)
                {
                    //Alert("所查询条件存在日统计错误");
                    CODSearchCondition.IsDifference = 1;
                }
                else
                {
                    CODSearchCondition.IsDifference = 0;
                }

                string accountNo = string.Empty;
                if (cODAccountService.SaveAccount(CODSearchCondition, CODSearchResult, Userid.ToString(), out accountNo))
                {
                    AccountNO = accountNo;
                    InitEdit(false);
                    Alert("保存成功");
                }
                else
                    Alert("保存失败");
            }
			catch (Exception ex)
			{
				Alert("保存失败<br>" + ex.Message);
			}
		}		

		protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				int n = int.Parse(gvList.DataKeys[e.Row.RowIndex].Values[3].ToString());
				switch (n)
				{
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

		protected void DeleteAccountNO_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(AccountNO))
				{
					Alert("没有找到结算单号");
					return;
				}

				IList<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
				keyValuePairs.Add(new KeyValuePair<string, string>(AccountNO, AccountNO));
				if (cODAccountService.DeleteAccountNo(keyValuePairs, Userid.ToString()))
				{
					InitCreate();
					Alert("删除成功");
				}
				else
					Alert("删除失败");
			}
			catch (Exception ex)
			{
				Alert("删除失败<br>" + ex.Message);
			}
		}

		protected void UpdateMsg_Click(object sender, EventArgs e)
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
				//            checkRows.Add(new KeyValuePair<string, string>(dataKey.Value.ToString(), dataKey.Values[4].ToString()));
				//        }
				//    }
				//}
				//if (checkRows.Count <= 0)
				//{
				//    Alert("没有选择需要修改仓库");
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

				//string url = string.Format("'CODAccountFeeEdit.aspx?accountDetailId={0}'", checkRows[0].Key);
				string url = string.Format("'CODAccountFeeEdit.aspx?accountNo={0}'", AccountNO);
				RunJS(string.Format("fnShowLayer({0});", url));
			}
			catch (Exception ex)
			{
				Alert("打开更新失败<br>" + ex.Message);
			}
		}

		protected void SubmitAccount_Click(object sender, EventArgs e)
		{
			try
			{
				IList<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
				keyValuePairs.Add(new KeyValuePair<string, string>(AccountNO, ""));

				if (cODAccountService.UpdateAccountAuditStatus(keyValuePairs, (int)EnumAccountAudit.A2, Userid.ToString()))
					RunJS("alert('提交成功');fnCloseNewPage('" + MenuLibrary.AccountEditMenu("").MenuID + "')");
				else
					Alert("提交失败");
			}
			catch (Exception ex)
			{
				Alert("提交失败<br>" + ex.Message);
			}
		}

		protected void ExportMsg_Click(object sender, EventArgs e)
		{
			try
			{
				if (gvList.Rows.Count <= 0)
				{
					Alert("请查询需要导出的数据");
					return;
				}
				List<string> l = new List<string>();
				foreach (DataControlField column in gvList.Columns)
				{
					l.Add(column.HeaderText);
				}
				ExportExcel(GridViewHelper.GridView2DataTable(gvList), l.ToArray(), "结算明细汇总表");
			}
			catch (Exception ex)
			{
				Alert("导出失败<br>" + ex.Message);
			}
		}

		protected void PrintMsg_Click(object sender, EventArgs e)
		{
			try
			{
				if (gvList.Rows.Count <= 0)
				{
					Alert("请查询需要打印的数据列表");
					return;
				}

				string headerText = string.Empty;
				DataTable dt = GridViewHelper.GridView2DataTable(gvList);
				List<string> l = new List<string>();
				foreach (DataControlField column in gvList.Columns)
				{
					headerText = column.HeaderText;
					if (headerText == "选择" ||
						headerText == "WarehouseID" ||
						headerText == "DataType" ||
						headerText == "AccountDetailID"
						)
					{
						if (dt.Columns.Contains(headerText))
							dt.Columns.Remove(headerText);
						continue;
					}
					l.Add(headerText);
				}
                int[] columnsWidth = { 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90 };
				ExportPDF(dt, l.ToArray(), AccountNO+"结算单详情汇总表", columnsWidth);
			}
			catch (Exception ex)
			{
				Alert("打印失败<br>" + ex.Message);
			}
		}

		public PageColumns PageColumn
		{
			get { return ViewState["PageColumns"] == null ? null : ViewState["PageColumns"] as PageColumns; }
			set { ViewState.Add("PageColumns", value); }
		}

		private void CreateVisibleColumns()
		{
			IList<string> columns = new List<string>();
			foreach (DataControlField dcf in gvList.Columns)
			{
				if (dcf.Visible == true)
					columns.Add(dcf.HeaderText);
			}
			BindCheckBoxListColumns(columns);
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
				if (k.Key == "商家" || k.Key == "配送站" || k.Key == "区域类型")
				{
					cb.Enabled = false;
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
			for (int i = 0; i < gvList.Columns.Count; i++)
			{
				if (gvList.Columns[i].HeaderText == headerText)
				{
					gvList.Columns[i].Visible = flag;
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

        protected void ExportDifference_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = cODAccountService.GetDifferenceData(CODSearchCondition);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("查询没有差异");
                    return;
                }
                List<string> l = new List<string>();
                foreach (DataColumn column in dt.Columns)
                {
                    l.Add(column.ColumnName);
                }
                ExportExcel(dt, l.ToArray(), null, "结算差异明细表");
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>" + ex.Message);
            }
        }
	}
}
