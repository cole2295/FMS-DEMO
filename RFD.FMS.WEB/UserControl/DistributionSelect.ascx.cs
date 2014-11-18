using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.UserControl
{
    public partial class DistributionSelect : System.Web.UI.UserControl
    {
        private IDistributionService distributionSrv = ServiceLocator.GetService<IDistributionService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {

            Distribution model = new Distribution();
            var dt = distributionSrv.GetDistribution(model);
            DistributionSelected.DataSource = dt;
            DistributionSelected.DataBind();

        }

        public string SelectDistributionID
        {
            get
            {
                return DistributionSelected.SelectedValuesToString();
            }
        }

        public bool IsSelectAll
        {
            get
            {
                bool flag = false;
                string[] selectCities = SelectDistributionID.Split(',');
                if (selectCities.Length == DistributionSelected.Items.Count)
                {
                    flag = true;
                }

                return flag;
            }
        }

        public string SelectDistributionName
        {
            get
            {
                return DistributionSelected.SelectedLabelsToString();
            }
        }

        public bool SelectEnable
        {
            get
            {
                return DistributionSelected.Enabled;
            }
            set
            {
                DistributionSelected.Enabled = value;
            }
        }

    }
}