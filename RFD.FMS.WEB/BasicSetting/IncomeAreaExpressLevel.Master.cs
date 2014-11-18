using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.ServiceImpl.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Model;
using RFD.FMS.MODEL;
using System.Data;
using RFD.FMS.WEB.UserControl;
using RFD.FMS.MODEL.Enumeration;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class IncomeAreaExpressLevel : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
                UCWareHouseCheckBox.DistributionCode = DistributionCode;
            }
            UCPager.PagerPageChanged += new EventHandler(AspNetPager1_PageChanged);
        }

        private void InitForm()
        {
            IExpressCompanyService expressCompany = ServiceLocator.GetService<IExpressCompanyService>();
            ExpressCompany ecModel = expressCompany.GetCompanyModelByDistributionCode(DistributionCode);
            if (ecModel != null)
            {
                ExpressCompanyID = ecModel.ExpressCompanyID;
                CompanyName = ecModel.CompanyName;
            }
            IStatusCodeService statusCodeService = ServiceLocator.GetService<IStatusCodeService>();
            statusCodeService.BindDropDownListByCodeType(ddlAreaType, "请选择", "", "AreaType", DistributionCode);

            UCGoodsCategoryCheckBox.MerchantID = -1;
            UCGoodsCategoryCheckBox.DistributionCode = DistributionCode;
            
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

        public string DistributionCode
        {
            get
            {
                return ViewState["DistributionCode"] == null ? null : ViewState["DistributionCode"].ToString();
            }
            set
            {
                ViewState.Add("DistributionCode", value);
            }
        }
        public int ExpressID
        {
            get
            {
                return ViewState["ExpressID"] == null ? 0 : ViewState["ExpressID"].ToString().TryGetInt();
            }
            set
            {
                ViewState.Add("ExpressID", value);
            }
        }
        public int ExpressName
        {
            get
            {
                return ViewState["ExpressName"] == null ? 0 : ViewState["ExpressName"].ToString().TryGetInt();
            }
            set
            {
                ViewState.Add("ExpressName", value);
            }
        }
        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            SearchData(UCPager.CurrentPageIndex);
        }

        public AreaLevelIncomeSearchModel IncomeSearchModel
        {
            get { return ViewState["AreaLevelIncomeSearchModel"] == null ? null : ViewState["AreaLevelIncomeSearchModel"] as AreaLevelIncomeSearchModel; }
            set { ViewState.Add("AreaLevelIncomeSearchModel", value); }
        }

        public void SearchData(int currentPageIndex)
        {
            lbMsg.Text = "";
            gvList.DataSource = null;
            gvList.DataBind();
            PageInfo pi = new PageInfo(100);
            UCPager.PageSize = 100;
            pi.CurrentPageIndex = currentPageIndex;
            IAreaExpressLevelIncomeService areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            DataTable dt = areaExpressLevelIncomeService.GetAreaLevelIncomeList(IncomeSearchModel, ref pi);

            if (dt == null || dt.Rows.Count <= 0)
            {
                UCPager.RecordCount = 0;
                noData.Visible = true;
                noData.Style.Add("text-align", "center");
                noData.Text = "查询无数据";
                return;
            }
            if (OperateContent.Controls.Count > 1)
                gvList.Columns[13].Visible = false;
            else
                gvList.Columns[13].Visible = true;

            noData.Visible = false;
            UCPager.RecordCount = pi.ItemCount;
            gvList.DataSource = dt;
            gvList.DataBind();
        }

        public DataTable SearchDataS()
        {
            if (!JudgeInputSearch())
            {
                return new DataTable();
            }
            AreaExpressLevelIncomeService areaExpressLevelIncomeService = new AreaExpressLevelIncomeService();
            DataTable dt = areaExpressLevelIncomeService.GetAreaLevelIncomeList(IncomeSearchModel);

            return dt;
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            try
            {
                AreaLevelIncomeSearchModel searchModel = new AreaLevelIncomeSearchModel();
                if (!JudgeInputSearch())
                {
                    return;
                }
                //查询
                SearchData(1);
                LoadSetGoodsCategory();
            }
            catch (Exception ex)
            {
                lbMsg.Text = "查询失败:" + ex.Message;
            }
        }

        /// <summary>
        /// 是否按货物品类
        /// </summary>
        public bool IsCategory
        {
            get
            {
                bool flag = false;
                if (!bool.TryParse(ViewState["IsCategory"].ToString(), out flag))
                    return false;
                else
                    return flag;
            }
            set { ViewState.Add("IsCategory", value); }
        }

        /// <summary>
        /// 是否按配送商
        /// </summary>
        public bool IsExpressCompany
        {
            get
            {
                bool flag = false;
                if (!bool.TryParse(ViewState["IsExpress"].ToString(), out flag))
                    return false;
                else
                    return flag;
            }
            set { ViewState.Add("IsExpress", value); }
        }

        public void LoadSetGoodsCategory()
        {
            GoodsCategoryCheckBox setCategory = OperateContent.FindControl("UCGoodsCategoryCheckBoxSet") as GoodsCategoryCheckBox;
            Label lbCategory = OperateContent.FindControl("lbCategory") as Label;
            Label lbSetMsg = OperateContent.FindControl("lbSetMsg") as Label;
            Button btnAddAreaType = OperateContent.FindControl("btnAddAreaType") as Button;
            if (btnAddAreaType == null) return;//不是设置页面

            IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
            var condition = new SearchCondition()
            {
                MerchantID = IncomeSearchModel.MerchantID,
                StatusList = "'" + (int)MaintainStatus.Audited + "'",
                IsRawData = true,
                DistributionCode = DistributionCode,
            };
            PageInfo pi = new PageInfo(1);
            var data = deliverFeeService.BindDeliverFeeList(condition,ref pi);

            if (data == null ||
                data.Rows.Count == 0)
            {
                lbSetMsg.Text = "请先设置商家基础信息且已审核后设置";
                btnAddAreaType.Enabled = false;
                IsCategory = false;
                setCategory.Visible = false;
                lbCategory.Visible = false;
                return;
            }

            lbSetMsg.Text = "";
            btnAddAreaType.Enabled = true;
            if( data.Rows[0]["IsCategory"] == DBNull.Value ||
                data.Rows[0]["IsCategory"].ToString() == "0")
            {
                IsCategory = false;
                setCategory.Visible = false;
                lbCategory.Visible = false;
                return;
            }

            IsCategory = true;//从商家基础信息中获取
            if (setCategory != null && lbCategory!=null)
            {
                setCategory.MerchantID = IncomeSearchModel.MerchantID;
                setCategory.DistributionCode = IncomeSearchModel.DistributionCode;
                setCategory.LoadData();
                setCategory.Visible = true;
                lbCategory.Visible = true;
            }
        }

        protected void btExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!JudgeInputSearch())
                {
                    return;
                }
                IAreaExpressLevelIncomeService areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                DataTable dt = areaExpressLevelIncomeService.GetAreaLevelIncomeExprotList(IncomeSearchModel);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    lbMsg.Text = "查询无数据";
                    return;
                }
                CSVExport.ExportFileToClient(dt, "支出区域类型导出", false, download_token_name.Value, download_token_value.Value);
            }
            catch (Exception ex)
            {
                lbMsg.Text = "导出错误:" + ex.Message;
            }
        }

        public bool JudgeInputSearch()
        {
            if (string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
            {
                lbMsg.Text = "商家必选";
                return false;
            }
           
            var searchModel = new AreaLevelIncomeSearchModel();
            /************新增配送商查询条件2013-5-22 zhangrongrong ****************/
                searchModel.ExpressCompanyID =  UCExpressCompanyTV.SelectExpressID == "0"||string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID)?0:
                    Int32.Parse(UCExpressCompanyTV.SelectExpressID); 
            /**************************************************/

            searchModel.ProvinceID = UCPCASerach.ProvinceId;
            searchModel.CityID = UCPCASerach.CityId;
            searchModel.AreaID = UCPCASerach.AreaId;
            searchModel.MerchantID = string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID) ? 0 : int.Parse(UCMerchantSourceTV.SelectMerchantID);
            //searchModel.ExpressCompanyID = 0;
            searchModel.WareHouse = UCWareHouseCheckBox.SelectWareHouseIds;
            searchModel.AuditStatus = ddlAudit.SelectedIndex == 0 ? -2 : int.Parse(ddlAudit.SelectedValue);
            searchModel.AreaType = ddlAreaType.SelectedIndex == 0 ? 0 : int.Parse(ddlAreaType.SelectedValue);
            searchModel.DistributionCode = DistributionCode;
            searchModel.GoodsCategoryCode = UCGoodsCategoryCheckBox.SelectCategoryID;
            IncomeSearchModel = searchModel;
            return true;
        }

        public IList<KeyValuePair<DataKey, GridViewRow>> GridViewChecked
        {
            get
            {
                return GridViewHelper.GetSelectedRows(gvList, "cbCheckBox");
            }
        }
    }
}