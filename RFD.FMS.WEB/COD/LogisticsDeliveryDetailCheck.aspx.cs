using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Model;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.COD;
using RFD.FMS.Service.Mail;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC.Mail;
using Mail = RFD.FMS.ServiceImpl.Mail.Mail;

namespace RFD.FMS.WEB.COD
{
    public partial class LogisticsDeliveryDetailCheck :BasePage
    {
        private readonly IExpressCompanyService expressSer = ServiceLocator.GetService<IExpressCompanyService>();
        private readonly ICODBaseInfoService codBaseInfoSer = ServiceLocator.GetService<ICODBaseInfoService>();
        private FMS_CODBaseInfoCheck model = new FMS_CODBaseInfoCheck();
        private static DataTable  ImportTable = new DataTable();
        private static DataTable  ResultCheckdt = new DataTable();
        private static DataTable  CheckSuccdt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                ImportTable.Clear();
                //BuildCompanyDDL();
                txtBeginTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                MerchantSrc.DistributionCode = base.DistributionCode;
            }
        }

        //private void BuildCompanyDDL()
        //{
       
        //   CompanyDDL.DataSource = expressSer.GetThirdCompanyList(base.DistributionCode);
        //   CompanyDDL.DataValueField = "ExpressCompanyID";
        //   CompanyDDL.DataTextField = "CompanyName";
        //   CompanyDDL.DataBind();
        //   ListItem li = new ListItem("全部", "-1");
        //   CompanyDDL.Items.Insert(0, li);
        //}

        protected void btnBatchQuery_Click(object sender, EventArgs e)
        {
           try
           {
               if (fileUpload.HasFile)
               {
                   string path = "/file/";
                   string temp = DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.Hour + DateTime.Now.Minute +
                                 DateTime.Now.Second + DateTime.Now.Millisecond;
                   string filename = path +temp+ "LogisticsDeliverycheck.xlsx";
                   fileUpload.PostedFile.SaveAs(Server.MapPath(filename));
                   DataSet dSet = GetExcelData(Server.MapPath(filename));
                   ImportTable.Clear();
                   ImportTable = dSet.Tables[0];
                   if(ImportTable.Columns[0].ColumnName !="订单号" 
                       || ImportTable.Columns[1].ColumnName != "区域类型"
                       || ImportTable.Columns[2].ColumnName != "配送费"
                       || ImportTable.Columns[3].ColumnName != "状态")
                   {
                       Alert("导入失败！请检查导入excel列标题是否依次是 订单号，区域类型，配送费，状态");
                       ImportTable.Clear();
                       return;
                   }
                   Alert("导入完成！");
               }
               else
               {
                   Alert("没有数据！");
               }
           }
           catch(Exception ex)
           {
               Alert("错误信息："+ex.Message.Replace("\n","<br>"));
               IMail mail = new Mail();
               mail.SendMailToUser("电子核对导入模版错误", "ErrorMessage:" + ex.Message + "ErrorStack:" + ex.StackTrace,
                                   "xueyi@vancl.cn");
           }
            
            
        }

        private  void InitModel()
        {
            if(model == null)
            model = new FMS_CODBaseInfoCheck();
            model.DistributionCode = base.DistributionCode;
            model.DistributionCompany = DataConvert.ToInt(DistributionCompany.SelectExpressID);
            model.STime = Convert.ToDateTime(txtBeginTime.Text.Trim());
            model.ETime = Convert.ToDateTime(txtEndTime.Text.Trim()).AddDays(1);
            if (MerchantSrc.IsSelectAll || string.IsNullOrEmpty(MerchantSrc.SelectMerchantSourcesID))
                model.MerchantIDs = "";
            else
                model.MerchantIDs = MerchantSrc.SelectMerchantSourcesID;      
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

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            if(ImportTable.Rows.Count ==0)
            {
                Alert("请先导入数据！");
                return;
            }
            if (string.IsNullOrEmpty(DistributionCompany.SelectExpressID))
            {
                Alert("请选择一家配送公司！");
                return;
            }
            InitModel();
            int CheckedSucc = 0;
            ICODBaseInfoService CodSer = ServiceLocator.GetService<ICODBaseInfoService>();
            var deliveryDic = CodSer.GetDeliveryList(model);
            var returnsDic = CodSer.GetReturnList(model);
            var visitDic = CodSer.GetVisitList(model);

            ResultCheckdt.Clear();
            CheckSuccdt.Clear();
            ResultCheckdt = CODCheckForDistribution(ImportTable, ref deliveryDic, ref returnsDic, ref visitDic,ref CheckedSucc,ref CheckSuccdt);
            
            
            foreach (var dic in deliveryDic)
            {
                if (!dic.Value.IsChecked)
                {
                    DataRow dr = ResultCheckdt.NewRow();
                    dr["订单号"] = dic.Value.WaybillNo; 
                    dr["商家"] = dic.Value.MerchantName;
                    dr["结算单位"] = dic.Value.AccountCompany;
                    dr["区域类型"] = dic.Value.AreaType;
                    dr["结算重量"] = dic.Value.AccountWeight;
                    dr["配送费"] = dic.Value.fee;
                    dr["收货人地址"] = dic.Value.Address;
                    dr["最终发日期"] = dic.Value.DeliveryTime;
                    dr["末级发货仓名称"] = dic.Value.FinalSorting;
                    dr["状态"] = dic.Value.Status;
                    dr["核对失败原因"] = "系统提供的订单配送商不存在";
                    ResultCheckdt.Rows.Add(dr);
                   
                }
            }

            foreach (var dic in returnsDic)
            {
                if (!dic.Value.IsChecked)
                {
                    DataRow dr = ResultCheckdt.NewRow();
                    dr["订单号"] = dic.Value.WaybillNo;
                    dr["商家"] = dic.Value.MerchantName;
                    dr["结算单位"] = dic.Value.AccountCompany;
                    dr["区域类型"] = dic.Value.AreaType;
                    dr["结算重量"] = dic.Value.AccountWeight;
                    dr["配送费"] = dic.Value.fee;
                    dr["收货人地址"] = dic.Value.Address;
                    dr["最终发日期"] = dic.Value.DeliveryTime;
                    dr["末级发货仓名称"] = dic.Value.FinalSorting;
                    dr["状态"] = dic.Value.Status;
                    dr["核对失败原因"] = "系统提供的订单配送商不存在";
                    ResultCheckdt.Rows.Add(dr);
                    
                }
            }

            foreach (var dic in visitDic)
            {
                if (!dic.Value.IsChecked)
                {
                    DataRow dr = ResultCheckdt.NewRow();
                    dr["订单号"] = dic.Value.WaybillNo;
                    dr["商家"] = dic.Value.MerchantName;
                    dr["结算单位"] = dic.Value.AccountCompany;
                    dr["区域类型"] = dic.Value.AreaType;
                    dr["结算重量"] = dic.Value.AccountWeight;
                    dr["配送费"] = dic.Value.fee;
                    dr["收货人地址"] = dic.Value.Address;
                    dr["最终发日期"] = dic.Value.DeliveryTime;
                    dr["末级发货仓名称"] = dic.Value.FinalSorting;
                    dr["状态"] = dic.Value.Status;
                    dr["核对失败原因"] = "系统提供的订单配送商不存在";
                    ResultCheckdt.Rows.Add(dr);
                    
                }
            }

             
            string SysTotal = (deliveryDic.Count + returnsDic.Count + visitDic.Count).ToString();
            string MerchantTotal = ImportTable.Rows.Count.ToString();
            SuccLabelTip.Text = "配送商提供数据" + MerchantTotal + "单,系统查询数据"+SysTotal+"单," +
                                "其中核对成功"+CheckedSucc+"单";
            FailLabelTips.Text = "配送商提供数据" + MerchantTotal + "单, 系统查询数据" + SysTotal + "单," +
                                 "其中核对失败" + ResultCheckdt.Rows.Count +"单";
            ImportTable.Clear();

        }

          public  DataTable CODCheckForDistribution(DataTable ImportDt,ref IDictionary<long, FMS_CODBaseInfoCheck> Deliverydic,
                 ref IDictionary<long, FMS_CODBaseInfoCheck> Returnsdic,ref IDictionary<long, FMS_CODBaseInfoCheck> Visitdic, ref int CheckedSucc,ref DataTable CheckSuccdt)
        {
            DataTable  Checkdt = new DataTable();
            Checkdt.Columns.Add("商家", typeof(string));
            Checkdt.Columns.Add("结算单位", typeof(string));
            Checkdt.Columns.Add("区域类型", typeof(int));
            Checkdt.Columns.Add("订单号", typeof(long));
            Checkdt.Columns.Add("结算重量", typeof(decimal));
            Checkdt.Columns.Add("配送费", typeof (decimal));
            Checkdt.Columns.Add("收货人地址", typeof(string));
            Checkdt.Columns.Add("最终发日期", typeof(DateTime));
            Checkdt.Columns.Add("末级发货仓名称", typeof(string));
            Checkdt.Columns.Add("状态", typeof(int));
            Checkdt.Columns.Add("核对失败原因", typeof(string));
            CheckSuccdt = Checkdt.Clone();
            CheckSuccdt.Columns.Remove("核对失败原因");
            CheckedSucc = 0;
             for (int i=0; i<ImportDt.Rows.Count;i++)
             {
                 long waybillNo = DataConvert.ToLong(ImportDt.Rows[i]["订单号"]);
                 string reason = "";
                 DataRow dr = Checkdt.NewRow();
                 DataRow sdr = CheckSuccdt.NewRow();
                 if (Deliverydic.ContainsKey(waybillNo))
                 {

                     reason = "";
                     if (Deliverydic[waybillNo].AreaType != DataConvert.ToInt(ImportDt.Rows[i]["区域类型"]))
                    {
                        reason += "区域类型不匹配：系统:" + Deliverydic[waybillNo].AreaType + ",配送商：" +
                                  ImportDt.Rows[i]["区域类型"].ToString();
                    }
                    if (Deliverydic[waybillNo].fee != DataConvert.ToDecimal(ImportDt.Rows[i]["配送费"]))
                    {
                        reason += "配送费核对不上：系统：" + Deliverydic[waybillNo].fee + ",配送商：" + ImportDt.Rows[i]["配送费"].ToString();
                    }
                    if (Deliverydic[waybillNo].Status != DataConvert.ToInt(ImportDt.Rows[i]["状态"]))
                    {
                        reason +="订单状态匹配不上： 系统："+Deliverydic[waybillNo].Status +",配送商 ："+
                            ImportDt.Rows[i]["状态"].ToString();
                    }
                    if(!string.IsNullOrEmpty(reason))
                    {
                       
                        dr["订单号"] = waybillNo;
                        dr["商家"] = Deliverydic[waybillNo].MerchantName;
                        dr["结算单位"] = Deliverydic[waybillNo].AccountCompany;
                        dr["区域类型"] = Deliverydic[waybillNo].AreaType;
                        dr["结算重量"] = Deliverydic[waybillNo].AccountWeight;
                        dr["配送费"] = Deliverydic[waybillNo].fee;
                        dr["收货人地址"] = Deliverydic[waybillNo].Address;
                        dr["最终发日期"] = Deliverydic[waybillNo].DeliveryTime;
                        dr["末级发货仓名称"] = Deliverydic[waybillNo].FinalSorting;
                        dr["状态"] = Deliverydic[waybillNo].Status;
                        dr["核对失败原因"] = reason;
                        Deliverydic[waybillNo].IsChecked = true;
                      
                    }
                    else if (string.IsNullOrEmpty(reason))
                    {
                        sdr["订单号"] = waybillNo;
                        sdr["商家"] = Deliverydic[waybillNo].MerchantName;
                        sdr["结算单位"] = Deliverydic[waybillNo].AccountCompany;
                        sdr["区域类型"] = Deliverydic[waybillNo].AreaType;
                        sdr["结算重量"] = Deliverydic[waybillNo].AccountWeight;
                        sdr["配送费"] = Deliverydic[waybillNo].fee;
                        sdr["收货人地址"] = Deliverydic[waybillNo].Address;
                        sdr["最终发日期"] = Deliverydic[waybillNo].DeliveryTime;
                        sdr["末级发货仓名称"] = Deliverydic[waybillNo].FinalSorting;
                        sdr["状态"] = Deliverydic[waybillNo].Status;
                        CheckSuccdt.Rows.Add(sdr);
                        CheckedSucc++;
                        Deliverydic[waybillNo].IsChecked = true;
                        continue;
                    }
                    
                }
                if(Returnsdic.ContainsKey(waybillNo))
                {
                    reason = "";
                    if (Returnsdic[waybillNo].AreaType != DataConvert.ToInt(ImportDt.Rows[i]["区域类型"]))
                    {
                        reason += "区域类型不匹配：系统:" + Returnsdic[waybillNo].AreaType + ",配送商：" +
                                  ImportDt.Rows[i]["区域类型"].ToString();
                    }
                    if (Returnsdic[waybillNo].fee != DataConvert.ToDecimal(ImportDt.Rows[i]["配送费"]))
                    {
                        reason += "配送费核对不上：系统：" + Returnsdic[waybillNo].fee + ",配送商：" + ImportDt.Rows[i]["配送费"].ToString();
                    }
                    if (Returnsdic[waybillNo].Status != DataConvert.ToInt(ImportDt.Rows[i]["状态"]))
                    {
                        reason += "订单状态匹配不上： 系统：" + Returnsdic[waybillNo].Status + ",配送商 ：" +
                        ImportDt.Rows[i]["状态"].ToString();
                    }
                    if (!string.IsNullOrEmpty(reason))
                    {
                        
                        dr["订单号"] = waybillNo;
                        dr["商家"] = Returnsdic[waybillNo].MerchantName;
                        dr["结算单位"] = Returnsdic[waybillNo].AccountCompany;
                        dr["区域类型"] = Returnsdic[waybillNo].AreaType;
                        dr["结算重量"] = Returnsdic[waybillNo].AccountWeight;
                        dr["收货人地址"] = Returnsdic[waybillNo].Address;
                        dr["最终发日期"] = Returnsdic[waybillNo].DeliveryTime;
                        dr["末级发货仓名称"] = Returnsdic[waybillNo].FinalSorting;
                        dr["状态"] = Returnsdic[waybillNo].Status;
                        dr["核对失败原因"] = reason;
                        Returnsdic[waybillNo].IsChecked = true;
                        
                    }
                    else if (string.IsNullOrEmpty(reason))
                    {
                        sdr["订单号"] = waybillNo;
                        sdr["商家"] = Returnsdic[waybillNo].MerchantName;
                        sdr["结算单位"] = Returnsdic[waybillNo].AccountCompany;
                        sdr["区域类型"] = Returnsdic[waybillNo].AreaType;
                        sdr["结算重量"] = Returnsdic[waybillNo].AccountWeight;
                        sdr["收货人地址"] = Returnsdic[waybillNo].Address;
                        sdr["最终发日期"] = Returnsdic[waybillNo].DeliveryTime;
                        sdr["末级发货仓名称"] = Returnsdic[waybillNo].FinalSorting;
                        sdr["状态"] = Returnsdic[waybillNo].Status;
                        CheckSuccdt.Rows.Add(sdr);
                        CheckedSucc++;
                        Returnsdic[waybillNo].IsChecked = true;
                        continue;
                    }
                   
                }
                if(Visitdic.ContainsKey(waybillNo))
                {
                    reason = "";
                    if (Visitdic[waybillNo].AreaType != DataConvert.ToInt(ImportDt.Rows[i]["区域类型"]))
                    {
                        reason += "区域类型不匹配：系统:" + Visitdic[waybillNo].AreaType + ",配送商：" +
                                  ImportDt.Rows[i]["区域类型"].ToString();
                    }
                    if (Visitdic[waybillNo].fee != DataConvert.ToDecimal(ImportDt.Rows[i]["配送费"]) && Visitdic[waybillNo].WaybillType == 1)
                    {
                        reason += "配送费核对不上：系统：" + Visitdic[waybillNo].fee + ",配送商：" + ImportDt.Rows[i]["配送费"].ToString();
                    }
                    if (Visitdic[waybillNo].Status != DataConvert.ToInt(ImportDt.Rows[i]["状态"]))
                    {
                        reason += "订单状态匹配不上： 系统：" + Visitdic[waybillNo].Status + ",配送商 ：" +
                            ImportDt.Rows[i]["状态"].ToString();
                    }
                    if (!string.IsNullOrEmpty(reason))
                    {
                        
                        dr["订单号"] = waybillNo;
                        dr["商家"] = Visitdic[waybillNo].MerchantName;
                        dr["结算单位"] = Visitdic[waybillNo].AccountCompany;
                        dr["区域类型"] = Visitdic[waybillNo].AreaType;
                        dr["结算重量"] = Visitdic[waybillNo].AccountWeight;
                        dr["收货人地址"] = Visitdic[waybillNo].Address;
                        dr["最终发日期"] = Visitdic[waybillNo].DeliveryTime;
                        dr["末级发货仓名称"] = Visitdic[waybillNo].FinalSorting;
                        dr["状态"] = Visitdic[waybillNo].Status;
                        dr["核对失败原因"] = reason;
                        Visitdic[waybillNo].IsChecked = true;
                       
                    }
                    else if (string.IsNullOrEmpty(reason))
                    {
                        sdr["订单号"] = waybillNo;
                        sdr["商家"] = Visitdic[waybillNo].MerchantName;
                        sdr["结算单位"] = Visitdic[waybillNo].AccountCompany;
                        sdr["区域类型"] = Visitdic[waybillNo].AreaType;
                        sdr["结算重量"] = Visitdic[waybillNo].AccountWeight;
                        sdr["收货人地址"] = Visitdic[waybillNo].Address;
                        sdr["最终发日期"] = Visitdic[waybillNo].DeliveryTime;
                        sdr["末级发货仓名称"] = Visitdic[waybillNo].FinalSorting;
                        sdr["状态"] = Visitdic[waybillNo].Status;
                        CheckSuccdt.Rows.Add(sdr);
                        CheckedSucc++;
                        Visitdic[waybillNo].IsChecked = true;
                        continue;
                    }
                    
                }
                if (!Visitdic.ContainsKey(waybillNo) && !Returnsdic.ContainsKey(waybillNo) && !Deliverydic.ContainsKey(waybillNo))
                {
                    dr["订单号"] =  waybillNo;;
                    dr["核对失败原因"] = "对方提供的订单号系统不存在";
                    Checkdt.Rows.Add(dr);
                    continue;
                }
               Checkdt.Rows.Add(dr);
            }
              return Checkdt;
        }

          protected void btnFailExcel_Click(object sender, EventArgs e)
          {
              string failedTitle = txtBeginTime.Text + "-" + txtEndTime.Text + DistributionCompany.SelectExpressName +
                                   "核对失败结果明细";
              if(ResultCheckdt.Rows.Count>0)
              {
                  CSVExport.DataTable2Excel(ResultCheckdt, failedTitle);
              }
              else
              {
                  Alert("没有数据");
              }

          }

          protected void btnSucOutExcel_Click(object sender, EventArgs e)
          {
              string succTile = txtBeginTime.Text + "-" + txtEndTime.Text + DistributionCompany.SelectExpressName +
                                "核对成功结果明细";
              if(CheckSuccdt.Rows.Count >0)
              {
                  CSVExport.DataTable2Excel(CheckSuccdt, succTile);
              }
              else
              {
                  Alert("没有数据");
              }
          }
   
    
    
    }

   
   

  
}