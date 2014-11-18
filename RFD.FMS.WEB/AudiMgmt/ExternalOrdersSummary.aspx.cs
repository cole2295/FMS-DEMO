using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEBLOGIC.BasicSetting;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.WEB.UserControl;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using RFD.FMS.MODEL;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.AudiMgmt
{
    public partial class ExternalOrdersSummary : BasePage
    {
        private readonly IMerchantService _merchantService = ServiceLocator.GetService<IMerchantService>();
        private readonly IExpressCompanyService _expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
        private readonly IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
        private readonly IWaybillForIncomeBaseInfoService _bll = ServiceLocator.GetService<IWaybillForIncomeBaseInfoService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                txtBTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                txtETime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                BuildAreaExpressLevel();
                BuildInefficacyStatus();
            }

            UCWareHouseCheckBox.DistributionCode = base.DistributionCode;
            UCStationRFDSite.DistributionCode = base.DistributionCode;
            UCPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
        }

        private void BuildAreaExpressLevel()
        {
            ddlAreaExpressLevel.BindListData(_statusInfoService.GetStatusInfoByTypeNo(305), "statusName", "statusNo", string.Empty);
            ddlAreaExpressLevel.Items.Insert(ddlAreaExpressLevel.Items.Count, new ListItem("空", "99"));
        }

        private void BuildInefficacyStatus()
        {
            ddlInefficacyStatus.BindListData(_statusInfoService.GetStatusInfoByTypeNo(308), "statusName", "statusNo", string.Empty);
        }

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            SearchData(UCPager.CurrentPageIndex);
        }

        public ThirdPartyWaybillSearchConditons SearchCondition
        {
            get { return ViewState["ThirdPartyWaybillSearchConditons"] == null ? null : ViewState["ThirdPartyWaybillSearchConditons"] as ThirdPartyWaybillSearchConditons; }
            set { ViewState.Add("ThirdPartyWaybillSearchConditons", value); }
        }

        protected void BtnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                ThirdPartyWaybillSearchConditons condition;

                if (!JudgeInput(out condition)) return;

                SearchCondition = condition;

                SearchData(1);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>" + ex.Message);
            }
        }

        private void SearchData(int currentPageIndex)
        {
            gridview1.DataSource = null;
            gridview1.DataBind();
            DataTable amount;

            DataTable dt = null;
            dt = _bll.SearchSummary(SearchCondition, out amount);

            WriteStatMsg(amount);

            if (dt == null || dt.Rows.Count <= 0)
            {
                noData.Visible = true;
                noData.Text = "查询无数据";

                return;
            }
            noData.Visible = false;
            BindDataWithBuildPage(dt, UCPager, gridview1);
        }

        private void WriteStatMsg(DataTable amount)
        {
            if (amount == null || amount.Rows.Count <= 0)
                return;
            lbWaybillStat.Text = amount.Rows[0]["订单量合计"].ToString();
            lbNeedStat.Text = amount.Rows[0]["应收订单量合计"].ToString();
            lbNeedPayStat.Text = amount.Rows[0]["应收款合计"].ToString();
            lbNeedBackStat.Text = amount.Rows[0]["应退订单量合计"].ToString();
            lbNeedBackPayStat.Text = amount.Rows[0]["应退款合计"].ToString();
            lbWeightStat.Text = amount.Rows[0]["结算重量合计"].ToString();
            lbProtectFare.Text = amount.Rows[0]["保价金额合计"].ToString();
            lbDeliverFare.Text = amount.Rows[0]["配送费合计"].ToString();
            lbposFare.Text = decimal.Parse(amount.Rows[0]["POS服务费合计"].ToString()).ToString("F2");
            lbCashFare.Text = decimal.Parse(amount.Rows[0]["代收手续费合计"].ToString()).ToString("F2");
        }

        protected bool JudgeInput(out ThirdPartyWaybillSearchConditons conditions)
        {
            conditions = new ThirdPartyWaybillSearchConditons();
            if (!string.IsNullOrEmpty(txtWaybillNO.Text.Trim())
                && (!Regex.Match(txtWaybillNO.Text.Trim(), @"^[0-9, ]{1,}$").Success)
                && DDLWaybillNO.SelectedValue == "0")
            {
                Alert("运单号只能输入数字！");
                return false;
            }
            if (string.IsNullOrEmpty(txtBTime.Text.Trim()))
            {
                Alert("开始日期不能为空");
                return false;
            }
            if (string.IsNullOrEmpty(txtETime.Text.Trim()))
            {
                Alert("结束日期不能为空");
                return false;
            }
            TimeSpan day = DateTime.Parse(txtETime.Text.Trim()) - DateTime.Parse(txtBTime.Text.Trim());
            if (day.TotalDays < 0)
            {
                Alert("开始日期不能大于等于结束日期");
            }
            if (day.TotalDays > 31)
            {
                Alert("日期范围不能大于31天");
                return false;
            }

            conditions.DistributionCode = base.DistributionCode;

            //if (txtWaybillNO.Text.Trim() == "" &&
            //    UCMerchantSourceTV.SelectMerchantID == "" &&
            //    UCStationThirdCompany.SelectExpressCompany == "" &&
            //    UCStationRFDSite.SelectExpressCompany == ""
            //    )
            //{
            //    Alert("请至少选择一个商家或一个配送商或一个订单号进行查询");
            //    return false;
            //}
            if (!string.IsNullOrEmpty(txtWaybillNO.Text.Trim()))
            {
                if (DDLWaybillNO.SelectedValue == "1")
                {
                    conditions.Customerorder = txtWaybillNO.Text.Trim();
                    //Alert("暂不支持订单号查询 ");
                    //return false;
                }
                else if (DDLWaybillNO.SelectedValue == "0")
                {
                    conditions.WaybillNO = Convert.ToInt64(txtWaybillNO.Text.Trim());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
                    conditions.MerchantID = UCMerchantSourceTV.SelectMerchantID.Replace(" ", "");

                #region 配送商 站点
                StringBuilder sbCompany = new StringBuilder();
                conditions.RFDType = ddlRFDType.SelectedValue;

                if (!string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID))
                    sbCompany.Append(UCExpressCompanyTV.SelectExpressID.Replace(" ", ""));

                if (sbCompany != null && sbCompany.Length > 0 && !string.IsNullOrEmpty(UCStationRFDSite.SelectExpressCompany))
                    sbCompany.Append(",");

                if (conditions.RFDType == "1")
                {
                    if (!string.IsNullOrEmpty(UCStationRFDSite.SelectExpressCompany))
                    {
                        if (!string.IsNullOrEmpty(UCStationRFDSite.SelectExpressCompanyChildrens))
                            sbCompany.Append(UCStationRFDSite.SelectExpressCompanyChildrens.Replace(" ", ""));
                        else
                        {
                            Alert("所选如风达分拣中心下无站点，请重新选择如风达分拣中心");
                            return false;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(UCStationRFDSite.SelectExpressCompany))
                        sbCompany.Append(UCStationRFDSite.SelectExpressCompany.Replace(" ", ""));
                }
                #endregion
                if (sbCompany != null && sbCompany.Length > 0)
                    conditions.DeliverStationID = sbCompany.ToString();

                if (!string.IsNullOrEmpty(DDLWaybillType.SelectedValue))
                {
                    conditions.WaybillType = DDLWaybillType.SelectedValue;
                }
                if (!string.IsNullOrEmpty(DDLWaybillStatus.SelectedValue))
                {
                    conditions.WaybillStatus = DDLWaybillStatus.SelectedValue;
                }
                if (!string.IsNullOrEmpty(DDLAcceptType.SelectedValue))
                {
                    conditions.PaymentType = DDLAcceptType.SelectedValue;
                }
                if (!string.IsNullOrEmpty(DDLWaybillBackStatus.SelectedValue))
                {
                    conditions.BackStatus = DDLWaybillBackStatus.SelectedValue;
                }
                conditions.AreaExpressLevel = ddlAreaExpressLevel.SelectedValue == "" ? 0 : int.Parse(ddlAreaExpressLevel.SelectedValue);
                if (DDLTime.SelectedValue == "0")
                {
                    conditions.DateType = 0;
                    if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
                        conditions.OutCreatTimeBegin = DateTime.Parse(txtBTime.Text.Trim());
                    if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
                        conditions.OutCreatTimeEnd = DateTime.Parse(txtETime.Text.Trim()).AddDays(1);
                }
                else if (DDLTime.SelectedValue == "1")
                {
                    conditions.DateType = 1;
                    if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
                        conditions.BackStationTimeBegin = DateTime.Parse(txtBTime.Text.Trim());
                    if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
                        conditions.BackStationTimeEnd = DateTime.Parse(txtETime.Text.Trim()).AddDays(1);
                }
                else if (DDLTime.SelectedValue == "2")
                {
                    conditions.DateType = 2;
                    if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
                        conditions.InCreatTimeBegin = DateTime.Parse(txtBTime.Text.Trim());
                    if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
                        conditions.InCreatTimeEnd = DateTime.Parse(txtETime.Text.Trim()).AddDays(1);
                }
                if (!string.IsNullOrEmpty(UCWareHouseCheckBox.SelectWareHouseIds))
                    conditions.SortingCenter = UCWareHouseCheckBox.SelectWareHouseIds.Replace(" ", "");
                if (!string.IsNullOrEmpty(ddlInefficacyStatus.SelectedValue.ToString()))
                {
                    conditions.InefficacyStatus = Convert.ToInt32(ddlInefficacyStatus.SelectedValue);
                }
            }
            return true;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (SearchCondition == null)
                {
                    ThirdPartyWaybillSearchConditons condition;

                    if (!JudgeInput(out condition)) return;

                    SearchCondition = condition;
                }

                DataTable dt = _bll.SearchSummaryForExport(SearchCondition);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("没有找到需要导出的数据");

                    return;
                }
                CSVExport.ExportFileToClient(dt, "外接订单汇总表", false, download_token_name.Value, download_token_value.Value);
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>" + ex.Message);
            }
        }

        protected void ddlRFDType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRFDType.SelectedIndex > 0)
            {
                UCStationRFDSite.LoadDataType = LoadDataType.RFDSortCenter;
            }
            else
            {
                UCStationRFDSite.LoadDataType = LoadDataType.RFDSite;
            }
            UCStationRFDSite.LoadData(true);
        }
    }
}