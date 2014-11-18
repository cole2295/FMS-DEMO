using System;

namespace RFD.FMS.WEB.UserControl
{
    public partial class SelectStationTmp : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string ID
        {
            get { return HidUserControlStationIDTmp.Value; }
            set { HidUserControlStationIDTmp.Value = value; }
        }

        public string Name
        {
            get { return TxtUserControlSatationSpellTmp.Value; }
            set { TxtUserControlSatationSpellTmp.Value = value; }
        }

        public bool Editable
        {
            set 
            {
                imgTemp.Disabled = !value;
                TxtUserControlSatationSpellTmp.Disabled = !value;
            }
        }

        public bool IsMustInput
        {
            set
            {
                spanMustInput.Visible = value;
            }
        }

        public void Reset()
        {
            TxtUserControlSatationSpellTmp.Value = "";
            HidUserControlStationIDTmp.Value = "";
        }
    }
}