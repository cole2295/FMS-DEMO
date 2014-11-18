using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
   public class SortingDetail
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long WaybillNo { get; set; }
        public string SortingMerchantIDs { get; set; }
        public bool SortingMerchantChk { get; set; }
        public bool MerchantChk { get; set; }
        public string MerchantIDs { get; set; }
        public string SortingCenterIDs { get; set; }
        public bool SortingCenterChk { get; set;}
        public string CityIDs { get; set; }
        public bool CityChk { get; set; }
        public string StationIDs { get; set; }
        public bool StationChk { get; set; }
        public string DistributionCodes { get; set;}
        public bool DistributionChk { get; set; }
        public string waybillType { get; set; }
        public bool waybillTypeChk { get; set; }
        public string DistributionCode { get; set; }
        public int InSortingCount { get; set; }
        public int ItemType { get; set; }
        public int startRowNum { get; set; }
        public int endRowNum { get; set; }
        
    }
}
