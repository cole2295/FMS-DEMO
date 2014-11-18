using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util.ControlHelper;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AreaExpressLevelSearch : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
                //UCWareHouseCheckBox.DistributionCode = base.DistributionCode;
            }
            UCPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
        }

        private void InitForm()
        {
            IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
            ddlAreaType.Items.Clear();
            ddlAreaType.AppendDataBoundItems = true;
            ddlAreaType.DataSource = _statusInfoService.GetStatusInfoByTypeNo(305);
            ddlAreaType.DataTextField = "statusName";
            ddlAreaType.DataValueField = "statusNo";
            ddlAreaType.DataBind();
            ddlAreaType.Items.Insert(0, new ListItem("全部", ""));
            ddlAreaType.Items.Insert(ddlAreaType.Items.Count, new ListItem("空", "99"));
        }

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            SearchData(UCPager.CurrentPageIndex);
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            ProvinceID = UCPCASerach.ProvinceId == null ? "" : UCPCASerach.ProvinceId;
            CityID = UCPCASerach.CityId == null ? "" : UCPCASerach.CityId;
            AreaID = UCPCASerach.AreaId == null ? "" : UCPCASerach.AreaId;
            ExpressCompanyID = UCSelectStation.StationID == null ? "" : UCSelectStation.StationID == "UCSelectStation" ? "" : UCSelectStation.StationID;
            AreaType = ddlAreaType.SelectedIndex <= 0 ? "" : ddlAreaType.SelectedValue;
            WareHouse = "" ;
            MerchantID = UCMerchantSourceTV.SelectMerchantID;
            AuditStatus = ddlAudit.SelectedIndex <= 0 ? "" : ddlAudit.SelectedValue;
            SearchData(1);
        }

        private void SearchData(int currentPageIndex)
        {
            gvList.DataSource = null;
            gvList.DataBind();
            PageInfo pi = new PageInfo(100);
            UCPager.PageSize = 100;
            pi.CurrentPageIndex = currentPageIndex;
            var dal = ServiceLocator.GetService<IAreaExpressLevelService>();
            DataTable dt = dal.SearchAreaTypeList(ProvinceID, CityID, AreaID, ExpressCompanyID, AreaType, WareHouse, MerchantID, AuditStatus,base.DistributionCode, ref pi);

            if (dt == null || dt.Rows.Count <= 0)
            {
                noData.Visible = true;
                noData.Style.Add("text-align", "center");
                noData.Text = "查询无数据";
                return;
            }
            noData.Visible = false;
            UCPager.RecordCount = pi.ItemCount;
            gvList.DataSource = dt;
            gvList.DataBind();
        }

        private string ProvinceID
        {
            get { return ViewState["ProvinceID"] == null ? "" : ViewState["ProvinceID"].ToString(); }
            set { ViewState.Add("ProvinceID", value); }
        }

        private string CityID
        {
            get { return ViewState["CityID"] == null ? "" : ViewState["CityID"].ToString(); }
            set { ViewState.Add("CityID", value); }
        }

        private string AreaID
        {
            get { return ViewState["AreaID"] == null ? "" : ViewState["AreaID"].ToString(); }
            set { ViewState.Add("AreaID", value); }
        }

        private string ExpressCompanyID
        {
            get { return ViewState["ExpressCompanyID"] == null ? "" : ViewState["ExpressCompanyID"].ToString(); }
            set { ViewState.Add("ExpressCompanyID", value); }
        }

        private string AreaType
        {
            get { return ViewState["AreaType"] == null ? "" : ViewState["AreaType"].ToString(); }
            set { ViewState.Add("AreaType", value); }
        }

        private string WareHouse
        {
            get { return ViewState["WareHouse"] == null ? "" : ViewState["WareHouse"].ToString(); }
            set { ViewState.Add("WareHouse", value); }
        }

        private string MerchantID
        {
            get { return ViewState["MerchantID"] == null ? "" : ViewState["MerchantID"].ToString(); }
            set { ViewState.Add("MerchantID", value); }
        }

        private string AuditStatus
        {
            get { return ViewState["AuditStatus"] == null ? "" : ViewState["AuditStatus"].ToString(); }
            set { ViewState.Add("AuditStatus", value); }
        }

        protected void btExport_Click(object sender, EventArgs e)
        {
            ProvinceID = UCPCASerach.ProvinceId == null ? "" : UCPCASerach.ProvinceId;
            CityID = UCPCASerach.CityId == null ? "" : UCPCASerach.CityId;
            AreaID = UCPCASerach.AreaId == null ? "" : UCPCASerach.AreaId;
            ExpressCompanyID = UCSelectStation.StationID == null ? "" : UCSelectStation.StationID == "UCSelectStation" ? "" : UCSelectStation.StationID;
            AreaType = ddlAreaType.SelectedIndex <= 0 ? "" : ddlAreaType.SelectedValue;
            WareHouse = "" ;
            MerchantID = UCMerchantSourceTV.SelectMerchantID;
            AuditStatus = ddlAudit.SelectedIndex <= 0 ? "" : ddlAudit.SelectedValue;
            var dal = ServiceLocator.GetService<IAreaExpressLevelService>();
            DataTable dt = dal.SearchAreaTypeExprotList(ProvinceID, CityID, AreaID, ExpressCompanyID, AreaType, WareHouse, MerchantID, AuditStatus,base.DistributionCode);

            if (dt == null || dt.Rows.Count <= 0)
            {
                Alert("查询无数据");
                return;
            }

            List<string> columnsList = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columnsList.Add(dc.ColumnName);
            }

            ExportExcel(dt, columnsList.ToArray(), null, "支出区域类型导出");
        }

        //protected void btDelete_Click(object sender, EventArgs e)
        //{
        //    IList<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
        //    foreach (GridViewRow r in gvList.Rows)
        //    {
        //        DataKey dataKey = gvList.DataKeys[r.RowIndex];
        //        keyValuePairs.Add(new KeyValuePair<string, string>(dataKey.Value.ToString(), ""));
        //    }

        //    var areaExpressLevelBll = ServiceLocator.GetService<IAreaExpressLevel>();
        //    if (areaExpressLevelBll.DeleteAreaType(keyValuePairs, Userid.ToString()))
        //    {
        //        Alert("删除成功");
        //    }
        //    else
        //        Alert("删除失败");
        //}
    }
}