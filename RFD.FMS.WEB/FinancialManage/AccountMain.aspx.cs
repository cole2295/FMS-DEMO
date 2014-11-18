using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AccountMain : BasePage
	{
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
            }
		}

		protected void btBuildAccount_Click(object sender, EventArgs e)
		{
			RunJS(MenuLibrary.IncomeAccountEditMenu("").JsString);
		}

		protected void btEditAccount_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(0, out checkList))
				return;

			RunJS(MenuLibrary.IncomeAccountEditMenu(checkList[0].Key).JsString);
		}

		private bool JudgeCheckList(int n,out IList<KeyValuePair<string, string>> checkList)
		{
			checkList = Master.gvListCheckList;
			if (checkList == null || checkList.Count <= 0)
			{
				Alert("没有选择需要操作的数据");
				return false;
			}

			if (n == 0 || n==2)
			{
				if (checkList.Count > 1)
				{
					Alert("能且只能操作一条数据");
					return false;
				}
			}

			if (n == 0 || n == 1)
			{
				foreach (KeyValuePair<string, string> k in checkList)
				{
					if (k.Value == EnumHelper.GetDescription(EnumAccountAudit.A4) ||
						k.Value == EnumHelper.GetDescription(EnumAccountAudit.A2))
					{
						Alert("不能操作结算状态为：" + EnumHelper.GetDescription(EnumAccountAudit.A4)
							+ "、" + EnumHelper.GetDescription(EnumAccountAudit.A2) + "的数据");
						return false;
					}
				}
			}

			return true;
		}

		protected void btDeleteAccount_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(1,out checkList))
				return;

			if (incomeAccountService.DeleteAccount(checkList, Userid))
			{
				Master.BindingGridView();
				Alert("删除成功");
			}
			else
			{
				Alert("删除失败");
			}
		}

		protected void btAccountAreaSummary_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(2, out checkList))
				return;

			RunJS(MenuLibrary.IncomeAccountAreaSummaryMenu(checkList[0].Key).JsString);
		}
	}
}
