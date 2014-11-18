using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class QueryDeliverFee : BasePage
	{
        IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
			if (!IsPostBack)
			{
                
				Master.MerchantIDTextBox.Text = String.IsNullOrEmpty(Request["mid"]) ? "" : Request["mid"];
				var status = String.IsNullOrEmpty(Request["status"]) ? MaintainStatus.Audited : (MaintainStatus)int.Parse(Request["status"]);
				//绑定维护状态
                deliverFeeService.BindStatus(Master.MaintainStatusCheckBoxList, status);
                //Master.BindDeliverFeeList(Master.UCPager.CurrentPageIndex);
			}
		}

		protected void btnViewPrice_Click(object sender, EventArgs e)
		{
			if (JudgeListCheck(0))
				RunJS(MenuLibrary.DeliverFeeSearchMenu(MerchantID).JsString);
		}

		private bool JudgeListCheck(int n)
		{
			IList<KeyValuePair<string, string>> checkList = Master.MerchantChecked;
			if (checkList.Count > 1)
			{
				Alert("只能同时操作一条操作");
				return false;
			}

			if (n == 1)
			{
				if (checkList.Count <= 0)
				{
					Alert("必须选择一条数据操作");
					return false;
				}

				foreach (KeyValuePair<string, string> k in checkList)
				{
					if (k.Value == EnumHelper.GetDescription(MaintainStatus.Audited))
					{
						Alert("不能操作已审核");
						return false;
					}
				}
			}

			if (checkList.Count == 1)
				MerchantID = checkList[0].Key;
			return true;
		}

		private string _merchantId;
		public string MerchantID
		{
			get { return _merchantId; }
			set { _merchantId = value; }
		}
	}
}
