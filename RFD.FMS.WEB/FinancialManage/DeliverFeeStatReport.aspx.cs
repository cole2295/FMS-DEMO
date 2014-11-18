using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using Org.BouncyCastle.Asn1.Ocsp;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class DeliverFeeStatReport : BasePage
    {
        IDeliverFeeStatService _feeService = RFD.FMS.Util.ServiceLocator.GetService<IDeliverFeeStatService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                txtBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                BindDll();
            }
        }

        private void BindDll()
        {
            BindOrderSource();
            BindMerchants(ddlMerchantList);
            //省份
            ddlProvince.DataSource = _feeService.GetProvince();
            ddlProvince.DataTextField = "ProvinceName";
            ddlProvince.DataValueField = "ProvinceID";
            ddlProvince.DataBind();
            ddlProvince.Items.Insert(0, new ListItem("---请选择---", ""));
           
        }
        #region 绑定订单来源
        private void BindOrderSource()
        {
            var dao = RFD.FMS.Util.ServiceLocator.GetService<IStatusInfoService>();
            this.ddlOrderSource.DataSource = dao.GetStatusInfoByTypeNo(3);
            this.ddlOrderSource.DataTextField = "StatusName";
            this.ddlOrderSource.DataValueField = "StatusNo";
            this.ddlOrderSource.DataBind();
            //插入"请选择"默认项
            ListItem li = new ListItem("--请选择--", "");
            this.ddlOrderSource.Items.Insert(0, li);
        }
        #endregion
        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //MerchantID,ExpressCompanyID,Sources
            lblText.Text = gv.DataKeys[e.NewEditIndex]["ExpressCompanyID"].ToString() + gv.DataKeys[e.NewEditIndex]["Sources"].ToString() + gv.DataKeys[e.NewEditIndex]["MerchantID"].ToString();
            string strQueryType = gv.Rows[e.NewEditIndex].Cells[2].Text.Trim();
            string[] strTimeList = strQueryType.Split('~');
            
            Hashtable ht = new Hashtable();
            ht.Add("Sources", gv.DataKeys[e.NewEditIndex]["Sources"]);
            ht.Add("MerchantID", gv.DataKeys[e.NewEditIndex]["MerchantID"]);
            ht.Add("StationID", gv.DataKeys[e.NewEditIndex]["ExpressCompanyID"]);
            if (strTimeList.Length > 1)
            {
                ht.Add("BegTime", strTimeList[0] + " 00:00:00");
                ht.Add("EndTime", strTimeList[1] + " 23:59:59");
            }
            else
            {
                ht.Add("BegTime", strTimeList[0] + " 00:00:00");
                ht.Add("EndTime", strTimeList[0] + " 23:59:59");
            }

            DataTable dtDetailData = _feeService.GetDetail(ht);
            
            ExportExcel(dtDetailData, null, gv.Rows[e.NewEditIndex].Cells[2].Text.Trim()+"配送费结算明细表");
                




        }

        private DataTable GetDetailData()
        {
            DataTable dt=new DataTable();
            return dt;
        }

        protected void DrpProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            //城市
            ddlCity.Items.Clear();
            ddlCity.DataSource = _feeService.GetCity(ddlProvince.SelectedValue);
            ddlCity.DataTextField = "CityName";
            ddlCity.DataValueField = "CityID";
            ddlCity.DataBind();
            ddlCity.Items.Insert(0, new ListItem("---请选择---", ""));
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            //城市
            ddlStation.Items.Clear();
            ddlStation.DataSource = _feeService.GetStationList(ddlCity.SelectedValue);
            ddlStation.DataTextField = "CompanyName";
            ddlStation.DataValueField = "ExpressCompanyID";
            ddlStation.DataBind();
            ddlStation.Items.Insert(0, new ListItem("---请选择---", ""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        private void BindData()
        {
            DataTable dt = _feeService.GetQueryData(GetQueryCon());
            GetSumData(dt);
            gv.DataSource = dt;
            gv.DataBind();
        }

        private Hashtable GetQueryCon()
        {
            Hashtable ht=new Hashtable();
            ht.Add("Sources",ddlOrderSource.SelectedValue);
            ht.Add("MerchantID",ddlMerchantList.SelectedValue);
            ht.Add("QueryType",rbtnReportType.SelectedValue);
            ht.Add("BegTime", txtBeginTime.Text.Trim()+" 00:00:00");
            ht.Add("EndTime", txtEndTime.Text.Trim() + " 23:59:59");
            ht.Add("ProvinceID", ddlProvince.SelectedValue.Trim());
            ht.Add("CityID", ddlCity.SelectedValue.Trim());
            ht.Add("StationID", ddlStation.SelectedValue.Trim());
            return ht;
        }

        private void GetSumData(DataTable dt)
        {
            if(dt.Rows.Count>0)
            {
                int SuccessSum =0;
                decimal SuccessFeeSum =decimal.Zero;
                int RefuseSum =0;
                decimal RefuseFeeSum =decimal.Zero;
                decimal SumFeeSum =decimal.Zero;
                decimal SumProtectedprice = decimal.Zero;
                foreach (DataRow dr in dt.Rows)
                {
                    SuccessSum += Convert.ToInt32(dr["success"]);
                    SuccessFeeSum += Convert.ToString(dr["SuccessDeliverFee"]) == "" ? 0 : Convert.ToDecimal(dr["SuccessDeliverFee"]);
                    RefuseSum += Convert.ToInt32(dr["Refuse"]);
                    RefuseFeeSum += Convert.ToString(dr["RefuseDeliverFee"]) == "" ? 0 : Convert.ToDecimal(dr["RefuseDeliverFee"]);
                    SumFeeSum += Convert.ToString(dr["SumDeliverFee"]) == "" ? 0 : Convert.ToDecimal(dr["SumDeliverFee"]);
                    SumProtectedprice += Convert.ToString(dr["Protectedprice"]) == "" ? 0 : Convert.ToDecimal(dr["Protectedprice"]);
                }

                if(Convert.ToDateTime(txtBeginTime.Text).ToString("yyyy-dd-mm")==Convert.ToDateTime(txtEndTime.Text).ToString("yyyy-dd-mm"))
                {
                    lblReportCycle.Text = Convert.ToDateTime(txtBeginTime.Text).ToString("yyyy-MM-dd");
                }
                else
                {
                    lblReportCycle.Text = Convert.ToDateTime(txtBeginTime.Text).ToString("yyyy-MM-dd") +"~"+
                                          Convert.ToDateTime(txtEndTime.Text).ToString("yyyy-MM-dd");
                    
                    
                }

                lblSuccessCount.Text = SuccessSum.ToString();
                lblSuccessFeeSum.Text = SuccessFeeSum.ToString("n");
                lblFailCount.Text = RefuseSum.ToString();
                lblFailFeeSum.Text = RefuseFeeSum.ToString("n");
                lblSumCount.Text = (SuccessSum + RefuseSum).ToString();
                lblProtectedprice.Text = SumProtectedprice.ToString("n");
                lblFeeSum.Text = (SuccessFeeSum + RefuseFeeSum + SumProtectedprice).ToString("n");
            }
            else
            {
                lblSuccessCount.Text = "";
                lblSuccessFeeSum.Text = "";
                lblFailCount.Text = "";
                lblFailFeeSum.Text = "";
                lblSumCount.Text = "";
                lblFeeSum.Text = "";
            }
        }

        protected void btnSearchDetails_Click(object sender, EventArgs e)
        {
            var dt = GridViewHelper.GridView2DataTable(gv);
            dt.Columns.Remove("导出明细");
            CSVExport.DataTable2Excel(dt, "配送费结算统计报表");
        }
    }
}
