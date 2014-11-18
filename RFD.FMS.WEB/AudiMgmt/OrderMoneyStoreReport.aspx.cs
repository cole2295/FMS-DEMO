using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEB.UserControl;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.AudiMgmt;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.AudiMgmt
{
    public partial class OrderMoneyStoreReport : FMSBasePage
	{
        IMoneyStoreService Service = ServiceLocator.GetService<IMoneyStoreService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindOrderSource();
				BindMerchants(this.ddlMerchantList);
				//txtIntoStationTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
				//txtSignTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
				txtBegTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				btnReportData.Enabled = false;
				if (Request.QueryString["IsEdit"] != null && Request.QueryString["IsEdit"] == "1")
				{
					gv.Columns[20].Visible = true;
				}
				else
				{
					gv.Columns[20].Visible = false;
				}

			}
			//ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "init", "init();", true);
			pager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
			//ShowPager(pager, hidTotalCount.Value);
		}

		/// <summary>
		/// 翻页事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AspNetPager_PageChanged(object sender, EventArgs e)
		{
			if (TotalData == null)
			{
				BindData();
			}
			else
			{
				BindDataWithBuildPage(TotalData, pager, gv);
			}
		}
		///// <summary>
		///// 注册分页事件并显示分页控件
		///// </summary>
		//private void ShowPager(Pager pager, string recordCount)
		//{
		//    var count = recordCount.ConvertToInt(0);
		//    pager.Visible = count > pager.PageSize;
		//}
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
		protected void btnQuery_Click(object sender, EventArgs e)
		{
            try
            {
                BindData();
            }
            catch (Exception ex)
            {
                Alert("查询错误<br>"+ex.Message);
            }
		}

		private void BindData()
		{
			TotalData = null;
			lblMessage.Text = "";

			hidBatchData.Value = "0";
			btnReportData.Enabled = false;
			Hashtable ht = GetQueryPre(txtWayBillNO.Text.Trim(), true);
			DataTable dt = Service.GetOrderMoneyStoreInfo(ht);
			if (dt != null && dt.Rows.Count > 0)
			{
				//分页
				var rowCount = Convert.ToInt32(dt.Rows[0]["RowCount"].ToString());
				pager.RecordCount = rowCount;
				BindSumData(dt);
				gv.DataSource = dt;
				gv.DataBind();
				btnReportData.Enabled = true;
			}
		}

		private void BindSumData(DataTable dt)
		{
			int iInBoundSum = 0;
			int iNotInBoundSum = 0;
			int iMoneyStoreSum = 0;
			int iNotMoneyStoreSim = 0;
			if (dt.Rows.Count > 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					if (Convert.ToString(dr["FinancialStatusID"]) == "1")
					{
						iMoneyStoreSum++;
					}
					else
					{
						iNotMoneyStoreSim++;
					}

					if (Convert.ToString(dr["BackStatus"]) == "6" || Convert.ToString(dr["BackStatus"]) == "7")
					{
						iInBoundSum++;
					}
					else
					{
						iNotInBoundSum++;
					}
				}
			}
			lblMoneyOrderCount.Text = iMoneyStoreSum.ToString();
			lblNoMoneyOrderCount.Text = iNotMoneyStoreSim.ToString();
			lblInBoundCount.Text = iInBoundSum.ToString();
			lblNotInBoundCount.Text = iNotInBoundSum.ToString();

		}
		private Hashtable GetQueryPre(string waybillNOList, bool isPageing)
		{
			string strWaybillNOList = waybillNOList;
			string strStationID = string.Empty;

			strStationID = station.ID;
			if (((HiddenField)station.FindControl("HidUserControlStationID")).Value.Trim() != null)
			{
				strStationID = ((HiddenField)station.FindControl("HidUserControlStationID")).Value.Trim();
			}
			//if(ISBatch==true)
			//{
			//    strWaybillNOList = txtWayBillNO.Text.Trim();
			//}
			//else
			//{
			//    strWaybillNOList = txtWayBillNO.Text.Trim();
			//}
			var code = default(int);
			if (!int.TryParse(strStationID, out code))
			{
				strStationID = "";
			}

			Hashtable ht = new Hashtable();
			ht.Add("WaybillNO", strWaybillNOList);
			ht.Add("StationID", strStationID);
			ht.Add("IntoBound", ddlInBoundStatus.SelectedValue.Trim());
			ht.Add("MoneyStatus", ddlMoneyStatus.SelectedValue.Trim());
			ht.Add("IntoTime", txtIntoStationTime.Text.Trim());
			ht.Add("SignTime", txtSignTime.Text.Trim());
			ht.Add("BegTime", txtBegTime.Text.Trim());
			ht.Add("EndTime", txtEndTime.Text.Trim());
			ht.Add("Source", ddlOrderSource.SelectedValue.Trim());
			ht.Add("MerchantID", ddlMerchantList.SelectedValue.Trim());
			ht.Add("IsPageing", isPageing ? "1" : "0");
			if (isPageing)
			{
				ht.Add("StartIndex", (pager.CurrentPageIndex - 1) * pager.PageSize + 1);
				ht.Add("EndIndex", pager.CurrentPageIndex * pager.PageSize);
			}
			return ht;
		}

        #region 批量查询
        protected void btnBatchQuery_Click(object sender, EventArgs e)
		{
            try
            {
                //var code = default(int);
                //lblMessage.Text = "";
                //if (!int.TryParse(station.ID, out code))
                //{
                //    lblMessage.Text = "<font color='red'>请选择配送站</font>";
                //    return;
                //}
                if (String.IsNullOrEmpty(txtFile.Value))
                {
                    lblMessage.Text = "<font color='red'>请选择模板</font>";
                    return;
                }

                //Response.Write(fileUpload.PostedFile.FileName + "<br />");
                //DataSet dset = GetExcelData(fileUpload.PostedFile.FileName);
                //fileUpload.PostedFile.SaveAs(Server.MapPath("/UpFile/") + "cwskhrkcx.xls");
                var data = ImportExcel(txtFile);

                //if (data.Rows.Count > 50)

                //DataSet dset = GetExcelData(Server.MapPath("/UpFile/cwskhrkcx.xls"));
                //if (dset.Tables.Count <= 0)
                //{
                //    lblMessage.Text = "<font color='red'>没有数据</font>";
                //    return;
                //}
                if (data.Columns[0].ColumnName == "订单号")
                {
                    //string  = "";
                    StringBuilder strWaybillNOList = new StringBuilder();

                    foreach (DataRow dr in data.Rows)
                    {
                        if (String.IsNullOrEmpty(strWaybillNOList.ToString()))
                        {
                            strWaybillNOList.Append(" insert into #t_orderform(formcode) VALUES (" + Convert.ToString(dr["订单号"]) + ") ");
                        }
                        else
                        {
                            strWaybillNOList.Append(" insert into #t_orderform(formcode) VALUES (" + Convert.ToString(dr["订单号"]) + ") ");
                        }
                    }

                    TotalData = Service.GetBatchQuery(strWaybillNOList.ToString());

                    hidBatchData.Value = "1";
                    btnReportData.Enabled = false;
                    BindSumData(TotalData);
                    BindDataWithBuildPage(TotalData, pager, gv);
                    if (TotalData != null && TotalData.Rows.Count > 0)
                    {
                        btnReportData.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
		}

        protected void btnBatchQueryV2_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtFile.Value))
                {
                    lblMessage.Text = "<font color='red'>请选择模板</font>";
                    return;
                }
                var data = ImportExcel(txtFile);
                if (data.Columns[0].ColumnName == "运单号" && data.Columns[1].ColumnName == "订单号")
                {
                    TotalData = Service.GetBatchQueryV2(BuildSearchCondition(SearchType.AllDetails));

                    hidBatchData.Value = "1";
                    btnReportData.Enabled = false;
                    BindSumData(TotalData);
                    BindDataWithBuildPage(TotalData, pager, gv);
                    if (TotalData != null && TotalData.Rows.Count > 0)
                    {
                        btnReportData.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
        }

        protected override SearchCondition BuildSearchCondition(SearchType searchType)
        {
            return new SearchCondition()
            {
                ExportPath = Server.MapPath("~\\file\\") + DateTime.Now.ToString("yyyyMMddHHmmssms") + "\\入库状态查询",
                OrderNoList = GetOrderNoList(),
                SearchWaybillType=rbWaybillno.Checked==true?0:1,
            };
        }

        private DataTable GetOrderNoList()
        {
            var orders = ImportExcel(this.txtFile);
            if (!orders.IsEmpty())
            {
                if (!orders.Columns.Contains("订单号"))
                {
                    Alert("请你先下载模板到本地，然后填写相关的订单信息再进行导入！");
                    return null;
                }
                orders.RemoveEmptyRow();
            }
            if (orders == null)
            {
                lblMessage.Text = "没有单号";
            }
            else
            {
                lblMessage.Text = "查询" + orders.Rows.Count.ToString() + "单";
            }
            return orders;
        }
        #endregion
        /*
		private DataSet GetExcelData(string filename)
		{
			DataSet dSet = new DataSet();
			//string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filename + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";


			string conStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0 Xml;HDR=YES'";

			OleDbConnection conn = new OleDbConnection(conStr);
			OleDbDataAdapter oda = new OleDbDataAdapter("select * from [BatchQueryData$]", conn);
			try
			{
				oda.Fill(dSet, "excel");
			}
			catch (Exception ex)
			{
				Response.Write(ex.Message);
			}
			return dSet;
		}
		*/
		protected void btnReportData_Click(object sender, EventArgs e)
		{
			if (hidBatchData.Value == "0")//查询导出
			{
				//hemingyu 2011-08-31 
				Hashtable ht = GetQueryPre(txtWayBillNO.Text.Trim(), false);
				DataTable data = Service.GetOrderMoneyStoreInfo(ht);
				if (data != null && data.Rows.Count > 0)
				{
					ExportExcel(data, null, "财务收款和入库状态明细表");
				}
			}
			else
			{
				if (TotalData == null)//批量查询导出
				{
					Alert("请重新导入！");
					return;
				}
				var columns = GridViewHelper.GetGridViewHeaders(gv, new string[] { "确认" });
				var removeColumns = new string[] { "WaybillSignInfoID", "SignStatus", "FinancialStatusID", "Status", "WaybillType", "BackStatus" };
				ExportExcel(TotalData, columns, removeColumns, "财务收款和入库状态明细表");
			}
		}

		protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
		{
			string strStatus = gv.Rows[e.NewEditIndex].Cells[7].Text.Trim();

			if (strStatus != "妥投")
			{
				Alert("确认状态必须为妥投！");
				return;
			}
			string strSignId = gv.DataKeys[e.NewEditIndex].Value.ToString();

			if (Service.UpdateFinancialStatus(strSignId))
			{
				Alert("修改状态成功！");
			}
			else
			{
				Alert("修改状态失败！");
			}
			BindData();
			//lblMessage.Text = "<font color='red'>修改状态成功！</font>";
		}


		//protected void gv_PageIndexChanged(object sender, EventArgs e)
		//{
		//    var pager = sender as Wuqi.Webdiyer.AspNetPager;
		//    if (totalPager.InnerPager.Equals(pager))
		//    {
		//        BindDataWithBuildPage(TotalData, totalPager, gvSummary);
		//        return;
		//    }
		//}

		public DataTable TotalData
		{
			get
			{
				return Session["TotalData"] as DataTable;
			}
			set
			{
				Session["TotalData"] = value;
			}
		}
	}
}
