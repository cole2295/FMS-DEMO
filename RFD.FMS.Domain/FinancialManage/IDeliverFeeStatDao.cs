using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IDeliverFeeStatDao
    {
        decimal? GetDeliverFee(int? MerchantID, decimal? Weight, decimal? Volume, RFD.FMS.MODEL.Enumeration.WayBillStatus status, out decimal Protectedprice);
        System.Data.DataTable GetDetail(System.Collections.Hashtable ht);
        System.Data.DataTable GetQueryData(System.Collections.Hashtable ht);
        System.Data.DataTable GetQueryDataV2(System.Collections.Hashtable ht);
        System.Data.DataTable GetStationList(string strCityID);
    }
}
