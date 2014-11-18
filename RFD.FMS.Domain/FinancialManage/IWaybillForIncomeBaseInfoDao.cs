using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IWaybillForIncomeBaseInfoDao
    {
        DataTable SearchDetails<T>(string conditionStr,List<T> parameterList, ref PageInfo pi, bool isPage);
        DataTable SearchDetailsV2<T>(string conditionStr, List<T> parameterList, ref PageInfo pi, bool isPage);
        DataTable SearchStat<T>(string conditionStr, List<T> parameterList);
        DataTable SearchStatV2<T>(string conditionStr, List<T> parameterList);
        DataTable SearchSummary<T>(string conditionStr, List<T> parameterList, ThirdPartyWaybillSearchConditons condition);

        DataTable GetWaybillInfoByNOForIncomeBaseInfo(long waybillNO);
    }
}
