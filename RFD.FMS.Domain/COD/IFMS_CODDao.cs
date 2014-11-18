//2011-10-13 星期四
//马中华

using System;
using System.Data;
using RFD.FMS.MODEL.COD;
using RFD.FMS.MODEL;

namespace RFD.FMS.Domain.COD
{
    public interface IFMS_CODDao
    {
        int AddWaybillLmsToMedium(LMS_Syn_FMS_COD model);

        int AddWaybillLmsToMediumV2(LMS_Syn_FMS_COD model);

        int UpDateMediumForSyn(string id, int isSyn);

        DataTable SearchIdsForShip(string topNumber, string synType);

        DataTable SearchIdsForShipBySynId(string synId);

        DataTable SearchIdsForBack(string topNumber, string synType);

        DataTable SearchIdsForBackAdvanced(string topNumber, string synType);

        DataTable SearchWaybillnosForBack(string topNumber);

        DataTable SearchInfoForShip(string ids);

        DataTable SearchInfoForBack(string ids);

        DataTable SearchInfoForBackAdvanced(string ids);

        int InsertForShip(FMS_CODBaseInfo model);

        int UpdateForBack(FMS_CODBaseInfo model);

        int AdvancedUpdateForBack(FMS_CODBaseInfo model);

        long SearchUpdateMediumId(long waybillno, string waybillType, string backStatus);
        long SearchUpdateMediumIdTemp(long waybillno);

        DataTable SearchAnyInfoOnMedium(string sql);

        int UpdateDelete(string sql);

        bool UpdateForInvalid(long waybillNo);

        DataTable SearchInfoForDeliverTimeAndOutBountStation(string ids);

        DataTable SearchIdsForDeliverTimeAndOutBountStation(string topNumber, string synType);
        int UpdateWaybillForCOD(long waybillno,int issyn);

        string GetStationByIDNOTwo(long id, long waybillno);

        string GetStationByIDNOThird(long id, long waybillno);
    }
}
