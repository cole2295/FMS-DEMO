using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFD.FMS.WEB.UserControl
{
    public partial class AccountPeriodTV : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 账期时间
        /// </summary>
        public string AccountDate
        {
            get { return HidAccountDate.Value; }
            set { HidAccountDate.Value = value; }
        }

        /// <summary>
        /// 账期开始日期
        /// </summary>
        public string AccountDateStart
        {
            get { return HidAccountDateStr.Value; }
            set { HidAccountDateStr.Value = value; }
        }

        /// <summary>
        /// 账期结束日期
        /// </summary>
        public string AccountDateEnd
        {
            get { return HidAccountDateEnd.Value; }
            set { HidAccountDateEnd.Value = value; }
        }

        /// <summary>
        /// 账期结算 配送公司类型 类型：1、包含,2、不包含
        /// </summary>
        public string ExpressType
        {
            get { return HidExpressType.Value; }
            set { HidExpressType.Value = value; }
        }
        /// <summary>
        /// 账期结算附件配送公司
        /// </summary>
        public string SelectAccountExpressID
        {
            get { return HidAccountExpressID.Value; }
            set { HidAccountExpressID.Value = value; }
        }
        /// <summary>
        /// 账期结算对象ID
        /// </summary>
        public string SelectAccountPeriodObjectID
        {
            get { return HidAccountPeriodObjectID.Value; }
            set { HidAccountPeriodObjectID.Value = value; }
        }

        /// <summary>
        /// 账期ID
        /// </summary>
        public string SelectAccountPeriodID
        {
            get { return HidAccountPeriodID.Value; }
            set { HidAccountPeriodID.Value = value; }
        }

        /// <summary>
        /// 账期名称+结算对象名称
        /// </summary>
        public string SelectAccountPeriodName
        {
            get { return txtAccountPeriodName.Value; }
            set { txtAccountPeriodName.Value = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Editable
        {
            set
            {
                imgMain.Disabled = !value;
                txtAccountPeriodName.Disabled = !value;
            }
        }

        public void Reset()
        {
            txtAccountPeriodName.Value = "";
            HidAccountPeriodID.Value = "";
        }

        protected PeriodDataSource _periodDataSource;
        public PeriodDataSource PeriodDataSource
        {
            get { return _periodDataSource; }
            set { _periodDataSource = value; }
        }

        protected PeriodLoadType _periodLoadType;
        public PeriodLoadType PeriodLoadType
        {
            get { return _periodLoadType; }
            set { _periodLoadType = value; }
        }
    }

    public enum PeriodDataSource
    {
        /// <summary>
        /// 第三方配送公司
        /// </summary>
        Express_All = 0,
        /// <summary>
        /// 第三方配送公司
        /// </summary>
        Express_Third = 1,
        /// <summary>
        /// 站点
        /// </summary>
        Express_RFDSite = 2,
        /// <summary>
        /// 分拣
        /// </summary>
        Express_RFDSortCenter = 3,
        /// <summary>
        /// 所有商家
        /// </summary>
        Merchant_All = 4,
        /// <summary>
        /// 第三方商家
        /// </summary>
        Merchant_Third = 5,
        /// <summary>
        /// 第三方商家--去除快递
        /// </summary>
        Merchant_ThirdNoExpress = 6
    }

    public enum PeriodLoadType
    {
        /// <summary>
        /// 财务商家账期
        /// </summary>
        zq_cw_Merchant_In=1,
        /// <summary>
        /// 内控商家账期
        /// </summary>
        zq_nk_Merchant_In=2
    }
}