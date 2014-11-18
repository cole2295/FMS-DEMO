using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AccountSearch : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
            }
		}

		protected void btViewAccountDetail_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(out checkList))
				return;
			string url = string.Format("AccountDetailView.aspx?accountNo={0}", checkList[0].Key);
			RunJS(string.Format("fnOpenModalDialog('{0}',1200, 500);", url));
		}

		private bool JudgeCheckList(out IList<KeyValuePair<string,string>> checkList)
		{
			checkList = Master.gvListCheckList;
			if (checkList == null || checkList.Count <= 0)
			{
				Alert("没有选择需要操作的数据");
				return false;
			}

			if (checkList.Count > 1)
			{
				Alert("能且只能同时操作一条数据");
				return false;
			}

			return true;
		}

		protected void btViewAccountAreaSummary_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(out checkList))
				return;

			RunJS(MenuLibrary.IncomeAccountAreaSummaryMenu(checkList[0].Key).JsString);
		}
	}
}
