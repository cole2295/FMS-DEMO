using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IFMS_StationDailyFinanceSumDao
    {
        int Add(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        int AddV2(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        bool Delete(RFD.FMS.MODEL.SearchModel searchInfo);
        bool DeleteV2(RFD.FMS.MODEL.SearchModel searchInfo);
        bool Exists(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        bool ExistsV2(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        System.Data.DataTable GetWaybillByNo(string waybillNo);
    }
}
