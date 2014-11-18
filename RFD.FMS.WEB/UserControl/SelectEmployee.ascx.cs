using System;

namespace RFD.FMS.WEB.UserControl
{
    public partial class SelectEmployee : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string ID
        {
            get { return HidUserControlEmployeeID.Value; }
            set { HidUserControlEmployeeID.Value = value; }
        }

        public string Name
        {
            get { return TxtUserControlEmployeeCode.Value; }
            set { TxtUserControlEmployeeCode.Value = value; }
        }
    }
}