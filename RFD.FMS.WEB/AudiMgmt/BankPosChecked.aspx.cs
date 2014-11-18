using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.AudiMgmt;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.AudiMgmt
{
	public partial class BankPosChecked : BasePage
	{
        private IBankPosCheckService ser = ServiceLocator.GetService<IBankPosCheckService>();

		private DataTable dtIn;//导入数据
		private DataTable OrderForm;//系统数据
		private DataTable dtReturn;//成功数据
		private DataTable dtLost;//失败数据
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				txtBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				BindUIData();
			}
			ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "buttons", "enableButtons();", true);
		}

		private void BindUIData()
		{
			ddlStation.DataSource = ser.GetOrderMoneyStoreInfo();
			ddlStation.DataValueField = "ExpressCompanyID";
			ddlStation.DataTextField = "CompanyName";
			ddlStation.DataBind();
			ListItem li = new ListItem("全部", "");
			ddlStation.Items.Insert(0, li);
		}

		private DataSet GetExcelData(string filename)
		{
			DataSet dSet = new DataSet();
			//string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filename + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
            
            string conStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0 Xml;HDR=YES'";

            OleDbConnection conn = new OleDbConnection(conStr);
			OleDbDataAdapter oda = new OleDbDataAdapter("select * from [汇总$]", conn);
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

		protected void btnBatchQuery_Click(object sender, EventArgs e)
		{
			if (fileUpload.HasFile)
			{
				//Response.Write(fileUpload.PostedFile.FileName + "<br />");

                fileUpload.PostedFile.SaveAs(Server.MapPath("/UpFile/") + "bankposcheck.xls");
                DataSet dset = GetExcelData(Server.MapPath("/UpFile/bankposcheck.xls"));
				if (dset.Tables.Count > 0)
				{
					if (dset.Tables[0].Columns.Count == 5)
					{
						try
						{
							DataTable dt = dset.Tables[0];
							foreach (DataColumn dc in dt.Columns)
							{
								dc.ColumnName = dc.ColumnName.Trim();
							}
							dtIn = dt;
							dtIn = CleanDataTable(dtIn);

							ViewState["InData"] = dtIn;
							this.hidTotalCount.Value = dtIn.Rows.Count.ToString();
							gvInData.DataSource = dtIn;
							gvInData.DataBind();
						}
						catch (Exception ee)
						{
							Alert("数据格式不正确" + ee.Message);
						}
					}
					else
					{
						Alert("数据格式不正确");
					}
				}
				else
				{
					Alert("没有数据");
				}
			}
		}

		protected void btnCheck_Click(object sender, EventArgs e)
		{
			dtIn = (DataTable)ViewState["InData"];

			OrderForm = ser.GetCheckData(new SearchCondition()
			{
				BeginTime = Convert.ToDateTime(this.txtBeginTime.Text.Trim()),
				EndTime = Convert.ToDateTime(this.txtEndTime.Text.Trim()),
				DeliverStation = ddlStation.SelectedIndex == 0 ? -1 : Convert.ToInt32(ddlStation.SelectedValue)
			});

			GetResult();

			gvSucData.DataSource = dtReturn;
			gvSucData.DataBind();

			int drc = dtReturn.Rows.Count;
			this.hidSuccessCount.Value = drc.ToString();

			lblSucTitle.Text = string.Format(@"核对成功结果 核对成功{0}条记录", drc);


			gvFailData.DataSource = dtLost;
			gvFailData.DataBind();

			int dlc = dtLost.Rows.Count;

			this.hidFailCount.Value = dlc.ToString();
			lblFailTitle.Text = string.Format(@"核对失败结果 核对失败{0}条记录", dlc);
			lblInTitle.Text = string.Format(@"导入列表 核对完成：{0}/{1}", dtIn.Rows.Count, dtIn.Rows.Count);
		}

		private DataTable CleanDataTable(DataTable dt)
		{
			DataTable result = CreateDataTable();
			//DataRow[] dr = dt.Select("F4<>'' AND F2<>'卡号'");
			DataRow[] dr = dt.Select(" 1=1");

			for (int i = 0; i < dr.Length; i++)
			{
				DataRow drnew = result.NewRow();
				drnew["Seq"] = i + 1;
				drnew["ClientCode"] = dr[i][0].ToString();
				drnew["CardNumber"] = dr[i][1].ToString();
				drnew["TradeDate"] = dr[i][2].ToString();
				drnew["TradeTime"] = dr[i][3].ToString();
				drnew["TradeMoney"] = dr[i][4].ToString();
				result.Rows.Add(drnew);
			}
			return result;
		}


		private DataTable CreateDataTable()
		{
			DataTable result = new DataTable();
			result.Columns.Add("ClientCode", typeof(string));
			result.Columns.Add("CardNumber", typeof(string));
			result.Columns.Add("TradeDate", typeof(string));
			result.Columns.Add("TradeTime", typeof(string));
			result.Columns.Add("TradeMoney", typeof(decimal));
			result.Columns.Add("Result", typeof(string));
			result.Columns.Add("Seq", typeof(int));
			result.Columns.Add("OrderForm", typeof(string));
			result.Columns.Add("ExpressCompanyName", typeof(string));
			return result;
		}
		private void GetResult()
		{
			dtReturn = dtIn.Clone();

			dtLost = OrderForm.Clone();
			//dtLost.Columns.Add("Result");
			int c = 0;
			ArrayList listOtherFormcode = new ArrayList();
			foreach (DataRow dr in dtIn.Rows)
			{
				DataRow[] drtemp = OrderForm.Select("POSCode='" + dr["ClientCode"] + "' and FactAmount='" + dr["TradeMoney"] + "'");
				if (drtemp.Length != 0)
				{//核对结果数量不为0										
					int i = GetPosCodes(dr["TradeMoney"].ToString(), dr["ClientCode"].ToString());//计算导入数据中该终端相同金额记录的条数

					if (i == drtemp.Length)
					{//如果数据库中找到的数据条数==Excel中的核对数据条数 则成功
						DataRow drnew = dtReturn.NewRow();
						drnew = dr;
						drnew["OrderForm"] = drtemp[0]["WaybillNO"].ToString();
						drnew["ExpressCompanyName"] = drtemp[0]["CompanyName"].ToString();
						drnew["Result"] = "核对成功";
						dtReturn.Rows.Add(drnew.ItemArray);
						listOtherFormcode.Add(drtemp[0]["WaybillNO"].ToString());

					}

				}

				c++;

			}
			//查询其他订单信息
			string sOtherFormcode = "";
			for (int i = 0; i < listOtherFormcode.Count; i++)
			{
				sOtherFormcode = sOtherFormcode + listOtherFormcode[i].ToString() + ",";
			}
			if (listOtherFormcode.Count > 0)
			{
				sOtherFormcode = sOtherFormcode.TrimEnd(',');
			}
			if (sOtherFormcode == "")
			{
				sOtherFormcode = "0";
			}
			DataRow[] drOtherCode = OrderForm.Select("WaybillNO not in (" + sOtherFormcode + ")");
			if (drOtherCode.Length != 0)
			{
				foreach (DataRow dr in drOtherCode)
				{
					DataRow drnew = dtLost.NewRow();
					drnew["POSCode"] = dr["POSCode"].ToString();
					//drnew["PostTime"] = dr["PostTime"].ToString();
					drnew["FactAmount"] = dr["FactAmount"].ToString();
					drnew["WaybillNO"] = dr["WaybillNO"].ToString();
					drnew["CompanyName"] = dr["CompanyName"].ToString();
					drnew["Result"] = "核对失败";
					dtLost.Rows.Add(drnew.ItemArray);
				}
			}

		}
		private int GetPosCodes(string tradeMoney, string clientCode)
		{
			DataRow[] dr = dtIn.Select("TradeMoney='" + tradeMoney + "' and ClientCode='" + clientCode + "'");
			return dr.Length;
		}

		protected void btnSucOutExcel_Click(object sender, EventArgs e)
		{
			var dt = GridViewHelper.GridView2DataTable(gvSucData);
			if (dt.Rows.Count > 0)
			{
				ExportExcel(dt, null, "核对结果成功数据");
			}
		}

		protected void btnSucPDF_Click(object sender, EventArgs e)
		{
			var dt = GridViewHelper.GridView2DataTable(gvSucData);
			if (dt.Rows.Count > 0)
			{
				ExportPDF(dt, null, "核对结果成功数据", new int[] { 10, 15, 15, 15, 15, 15, 15 });
			}
		}

		protected void btnFailExcel_Click(object sender, EventArgs e)
		{
			var dt = GridViewHelper.GridView2DataTable(gvFailData);
			if (dt.Rows.Count > 0)
			{
				ExportExcel(dt, null, "核对结果失败数据");
			}
		}

		protected void btnFailPDF_Click(object sender, EventArgs e)
		{
			var dt = GridViewHelper.GridView2DataTable(gvFailData);
			if (dt.Rows.Count > 0)
			{
				ExportPDF(dt, null, "核对结果成功数据", new int[] { 15, 15, 20, 15, 15, 20 });
			}
		}
	}
}
