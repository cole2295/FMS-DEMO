using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Domain.COD
{
    public interface ILogisticsDeliveryDao
    {
        DataTable SearchCodDetailsV2(string condition, ref PageInfo pi, CodSearchCondition searchCondition);

        DataTable SearchCodDetails(string condition, ref PageInfo pi, CodSearchCondition searchCondition);
        DataTable SearchCodDetails<T>(string condition, ref PageInfo pi, CodSearchCondition searchCondition, List<T> parameterList,bool isPage);

        DataTable StatCodV2(string condition, CodSearchCondition searchCondition);

        DataTable StatCod(string condition, CodSearchCondition searchCondition);
        DataTable StatCod<T>(string condition, CodSearchCondition searchCondition, List<T> parameterList);

        DataTable SearchCodStatV2(string condition, CodSearchCondition searchCondition);

        DataTable SearchCodStat(string condition, CodSearchCondition searchCondition);

        DataTable SearchExprotDetailDataV2(string condition, CodSearchCondition searchCondition);

        DataTable SearchExprotDetailData(string condition, CodSearchCondition searchCondition);
        DataTable SearchExprotDetailData<T>(string condition, CodSearchCondition searchCondition, List<T> parameterList);

        DataTable SearchExprotStatDataV2(string condition, CodSearchCondition searchCondition);

        DataTable SearchExprotStatData(string condition, CodSearchCondition searchCondition);

        DataTable SearchLogisticsDeliverDailyV2(string condition, string type, CODSearchCondition csc, string accountDateColumnName);

        DataTable SearchLogisticsDeliverDaily(string condition, string type, CODSearchCondition csc, string accountDateColumnName);
        DataTable SearchLogisticsDeliverDaily<T>(string condition, string type, CODSearchCondition csc, string accountDateColumnName, List<T> parameterList);

        DataTable SearchLogisticsReturnsDailyV2(string condition, string type, CODSearchCondition csc, string accountDateColumnName);

        DataTable SearchLogisticsReturnsDaily(string condition, string type, CODSearchCondition csc, string accountDateColumnName);
        DataTable SearchLogisticsReturnsDaily<T>(string condition, string type, CODSearchCondition csc, string accountDateColumnName, List<T> parameterList);

        DataTable SearchLogisticsSignedReturnDaily(string condition, string type, CODSearchCondition csc,
                                                   string accountDateColumnName);
    }
}
