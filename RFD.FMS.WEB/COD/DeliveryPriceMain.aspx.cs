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
using System.Text;
using System.IO;
using System.Data;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.COD
{
	public partial class DeliveryPriceMain : BasePage
	{
        IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();

		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {           
            }
           
		}

		protected void AddDeliveryPrice_Click(object sender, EventArgs e)
		{
			RunJS("fnOpenModalDialog('DeliveryPriceEdit.aspx?lineNo=&isEffect=0', 400, 400);");
		}

		protected void UpDateDeliveryPrice_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;

			if (keyValuePairs.Count <= 0)
			{
				Alert("请选择需要更新的线路");
				return;
			}

			if (keyValuePairs.Count >1)
			{
				Alert("只能同时更新一条线路");
				return;
			}

			if (keyValuePairs[0].Value == EnumHelper.GetDescription(EnumCODAudit.A1))
			{
				Alert("不能更新已审核的线路");
				return;
			}

			string url = string.Format("DeliveryPriceEdit.aspx?lineNo={0}&isEffect=0", keyValuePairs[0].Key);
			RunJS(string.Format("fnOpenModalDialog('{0}', 400, 400);",url));
		}

		protected void Delete_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
			if (keyValuePairs.Count <= 0)
			{
				Alert("没有选择需要删除的线路");
				return;
			}
			foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
			{
				if (keyValuePair.Value == EnumHelper.GetDescription(EnumCODAudit.A1))
				{
					Alert("删除中不能包含已审核的线路");
					return;
				}
			}

			if (deliveryPriceService.DeleteDeliveryPrice(keyValuePairs, Userid.ToString()))
			{
				Master.BindGridViewList(-1);
				Alert("删除成功");
			}
			else
				Alert("删除失败");
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

		protected void AddEffectDeliveryPrice_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
			if (keyValuePairs.Count <= 0)
			{
				Alert("请选择需要添加待生效的线路");
				return;
			}

			if (keyValuePairs.Count > 1)
			{
				Alert("只能同时添加一条待生效线路");
				return;
			}
			string url = string.Format("DeliveryPriceEdit.aspx?lineNo={0}&isEffect=1", keyValuePairs[0].Key);
			RunJS(string.Format("fnOpenModalDialog('{0}', 400, 400);", url));
		}

		protected void UpdateEffectDeliveryPrice_Click(object sender, EventArgs e)
		{
			IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListChecked;
			if (keyValuePairs.Count <= 0)
			{
				Alert("请选择需要更新待生效的线路");
				return;
			}

			if (keyValuePairs.Count > 1)
			{
				Alert("只能同时更新一条待生效线路");
				return;
			}

			if (keyValuePairs[0].Value == EnumHelper.GetDescription(EnumCODAudit.A1))
			{
				Alert("不能更新已审核的线路");
				return;
			}
			string url = string.Format("DeliveryPriceEdit.aspx?lineNo={0}&isEffect=2", keyValuePairs[0].Key);
			RunJS(string.Format("fnOpenModalDialog('{0}', 400, 400);", url));
		}

		protected void DownLoad_Click(object sender, EventArgs e)
		{
			try
			{
                if (Master.gvListDataTable == null || Master.gvListDataTable.Rows.Count<=0)
				{
					Alert("没有找到可下载数据");
					return;
				}

				ExportExcel(Master.gvListDataTable, Master.gvListColumnArray, "配送价格列表");
			}
			catch(Exception ex)
			{
				Alert("下载失败");
			}
		}

		protected void ImportExcel_Click(object sender, EventArgs e)
		{
			try
			{
				string filePath = "~/UpFile/配送价格导入模板.xls";
				DownLoadTemplet(filePath, "配送价格导入模板.xls");
			}
			catch (Exception ex)
			{
				Alert("下载失败");
			}
		}

		protected void Import_Click(object sender, EventArgs e)
		{
			string fielName = fuExprot.FileName.ToString().Trim();
			string localPath = Server.MapPath("~/Upload/file/");
			if (!Directory.Exists(localPath))
				Directory.CreateDirectory(localPath);
			if (!this.fuExprot.HasFile)
			{
				Alert("请选择要导入的文件");
				return;
			}
			try
			{
				string path = localPath + fielName;
				this.fuExprot.SaveAs(path);
				DataSet ds = Excel.ExcelToDataSetFor03And07(path);
				DataTable dtError;
				if (deliveryPriceService.ExportDeliveryPrice(ds.Tables[0], Userid, out dtError,base.DistributionCode))
				{
					if (dtError != null && dtError.Rows.Count > 0)
					{
						List<string> l = new List<string>();
						foreach (DataColumn column in dtError.Columns)
						{
							l.Add(column.ColumnName);
						}
						ExportExcel(dtError, l.ToArray(),null, "配送价格导入错误反馈");
					}
					else
					{
						Alert("导入成功");
					}
				}
				else
				{
					Alert("导入失败");
				}
			}
			catch (Exception ex)
			{
				Alert("导入失败<br>" + ex.Message);
			}
			finally
			{
				if (File.Exists(fielName))
				{
					File.Delete(fielName);
				}
			}
		}

		private void DownLoadTemplet(string filePath, string clientFileName)
		{
			System.Web.HttpContext.Current.Response.Clear();
			System.Web.HttpContext.Current.Response.Buffer = true;
			System.Web.HttpContext.Current.Response.Charset = "gb2312";
			System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
			System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(clientFileName, System.Text.Encoding.UTF8));
			System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
			System.Web.HttpContext.Current.Response.BinaryWrite(File.ReadAllBytes(Server.MapPath(filePath)));
		}

        protected void BatchAddEffectDeliveryPrice_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListCheckedByAreaType;
            if (keyValuePairs.Count <= 0)
            {
                Alert("请选择需要批量添加待生效的线路");
                return;
            }
            string lineNos = string.Empty;
            if (!JudgeAreaType(keyValuePairs, ref lineNos))
            {
                Alert("所选批量添加待生效的线路中存在区域类型不一致，请重新选择");
                return;
            }
            string url = string.Format("DeliveryPriceEdit.aspx?isEffect=3&lineNo={0}", lineNos);
            RunJS(string.Format("fnOpenModalDialog('{0}', 400, 400);", url));
        }

        protected void BatchUpdateEffectDeliveryPrice_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, string>> keyValuePairs = Master.gvListCheckedByAreaType;
            if (keyValuePairs.Count <= 0)
            {
                Alert("请选择需要批量修改待生效的线路");
                return;
            }
            string lineNos = string.Empty;
            if (!JudgeAreaType(keyValuePairs, ref lineNos))
            {
                Alert("所选批量添加待生效的线路中存在区域类型不一致，请重新选择");
                return;
            }
            string url = string.Format("DeliveryPriceEdit.aspx?isEffect=4&lineNo={0}", lineNos);
            RunJS(string.Format("fnOpenModalDialog('{0}', 400, 400);", url));
        }

        private bool JudgeAreaType(IList<KeyValuePair<string, string>> keyValuePairs,ref string lineNos)
        {
            if (keyValuePairs == null || keyValuePairs.Count <= 0)
                return false;

            string oldAreaType = keyValuePairs[0].Value;
            string newAreaType = string.Empty;
            StringBuilder sbStr =new StringBuilder();
            foreach (KeyValuePair<string, string> k in keyValuePairs)
            {
                sbStr.Append(k.Key + ",");//只有true的时候才全部累加上,才可以执行批量
                newAreaType = k.Value;
                if (oldAreaType != newAreaType)
                    return false;
            }

            lineNos = sbStr.ToString().TrimEnd(',');

            return true;
        }
	}
}
