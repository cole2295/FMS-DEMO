using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AuditDeliverFee : BasePage
	{
        IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
			if (!IsPostBack)
			{
                
				Master.MerchantIDTextBox.Text = String.IsNullOrEmpty(Request["mid"]) ? "" : Request["mid"];
				var status = String.IsNullOrEmpty(Request["status"]) ? MaintainStatus.Auditing : (MaintainStatus)int.Parse(Request["status"]);
				//绑定维护状态
                deliverFeeService.BindStatus(Master.MaintainStatusCheckBoxList, status);
                //Master.BindDeliverFeeList(Master.UCPager.CurrentPageIndex);
			}
		}

		protected void btnAuditPrice_Click(object sender, EventArgs e)
		{
			if (JudgeListCheck(0))
				RunJS(MenuLibrary.DeliverFeeAuditMenu(MerchantID).JsString);
		}

		protected void btnAuditBasic_Click(object sender, EventArgs e)
		{
			try
			{
				IList<KeyValuePair<string, string>> checkList;
				if (!JudgeListChecks(EnumHelper.GetDescription(MaintainStatus.Audited), out checkList))
					return;
                if (deliverFeeService.UpdateMerchantDeliverFeeStatus(checkList, (int)MaintainStatus.Audited, Userid, base.DistributionCode))
				{
					Alert("审核成功");
                    Master.BindDeliverFeeList(Master.UCPager.CurrentPageIndex);
				}
				else
				{
					Alert("审核失败");
				}
			}
			catch (Exception ex)
			{
				Alert("审核失败<br>" + ex.Message);
			}
		}

		protected void btnResetBaic_Click(object sender, EventArgs e)
		{
			try
			{
				IList<KeyValuePair<string, string>> checkList;
				if(!JudgeListChecks(EnumHelper.GetDescription(MaintainStatus.Reset), out checkList))
					return;
                if (deliverFeeService.UpdateMerchantDeliverFeeStatus(checkList, (int)MaintainStatus.Reset, Userid, base.DistributionCode))
				{
					Alert("置回成功");
                    Master.BindDeliverFeeList(Master.UCPager.CurrentPageIndex);
				}
				else
				{
					Alert("置回失败");
				}
			}
			catch (Exception ex)
			{
				Alert("置回失败<br>"+ex.Message);
			}
		}

		private bool JudgeListChecks(string str, out IList<KeyValuePair<string, string>> checkList)
		{
			checkList = Master.MerchantChecked;

			if (checkList.Count <=0)
			{
				Alert("没有选择需要操作的数据");
				return false;
			}

			foreach (KeyValuePair<string, string> k in checkList)
			{
				if (k.Value == str)
				{
					Alert("不能操作" + str);
					return false;
				}
			}

			return true;
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

        protected void btAuditEffectBasic_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<string, string>> checkList;
                if (!JudgeListChecks(EnumHelper.GetDescription(MaintainStatus.Audited), out checkList))
                    return;
                if (deliverFeeService.UpdateEffectMerchantDeliverFeeStatus(checkList, (int)MaintainStatus.Audited, Userid, base.DistributionCode))
                {
                    Alert("审核成功");
                    Master.BindDeliverFeeList(Master.UCPager.CurrentPageIndex);
                }
                else
                {
                    Alert("审核失败");
                }
            }
            catch (Exception ex)
            {
                Alert("审核失败<br>" + ex.Message);
            }
        }

        protected void btnResetEffectBasic_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<string, string>> checkList;
                if (!JudgeListChecks(EnumHelper.GetDescription(MaintainStatus.Reset), out checkList))
                    return;
                if (deliverFeeService.UpdateEffectMerchantDeliverFeeStatus(checkList, (int)MaintainStatus.Reset, Userid, base.DistributionCode))
                {
                    Alert("置回成功");
                    Master.BindDeliverFeeList(Master.UCPager.CurrentPageIndex);
                }
                else
                {
                    Alert("置回失败");
                }
            }
            catch (Exception ex)
            {
                Alert("置回失败<br>" + ex.Message);
            }
        }
	}
}
