using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.WEBLOGIC;
using System.Text;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.COD
{
	public partial class DeliveryPriceAudit : BasePage
	{
        IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();

		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
            }
            
		}

		protected void Reset_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = JudgeAudit("置回", EnumHelper.GetDescription(EnumCODAudit.A3));
			if (keyValuePairs == null || keyValuePairs.Count <= 0)
				return;

			if (deliveryPriceService.UpdateDeliveryPriceAuditStatus(keyValuePairs, Userid.ToString(), (int)EnumCODAudit.A3))
			{
				Master.BindGridViewList(-1);
				Alert("置回成功");
			}
			else
				Alert("置回失败");
		}

		protected void Audit_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = JudgeAudit("审核", EnumHelper.GetDescription(EnumCODAudit.A1));
			if (keyValuePairs == null || keyValuePairs.Count <= 0)
				return;

			if (deliveryPriceService.UpdateDeliveryPriceAuditStatus(keyValuePairs, Userid.ToString(), (int)EnumCODAudit.A1))
			{
				Master.BindGridViewList(-1);
				Alert("审核成功");
			}
			else
				Alert("审核失败");
		}

		protected void SearchHistory_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
			StringBuilder sb = new StringBuilder();
			if (keyValuePairs.Count > 0)
			{
				foreach (KeyValuePair<string, string> k in keyValuePairs)
				{
					sb.Append(k.Key + ",");
				}
			}
			string url = string.Format("'DeliveryPriceHistory.aspx?lineNo={0}'", sb.ToString().TrimEnd(','));
			RunJS(string.Format("fnOpenModalDialog({0}, 800, 400);", url));
		}

		protected void SearchLog_Click(object sender, EventArgs e)
		{
            IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
            if (keyValuePairs.Count <= 0)
            {
                Alert("请选择需要查看日志的线路");
                return;
            }

            if (keyValuePairs.Count > 1)
            {
                Alert("只能同时查看一条线路的日志");
                return;
            }
            string url = string.Format("DeliveryPriceLog.aspx?lineNo={0}", keyValuePairs[0].Key);
            RunJS(string.Format("fnOpenModalDialog('{0}', 800, 400);", url));
		}

		protected void ResetEffect_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = JudgeAudit("置回", EnumHelper.GetDescription(EnumCODAudit.A3));
			if (keyValuePairs == null || keyValuePairs.Count <= 0)
				return;

			if (deliveryPriceService.UpdateEffectDeliveryPriceAuditStatus(keyValuePairs, Userid.ToString(), (int)EnumCODAudit.A3))
			{
				Master.BindGridViewList(-1);
				Alert("置回成功");
			}
			else
				Alert("置回失败");
		}

		protected void AuditEffect_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = JudgeAudit("审核", EnumHelper.GetDescription(EnumCODAudit.A1));
			if (keyValuePairs == null || keyValuePairs.Count <= 0)
				return;

			if (deliveryPriceService.UpdateEffectDeliveryPriceAuditStatus(keyValuePairs, Userid.ToString(), (int)EnumCODAudit.A1))
			{
				Master.BindGridViewList(-1);
				Alert("审核成功");
			}
			else
				Alert("审核失败");
		}

		private IList<KeyValuePair<string, string>> JudgeAudit(string type,string enumType)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
			if (keyValuePairs.Count <= 0)
			{
				Alert(string.Format("请选择需要{0}的线路", type));
				return null;
			}
			foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
			{
				if (keyValuePair.Value == enumType)
				{
					Alert(string.Format("{0}中不能包含已{0}的线路", type));
					return null;
				}
			}

			return keyValuePairs;
		}

		protected void DownLoad_Click(object sender, EventArgs e)
		{
			try
			{
                if (Master.gvListDataTable == null || Master.gvListDataTable.Rows.Count <= 0)
                {
                    Alert("没有找到可下载数据");
                    return;
                }

				ExportExcel(Master.gvListDataTable, Master.gvListColumnArray, "配送价格列表");
			}
			catch (Exception ex)
			{
				Alert("下载失败");
			}
		}
	}
}
