using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.MODEL;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AccountAudit : BasePage
	{
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
                
            }
		}

		protected void btAudit_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if(!JudgeCheckList(1,EnumHelper.GetDescription(EnumAccountAudit.A4),out checkList))
				return;
            if (incomeAccountService.UpdateAccountStatus(checkList, Userid, (int)EnumAccountAudit.A4))
			{
				Master.BindingGridView();
				Alert("审核成功");
			}
			else
			{
				Alert("审核失败");
			}
		}

		protected void btReset_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(1,EnumHelper.GetDescription(EnumAccountAudit.A3), out checkList))
				return;
            if (incomeAccountService.UpdateAccountStatus(checkList, Userid, (int)EnumAccountAudit.A3))
			{
				Master.BindingGridView();
				Alert("置回成功");
			}
			else
			{
				Alert("置回失败");
			}
		}

		private bool JudgeCheckList(int n,string str,out IList<KeyValuePair<string, string>> checkList)
		{
			checkList = Master.gvListCheckList;
			if (checkList == null || checkList.Count <= 0)
			{
				Alert("没有选择需要操作的数据");
				return false;
			}

			if (n == 1)
			{
				foreach (KeyValuePair<string, string> k in checkList)
				{
					if (k.Value != EnumHelper.GetDescription(EnumAccountAudit.A2))
					{
						Alert("只能操作" + EnumHelper.GetDescription(EnumAccountAudit.A2) + "的数据");
						return false;
					}

					if (k.Value == str)
					{
						Alert("不能操作" + str + "的数据");
						return false;
					}
				}
			}

			if (n == 2)
			{
				if (checkList.Count > 1)
				{
					Alert("能且只能同时操作一项");
					return false;
				}
			}

			return true;
		}

		protected void btViewDetail_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(0,"",out checkList))
				return;
			string url = string.Format("AccountDetailView.aspx?accountNo={0}", checkList[0].Key);
			RunJS(string.Format("fnOpenModalDialog('{0}',1200, 500);", url));
		}

		protected void btViewAccountAreaSummary_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(2,"",out checkList))
				return;

			RunJS(MenuLibrary.IncomeAccountAreaSummaryMenu(checkList[0].Key).JsString);
		}
	}
}
