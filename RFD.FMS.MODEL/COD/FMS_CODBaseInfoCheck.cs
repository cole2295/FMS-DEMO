using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.COD
{
    public class FMS_CODBaseInfoCheck
    {
        public DateTime STime { get; set; }
        public DateTime ETime { get; set; }
        public int DistributionCompany { get; set; }
        public string DistributionCode { get; set; }
        public long WaybillNo { get; set; }
        public int AreaType { get; set; }
        public int Status { get; set; }
        public decimal fee { get; set; }
        public int WaybillType { get; set; }
        public string MerchantName { get; set; }
        public string AccountCompany { get; set; }
        public decimal AccountWeight { get; set; }
        public string Address { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string FinalSorting { get; set; }
        public bool IsChecked { get; set; }
        public string MerchantIDs { get; set; }


    }
}
