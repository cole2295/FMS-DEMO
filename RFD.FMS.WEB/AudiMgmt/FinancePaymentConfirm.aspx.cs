using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Model.UnionPay;

namespace RFD.FMS.WEB.AudiMgmt
{
	public partial class FinancePaymentConfirm : FMSBasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindPayType(ddlPayType);
                Master.BindMerchantSource(base.DistributionCode);
			}
			RegisterPager();//注册分页事件
			ReloadScript(LoadScriptType.Init);//注册脚本
		}

		#region 建立搜索条件以及搜索

		protected override SearchCondition BuildSearchCondition(SearchType searchType)
		{
			var condition = new SearchCondition();
			switch (searchType)
			{
				case SearchType.Total:
					condition.DeliverStation = station.Name.IsNullData() ? 0 : station.ID.ConvertToInt();
					condition.StationName = station.Name.Trim();
					condition.MerchantID = Master.MerchantSourceDropDownList.SelectedValue.ConvertToInt();
					condition.MerchantName = Master.MerchantSourceDropDownList.SelectedItem.Text;
					condition.Source = Master.OrderSourceDropDownList.SelectedValue.ConvertToInt(-1);
					condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
					condition.BeginTime = StringUtil.FormatDateTime(txtBeginTime.Text, DateTime.Now);
					condition.EndTime = StringUtil.FormatDateTime(txtEndTime.Text, DateTime.Now);
					condition.OrderBy = GridViewSortExpression;
					condition.Direction = GridViewSortDirection == SortDirection.Descending ? "DESC" : "ASC";
					condition.IsRawData = false;
					ViewState["TotalCondition"] = condition;
					break;
				case SearchType.Details:
					condition.DeliverStation = hidDetailsParams.Value.Split('&')[0].ConvertToInt();
					condition.StationName = hidDetailsParams.Value.Split('&')[1];
					condition.MerchantID = hidDetailsParams.Value.Split('&')[2].ConvertToInt(0);
					condition.MerchantName = hidDetailsParams.Value.Split('&')[3];
					condition.Source = hidDetailsParams.Value.Split('&')[4].ConvertToInt(-1);
					condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
					condition.BeginTime = StringUtil.FormatDateTime(hidDetailsParams.Value.Split('&')[5], DateTime.Now);
					condition.EndTime = StringUtil.FormatDateTime(hidDetailsParams.Value.Split('&')[5], DateTime.Now);
					condition.IsRawData = false;
					ViewState["DetailsCondition"] = condition;
					break;
				case SearchType.AllDetails:
					condition.MerchantID = Master.MerchantSourceDropDownList.SelectedValue.ConvertToInt();
					condition.Source = Master.OrderSourceDropDownList.SelectedValue.ConvertToInt(-1);
					condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
					condition.BeginTime = StringUtil.FormatDateTime(txtBeginTime.Text, DateTime.Now);
					condition.EndTime = StringUtil.FormatDateTime(txtEndTime.Text, DateTime.Now);
					condition.IsRawData = false;
					break;
				case SearchType.Reload:
					condition.DeliverStation = station.ID.ConvertToInt();
					condition.MerchantID = Master.MerchantSourceDropDownList.SelectedValue.ConvertToInt();
					condition.Source = Master.OrderSourceDropDownList.SelectedValue.ConvertToInt(-1);
					condition.SearchDate = StringUtil.FormatDateTime(txtReloadDate.Text, DateTime.Now);
					break;
			}
			return condition;
		}

		protected override DataTable GetTotalData(SearchCondition condition)
		{
			return service.GetTotalFinanceDailyData(condition);
		}

		protected override DataTable GetDetailsData(SearchCondition condition)
		{
			return service.GetDetailsFinanceDailyData(condition);
		}

        //protected override DataTable GetDetailsDataV2(SearchCondition condition)
        //{
        //    return service.GetDetailsFinanceDailyDataV2(condition);
        //}

        protected override DataTable GetAllDetailsData(SearchCondition condition)
        {
            return service.GetDetailsFinanceDailyData(condition);
        }

        //protected override DataTable GetAllDetailsDataV2(SearchCondition condition)
        //{
        //    return service.GetDetailsFinanceDailyDataV2(condition);
        //}

		protected override void BindTotalData()
		{
			//ClearCache(SearchType.Total);
			ShowGridviewColumn(SearchType.Total);

			if (TotalData != null)
			{
				hidTotalCount.Value = TotalData.Rows.Count.ToString();

				hidDetailsCount.Value = "0";

				ShowPager(totalPager, hidTotalCount.Value);
				//隐藏详情数据
				detailsPager.Visible = ltlDetails.Visible = gvDetails.Visible = false;

                ltlTotal.Text = ShowTipText(TotalData);

				if (TotalData.Rows.Count == 0)
				{
					GridViewHelper.ShowEmptyGridHeader(gvSummary, TotalData, "查找不到任何关于此站点的数据，请重新设定条件查询！");

					return;
				}

				BindDataWithBuildPage(TotalData, totalPager, gvSummary);

				HideGridviewColumn(SearchType.Total);

				gvSummary.RowStyle.HorizontalAlign = HorizontalAlign.Left;

				ReloadScript(LoadScriptType.Search);
			}
		}
        private string ShowTipText(DataTable dt)
        {
            var tip = new StringBuilder();
            //var total = dt;
            int CashWaybillCount = 0;
            int POSWaybillCount = 0;
            int SucessWaybillCount = 0;
            decimal AcceptAmount = 0;
            int BackWaybillCount = 0;
            decimal CashRealOutSum = 0;
            decimal SaveAmount = 0;
            if(dt!=null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    CashWaybillCount += dr["CashSuccOrderCount"].ConvertToInt();
                    POSWaybillCount += dr["PosSuccOrderCount"].ConvertToInt();
                    SucessWaybillCount = CashWaybillCount + POSWaybillCount;
                    AcceptAmount += dr["AcceptAmount"].ConvertToDecimal();
                    BackWaybillCount += dr["DayOutOrderCount"].ConvertToInt();
                    CashRealOutSum += dr["CashRealOutSum"].ConvertToDecimal();
                    SaveAmount += dr["SaveAmount"].ConvertToDecimal();
                }
            }
            else
            {
               return String.Empty; 
            } 
            
            //if (StringUtil.IsEmptyDataRow(dr)) return String.Empty;
            tip.AppendFormat(@"<hr color='#999999'/>
                                          现金成功单量：<font color='blue'><b>{0}</b></font>，
                                          POS机成功单量：<font color='blue'><b>{1}</b></font>，
                                          成功单量：<font color='blue'><b>{2}</b></font>，
                                          成功金额：<font color='blue'><b>{3}</b></font>，
                                          退款订单量：<font color='blue'><b>{4}</b></font>，
                                          退款金额：<font color='blue'><b>{5}</b></font>，
                                          存款金额：<font color='blue'><b>{6}</b></font>
                                          ",
                                          CashWaybillCount,
                                          POSWaybillCount,
                                          SucessWaybillCount,
                                          AcceptAmount,
                                          BackWaybillCount,
                                          CashRealOutSum,
                                          SaveAmount);
            return tip.ToString();
        }
		protected override void BindDetailsData()
		{
			//ClearCache(SearchType.Details);
			ShowGridviewColumn(SearchType.Details);
			if (DetailsData != null)
			{
				hidDetailsCount.Value = DetailsData.Rows.Count.ToString();
				ShowPager(detailsPager, hidDetailsCount.Value);
				//显示详情数据
				ltlDetails.Visible = gvDetails.Visible = true;
				ltlDetails.Text = ShowTipText(SearchType.Details, ViewState["DetailsCondition"] as SearchCondition);
				if (DetailsData.Rows.Count == 0)
				{
					GridViewHelper.ShowEmptyGridHeader(gvDetails, DetailsData, "该站点无任何明细数据，请重新设定条件查询！");
					return;
				}
				BindDataWithBuildPage(DetailsData, detailsPager, gvDetails);
				HideGridviewColumn(SearchType.Details);
				gvDetails.RowStyle.HorizontalAlign = HorizontalAlign.Left;
				ReloadScript(LoadScriptType.Search);
			}
		}
		#endregion

		#region GridView相关操作
		protected override void ShowGridviewColumn(SearchType searchType)
		{
			switch (searchType)
			{
				case SearchType.Total:
					//gvSummary.Columns[4].Visible = true;//商家名称列
					gvSummary.Columns[3].Visible = true;//现金数量列
					gvSummary.Columns[4].Visible = true;//POS数量列
					break;
				case SearchType.Details:
					//gvDetails.Columns[3].Visible = true;//订单号列
					break;
			}
		}
		protected override void HideGridviewColumn(SearchType searchType)
		{
			switch (searchType)
			{
				case SearchType.Total:
					//gvSummary.Columns[4].Visible = Master.IsShowMerchantName;
					gvSummary.Columns[3].Visible = IsShowCashWaybillCount;
					gvSummary.Columns[4].Visible = IsShowPOSWaybillCount;
					break;
				case SearchType.Details:
					//gvDetails.Columns[3].Visible = Master.IsShowCustomerOrder;
					break;
			}
		}
		#endregion

		#region 在后台执行脚本
		protected override void ReloadScript(LoadScriptType type)
		{
			switch (type)
			{
				case LoadScriptType.Search:
					RegisterScript(@"search.addSortIcon({orderby:'" + GridViewSortExpression +
						"',direction:'" + (GridViewSortDirection == SortDirection.Descending ? "DESC" : "ASC") + "'});", "sort");
					break;
				case LoadScriptType.Init:
					RegisterScript(@"init();", "init");
					break;
			}
		}
		#endregion

		#region 属性
		public bool IsShowCashWaybillCount
		{
			get
			{
				return this.ddlPayType.SelectedValue != ((int)PaymentType.POS).ToString();
			}
		}
		public bool IsShowPOSWaybillCount
		{
			get
			{
				return this.ddlPayType.SelectedValue != ((int)PaymentType.Cash).ToString();
			}
		}
		protected override string DefaultSortField
		{
			get
			{
				return "CompanyName";
			}
		}
		protected override SortDirection DefaultSortDirection
		{
			get
			{
				return SortDirection.Ascending;
			}
		}
		#endregion

		#region 事件
		protected void gvSummary_Sorting(object sender, GridViewSortEventArgs e)
		{
			GridViewSortExpression = e.SortExpression;
			var condition = new SearchCondition()
			{
				DeliverStation = station.ID.ConvertToInt(),
				BeginTime = StringUtil.FormatDateTime(txtBeginTime.Text, DateTime.Now),
				EndTime = StringUtil.FormatDateTime(txtEndTime.Text, DateTime.Now),
				Source = Master.OrderSourceDropDownList.SelectedValue.ConvertToInt(-1),
				MerchantID = Master.MerchantSourceDropDownList.SelectedValue.ConvertToInt(),
				PayType = ddlPayType.SelectedValue.ConvertToInt(-1),
				OrderBy = GridViewSortExpression
			};
			Sorting(condition);
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			Search(SearchType.Total);//搜索汇总数据
		}

		protected void btnSearchDetails_Click(object sender, EventArgs e)
		{
			Search(SearchType.Details);//搜索明细数据
		}
		protected void btnSummary_Click(object sender, EventArgs e)
		{
			//订单来源为"其他"或不选时才出现"商家名称"列
			Export(Master.CurrentExportFormat, SearchType.Total, TotalData);
		}
		protected void btnDetails_Click(object sender, EventArgs e)
		{
			//订单来源为"其他"时才出现"订单号"列
			Export(Master.CurrentExportFormat, SearchType.Details, DetailsData);
		}
		protected void btnAllDetails_Click(object sender, EventArgs e)
		{
			//导出所有明细
			Export(Master.CurrentExportFormat, SearchType.AllDetails, null);
		}

		protected void gvSummary_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "confirm")
			{
				var model = new FMS_StationDailyFinanceSum()
				{
					ID = e.CommandArgument.ConvertToInt(),
					RealInCome = decimal.Parse(StringUtil.FormatMoney(hidConfirmAmount.Value)),
					FinanceStatus = (int)AcceptStatus.Received
				};
				var result = service.ConfirmDailyTotalAmount(model);
				if (!result)
				{
					Alert("确认失败！");
					return;
				}
				BindTotalData();
			}
		}

        protected void btnReload_ClickV2(object sender, EventArgs e)
        {
            var service = ServiceLocator.GetService<IStationDailyService>();
            var condition = BuildSearchCondition(SearchType.Reload);
            var result = service.ReloadStationDailyDataV2(new RFD.FMS.MODEL.SearchModel()
            {
                dtDailyDate = condition.SearchDate,
                StationId = condition.DeliverStation.ToString(),
                MerchantId = condition.MerchantID.ToString(),
                Sources = condition.Source.ToString()
            });
            if (!result)
            {
                Alert("重新生成失败！请检查当前的服务是否已启动！");

                return;
            }
            BindTotalData();
        }

		protected void btnReload_Click(object sender, EventArgs e)
		{
            var service = ServiceLocator.GetService<IStationDailyService>();
			var condition = BuildSearchCondition(SearchType.Reload);
			var result = service.ReloadStationDailyData(new RFD.FMS.MODEL.SearchModel()
			{
				dtDailyDate = condition.SearchDate,
				StationId = condition.DeliverStation.ToString(),
				MerchantId = condition.MerchantID.ToString(),
				Sources = condition.Source.ToString()
			});
			if (!result)
			{
				Alert("重新生成失败！请检查当前的服务是否已启动！");

				return;
			}
			BindTotalData();
		}

		protected override void AspNetPager_PageChanged(object sender, EventArgs e)
		{
			var pager = sender as Wuqi.Webdiyer.AspNetPager;
			if (totalPager.InnerPager.Equals(pager))
			{
				BindDataWithBuildPage(TotalData, totalPager, gvSummary);
				HideGridviewColumn(SearchType.Total);
				ReloadScript(LoadScriptType.Search);
				return;
			}
			if (detailsPager.InnerPager.Equals(pager))
			{
				BindDataWithBuildPage(DetailsData, detailsPager, gvDetails);
				HideGridviewColumn(SearchType.Details);
				ReloadScript(LoadScriptType.Search);
				return;
			}
		}

		protected override void RegisterPager()
		{
			totalPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
			detailsPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
			ShowPager(totalPager, hidTotalCount.Value);
			ShowPager(detailsPager, hidDetailsCount.Value);
		}
		#endregion

		#region 导出Excel,PDF
		protected override string[] GetExportGridViewHeaders(SearchType searchType)
		{
			var result = new string[] { };
			var ignoreHeaders = new List<string>();
			switch (searchType)
			{
				case SearchType.Total:
					ignoreHeaders.Add("确认金额");
					//if (!Master.IsShowMerchantName) ignoreHeaders.Add("商家名称");
					if (!IsShowCashWaybillCount) ignoreHeaders.Add("现金成功单量");
					if (!IsShowPOSWaybillCount) ignoreHeaders.Add("POS机成功单量");
					result = GridViewHelper.GetGridViewHeaders(gvSummary, ignoreHeaders.ToArray<string>());
					break;
				case SearchType.Details:
				case SearchType.AllDetails:
					//if (!Master.IsShowCustomerOrder) ignoreHeaders.Add("订单号");
					result = GridViewHelper.GetGridViewHeaders(gvDetails, ignoreHeaders.ToArray<string>());
					break;
			}
			return result;
		}
		protected override string GetExportTitle(SearchType searchType)
		{
			var result = string.Empty;
			switch (searchType)
			{
				case SearchType.Total:
					result = String.Format("{0}[{1}至{2}]汇总数据表", station.Name, this.txtBeginTime.Text, this.txtEndTime.Text);
					break;
				case SearchType.Details:
					result = String.Format("{0}[{1}]明细数据表", hidDetailsParams.Value.Split('&')[1], StringUtil.FormatDateTime(hidDetailsParams.Value.Split('&')[5]));
					break;
				case SearchType.AllDetails:
					result = String.Format("[{0}至{1}]所有明细数据表", this.txtBeginTime.Text, this.txtEndTime.Text);
					break;
			}
			return result;
		}
		protected override string[] GetExportIgnoreColumns(SearchType searchType)
		{
			var ignoreColumns = new List<string>();
			switch (searchType)
			{
				case SearchType.Total:
					ignoreColumns.Add("TotalDataID");
					ignoreColumns.Add("QueryParams");
					ignoreColumns.Add("CashRealInSum");
					ignoreColumns.Add("PosRealInSum");
					ignoreColumns.Add("RealInCome");
					//if (!Master.IsShowMerchantName) ignoreColumns.Add("MerchantName");
					if (!IsShowCashWaybillCount) ignoreColumns.Add("CashWaybillCount");
					if (!IsShowPOSWaybillCount) ignoreColumns.Add("POSWaybillCount");
					break;
				case SearchType.Details:
				case SearchType.AllDetails:
					//if (!Master.IsShowCustomerOrder) ignoreColumns.Add("CustomerOrder");
					break;
			}
			return ignoreColumns.ToArray<string>();
		}
		#endregion
	}
}
