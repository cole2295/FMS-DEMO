using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFD.FMS.WEB.UserControl
{
    public partial class MerchantSourceTV : System.Web.UI.UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 商家ID
        /// </summary>
        public string SelectMerchantID
        {
            get { return HidUserControlStationID.Value.Replace(" ",""); }
            set { HidUserControlStationID.Value = value; }
        }

        /// <summary>
        /// 站点、配送商名称
        /// </summary>
        public string SelectMerchantName
        {
            get { return TxtUserControlSatationSpell.Value; }
            set { TxtUserControlSatationSpell.Value = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Editable
        {
            set
            {
                imgMain.Disabled = !value;
                TxtUserControlSatationSpell.Disabled = !value;
            }
        }

        public void Reset()
        {
            TxtUserControlSatationSpell.Value = "";
            HidUserControlStationID.Value = "";
        }

        protected MerchantLoadType _merchantLoadType;
        public MerchantLoadType MerchantLoadType
        {
            get { return _merchantLoadType; }
            set { _merchantLoadType = value; }
        }

        protected MerchantTypeSource _merchantTypeSource;
        public MerchantTypeSource MerchantTypeSource
        {
            get { return _merchantTypeSource; }
            set { _merchantTypeSource = value; }
        }

        protected MerchantShowCheckBox _merchantShowCheckBox;
        public MerchantShowCheckBox MerchantShowCheckBox
        {
            get { return _merchantShowCheckBox; }
            set { _merchantShowCheckBox = value; }
        }
    }

    public enum MerchantLoadType
    {
        /// <summary>
        /// 所有商家
        /// </summary>
        All=0,
        /// <summary>
        /// 第三方商家
        /// </summary>
        ThirdMerchant = 1,
        ThirdMerchantNoExpress = 2
    }

    public enum MerchantTypeSource
    {
        /// <summary>
        /// 内控商家_收入
        /// </summary>
        nk_Merchant = 0,
        /// <summary>
        /// 财务商家_对内
        /// </summary>
        cw_Merchant_Inside = 1,
        /// <summary>
        /// 财务商家_对外
        /// </summary>
        cw_Merchant_Foreign=2,
        /// <summary>
        /// 内控商家_支出
        /// </summary>
        nk_Merchant_Expense=3,
        /// <summary>
        /// 基础信息商家_支出
        /// </summary>
        jc_Merchant_Expense = 4,
        /// <summary>
        /// 基础信息商家_收入
        /// </summary>
        jc_Merchant_Income = 5,
    }

    public enum MerchantShowCheckBox
    {
        /// <summary>
        /// 不显示
        /// </summary>
        False= 0,
        /// <summary>
        /// 显示
        /// </summary>
        True = 1,
    }
}