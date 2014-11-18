using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using RFD.Sync.Impl.Master2Slave;
using RFD.Sync.AdoNet;
using System.Threading;
using Microsoft.ApplicationBlocks.Data;
using System.Data;

namespace RFD.SyncData.WinService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };

            ServiceBase.Run(ServicesToRun);

            //test

            //M2S_FMS_StationDeliverFee stationDeliverFee = new M2S_FMS_StationDeliverFee();

            //stationDeliverFee.ExecuteSync();

           //M2S_FMS_SortingTransferDetail detail = new M2S_FMS_SortingTransferDetail();
           //detail.ExecuteSync();


            //M2S_FMS_IncomeBaseInfo_History incomeBaseInfo_His = new M2S_FMS_IncomeBaseInfo_History();
            //incomeBaseInfo_His.ExecuteSync();

            //M2S_FMS_IncomeFeeInfo_History incomeFeeInfo_His = new M2S_FMS_IncomeFeeInfo_History();
            //incomeFeeInfo_His.ExecuteSync();

            //M2S_FMS_CODBaseInfo_History codBaseInfo_His = new M2S_FMS_CODBaseInfo_History();
            //codBaseInfo_His.ExecuteSync();

            //string connstr = "OaUiMMBYnrZ1gv4wwBoL9C+ta+Rckt2bxZjaIEqAY6t2LR9XNG+M72QVhaViTC71fdNDI/gKlRf9PtIE45u6ZgNaGqlx2YEyFGXeeRNm6zymJ5Wu6UB4cEKrrSWKYH9bZWQzr8MpuznJvfE0yWpsqo3sw9L7jAI1um1X9kOXoOMdVxbPkpAKOYPoWXqglaVYL61r5FyS3Ztv14h4En3TzA==";

            //var cons = DbDES.Decrypt3DES(connstr, Encoding.UTF8);

            //RunSQL(cons);

            //while (true)
            //{
            //M2S_FMS_CODBaseInfo codBaseInfo = new M2S_FMS_CODBaseInfo();

            //codBaseInfo.ExecuteSync();

            //    M2S_FMS_IncomeBaseInfo incomeBaseInfo = new M2S_FMS_IncomeBaseInfo();

            //    incomeBaseInfo.ExecuteSync();

            //    M2S_FMS_IncomeFeeInfo incomeFeeInfo = new M2S_FMS_IncomeFeeInfo();

            //    incomeFeeInfo.ExecuteSync();

                //M2S_FMS_StationDailyFinanceDetails financeDetails = new M2S_FMS_StationDailyFinanceDetails();

                //financeDetails.ExecuteSync();

                //M2S_FMS_StationDailyFinanceSum financeSum = new M2S_FMS_StationDailyFinanceSum();

                //financeSum.ExecuteSync();

            //    M2S_AreaExpressLevel areaExpressLevel = new M2S_AreaExpressLevel();

            //    areaExpressLevel.ExecuteSync();

            //    M2S_AreaExpressLevelIncome areaExpressLevelIncome = new M2S_AreaExpressLevelIncome();

            //    areaExpressLevelIncome.ExecuteSync();

            //    M2S_AreaExpressLevelIncomeLog areaExpressLevelIncomeLog = new M2S_AreaExpressLevelIncomeLog();

            //    areaExpressLevelIncomeLog.ExecuteSync();

            //    M2S_AreaExpressLevelLog areaExpressLevelLog = new M2S_AreaExpressLevelLog();

            //    areaExpressLevelLog.ExecuteSync();

            //    M2S_FMS_CODAccount codAccount = new M2S_FMS_CODAccount();

            //    codAccount.ExecuteSync();

            //    M2S_FMS_CODAccountDetail codAccountDetails = new M2S_FMS_CODAccountDetail();

            //    codAccountDetails.ExecuteSync();

            //    M2S_FMS_CODDeliveryCount cODDeliveryCount = new M2S_FMS_CODDeliveryCount();

            //    cODDeliveryCount.ExecuteSync();

            //    M2S_FMS_CODLine codLine = new M2S_FMS_CODLine();

            //    codLine.ExecuteSync();

            //    M2S_FMS_CODLineHistory codLineHistory = new M2S_FMS_CODLineHistory();

            //    codLineHistory.ExecuteSync();

            //    M2S_FMS_CODLineWaitEffect cODLineWaitEffect = new M2S_FMS_CODLineWaitEffect();

            //    cODLineWaitEffect.ExecuteSync();

            //    M2S_FMS_CODOperatorLog codOperatorLog = new M2S_FMS_CODOperatorLog();

            //    codOperatorLog.ExecuteSync();

            //    M2S_FMS_CODReturnsCount codReturnCount = new M2S_FMS_CODReturnsCount();

            //    codReturnCount.ExecuteSync();

            //    M2S_FMS_CodStatsLog codStatsLog = new M2S_FMS_CodStatsLog();

            //    codStatsLog.ExecuteSync();

            //    M2S_FMS_CODVisitReturnsCount codVisitReturnsCount = new M2S_FMS_CODVisitReturnsCount();

            //    codVisitReturnsCount.ExecuteSync();

            //    M2S_FMS_IncomeAccount incomeAccount = new M2S_FMS_IncomeAccount();

            //    incomeAccount.ExecuteSync();

            //    M2S_FMS_IncomeAccountDetail incomeAccountDetail = new M2S_FMS_IncomeAccountDetail();

            //    incomeAccountDetail.ExecuteSync();

            //    M2S_FMS_IncomeDeliveryCount incomeDeliveryCount = new M2S_FMS_IncomeDeliveryCount();

            //    incomeDeliveryCount.ExecuteSync();

            //    M2S_FMS_IncomeOtherFeeCount incomeOtherFeeCount = new M2S_FMS_IncomeOtherFeeCount();

            //    incomeOtherFeeCount.ExecuteSync();

            //    M2S_FMS_IncomeReturnsCount incomeReturnsCount = new M2S_FMS_IncomeReturnsCount();

            //    incomeReturnsCount.ExecuteSync();

            //    M2S_FMS_IncomeStatLog incomeStatLog = new M2S_FMS_IncomeStatLog();

            //    incomeStatLog.ExecuteSync();

            //    M2S_FMS_IncomeVisitReturnsCount incomeVisitReturnsCount = new M2S_FMS_IncomeVisitReturnsCount();

            //    incomeVisitReturnsCount.ExecuteSync();

            //    M2S_FMS_MerchantDeliverFee merchantDeliverFee = new M2S_FMS_MerchantDeliverFee();

            //    merchantDeliverFee.ExecuteSync();

            //    M2S_FMS_OperateLog operateLog = new M2S_FMS_OperateLog();

            //    operateLog.ExecuteSync();

            //    M2S_FMS_SortingTransferDetail sortingTransferDetail = new M2S_FMS_SortingTransferDetail();

            //    sortingTransferDetail.ExecuteSync();

            //    M2S_FMS_StationDeliverFee stationDeliverFee = new M2S_FMS_StationDeliverFee();

            //    stationDeliverFee.ExecuteSync();

            //    M2S_FMS_TypeRelation typeRelation = new M2S_FMS_TypeRelation();

            //    typeRelation.ExecuteSync();

            //    M2S_StatusCodeInfo statusCodeInfo = new M2S_StatusCodeInfo();

            //    statusCodeInfo.ExecuteSync();

            //    M2S_FMS_NoGenerateEx noGenerateEx = new M2S_FMS_NoGenerateEx();

            //    noGenerateEx.ExecuteSync();

            //    M2S_FMSTableColumnDic tableColumnDic = new M2S_FMSTableColumnDic();

            //    tableColumnDic.ExecuteSync();
            //}
        }

        private static void RunSQL(string cons)
        {
            string sql = @"update RFD_FMS.dbo.StatusCodeInfo set IsChange=1 where codetype='AreaTypeAudit' and codeno=1";

            while (true)
            {
                SqlHelper.ExecuteNonQuery(cons, CommandType.Text, sql);

                Thread.Sleep(2000);
            }
        }
    }
}
