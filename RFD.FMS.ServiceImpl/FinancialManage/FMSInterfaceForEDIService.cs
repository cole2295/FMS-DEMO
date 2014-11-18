using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Activation;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
   [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class FMSInterfaceForEDIService : IFMSInterfaceForEDIService
    {
        private IncomeAccountDao incomeaccountdao = new IncomeAccountDao();

        ///// <summary>
        ///// 是否还有未对接过的账单
        ///// </summary>
        ///// <param name="MerchantID">商家id</param>
        ///// <param name="benginDate">开始时间 </param>
        ///// <param name="endDate">结束时间 </param>
        ///// <returns>true代表还有未对接的账单，false代表没有对接的账单</returns>
        //public bool IsAccessGetIncomeAccount(int MerchantID, DateTime benginDate, DateTime endDate)
        //{
        //    return incomeaccountdao.IsAccessGetIncomeAccount(MerchantID, benginDate, endDate);
        //}

        /// <summary>
        /// 根据商家id和记账日期返回账单明细
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        public IList<ExternalIncomeAccountDetail> GetIncomeAccountDetail(int MerchantID, DateTime benginDate,
                                                                        DateTime endDate)
        {
            if (incomeaccountdao.IsAccessGetIncomeAccount(MerchantID, benginDate, endDate))
            {
                var result = new List<ExternalIncomeAccountDetail>();
                //查出serchcondition 
                var dt = incomeaccountdao.GetAccountSearchCondition(MerchantID, benginDate, endDate);
                //
                if (dt != null && dt.Rows.Count > 0)
                {
                    var temp = new DataTable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var bengindate = DateTime.Parse(dr["SearchDateStr"].ToString());
                        var enddate = DateTime.Parse(dr["SearchDateEnd"].ToString());
                        var dataTable = incomeaccountdao.GetIncomeAccountDetail(MerchantID, bengindate, enddate);
                        if (temp.Columns.Count == 0)
                        {
                            temp = dataTable.Clone();
                        }
                        foreach (DataRow dataTabledr in dataTable.Rows)
                        {
                            var tempdr = temp.NewRow();
                            foreach (DataColumn dataColumn in dataTable.Columns)
                            {
                                tempdr[dataColumn.ColumnName] = dataTabledr[dataColumn.ColumnName];
                            }
                            temp.Rows.Add(tempdr);
                        }
                        //更新推送状态
                        
                    }
                  
                    if (temp.Rows.Count>0)
                    {
                        result.AddRange(from DataRow tempdr in temp.Rows select TransformFromDrToExternalIncomeAccountDetails(tempdr));
                    }
                    
                }
                return result;
            }
            return new List<ExternalIncomeAccountDetail>();
        }

        private ExternalIncomeAccountDetail TransformFromDrToExternalIncomeAccountDetails(DataRow dataRow)
        {
            var externalIncomeAccountDetails = new ExternalIncomeAccountDetail();
             //运单号")]
        // Int64 WaybillNo ;
            if (dataRow.Table.Columns.Contains("WaybillNo"))
            {
                if (dataRow["WaybillNo"]!=null&&!string.IsNullOrEmpty(dataRow["WaybillNo"].ToString()))
                {
                 externalIncomeAccountDetails.WaybillNo = Int64.Parse(dataRow["WaybillNo"].ToString());
                }
            }
        //订单号")]
        // string CustomerOrder ;
            if (dataRow.Table.Columns.Contains("CustomerOrder"))
            {
                externalIncomeAccountDetails.CustomerOrder = dataRow["CustomerOrder"].ToString();
            }
           
        //运单类型")]
        // string BillType ;
             if (dataRow.Table.Columns.Contains("BillType"))
             {
                 externalIncomeAccountDetails.BillType = dataRow["BillType"].ToString();
             }
        //商家")]
        // string MerchantName ;
             if (dataRow.Table.Columns.Contains("MerchantName"))
             {
                 externalIncomeAccountDetails.MerchantName = dataRow["MerchantName"].ToString();
             }

        //接单时间")]
        // DateTime AcceptTime ;
            externalIncomeAccountDetails.AcceptTime = Convert.ToDateTime("1900-01-01");
             if (dataRow.Table.Columns.Contains("AcceptTime"))
            {
                if (dataRow["AcceptTime"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["AcceptTime"].ToString()))
                {
                    externalIncomeAccountDetails.AcceptTime = Convert.ToDateTime(dataRow["AcceptTime"].ToString());
                }
            }
        //发货分拣中心")]
        // string SortCompanyName ;
            if (dataRow.Table.Columns.Contains("SortCompanyName"))
            {
                if (dataRow["SortCompanyName"]!=DBNull.Value)
                {
                  externalIncomeAccountDetails.SortCompanyName = dataRow["SortCompanyName"].ToString();
                }
            }
        //归班时间")]
        // DateTime BackStationTime ;
            externalIncomeAccountDetails.BackStationTime = Convert.ToDateTime("1900-01-01");
            if (dataRow.Table.Columns.Contains("BackStationTime"))
            {
                if (dataRow["BackStationTime"]!=DBNull.Value&&!string.IsNullOrEmpty(dataRow["BackStationTime"].ToString()))
                {
                    externalIncomeAccountDetails.BackStationTime = DateTime.Parse(dataRow["BackStationTime"].ToString());
                }  
            }
        //运单状态")]
        // string Status ;
            if (dataRow.Table.Columns.Contains("Status"))
            {
                if (dataRow["Status"]!=DBNull.Value)
                {
                    externalIncomeAccountDetails.Status = dataRow["Status"].ToString();
                }  
            }
        //拒收入库时间")]
        // DateTime ReturnTime ;
            externalIncomeAccountDetails.ReturnTime = Convert.ToDateTime("1900-01-01");
            if (dataRow.Table.Columns.Contains("ReturnTime"))
            {
                if (dataRow["ReturnTime"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["BackStationTime"].ToString()))
                {
                    externalIncomeAccountDetails.ReturnTime = Convert.ToDateTime(dataRow["BackStationTime"]);
                } 
            }
        //返货状态")]
        // string ReturnStatus ;
            if (dataRow.Table.Columns.Contains("ReturnStatus"))
            {
                if (dataRow["ReturnStatus"]!=DBNull.Value)
                {
                    externalIncomeAccountDetails.ReturnStatus = dataRow["ReturnStatus"].ToString();
                }  
            }
        //结算重量")]
        // decimal AccountWeight ;
            if (dataRow.Table.Columns.Contains("AccountWeight"))
            {
                if (dataRow["AccountWeight"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["AccountWeight"].ToString()))
                {
                    externalIncomeAccountDetails.AccountWeight = decimal.Parse(dataRow["AccountWeight"].ToString());
                }
            }
        //配送费")]
        // decimal AccountFare ;
            if (dataRow.Table.Columns.Contains("AccountFare"))
            {
                if (dataRow["AccountFare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["AccountFare"].ToString()))
                {
                    externalIncomeAccountDetails.AccountFare = decimal.Parse(dataRow["AccountFare"].ToString());
                }
            }
            if (dataRow.Table.Columns.Contains("NeedPayAmount"))
            {
                if (dataRow["NeedPayAmount"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["NeedPayAmount"].ToString()))
                {
                    externalIncomeAccountDetails.NeedPayAmount = decimal.Parse(dataRow["NeedPayAmount"].ToString());
                } 
            }
        //应收款")]
        // decimal NeedPayAmount ;
            if (dataRow.Table.Columns.Contains("NeedPayAmount"))
            {
                if (dataRow["NeedPayAmount"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["NeedPayAmount"].ToString()))
                {
                    externalIncomeAccountDetails.NeedPayAmount = decimal.Parse(dataRow["NeedPayAmount"].ToString());
                }
            }
        //应退金额")]
        // decimal NeedBackAmount ;
            if (dataRow.Table.Columns.Contains("NeedBackAmount"))
            {
                if (dataRow["NeedBackAmount"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["NeedBackAmount"].ToString()))
                {
                    externalIncomeAccountDetails.NeedBackAmount = decimal.Parse(dataRow["NeedBackAmount"].ToString());
                }
            }
        //保价金额")]
        // decimal ProtectedAmount ;
            if (dataRow.Table.Columns.Contains("ProtectedAmount"))
            {
                if (dataRow["ProtectedAmount"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ProtectedAmount"].ToString()))
                {
                    externalIncomeAccountDetails.ProtectedAmount = decimal.Parse(dataRow["ProtectedAmount"].ToString());
                }
            }
        //无效状态")]
        // string VoildStatus ;
            if (dataRow.Table.Columns.Contains("VoildStatus"))
            {
                if (dataRow["VoildStatus"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["VoildStatus"].ToString()))
                {
                    externalIncomeAccountDetails.VoildStatus = dataRow["VoildStatus"].ToString();
                } 
            }
        //支付方式")]
        // string PayType ;
            if (dataRow.Table.Columns.Contains("PayType"))
            {
                if (dataRow["PayType"] != DBNull.Value )
                {
                    externalIncomeAccountDetails.PayType = dataRow["PayType"].ToString();
                }
            }
        //收入区域类型")]
        // string AreaType ;
            if (dataRow.Table.Columns.Contains("AreaType"))
            {
                if (dataRow["AreaType"]!=DBNull.Value)
                {
                    externalIncomeAccountDetails.AreaType = dataRow["AreaType"].ToString();
                }
            }
        //省")]
        // string Province ;
            if (dataRow.Table.Columns.Contains("Province"))
            {
                if (dataRow["Province"]!=DBNull.Value)
                {
                    externalIncomeAccountDetails.Province = dataRow["Province"].ToString();
                }
            }
        //市")]
        // string City ;
            if (dataRow.Table.Columns.Contains("City"))
            {
                if (dataRow["City"]!=DBNull.Value)
                {
                    externalIncomeAccountDetails.City = dataRow["City"].ToString();
                } 
            }
        //区")]
        // string Area ;
            if (dataRow.Table.Columns.Contains("Area"))
            {
                if (dataRow["Area"]!=DBNull.Value)
                {
                    externalIncomeAccountDetails.Area = dataRow["Area"].ToString();
                } 
            }
        //地址")]
        // string Address ;
            if (dataRow.Table.Columns.Contains("Address"))
            {
                if (dataRow["Address"]!=DBNull.Value)
                {
                    externalIncomeAccountDetails.Address = dataRow["Address"].ToString();
                } 
            }
        //保价费")]
        // decimal ProtectedFee ;
            if (dataRow.Table.Columns.Contains("ProtectedFee"))
            {
                if (dataRow["ProtectedFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ProtectedFee"].ToString()))
                {
                    externalIncomeAccountDetails.ProtectedFee = decimal.Parse(dataRow["ProtectedFee"].ToString());
                }
            }
        //现金服务费")]
        // decimal CashServiceFee ;
            if (dataRow.Table.Columns.Contains("CashServiceFee"))
            {
                if (dataRow["CashServiceFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["CashServiceFee"].ToString()))
                {
                    externalIncomeAccountDetails.CashServiceFee = decimal.Parse(dataRow["CashServiceFee"].ToString());
                } 
            }
        //现金手续费")]
        // decimal ReceiveFee ;
            if (dataRow.Table.Columns.Contains("ReceiveFee"))
            {
                if (dataRow["ReceiveFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ReceiveFee"].ToString()))
                {
                    externalIncomeAccountDetails.ReceiveFee = decimal.Parse(dataRow["ReceiveFee"].ToString());
                } 
            }
        ////POS服务费")]
        //// decimal PosServiceFee ;
        //    if (dataRow.Table.Columns.Contains("PosServiceFee"))
        //    {
        //        if (dataRow["PosServiceFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["PosServiceFee"].ToString()))
        //        {
        //            externalIncomeAccountDetails.PosServiceFee = decimal.Parse(dataRow["PosServiceFee"].ToString());

        //        }   
        //    }
        ////POS手续费")]
        //// decimal PosReceiveFee ;
        //    if (dataRow.Table.Columns.Contains("PosReceiveFee"))
        //    {
        //        if (dataRow["PosReceiveFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["PosReceiveFee"].ToString()))
        //        {
        //            externalIncomeAccountDetails.PosServiceFee = decimal.Parse(dataRow["PosReceiveFee"].ToString());
        //        }
        //    }
            return externalIncomeAccountDetails;
        }

        /// <summary>
        /// 根据商家id和记账日期返回账单总表
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        public ExternalIncomeAccount GetIncomeAccount(int MerchantID, DateTime benginDate, DateTime endDate)
        {
            if (incomeaccountdao.IsAccessGetIncomeAccount(MerchantID, benginDate, endDate))
            {
                DataTable dataTable= incomeaccountdao.GetUnPushedIncomeAccountAndDetail(MerchantID, benginDate, endDate);
                if (dataTable!=null&&dataTable.Rows.Count>0)
                {
                    var result = TransformFromDrToExternalIncomeAccount(dataTable.Rows[0]);
                    //更新推送数据状态

                    return result;

                }
            }
            return new ExternalIncomeAccount();
        }

        public IList<ExternalIncomeAccountDetail> GetIncomeAccountDetailByBillType(int MerchantID, DateTime benginDate, DateTime endDate, List<int> BillType)
       {
           //if (incomeaccountdao.IsAccessGetIncomeAccount(MerchantID, benginDate, endDate))
           //{
               var result = new List<ExternalIncomeAccountDetail>();
               //查出serchcondition 
               //var dt = incomeaccountdao.GetAccountSearchCondition(MerchantID, benginDate, endDate);
               ////
               //if (dt != null && dt.Rows.Count > 0)
               //{
                   var temp = new DataTable();
               //    foreach (DataRow dr in dt.Rows)
               //    {
               //        var bengindate = DateTime.Parse(dr["SearchDateStr"].ToString());
               //        var enddate = DateTime.Parse(dr["SearchDateEnd"].ToString());

                   var dataTable = incomeaccountdao.GetNormalIncomeAccountDetail(MerchantID, benginDate, endDate, BillType);
                       if (temp.Columns.Count == 0)
                       {
                           temp = dataTable.Clone();
                       }
                       foreach (DataRow dataTabledr in dataTable.Rows)
                       {
                           var tempdr = temp.NewRow();
                           foreach (DataColumn dataColumn in dataTable.Columns)
                           {
                               tempdr[dataColumn.ColumnName] = dataTabledr[dataColumn.ColumnName];
                           }
                           temp.Rows.Add(tempdr);
                       }
                       //更新推送状态

                   //}

                   if (temp.Rows.Count > 0)
                   {
                       result.AddRange(from DataRow tempdr in temp.Rows select TransformFromDrToExternalIncomeAccountDetails(tempdr));
                   }

               //}
               return result;
           //}
           //return new List<ExternalIncomeAccountDetail>();
       }

       private ExternalIncomeAccount TransformFromDrToExternalIncomeAccount(DataRow dataRow)
        { var result=new ExternalIncomeAccount();
            //"结算单号")]
        // public string AccountNO { get; set; }
        if (dataRow.Table.Columns.Contains("AccountNO"))
        {
            result.AccountNO = dataRow["AccountNO"].ToString();
        }
        //"商家")]
        // public string MerchantName { get; set; }
        if (dataRow.Table.Columns.Contains("MerchantName"))
        {
            result.MerchantName = dataRow["MerchantName"].ToString();
        }
        //"结算状态")]
        // public string AccountStatus { get; set; }
        if (dataRow.Table.Columns.Contains("AccountStatus"))
        {
            result.AccountStatus = dataRow["AccountStatus"].ToString();
        }
        //"实际结算费用")]
        // public decimal Fare { get; set; }
        if (dataRow.Table.Columns.Contains("Fare"))
            {
                if (dataRow["Fare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["Fare"].ToString()))
                {
                    result.Fare = decimal.Parse(dataRow["Fare"].ToString());
                }
            }
        //"普发数")]
        // public int DeliveryNum { get; set; }
        if (dataRow.Table.Columns.Contains("DeliveryNum"))
            {
                if (dataRow["DeliveryNum"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["DeliveryNum"].ToString()))
                {
                    result.DeliveryNum = Int32.Parse(dataRow["DeliveryNum"].ToString());
                }
            }
        //"换发数")]
        // public int DeliveryVNum { get; set; }
        if (dataRow.Table.Columns.Contains("DeliveryVNum"))
        {
            if (dataRow["DeliveryVNum"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["DeliveryVNum"].ToString()))
            {
                result.DeliveryNum = Int32.Parse(dataRow["DeliveryVNum"].ToString());
            }
        }
        //"普拒数")]
        // public int ReturnsNum { get; set; }
        if (dataRow.Table.Columns.Contains("ReturnsNum"))
        {
            if (dataRow["ReturnsNum"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ReturnsNum"].ToString()))
            {
                result.ReturnsNum = Int32.Parse(dataRow["ReturnsNum"].ToString());
            }
        }
        //"换拒数")]
        // public int ReturnsVNum { get; set; }
        if (dataRow.Table.Columns.Contains("ReturnsVNum"))
        {
            if (dataRow["ReturnsVNum"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ReturnsVNum"].ToString()))
            {
                result.ReturnsVNum = Int32.Parse(dataRow["ReturnsVNum"].ToString());
            }
        }
        //"退数")]
        // public int VisitReturnsNum { get; set; }
        if (dataRow.Table.Columns.Contains("VisitReturnsNum"))
        {
            if (dataRow["VisitReturnsNum"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["VisitReturnsNum"].ToString()))
            {
                result.VisitReturnsNum = Int32.Parse(dataRow["VisitReturnsNum"].ToString());
            }
        }
        //"退拒数")]
        // public int VisitReturnsVNum { get; set; }
        if (dataRow.Table.Columns.Contains("VisitReturnsVNum"))
        {
            if (dataRow["VisitReturnsVNum"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["VisitReturnsVNum"].ToString()))
            {
                result.VisitReturnsVNum = Int32.Parse(dataRow["VisitReturnsVNum"].ToString());
            }
        }
        //"普发配送费")]
        // public decimal DeliveryFare { get; set; }
        if (dataRow.Table.Columns.Contains("DeliveryFare"))
        {
            if (dataRow["DeliveryFare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["DeliveryFare"].ToString()))
            {
                result.DeliveryFare = decimal.Parse(dataRow["DeliveryFare"].ToString());
            }
        }
        //"换发配送费")]
        // public decimal DeliveryVFare { get; set; }
        if (dataRow.Table.Columns.Contains("DeliveryVFare"))
        {
            if (dataRow["DeliveryVFare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["DeliveryVFare"].ToString()))
            {
                result.DeliveryVFare = decimal.Parse(dataRow["DeliveryVFare"].ToString());
            }
        }
        //"普拒配送费")]
        // public decimal RetrunsFare { get; set; }
        if (dataRow.Table.Columns.Contains("RetrunsFare"))
        {
            if (dataRow["RetrunsFare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["RetrunsFare"].ToString()))
            {
                result.RetrunsFare = decimal.Parse(dataRow["RetrunsFare"].ToString());
            }
        }
        //"换拒配送费")]
        // public decimal ReturnsVFare { get; set; }
        if (dataRow.Table.Columns.Contains("ReturnsVFare"))
        {
            if (dataRow["ReturnsVFare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ReturnsVFare"].ToString()))
            {
                result.ReturnsVFare = decimal.Parse(dataRow["ReturnsVFare"].ToString());
            }
        }
        //"退配送费")]
        // public decimal VisitReturnsFare { get; set; }
        if (dataRow.Table.Columns.Contains("VisitReturnsFare"))
        {
            if (dataRow["VisitReturnsFare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["VisitReturnsFare"].ToString()))
            {
                result.VisitReturnsFare = decimal.Parse(dataRow["VisitReturnsFare"].ToString());
            }
        }
        //"退拒配送费")]
        // public decimal VisitReturnsVFare { get; set; }
        if (dataRow.Table.Columns.Contains("VisitReturnsVFare"))
        {
            if (dataRow["VisitReturnsVFare"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["VisitReturnsVFare"].ToString()))
            {
                result.VisitReturnsVFare = decimal.Parse(dataRow["VisitReturnsVFare"].ToString());
            }
        }
        //"保价费")]
        // public decimal ProtectedFee { get; set; }
        if (dataRow.Table.Columns.Contains("ProtectedFee"))
        {
            if (dataRow["ProtectedFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ProtectedFee"].ToString()))
            {
                result.ProtectedFee = decimal.Parse(dataRow["ProtectedFee"].ToString());
            }
        }
        //"代收货款现金手续费")]
        // public decimal ReceiveFee { get; set; }
        if (dataRow.Table.Columns.Contains("ReceiveFee"))
        {
            if (dataRow["ReceiveFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ReceiveFee"].ToString()))
            {
                result.ReceiveFee = decimal.Parse(dataRow["ReceiveFee"].ToString());
            }
        }
        //"代收货款POS机手续费")]
        // public decimal ReceivePOSFee { get; set; }
        if (dataRow.Table.Columns.Contains("ReceivePOSFee"))
        {
            if (dataRow["ReceivePOSFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["ReceivePOSFee"].ToString()))
            {
                result.ReceivePOSFee = decimal.Parse(dataRow["ReceivePOSFee"].ToString());
            }
        }
        //"延迟扣款")]
        // public decimal DelayedDeductions { get; set; }
        if (dataRow.Table.Columns.Contains("DelayedDeductions"))
        {
            if (dataRow["DelayedDeductions"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["DelayedDeductions"].ToString()))
            {
                result.DelayedDeductions = decimal.Parse(dataRow["DelayedDeductions"].ToString());
            }
        }
        //"丢失货款")]
        // public Decimal LostDeductions { get; set; }
        if (dataRow.Table.Columns.Contains("LostDeductions"))
        {
            if (dataRow["LostDeductions"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["LostDeductions"].ToString()))
            {
                result.LostDeductions = decimal.Parse(dataRow["LostDeductions"].ToString());
            }
        }
        //"提货费")]
        // public decimal DeliveryFee { get; set; }
        if (dataRow.Table.Columns.Contains("DeliveryFee"))
        {
            if (dataRow["DeliveryFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["DeliveryFee"].ToString()))
            {
                result.DeliveryFee = decimal.Parse(dataRow["DeliveryFee"].ToString());
            }
        }
        //"折扣")]
        // public decimal DiscountFee { get; set; }
        if (dataRow.Table.Columns.Contains("DiscountFee"))
        {
            if (dataRow["DiscountFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["DiscountFee"].ToString()))
            {
                result.DiscountFee = decimal.Parse(dataRow["DiscountFee"].ToString());
            }
        }
        //"其他费用")]
        // public decimal OtherFee { get; set; }
        if (dataRow.Table.Columns.Contains("OtherFee"))
        {
            if (dataRow["OtherFee"] != DBNull.Value && !string.IsNullOrEmpty(dataRow["OtherFee"].ToString()))
            {
                result.OtherFee = decimal.Parse(dataRow["OtherFee"].ToString());
            }
        }
           return  result;
        }
    }
}