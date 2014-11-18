using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.COD;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service;
using RFD.FMS.MODEL;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEB.COD
{
    public partial class LogisticsDeliveryDaily : BasePage
    {
        private ILogisticsDeliveryService bll = ServiceLocator.GetService<ILogisticsDeliveryService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ucDeliveryHouse.DistributionCode = base.DistributionCode;

                DateTime dt = DateTime.Now.AddDays(-1);
                tbDate_D_S.Text = dt.ToString("yyyy-MM-dd") ;
                tbDate_D_E.Text = dt.ToString("yyyy-MM-dd") ;
            }

            UCStationCheckBoxSite.DistributionCode = base.DistributionCode;

            pager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
        }

        public bool IsClickV2
        {
            get { return DataConvert.ToBoolean(ViewState["ClickV2"]); }
            set { ViewState["ClickV2"] = value; }
        }

        public CODSearchCondition SearchCondition
        {
            get { return ViewState["CODSearchCondition"] == null ? null : ViewState["CODSearchCondition"] as CODSearchCondition; }
            set { ViewState.Add("CODSearchCondition", value); }
        }

        protected void btSearch_ClickV2(object sender, EventArgs e)
        {
            try
            {
                IsClickV2 = true;

                BindData();
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>" + ex.Message);
                return;
            }
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            try
            {
                IsClickV2 = false;

                BindData();
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
                return;
            }
        }

        private void BindData()
        {
            if (!JudgeInput()) return;
            string TimeMessage = "";
            SetSearchCondition();

            if (IsClickV2 == true)
            {
                TotalData = bll.SearchLogisticsDailyV2(SearchCondition);
            }
            else
            {
                TotalData = bll.SearchLogisticsDaily(SearchCondition,ref TimeMessage);
            }

            if (TotalData != null && TotalData.Rows.Count > 0)
            {
                BindDataWithBuildPage(TotalData, pager, gvList);
                DataRow[] drs = TotalData.Select("结算单位='合计'");
                if (drs.Length >= 1)
                {
                    lb_D_Sum.Text = drs[0]["普通发货数"].ToString();
                    lb_D_R_Sum.Text = drs[0]["普通拒收数"].ToString();
                    lb_DV_Sum.Text = drs[0]["上门换发数"].ToString();
                    lb_DV_R_Sum.Text = drs[0]["上门换拒数"].ToString();
                    lb_V_Sum.Text = drs[0]["上门退单数"].ToString();
                    lb_SR_D_Sum.Text = drs[0]["签单返回发数"].ToString();
                    lb_SR_R_Sum.Text = drs[0]["签单返回拒数"].ToString();
                    lb_PaySum.Text = drs[0]["实际支付订单量"].ToString();
                    lb_PayFee.Text = drs[0]["实际支付配送费"].ToString();
                }
            }
            else
            {
                throw new Exception("查询无记录");
            }
            TimeLabel.Text = TimeMessage;
        }

        private bool JudgeInput()
        {
            //if (string.IsNullOrEmpty(ucDeliveryHouse.SelectWareHouseIds))
            //{
            //    Alert("仓库必选");
            //    return false;
            //}

            //if (string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID) &&
            //    string.IsNullOrEmpty(UCStationCheckBoxSite.SelectExpressCompany) &&
            //    string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID)
            //    )
            //{
            //    Alert("商家、配送商、站点必选一个");
            //    return false;
            //}

            if (string.IsNullOrEmpty(tbDate_D_S.Text) ||
                string.IsNullOrEmpty(tbDate_D_E.Text)
                )
            {
                Alert("时间必填");
                return false;
            }

            if ((RFD.FMS.Util.Common.JudgeDateAreaByDay(DateTime.Parse(tbDate_D_S.Text), DateTime.Parse(tbDate_D_E.Text))))
            {
                Alert("结束时间不能早于开始时间");
                return false;
            }

            ////仓库一致
            //if ((ucDeliveryHouse.SelectWareHouseIds != ucReturnHouse.SelectWareHouseIds ||
            //    ucDeliveryHouse.SelectWareHouseIds != ucVisitReturnHouse.SelectWareHouseIds ||
            //    ucReturnHouse.SelectWareHouseIds != ucVisitReturnHouse.SelectWareHouseIds))
            //{
            //    Alert("发货、拒收、上门退仓库选择必须一致");
            //    return false;
            //}

            int maxSearchDay = 31;
            //时间限制31天内
            TimeSpan dayD = DateTime.Parse(tbDate_D_E.Text) - DateTime.Parse(tbDate_D_S.Text);
            if (dayD.TotalDays > maxSearchDay )
            {
                Alert(string.Format("时间区域不能大于{0}天", maxSearchDay.ToString()));
                return false;
            }

            return true;
        }

        protected void btExprot_ClickV2(object sender, EventArgs e)
        {
            try
            {
                BindData();

                CSVExport.ExportFile(TotalData, null, null, "支出日报-" + DateTime.Parse(tbDate_D_S.Text.Trim()).ToString("yyyyMMdd"),false);
            }
            catch (Exception ex)
            {
                Alert("导出错误<br>" + ex.Message);

                return;
            }
        }

        protected void btExprot_Click(object sender, EventArgs e)
        {
            try
            {
                BindData();

                CSVExport.ExportFileToClient(TotalData, "支出日报-" + DateTime.Parse(tbDate_D_S.Text.Trim()).ToString("yyyyMMdd"), false, download_token_name.Value, download_token_value.Value);
                //var fullPath = CSVExport.WriteFile(TotalData, null, null, "支出日报-" + DateTime.Parse(tbDate_D_S.Text.Trim()).ToString("yyyyMMdd"), false);

                //ClientScript.RegisterStartupScript(typeof(LogisticsDeliverySummary), "Redirect", "downloadURL('" + ResolveUrl(fullPath) + "');", true);
            }
            catch (Exception ex)
            {
                Alert("导出错误<br>"+ex.Message);

                return;
            }
        }

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            if (TotalData == null)
            {
                BindData();
            }
            else
            {
                BindDataWithBuildPage(TotalData, pager, gvList);
            }
        }

        private void SetSearchCondition()
        {
            CODSearchCondition csc = new CODSearchCondition();
            csc.HouseD = ucDeliveryHouse.SelectWareHouseIds;
            csc.Date_D_S = DateTime.Parse(tbDate_D_S.Text.Trim());
            csc.Date_D_E = DateTime.Parse(tbDate_D_E.Text.Trim()).AddDays(1);

            string expresscompanyid=string.Empty;
            //if (UCStationCheckBoxSite.IsSelectAll && UCStationCheckBox.IsSelectAll)
            //    expresscompanyid = "";
            //else
            expresscompanyid = UCStationCheckBoxSite.SelectExpressCompany + "," + UCExpressCompanyTV.SelectExpressID;

            csc.ExpressCompanyID = expresscompanyid.TrimEnd(',').TrimStart(',');
            csc.MerchantID = UCMerchantSourceTV.SelectMerchantID;
            csc.DistributionCode = base.DistributionCode;
            csc.IsAreaType = IsAreaType.Checked;
            csc.IsCOD = cbIsCod.Checked;
            SearchCondition = csc;
        }

        public DataTable TotalData
        {
            get { return ViewState["TotalData"] == null ? null : ViewState["TotalData"] as DataTable; }
            set { ViewState.Add("TotalData", value); }
        }
    }
}