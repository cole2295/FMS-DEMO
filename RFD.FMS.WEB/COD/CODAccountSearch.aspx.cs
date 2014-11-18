using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL;
using System.Data;

namespace RFD.FMS.WEB.COD
{
	public partial class CODAccountSearch : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {

            }
		}

		protected void ViewAccountDetail_Click(object sender, EventArgs e)
		{
			try
			{
				string accountNo = JudgeCheckedOne("结算单详情");
				if(accountNo.Length<=0) return;
				RunJS(MenuLibrary.AccountDetailViewMenu(accountNo).JsString);
			}
			catch (Exception ex)
			{
				Alert("查看结算单详情失败");
			}
		}

		protected void AreaFare_Click(object sender, EventArgs e)
		{
			try
			{
				string accountNo = JudgeCheckedOne("区域运费");
				if (accountNo.Length <= 0) return;
				RunJS(MenuLibrary.AccountAreaFareMenu(accountNo).JsString);
			}
			catch (Exception ex)
			{
				Alert("查看区域运费失败");
			}
		}

		private string JudgeCheckedOne(string msg)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
			if (keyValuePairs.Count <= 0)
			{
				Alert(string.Format("没有选择需要查询{0}的结算单", msg));
				return "";
			}
			if (keyValuePairs.Count > 1)
			{
				Alert(string.Format("能且只能同时查询一个{0}", msg));
				return "";
			}

			return keyValuePairs[0].Key;
		}

		protected void ExprotMsg_Click(object sender, EventArgs e)
		{
			try
			{
				DataTable dt = Master.GetExportData();
				if (dt == null || dt.Rows.Count <= 0)
				{
					Alert("无导出的数据");
					return;
				}
				List<string> l = new List<string>();
				foreach (DataColumn column in dt.Columns)
				{
					l.Add(column.ColumnName);
				}
				ExportExcel(dt, l.ToArray(), "结算汇总表");
			}
			catch (Exception ex)
			{
				Alert("导出失败<br>" + ex.Message);
			}
		}

		protected void btViewDetail_Click(object sender, EventArgs e)
		{
			try
			{
				string accountNo = JudgeCheckedOne("明细");
				if (accountNo.Length <= 0) return;
				RunJS(MenuLibrary.AccountDetailMenu(accountNo).JsString);
			}
			catch (Exception ex)
			{
				Alert("查看明细失败");
			}
		}

		protected void PrintMsg_Click(object sender, EventArgs e)
		{
			try
			{
				DataTable dt = Master.GetPrintData();
				if (dt == null || dt.Rows.Count <= 0)
				{
					Alert("无打印的数据列表");
					return;
				}
				ExportPDF(dt, Master.gvListPrintColumn, "结算汇总表", Master.gvListPrintColumnWidth);
			}
			catch (Exception ex)
			{
				Alert("打印失败<br>" + ex.Message);
			}
		}
	}
}
