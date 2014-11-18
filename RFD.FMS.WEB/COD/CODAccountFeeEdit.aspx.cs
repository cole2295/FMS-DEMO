using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using RFD.FMS.MODEL;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.COD
{
	public partial class CODAccountFeeEdit : BasePage
	{
        ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (string.IsNullOrEmpty(AccountNO))
				{
					Alert("没有找到结算单号");
					return;
				}

                CODAccountDetail ad = cODAccountService.SearchAccountDetail(AccountNO);
				tbAllowance.Text = ad.Allowance.ToString();
				tbIntercityLose.Text = ad.IntercityLose.ToString();
				tbKPI.Text = ad.KPI.ToString();
				tbOtherCost.Text = ad.OtherCost.ToString();
				tbPOSPrice.Text = ad.POSPrice.ToString();
				tbStrandedPrice.Text = ad.StrandedPrice.ToString();
			    tbCollectionFee.Text = ad.CollectionFee.ToString();
			    tbDeliveryFee.Text = ad.DeliveryFee.ToString();

			}
		}

		public string AccountNO
		{
			get { return string.IsNullOrEmpty(Request.QueryString["accountNo"]) ? null : Request.QueryString["accountNo"].ToString(); }
		}

		protected void btOK_Click(object sender, EventArgs e)
		{
			if (!JudgeInput())
				return;

			CODAccountDetail ad = new CODAccountDetail();
			ad.AccountNO = AccountNO;
			ad.Allowance = decimal.Parse(tbAllowance.Text.Trim());
			ad.IntercityLose = decimal.Parse(tbIntercityLose.Text.Trim());
			ad.KPI = decimal.Parse(tbKPI.Text.Trim());
			ad.OtherCost = decimal.Parse(tbOtherCost.Text.Trim());
			ad.POSPrice = decimal.Parse(tbPOSPrice.Text.Trim());
			ad.StrandedPrice = decimal.Parse(tbStrandedPrice.Text.Trim());
		    ad.CollectionFee = decimal.Parse(tbCollectionFee.Text.Trim());
		    ad.DeliveryFee = decimal.Parse(tbDeliveryFee.Text.Trim());

			if (cODAccountService.UpdateAccountFee(ad,Userid.ToString()))
			{
				RunJS("alert('修改成功');window.returnValue = 'refreshParent';window.close();");
			}
			else
			{
				Alert("修改失败");
			}
		}

		private bool JudgeInput()
		{
			if(string.IsNullOrEmpty(tbAllowance.Text.Trim()))
			{
				Alert("超区补助 不能为空");
				return false;
			}

            if (string.IsNullOrEmpty(tbKPI.Text.Trim()))
			{
				Alert("KPI考核 不能为空");
				return false;
			}

            if (string.IsNullOrEmpty(tbPOSPrice.Text.Trim()))
			{
				Alert("POS机手续费 不能为空");
				return false;
			}

            if (string.IsNullOrEmpty(tbStrandedPrice.Text.Trim()))
			{
				Alert("滞留扣款 不能为空");
				return false;
			}

            if (string.IsNullOrEmpty(tbIntercityLose.Text.Trim()))
			{
				Alert("城际丢失调账 不能为空");
				return false;
			}

            if (string.IsNullOrEmpty(tbCollectionFee.Text.Trim()))
			{
                Alert("代收手续费 不能为空");
				return false;
			}
            if (string.IsNullOrEmpty(tbDeliveryFee.Text.Trim()))
            {
                Alert("投递费 不能为空");
                return false;
            }
            if (string.IsNullOrEmpty(tbOtherCost.Text.Trim()))
            {
                Alert("其他费用 不能为空");
                return false;
            }

			if (!RegexFare(tbAllowance.Text.Trim()))
			{
				Alert("超区补助 不符合金额规则");
				return false;
			}

            if (!RegexFare(tbKPI.Text.Trim()))
			{
				Alert("KPI考核 不符合金额规则");
				return false;
			}

            if (!RegexFare(tbPOSPrice.Text.Trim()))
			{
				Alert("POS机手续费 不符合金额规则");
				return false;
			}

            if (!RegexFare(tbStrandedPrice.Text.Trim()))
			{
				Alert("滞留扣款 不符合金额规则");
				return false;
			}

            if (!RegexFare(tbIntercityLose.Text.Trim()))
			{
				Alert("城际丢失调账 不符合金额规则");
				return false;
			}
            if (!RegexFare(tbCollectionFee.Text.Trim()))
            {
                Alert("代收手续费 不符合金额规则");
                return false;
            }
            if (!RegexFare(tbDeliveryFee.Text.Trim()))
            {
                Alert("投递费 不符合金额规则");
                return false;
            }

			if (!RegexFare(tbAllowance.Text.Trim()))
			{
				Alert("其他费用 不符合金额规则");
				return false;
			}

			return true;
		}

		/// <summary>
		///  验证是否金额 -10.20、10.20、10.2
		/// </summary>
		/// <param name="fare"></param>
		/// <returns></returns>
		public static bool RegexFare(string fare)
		{
			return Regex.IsMatch(fare, @"^(-)?(\d)*(\.(\d){1,2})?$");
		}
	}
}
