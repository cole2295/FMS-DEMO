using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEB.UserControl;
using RFD.FMS.Service.QueryStatistics;
using RFD.FMS.WEBLOGIC.BasicSetting;

namespace RFD.FMS.WEB.QueryStatistics
{
    public partial class CODTransferDetails : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
                UCWareHouseCheckBox.DistributionCode = base.DistributionCode;
            }
            UCStationRFDSite.DistributionCode = base.DistributionCode;
            UCPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
            BindCheckBoxListColumns(new List<string>());
        }

        private readonly IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
        private readonly ICODTransferService _bll = ServiceLocator.GetService<ICODTransferService>();

        public bool IsClickV2
        {
            get { return DataConvert.ToBoolean(ViewState["ClickV2"]); }
            set { ViewState["ClickV2"] = value; }
        }

        private void InitForm()
        {
            txtBTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            txtETime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        }

        protected void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlReportType.SelectedValue)
            {
                case "":
                    BindShipmentType();
                    break;
                //拒收情况的明细
                case "7":
                    BindShipmentType7();
                    break;
                //上门退换的明细
                case "6":
                    BindShipmentType6();
                    break;
                default:
                    break;
            }
        }

        private void BindShipmentType()
        {
            ddlShipmentType.Items.Clear();
            ddlShipmentType.Items.Insert(0, new ListItem("全部", ""));
            ddlShipmentType.Items.Insert(1, new ListItem("普通发货", "0"));
            ddlShipmentType.Items.Insert(2, new ListItem("上门换发货", "1"));

            ddlDateType.Items.Clear();
            ddlDateType.Items.Insert(0, new ListItem("最终发日期", "1"));
            ddlDateType.Items.Insert(1, new ListItem("初始发日期", "0"));


            lbHouseType.Text = "最终发货仓";
            hfHouseType.Value = "1";
        }

        private void BindShipmentType6()
        {
            ddlShipmentType.Items.Clear();
            ddlShipmentType.Items.Insert(0, new ListItem("全部", ""));
            ddlShipmentType.Items.Insert(1, new ListItem("上门换明细", "1"));
            ddlShipmentType.Items.Insert(2, new ListItem("上门退明细", "2"));

            ddlDateType.Items.Clear();
            ddlDateType.Items.Insert(0, new ListItem("入库时间", "0"));

            lbHouseType.Text = "入库仓库";
            hfHouseType.Value = "0";
        }

        private void BindShipmentType7()
        {
            ddlShipmentType.Items.Clear();
            ddlShipmentType.Items.Insert(0, new ListItem("全部", ""));
            ddlShipmentType.Items.Insert(1, new ListItem("普通拒收", "0"));
            ddlShipmentType.Items.Insert(2, new ListItem("上门换拒收", "1"));
            ddlShipmentType.Items.Insert(3, new ListItem("签单返回拒收", "3"));

            ddlDateType.Items.Clear();
            ddlDateType.Items.Insert(0, new ListItem("入库时间", "0"));

            lbHouseType.Text = "入库仓库";
            hfHouseType.Value = "0";
        }

        protected void btSearch_ClickV2(object sender, EventArgs e)
        {
            try
            {
                IsClickV2 = true;

                CodSearchCondition condition;

                if (!JudgeInput(out condition)) return;

                SearchCondition = condition;

                PageColumn = null;

                SearchData(1);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>" + ex.Message);
            }
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            try
            {
                IsClickV2 = false;

                CodSearchCondition condition;

                if (!JudgeInput(out condition)) return;

                SearchCondition = condition;

                PageColumn = null;

                SearchData(1);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>" + ex.Message);
            }
        }

        public CodSearchCondition SearchCondition
        {
            get { return ViewState["CodSearchCondition"] == null ? null : ViewState["CodSearchCondition"] as CodSearchCondition; }
            set { ViewState.Add("CodSearchCondition", value); }
        }

        public PageColumns PageColumn
        {
            get { return ViewState["PageColumns"] == null ? null : ViewState["PageColumns"] as PageColumns; }
            set { ViewState.Add("PageColumns", value); }
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

        protected void ddlDateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDateType.SelectedIndex == 0)
            {
                lbHouseType.Text = "最终发货仓";
                hfHouseType.Value = "1";
            }
            else
            {
                if (ddlReportType.SelectedIndex == 0)
                {
                    lbHouseType.Text = "初始发货仓";
                    hfHouseType.Value = "0";
                }
                else
                {
                    lbHouseType.Text = "入库仓库";
                    hfHouseType.Value = "0";
                }
            }
        }

        private bool JudgeInput(out CodSearchCondition condition)
        {
            condition = new CodSearchCondition();
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
                Alert("开始日期不能大于结束日期");
            }
            if (day.TotalDays > 31)
            {
                Alert("日期范围不能大于31天");
                return false;
            }

            if (txtWaybillNO.Text.Trim() == "" &&
                UCMerchantSourceTV.SelectMerchantID == "" &&
                UCExpressCompanyTV.SelectExpressID == "" &&
                UCStationRFDSite.SelectExpressCompany == ""
                )
            {
                Alert("请至少选择一个商家或一个配送商或一个订单号进行查询");
                return false;
            }

            if (string.IsNullOrEmpty(base.DistributionCode))
            {
                Alert("未找到用户隶属配送公司编码");
                return false;
            }
            condition.DistributionCode = base.DistributionCode;

            StringBuilder sbCompany = new StringBuilder();
            condition.RFDType = ddlRFDType.SelectedValue;

            if (!string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID))
                sbCompany.Append(UCExpressCompanyTV.SelectExpressID.Replace(" ", ""));

            if (sbCompany != null && sbCompany.Length > 0 && !string.IsNullOrEmpty(UCStationRFDSite.SelectExpressCompany))
                sbCompany.Append(",");

            if (condition.RFDType == "1")
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

            if (sbCompany != null && sbCompany.Length > 0)
                condition.ExpressCompanyID = sbCompany.ToString();

            if (!string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
                condition.Sources = UCMerchantSourceTV.SelectMerchantID;

            if (!string.IsNullOrEmpty(txtWaybillNO.Text.Trim()))
                condition.WaybillNO = txtWaybillNO.Text.Trim();

            condition.ReportType = ddlReportType.SelectedValue;
            condition.ShipmentType = ddlShipmentType.SelectedValue;
            condition.DateStr = DateTime.Parse(txtBTime.Text.Trim()).ToString("yyyy-MM-dd");
            condition.DateEnd = DateTime.Parse(txtETime.Text.Trim()).AddDays(1).ToString("yyyy-MM-dd");
            condition.DateType = ddlDateType.SelectedValue;
            condition.HouseType = hfHouseType.Value;//ddlHouseType.SelectedValue;
            condition.HouseCode = UCWareHouseCheckBox.SelectWareHouseIds.Replace(" ", "");

            return true;
        }

        private string CompanyIdToString(DataTable dt)
        {
            if (dt == null && dt.Rows.Count <= 0)
                return "";
            StringBuilder sb = new StringBuilder();

            foreach (DataRow dr in dt.Rows)
            {
                sb.Append(dr["ExpressCompanyID"].ToString() + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            SearchData(UCPager.CurrentPageIndex);
        }

        private void SearchData(int currentPageIndex)
        {
            gvList.DataSource = null;
            gvList.DataBind();

            PageInfo pi = new PageInfo(50);

            UCPager.PageSize = 50;

            pi.CurrentPageIndex = currentPageIndex;

            DataTable dtSummary;
            DataTable dt;

            if (IsClickV2 == true)
            {
                dt = _bll.SearchCodDetailsV2(SearchCondition, ref pi, out dtSummary);
            }
            else
            {
                dt = _bll.SearchCodDetails(SearchCondition, ref pi, out dtSummary);
            }

            WriteStatMag(dtSummary);

            if (dt == null || dt.Rows.Count <= 0)
            {
                pColumns.Visible = false;
                noData.Visible = true;
                noData.Text = "查询无数据";

                return;
            }

            noData.Visible = false;

            UCPager.RecordCount = pi.ItemCount;

            CreateGridViewColumns(dt.Columns);

            gvList.DataSource = dt;

            gvList.DataBind();

            IList<string> columns = new List<string>();

            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName);
            }

            BindCheckBoxListColumns(columns);
        }

        private void CreateGridViewColumns(DataColumnCollection dcc)
        {
            gvList.Columns.Clear();
            foreach (DataColumn dc in dcc)
            {
                BoundField bc = new BoundField();
                bc.DataField = dc.ColumnName;
                bc.HeaderText = dc.ColumnName;// dc.Caption.ToString();
                bc.ItemStyle.HorizontalAlign = HorizontalAlign.Left;//居中对齐
                gvList.Columns.Add(bc);
            }
        }

        private void BindCheckBoxListColumns(IList<string> columns)
        {
            pColumns.Visible = true;
            if (PageColumn == null)//区别首次
            {
                CreatePageColumnsModel(columns);
            }
            pColumns.Controls.Clear();//每次查询清空
            //if (pColumns.Controls.Count <= 0)
            CreateCheckBoxs();
            //else
            //JudgeColumnsChecked();
            foreach (KeyValuePair<string, bool> k in PageColumn.ColumnsShow)
            {
                GridViewColumnVisible(k.Key, k.Value);
            }
        }

        private void JudgeColumnsChecked()
        {
            int i = 0;
            foreach (KeyValuePair<string, bool> k in PageColumn.ColumnsShow)
            {
                CheckBox ckf = (CheckBox)pColumns.FindControl("cb" + i);
                ckf.Checked = k.Value;
                i++;
            }
        }

        private void CreateCheckBoxs()
        {
            int i = 0;
            foreach (KeyValuePair<string, bool> k in PageColumn.ColumnsShow)
            {
                CheckBox cb = new CheckBox();
                cb.AutoPostBack = true;
                cb.ID = "cb" + i;
                cb.Text = k.Key;
                cb.Checked = k.Value;
                if (k.Key == "订单号")
                {
                    cb.Enabled = false;
                }
                cb.CheckedChanged += cb_CheckedChanged;
                pColumns.Controls.Add(cb);
                i++;
            }
        }

        protected void cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            GridViewColumnVisible(cb.Text, cb.Checked);

            PageColumns p = new PageColumns();
            IDictionary<string, bool> columns = new Dictionary<string, bool>();
            for (int i = 0; i < pColumns.Controls.Count; i++)
            {
                CheckBox ckf = (CheckBox)pColumns.FindControl("cb" + i);
                columns.Add(new KeyValuePair<string, bool>(ckf.Text, ckf.Checked));
            }
            p.ColumnsShow = columns;
            p.DataColumns = PageColumn.DataColumns;
            PageColumn = p;
            BindCheckBoxListColumns(PageColumn.DataColumns);
        }

        private void GridViewColumnVisible(string headerText, bool flag)
        {
            for (int i = 0; i < gvList.Columns.Count; i++)
            {
                if (gvList.Columns[i].HeaderText == headerText)
                {
                    gvList.Columns[i].Visible = flag;
                    break;
                }
            }
        }

        private void CreatePageColumnsModel(IList<string> columnNames)
        {
            PageColumns p = new PageColumns();
            IDictionary<string, bool> columns = new Dictionary<string, bool>();
            foreach (string s in columnNames)
            {
                columns.Add(new KeyValuePair<string, bool>(s, true));
            }
            p.ColumnsShow = columns;
            p.DataColumns = columnNames;
            PageColumn = p;
        }

        private void WriteStatMag(DataTable dt)
        {
            lbWaybillStat.Text = dt.Rows[0]["订单量合计"].ToString();
            lbWeightStat.Text = dt.Rows[0]["结算重量合计"].ToString();
            lbTotalAmount.Text = dt.Rows[0]["总价合计"].ToString();
            lbPaidName.Text = dt.Columns[2].ColumnName;
            lbPaidStat.Text = dt.Rows[0][2].ToString();
            lbNeedName.Text = dt.Columns[3].ColumnName;
            lbNeedStat.Text = dt.Rows[0][3].ToString();
            lbNeedPayName.Text = dt.Columns[4].ColumnName;
            lbNeedPayStat.Text = dt.Rows[0][4].ToString();
        }

        protected void btExprot_ClickV2(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = _bll.SearchExprotDetailDataV2(SearchCondition);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("没有找到可以导出的数据");

                    return;
                }

                CSVExport.DataTable2Excel(dt, "COD交接明细表");
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>" + ex.Message);
            }
        }

        protected void btExprot_Click(object sender, EventArgs e)
        {
            try
            {
                CodSearchCondition condition;

                if (!JudgeInput(out condition)) return;

                SearchCondition = condition;
                DataTable dt = _bll.SearchExprotDetailData(SearchCondition);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("没有找到可以导出的数据");

                    return;
                }
                CSVExport.ExportFileToClient(dt, "COD交接明细表", false, download_token_name.Value, download_token_value.Value);
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>" + ex.Message);
            }
        }
    }
}