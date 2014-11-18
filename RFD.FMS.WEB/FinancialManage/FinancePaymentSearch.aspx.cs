using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class FinancePaymentSearch : FMSBasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                BindOrderSource();
                BindExportFormat();
				BindPayType(ddlPayType);
			    BindPOStype();
			    ddlPOSType.Enabled = false;
                UCMerchantSourceTV.Editable = false;
			}
			RegisterPager();//注册分页事件
			ReloadScript(LoadScriptType.Init);//注册脚本
		}

        public bool IsClickV2
        {
            get { return DataConvert.ToBoolean(ViewState["ClickV2"]); }
            set { ViewState["ClickV2"] = value; }
        }

        /// <summary>
        /// 绑定订单来源
        /// </summary>
        public void BindOrderSource()
        {
            var data = EnumHelper.GetEnumValueAndDescriptions<WaybillSourse>();
            ddlOrderSource.BindListData(data);
        }

        public void BindExportFormat()
        {
            var data = EnumHelper.GetEnumValueAndDescriptions<ExportFileFormat>();
            rblExportFormat.BindListData(data, "");
        }

        private void BindPOStype()
        {
            IStatusInfoService sis = ServiceLocator.GetService<IStatusInfoService>();

            DataTable dt = sis.GetStatusInfoByTypeNo(401);
            ddlPOSType.DataSource = dt;
            ddlPOSType.DataTextField = "statusName";
            ddlPOSType.DataValueField = "statusNo";
            ddlPOSType.DataBind();
            ddlPOSType.Items.Insert(0, new ListItem("全部", "-1"));
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
                    condition.MerchantIDs = UCMerchantSourceTV.SelectMerchantID;
                    condition.MerchantName = UCMerchantSourceTV.SelectMerchantID;
                    condition.Source = ddlOrderSource.SelectedValue.ConvertToInt(-1);
					condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
                    condition.POSType = ddlPOSType.SelectedValue.ConvertToInt(-1);
					condition.BeginTime = StringUtil.FormatDateTime(txtBeginTime.Text, DateTime.Now);
					condition.EndTime = StringUtil.FormatDateTime(txtEndTime.Text, DateTime.Now);
					condition.OrderBy = GridViewSortExpression;
					condition.Direction = GridViewSortDirection == SortDirection.Descending ? "DESC" : "ASC";
					ViewState["TotalCondition"] = condition;
			        condition.DistributionCode = DistributionCode;
					break;
				case SearchType.Details:
					condition.DeliverStation = hidDetailsParams.Value.Split('&')[0].ConvertToInt();
					condition.StationName = hidDetailsParams.Value.Split('&')[1];
					condition.MerchantID = hidDetailsParams.Value.Split('&')[2].ConvertToInt(0);
					condition.MerchantName = hidDetailsParams.Value.Split('&')[3];
					condition.Source = hidDetailsParams.Value.Split('&')[4].ConvertToInt(-1);
					condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
                    condition.POSType = ddlPOSType.SelectedValue.ConvertToInt(-1);
					condition.BeginTime = StringUtil.FormatDateTime(hidDetailsParams.Value.Split('&')[5], DateTime.Now);
					condition.EndTime = StringUtil.FormatDateTime(hidDetailsParams.Value.Split('&')[5], DateTime.Now);
					condition.DistributionCode = hidDetailsParams.Value.Split('&')[6];
					ViewState["DetailsCondition"] = condition;
					break;
				case SearchType.AllDetails:
                    condition.MerchantIDs = UCMerchantSourceTV.SelectMerchantID;
                    condition.Source = ddlOrderSource.SelectedValue.ConvertToInt(-1);
					condition.PayType = ddlPayType.SelectedValue.ConvertToInt(-1);
                    condition.POSType = ddlPOSType.SelectedValue.ConvertToInt(-1);
					condition.BeginTime = StringUtil.FormatDateTime(txtBeginTime.Text, DateTime.Now);
					condition.EndTime = StringUtil.FormatDateTime(txtEndTime.Text, DateTime.Now);
                    condition.DistributionCode = DistributionCode;
					break;
			}
			return condition;
		}

		protected override DataTable GetTotalData(SearchCondition condition)
		{
            if (IsClickV2 == false)
            {
                return service.GetTotalFinanceDataNew(condition);
            }
            else
            {
                return service.GetTotalFinanceDataNewV2(condition);
            }
		}

		protected override DataTable GetDetailsData(SearchCondition condition)
		{
            if (IsClickV2 == false)
            {
                return service.GetDetailsFinanceDataNew(condition);
            }
            else
            {
                return service.GetDetailsFinanceDataNewV2(condition);
            }
		}

		protected override DataTable GetAllDetailsData(SearchCondition condition)
		{
			return service.GetAllDetailsFinanceData(condition);
		}

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
				ltlTotal.Text = ShowTipText(SearchType.Total, ViewState["TotalCondition"] as SearchCondition);
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

		protected override string ShowTipText(SearchType searchType, SearchCondition condition)
		{
			var tip =  base.ShowTipText(searchType, condition);

			if (searchType == SearchType.Total)
			{
                if (IsClickV2 == false)
                {
                    tip += service.GetTotalFinanceDataTipNew(condition);
                }
                else
                {
                    tip += service.GetTotalFinanceDataTipNewV2(condition);
                }
			}

			return tip;
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
					gvSummary.Columns[5].Visible = true;//现金数量列
					gvSummary.Columns[6].Visible = true;//POS数量列
					break;
				case SearchType.Details:
					gvDetails.Columns[3].Visible = true;//订单号列
					break;
			}
		}
		protected override void HideGridviewColumn(SearchType searchType)
		{
			switch (searchType)
			{
				case SearchType.Total:
					//gvSummary.Columns[4].Visible = Master.IsShowMerchantName;
					gvSummary.Columns[5].Visible = IsShowCashWaybillCount;
					gvSummary.Columns[6].Visible = IsShowPOSWaybillCount;
					break;
				case SearchType.Details:
					gvDetails.Columns[3].Visible = IsShowCustomerOrder;
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
                Source = ddlOrderSource.SelectedValue.ConvertToInt(-1),
                MerchantIDs = UCMerchantSourceTV.SelectMerchantID,
				PayType = ddlPayType.SelectedValue.ConvertToInt(-1),
				OrderBy = GridViewSortExpression
			};
			Sorting(condition);
		}
		protected void btnSearch_Click(object sender, EventArgs e)
		{
            IsClickV2 = false;

			Search(SearchType.Total);//搜索汇总数据
		}

        protected void btnSearch_ClickV2(object sender, EventArgs e)
        {
            IsClickV2 = true;

            Search(SearchType.Total);//搜索汇总数据
        }

		protected void btnSearchDetails_Click(object sender, EventArgs e)
		{
			Search(SearchType.Details);//搜索明细数据
		}
		protected void btnSummary_Click(object sender, EventArgs e)
		{
			//订单来源为"其他"或不选时才出现"商家名称"列
			Export(CurrentExportFormat, SearchType.Total, TotalData);
		}
		protected void btnDetails_Click(object sender, EventArgs e)
		{
			//订单来源为"其他"时才出现"订单号"列
			Export(CurrentExportFormat, SearchType.Details, DetailsData);
		}
		protected void btnAllDetails_Click(object sender, EventArgs e)
		{
			//导出所有明细
			Export(CurrentExportFormat, SearchType.AllDetails, null);
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
					//if (!Master.IsShowMerchantName) ignoreHeaders.Add("商家名称");
					if (!IsShowCashWaybillCount) ignoreHeaders.Add("现金成功单量");
					if (!IsShowPOSWaybillCount) ignoreHeaders.Add("POS机成功单量");
					result = GridViewHelper.GetGridViewHeaders(gvSummary, ignoreHeaders.ToArray<string>());
					break;
				case SearchType.Details:
					if (!IsShowCustomerOrder) ignoreHeaders.Add("订单号");
					result = GridViewHelper.GetGridViewHeaders(gvDetails, ignoreHeaders.ToArray<string>());
					break;
				case SearchType.AllDetails:
                    var cols = new string[] { "序号", "配送站", "运单号", "订单号", "应收金额", "应退金额", "支付方式", "财务收款状态", "POS终端号", "POS机类型", "入站时间", "提交时间" }.ToList<string>();
					if (!IsShowCustomerOrder)
					{
						cols.Remove("订单号");
					}
					result = cols.ToArray();
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
					ignoreColumns.Add("QueryParams");
					//if (!Master.IsShowMerchantName) ignoreColumns.Add("MerchantName");
					if (!IsShowCashWaybillCount) ignoreColumns.Add("CashWaybillCount");
					if (!IsShowPOSWaybillCount) ignoreColumns.Add("POSWaybillCount");
					break;
				case SearchType.Details:
				case SearchType.AllDetails:
					if (!IsShowCustomerOrder) ignoreColumns.Add("CustomerOrder");
					break;
			}
			return ignoreColumns.ToArray<string>();
		}
        protected override string[] GetExportFixColumns(SearchType searchType)
        {
            var ignoreColumns = new List<string>();
            switch (searchType)
            {
                case SearchType.Total:
                    break;
                case SearchType.Details:
                    break;
                case SearchType.AllDetails:
                    break;
            }
            ignoreColumns.Add("订单号");
            return ignoreColumns.ToArray<string>();
        }
		#endregion

        protected void ddlPayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlPayType.SelectedValue=="1")
            {
                ddlPOSType.Enabled = true;
            }
            else
            {
                ddlPOSType.Enabled = false;
            }
        }

        public bool IsShowCustomerOrder
        {
            get
            {
                return this.ddlOrderSource.SelectedValue == ((int)WaybillSourse.Other).ToString();
            }
        }

        public ExportFileFormat CurrentExportFormat
        {
            get { return (ExportFileFormat)rblExportFormat.SelectedIndex; }
        }

        protected void ddlOrderSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlOrderSource.SelectedValue == "2")
            {
                UCMerchantSourceTV.Editable = true;
            }
            else
            {
                UCMerchantSourceTV.Editable = false;
            }
        }
	}
}
