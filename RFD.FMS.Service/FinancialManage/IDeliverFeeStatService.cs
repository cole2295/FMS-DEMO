using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IDeliverFeeStatService
    {
        System.Data.DataTable GetCity(string strProvinceID);
        System.Data.DataTable GetDetail(System.Collections.Hashtable ht);
        System.Data.DataTable GetProvince();
        System.Data.DataTable GetQueryData(System.Collections.Hashtable ht);
        System.Data.DataTable GetQueryDataV2(System.Collections.Hashtable ht);
        System.Data.DataTable GetStationList(string strCityID);
    }
}
