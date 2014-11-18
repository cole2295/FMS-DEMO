using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AreaExpressLevelAudit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindAreaType();
                //BindWareHouseExpressCompany();
            }

            // 注册分页用户控件事件处理
            if (IsPostBack)
            {
                Pager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
            }
        }

        protected void BindAreaType()
        {
            IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
            drpAreaType.Items.Clear();
            drpAreaType.AppendDataBoundItems = true;
            drpAreaType.DataSource = _statusInfoService.GetStatusInfoByTypeNo(305);
            drpAreaType.DataTextField = "statusName";
            drpAreaType.DataValueField = "statusNo";
            drpAreaType.DataBind();
            drpAreaType.Items.Insert(0, new ListItem("全部", ""));
            drpAreaType.Items.Insert(drpAreaType.Items.Count, new ListItem("空", "99"));
        }

        protected void BindData(ref PageInfo pi)
        {
            int status = int.Parse(this.drpStatus.SelectedValue);

            string areaid = null;
            if (!string.IsNullOrEmpty(this.PCASerach.AreaId))
            {
                areaid = this.PCASerach.AreaId;
            }

            string cityid = null;
            if (!string.IsNullOrEmpty(this.PCASerach.CityId))
            {
                cityid = this.PCASerach.CityId;
            }

            string provinceid = null;
            if (!string.IsNullOrEmpty(this.PCASerach.ProvinceId))
            {
                provinceid = this.PCASerach.ProvinceId;
            }

            int expresscompanyid = 0;
            if (!string.IsNullOrEmpty(SelectStationCommon.StationID) && SelectStationCommon.StationID != "SelectStationCommon")
            {
                expresscompanyid = int.Parse(SelectStationCommon.StationID);
            }

            int areatype = 0;
            if (!string.IsNullOrEmpty(drpAreaType.SelectedValue))
            {
                areatype = int.Parse(drpAreaType.SelectedValue);
            }

            int type = 0;
            //if (!string.IsNullOrEmpty(drpType.SelectedValue))
            //{
            //    type = int.Parse(drpType.SelectedValue);
            //}

            string warehouseid = null;
            //if (!string.IsNullOrEmpty(drpWarehouseExpressCompany.SelectedValue) && type != 0)
            //{
            //    warehouseid = drpWarehouseExpressCompany.SelectedValue;
            //}

            int merchantId = 0;
            if (!string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
            {
                merchantId = int.Parse(UCMerchantSourceTV.SelectMerchantID);
            }

            //分页
            pi.CurrentPageIndex = Pager.CurrentPageIndex;
            pi.PageSize = 100;

            //查询
            var area = ServiceLocator.GetService<IAreaExpressLevelService>();
            DataTable dataTable = area.SearchAreaExpressCompanyLevel(status, areaid, cityid, provinceid, expresscompanyid, areatype, type, warehouseid, merchantId,base.DistributionCode, ref pi);

            gvAreaExpressLevel.DataSource = dataTable;
            gvAreaExpressLevel.DataBind();

            //分页
            Pager.PageSize = pi.PageSize;
            Pager.RecordCount = pi.ItemCount;
            Pager.CurrentPageIndex = pi.CurrentPageIndex;

        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo(this.Pager.PageSize);

            BindData(ref pageInfo);

            gvAreaExpressLevelDetail.DataSource = null;
            gvAreaExpressLevelDetail.DataBind();
        }

        protected void gvAreaExpressLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string areaid = gvAreaExpressLevel.SelectedRow.Cells[7].Text;
            int status = int.Parse(gvAreaExpressLevel.SelectedRow.Cells[8].Text);

            int stationid = 0;
            if (!string.IsNullOrEmpty(SelectStationCommon.StationID) && SelectStationCommon.StationID != "SelectStationCommon")
            {
                stationid = int.Parse(SelectStationCommon.StationID);
            }

            int areatype1 = 0;
            if (!string.IsNullOrEmpty(drpAreaType.SelectedValue))
            {
                areatype1 = int.Parse(drpAreaType.SelectedValue);
            }

            int warehosetype = 0;
            //if (!string.IsNullOrEmpty(drpType.SelectedValue))
            //{
            //    warehosetype = int.Parse(drpType.SelectedValue);
            //}

            string warehousecode = null;
            //if (!string.IsNullOrEmpty(drpWarehouseExpressCompany.SelectedValue) && warehosetype != 0)
            //{
            //    warehousecode = drpWarehouseExpressCompany.SelectedValue;
            //}

            int merchant = 0;
            if (!string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
            {
                merchant = int.Parse(UCMerchantSourceTV.SelectMerchantID);
            }

            var areaExLevel = ServiceLocator.GetService<IAreaExpressLevelService>();
            DataTable dataTableDetail = areaExLevel.SearchAreaExpressCompanyLevelDetail(areaid, status, stationid, areatype1, warehosetype, warehousecode, merchant);

            gvAreaExpressLevelDetail.DataSource = dataTableDetail;
            gvAreaExpressLevelDetail.DataBind();

        }

        protected void gvAreaExpressLevel_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[0].Attributes.Add("style", "display:block");
                if (drpStatus.SelectedValue == "0")
                {
                    //e.Row.Cells[0].Attributes.Add("style", "display:block");
                    e.Row.Cells[4].Attributes.Add("style", "display:none");
                    e.Row.Cells[5].Attributes.Add("style", "display:none");
                }
                else if (drpStatus.SelectedValue == "3")
                {
                    //e.Row.Cells[0].Attributes.Add("style", "display:block");
                    e.Row.Cells[4].Attributes.Add("style", "display:none");
                    e.Row.Cells[5].Attributes.Add("style", "display:none");
                }
                else if (drpStatus.SelectedValue == "1")
                {
                    e.Row.Cells[9].Attributes.Add("style", "display:block");
                    //e.Row.Cells[0].Attributes.Add("style", "display:none");
                }
                else
                {
                    //e.Row.Cells[0].Attributes.Add("style", "display:none");
                }
            }
        }

        protected void BtnAudit_Click(object sender, EventArgs e)
        {
            DateTime doDate = System.DateTime.MinValue;

            if (!DateTime.TryParse(this.txtDoDate.Text.Trim(), out doDate))
            {
                Alert("生效时间不正确，请重新输入！");
                return;
            }


            if (doDate.Date <= System.DateTime.Now.Date)
            {
                Alert("生效时间必须大于今天！");
                return;
            }

            //是否选择了订单
            string areaChecked = Request.Form["cbChecked"];

            if (string.IsNullOrEmpty(areaChecked))
            {
                Alert("请先选择需要审批的区域！");
                return;
            }

            string areas = Request.Form["cbChecked"];
            areas = string.Format("{0},", areas);
            areas = areas.Substring(0, areas.Length - 1);

            //操作日志
            AreaExpressLevelLog operateLog = new AreaExpressLevelLog();
            operateLog.CreateBy = base.Userid.ToString();
            operateLog.CreateTime = DateTime.Now;
            operateLog.DistributionCode = base.DistributionCode;
            int auditstatus = 0;
            auditstatus = int.Parse(drpStatus.SelectedValue);

            var areaExLevelSet = ServiceLocator.GetService<IAreaExpressLevelService>();
            //bool IsAudit = areaExLevelSet.SetAreaExpressCompanyLeverAudit(areas, doDate, operateLog);

            bool IsAudit = areaExLevelSet.SetAreaExpressCompanyLeverAuditEx(areas, doDate, auditstatus, operateLog);

            if (IsAudit)
            {
                Alert("操作成功");
            }
            else
            {
                Alert("操作失败");
            }

            PageInfo pageInfo = new PageInfo(this.Pager.PageSize);

            BindData(ref pageInfo);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo(this.Pager.PageSize);

            BindData(ref pageInfo);
        }
        //设置置回
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            //是否选择了订单
            string areaChecked = Request.Form["cbChecked"];

            if (string.IsNullOrEmpty(areaChecked))
            {
                Alert("请先选择需要置回的区域！");
                return;
            }

            string areas = Request.Form["cbChecked"];
            areas = string.Format("{0},", areas);
            areas = areas.Substring(0, areas.Length - 1);

            //操作日志
            AreaExpressLevelLog operateLog = new AreaExpressLevelLog();
            operateLog.CreateBy = base.Userid.ToString();
            operateLog.CreateTime = DateTime.Now;
            operateLog.DistributionCode = base.DistributionCode;

            var areaExLevelSet = ServiceLocator.GetService<IAreaExpressLevelService>();
            bool IsAudit = areaExLevelSet.ReSetAreaExpressCompanyLevel(areas, operateLog);

            if (IsAudit)
            {
                Alert("操作成功");
            }
            else
            {
                Alert("操作失败");
            }

            PageInfo pageInfo = new PageInfo(this.Pager.PageSize);

            BindData(ref pageInfo);
        }

        protected void BindWareHouseExpressCompany()
        {
            var wareHouseInfo = ServiceLocator.GetService<IWarehouseSortRelationService>();
            DataTable dtWareHouse = wareHouseInfo.GetWarehouseList();
            DataTable dtSortCenter = wareHouseInfo.GetSortationList(DistributionCode);

            foreach (DataRow dr in dtSortCenter.Rows)
            {
                DataRow drI = dtWareHouse.NewRow();
                drI["WarehouseId"] = "S_" + dr["ExpressCompanyID"].ToString();
                drI["WarehouseName"] = dr["CompanyName"].ToString();
                dtWareHouse.Rows.Add(drI);
            }

            //DropDownListHelper.DropDownListBind(drpWarehouseExpressCompany, dtWareHouse, "WarehouseName", "WarehouseId");
        }

        //protected void drpType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var wareHouse = ServiceLocator.GetService<IWarehouseSortRelationService>();

        //    DataTable dtWarehouse = null;
        //    DataTable dtSortCenter = null;
        //    drpWarehouseExpressCompany.Visible = true;
        //    if (drpType.SelectedValue == "1")
        //    {

        //        dtWarehouse = wareHouse.GetWarehouseList();
        //        DropDownListHelper.DropDownListBind(drpWarehouseExpressCompany, dtWarehouse, "WarehouseName", "WarehouseId");
        //    }
        //    else if (drpType.SelectedValue == "2")
        //    {
        //        dtSortCenter = wareHouse.GetSortationList(DistributionCode);


        //        DropDownListHelper.DropDownListBind(drpWarehouseExpressCompany, dtSortCenter, "CompanyName", "ExpressCompanyID");
        //    }
        //    else
        //    {
        //        drpWarehouseExpressCompany.Visible = false;
        //    }


        //}
    }
}