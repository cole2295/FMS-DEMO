using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.Mail;
using RFD.FMS.ServiceImpl.Mail;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Service.AudiMgmt;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.COD;
using RFD.FMS.MODEL;
using System.Collections;

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class ChangeCODDeliverFee : FMSBasePage
	{
        private static DataTable ImportTable = new DataTable();
        private static DataTable CurrTable = new DataTable();
        private static DataTable FailedTable = new DataTable();
        public  int Mode
        {
            get { return DataConvert.ToInt(ViewState["mode"]); }
            set { ViewState["mode"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
		{
            if(!IsPostBack)
            {
                txtFormula.Attributes["onclick"] = "fnPriceClick('../COD/DeliveryPriceChoose.aspx','" + txtFormula.ClientID + "')";
                
            }

		}

        public DeliverFeeModel ResultModel
        {
            get { return ViewState["ResultModel"] as DeliverFeeModel; }
            set { ViewState["ResultModel"] = value; }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtWaybillNo.Text.Trim().Length == 0)
            {
                Alert("请输入运单号!");

                return;
            }

            var service = ServiceLocator.GetService<ICODBaseInfoService>();

            DeliverFeeModel model = service.GetDeliverFeeParameter(DataConvert.ToLong(txtWaybillNo.Text.Trim()),DistributionCode);

            if (model == null)
            {
                Alert("没有查询到运单费用信息!");

                return;
            }

            ResultModel = model;

            txtArea.Text = model.Area.ToString();
            txtFormula.Text = model.Formula;
            txtWeight.Text = model.Weight.ToString();

            if (model.IsFare == (int)BizEnums.EnumIsFareType.Eval)
            {
                lblFee.Text = "配送费：" + model.DeliverFee.ToString();
            }
            else
            {
                lblFee.Text = EnumHelper.GetDescription((BizEnums.EnumIsFareType)model.IsFare);
            }
        }

        protected void btnEvalFee_Click(object sender, EventArgs e)
        {
            if (txtWaybillNo.Text.Trim().Length == 0)
            {
                Alert("请输入运单号!");

                return;
            }

            if (ResultModel == null)
            {
                Alert("请先查询运单配送费信息!");

                return;
            }

            DeliverFeeModel model = ResultModel;

            model.Area = DataConvert.ToInt(txtArea.Text.Trim());
            model.Weight = DataConvert.ToDecimal(txtWeight.Text.Trim());
            model.Formula = Request.Form[txtFormula.ClientID.Replace("_", "$")].Trim();

            txtFormula.Text = model.Formula.ToString();

            var service = ServiceLocator.GetService<ICODBaseInfoService>();

            if (service.EvalDeliverFee(model) == false)
            {
                lblFee.Text = "配送费计算错误!";
            }
            else
            {
                lblFee.Text = "配送费：" + model.DeliverFee.ToString();

                ResultModel = model;
            }
        }

        protected void btnCommitChange_Click(object sender, EventArgs e)
        {
            if (txtWaybillNo.Text.Trim().Length == 0)
            {
                Alert("请输入运单号!");

                return;
            }

            if (ResultModel == null)
            {
                Alert("请先查询运单配送费信息!");

                return;
            }

            DeliverFeeModel model = ResultModel;

            var service = ServiceLocator.GetService<ICODBaseInfoService>();

            if (service.SaveDeliverFee(model) == true)
            {
                Alert("修改成功!");
            }
            else
            {
                Alert("修改失败!");
            }
        }

        protected void btnReEval_Click(object sender, EventArgs e)
        {
            if (txtWaybillNo.Text.Trim().Length == 0)
            {
                Alert("请输入运单号!");

                return;
            }

            var service = ServiceLocator.GetService<ICODBaseInfoService>();

            if (service.UpdateEvalStatus(txtWaybillNo.Text.Trim()) == true)
            {
                Alert("配送费正在重新计算,请等待一分钟后查看，谢谢!");
            }
            else
            {
                Alert("重新计算失败!");
            }
        }

        protected void btnBatchImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUpload.HasFile)
                {
                    string path = "/file/";
                    string temp = DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.Hour + DateTime.Now.Minute +
                                  DateTime.Now.Second + DateTime.Now.Millisecond;
                    string filename = path + temp + "CodDeliveryFeeChange.xlsx";
                    fileUpload.PostedFile.SaveAs(Server.MapPath(filename));
                    DataSet dSet = GetExcelData(Server.MapPath(filename));
                    ImportTable.Clear();
                    CurrTable.Clear();
                    ImportTable = dSet.Tables[0];
                    var checks = ImportTable.AsEnumerable().Where(r => !string.IsNullOrWhiteSpace(r[0].ToString())).ToList();
                    if (checks.Count > 1000)
                    {
                        Alert("已经超出导入上限1000，不允许导入");
                        return;
                    }
                    string errorTips = "";
                    if (ImportTable.Columns[0].ColumnName != "唯一编号")
                    {
                        errorTips += " 唯一编号";
                    }
                    if (ImportTable.Columns[1].ColumnName != "分配区域")
                    {
                         errorTips += " 分配区域";
                    }
                    if (ImportTable.Columns[2].ColumnName != "订单号")
                     {
                             errorTips += " 订单号";
                      }
                    if (ImportTable.Columns[3].ColumnName != "结算重量")
                    {
                             errorTips += " 结算重量";
                    }
                     if (ImportTable.Columns[4].ColumnName != "配送费公式")
                      {
                             errorTips += " 配送费公式";
                      }

                    if(!string.IsNullOrEmpty(errorTips))   
                    {
                        Alert("导入失败！请检查导入excel列标题 "+errorTips +"是否正确");
                        ImportTable.Clear();
                        return;
                    }
                    else
                    {
                        ImportTable.Columns[0].ColumnName = "InfoID";
                        ImportTable.Columns[1].ColumnName = "AreaType";
                        ImportTable.Columns[2].ColumnName = "WaybillNo";
                        ImportTable.Columns[3].ColumnName = "AccountWeight";
                        ImportTable.Columns[4].ColumnName = "FareFormula";
                        ImportTable.Columns.Add("CompanyName",typeof(string));
                        ImportTable.Columns.Add("Fare",typeof(decimal));
                        ImportTable.Columns.Add("Reason",typeof(string));

                        CheckImportTable(ref ImportTable);


                    }
                    gvList.DataSource = ImportTable;
                    gvList.DataBind();
                    TipsLab.Text = "导入成功"+ImportTable.Rows.Count+"单，失败"+FailedTable.Rows.Count+"单";
                    ExportFailedBtn.Text = "导出导入失败单据";
                    Alert("导入完成！");
                }
                else
                {
                    Alert("没有数据！");
                }

            }
            catch (Exception ex)
            {
                Alert("错误信息：" + ex.Message.Replace("\n", "<br>"));
                IMail mail = new Mail();
                mail.SendMailToUser("配送费修改导入模版错误", "ErrorMessage:" + ex.Message + "ErrorStack:" + ex.StackTrace,
                                    "xueyi@vancl.cn");
            }

        }

        private void CheckImportTable( ref DataTable dt)
        {
            var service =ServiceLocator.GetService<ICODBaseInfoService>();
            FailedTable.Clear();
            FailedTable = dt.Clone();
            int flag = 0;
            for (int i = dt.Rows.Count-1;i>=0;i--)
            {
                
                if(dt.Rows[i]["InfoID"] == DBNull.Value 
                    && dt.Rows[i]["WaybillNo"] ==DBNull.Value
                    && dt.Rows[i]["AreaType"] == DBNull.Value
                    && dt.Rows[i]["AccountWeight"] == DBNull.Value
                    && dt.Rows[i]["FareFormula"] ==DBNull.Value
                    )
                {
                    dt.Rows.Remove(dt.Rows[i]);
                    continue;
                }
                
                if (!service.ExsitCodBaseInfoByNo(DataConvert.ToLong(dt.Rows[i]["WaybillNo"]),DataConvert.ToLong(dt.Rows[i]["InfoID"])))
                {
                    flag = 1;
                    dt.Rows[i]["Reason"] = dt.Rows[i]["Reason"].ToString() + "运单号不存在或唯一编号不正确";
                }

                if (!string.IsNullOrEmpty(dt.Rows[i]["AreaType"].ToString()) && !Regex.Match(dt.Rows[i]["AreaType"].ToString(), @"^\d+").Success)
                {
                    flag = 1;
                    dt.Rows[i]["Reason"] = dt.Rows[i]["Reason"].ToString() + "分配区域不正确";
                }

                if (!string.IsNullOrEmpty(dt.Rows[i]["AccountWeight"].ToString()) && !Regex.Match(dt.Rows[i]["AccountWeight"].ToString(), @"(^\d{1,3}$)|(^\d{1,3}\.\d{1,3}$)").Success)
                {
                    flag = 1;
                    dt.Rows[i]["Reason"] = dt.Rows[i]["Reason"].ToString() + "结算重量不正确，不超过3位数，小数点不超过3位";
                }

                if (flag == 1)
                {
                    FailedTable.Rows.Add(dt.Rows[i].ItemArray);
                    dt.Rows.Remove(dt.Rows[i]);

                }

            }
        }


        private DataSet GetExcelData(string filename)
        {
            DataSet dSet = new DataSet();
            // string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filename + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";

            string conStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0 Xml;HDR=YES'";

            OleDbConnection conn = new OleDbConnection(conStr);
            string searchsql = "select * from [Sheet1$]";
            OleDbDataAdapter oda = new OleDbDataAdapter(searchsql, conn);
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


        protected void SysCalcuBtn_Click1(object sender, EventArgs e)
        {
            var service = ServiceLocator.GetService<ICODBaseInfoService>();
            
            var dt = service.GetCODDeliveryFeeInfo(ImportTable,0);
            FailedTable.Clear();
            FailedTable = dt.Clone();
            for (int i = dt.Rows.Count - 1; i >=0; i-- )
            {
                if (dt.Rows[i]["IsSucc"].ToString() != "1")
                {
                    FailedTable.Rows.Add(dt.Rows[i].ItemArray);
                    dt.Rows.Remove(dt.Rows[i]);
                }
            }
            TipsLab.Text = "计算成功" + dt.Rows.Count + "单，计算失败" + FailedTable.Rows.Count + "单";
            gvList.DataSource = dt;
            CurrTable = dt;
            gvList.DataBind();
            ExportFailedBtn.Text = "导出计算失败单据";
            Alert("计算完成！");
        }

        protected void BatchSaveBtn_Click(object sender, EventArgs e)
        {
            var service = ServiceLocator.GetService<ICODBaseInfoService>();
            
            if(CurrTable == null || CurrTable.Rows.Count == 0)
            {
                Alert("没有要修改的数据");
                return;
            }

           if( service.UpdateBatchDeliverFee(CurrTable))
           {
               Alert("修改成功！");
               CurrTable.Clear();
           }
           else
           {
               Alert("修改失败！");
           }
 

        }

        protected void ImportCalcuBtn_Click(object sender, EventArgs e)
        {
            var service = ServiceLocator.GetService<ICODBaseInfoService>();

            var dt = service.GetCODDeliveryFeeInfo(ImportTable,1);
            FailedTable.Clear();
            FailedTable = dt.Clone();
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["IsSucc"].ToString() != "1")
                {
                    if (dt.Rows[i]["IsSucc"].ToString() =="0")
                    {
                        dt.Rows[i]["IsSucc"] = "计算失败";
                    }
                    FailedTable.Rows.Add(dt.Rows[i].ItemArray);
                    dt.Rows.Remove(dt.Rows[i]);
                }
            }
            TipsLab.Text = "计算成功" + dt.Rows.Count + "单，计算失败" + FailedTable.Rows.Count + "单";
            gvList.DataSource = dt;
            CurrTable = dt;
            gvList.DataBind();
            ExportFailedBtn.Text = "导出计算失败单据";
            Alert("计算完成！");
        }

        protected void ExportFailedBtn_Click(object sender, EventArgs e)
        {
            if(FailedTable.Rows.Count ==0)
            {
                Alert("没有失败数据导出！");
                return;
            }
            else
            {
                if (FailedTable.Columns.Contains("InfoID"))
                {
                    FailedTable.Columns["InfoID"].ColumnName = "唯一编号";
                    FailedTable.Columns["AreaType"].ColumnName = "分配区域";
                    FailedTable.Columns["WaybillNo"].ColumnName = "订单号";
                    FailedTable.Columns["AccountWeight"].ColumnName = "结算重量";
                    FailedTable.Columns["FareFormula"].ColumnName = "配送公式";
                    FailedTable.Columns["CompanyName"].ColumnName = "配送公司";
                    FailedTable.Columns["Fare"].ColumnName = "配送费";
                    if (FailedTable.Columns.Contains("IsSucc"))
                    {
                        FailedTable.Columns["IsSucc"].ColumnName = "计算失败原因";
                        CSVExport.DataTable2Excel(FailedTable, "COD配送费修改计算失败数据");
                    }
                    else
                    {
                        FailedTable.Columns["Reason"].ColumnName = "导入失败原因";
                        CSVExport.DataTable2Excel(FailedTable, "COD配送费修改导入失败数据");
                    }
                }
                else
                {
                    if (FailedTable.Columns.Contains("计算失败原因")  )
                    {
                        
                        CSVExport.DataTable2Excel(FailedTable, "COD配送费修改计算失败数据");
                    }
                    else if (FailedTable.Columns.Contains("导入失败原因"))
                    {
                       
                        CSVExport.DataTable2Excel(FailedTable, "COD配送费修改导入失败数据");
                    }
                }
                
            }

        }

      }

       
}
