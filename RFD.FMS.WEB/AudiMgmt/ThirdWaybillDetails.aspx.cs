using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEB.UserControl;
using RFD.FMS.MODEL.BasicSetting;
using System.Text;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.AudiMgmt
{
    public partial class ThirdWaybillDetails : BasePage
    {
        private readonly IMerchantService _merchantService = ServiceLocator.GetService<IMerchantService>();
        private readonly IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
        private readonly IWaybillService _waybillService = ServiceLocator.GetService<IWaybillService>();
        private static bool _isQuickQuery =true;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
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
            this.Pager1.PageChanged += new EventHandler(AspNetPager_PageChanged);
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

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            var pageInfo = new PageInfo(Pager1.PageSize);
            BuidPage(true, ref pageInfo);
        }

        protected void BtnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                _isQuickQuery = false;
                if (!string.IsNullOrEmpty(txtWaybillNO.Text.Trim())
                    && (!Regex.Match(txtWaybillNO.Text.Trim(), @"^[0-9, ]{1,}$").Success)
                    && DDLWaybillNO.SelectedValue == "0")
                {
                    Alert("运单号只能输入数字！");
                    return;
                }
                var pageInfo = new PageInfo(this.Pager1.PageSize);
                BuidPage(true, ref pageInfo);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
        }

        protected void BuidPage(bool pageOrNo, ref PageInfo pi)
        {
            DataTable amount;
            pi.CurrentPageIndex = Pager1.CurrentPageIndex;
            DataTable dataTable = DtSource(pageOrNo, ref pi,out amount);
            Pager1.PageSize = pi.PageSize;
            Pager1.RecordCount = pi.ItemCount;
            Pager1.CurrentPageIndex = pi.CurrentPageIndex;

            gridview1.DataSource = dataTable;
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

        protected DataTable DtSource(bool pageOrNo, ref PageInfo pi, out DataTable amount)
        {
			ThirdPartyWaybillSearchConditons conditions;
			if (!GetConditions(out conditions))
			{
				amount = null;
				return null;
			}

            if (CheckSearchScope(conditions))
            {
                DataTable dt = _waybillService.GetThirdWaybillDetails(conditions, pageOrNo, ref pi, out amount);
                return dt;
            }
            else
            {
                var dt = new DataTable();
                amount = null;
                return dt;
            }
        }

        protected bool CheckSearchScope(ThirdPartyWaybillSearchConditons conditons)
        {
            if(!string.IsNullOrEmpty(conditons.OutCreatTimeEnd.ToString())
                &&!string.IsNullOrEmpty(conditons.OutCreatTimeBegin.ToString()))
            {
                if ((conditons.OutCreatTimeEnd - conditons.OutCreatTimeBegin).Value.Days > 31)
                {
                    Alert("查询规模过大，请减小时间跨度！");
                    return false;
                }
                if ((conditons.OutCreatTimeEnd - conditons.OutCreatTimeBegin).Value.Days >7
                    && conditons.MerchantID == null
                && conditons.SortingCenter == null
                && string.IsNullOrEmpty(conditons.DeliverStationID))
                {
                    Alert("查询规模过大，请细化商家、分拣中心或配送商等条件！");
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(conditons.OutCreatTimeEnd.ToString())
                 && string.IsNullOrEmpty(conditons.OutCreatTimeBegin.ToString()))
            {
                return false;
            }
            else if (string.IsNullOrEmpty(conditons.OutCreatTimeEnd.ToString())
                 && !string.IsNullOrEmpty(conditons.OutCreatTimeBegin.ToString()))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(conditons.BackStationTimeEnd.ToString())
                && !string.IsNullOrEmpty(conditons.BackStationTimeBegin.ToString()))
            {
                if ((conditons.BackStationTimeEnd - conditons.BackStationTimeBegin).Value.Days > 31)
                {
                    Alert("查询规模过大，请减小时间跨度！");
                    return false;
                }
                if((conditons.BackStationTimeEnd - conditons.BackStationTimeBegin).Value.Days > 7
                    &&conditons.MerchantID==null
                && conditons.SortingCenter==null
                && string.IsNullOrEmpty(conditons.DeliverStationID))
                {
                    Alert("查询规模过大，请细化商家、分拣中心或配送商等条件！");
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(conditons.BackStationTimeEnd.ToString())
                && string.IsNullOrEmpty(conditons.BackStationTimeBegin.ToString()))
            {
                return false;
            }
            else if(string.IsNullOrEmpty(conditons.BackStationTimeEnd.ToString())
                && !string.IsNullOrEmpty(conditons.BackStationTimeBegin.ToString()))
            {
                return false;
            }
            return true;
         }

		protected bool GetConditions(out ThirdPartyWaybillSearchConditons conditions)
        {
            conditions = new ThirdPartyWaybillSearchConditons();
            conditions.IsQucikQuery = _isQuickQuery;
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

			if (txtWaybillNO.Text.Trim() == "" &&
				UCMerchantSource.SelectMerchantSourcesID == "" &&
				UCStationThirdCompany.SelectExpressCompany == "" &&
				UCStationRFDSite.SelectExpressCompany == ""
				)
			{
				Alert("请至少选择一个商家或一个配送商或一个订单号进行查询");
				return false;
			}
			if (!string.IsNullOrEmpty(txtWaybillNO.Text.Trim()))
			{
				if (DDLWaybillNO.SelectedValue == "1")
				{
					conditions.Customerorder = txtWaybillNO.Text.Trim();
				}
				else if (DDLWaybillNO.SelectedValue == "0")
				{
					conditions.WaybillNO = Convert.ToInt64(txtWaybillNO.Text.Trim());
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(UCMerchantSource.SelectMerchantSourcesID))
					conditions.MerchantID = UCMerchantSource.SelectMerchantSourcesID.Replace(" ", "");

				#region 配送商 站点
				StringBuilder sbCompany = new StringBuilder();
				conditions.RFDType = ddlRFDType.SelectedValue;

				if (!string.IsNullOrEmpty(UCStationThirdCompany.SelectExpressCompany))
					sbCompany.Append(GetAllCompanyAndStation(UCStationThirdCompany.SelectExpressCompany.Replace(" ", "")));

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
				switch (ddlAreaExpressLevel.SelectedValue)
				{
					//????????????????????????????需要再考虑
					case "0":
						break;
					case "":
						conditions.AreaExpressLevel = null;
						break;
					default:
						conditions.AreaExpressLevel = Convert.ToInt32(ddlAreaExpressLevel.SelectedValue);
						break;
				}
				if (DDLTime.SelectedValue == "0")
				{
					if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
						conditions.OutCreatTimeBegin = Convert.ToDateTime(txtBTime.Text.Trim());
					if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
						conditions.OutCreatTimeEnd = Convert.ToDateTime(txtETime.Text.Trim());
				}
				else if (DDLTime.SelectedValue == "1")
				{
					if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
						conditions.BackStationTimeBegin = Convert.ToDateTime(txtBTime.Text.Trim());
					if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
						conditions.BackStationTimeEnd = Convert.ToDateTime(txtETime.Text.Trim());
				}
				else if (DDLTime.SelectedValue == "2")
				{
					if (!string.IsNullOrEmpty(txtBTime.Text.Trim()))
						conditions.InCreatTimeBegin = Convert.ToDateTime(txtBTime.Text.Trim());
					if (!string.IsNullOrEmpty(txtETime.Text.Trim()))
						conditions.InCreatTimeEnd = Convert.ToDateTime(txtETime.Text.Trim());
				}
				if (!string.IsNullOrEmpty(UCWareHouseCheckBox.SelectWareHouseIds))
					conditions.SortingCenter = UCWareHouseCheckBox.SelectWareHouseIds.Replace(" ", "");
                if(!string.IsNullOrEmpty(ddlInefficacyStatus.SelectedValue.ToString()))
                {
                    conditions.InefficacyStatus = Convert.ToInt32(ddlInefficacyStatus.SelectedValue);
                }
			}
            return true;
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                _isQuickQuery = false;
                var p = new PageInfo(Pager1.PageSize);
                var amount = new DataTable();
                //CSVExport.DataTable2Excel(DtSource(false, ref p, out amount), "外单接收查询明细表");
                CSVExport.ExportFile(DtSource(false, ref p, out amount), null, null, "外单接收查询明细表",true);
            }
            catch (Exception ex)
            {
                Alert("导出出错<br>" + ex.Message);
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

        protected void BtnQuickQuery_Click(object sender, EventArgs e)
        {
            try
            {
                _isQuickQuery = true;
                if (!string.IsNullOrEmpty(txtWaybillNO.Text.Trim())
                    && (!Regex.Match(txtWaybillNO.Text.Trim(), @"^[0-9, ]{1,}$").Success)
                    && DDLWaybillNO.SelectedValue == "0")
                {
                    Alert("运单号只能输入数字！");
                    return;
                }
                var pageInfo = new PageInfo(this.Pager1.PageSize);
                BuidPage(true, ref pageInfo);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>" + ex.Message);
            }
        }

        protected void BtnQuickExport_Click(object sender, EventArgs e)
        {
            try
            {
                _isQuickQuery = true;
                var p = new PageInfo(Pager1.PageSize);
                var amount = new DataTable();
                //CSVExport.DataTable2Excel(DtSource(false, ref p, out amount), "外单接收查询明细表");
                CSVExport.ExportFile(DtSource(false, ref p, out amount), null, null, "外单接收查询明细表",true);
            }
            catch (Exception ex)
            {
                Alert("导出出错<br>"+ex.Message);
            }
        }

        private string GetAllCompanyAndStation(string companys)
        {
            IExpressCompanyService service = ServiceLocator.GetService<IExpressCompanyService>();

            return service.GetCompanyAndStation(companys);
        }
    }
}
