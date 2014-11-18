using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class MerchantDeliverFeeAudit : BasePage
	{
        IMerchantDeliverFee merchantDeliverFee = ServiceLocator.GetService<IMerchantDeliverFee>();
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
            }
            
		}

		protected void btnAuditFee_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(1,EnumHelper.GetDescription(EnumCODAudit.A1), out checkList))
				return;

            if (merchantDeliverFee.UpdateDeliverFeeStatus(checkList, Userid, (int)EnumCODAudit.A1))
			{
				Master.BindGridView(Master.UCPager.CurrentPageIndex);
				Alert("审核成功");
			}
			else
			{
				Alert("审核失败");
			}
		}

		protected void btnResetFee_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> checkList;
			if (!JudgeCheckList(1, EnumHelper.GetDescription(EnumCODAudit.A3), out checkList))
				return;

            if (merchantDeliverFee.UpdateDeliverFeeStatus(checkList, Userid, (int)EnumCODAudit.A3))
			{
                Master.BindGridView(Master.UCPager.CurrentPageIndex);
				Alert("置回成功");
			}
			else
			{
				Alert("置回失败");
			}
		}

		private bool JudgeCheckList(int n,string status, out IList<KeyValuePair<string, string>> checkList)
		{
			checkList = Master.GridViewCheckList;
			if (checkList.Count <= 0)
			{
				Alert("至少选择一项操作");
				return false;
			}

			if (n == 0)
			{
				if (checkList.Count > 1)
				{
					Alert("能且只能同步操作一项");
					return false;
				}
			}

			foreach (KeyValuePair<string, string> k in checkList)
			{
				if (k.Value == status)
				{
					Alert("不能操作已审核项");
					return false;
				}
			}

			return true;
		}

        protected void btnAuditFeeWait_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, string>> checkList;
            if (!JudgeCheckList(1, EnumHelper.GetDescription(EnumCODAudit.A1), out checkList))
                return;

            if (merchantDeliverFee.UpdateWaitDeliverFeeStatus(checkList, Userid, (int)EnumCODAudit.A1))
            {
                Master.BindGridView(Master.UCPager.CurrentPageIndex);
                Alert("审核成功");
            }
            else
            {
                Alert("审核失败");
            }
        }

        protected void btnResetFeeWait_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, string>> checkList;
            if (!JudgeCheckList(1, EnumHelper.GetDescription(EnumCODAudit.A3), out checkList))
                return;

            if (merchantDeliverFee.UpdateWaitDeliverFeeStatus(checkList, Userid, (int)EnumCODAudit.A3))
            {
                Master.BindGridView(Master.UCPager.CurrentPageIndex);
                Alert("置回待生效成功");
            }
            else
            {
                Alert("置回待生效失败");
            }
        }
	}
}
