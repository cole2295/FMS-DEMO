using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;
using System.Data;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util.ControlHelper;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class AccountAreaSummary : BasePage
	{
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadData();
			}
		}

		public string AccountNO
		{
			get { return Request.QueryString["accountNo"] == null ? "" : Request.QueryString["accountNo"].ToString(); }
		}

		private void LoadData()
		{
			if (AccountNO == "")
			{
				RunJS("alert('读取数据错误，结算状态可能已被更改，或被删除');fnCloseNewPage('" + MenuLibrary.IncomeAccountAreaSummaryMenu("").MenuID + "');");
				return;
			}

            DataTable dtSummary = incomeAccountService.GetAccountAreaSummary(AccountNO);

			if (dtSummary == null || dtSummary.Rows.Count<=0)
			{
				RunJS("alert('读取数据错误，结算状态可能已被更改，或被删除');fnCloseNewPage('" + MenuLibrary.IncomeAccountAreaSummaryMenu("").MenuID + "');");
				return;
			}

            IncomeSearchCondition condition = incomeAccountService.GetAccountSearchCondition(AccountNO);

			gvList.DataSource = dtSummary;
			gvList.DataBind();

			lbAccountNO.Text = AccountNO;
			if (condition == null)
				return;

			lbMerchant.Text = condition.MerchantName;
			lbDateStr.Text = condition.DateStr.ToString("yyyy-MM-dd");
			lbDateEnd.Text = condition.DateEnd.ToString("yyyy-MM-dd");
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
				List<string> l = new List<string>();
				foreach (DataControlField column in gvList.Columns)
				{
					headerText = column.HeaderText;
					l.Add(headerText);
				}
				int[] columnsWidth = { 150, 150, 150, 150, 150, 150, 150, 150, 150, 150,150,150 };

				DateTime now = DateTime.Now;
				string fileName = string.Format("收入结算区域汇总表-{0}.pdf", now.ToString("HHmmss") + now.Millisecond.ToString());
				string dicName = Server.MapPath(string.Format("~/file/PDF/{0}/{1}/{2}/", now.Year, now.Month, now.Day));
				if (!Directory.Exists(dicName))
					Directory.CreateDirectory(dicName);
				Document document = new Document();
				PdfWriter.GetInstance(document, new FileStream(dicName + fileName, FileMode.Create));
				document.Open();
				BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
				//字体
				iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 8, Font.NORMAL, new BaseColor(0, 0, 0));
                iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bfChinese, 10, Font.BOLD, new BaseColor(0, 0, 0));
                iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bfChinese, 15, Font.BOLD, new BaseColor(0, 0, 0));
				var pdfTitle = new Paragraph(AccountNO + "收入结算区域汇总表", fontTitle);
				pdfTitle.Alignment = iTextSharp.text.Rectangle.ALIGN_CENTER; document.Add(pdfTitle);

				#region 页头
				PdfPTable headText = new PdfPTable(3);
				PdfPCell accountNo = new PdfPCell(new Paragraph("结算单号：" + lbAccountNO.Text, fontChinese));
				accountNo.Border = iTextSharp.text.Rectangle.NO_BORDER;
				headText.AddCell(accountNo);

				PdfPCell companyName = new PdfPCell(new Paragraph("商家：" + lbMerchant.Text, fontChinese));
				companyName.Border = iTextSharp.text.Rectangle.NO_BORDER;
				headText.AddCell(companyName);

				PdfPCell R_Date = new PdfPCell(new Paragraph("时间：" + lbDateStr.Text + " ~ " + lbDateEnd.Text, fontChinese));
				R_Date.Border = iTextSharp.text.Rectangle.NO_BORDER;
				headText.AddCell(R_Date);

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

				document.Close();

				DownLoadFile(dicName, fileName);
			}
			catch (Exception ex)
			{
				Alert("打印失败");
			}
		}

		private void DownLoadFile(string dicName, string fileName)
		{
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

		protected void btExprot_Click(object sender, EventArgs e)
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
				ExportExcel(GridViewHelper.GridView2DataTable(gvList), l.ToArray(), "收入结算区域汇总表");
			}
			catch (Exception ex)
			{
				Alert("导出失败");
			}
		}
	}
}
