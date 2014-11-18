using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AreaExpressLevelIncomeAudit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMerchant();
                BindAreaType();
            }
        }

        /// <summary>
        /// 绑定商家列表
        /// </summary>
        public void BindMerchant()
        {
            drpMerchant.Items.Clear();
            var merchant = ServiceLocator.GetService<IMerchantService>();
            DataTable dtMerchant = merchant.GetAllMerchants(base.DistributionCode);
            drpMerchant.BindListData(dtMerchant, "MerchantName", "ID", "所有", "");
        }

        protected void BindAreaType()
        {
            IStatusCodeService statusCodeService = ServiceLocator.GetService<IStatusCodeService>();
            statusCodeService.BindDropDownListByCodeType(drpAreaType, "请选择", "", "AreaType", base.DistributionCode);
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


            int areatype = 0;
            if (!string.IsNullOrEmpty(drpAreaType.SelectedValue))
            {
                areatype = int.Parse(drpAreaType.SelectedValue);
            }

            int merchantId = 0;
            if (!string.IsNullOrEmpty(this.drpMerchant.SelectedValue))
            {
                merchantId = int.Parse(this.drpMerchant.SelectedValue);
            }

            int expresscompanyid = 0;
            if (!string.IsNullOrEmpty(SelectStationCommon.StationID) && SelectStationCommon.StationID != "SelectStationCommon")
            {
                expresscompanyid = int.Parse(SelectStationCommon.StationID);
            }

            //分页
            pi.CurrentPageIndex = Pager.CurrentPageIndex;
            pi.PageSize = 100;

            //查询
            var areaLevelIncome = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            DataTable dataTable = areaLevelIncome.SearchAreaMerchantLevel(status, areaid, cityid, provinceid, merchantId, areatype, expresscompanyid,base.DistributionCode, ref pi);

            gvAreaExpressIncomeLevel.DataSource = dataTable;
            gvAreaExpressIncomeLevel.DataBind();

            //分页
            Pager.PageSize = pi.PageSize;
            Pager.RecordCount = pi.ItemCount;
            Pager.CurrentPageIndex = pi.CurrentPageIndex;


            gvAreaExpressIncomeLevelDetail.DataSource = null;
            gvAreaExpressIncomeLevelDetail.DataBind();
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo(this.Pager.PageSize);

            BindData(ref pageInfo);

            gvAreaExpressIncomeLevelDetail.DataSource = null;
            gvAreaExpressIncomeLevelDetail.DataBind();


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
            AreaExpressLevelIncomeLog operateLog = new AreaExpressLevelIncomeLog();
            operateLog.CreateBy = base.Userid;
            operateLog.CreateTime = DateTime.Now;
            operateLog.DistributionCode = base.DistributionCode;
            int auditstatus = 0;
            auditstatus = int.Parse(drpStatus.SelectedValue);

            var areaExLevelIncomeSet = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            //bool IsAudit = areaExLevelIncomeSet.SetAreaMerchantLeverAudit(areas, doDate, operateLog);

            bool IsAudit = areaExLevelIncomeSet.SetAreaMerchantLeverAuditEx(areas, doDate, auditstatus, operateLog);

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

        protected void gvAreaExpressIncomeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string areaid = gvAreaExpressIncomeLevel.SelectedRow.Cells[7].Text;
            int status = int.Parse(gvAreaExpressIncomeLevel.SelectedRow.Cells[8].Text);

            int areatype = 0;
            if (!string.IsNullOrEmpty(drpAreaType.SelectedValue))
            {
                areatype = int.Parse(drpAreaType.SelectedValue);
            }

            int merchantId = 0;
            if (!string.IsNullOrEmpty(this.drpMerchant.SelectedValue))
            {
                merchantId = int.Parse(this.drpMerchant.SelectedValue);
            }

            int expresscompanyid = 0;
            if (!string.IsNullOrEmpty(SelectStationCommon.StationID) && SelectStationCommon.StationID != "SelectStationCommon")
            {
                expresscompanyid = int.Parse(SelectStationCommon.StationID);
            }

            var areaExLevelIncome = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            DataTable dataTableDetail = areaExLevelIncome.SearchAreaMerchantLevelDetail(areaid, status, merchantId, areatype, expresscompanyid,base.DistributionCode);

            gvAreaExpressIncomeLevelDetail.DataSource = dataTableDetail;
            gvAreaExpressIncomeLevelDetail.DataBind();
        }

        protected void gvAreaExpressIncomeLevel_RowDataBound(object sender, GridViewRowEventArgs e)
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

        //置回
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
            AreaExpressLevelIncomeLog operateLog = new AreaExpressLevelIncomeLog();
            operateLog.CreateBy = base.Userid;
            operateLog.CreateTime = DateTime.Now;
            operateLog.DistributionCode = base.DistributionCode;

            var areaExLevelIncomeSet = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            bool IsAudit = areaExLevelIncomeSet.ReSetAreaMerchantLevel(areas, operateLog);

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
    }
}