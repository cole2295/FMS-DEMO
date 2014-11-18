using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Wuqi.Webdiyer;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.UserControl;

namespace RFD.FMS.WEB.AudiMgmt
{
	public partial class DelayOrderChecked : FMSBasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				//BindExportFormat();
			}
			//RegisterPager();
			//ReloadScript(LoadScriptType.Init);
		}

		#region 从导入文件中获取订单号列表
		/// <summary>
		/// 从导入文件中获取订单号列表
		/// </summary>
		/// <returns></returns>
		private DataTable GetOrderNoList()
		{
			var orders = ImportExcel(this.txtFile);
			if (!orders.IsEmpty())
			{
				if (!orders.Columns.Contains("订单号"))
				{
					Alert("请你先下载模板到本地，然后填写相关的订单信息再进行导入！");
					return null;
				}
				orders.RemoveEmptyRow();
			}
            if (orders == null)
            {
                Literal12.Text = "没有核对单号";
            }
            else
            {
                Literal12.Text = "核对" + orders.Rows.Count.ToString() + "单";
            }
			return orders;
		}
		/*
		private void ClearAllGridViewData()
		{
			TotalData = null;

			this.gvTotalData.DataSource = null;
			this.gvTotalData.DataBind();

			this.gvSuccessOrders.DataSource = null;
			this.gvSuccessOrders.DataBind();

			this.gvDelayOrders.DataSource = null;
			this.gvDelayOrders.DataBind();

			this.ltlSuccessInfo.Visible = this.ltlDelayInfo.Visible = false;
			this.hidSuccessCount.Value = this.hidDelayCount.Value = "0";
			ShowPager(successPager, hidSuccessCount.Value);
			ShowPager(delayPager, hidDelayCount.Value);
		}
		 * */
		#endregion

		#region 导入文件后绑定所有订单数据
		protected override SearchCondition BuildSearchCondition(SearchType searchType)
		{
			return new SearchCondition()
			{
				ExportPath = Server.MapPath("~\\file\\") + DateTime.Now.ToString("yyyyMMddHHmmssms") + "\\滞留订单核对报表",
				OrderNoList = GetOrderNoList()
			};
		}
		protected void btnImport_Click(object sender, EventArgs e)
		{
            try
            {
                var condition = BuildSearchCondition(SearchType.Delay);
                var result = String.Empty;
                var reports = service.CheckDelayOrders(condition, out result);
                this.ltlResult.Text = result;
                //执行导出
                service.ExportDelayReports(reports, condition);
            }
            catch (Exception ex)
            {
                Alert("核对错误<br>" + ex.Message);
            }
		}
		//private void BindExportFormat()
		//{
		//    var data = EnumHelper.GetEnumValueAndDescriptions<ExportFileFormat>();
		//    rblExportFormat.BindListData(data, "");
		//}
		/*
				/// <summary>
				/// 根据条件绑定数据
				/// </summary>
				/// <param name="condition"></param>
				private void BindOrderData(SearchCondition condition)
				{
					var data = service.GetWaybillListByOrderNo(condition);
					if (data != null)
					{
						this.hidTotalCount.Value = data.Rows.Count.ToString();
						ShowPager(totalPager, hidTotalCount.Value);
						if (data.Rows.Count == 0)
						{
							//无数据时显示表头
							GridViewHelper.ShowEmptyGridHeader(gvTotalData, data, "无任何数据，请重新导入！");
							return;
						}
						TotalData = data;
						BindDataWithBuildPage(TotalData, totalPager, gvTotalData);
						this.ltlTotalInfo.Text = String.Format("(共导入<font color='blue'><b>{0}</b></font>条订单数据)", hidTotalCount.Value);
						this.gvTotalData.RowStyle.HorizontalAlign = HorizontalAlign.Left;
					}
					else
					{
						this.ltlTotalInfo.Text = String.Empty;
						this.hidTotalCount.Value = "0";
						ShowPager(totalPager, hidTotalCount.Value);
					}
				}
				#endregion

				#region 核对订单并绑定妥投及滞留订单
				protected void btnCheck_Click(object sender, EventArgs e)
				{
					this.ltlSuccessInfo.Visible = this.ltlDelayInfo.Visible = true;
					//绑定妥投/拒收入库/退换货入库订单
					BindSuccessOrder();
					//绑定滞留订单
					BindDelayOrder();
				}

				/// <summary>
				/// 绑定妥投/拒收入库/退换货入库订单
				/// </summary>
				private void BindSuccessOrder()
				{
					if (SuccessOrders != null)
					{
						this.hidSuccessCount.Value = SuccessOrders.Rows.Count.ToString();
						ShowPager(successPager, hidSuccessCount.Value);
						if (SuccessOrders.Rows.Count == 0)
						{
							//无数据时显示表头
							GridViewHelper.ShowEmptyGridHeader(gvSuccessOrders, SuccessOrders, "无妥投/拒收入库/退换货入库订单数据！");
							return;
						}
						BindDataWithBuildPage(SuccessOrders, successPager, gvSuccessOrders);
						this.ltlSuccessInfo.Text = String.Format("(共导入<font color='blue'><b>{0}</b></font>条妥投订单数据)", hidSuccessCount.Value);
						this.gvSuccessOrders.RowStyle.HorizontalAlign = HorizontalAlign.Left;
					}
					else
					{
						this.ltlSuccessInfo.Text = String.Empty;
						this.hidSuccessCount.Value = "0";
					}
				}
				/// <summary>
				/// 绑定滞留订单
				/// </summary>
				private void BindDelayOrder()
				{
					if (DelayOrders != null)
					{
						this.hidDelayCount.Value = DelayOrders.Rows.Count.ToString();
						ShowPager(delayPager, hidDelayCount.Value);
						if (DelayOrders.Rows.Count == 0)
						{
							//无数据时显示表头
							GridViewHelper.ShowEmptyGridHeader(gvDelayOrders, DelayOrders, "无滞留订单数据！");
							return;
						}
						BindDataWithBuildPage(DelayOrders, delayPager, gvDelayOrders);
						this.ltlDelayInfo.Text = String.Format("(共导入<font color='blue'><b>{0}</b></font>条滞留订单数据)", hidDelayCount.Value);
						this.gvDelayOrders.RowStyle.HorizontalAlign = HorizontalAlign.Left;
					}
					else
					{
						this.ltlDelayInfo.Text = String.Empty;
						this.hidDelayCount.Value = "0";
					}
				}
				#endregion

				#region 事件
				protected void btnSuccessOrder_Click(object sender, EventArgs e)
				{
					Export(CurrentExportFormat, SearchType.Success, SuccessOrders);
				}
				protected void btnDelayOrder_Click(object sender, EventArgs e)
				{
					Export(CurrentExportFormat, SearchType.Delay, DelayOrders);
				}
				protected override void AspNetPager_PageChanged(object sender, EventArgs e)
				{
					var pager = sender as Wuqi.Webdiyer.AspNetPager;
					if (totalPager.InnerPager.Equals(pager))
					{
						BindDataWithBuildPage(TotalData, totalPager, gvTotalData);
						return;
					}
					if (successPager.InnerPager.Equals(pager))
					{
						BindDataWithBuildPage(SuccessOrders, successPager, gvSuccessOrders);
						return;
					}
					if (delayPager.InnerPager.Equals(pager))
					{
						BindDataWithBuildPage(DelayOrders, delayPager, gvDelayOrders);
						return;
					}
				}
				protected override void RegisterPager()
				{
					totalPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
					successPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
					delayPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
					ShowPager(totalPager, hidTotalCount.Value);
					ShowPager(successPager, hidSuccessCount.Value);
					ShowPager(delayPager, hidDelayCount.Value);
				}
				*/
				#endregion

		#region 在后台执行脚本
		protected override void ReloadScript(LoadScriptType type)
		{
			if (type == LoadScriptType.Init)
			{
				RegisterScript("init();", "init");
			}
		}
		#endregion

		#region 属性
		//protected override DataTable TotalData
		//{
		//    get
		//    {
		//        return Session["TotalOrders"] as DataTable;
		//    }
		//    set
		//    {
		//        Session["TotalOrders"] = value;
		//    }
		//}
		//public DataTable SuccessOrders
		//{
		//    get
		//    {
		//        if (TotalData == null) return null;
		//        var successOrders = TotalData.Clone();
		//        var data = TotalData.Copy().Select(" IsDelay = 1 ");
		//        for (var i = 0; i < data.Length; i++)
		//        {
		//            data[i]["ID"] = i + 1;
		//            successOrders.ImportRow(data[i]);
		//        }
		//        return successOrders;
		//    }
		//}
		//public DataTable DelayOrders
		//{
		//    get
		//    {
		//        if (TotalData == null) return null;
		//        var delayOrders = TotalData.Clone();
		//        var data = TotalData.Copy().Select(" IsDelay = 0 ");
		//        for (var i = 0; i < data.Length; i++)
		//        {
		//            data[i]["ID"] = i + 1;
		//            delayOrders.ImportRow(data[i]);
		//        }
		//        return delayOrders;
		//    }
		//}
		//public ExportFileFormat CurrentExportFormat
		//{
		//    get { return (ExportFileFormat)rblExportFormat.SelectedIndex; }
		//}
		#endregion

		#region 导出
		/*
		protected override string[] GetExportGridViewHeaders(SearchType searchType)
		{
			var result = new string[] { };
			switch (searchType)
			{
				case SearchType.Success:
					result = GridViewHelper.GetGridViewHeaders(gvSuccessOrders);
					break;
				case SearchType.Delay:
					result = GridViewHelper.GetGridViewHeaders(gvDelayOrders);
					break;
			}
			return result;
		}
		 * */
		//protected override string GetExportTitle(SearchType searchType)
		//{
		//    var result = string.Empty;
		//    switch (searchType)
		//    {
		//        case SearchType.Success:
		//            result = "妥投-拒收入库-退换货入库订单表";
		//            break;
		//        case SearchType.Delay:
		//            result = "滞留订单表";
		//            break;
		//    }
		//    return result;
		//}
		//protected override string[] GetExportIgnoreColumns(SearchType searchType)
		//{
		//    var ignoreColumns = new List<string>();
		//    switch (searchType)
		//    {
		//        case SearchType.Success:
		//        case SearchType.Delay:
		//            ignoreColumns.Add("Status");
		//            ignoreColumns.Add("BackStatus");
		//            ignoreColumns.Add("IsDelay");
		//            break;
		//    }
		//    return ignoreColumns.ToArray<string>();
		//}
		#endregion
	}
}
