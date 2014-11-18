using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Model;

namespace RFD.FMS.WEB.UserControl
{
    public partial class PCASerach : System.Web.UI.UserControl
    {
        /// <summary>
        /// 省份ID
        /// </summary>
        public string ProvinceId { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// 地区ID
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 省份Name 
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市Name 
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 地区Name 
        /// </summary>
        public string AreaName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BuidProvinceCityArea();
                InitControl();
            }
            SetValue();
        }

        /// <summary>
        /// 绑定省市区
        /// </summary>
        private void BuidProvinceCityArea()
        {
            BuidProvince();
            if (!string.IsNullOrEmpty(ProvinceId))
            {
                DrpProvince.Items.FindByValue(ProvinceId).Selected = true;
            }
            var city = new City { ProvinceID = DrpProvince.SelectedValue };
            BuidCity(city);
            if (!string.IsNullOrEmpty(CityId))
            {
                DrpCity.Items.FindByValue(CityId).Selected = true;
            }
            var area = new Area { CityID = DrpCity.SelectedValue };
            BuidArea(area);
            if (!string.IsNullOrEmpty(AreaId))
            {
                DrpArea.Items.FindByValue(AreaId).Selected = true;
            }
        }

        /// <summary>
        /// 绑定省份
        /// </summary>
        private void BuidProvince()
        {
            var province = ServiceLocator.GetService<IProvince>();
            DataTable dataTable = province.GetProvinceList();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                //if (!string.IsNullOrEmpty(GetProvincePermission()))
                //{
                //    string strSql = string.Format("provinceID in ({0})", GetProvincePermission());
                //    dataTable.DefaultView.RowFilter = strSql;
                //}

                DrpProvince.Items.Clear();
                DrpProvince.DataSource = dataTable.DefaultView;
                DrpProvince.DataValueField = "provinceID";
                DrpProvince.DataTextField = "provinceName";
                DrpProvince.AppendDataBoundItems = true;
                DrpProvince.Items.Insert(0, new ListItem("全部", ""));
                DrpProvince.DataBind();
            }
            //-加入权限控制的加载
            //var service = ServiceLocator.GetService<IPermissionProvider>();
            //Dictionary<string, string> dic = service.LoadDataPermissionDic(base.UserCode, "1", "StationID", 2);//加载省级别
            //DrpProvince.Items.Clear();
            //DrpProvince.DataSource = dic;
            //DrpProvince.DataValueField = "key";
            //DrpProvince.DataTextField = "value";
            //DrpProvince.AppendDataBoundItems = true;
            //DrpProvince.Items.Insert(0, new ListItem("全部", ""));
            //DrpProvince.DataBind();
        }

        /// <summary>
        /// 绑定城市
        /// </summary>
        /// <param name="city"></param>
        private void BuidCity(City city)
        {
            DrpCity.Items.Clear();
            if (!string.IsNullOrEmpty(city.ProvinceID))
            {
                var cityService = ServiceLocator.GetService<ICityService>();
                DataTable dataTable = cityService.GetCityList(city);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    DrpCity.DataSource = dataTable;
                    DrpCity.Items.Clear();
                    DrpCity.DataSource = dataTable.DefaultView;
                    DrpCity.DataValueField = "CityID";
                    DrpCity.DataTextField = "CityName";
                }
            }
            DrpCity.AppendDataBoundItems = true;
            DrpCity.Items.Insert(0, new ListItem("全部", ""));
            DrpCity.DataBind();
            //-加权限控制
            //var service = ServiceLocator.GetService<IPermissionProvider>();
            //Dictionary<string, string> dic = service.LoadDataPermissionDicByParentId(base.UserCode, "1", "StationID", this.DrpProvince.SelectedValue);//加载省级别
            //DrpCity.Items.Clear();
            //DrpCity.DataSource = dic;
            //DrpCity.DataValueField = "key";
            //DrpCity.DataTextField = "value";
            //DrpCity.AppendDataBoundItems = true;
            //DrpCity.Items.Insert(0, new ListItem("全部", ""));
            //DrpCity.DataBind();
        }

        /// <summary>
        /// 绑定区县
        /// </summary>
        /// <param name="area"></param>
        private void BuidArea(Area area)
        {
            DrpArea.Items.Clear();
            if (!string.IsNullOrEmpty(area.CityID))
            {
                var aeraService = ServiceLocator.GetService<IAreaService>();
                DataTable dataTable = aeraService.GetAreaList(area);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    DrpArea.DataSource = dataTable;
                    DrpArea.DataValueField = "AreaID";
                    DrpArea.DataTextField = "AreaName";
                }
            }
            DrpArea.AppendDataBoundItems = true;
            DrpArea.Items.Insert(0, new ListItem("全部", ""));
            DrpArea.DataBind();
        }

        /// <summary>
        /// 给属性赋值
        /// </summary>
        private void SetValue()
        {
            //省份ID
            ProvinceId = DrpProvince.SelectedValue;
            //城市ID
            CityId = DrpCity.SelectedValue;
            //地区ID
            AreaId = DrpArea.SelectedValue;

            //省份ID
            ProvinceName = DrpProvince.SelectedItem == null ? "" : DrpProvince.SelectedItem.Text;
            //城市ID
            CityName = DrpCity.SelectedItem == null ? "" : DrpCity.SelectedItem.Text;
            //地区ID
            AreaName = DrpArea.SelectedItem == null ? "" : DrpArea.SelectedItem.Text;
        }


        /// <summary>
        /// 选择省份的时候重新绑定城市;同时重新给属性赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DrpProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            var city = new City { ProvinceID = DrpProvince.SelectedValue };
            BuidCity(city);
            var area = new Area { CityID = DrpCity.SelectedValue };
            BuidArea(area);
            SetValue(); //重新赋值
        }

        /// <summary>
        /// 选择城市的时候重新绑定区县;同时重新给属性赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DrpCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            var area = new Area { CityID = DrpCity.SelectedValue };
            BuidArea(area);
            SetValue(); //重新赋值
        }

        //by mazhonghua 2011-09-27
        internal void InitControl()
        {
            DrpProvince.SelectedValue = ProvinceId;
            DrpCity.SelectedValue = CityId;
            DrpArea.SelectedValue = AreaId;
        }
    }
}