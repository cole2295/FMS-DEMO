using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using System.IO;
using System.Data;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class MerchantDeliverFeeMain : BasePage
	{
        IMerchantDeliverFee merchantDeliverFee = ServiceLocator.GetService<IMerchantDeliverFee>();
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
            } 
		}

		protected void btnAddFee_Click(object sender, EventArgs e)
		{
			try
			{
				RunJS("fnOpenModalDialog('MerchantDeliverFeeEdit.aspx?opType=0&id=',400,400)");
			}
			catch (Exception ex)
			{
				Alert("打开新建收入配送价格页面失败<br>" + ex.Message);
			}
		}

		protected void btnEditFee_Click(object sender, EventArgs e)
		{
			try
			{
				IList<KeyValuePair<string, string>> checkList;
				if (!JudgeCheckList(0, out checkList))
					return;
                RunJS("fnOpenModalDialog('MerchantDeliverFeeEdit.aspx?opType=0&id=" + checkList[0].Key + "',400,400)");
			}
			catch (Exception ex)
			{
				Alert("打开修改收入配送价格页面失败<br>" + ex.Message);
			}
		}

		protected void btnDelFee_Click(object sender, EventArgs e)
		{
			try
			{
				IList<KeyValuePair<string, string>> checkList;
				if (!JudgeCheckList(2, out checkList))
					return;

                if (merchantDeliverFee.DeleteDeliverFee(checkList, Userid))
				{
                    Master.BindGridView(Master.UCPager.CurrentPageIndex);
					Alert("删除成功");
				}
				else
				{
					Alert("删除失败");
				}
			}
			catch (Exception ex)
			{
				Alert("删除失败<br>" + ex.Message);
			}
		}

		private bool JudgeCheckList(int n,out IList<KeyValuePair<string,string>> checkList)
		{
			checkList = Master.GridViewCheckList;
			if (checkList.Count <= 0)
			{
				Alert("至少选择一项操作");
				return false;
			}

			if (n == 0)
			{
				if (checkList.Count > 1)
				{
					Alert("能且只能同步操作一项");
					return false;
				}
			}
            if (n == 0 || n == 2)
            {
                foreach (KeyValuePair<string, string> k in checkList)
                {
                    if (k.Value == EnumHelper.GetDescription(EnumCODAudit.A1))
                    {
                        Alert("不能操作已审核项");
                        return false;
                    }
                }
            }

            if (n == 1)
            {
                foreach (KeyValuePair<string, string> k in checkList)
                {
                    if (k.Value != EnumHelper.GetDescription(EnumCODAudit.A1))
                    {
                        Alert("只能操作已审核项");
                        return false;
                    }
                }
            }

			return true;
		}

		protected void btDownTemplet_Click(object sender, EventArgs e)
		{
			try
			{
				string filePath = "~/UpFile/收入配送价格导入模板.xls";
				DownLoadTemplet(filePath, "收入配送价格导入模板.xls");
			}
			catch (Exception ex)
			{
				Alert("下载失败");
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

		protected void btImport_Click(object sender, EventArgs e)
		{
			string fielName = FileUpload1.FileName.ToString().Trim();
			string localPath = Server.MapPath("~/Upload/file/");
			if (!this.FileUpload1.HasFile)
			{
				Alert("请选择要导入的文件");
				return;
			}
			if (!Directory.Exists(localPath))
				Directory.CreateDirectory(localPath);
			try
			{
				string path = localPath + fielName;
				this.FileUpload1.SaveAs(path);
				DataSet ds = Excel.ExcelToDataSetFor03And07(path);
				DataTable dtError;
                //
                if (merchantDeliverFee.ImportFee(ds.Tables[0], Userid, out dtError, base.DistributionCode))
				{
					if (dtError != null && dtError.Rows.Count > 0)
					{
						List<string> l = new List<string>();
						foreach (DataColumn column in dtError.Columns)
						{
							l.Add(column.ColumnName);
						}
						ExportExcel(dtError, l.ToArray(), null, "收入配送价格导入错误反馈");
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

        protected void btnAddFeeWait_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<string, string>> checkList;
                if (!JudgeCheckList(1, out checkList))
                    return;
                if (merchantDeliverFee.GetWaitDeliverFeeyFeeId(int.Parse(checkList[0].Key))>0)
                {
                    Alert("已经存在待生效，请执行更新待生效");
                    return;
                }
                else
                {
                    RunJS("fnOpenModalDialog('MerchantDeliverFeeEdit.aspx?opType=1&id=" + checkList[0].Key + "',400,400)");
                }
            }
            catch (Exception ex)
            {
                Alert("打开增加待生效收入配送价格页面失败<br>" + ex.Message);
            }
        }

        protected void btnUpdateFeeWait_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<string, string>> checkList;
                if (!JudgeCheckList(0, out checkList))
                    return;
                RunJS("fnOpenModalDialog('MerchantDeliverFeeEdit.aspx?opType=2&id=" + checkList[0].Key + "',400,400)");
            }
            catch (Exception ex)
            {
                Alert("打开修改待生效收入配送价格页面失败<br>" + ex.Message);
            }
        }
	}
}
