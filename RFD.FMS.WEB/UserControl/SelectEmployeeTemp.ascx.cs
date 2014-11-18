using System;

namespace RFD.FMS.WEB.UserControl
{
    public partial class SelectEmployeeTemp : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string UserID
        {
            get { return HidUserControlEmployeeIDTemp.Value; }
            set { HidUserControlEmployeeIDTemp.Value = value; }
        }

        public string Name
        {
            get { return TxtUserControlEmployeeCodeTemp.Value; }
            set { TxtUserControlEmployeeCodeTemp.Value = value; }
        }
    }
}