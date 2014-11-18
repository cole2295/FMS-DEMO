using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IDeliverFeeService
    {
        DataTable BindDeliverFeeList(RFD.FMS.MODEL.BasicSetting.SearchCondition condition,ref PageInfo pi);
        DataTable BindDeliverFeeListHistory(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        void BindFactors(System.Web.UI.WebControls.ListControl bindControl);
        void BindStatus(System.Web.UI.WebControls.ListControl bindControl, RFD.FMS.MODEL.Enumeration.MaintainStatus status);
        void BindStatus(System.Web.UI.WebControls.ListControl bindControl, RFD.FMS.MODEL.Enumeration.MaintainStatus status, bool IsAddAllOption, bool IsDefaultSelected);
        bool CheckImportData(System.Data.DataTable uploadData, System.Data.DataTable template, out string error);
        System.Collections.Generic.IDictionary<int, string> GetMerchantDictionary();
        string GetMerchantNameFromDictionary(int merchantID);
        bool SaveDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee merchant);
        System.Data.DataTable SearchStationDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee model);
        bool UpdateDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee merchant);
        bool UpdateMerchantDeliverFeeStatus(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> checkList, int status, int auditBy, string distributionCode);

        void DealDataTable(DataRow dr);

        bool SaveEffectDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee merchant);
        bool UpdateEffectDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee merchant);
        bool UpdateEffectMerchantDeliverFeeStatus(IList<System.Collections.Generic.KeyValuePair<string, string>> checkList, int status, int auditBy, string distributionCode);
        int GetEffectMerchantDeliverByMerchantID(int merchantId);

        //生效服务
        DataTable GetWaitFeeList();
        bool UpdateToEffect(DataRow dr);
        bool DeleteWaitMerchantDeliverFee(string feeid);
    }
}
