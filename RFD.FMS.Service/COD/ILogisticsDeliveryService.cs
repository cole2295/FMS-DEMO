using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.COD
{
    public interface ILogisticsDeliveryService
    {
        DataTable SearchCodDetailsV2(CodSearchCondition condition, ref PageInfo pi, out DataTable amount);

        DataTable SearchCodDetails(CodSearchCondition condition, ref PageInfo pi, out DataTable amount);

        DataTable SearchCodStatV2(CodSearchCondition condition, out DataTable amount);

        DataTable SearchCodStat(CodSearchCondition condition, out DataTable amount);

        DataTable SearchExprotDetailDataV2(CodSearchCondition condition, string exprotPath);

        DataTable SearchExprotDetailData(CodSearchCondition condition, string exprotPath);

        DataTable SearchExprotStatDataV2(CodSearchCondition condition);

        DataTable SearchExprotStatData(CodSearchCondition condition);

        DataTable SearchLogisticsDailyV2(CODSearchCondition condition);

        DataTable SearchLogisticsDaily(CODSearchCondition condition, ref string TimeMessage);
    }
}
