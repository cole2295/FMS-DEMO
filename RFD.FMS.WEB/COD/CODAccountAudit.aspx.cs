﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using System.Data;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEB.COD
{
	public partial class CODAccountAudit : BasePage
	{
        ICODAccountService cODAccountService = ServiceLocator.GetService<ICODAccountService>();

		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
                
            }
		}

		protected void Reset_Click(object sender, EventArgs e)
		{
			try
			{
				IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
				if (!JudgeListCheck(keyValuePairs, EnumHelper.GetDescription(EnumAccountAudit.A3)))
					return;

                if (cODAccountService.UpdateAccountAuditStatus(keyValuePairs, (int)EnumAccountAudit.A3, Userid.ToString()))
				{
					Alert("置回成功");
					Master.BingGridViewList(-1);
				}
				else
					Alert("置回失败");
			}
			catch (Exception ex)
			{
				Alert("置回失败<br>" + ex.Message);
			}
		}

		protected void Audit_Click(object sender, EventArgs e)
		{
			try
			{
				IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
				if (!JudgeListCheck(keyValuePairs, EnumHelper.GetDescription(EnumAccountAudit.A4)))
					return;

				if (cODAccountService.UpdateAccountAuditStatus(keyValuePairs, (int)EnumAccountAudit.A4, Userid.ToString()))
				{
					Alert("审核成功");
					Master.BingGridViewList(-1);
				}
				else
					Alert("审核失败");
			}
			catch (Exception ex)
			{
				Alert("审核失败<br>" + ex.Message);
			}
		}

		private bool JudgeListCheck(IList<KeyValuePair<string, string>> keyValuePairs,string statusStr)
		{
			if (keyValuePairs.Count <= 0)
			{
				Alert("没有选择需要操作的结算单");
				return false;
			}
			foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
			{
				if (keyValuePair.Value == statusStr)
				{
					Alert(string.Format("选中结算单不能包含 {0} 结算单", statusStr));
					return false;
				}

				if (keyValuePair.Value != EnumHelper.GetDescription(EnumAccountAudit.A2))
				{
					Alert(string.Format("只能选择结算待审核的结算单操作"));
					return false;
				}
			}

			return true;
		}

		protected void ViewAccountDetail_Click(object sender, EventArgs e)
		{
			try
			{
				string accountNo = JudgeCheckedOne("结算单详情");
				if (accountNo.Length <= 0) return;
				RunJS(MenuLibrary.AccountDetailViewMenu(accountNo).JsString);
			}
			catch (Exception ex)
			{
				Alert("查看结算单详情失败<br>" + ex.Message);
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
				Alert("查看结算详情失败<br>" + ex.Message);
			}
		}

		private string JudgeCheckedOne(string msg)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
			if (keyValuePairs.Count <= 0)
			{
				Alert(string.Format("没有选择需要查询{0}的结算单",msg));
				return "";
			}
			if (keyValuePairs.Count > 1)
			{
				Alert(string.Format("能且只能同时查询一个{0}",msg));
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
				Alert("查看明细失败<br>" + ex.Message);
			}
		}

		protected void PrintMsg_Click(object sender, EventArgs e)
		{
			try
			{
				DataTable dt = Master.GetPrintData();
				if (dt==null || dt.Rows.Count <= 0)
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
