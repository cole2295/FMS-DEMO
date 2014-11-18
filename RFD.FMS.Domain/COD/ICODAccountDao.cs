using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL;
using System.Data;

namespace RFD.FMS.Domain.COD
{
    public interface ICODAccountDao
    {
        bool AddAccount(RFD.FMS.MODEL.CODSearchCondition searchCondition, string createBy, string accountNo);
        bool AddAccountDetail(System.Data.DataRow dr, string createBy, string accountNo);
        DataTable GetChanageCountList(CODSearchCondition searchCondition);
        bool ChanageCountAcountNO(RFD.FMS.MODEL.CODSearchCondition searchCondition, string createBy, string accountNo);
        bool BatchChangeCountAccountNO(string accountIds, string createBy, string AccountNo, string tableName);
        bool DeleteAccountNo(string accountNo, string updateBy);
        DataTable GetAreaFare(string accountNo);
        DataTable GetCompanyIdByRfd();
        DataTable GetCompanyIdByTopCODCompanyID(string topCodCompanyId);
        DataTable GetDetail(string searchType, CODSearchCondition searchCondition, ref PageInfo pi);
        DataTable GetErrorLog(RFD.FMS.MODEL.CODSearchCondition condition);
        DataTable GetExportDetail(string searchType, CODSearchCondition searchCondition,bool isDifference);
        DataTable SearchAccount(string auditStatus, string expressCompanyId, string accountDateS, string accountDateE, string accountNo, string merchantId, RFD.FMS.MODEL.PageInfo pi, bool isPage);
        DataTable SearchAccountCondition(string accountNo, bool flag);
        DataTable SearchAccountDetail(string accountNo, string dataType, bool flag);
        DataTable SearchUniteAccount(RFD.FMS.MODEL.CODSearchCondition condition);
        bool UpdateAccountAuditStatus(string accountNo, int auditStatus, string updateBy);
        bool UpdateAccountDetailFee(RFD.FMS.MODEL.CODAccountDetail accountDetail, string updateBy);
    }
}
