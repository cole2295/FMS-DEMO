//2011-10-13 星期四
//马中华

using System;
using System.Data;
using RFD.FMS.MODEL.COD;
using RFD.FMS.MODEL;
using System.Collections.Generic;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Domain.COD
{
    public interface ICODBaseInfoDao
    {
        int Add(FMS_CODBaseInfo model);

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        FMS_CODBaseInfo GetModel(long id);

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        FMS_CODBaseInfo GetModelByWaybillNO(Int64 waybillNo);

        /// <summary>
        /// 根据条件得到一个对象实体集
        /// </summary>
        List<FMS_CODBaseInfo> GetModelList(Dictionary<string, object> searchParams);

        /// <summary>
        /// 更改金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateAmountByID(FMS_CODBaseInfo info);

        /// <summary>
        /// 将制定ID置为停用isdeleted=1
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateIsDeletedByID(FMS_CODBaseInfo info);

        List<FMS_CODBaseInfo> GetDeliverDetails(int accountDays, int tops, string syncStartTime);

        List<FMS_CODBaseInfo> GetReturnDetails(int accountDays, int tops, string syncStartTime);

        List<FMS_CODBaseInfo> GetVisitReturnDetails(int accountDays, int tops, string syncStartTime);

        bool UpdateCodFare(FMS_CODBaseInfo detail);

        bool UpdateBackError(FMS_CODBaseInfo detail);

        List<CodStatsLogModel> GetDeliverToDayStatsInfo(DateTime accountDate);

        int GetDeliverAllCountByExpressWareHose(CodStatsLogModel codStatsLog);

        int GetDeliverFareCountByExpressWareHouse(CodStatsLogModel codStatsLog);

        List<CodStatsModel> GetDeliverAccountByDay(CodStatsLogModel codStatsLog);

        List<CodStatsLogModel> GetReturnToDayStatsInfo(DateTime accountDate);

        int GetReturnAllCountByExpressWareHose(CodStatsLogModel codStatsLog);

        int GetReturnFareCountByExpressWareHouse(CodStatsLogModel codStatsLog);

        List<CodStatsModel> GetReturnAccountByDay(CodStatsLogModel codStatsLog);

        List<CodStatsLogModel> GetVisitToDayStatsInfo(DateTime accountDate);

        int GetVisitAllCountByExpressWareHose(CodStatsLogModel codStatsLog);

        int GetVisitFareCountByExpressWareHouse(CodStatsLogModel codStatsLog);

        List<CodStatsModel> GetVisitAccountByDay(CodStatsLogModel codStatsLog);

        bool InsertDeliverAccount(List<CodStatsModel> codStatsList, string date);

        bool InsertReturnsAccount(List<CodStatsModel> codStatsList, string date);

        bool InsertVisitReturnsAccount(List<CodStatsModel> codStatsList, string date);

        bool JudgeLogExists(CodStatsLogModel codStatsLog);

        bool UpdateStatisticsLog(CodStatsLogModel codStatsLog);

        bool WriteStatisticsLog(CodStatsLogModel codStatsLog);

        List<CodStatsLogModel> GetStatsLogError(int statisticsType, string dateRemove, int accountDays);

        DataTable GetDeliver(int accountDays);

        bool ChangeDeliverBack(List<string> noList);

        DataTable GetError8(int accountDays);

        DataTable GetError9(int accountDays);

        DataTable GetError7(int accountDays);

        DataTable GetError6(int accountDays);

        DataTable GetError5(int accountDays);

        DataTable GetError34(int errorType, int accountDays);

        DataTable GetDeliveryList(FMS_CODBaseInfoCheck model);

        DataTable GetReturnList(FMS_CODBaseInfoCheck model);

        DataTable GetVisitList(FMS_CODBaseInfoCheck model);

        /// <summary>
        /// 查询运单配送费计算参数
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <returns>参数</returns>
        DataTable GetDeliverFeeParameter(long waybillNo, string distributionCode);

        /// <summary>
        /// 保存修改的配送费信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SaveDeliverFee(DeliverFeeModel model);
        /// <summary>
        /// 根据ID修改配送费信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SaveDeliverFeeByID(DeliverFeeModel model);

        /// <summary>
        /// 更新配送费是否已计算标志，重新计算配送费
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <returns>是否成功</returns>
        bool UpdateEvalStatus(long waybillNo);

        /// <summary>
        /// 更新配送费是否已计算标志，重新计算配送费
        /// </summary>
        /// <param name="waybillNo">运单号集合</param>
        /// <returns>是否成功</returns>
        bool UpdateEvalStatus(string waybillNos);

        bool UpdateEvalStatusByInfoID(string infoIDs);

        DataTable GetCODDeliveryFeeInfo(string InfoIDs);

        bool ExsitCodBaseInfoByNo(long waybillNo,long infoID);
    }
}
