using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IMerchantDeliverFeeDao
    {
        bool AddStationDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee model, out int id);
        bool DeleteDeliverFee(int id, int updateBy);
        System.Data.DataTable GetDeliverFeeById(int id);
        int GetDeliverFeeStat(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode);
        DataTable GetDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode,PageInfo pi);
        System.Data.DataSet GetExportData();
        string SqlStr { get; set; }
        bool UpdateDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee model);
        bool UpdateDeliverFeeStatus(int id, int status, int auditBy);

        int GetDeliverFeeWaitStat(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode);
        System.Data.DataTable GetDeliverFeeWaitList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode, PageInfo pi);
        System.Data.DataTable GetWaitDeliverFeeById(string id);
        bool AddWaitStationDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee model, out int id);
        bool UpdateWaitDeliverFee(RFD.FMS.MODEL.FMS_StationDeliverFee model);
        bool UpdateWaitDeliverFeeStatus(string id, int status, int auditBy);
        int GetWaitDeliverFeeyFeeId(int feeid);

        //生效服务
        DataTable GetDeliverFeeEffect();

        bool UpdateToEffect(FMS_StationDeliverFee model);

        bool DeleteWaitStationDeliverFee(string effectKid);

        DataTable GetBasicDeliverFeeByCondition(int merchantId, string warehouseId, int areaType, int isCategory, string waybillCategory, string distributionCode);
        DataTable GetExportDeliverFeeWaitList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode);
        DataTable GetExportDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode);
    }
}
