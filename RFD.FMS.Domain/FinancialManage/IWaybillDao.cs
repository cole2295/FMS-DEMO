using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IWaybillDao
    {
        string CreateTempTable(RFD.FMS.MODEL.BasicSetting.ThirdPartyWaybillSearchConditons conditions);
        string Details(RFD.FMS.MODEL.BasicSetting.ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref RFD.FMS.MODEL.PageInfo pi, int num);
        System.Data.DataTable GetThirdWaybillDetails(RFD.FMS.MODEL.BasicSetting.ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref RFD.FMS.MODEL.PageInfo pi, out System.Data.DataTable amount);
        System.Data.DataTable GetThirdWaybillDetailsV2(RFD.FMS.MODEL.BasicSetting.ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref RFD.FMS.MODEL.PageInfo pi, out System.Data.DataTable amount);
        System.Data.DataTable GetThirdWaybillStat(RFD.FMS.MODEL.BasicSetting.ThirdPartyWaybillSearchConditons conditions);
        System.Data.DataTable StatisticsForDetails(RFD.FMS.MODEL.BasicSetting.ThirdPartyWaybillSearchConditons conditions);
        System.Data.DataTable Summary(RFD.FMS.MODEL.BasicSetting.ThirdPartyWaybillSearchConditons conditions);
        DataTable GetAllSortingData(Int64 waybillNo);
        DataTable GetSortingToStation(long waybillNo);
        DataTable GetSortingToCity(long waybillNo);
        DataTable GetReturnToSortingCenter(long waybillNo);
        DataTable GetMerchantToSortingCenter(long waybillNo);
    }
}
