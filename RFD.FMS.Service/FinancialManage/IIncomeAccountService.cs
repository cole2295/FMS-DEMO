using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IIncomeAccountService
    {
        int AddIncomeDeliveryAccount(RFD.FMS.MODEL.FinancialManage.IncomeDeliveryCount model, RFD.FMS.MODEL.FinancialManage.IncomeStatLog modelLog);
        bool AddIncomeDeliveryDetails(int merchantId, DateTime countDate, int deliverstationid, int expresscompanyid);
        bool AddIncomeOtherReceiveFee(int merchantId, DateTime countDate, int deliverStationId);
        bool AddIncomeReturnsAccountDetails(int merchantId, DateTime countDate, int deliverStationId, int returnExpressId);
        int AddIncomeStatLog(RFD.FMS.MODEL.FinancialManage.IncomeStatLog model);
        bool AddIncomeVisitReturnsAccountDetails(int merchantId, DateTime countDate, int deliverStationId, int returnExpressId);
        void BuildAccountDetail(ref System.Collections.Generic.List<RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail> list);
        int CollateIncomeDeliveryAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        int CollateIncomeOtherReceiveFee(int merchantid, DateTime accountDate, int deliverstationid);
        int CollateIncomeReturnsAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        int CollateIncomeVisitReturnsAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        bool CreateIncomeAccount(RFD.FMS.MODEL.FinancialManage.IncomeSearchCondition condition, System.Collections.Generic.List<RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail> list, int createBy, out string AccountNo);
        bool DeleteAccount(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> checkList, int updateBy);
        System.Data.DataTable GetAccountAreaSummary(string accountNo);
        RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail GetAccountDetail(string detailId);
        RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail GetAccountDetailByAccountNo(string accountNo);
        System.Collections.Generic.List<RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail> GetAccountDetails(string accountNo);
        System.Collections.Generic.List<RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail> GetAccountDetailsNew(string accountNo);
        System.Data.DataTable GetAccountList(string accountStatus, string merchantId, string dateStr, string dateEnd, string accountNo);
        RFD.FMS.MODEL.FinancialManage.IncomeSearchCondition GetAccountSearchCondition(string accountNo);
        System.Data.DataTable GetIncomeDeliveryAccount(int merchantId, DateTime accountDate, int deliverstationid, int expresscompanyid);
        System.Data.DataTable GetIncomeDeliveryAccountInfo(int merchantId, DateTime accountDate);
        System.Data.DataTable GetIncomeHistoryCount(DateTime accountDate);
        System.Data.DataTable GetIncomeOtherReceiveFee(int merchantId, DateTime accountDate);
        System.Data.DataTable GetIncomeReturnsCount(int merchantId, DateTime accountDate);
        System.Data.DataTable GetIncomeVisitReturnsCount(int merchantId, DateTime accountDate);
        System.Data.DataTable GetMerchantInfo(string distributionCode);
        System.Collections.Generic.List<RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail> GetUniteAccount(RFD.FMS.MODEL.FinancialManage.IncomeSearchCondition IncomeSearchCondition);
        int IncomeDeliveryAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        int IncomeOtherReceiveFeeHistory(int merchantid, DateTime accountDate, int deliverstationid);
        int IncomeReturnsAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        int IncomeVisitReturnsAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid);
        int IsIncomeHistoryCount(DateTime accountDate);
        bool UpdateAccountFee(RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail i);
        bool UpdateAccountFeeByAccountNo(RFD.FMS.MODEL.FinancialManage.IncomeAccountDetail i);
        bool UpdateAccountStatus(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> list, int auditBy, int status);
        bool UpdateIncomeStatLog(RFD.FMS.MODEL.FinancialManage.IncomeStatLog model);
    }
}
