using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IIncomeAccountDao
    {
        bool AddAccount(RFD.FMS.MODEL.FinancialManage.IncomeSearchCondition condition, int createBy, string accountNo);
        bool AddAccountDetail(RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail m, int createBy, string accountNo);
        int AddIncomeDeliveryAccount(RFD.FMS.MODEL.FinancialManage.IncomeDeliveryCount model);
        int AddIncomeOtherFeeCount(RFD.FMS.MODEL.FinancialManage.IncomeOtherFeeCount model);
        int AddIncomeReturnsCountInfo(RFD.FMS.MODEL.FinancialManage.IncomeReturnsCount model);
        int AddIncomeStatLog(RFD.FMS.MODEL.FinancialManage.IncomeStatLog model);
        int AddIncomeVisitReturnsCountInfo(RFD.FMS.MODEL.FinancialManage.IncomeVisitReturnsCount model);
        bool ChanageCountAcountNO(RFD.FMS.MODEL.FinancialManage.IncomeSearchCondition condition, int createBy, string accountNo);
        int CollateIncomeDeliveryAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        int CollateIncomeOtherReceiveFee(int merchantid, DateTime accountDate, int deliverstationid);
        int CollateIncomeReturnsAccountInfo(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid);
        int CollateIncomeVisitReturnsAccountInfo(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid);
        bool DeleteAccount(string accountNo, int updateBy);
        bool DeleteCount(string accountNo, int updateBy);
        System.Data.DataTable GetAccountAreaSummary(string accountNo);
        System.Data.DataTable GetAccountDetail(string accountNo, string detailId);
        System.Data.DataTable GetAccountDetailNew(string accountNo,string dateType);
        System.Data.DataTable GetAccountList(string accountStatus, string merchantId, string dateStr, string dateEnd, string accountNo);
        System.Data.DataTable GetAccountSearchCondition(string accountNo);
        System.Data.DataTable GetIncomeDeliveryAccount(int merchantId, DateTime accountDate, int deliverstationid, int expresscompanyid);
        System.Data.DataTable GetIncomeDeliveryAccountDetails(int merchantid, DateTime countDate);
        System.Data.DataTable GetIncomeDeliveryAccountInfo(int merchantId, DateTime accountDate);
        System.Data.DataTable GetIncomeHistoryCount(DateTime countdate);
        System.Data.DataTable GetIncomeOtherReceiveFee(int merchantid, DateTime accountdate);
        System.Data.DataTable GetIncomeOtherReceiveFeeDetails(int merchantId, DateTime accountDate, int deliverStationId);
        System.Data.DataTable GetIncomeReturnsAccount(int merchantId, DateTime accountDate, int deliverStationId, int returnExpressId);
        System.Data.DataTable GetIncomeReturnsCount(int merchantid, DateTime accountdate);
        System.Data.DataTable GetIncomeVisitReturnsAccount(int merchantId, DateTime accountDate, int deliverStationId, int returnExpressId);
        System.Data.DataTable GetIncomeVisitReturnsCount(int merchantid, DateTime accountdate);
        System.Data.DataTable GetMerchantInfo(string distributionCode);
        System.Data.DataTable GetUniteAccount(RFD.FMS.MODEL.FinancialManage.IncomeSearchCondition condition);
        int IncomeDeliveryAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        int IncomeOtherReceiveFeeHistory(int merchantid, DateTime accountDate, int deliverstationid);
        int IncomeReturnsAccountHistory(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid);
        int IncomeVisitReturnsAccountHistory(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid);
        int IsIncomeHistoryCount(DateTime countdate);
        string SqlStr { get; set; }
        bool UpdateAccountDetailFee(RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail i);
        bool UpdateAccountDetailFeeAll(RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail i);
        bool UpdateAccountDetailFeeAllByAccountNo(RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail i);
        bool UpdateAccountStatus(string accountNo, int auditBy, int status);
        bool UpdateIncomeStatLog(RFD.FMS.MODEL.FinancialManage.IncomeStatLog model);
        /// <summary>
        /// 是否还有未对接过的账单
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>true代表还有未对接的账单，false代表没有对接的账单</returns>
        bool IsAccessGetIncomeAccount(int MerchantID, DateTime benginDate, DateTime endDate);

        /// <summary>
        /// 根据商家id和记账日期返回账单明细
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        DataTable GetIncomeAccountDetail(int MerchantID, DateTime benginDate, DateTime endDate);
        /// <summary>
        /// 根据商家id和记账日期返回账单总表
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        DataTable GetUnPushedIncomeAccountAndDetail(int MerchantID, DateTime benginDate, DateTime endDate);
        /// <summary>
        /// 根据商家id 开始时间结束时间查询已审核未推送的商家结算单统计的时间
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>查询结果</returns>
        DataTable GetAccountSearchCondition(int MerchantID, DateTime benginDate, DateTime endDate);
    }
}
