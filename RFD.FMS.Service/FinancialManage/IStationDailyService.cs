using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using RFD.FMS.DAL.FinancialManage;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Model.UnionPay;

namespace RFD.FMS.Service.FinancialManage
{
    /// <summary>
    ///  站点日报
    /// </summary>
    public interface IStationDailyService
    {
        bool AddDetailsV2(FMS_StationDailyFinanceDetails model);

        bool AddDetails(FMS_StationDailyFinanceDetails model);

        bool AddSumV2(FMS_StationDailyFinanceSum model);

        bool AddSum(FMS_StationDailyFinanceSum model);

        DataTable GetOrderDetil(DateTime dtDailyDate, string strStaionID, string strSources, string strMerchantID);

        DataTable GetOrderDetil(SearchModel searchInfo);

        IList<WaybillDistribution> ConvertToWaybillDistributionModelList(DataTable dataTable);

        WaybillDistribution GetModel(DataRow dataRow);

        void AddStationDailyDataV2(SearchModel searchInfo);

        void AddStationDailyData(SearchModel searchInfo);

        void WriteTest(string tips);

        bool ReloadStationDailyDataV2(SearchModel searchInfo);

        bool ReloadStationDailyData(SearchModel searchInfo);

        DataTable GetWaybillByNo(string waybillNo);

        int ReloadStationDailyByWaybillNosV2(string waybillNos);

        int ReloadStationDailyByWaybillNos(string waybillNos);

        #region IWaybillDistribution 成员
        int CountAll(IList<WaybillDistribution> waybillDistributions);

        int TodayCount(IList<WaybillDistribution> waybillDistributions, DateTime dateTime);

        decimal TodayIntoStationMastRefund(IList<WaybillDistribution> waybillDistributions, int? type,
                                                  DateTime dateTime, string stationCode, int? merchantID);

        int RejectWaybillCount(IList<WaybillDistribution> waybillDistributions);

        decimal TodayIntoStationNeedamountSum(IList<WaybillDistribution> waybillDistributions, DateTime dateTime);

        decimal AllRejectNeedamountSum(IList<WaybillDistribution> waybillDistributions);

        decimal TodayIntoStationNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions,
                                                         DateTime dateTime);

        decimal AllNeedamountSum(IList<WaybillDistribution> waybillDistributions);

        decimal AllRejectNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions);

        decimal TodayNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions, DateTime dateTime);

        int TodayRetentionCount(IList<WaybillDistribution> waybillDistributions);

        int TodayBackWaybillSuccessCount(IList<WaybillDistribution> waybillDistributions);

        int CashSuccessCount(IList<WaybillDistribution> waybillDistributions);

        decimal TodayRetentionNeedAmountSum(IList<WaybillDistribution> waybillDistributions);
        
        decimal CashFactAmountSum(IList<WaybillDistribution> waybillDistributions);

        decimal TodayRetentionNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions);

        decimal CashFactBackAmountSum(IList<WaybillDistribution> waybillDistributions);

        int PosSuccessCount(IList<WaybillDistribution> waybillDistributions);

        int TodayOutStationCount(int? type, DateTime dateTime, string stationCode, int? merchantID);

        decimal TodayOutStationNeedAmoutSum(int? type, DateTime dateTime, string stationCode, int? merchantID);

        decimal TodayOutStationNeedBackAmoutSum(int? type, DateTime dateTime, string stationCode, int? merchantID);

        int TodaySwitchIntoStationCount(IList<WaybillDistribution> waybillDistributions, int? type,
                                               DateTime dateTime, string stationCode, int? merchantID);

        decimal TodaySwitchIntoStationSum(IList<WaybillDistribution> waybillDistributions, int? type,
                                                 DateTime dateTime, string stationCode, int? merchantID);

        decimal PosFactAmountSum(IList<WaybillDistribution> waybillDistributions);

        decimal DistributionSuccessRate(IList<WaybillDistribution> waybillDistributions);

        decimal RetentionRate(IList<WaybillDistribution> waybillDistributions);

        int ZoreDepositCount(IList<WaybillDistribution> waybillDistributions);

        decimal RejectRate(IList<WaybillDistribution> waybillDistributions);

        decimal DepositSum(IList<WaybillDistribution> waybillDistributions);

        #endregion
    }
}