using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEB.UserControl;
using System.Text.RegularExpressions;
using RFD.FMS.WEB.Main;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using System.Text;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.AudiMgmt
{
    public partial class ThirdWaybillStat : BasePage
    {
        private readonly IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
        private readonly IWaybillService _waybillService = ServiceLocator.GetService<IWaybillService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                txtBTime.Text = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd 00:00:00");
                txtETime.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                BuildWaybillStatus();
                BuildWaybillBackStatus();
                BuildAreaExpressLevel();
                BuildInefficacyStatus();
            }
            UCMerchantSource.DistributionCode = base.DistributionCode;
            UCWareHouseCheckBox.DistributionCode = base.DistributionCode;
            UCStationRFDSite.DistributionCode = base.DistributionCode;
            UCStationThirdCompany.DistributionCode = base.DistributionCode;
        }

        private void BuildWaybillStatus()
        {
            DDLWaybillStatus.BindListData(_statusInfoService.GetStatusInfoByTypeNo(1), "statusName", "statusNo", string.Empty);
        }

        private void BuildWaybillBackStatus()
        {
            DDLWaybillBackStatus.BindListData(_statusInfoService.GetStatusInfoByTypeNo(5), "statusName", "statusNo", string.Empty);
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

        protected void BtnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                //查询
                DataTable amount = new DataTable();
                DataTable dt = GetData(out amount);

                gridview1.DataSource = dt;
                gridview1.DataBind();

                if (amount == null || amount.Rows.Count <= 0)
                    return;

                lbWaybillStat.Text = amount.Rows[0]["订单量合计"].ToString();
                lbNeedStat.Text = amount.Rows[0]["应收订单量合计"].ToString();
                lbNeedPayStat.Text = amount.Rows[0]["应收款合计"].ToString();
                lbNeedBackStat.Text = amount.Rows[0]["应退订单量合计"].ToString();
                lbNeedBackPayStat.Text = amount.Rows[0]["应退款合计"].ToString();
                lbWeightStat.Text = amount.Rows[0]["结算重量合计"].ToString();
                lbProtectFare.Text = amount.Rows[0]["保价金额合计"].ToString();
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>" + ex.Message);
            }
        }

        protected DataTable GetData(out DataTable amount)
        {
            ThirdPartyWaybillSearchConditons conditions;
            if (!GetConditions(out conditions))
            {
                amount = null;
                return null;
            }
            
            return _waybillService.GetThirdWaybillStat(conditions, out amount);
        }

        protected bool GetConditions(out ThirdPartyWaybillSearchConditons conditions)
        {
            conditions = new ThirdPartyWaybillSearchConditons();
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
            if (day.TotalDays <= 0)
            {
                Alert("开始日期不能大于等于结束日期");
            }
            if (day.TotalDays > 31)
            {
                Alert("日期范围不能大于31天");
                return false;
            }

            if (UCMerchantSource.SelectMerchantSourcesID == "" &&
                UCStationThirdCompany.SelectExpressCompany == "" &&
                UCStationRFDSite.SelectExpressCompany == ""
                )
            {
                Alert("请至少选择一个商家或一个配送商或一个订单号进行查询");
                return false;
            }
            
            if (!string.IsNullOrEmpty(UCMerchantSource.SelectMerchantSourcesID))
                conditions.MerchantID = UCMerchantSource.SelectMerchantSourcesID.Replace(" ", "");

            #region 配送商 站点
            StringBuilder sbCompany = new StringBuilder();
            conditions.RFDType = ddlRFDType.SelectedValue;

            if (!string.IsNullOrEmpty(UCStationThirdCompany.SelectExpressCompany))
                sbCompany.Append(UCStationThirdCompany.SelectExpressCompany.Replace(" ", ""));

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
                conditions.TimeType = 1;
                if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
                    conditions.OutCreatTimeBegin = Convert.ToDateTime(txtBTime.Text.Trim());
                if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
                    conditions.OutCreatTimeEnd = Convert.ToDateTime(txtETime.Text.Trim());
            }
            else if (DDLTime.SelectedValue == "1")
            {
                conditions.TimeType = 2;
                if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
                    conditions.BackStationTimeBegin = Convert.ToDateTime(txtBTime.Text.Trim());
                if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
                    conditions.BackStationTimeEnd = Convert.ToDateTime(txtETime.Text.Trim());
            }
            else if (DDLTime.SelectedValue == "2")
            {
                conditions.TimeType = 3;
                if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
                    conditions.InCreatTimeBegin = Convert.ToDateTime(txtBTime.Text.Trim());
                if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
                    conditions.InCreatTimeEnd = Convert.ToDateTime(txtETime.Text.Trim());
            }
            if (!string.IsNullOrEmpty(UCWareHouseCheckBox.SelectWareHouseIds))
                conditions.SortingCenter = UCWareHouseCheckBox.SelectWareHouseIds.Replace(" ", "");
            if (!string.IsNullOrEmpty(ddlInefficacyStatus.SelectedValue.ToString()))
            {
                conditions.InefficacyStatus = Convert.ToInt32(ddlInefficacyStatus.SelectedValue);
            }
            return true;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                //导出
                DataTable amount = new DataTable();
                //CSVExport.DataTable2Excel(GetData(out amount), "外单接收查询汇总表");
                CSVExport.ExportFile(GetData(out amount), null,null,"外单接收查询汇总表",false);
            }
            catch (Exception ex)
            {
                Alert("导出错误<br>" + ex.Message);
            }
        }
    }
}
