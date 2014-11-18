using System;

namespace RFD.FMS.WEB.UserControl
{
    public partial class SelectStation : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

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
			get { return HidUserControlStationID.Value; }
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
    }
}