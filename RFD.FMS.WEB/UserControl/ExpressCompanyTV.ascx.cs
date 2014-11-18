using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFD.FMS.WEB.UserControl
{
    public partial class ExpressCompanyTV : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 配送商ID
        /// </summary>
        public string SelectExpressID
        {
            get { return HidUserControlStationID.Value; }
            set { HidUserControlStationID.Value = value; }
        }

        /// <summary>
        /// 配送商名称
        /// </summary>
        public string SelectExpressName
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

        protected ExpressLoadType _expressLoadType;
        public ExpressLoadType ExpressLoadType
        {
            get { return _expressLoadType; }
            set { _expressLoadType = value; }
        }
        protected ExpressCompanyShowCheckBox _expressCompanyShowCheckBox;
        public ExpressCompanyShowCheckBox ExpressCompanyShowCheckBox
        {
            get { return _expressCompanyShowCheckBox; }
            set { _expressCompanyShowCheckBox = value; }
        }
        protected ExpressTypeSource _expressTypeSource;
        public ExpressTypeSource ExpressTypeSource
        {
            get { return _expressTypeSource; }
            set { _expressTypeSource = value; }
        }
    }

    public enum ExpressLoadType
    {
        /// <summary>
        /// 第三方配送公司
        /// </summary>
        ThirdCompany = 0,
        /// <summary>
        /// 站点
        /// </summary>
        RFDSite = 1,
        /// <summary>
        /// 分拣
        /// </summary>
        RFDSortCenter = 2
    }

    public enum ExpressTypeSource
    {
        /// <summary>
        /// 匿名
        /// </summary>
        other_Express = -1,
        /// <summary>
        /// 内控配送商
        /// </summary>
        nk_Express = 0,
        /// <summary>
        /// 财务配送商_对外
        /// </summary>
        cw_Express_Foreign = 1
    }

    public enum ExpressCompanyShowCheckBox
    {
        /// <summary>
        /// 不显示
        /// </summary>
        False = 0,
        /// <summary>
        /// 显示
        /// </summary>
        True = 1,
    }
}