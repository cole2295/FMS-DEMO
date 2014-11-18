using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL;
using System.Data;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.Main;

using RFD.FMS.Util.ControlHelper;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.COD
{
	public partial class CODAccountDetailView : BasePage
	{
        ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				LoadData();

            ucDeliveryHouse.DistributionCode = base.DistributionCode;
            ucReturnHouse.DistributionCode = base.DistributionCode;
            ucVisitReturnHouse.DistributionCode = base.DistributionCode;
		}

		private string AccountNO
		{
			get { return string.IsNullOrEmpty(Request.QueryString["accountNo"]) ? null : Request.QueryString["accountNo"].ToString(); }
			set { lbAccountNO.Text = value; }
		}

		private void LoadData()
		{
			if (string.IsNullOrEmpty(AccountNO))
			{
				Alert("获取结算单号失败");
				return;
			}

			CODSearchCondition searchCondition;
			DataTable dtSearchResult = cODAccountService.AccountSearchByNo(AccountNO, true, out searchCondition);
			if (dtSearchResult == null || dtSearchResult.Rows.Count <= 0 || searchCondition == null)
			{
				RunJS("alert('读取数据错误，结算状态可能已被更改，或被删除');window.close();");
				return;
			}

			AccountNO = searchCondition.AccountNO;
			lbCompanyName.Text = searchCondition.DisplayCompanyName;
            lbMerchantName.Text = searchCondition.DisplayMerchantName;
			lb_D_Date_S.Text = searchCondition.Date_D_S.ToString("yyyy-MM-dd");
			lb_D_Date_E.Text = searchCondition.Date_D_E.ToString("yyyy-MM-dd");
			lb_R_Date_S.Text = searchCondition.Date_R_S.ToString("yyyy-MM-dd");
			lb_R_Date_E.Text = searchCondition.Date_R_E.ToString("yyyy-MM-dd");
			lb_V_Date_S.Text = searchCondition.Date_V_S.ToString("yyyy-MM-dd");
			lb_V_Date_E.Text = searchCondition.Date_V_E.ToString("yyyy-MM-dd");
			ucDeliveryHouse.SelectEnable = false;
			ucReturnHouse.SelectEnable = false;
			ucVisitReturnHouse.SelectEnable = false;
			ucDeliveryHouse.SelectWareHouseIds = searchCondition.HouseD;
			ucReturnHouse.SelectWareHouseIds = searchCondition.HouseR;
			ucVisitReturnHouse.SelectWareHouseIds = searchCondition.HouseV;
			gvList.DataSource = dtSearchResult;
			gvList.DataBind();
		}

		protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				int n = int.Parse(gvList.DataKeys[e.Row.RowIndex].Values[3].ToString());
				switch (n)
				{
					case 2:
						e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#A7DEF5");
						break;
					default:
						e.Row.ForeColor = System.Drawing.Color.Black;
						e.Row.BackColor = System.Drawing.Color.White;
						break;
				}
			}
		}

		protected void btExport_Click(object sender, EventArgs e)
		{
			try
			{
				if (gvList.Rows.Count <= 0)
				{
					Alert("未找到导出数据");
					return;
				}
				List<string> l = new List<string>();
				foreach (DataControlField column in gvList.Columns)
				{
					l.Add(column.HeaderText);
				}
				ExportExcel(GridViewHelper.GridView2DataTable(gvList), l.ToArray(), "结算单明细");
			}
			catch (Exception ex)
			{
				Alert("导出失败");
			}
		}

		protected void btPrint_Click(object sender, EventArgs e)
		{
			try
			{
				if (gvList.Rows.Count <= 0)
				{
					Alert("请查询需要打印的数据列表");
					return;
				}

				string headerText = string.Empty;
				DataTable dt = GridViewHelper.GridView2DataTable(gvList);
				string[] removeColumns = { "DataType", "AccountDetailID" };
				foreach (string s in removeColumns)
				{
					if (dt.Columns.Contains(s))
					{
						dt.Columns.Remove(s);
					}
				}
				List<string> l = new List<string>();
				foreach (DataColumn column in dt.Columns)
				{
					headerText = column.ColumnName;
					l.Add(headerText);
				}
				int[] columnsWidth = { 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90 };

				DateTime now = DateTime.Now;
				string fileName = string.Format(AccountNO+"结算单详情汇总表-{0}.pdf", now.ToString("HHmmss") + now.Millisecond.ToString());
				string dicName = Server.MapPath(string.Format("~/file/PDF/{0}/{1}/{2}/", now.Year, now.Month, now.Day));
				if (!Directory.Exists(dicName))
				{
					Directory.CreateDirectory(dicName);
				}
				Rectangle r = new Rectangle(800, 480);
				Document document = new Document(r);
				PdfWriter.GetInstance(document, new FileStream(dicName + fileName, FileMode.Create));
				document.Open();
				BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
				//字体
                iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 8, Font.NORMAL, new BaseColor(0, 0, 0));
                iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bfChinese, 10, Font.BOLD, new BaseColor(0, 0, 0));
                iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bfChinese, 15, Font.BOLD, new BaseColor(0, 0, 0));
				var pdfTitle = new Paragraph(AccountNO + "结算单详情汇总表", fontTitle);
				pdfTitle.Alignment = iTextSharp.text.Rectangle.ALIGN_CENTER; document.Add(pdfTitle);

				#region 页头
				PdfPTable headText = new PdfPTable(3);
				PdfPCell accountNo = new PdfPCell(new Paragraph("结算单号：" + lbAccountNO.Text, fontChinese));
				accountNo.Border = iTextSharp.text.Rectangle.NO_BORDER;
				headText.AddCell(accountNo);

				PdfPCell companyName = new PdfPCell(new Paragraph("配送商：" + lbCompanyName.Text, fontChinese));
				companyName.Border = iTextSharp.text.Rectangle.NO_BORDER;
				headText.AddCell(companyName);

				PdfPCell merchant = new PdfPCell(new Paragraph("商家：" + lbMerchantName.Text, fontChinese));
				merchant.Border = iTextSharp.text.Rectangle.NO_BORDER;
				headText.AddCell(merchant);

				headText.WidthPercentage = 100;
				document.Add(headText);
				#endregion

				#region 表
				PdfPTable table = new PdfPTable(dt.Columns.Count);
				foreach (string col in l.ToArray())
				{
					PdfPCell cell = new PdfPCell(new Paragraph(col, fontHeader));
					cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
					table.AddCell(cell);
				}
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					for (int j = 0; j < dt.Columns.Count; j++)
					{
						table.AddCell(new Phrase(dt.Rows[i][j].ToString().Replace("&nbsp;", " "), fontChinese));
					}
				}
				table.SetWidths(columnsWidth);
				table.WidthPercentage = 100;
				document.Add(table);
				#endregion

				#region 页尾
				PdfPTable footText = new PdfPTable(3);
				PdfPCell D_Date = new PdfPCell(new Paragraph("发货时间：" + lb_D_Date_S.Text + "~" + lb_D_Date_E.Text, fontChinese));
				D_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
				footText.AddCell(D_Date);

				PdfPCell R_Date = new PdfPCell(new Paragraph("拒收入库时间：" + lb_R_Date_S.Text + "~" + lb_R_Date_E.Text, fontChinese));
				R_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
				footText.AddCell(R_Date);

				PdfPCell V_Date = new PdfPCell(new Paragraph("上门退入库时间：" + lb_V_Date_S.Text + "~" + lb_V_Date_E.Text, fontChinese));
				V_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
				footText.AddCell(V_Date);

				footText.WidthPercentage = 100;
				document.Add(footText);
				#endregion
				document.Close();

				DownLoadFile(dicName, fileName);
			}
			catch (Exception ex)
			{
				Alert("打印失败<br>" + ex.Message);
			}
		}

		private void DownLoadFile(string dicName, string fileName)
		{
			if (!Directory.Exists(dicName))
			{
				Directory.CreateDirectory(dicName);
			}
			FileStream fFileStream = new FileStream(dicName + fileName, FileMode.Open);
			long fFileSize = fFileStream.Length;
			Context.Response.ContentType = "application/octet-stream";
			Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(fileName), System.Text.Encoding.UTF8) + "\"");
			Context.Response.AddHeader("Content-Length", fFileSize.ToString());
			byte[] fFileBuffer = new byte[fFileSize];
			fFileStream.Read(fFileBuffer, 0, (int)fFileSize);
			fFileStream.Close();
			Context.Response.BinaryWrite(fFileBuffer);
			Context.Response.End();
			File.Delete(dicName + fileName);
		}
	}
}
