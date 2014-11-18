using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.BasicSetting;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;
using RFD.FMS.WEB.UserControl;
using System.Text;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Service;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEB.COD
{
    public partial class LogisticsDeliveryDetail : BasePage
    {
        private readonly IStatusInfoService _statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
        private ILogisticsDeliveryService _bll = ServiceLocator.GetService<ILogisticsDeliveryService>();
        protected string pageGuid = "pageGuid";
        protected readonly string pageGuname = "pageGuname";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
            }

            foreach (GridViewRow aa in gvList.Rows)
            {
                aa.Cells[2].Text =
                    string.Format(
                        "<a onclick=\"fnOpenModalDialog('../BasicSetting/OperateLogView.aspx?PK_NO={0}&LogType=8',500,300);return false;\" href=\"javascript:void(0)\">日志</a>",
                        aa.Cells[1].Text);
            }

            UCWareHouseCheckBox.DistributionCode = base.DistributionCode;
            UCStationRFDSite.DistributionCode = base.DistributionCode;
            UCPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
            BindCheckBoxListColumns(new List<string>());
            
        }

        public bool IsClickV2
        {
            get { return DataConvert.ToBoolean(ViewState["ClickV2"]); }
            set { ViewState["ClickV2"] = value; }
        }

        private void InitForm()
        {
            BindAreaType();
            txtBTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            txtETime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        }

        private void BindAreaType()
        {
            ddlAreaType.Items.Clear();
            ddlAreaType.AppendDataBoundItems = true;
            ddlAreaType.DataSource = _statusInfoService.GetStatusInfoByTypeNo(305);
            ddlAreaType.DataTextField = "statusName";
            ddlAreaType.DataValueField = "statusNo";
            ddlAreaType.DataBind();
            ddlAreaType.Items.Insert(0, new ListItem("全部", "-1"));
            ddlAreaType.Items.Insert(ddlAreaType.Items.Count, new ListItem("空", "99"));
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

        //private void BindShipmentType3()
        //{
        //    ddlShipmentType.Items.Clear();
        //    ddlShipmentType.Items.Insert(0, new ListItem("全部", "3"));

        //    ddlDateType.Items.Clear();
        //    ddlDateType.Items.Insert(0, new ListItem("最终发日期", "1"));
        //    ddlDateType.Items.Insert(1, new ListItem("初始发日期", "0"));

        //    lbHouseType.Text = "最终发货仓";
        //    hfHouseType.Value = "1";
        //}

        private void BindShipmentType()
        {
            ddlShipmentType.Items.Clear();
            ddlShipmentType.Items.Insert(0, new ListItem("全部", ""));
            ddlShipmentType.Items.Insert(1, new ListItem("普通发货", "0"));
            ddlShipmentType.Items.Insert(2, new ListItem("上门换发货", "1"));
            ddlShipmentType.Items.Insert(3, new ListItem("签单返回发货", "3"));

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

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            SearchData(UCPager.CurrentPageIndex);
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
            string amountStr = txtAmountStr.Text.Trim();
            string amountEnd = txtAmountEnd.Text.Trim();
            if (!Util.Common.IsNumeric(amountStr))
            {
                Alert("金额必须为数字");
                return false;
            }
            if (!Util.Common.IsNumeric(amountEnd))
            {
                Alert("金额必须为数字");
                return false;
            }

            //if (txtWaybillNO.Text.Trim() == "" &&
            //    UCMerchantSourceTV.SelectMerchantID == "" &&
            //    UCExpressCompanyTV.SelectExpressID == "" &&
            //    UCStationRFDSite.SelectExpressCompany == ""
            //    )
            //{
            //    Alert("请至少选择一个商家或一个配送商或一个订单号进行查询");
            //    return false;
            //}

            if (string.IsNullOrEmpty(base.DistributionCode))
            {
                Alert("未找到用户隶属配送公司编码");
                return false;
            }
            condition.DistributionCode = base.DistributionCode;
            StringBuilder sbCompany = new StringBuilder();
            condition.RFDType = ddlRFDType.SelectedValue;

            //if ((UCStationThirdCompany.IsSelectAll && !UCStationRFDSite.IsSelectAll) ||
            //    (!UCStationThirdCompany.IsSelectAll && UCStationRFDSite.IsSelectAll) ||
            //    (!UCStationThirdCompany.IsSelectAll && !UCStationRFDSite.IsSelectAll))
            //{
            if (!string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressID))
                sbCompany.Append(UCExpressCompanyTV.SelectExpressID.Replace(" ", ""));
            //}

            if (sbCompany != null && sbCompany.Length > 0 && !string.IsNullOrEmpty(UCStationRFDSite.SelectExpressCompany))
                sbCompany.Append(",");

            if (condition.RFDType == "1")
            {
                //if ((UCStationThirdCompany.IsSelectAll && !UCStationRFDSite.IsSelectAll) ||
                //(!UCStationThirdCompany.IsSelectAll && UCStationRFDSite.IsSelectAll) ||
                //    (!UCStationThirdCompany.IsSelectAll && !UCStationRFDSite.IsSelectAll))
                //{
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
                //}
            }
            else
            {
                //if ((UCStationThirdCompany.IsSelectAll && !UCStationRFDSite.IsSelectAll) ||
                //(!UCStationThirdCompany.IsSelectAll && UCStationRFDSite.IsSelectAll) ||
                //    (!UCStationThirdCompany.IsSelectAll && !UCStationRFDSite.IsSelectAll))
                //{
                if (!string.IsNullOrEmpty(UCStationRFDSite.SelectExpressCompany))
                    sbCompany.Append(UCStationRFDSite.SelectExpressCompany.Replace(" ", ""));
                //}
            }

            if (sbCompany != null && sbCompany.Length > 0)
                condition.ExpressCompanyID = sbCompany.ToString();

            //condition.ExpressCompanyID = CompanyIdToString(_companyBll.GetExpressByTOPCodCompanyID(sbCompany.ToString()));

            if (!string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
                condition.Sources = UCMerchantSourceTV.SelectMerchantID;

            if (ddlAreaType.SelectedIndex > 0)
                condition.AreaType = ddlAreaType.SelectedValue;
            int acount;
            string waybillNOs = string.Empty;
            waybillNOs = GetWaybillNO(txtWaybillNO.Text.Trim(), out acount);
            if (!string.IsNullOrEmpty(waybillNOs) && (!Regex.Match(waybillNOs, @"^[0-9, ]{1,}$").Success))
            {
                Alert("运单号只能输入数字！");
                return false;
            }
            else if(acount>100)
            {
                Alert("运单号不能超过100单！");
                return false;
            }
            else
            {
                condition.WaybillNO = waybillNOs;
            }
            condition.ReportType = ddlReportType.SelectedValue;
            condition.ShipmentType = ddlShipmentType.SelectedValue;
            condition.DateStr = DateTime.Parse(txtBTime.Text.Trim()).ToString("yyyy-MM-dd");
            condition.DateEnd = DateTime.Parse(txtETime.Text.Trim()).AddDays(1).ToString("yyyy-MM-dd");
            condition.DateType = ddlDateType.SelectedValue;
            condition.HouseType = hfHouseType.Value;//ddlHouseType.SelectedValue;
            condition.HouseCode = UCWareHouseCheckBox.SelectWareHouseIds.Replace(" ", "");
            condition.IsCod = int.Parse(ddlIsCod.SelectedValue);//是否COD
            condition.AmountStr = DataConvert.ToDecimal(txtAmountStr.Text.Trim(), 0);
            condition.AmountEnd = DataConvert.ToDecimal(txtAmountEnd.Text.Trim(), 0);
            return true;
        }
        private string  GetWaybillNO(string waybillnos,out int acount)
        {
            var wno = txtWaybillNO.Text.Trim();
            wno = wno.Replace("\r\n", ",");
            wno = wno.Replace("'", "");
            wno = wno.Replace("，", ",");
            wno = wno.TrimEnd(',');
            var wnos = wno.Split(',').ToList().Where(s => s.Length > 0);
            acount = wnos.Count();
            var renos = string.Join(",", wnos);
            return renos;
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

        private void SearchData(int currentPageIndex)
        {
            gvList.DataSource = null;
            gvList.DataBind();

            PageInfo pi = new PageInfo(50);

            UCPager.PageSize = 50;

            pi.CurrentPageIndex = currentPageIndex;

            DataTable dtSummary;

            DataTable dt = null;

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
            CreateGridViewColumns(dt.Columns,dt);
            gvList.DataSource = dt;

            gvList.DataBind();

            IList<string> columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName);
            }
            BindCheckBoxListColumns(columns);
        }

        private void CreateGridViewColumns(DataColumnCollection dcc,DataTable dt)
        {
            gvList.Columns.Clear();
            foreach (DataColumn dc in dcc)
            {
                if (dc.ColumnName == "明细")
                {
                    TemplateField tf = new TemplateField();
                    tf.HeaderText = "明细";
                    tf.ItemTemplate= new MyTemplate("明细",dt,DataControlRowType.DataRow);
                    
                    gvList.Columns.Add(tf);
                }
                else
                {
                    BoundField bc = new BoundField();
                    bc.DataField = dc.ColumnName;
                    bc.HeaderText = dc.ColumnName;// dc.Caption.ToString();
                    bc.ItemStyle.HorizontalAlign = HorizontalAlign.Left;//居中对齐
                    gvList.Columns.Add(bc);
                }
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
            lbFareStat.Text = dt.Rows[0]["配送费合计"].ToString();
            lbProtectedPriceStat.Text = dt.Rows[0]["保价金额合计"].ToString();
        }

        protected void btExprot_ClickV2(object sender, EventArgs e)
        {
            try
            {
                CodSearchCondition condition;
                if (!JudgeInput(out condition))
                    return;

                //SearchCondition = condition;
                //string exportPath = Server.MapPath("~\\file\\") + DateTime.Now.ToString("yyyyMMddHHmmssms") + "\\LogisticsReport";
                //_bll.SearchExprotDetailData(SearchCondition, exportPath);
                DataTable dt = _bll.SearchExprotDetailDataV2(condition, "");

                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("没有找到可以导出的数据");

                    return;
                }
                //CSVExport.DataTable2Excel(dt, "COD交接明细表");
                //ExcelHelper.ExportByWeb(dt, "COD交接明细表");
                //CSVExport.Export(dt, "物流发货明细表");
                CSVExport.ExportFile(dt, null, null, "物流发货明细表", false);
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
                if (!JudgeInput(out condition))
                    return;

                //SearchCondition = condition;
                //string exportPath = Server.MapPath("~\\file\\") + DateTime.Now.ToString("yyyyMMddHHmmssms") + "\\LogisticsReport";
                //_bll.SearchExprotDetailData(SearchCondition, exportPath);

                DataTable dt = _bll.SearchExprotDetailData(condition, "");
                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("没有找到可以导出的数据");
                    return;
                }
                //CSVExport.DataTable2Excel(dt, "COD交接明细表");
                //ExcelHelper.ExportByWeb(dt, "COD交接明细表");
                //CSVExport.Export(dt, "物流发货明细表");
                CSVExport.ExportFile(dt, null, null, "物流发货明细表", false);
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>" + ex.Message);
            }
        }

        protected void btExprot_ClickV3(object sender, EventArgs e)
        {
            try
            {
                CodSearchCondition condition;
                if (!JudgeInput(out condition))
                    return;


                var asyncTaskHelper = new AsyncTaskHelper<DataTable>();
                DataTable dt = null;
                var startTime = DateTime.Parse(condition.DateStr);
                var endTime = DateTime.Parse(condition.DateEnd);
                for (DateTime d = startTime; d < endTime; )
                {
                    CodSearchCondition conditionObj;
                    JudgeInput(out conditionObj);
                    var ed = d.AddDays(2);
                    conditionObj.DateStr = d.ToString("yyyy-MM-dd");
                    if (ed < endTime)
                        conditionObj.DateEnd = ed.ToString("yyyy-MM-dd");
                    else
                        conditionObj.DateEnd = endTime.ToString("yyyy-MM-dd");

                    asyncTaskHelper.Add(d.ToString("yyyy-MM-dd"), () => _bll.SearchExprotDetailData(conditionObj, ""));

                    d = ed;
                }
                dt = asyncTaskHelper.RunAllAndMerge() as DataTable;
                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("没有找到可以导出的数据");
                    return;
                }
                CSVExport.ExportFileToClient(dt, "物流发货明细表", false, download_token_name.Value, download_token_value.Value);
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>" + ex.Message);
            }
        }
    }

    public class MyTemplate : ITemplate
    {
        private int index = 0;
        private string strColumnName;
        private DataControlRowType dcrtColumnType;
        private DataTable dt;
        public MyTemplate(string strColumnName,DataTable dt, DataControlRowType dcrtColumnType)
        {
            index = 0;
            this.strColumnName = strColumnName;
            this.dcrtColumnType = dcrtColumnType;
            this.dt = dt;
        }

        public void InstantiateIn(Control container)
        {
            switch (dcrtColumnType)
            {
                case DataControlRowType.Header:
                    Literal ltr = new Literal();
                    ltr.Text = strColumnName;
                    container.Controls.Add(ltr);
                    break;
                    break;
                case DataControlRowType.DataRow:
                    LinkButton lbn = new LinkButton();
                    lbn.Text = "日志";
                    lbn.OnClientClick =string.Format(
                        "fnOpenModalDialog('../BasicSetting/OperateLogView.aspx?PK_NO={0}&LogType=8',500,300);return false;", dt.Rows[index]["明细"]);
                    container.Controls.Add(lbn);
                    index++;
                    break;
            }


        }
    }
}
