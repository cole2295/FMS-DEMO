using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using LMS.Model;
using RFD.FMS.Model;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.UserControl
{
    public partial class CitySelected : System.Web.UI.UserControl
    {
        private ICityService citySrv = ServiceLocator.GetService<ICityService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
               LoadData();
            }
           
        }

        private void LoadData()
        {
            if (CitySelect.Items.Count <= 0)
            {
                var dt = citySrv.GetCityList(new City(){Level = "1"});
                CitySelect.DataSource = dt;
                CitySelect.DataBind();
            }
        }

        public string SelectCityID
        {
           set 
           {
               string id = value;
               CitySelect.Items.Clear();
               LoadData();
               foreach (ListItem item in CitySelect.Items )
               {
                  if(id == item.Value)
                  {
                      item.Selected = true;
                  }
               }
           }
            
            get
            {
                return CitySelect.SelectedValuesToString().Replace(" ","");
            }
        }

        public bool IsSelectAll
        {
            get
            {
                bool flag = false;
                string[] selectCities = SelectCityID.Split(',');
                if (selectCities.Length == CitySelect.Items.Count)
                {
                    flag = true;
                }

                return flag;
            }
        }

        public string SelectCityName
        {
            get
            {
                return CitySelect.SelectedLabelsToString();
            }
        }

        public bool SelectEnable
        {
            get
            {
                return CitySelect.Enabled;
            }
            set
            {
                CitySelect.Enabled = value;
            }
        }

          
    }
}