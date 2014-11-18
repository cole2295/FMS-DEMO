using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using System.Text.RegularExpressions;
using RFD.FMS.MODEL;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AccountFeeEdit : BasePage
	{
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                    if (string.IsNullOrEmpty(AccountNo))
                    {
                        Alert("没有找到结算明细号");
                        return;
                    }

                    IncomeAccountDetail i = incomeAccountService.GetAccountDetailByAccountNo(AccountNo);
                    if (i == null)
                    {

                        txtReceiveFee.Text = "0.00";
                        txtReceivePOSFee.Text = "0.00";
                        txtProtectedFee.Text = "0.00";
                        txtOverAreaSubsidy.Text = "0.00";
                        txtKPI.Text = "0.00";
                        txtLostDeduction.Text = "0.00";
                        txtResortDeduction.Text = "0.00";
                        txtOtherFee.Text = "0.00";
                        txtDeliveryFee.Text = "0.00";
                        txtDiscountFee.Text = "0.00";
                    }
                    else
                    {
                        txtReceiveFee.Text = i.ReceiveFee.ToString();
                        txtReceivePOSFee.Text = i.ReceivePOSFee.ToString();
                        txtProtectedFee.Text = i.ProtectedFee.ToString();
                        txtOverAreaSubsidy.Text = i.OverAreaSubsidy.ToString();
                        txtKPI.Text = i.KPI.ToString();
                        txtLostDeduction.Text = i.LostDeduction.ToString();
                        txtResortDeduction.Text = i.ResortDeduction.ToString();
                        txtOtherFee.Text = i.OtherFee.ToString();
                        txtDeliveryFee.Text = i.DeliveryFee.ToString(CultureInfo.InvariantCulture);
                        txtDiscountFee.Text = i.DiscountFee.ToString(CultureInfo.InvariantCulture);
                    }
            }
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		public string DetailID
		{
			get { return string.IsNullOrEmpty(Request.QueryString["detailId"]) ? null : Request.QueryString["detailId"].ToString(); }
		}

        public string AccountNo
        {
            get { return string.IsNullOrEmpty(Request.QueryString["AccountNo"]) ? null : Request.QueryString["AccountNo"].ToString(); }
        }

        //public string flag
        //{
        //    get { return string.IsNullOrEmpty(Request.QueryString["flag"]) ? null : Request.QueryString["flag"].ToString(); }
        //}

		protected void btOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtOtherFee.Text.Trim()))
			{
				Alert("其他费用 不能为空");
				return;
			}

			if (!RegexFare(txtOtherFee.Text.Trim()))
			{
				Alert("其他费用 不符合金额规则");
				return;
			}

            if(string.IsNullOrEmpty(txtReceiveFee.Text.Trim()))
            {
                Alert("代收货款手续费 不能为空");
                return;
            }

            if (!RegexFare(txtReceiveFee.Text.Trim()))
            {
                Alert("代收货款手续费 不符合金额规则");
                return;
            }

            if (string.IsNullOrEmpty(txtReceivePOSFee.Text.Trim()))
            {
                Alert("POS服务费 不能为空");
                return;
            }

            if (!RegexFare(txtReceivePOSFee.Text.Trim()))
            {
                Alert("POS服务费 不符合金额规则");
                return;
            }

            if (string.IsNullOrEmpty(txtProtectedFee.Text.Trim()))
            {
                Alert("保价费 不能为空");
                return;
            }

            if (!RegexFare(txtProtectedFee.Text.Trim()))
            {
                Alert("保价费 不符合金额规则");
                return;
            }

            if (string.IsNullOrEmpty(txtOverAreaSubsidy.Text.Trim()))
            {
                Alert("超区补助 不能为空");
                return;
            }

            if (!RegexFare(txtOverAreaSubsidy.Text.Trim()))
            {
                Alert("超区补助 不符合金额规则");
                return;
            }

            if (string.IsNullOrEmpty(txtKPI.Text.Trim()))
            {
                Alert("KPI考核 不能为空");
                return;
            }

            if (!RegexFare(txtKPI.Text.Trim()))
            {
                Alert("KPI考核 不符合金额规则");
                return;
            }

            if (string.IsNullOrEmpty(txtLostDeduction.Text.Trim()))
            {
                Alert("丢失扣款 不能为空");
                return;
            }

            if (!RegexFare(txtLostDeduction.Text.Trim()))
            {
                Alert("丢失扣款 不符合金额规则");
                return;
            }

            if (string.IsNullOrEmpty(txtResortDeduction.Text.Trim()))
            {
                Alert("滞留扣款 不能为空");
                return;
            }

            if (!RegexFare(txtResortDeduction.Text.Trim()))
            {
                Alert("滞留扣款 不符合金额规则");
                return;
            }
		    if (!RegexFare(txtDeliveryFee.Text.Trim()))
		    {
                Alert("提货费 不符合金额规则");
                return; 
		    }
            if (!RegexFare(txtDiscountFee.Text.Trim()))
            {
                Alert("折扣 不符合金额规则");
                return;
            }

		    IncomeAccountDetail i = new IncomeAccountDetail();
			i.DetailID = DetailID;
			i.OtherFee = Decimal.Parse(txtOtherFee.Text.Trim());
			i.UpdateBy = Userid;
		    i.AccountNO = AccountNo;
		    i.ReceiveFee = Decimal.Parse(txtReceiveFee.Text.Trim());
		    i.ReceivePOSFee = Decimal.Parse(txtReceivePOSFee.Text.Trim());
		    i.ProtectedFee = Decimal.Parse(txtProtectedFee.Text.Trim());
            i.OverAreaSubsidy = Decimal.Parse(txtOverAreaSubsidy.Text.Trim());
		    i.KPI = Decimal.Parse(txtKPI.Text.Trim());
		    i.LostDeduction = Decimal.Parse(txtLostDeduction.Text.Trim());
		    i.ResortDeduction = Decimal.Parse(txtResortDeduction.Text.Trim());
		    i.DeliveryFee = Decimal.Parse(txtDeliveryFee.Text.Trim());
		    i.DiscountFee = Decimal.Parse(txtDiscountFee.Text.Trim());
            if(incomeAccountService.UpdateAccountFeeByAccountNo(i))
			{
				RunJS("alert('修改成功');window.returnValue = 'refreshParent';window.close();");
			}
			else
			{
				Alert("修改失败");
			}
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
