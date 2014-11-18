using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using System.IO;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Model;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AreaExpressLevelSetIncome : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
            }
            if (gvArea.Rows.Count <= 0)
                RegisterScript("ShowCenterRight('none');", "ShowCenterRight");
        }

        private void InitForm()
        {
            IExpressCompanyService expressCompany = ServiceLocator.GetService<IExpressCompanyService>();
            ExpressCompany ecModel = expressCompany.GetCompanyModelByDistributionCode(base.DistributionCode);
			if (ecModel!=null)
        	{
				ExpressCompanyID = ecModel.ExpressCompanyID;
				CompanyName = ecModel.CompanyName;
				txtExpressCompany.Text = CompanyName;
        	}
            

            IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
            ddlAreaType.Items.Clear();
            ddlAreaType.AppendDataBoundItems = true;
            ddlAreaType.DataSource = _statusInfoService.GetStatusInfoByTypeNo(305);
            ddlAreaType.DataTextField = "statusName";
            ddlAreaType.DataValueField = "statusNo";
            ddlAreaType.DataBind();
            ddlAreaType.Items.Insert(0, new ListItem("请选择", ""));

            IWareHouseService wareHouseService = ServiceLocator.GetService<IWareHouseService>();
            DataTable dtWareHouse = wareHouseService.GetSortCenter(base.DistributionCode);
            DropDownListHelper.DropDownListBind(ddlWareHouse, dtWareHouse, "WarehouseName", "WarehouseId");

            var merchant = ServiceLocator.GetService<IMerchantService>();
            DataTable dtMerchant = merchant.GetAllMerchants(base.DistributionCode);
            ddlMerchant.BindListData(dtMerchant, "MerchantName", "ID", "所有", "");
            ddlMerchant1.BindListData(dtMerchant, "MerchantName", "ID", "所有", "");
        }

        public int ExpressCompanyID
        {
            get
            {
                return ViewState["ExpressCompanyID"] == null ? 0 : ViewState["ExpressCompanyID"].ToString().TryGetInt();
            }
            set
            {
                ViewState.Add("ExpressCompanyID", value);
            }
        }

        public string CompanyName
        {
            get
            {
                return ViewState["CompanyName"] == null ? null : ViewState["CompanyName"].ToString();
            }
            set
            {
                ViewState.Add("CompanyName", value);
            }
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
            RegisterScript("ShowCenterRight('none');", "ShowCenterRight");
            var areaExpressLevelIncome = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
			//gvArea.DataSource = areaExpressLevelIncome.SearchArea(ucPCASerach.ProvinceId,
			//                                                ucPCASerach.CityId, ucPCASerach.AreaId,
			//                                                ddlMerchant.SelectedValue == "--请选择--" ? "" : ddlMerchant.SelectedValue,base.DistributionCode);
			//gvArea.DataBind();

			DataTable dataTable = areaExpressLevelIncome.SearchArea(ucPCASerach.ProvinceId,
															ucPCASerach.CityId, ucPCASerach.AreaId,
															ddlMerchant.SelectedValue == "--请选择--" ? "" : ddlMerchant.SelectedValue, base.DistributionCode);
			BindDataWithBuildPage(dataTable, UCPager, gvArea);
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

            var areaExpressLevel = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();

            DataTable dtAreaType = areaExpressLevel.SearchAreaType(AreaID,base.DistributionCode);
            if (dtAreaType == null || dtAreaType.Rows.Count <= 0)
                btDelete.Visible = false;
            else
                btDelete.Visible = true;
            gvAreaType.DataSource = dtAreaType;
            gvAreaType.DataBind();
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
                if (!JudgeInput(keyValuePairs))
                    return;

                List<AreaExpressLevelIncome> areaExpressLevelIncomes = new List<AreaExpressLevelIncome>();
                foreach (var keyPair in keyValuePairs)
                {
                    AreaExpressLevelIncome areaExpressLevelIncome = new AreaExpressLevelIncome();
                    areaExpressLevelIncome.DistributionCode = base.DistributionCode;
                    areaExpressLevelIncome.AreaID = keyPair.Key;
                    areaExpressLevelIncome.AreaName = keyPair.Value;
                    areaExpressLevelIncome.ExpressCompanyID = ExpressCompanyID;
                    areaExpressLevelIncome.CompanyName = CompanyName;
                    areaExpressLevelIncome.MerchantID = int.Parse(ddlMerchant1.SelectedValue);
                    areaExpressLevelIncome.WareHouseID = ddlWareHouse.SelectedValue;
                    areaExpressLevelIncome.MerchantName = ddlMerchant1.SelectedItem.Text;
                    areaExpressLevelIncome.AreaType = int.Parse(ddlAreaType.SelectedValue);
                    areaExpressLevelIncome.EffectAreaType = int.Parse(ddlAreaType.SelectedValue);
                    areaExpressLevelIncome.Enable = 3;
                    areaExpressLevelIncome.CreateBy = base.Userid;
                    areaExpressLevelIncome.AuditStatus = (int)AreaLevelStatus.S1;
                    areaExpressLevelIncomes.Add(areaExpressLevelIncome);
                }
                if (areaExpressLevelIncomes.Count <= 0)
                {
                    Alert("未能找到添加元素");
                    return;
                }
                var areaExpressLevelIncomeBll = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                string msg = string.Empty;
                if (areaExpressLevelIncomeBll.AddAreaType(areaExpressLevelIncomes, out msg))
                {
                    gvAreaType.DataSource = areaExpressLevelIncomeBll.SearchAreaType(AreaID, base.DistributionCode);
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
                Alert("添加失败<br>" + ex.Message);
            }
        }

        private bool JudgeInput(IList<KeyValuePair<string, string>> keyValuePairs)
        {
            if (keyValuePairs.Count <= 0)
            {
                Alert("未选择区域");
                return false;
            }

            if (ddlAreaType.SelectedValue == "")
            {
                Alert("未选择区域类型");
                return false;
            }

            if (ddlMerchant1.SelectedValue == "--请选择--")
            {
                Alert("未选择商家");
                return false;
            }

            if (ddlWareHouse.SelectedValue == "")
            {
                Alert("未选择仓库");
                return false;
            }

            return true;
        }

        protected void btUpdateAreaType_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, string>> keyValuePairs = GridViewHelper.GetSelectedRows<string>(gvArea, "cbCheckBox", 6);
            if (!JudgeInput(keyValuePairs))
                return;

            List<AreaExpressLevelIncome> areaExpressLevelIncomes = new List<AreaExpressLevelIncome>();
            foreach (var keyPair in keyValuePairs)
            {
                AreaExpressLevelIncome areaExpressLevelIncome = new AreaExpressLevelIncome();
                areaExpressLevelIncome.AreaID = keyPair.Key;
                areaExpressLevelIncome.AreaName = keyPair.Value;
                areaExpressLevelIncome.ExpressCompanyID = ExpressCompanyID;
                areaExpressLevelIncome.CompanyName = CompanyName;
                areaExpressLevelIncome.MerchantID = int.Parse(ddlMerchant1.SelectedValue);
                areaExpressLevelIncome.MerchantName = ddlMerchant1.SelectedItem.Text;
                areaExpressLevelIncome.WareHouseID = ddlWareHouse.SelectedValue;
                areaExpressLevelIncome.EffectAreaType = int.Parse(ddlAreaType.SelectedValue);
                areaExpressLevelIncome.UpdateBy = base.Userid;
                areaExpressLevelIncome.AuditStatus = (int)AreaLevelStatus.S1;
                areaExpressLevelIncome.DistributionCode = base.DistributionCode;
                areaExpressLevelIncomes.Add(areaExpressLevelIncome);
            }
            if (areaExpressLevelIncomes.Count <= 0)
            {
                Alert("未能找到更新元素");
                return;
            }
            var areaExpressLevelIncomeBll = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            string msg = string.Empty;
            if (areaExpressLevelIncomeBll.UpdateAreaType(areaExpressLevelIncomes, out msg))
            {
                gvAreaType.DataSource = areaExpressLevelIncomeBll.SearchAreaType(AreaID,base.DistributionCode);
                gvAreaType.DataBind();
                if (!string.IsNullOrEmpty(msg))
                    Alert("更新成功，以下为警告信息：<br>" + msg);
                else
                    Alert("更新成功");
            }
            else
                Alert("更新失败");
        }

        protected void btDelete_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, string>> keyValuePairs = GridViewHelper.GetSelectedRows<string>(gvAreaType, "cbTypeCheckBox", 3);
            if (keyValuePairs.Count <= 0)
            {
                Alert("没有选择要删除的区域类型");
                return;
            }

            var areaExpressLevelIncomeBll = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            if (areaExpressLevelIncomeBll.DeleteAreaType(keyValuePairs, Userid))
            {
                Alert("删除成功");
                gvAreaType.DataSource = areaExpressLevelIncomeBll.SearchAreaType(AreaID,base.DistributionCode);
                gvAreaType.DataBind();
            }
            else
                Alert("删除失败");
        }

        protected void btnExprotDownLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "~/UpFile/收入区域类型导入模板.xls";
                DownLoadTemplet(filePath, "收入区域类型导入模板.xls");
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
                var areaExpressLevelBll = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                DataTable dtError;
                if (areaExpressLevelBll.ExportAreaType(ds.Tables[0], Userid, out dtError,base.DistributionCode,ExpressCompanyID))
                {
                    if (dtError != null && dtError.Rows.Count > 0)
                    {
                        List<string> l = new List<string>();
                        foreach (DataColumn column in dtError.Columns)
                        {
                            l.Add(column.ColumnName);
                        }
                        ExportExcel(dtError, l.ToArray(), null, "收入区域类型导入错误反馈");
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
                Alert("导入失败<br>"+ex.Message);
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
    }
}