using System;
using System.Data;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class MaintainDeliverFee : BasePage
	{
        IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
			if (!IsPostBack)
			{
                
				Master.MerchantIDTextBox.Text = String.IsNullOrEmpty(Request["mid"]) ? "" : Request["mid"];
				var status = String.IsNullOrEmpty(Request["status"]) ? MaintainStatus.Maintain : (MaintainStatus)int.Parse(Request["status"]);
				//绑定维护状态
                deliverFeeService.BindStatus(Master.MaintainStatusCheckBoxList, status);
                //Master.BindDeliverFeeList(Master.UCPager.CurrentPageIndex);
			}
		}

		protected void btnEditPrice_Click(object sender, EventArgs e)
		{
			if(JudgeListCheck(0))
				RunJS(MenuLibrary.DeliverFeeMenu(MerchantID).JsString);
		}

		protected void btnEditBasic_Click(object sender, EventArgs e)
		{
			if (JudgeListCheck(1))
                RunJS("fnOpenModalDialog('UpdateDeliverFee.aspx?opType=0&mid=" + MerchantID + "',700,500);");
		}

		private bool JudgeListCheck(int n)
		{
			IList<KeyValuePair<string, string>> checkList = Master.MerchantChecked;
			if (checkList.Count > 1)
			{
				Alert("只能同时操作一条操作");
				return false;
			}

            if (n == 1 || n == 2)
			{
				if (checkList.Count <= 0)
				{
					Alert("必须选择一条数据操作");
					return false;
				}
			}

            if (n == 1)
            {
                foreach (KeyValuePair<string, string> k in checkList)
                {
                    if (k.Value == EnumHelper.GetDescription(MaintainStatus.Audited))
                    {
                        Alert("不能操作已审核");
                        return false;
                    }
                }
            }

            if (n == 2)
            {
                foreach (KeyValuePair<string, string> k in checkList)
                {
                    if (k.Value != EnumHelper.GetDescription(MaintainStatus.Audited))
                    {
                        Alert("只能操作已审核");
                        return false;
                    }
                }
            }

			if(checkList.Count==1)
				MerchantID = checkList[0].Key;
			return true;
		}

		private string _merchantId;
		public string MerchantID
		{
			get { return _merchantId; }
			set { _merchantId = value; }
		}

        protected void btnAddEffectBasic_Click(object sender, EventArgs e)
        {
            if (JudgeListCheck(2))
            {
                if (deliverFeeService.GetEffectMerchantDeliverByMerchantID(int.Parse(MerchantID)) > 0)
                {
                    Alert("已经存在待生效，执行更新待生效操作");
                    return;
                }
                else
                {
                    RunJS("fnOpenModalDialog('UpdateDeliverFee.aspx?opType=1&mid=" + MerchantID + "',700,500);");
                }
            }   
        }

        protected void btnUpdateEffectBasic_Click(object sender, EventArgs e)
        {
            if (JudgeListCheck(1))
                RunJS("fnOpenModalDialog('UpdateDeliverFee.aspx?opType=2&mid=" + MerchantID + "',700,500);");
        }
	}
}
