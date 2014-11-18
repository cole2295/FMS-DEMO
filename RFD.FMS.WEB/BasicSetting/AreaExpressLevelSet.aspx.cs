using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.Util.ControlHelper;
using System.IO;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Model;
using RFD.FMS.WEB.UserControl;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AreaExpressLevelSet : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                IniForm();
            }
            // 注册分页用户控件事件处理
            if (IsPostBack)
            {
                UCPager1.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
                UCPagerType.PagerPageChanged += new EventHandler(AspNetPagerType_PageChanged);
            }
            
            if (gvArea.Rows.Count <= 0)
                RegisterScript("ShowCenterRight('none');", "ShowCenterRight");
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo(this.UCPager1.PageSize);
            BindAreaData(ref pageInfo);
        }

        protected void AspNetPagerType_PageChanged(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo(this.UCPagerType.PageSize);
            BindTypeData(ref pageInfo);
        }
        private void IniForm()
        {
            BindAreaType();
            var wareHouseInfo = ServiceLocator.GetService<IWarehouseSortRelationService>();
            DataTable dtWareHouse = wareHouseInfo.GetWarehouseList();
            DataTable dtSortCenter = wareHouseInfo.GetSortationList(DistributionCode);

            //foreach (DataRow dr in dtSortCenter.Rows)
            //{
            //    DataRow drI = dtWareHouse.NewRow();
            //    drI["WarehouseId"] = "S_" + dr["ExpressCompanyID"].ToString();
            //    drI["WarehouseName"] = dr["CompanyName"].ToString();
            //    dtWareHouse.Rows.Add(drI);
            //}
            //ddlWareHouse.BindListData(dtWareHouse, "WarehouseName", "WarehouseId", "所有", "");

            var merchant = ServiceLocator.GetService<IMerchantService>();
            DataTable dtMerchant = merchant.GetAllMerchants(base.DistributionCode);
        }

        private void BindAreaType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(ddlAreaType, "请选择", "", "AreaType", base.DistributionCode);
        }

        public string AreaID
        {
            get
            {
                return ViewState["AreaID"] == null ? null : ViewState["AreaID"].ToString();
            }
            set
            {
                ViewState.Add("AreaID", value);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            PageInfo piArea=new PageInfo(20);
            UCPager1.CurrentPageIndex = 1;
            BindAreaData(ref piArea);


        }
        

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (System.Web.UI.WebControls.GridViewRow row in gvArea.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int rowIndex = row.RowIndex;
                    row.Cells[2].Attributes["onclick"] = "fnColumnOnClick(" + rowIndex + ");";
                    row.Cells[4].Attributes["onclick"] = "fnColumnOnClick(" + rowIndex + ");";
                    row.Cells[6].Attributes["onclick"] = "fnColumnOnClick(" + rowIndex + ");";
                    row.Cells[7].Attributes["onclick"] = "fnColumnOnClick(" + rowIndex + ");";
                    row.Attributes["style"] = "cursor:pointer";
                }
            }
            base.Render(writer);
        }

		protected void command(object o, CommandEventArgs e)
		{
			RegisterScript("ShowCenterRight('block');", "ShowCenterRight");
			AreaDisplay.Text = "";
			int index = int.Parse(e.CommandArgument.ToString());
			GridViewRow gvr = gvArea.Rows[index];
			DataKey key = gvArea.DataKeys[index];
			AreaID = key.Value.ToString();

			AreaDisplay.Text = string.Format("{0}->{1}->{2}", gvr.Cells[2].Text.ToString(), gvr.Cells[4].Text.ToString(), gvr.Cells[6].Text.ToString());
            PageInfo pi = new PageInfo(this.UCPagerType.PageSize);
            UCPagerType.CurrentPageIndex = 1;
            BindData(ref pi);
		}


        protected void ods_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["statusTypeNo"] = "305";
        }

        protected void btAddAreaType_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<string, string>> keyValuePairs = GridViewHelper.GetSelectedRows<string>(gvArea, "cbCheckBox", 6);
                int expressCompanyId = 0;
                if (!JudgeInput(keyValuePairs, out expressCompanyId))
                    return;

                List<AreaExpressLevel> areaExpressLevels = new List<AreaExpressLevel>();
                foreach (var keyPair in keyValuePairs)
                {
                    AreaExpressLevel areaExpressLevel = new AreaExpressLevel();
                    areaExpressLevel.DistributionCode = base.DistributionCode;
                    areaExpressLevel.AreaID = keyPair.Key;
                    areaExpressLevel.AreaName = keyPair.Value;
                    areaExpressLevel.ExpressCompanyID = expressCompanyId;
                    ExpressCompany expressCompany = new ExpressCompany();
                    expressCompany = ucSelectStation.ExpressCompany;
                    areaExpressLevel.CompanyName = expressCompany.CompanyName;
                    areaExpressLevel.AreaType = int.Parse(ddlAreaType.SelectedValue);
                    areaExpressLevel.EffectAreaType = int.Parse(ddlAreaType.SelectedValue);
                    //if (ddlWareHouse.SelectedValue == "")
                    //    areaExpressLevel.WareHouseType = 0;
                    //else if (ddlWareHouse.SelectedValue.Contains("S_"))
                    //    areaExpressLevel.WareHouseType = 2;
                    //else
                    //    areaExpressLevel.WareHouseType = 1;

                    //areaExpressLevel.WareHouseID = ddlWareHouse.SelectedValue.Contains("S_") ?
                    //    ddlWareHouse.SelectedValue.Replace("S_", "") : ddlWareHouse.SelectedValue;

                    areaExpressLevel.WareHouseType = 0;
                    areaExpressLevel.WareHouseID = "";

                    areaExpressLevel.MerchantID = int.Parse(UCMerchantSourceTV.SelectMerchantID);
                    areaExpressLevel.ProductID = 1;

                    areaExpressLevel.Enable = 3;
                    areaExpressLevel.CreateBy = base.Userid.ToString();
                    areaExpressLevel.AuditStatus = (int)AreaLevelStatus.S1;
                    areaExpressLevels.Add(areaExpressLevel);
                }
                if (areaExpressLevels.Count <= 0)
                {
                    Alert("未能找到添加元素");
                    return;
                }
                var areaExpressLevelBll = ServiceLocator.GetService<IAreaExpressLevelService>();
                string msg = string.Empty;
                if (areaExpressLevelBll.AddAreaType(areaExpressLevels, out msg))
                {
                    PageInfo pi = new PageInfo(this.UCPagerType.PageSize);
                    gvAreaType.DataSource = areaExpressLevelBll.SearchAreaType(AreaID, base.DistributionCode,ref pi);
                    gvAreaType.DataBind();
                    if (!string.IsNullOrEmpty(msg))
                        Alert("添加成功，以下为警告信息：<br>" + msg);
                    else
                        Alert("添加成功");
                }
                else
                    Alert("添加失败");
            }
            catch (Exception ex)
            {
                Alert("添加失败<br>"+ex.Message);
            }
        }

        private bool JudgeInput(IList<KeyValuePair<string, string>> keyValuePairs, out int expressCompanyId)
        {
            if (keyValuePairs.Count <= 0)
            {
                Alert("未选择区域");
                expressCompanyId = 0;
                return false;
            }

            if (ddlAreaType.SelectedIndex <= 0)
            {
                Alert("未选择区域类型");
                expressCompanyId = 0;
                return false;
            }

            if (ucSelectStation.StationID == "ucSelectStation" || ucSelectStation.StationID == "")
            {
                Alert("未选择配送商公司");
                expressCompanyId = 0;
                return false;
            }

            if (string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
            {
                Alert("未选择商家");
                expressCompanyId = 0;
                return false;
            }

            int.TryParse(ucSelectStation.StationID == "ucSelectStation" ? "" : ucSelectStation.StationID, out expressCompanyId);
            if (expressCompanyId <= 0)
            {
                Alert("配送商公司 异常");
                return false;
            }

            return true;
        }

        protected void btUpdateAreaType_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<string, string>> keyValuePairs = GridViewHelper.GetSelectedRows<string>(gvArea, "cbCheckBox", 6);
                int expressCompanyId = 0;
                if (!JudgeInput(keyValuePairs, out expressCompanyId))
                    return;

                List<AreaExpressLevel> areaExpressLevels = new List<AreaExpressLevel>();
                foreach (var keyPair in keyValuePairs)
                {
                    AreaExpressLevel areaExpressLevel = new AreaExpressLevel();
                    areaExpressLevel.DistributionCode = base.DistributionCode;
                    areaExpressLevel.AreaID = keyPair.Key;
                    areaExpressLevel.AreaName = keyPair.Value;
                    areaExpressLevel.ExpressCompanyID = expressCompanyId;
                    ExpressCompany expressCompany = new ExpressCompany();
                    expressCompany = ucSelectStation.ExpressCompany;
                    areaExpressLevel.CompanyName = expressCompany.CompanyName;
                    areaExpressLevel.EffectAreaType = int.Parse(ddlAreaType.SelectedValue);
                    //if (ddlWareHouse.SelectedValue == "")
                    //    areaExpressLevel.WareHouseType = 0;
                    //else if (ddlWareHouse.SelectedValue.Contains("S_"))
                    //    areaExpressLevel.WareHouseType = 2;
                    //else
                    //    areaExpressLevel.WareHouseType = 1;
                    //areaExpressLevel.WareHouseID = ddlWareHouse.SelectedValue.Contains("S_") ?
                    //    ddlWareHouse.SelectedValue.Replace("S_", "") : ddlWareHouse.SelectedValue;

                    areaExpressLevel.WareHouseType = 0;
                    areaExpressLevel.WareHouseID = "";

                    areaExpressLevel.MerchantID = int.Parse(UCMerchantSourceTV.SelectMerchantID);
                    areaExpressLevel.ProductID = 1;

                    areaExpressLevel.UpdateBy = base.Userid.ToString();
                    areaExpressLevel.AuditStatus = (int)AreaLevelStatus.S1;
                    areaExpressLevels.Add(areaExpressLevel);
                }
                if (areaExpressLevels.Count <= 0)
                {
                    Alert("未能找到更新元素");
                    return;
                }
                var areaExpressLevelBll = ServiceLocator.GetService<IAreaExpressLevelService>();
                string msg = string.Empty;
                if (areaExpressLevelBll.UpdateAreaType(areaExpressLevels, out msg))
                {
                    PageInfo pi = new PageInfo(this.UCPagerType.PageSize);
                    gvAreaType.DataSource = areaExpressLevelBll.SearchAreaType(AreaID, base.DistributionCode,ref pi);
                    gvAreaType.DataBind();
                    if (!string.IsNullOrEmpty(msg))
                        Alert("更新成功，以下为警告信息：<br>" + msg);
                    else
                        Alert("更新成功");
                }
                else
                    Alert("更新失败");
            }
            catch (Exception ex)
            {
                Alert("更新失败<br>"+ex.Message);
            }
        }

        protected  void btnSecondSerch_Click(object sender,EventArgs e)
        {
            PageInfo piType =new PageInfo(20);
            UCPagerType.CurrentPageIndex = 1;
            BindTypeData(ref piType);
        }

        private void BindTypeData(ref PageInfo piType)
        {
            string areaType = ddlAreaType.SelectedValue;
            string stationID = "";
            string merchantID = "";
            if (!string.IsNullOrEmpty(ucSelectStation.StationName))
            {
                stationID = ucSelectStation.StationID == "ucSelectStation" ? "" : ucSelectStation.StationID;
            }
            if (!string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantName))
            {
                merchantID = UCMerchantSourceTV.SelectMerchantID; ;
            }
            var areaExpressLevel = ServiceLocator.GetService<IAreaExpressLevelService>();
            piType.CurrentPageIndex = UCPagerType.CurrentPageIndex;
            DataTable dtAreaType = areaExpressLevel.SearchSecondAreaType(AreaID, areaType, stationID, merchantID, base.DistributionCode, ref piType);

            gvAreaType.DataSource = dtAreaType;
            gvAreaType.DataBind();

            //分页
            UCPagerType.PageSize = piType.PageSize;
            UCPagerType.RecordCount = piType.ItemCount;
            UCPagerType.CurrentPageIndex = piType.CurrentPageIndex;
        }

        private void BindData(ref PageInfo pi)
        {
            //分页
            pi.CurrentPageIndex = UCPagerType.CurrentPageIndex;
            string areaType = "";
            string stationID = "";
            string merchantID = "";
           var areaExpressLevel = ServiceLocator.GetService<IAreaExpressLevelService>();
           DataTable dtAreaType = areaExpressLevel.SearchSecondAreaType(AreaID, areaType, stationID, merchantID, base.DistributionCode, ref pi);
            if (dtAreaType == null || dtAreaType.Rows.Count <= 0)
                btDelete.Visible = false;
            else
                btDelete.Visible = true;
            gvAreaType.DataSource = dtAreaType;
            gvAreaType.DataBind();
            //分页
            UCPagerType.PageSize = pi.PageSize;
            UCPagerType.RecordCount = pi.ItemCount;
            UCPagerType.CurrentPageIndex = pi.CurrentPageIndex;
        }
       
        protected void btDelete_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, string>> keyValuePairs = GridViewHelper.GetSelectedRows<string>(gvAreaType, "cbTypeCheckBox", 3);
            if (keyValuePairs.Count <= 0)
            {
                Alert("没有选择要删除的区域类型");
                return;
            }

            var areaExpressLevelBll = ServiceLocator.GetService<IAreaExpressLevelService>();
            if (areaExpressLevelBll.DeleteAreaType(keyValuePairs, Userid.ToString()))
            {
                Alert("删除成功");
                PageInfo pi = new PageInfo(this.UCPagerType.PageSize);
                gvAreaType.DataSource = areaExpressLevelBll.SearchAreaType(AreaID,base.DistributionCode,ref pi);
                gvAreaType.DataBind();
            }
            else
                Alert("删除失败");
        }

        protected void btnExprotDownLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "~/UpFile/支出区域类型导入模板.xls";
                DownLoadTemplet(filePath, "支出区域类型导入模板.xls");
            }
            catch (Exception ex)
            {
                Alert("下载失败");
            }
        }

        protected void btnExprot_Click(object sender, EventArgs e)
        {
            try
            {

                if (!this.fuExprot.HasFile)
                {
                    Alert("请选择要导入的文件");
                    return;
                }
                string path = "~/file/UpFile" + fuExprot.FileName.ToString().Trim();
                path = Server.MapPath(path);
                this.fuExprot.SaveAs(path);
                DataSet ds = Excel.ExcelToDataSetFor03And07(path);
                var areaExpressLevelBll = ServiceLocator.GetService<IAreaExpressLevelService>();
                DataTable dtError;
                if (areaExpressLevelBll.ExportAreaType(ds.Tables[0], Userid, out dtError,base.DistributionCode))
                {
                    if (dtError != null && dtError.Rows.Count > 0)
                    {
                        List<string> l = new List<string>();
                        foreach (DataColumn column in dtError.Columns)
                        {
                            l.Add(column.ColumnName);
                        }
                        ExportExcel(dtError, l.ToArray(), null, "支出区域类型导入错误反馈");
                    }
                    else
                        Alert("导入完成");
                }
                else
                {
                    Alert("导入失败");
                }
            }
            catch (Exception ex)
            {
                Alert("导入失败");
            }
        }

        private void DownLoadTemplet(string filePath, string clientFileName)
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Charset = "gb2312";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(clientFileName, System.Text.Encoding.UTF8));
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
            System.Web.HttpContext.Current.Response.BinaryWrite(File.ReadAllBytes(Server.MapPath(filePath)));
        }
        private void BindAreaData(ref PageInfo pi)
        {
            pi.CurrentPageIndex = UCPager1.CurrentPageIndex;
            RegisterScript("ShowCenterRight('none');", "ShowCenterRight");
            var areaExpressLevel = ServiceLocator.GetService<IAreaExpressLevelService>();
            string stationId = ucSelectStationTmp.StationID == "ucSelectStationTmp" ? "" : ucSelectStationTmp.StationID;
            string merchantId = UCMerchantSourceTVTmp.SelectMerchantID;
            DataTable dataTable = areaExpressLevel.SearchArea(ucPCASerach.ProvinceId,
                                                            ucPCASerach.CityId, ucPCASerach.AreaId,
                                                            stationId, merchantId, base.DistributionCode,ref pi);
            gvArea.DataSource = dataTable;
            gvArea.DataBind();

            UCPager1.PageSize = pi.PageSize;
            UCPager1.RecordCount = pi.ItemCount;
            UCPager1.CurrentPageIndex = pi.CurrentPageIndex;
        }
    }
}