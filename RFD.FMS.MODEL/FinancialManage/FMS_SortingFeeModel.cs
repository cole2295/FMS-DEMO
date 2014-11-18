using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    public class FMS_SortingFeeModel
    {
        public string SortingFeeID { get; set; }
        public string SortingFeeWaitID { get; set; }
        public int SortingMerchantID { get; set; }
        public int SortingCenterID { get; set; }
        public string CityID { get; set; }
        public int FareType { get; set; }
        public string AccountFare { get; set; }
        public int Status { get; set; }
        public int WaybillCount { get; set; }
        public int IsAccountBill { get; set; }
        public int CreateBy { get; set; }
        public int AuditBy { get; set; }
        public int UpdateBy { get; set; }
        public string DistributionCode { get; set; }
        public DateTime EffectDate { get; set; }
        public int MerchantID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsChange { get; set; }
        //查询条件
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SortingMerchantIDs { get; set; }
        public string SortingCenterIDs { get; set; }
        public string CityIDs { get; set; }
        public string MerchanIDs { get; set; }
    }
}
