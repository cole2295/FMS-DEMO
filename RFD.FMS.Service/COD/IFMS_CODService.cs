using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.COD;

namespace RFD.FMS.Service.COD
{
    public interface IFMS_CODService
    {        
        /// <summary>
        /// 业务表向中间表写数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddWaybillLmsToMedium(LMS_Syn_FMS_COD model);

        /// <summary>
        /// 更新中间表同步标志位
        /// </summary>
        /// <returns></returns>
        bool UpDateMediumForSyn(string ids, int isSyn);

        DataTable SearchIdsForShip(string topNumber,string synType);

        DataTable SearchIdsForBack(string topNumber, string synType);

        DataTable SearchInfoForShip(string ids);

        DataTable SearchInfoForBack(string ids);

        /// <summary>
        /// 检测是否应该向静态表插入数据
        /// </summary>
        /// <returns></returns>
        bool ShouldInsertForShip(DataRow waybill,out FMS_CODBaseInfo model,out string reason);

        /// <summary>
        /// 检测是否应该更新静态表
        /// </summary>
        /// <returns></returns>
        bool ShouldUpdateForBack(DataRow waybill,out FMS_CODBaseInfo model,out string reason);

        bool InsertForShip(FMS_CODBaseInfo model);

        bool UpdateForBack(FMS_CODBaseInfo model);

        string LmsSynFmsForBackTest(string ids);
        string LmsSynFmsForBack(string topNumber, string synType);
        string LmsSynFmsForShip(string topNumber, string synType);

        string LmsSynFmsForShipBySynId(string synIds);

        ///// <summary>
        ///// 更新FMS的COD基础信息表的末级发货仓、末级发货时间、区域类型等信息
        ///// </summary>
        ///// <returns></returns>
        //string LmsSynFmsForBackAdvanced(string topNumber, string synType);




        //string LmsSynFmsForBackTemp(string topNumber);

        string SearchAnyInfoOnMedium(string sql);

        int UpdateDelete(string sql);


        string SynForDeliverTimeAndOutBountStation();

        string SynForDeliverTimeAndOutBountStationForTest(string ids);

        bool AdvancedUpdateForBack(FMS_CODBaseInfo model);
    }
}
