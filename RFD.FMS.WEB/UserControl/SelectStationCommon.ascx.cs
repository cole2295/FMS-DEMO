using System;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Model;

namespace RFD.FMS.WEB.UserControl
{
	public partial class SelectStationCommon : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private readonly IExpressCompanyService iec = ServiceLocator.GetService<IExpressCompanyService>();
        public string ID
        {
            get { return HidUserControlStationID.Value; }
            set { HidUserControlStationID.Value = value; }
        }

        public string Name
        {
            get { return TxtUserControlSatationSpell.Value; }
            set { TxtUserControlSatationSpell.Value = value; }
        }

		/// <summary>
		/// 站点、配送商ID
		/// </summary>
		public string StationID
		{
            //get { return HidUserControlStationID.Value; }
            get { return this.HidUserControlStationID.Value.Trim(); }
			set { HidUserControlStationID.Value = value; }
		}

		/// <summary>
		/// 站点、配送商名称
		/// </summary>
		public string StationName
		{
			get { return TxtUserControlSatationSpell.Value; }
			set { TxtUserControlSatationSpell.Value = value; }
		}

        public bool IsMustInput
        {
            set 
            {
                //spanMustInput.Visible = value;
            }
        }

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

        public ExpressCompany ExpressCompany
        {
            get
            {
                try
                {
                    return iec.GetModel(int.Parse(this.HidUserControlStationID.Value.Trim()));
                }
                catch
                {
                    return null;
                }
            }
        }

		protected EnumLoadCompanyType _loadDataType;
		public EnumLoadCompanyType LoadDataType
		{
			get { return _loadDataType; }
			set { _loadDataType = value; }
		}
    }
}