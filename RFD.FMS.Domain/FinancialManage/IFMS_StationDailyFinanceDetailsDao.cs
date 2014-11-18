using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IFMS_StationDailyFinanceDetailsDao
    {
        int Add(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceDetails model);
        int AddV2(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceDetails model);
        bool Exists(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceDetails model);
        bool ExistsV2(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceDetails model);
        System.Data.DataTable GetOrderDetil(DateTime dtDailyDate, string strStaionID, string strSources, string strMerchantID);
        System.Data.DataTable GetTrunIntoStationSummaryByStationAndTime(int? type, string stationCode, DateTime dateTime, int? merchantID);
        System.Data.DataTable GetTrunOutStationSummaryByStationAndTime(int? type, string stationCode, DateTime dateTime, int? merchantID);
    }
}
