using System;
namespace RFD.FMS.Service.COD
{
    public interface ICODAccountService
    {
        System.Data.DataTable AccountSearchByNo(string accountNo, bool flag, out RFD.FMS.MODEL.CODSearchCondition searchCondition);
        bool DeleteAccountNo(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> keyValuePairs, string updateBy);
        System.Data.DataTable DisposeDataRowValue(System.Data.DataTable dt);
        System.Data.DataTable GetDifferenceData(RFD.FMS.MODEL.CODSearchCondition condition);
        System.Data.DataTable GetErrorLog(RFD.FMS.MODEL.CODSearchCondition condition);
        RFD.FMS.MODEL.CODSearchCondition GetSearchConditionByNo(string accountNo, bool flag);
        bool JudgeWareHouseContains(string wareHouses);
        bool SaveAccount(RFD.FMS.MODEL.CODSearchCondition searchCondition, System.Data.DataTable AccountDetail, string createBy, out string AccountNo);
        System.Data.DataTable SearchAccount(string auditStatus, string expressCompanyId, string accountDateS, string accountDateE, string accountNo, string merchantId, ref RFD.FMS.MODEL.PageInfo pi, bool isPage);
        RFD.FMS.MODEL.CODAccountDetail SearchAccountDetail(string accountDetailId);
        System.Data.DataTable SearchAreaFareByNo(string accountNo, out RFD.FMS.MODEL.CODSearchCondition searchCondition);
        System.Data.DataTable SearchDetail(string searchType, RFD.FMS.MODEL.CODSearchCondition searchCondition, ref RFD.FMS.MODEL.PageInfo pi);
        System.Data.DataTable SearchExportDetail(string exportType, RFD.FMS.MODEL.CODSearchCondition searchCondition);
        System.Data.DataTable SearchUniteAccount(RFD.FMS.MODEL.CODSearchCondition condition);
        bool UpdateAccountAuditStatus(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> keyValuePairs, int auditStatus, string updateBy);
        bool UpdateAccountFee(RFD.FMS.MODEL.CODAccountDetail accountDetail, string updateBy);
    }
}
