using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IFMS_MerchantDeliverFeeDao
    {
        int AddMerchantDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee model);
        DataTable GetAllMerchantList();
        DataTable GetMerchantDeliverFeeHistory(SearchCondition condition);

        int GetMerchantDeliverFeeStat(SearchCondition condition);
        DataTable GetMerchantDeliverFeeList(SearchCondition condition, PageInfo pi);
        System.Collections.Generic.Dictionary<RFD.FMS.MODEL.Enumeration.SettleAccountType, int> GetProxyCollectWeeklyData(string MerchantID);
        System.Data.DataTable SearchStationDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee model);
        bool UpdateMerchantDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee model);
        bool UpdateMerchantDeliverFeeStatus(int merchantId, int status, int auditBy, string distributionCode, out int id);

        int GetMerchantDeliverFeeEffectStat(SearchCondition condition);
        System.Data.DataTable GetMerchantDeliverFeeEffectList(SearchCondition condition,PageInfo pi);
        int AddEffectMerchantDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee merchant);
        bool UpdateEffectMerchantDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee merchant);
        bool UpdateEffectMerchantDeliverFeeStatus(int merchantId, int status, int auditBy, string distributionCode, out int id);
        int GetEffectMerchantDeliverByMerchantID(int merchantId);

        DataTable GetWaitFeeList();
        bool UpdateToEffect(FMS_MerchantDeliverFee model);
        bool DeleteWaitMerchantDeliverFee(string feeid);
    }
}
