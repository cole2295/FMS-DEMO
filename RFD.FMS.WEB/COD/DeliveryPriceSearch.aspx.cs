using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;

namespace RFD.FMS.WEB.COD
{
	public partial class DeliveryPriceSearch : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {   
            }
            
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
