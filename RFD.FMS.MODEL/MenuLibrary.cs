using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
	public class MenuLibrary
	{
		public static MenuModel AccountEditMenu(string accountNo)
		{
			return new MenuModel
			{
				MenuID = "AccountEdit",
				MenuTitle = "COD结算编辑",
				MenuUrl = string.Format("../COD/CODAccountEdit.aspx?accountNo={0}", accountNo),
				ShowMenuClose="true"
			};
		}

		public static MenuModel AccountDetailMenu(string accountNo)
		{
			return new MenuModel
			{
				MenuID = "AccountDetail",
				MenuTitle = "配送费发货结算明细",
				MenuUrl = string.Format("../COD/CODDetailView.aspx?accountNo={0}", accountNo),
				ShowMenuClose = "true"
			};
		}

		public static MenuModel AccountDetailViewMenu(string accountNo)
		{
			return new MenuModel
			{
				MenuID = "CODAccountDetailView",
                MenuTitle = "配送费发货结算单明细",
				MenuUrl = string.Format("../COD/CODAccountDetailView.aspx?accountNo={0}", accountNo),
				ShowMenuClose = "true"
			};
		}

		public static MenuModel AccountAreaFareMenu(string accountNo)
		{
			return new MenuModel
			{
				MenuID = "AccountAreaFareMenu",
                MenuTitle = "配送费发货区域运费汇总表",
				MenuUrl = string.Format("../COD/CODAreaFareSummary.aspx?accountNo={0}", accountNo),
				ShowMenuClose = "true"
			};
		}

		public static MenuModel DeliverFeeMenu(string merchantId)
		{
			return new MenuModel
			{
				MenuID = "DeliverFeeMenu",
                MenuTitle = "商家配送费设置",
				MenuUrl = string.Format("../FinancialManage/MerchantDeliverFeeMain.aspx?mid={0}", merchantId),
				ShowMenuClose = "true"
			};
		}

		public static MenuModel DeliverFeeAuditMenu(string merchantId)
		{
			return new MenuModel
			{
				MenuID = "DeliverFeeAuditMenu",
                MenuTitle = "商家配送费审核",
				MenuUrl = string.Format("../FinancialManage/MerchantDeliverFeeAudit.aspx?mid={0}", merchantId),
				ShowMenuClose = "true"
			};
		}

		public static MenuModel DeliverFeeSearchMenu(string merchantId)
		{
			return new MenuModel
			{
				MenuID = "DeliverFeeSearchMenu",
                MenuTitle = "商家配送费查询",
				MenuUrl = string.Format("../FinancialManage/MerchantDeliverFeeSearch.aspx?mid={0}", merchantId),
				ShowMenuClose = "true"
			};
		}

		public static MenuModel IncomeAccountEditMenu(string accountNo)
		{
			return new MenuModel
			{
				MenuID = "IncomeAccountEditMenu",
				MenuTitle = "配送费收入结算编辑",
				MenuUrl = string.Format("../FinancialManage/AccountBuild.aspx?accountNo={0}", accountNo),
				ShowMenuClose = "true"
			};
		}

		public static MenuModel IncomeAccountAreaSummaryMenu(string accountNo)
		{
			return new MenuModel
			{
				MenuID = "IncomeAccountAreaSummaryMenu",
                MenuTitle = "配送费收入结算区域汇总",
				MenuUrl = string.Format("../FinancialManage/AccountAreaSummary.aspx?accountNo={0}", accountNo),
				ShowMenuClose = "true"
			};
		}

        public static MenuModel AudiTasksMenuModel(int flowProcessId)
        {
            return new MenuModel
            {
                MenuID = "AudiTasksMenuModel",
                MenuTitle = "单据审核",
                MenuUrl = string.Format("~/AudiMgmt/Detail.aspx?Id={0}&Batch=false", flowProcessId),
                ShowMenuClose = "true"
            };
        }

        public static MenuModel GotoPasswordChanage(string url)
        {
            return new MenuModel
            {
                MenuID = "GotoPasswordChanage",
                MenuTitle = "修改密码",
                MenuUrl = url,
                ShowMenuClose = "true"
            };
        }
	}

	[Serializable]
	public class MenuModel
	{
		public string MenuID { get; set; }
		public string MenuTitle { get; set; }
		public string MenuUrl { get; set; }
		public string ShowMenuClose { get; set; }
		public string JsString
		{
			get { return string.Format("fnOpenNewPage('{0}','{1}','{2}',{3});", MenuID, MenuTitle, MenuUrl, ShowMenuClose); }
		}
	}
}
