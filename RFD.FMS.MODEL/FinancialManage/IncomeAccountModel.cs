using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    [Serializable]
    public class IncomeAccountModel : BaseModel
    {

    }

    [Serializable]
    public class IncomeSearchCondition : BaseModel
    {
        public String AccountNO { get; set; }
        public DateTime DateStr { get; set; }
        public DateTime DateEnd { get; set; }
        public String MerchantID { get; set; }
        public String MerchantName { get; set; }
    }

    [Serializable]
    public class IncomeAccountDetail : BaseModel
    {
        public String DetailID { get; set; }
        public String AccountNO { get; set; }
        public Int32 ExpressCompanyID { get; set; }
        public String CompanyName { get; set; }
        public String AreaType { get; set; }
        public Int32 DeliveryNum { get; set; }
        public Int32 DeliveryVNum { get; set; }
        public Int32 ReturnsNum { get; set; }
        public Int32 ReturnsVNum { get; set; }
        public Int32 VisitReturnsNum { get; set; }
        public Int32 VisitReturnsVNum { get; set; }
        public String DeliveryStandard { get; set; }
        public Decimal DeliveryFare { get; set; }
        public String DeliveryVStandard { get; set; }
        public Decimal DeliveryVFare { get; set; }
        public String RetrunsStandard { get; set; }
        public Decimal RetrunsFare { get; set; }
        public String ReturnsVStandard { get; set; }
        public Decimal ReturnsVFare { get; set; }
        public String VisitReturnsStandard { get; set; }
        public Decimal VisitReturnsFare { get; set; }
        public String VisitReturnsVStandard { get; set; }
        public Decimal VisitReturnsVFare { get; set; }
        public String ProtectedStandard { get; set; }
        public Decimal ProtectedFee { get; set; }
        public String ReceiveStandard { get; set; }
        public Decimal ReceiveFee { get; set; }
        public String ReceivePOSStandard { get; set; }
        public Decimal ReceivePOSFee { get; set; }
        public Decimal OtherFee { get; set; }
        public Decimal Fare { get; set; }
        public Int32 DataType { get; set; }
        public Decimal OverAreaSubsidy { get; set; }
        public Decimal KPI { get; set; }
        public Decimal LostDeduction { get; set; }
        public Decimal ResortDeduction { get; set; }
        public Int32 StationId { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal DiscountFee { get; set; }
    }

    /// <summary>
    /// 对外电子账单明细
    /// </summary>
    [Serializable]
    public class ExternalIncomeAccountDetail : BaseModel
    {
       
        public Int64 WaybillNo { get; set; }
       
        public string CustomerOrder { get; set; }
       
        public string BillType { get; set; }
       
        public string MerchantName { get; set; }
       
        public DateTime AcceptTime { get; set; }
      
        public string SortCompanyName { get; set; }
     
        public DateTime BackStationTime { get; set; }
        
        public string Status { get; set; }
      
        public DateTime ReturnTime { get; set; }
       
        public string ReturnStatus { get; set; }
      
        public decimal AccountWeight { get; set; }
      
        public decimal AccountFare { get; set; }
       
        public decimal NeedPayAmount { get; set; }

        public decimal NeedBackAmount { get; set; }
    
        public decimal ProtectedAmount { get; set; }
      
        public string VoildStatus { get; set; }
      
        public string PayType { get; set; }
        
        public string AreaType { get; set; }

        public string Province { get; set; }
      
        public string City { get; set; }
      
        public string Area { get; set; }
       
        public string Address { get; set; }

        public decimal ProtectedFee { get; set; }
     
        public decimal CashServiceFee { get; set; }
       
        public decimal ReceiveFee { get; set; }
       
        public decimal PosServiceFee { get; set; }
       
        public decimal PosReceiveFee { get; set; }
    }
    /// <summary>
    /// 对外电子账单总账
    /// </summary>
    [Serializable]
    public class ExternalIncomeAccount : BaseModel
    {
        [Description("结算单号")]
        public string AccountNO { get; set; }
        [Description("商家")]
        public string MerchantName { get; set; }
        [Description("结算状态")]
        public string AccountStatus { get; set; }
        [Description("实际结算费用")]
        public decimal Fare { get; set; }
        [Description("普发数")]
        public int DeliveryNum { get; set; }
        [Description("换发数")]
        public int DeliveryVNum { get; set; }
        [Description("普拒数")]
        public int ReturnsNum { get; set; }
        [Description("换拒数")]
        public int ReturnsVNum { get; set; }
        [Description("退数")]
        public int VisitReturnsNum { get; set; }
        [Description("退拒数")]
        public int VisitReturnsVNum { get; set; }
        [Description("普发配送费")]
        public decimal DeliveryFare { get; set; }
        [Description("换发配送费")]
        public decimal DeliveryVFare { get; set; }
        [Description("普拒配送费")]
        public decimal RetrunsFare { get; set; }
        [Description("换拒配送费")]
        public decimal ReturnsVFare { get; set; }
        [Description("退配送费")]
        public decimal VisitReturnsFare { get; set; }
        [Description("退拒配送费")]
        public decimal VisitReturnsVFare { get; set; }
        [Description("保价费")]
        public decimal ProtectedFee { get; set; }
        [Description("代收货款现金手续费")]
        public decimal ReceiveFee { get; set; }
        [Description("代收货款POS机手续费")]
        public decimal ReceivePOSFee { get; set; }
        [Description("延迟扣款")]
        public decimal DelayedDeductions { get; set; }
        [Description("丢失货款")]
        public Decimal LostDeductions { get; set; }
        [Description("提货费")]
        public decimal DeliveryFee { get; set; }
        [Description("折扣")]
        public decimal DiscountFee { get; set; }
        [Description("其他费用")]
        public decimal OtherFee { get; set; }
        //[Description("创建人")]
        //public string CreateBy { get; set; }
        //[Description("创建时间")]
        //public DateTime CreateTime { get; set; }
        //[Description("更新人")]
        //public string UpdateBy { get; set; }
        //[Description("更新时间")]
        //public DateTime UpdateTime { get; set; }
        //[Description("审核人")]
        //public string AuditBy { get; set; }
        //[Description("审核时间")]
        //public DateTime AuditTime { get; set; }

    }
}
