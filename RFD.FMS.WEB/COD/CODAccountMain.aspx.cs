using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL;
using System.Data;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEB.COD
{
	public partial class CODAccountMain : BasePage
	{
        ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {

            }
		}

		protected void AddAccount_Click(object sender, EventArgs e)
		{
			RunJS(MenuLibrary.AccountEditMenu("").JsString);
		}

		protected void EditAccount_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;

			if (!JudgeListCheck(keyValuePairs,0))
				return;

			if (keyValuePairs.Count > 1)
			{
				Alert("只能同时编辑一条结算");
				return;
			}
			RunJS(MenuLibrary.AccountEditMenu(keyValuePairs[0].Key).JsString);
		}

		protected void DeleteAccountNo_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;

			if (!JudgeListCheck(keyValuePairs,0))
				return;

			if (cODAccountService.DeleteAccountNo(keyValuePairs, Userid.ToString()))
			{
				Alert("删除成功");
				Master.BingGridViewList(-1);
			}
			else
				Alert("删除失败");
		}

		private bool JudgeListCheck(IList<KeyValuePair<string, string>> keyValuePairs,int n)
		{
			if (keyValuePairs.Count <= 0)
			{
				Alert("没有选择需要操作的结算单");
				return false;
			}
			if (n == 0)
			{
				foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
				{
					if (keyValuePair.Value == EnumHelper.GetDescription(EnumAccountAudit.A2) ||
						keyValuePair.Value == EnumHelper.GetDescription(EnumAccountAudit.A4)
						)
					{
						Alert("不能操作结算待审核、已结算的结算单");
						return false;
					}
				}
			}
			return true;
		}

		protected void AreaFare_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;

			if (!JudgeListCheck(keyValuePairs,1))
				return;

			if (keyValuePairs.Count > 1)
			{
				Alert("只能同时查看一条结算");
				return;
			}
			RunJS(MenuLibrary.AccountAreaFareMenu(keyValuePairs[0].Key).JsString);
		}

		protected void ExprotMsg_Click(object sender, EventArgs e)
		{
			try
			{
				DataTable dt = Master.GetExportData();
				if (dt==null || dt.Rows.Count<= 0)
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
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;

			if (keyValuePairs.Count <= 0)
			{
				Alert("没有选择需要查看的结算单");
				return;
			}

			if (keyValuePairs.Count > 1)
			{
				Alert("只能同时看看一条结算明细");
				return;
			}

			RunJS(MenuLibrary.AccountDetailMenu(keyValuePairs[0].Key).JsString);
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
