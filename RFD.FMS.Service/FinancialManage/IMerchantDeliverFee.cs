using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IMerchantDeliverFee
    {
        void MapAreaExpressLevelIncome(FMS_StationDeliverFee info, ref FMS_StationDeliverFee newInfo);

        bool BatchAddDeliverFee(List<FMS_StationDeliverFee> feeList, out string msg);

        string AddDeliverFee(FMS_StationDeliverFee stationDeliverFee,out int id);
        bool DeleteDeliverFee(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> checkList, int updateBy);
        RFD.FMS.MODEL.FMS_StationDeliverFee GetDeliverFeeById(int id);
        System.Data.DataTable GetDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, bool isWait, string categoryCode,ref PageInfo pi);
        bool ImportFee(System.Data.DataTable dt, int createBy, out System.Data.DataTable dtError, string distributionCode);
        string UpdateDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee stationDeliverFee);
        bool UpdateDeliverFeeStatus(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> checkList, int auditBy, int status);

        RFD.FMS.MODEL.FMS_StationDeliverFee GetWaitDeliverFeeById(string id);
        string AddWaitDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee stationDeliverFee);
        string UpdateWaitDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee stationDeliverFee);
        bool UpdateWaitDeliverFeeStatus(IList<KeyValuePair<string, string>> checkList, int auditBy, int status);
        int GetWaitDeliverFeeyFeeId(int feeid);

        //服务
        DataTable GetWaitFeeList();
        bool UpdateToEffect(DataRow dr);
        bool DeleteWaitStationDeliverFee(string effectKid);
        System.Data.DataTable GetExportDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, bool isWait, string categoryCode);
    }
}
