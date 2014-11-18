using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.QueryStatistics;

namespace RFD.FMS.Service.QueryStatistics
{
    public interface ICODWaybillFeeCheckService
    {
        /// <summary>
        /// 发货日统计
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="stopDate">结束时间</param>
        /// <returns></returns>
        IDictionary<int,FMSCodDaily> GetCODDeliveryDaily(string startDate, string stopDate);
        /// <summary>
        /// 发货明细
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        /// <returns></returns>
        IDictionary<int, FMSCODDetails> GetCODDeliveryDailyByStationID(string startDate, string stopDate, int StationID);
        /// <summary>
        /// 退货明细
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        /// <returns></returns>
        IDictionary<int, FMSCODDetails> GetCODReturnsDailyByStationID(string startDate, string stopDate, int StationID);
        /// <summary>
        /// 退货日统计(拒收等)
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="stopDate">结束时间</param>
        /// <returns></returns>
        IDictionary<int,FMSCodDaily> GetCODReturnsDaily(string startDate, string stopDate);

        /// <summary>
        /// 上门退日统计
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="stopDate">结束时间</param>
        /// <returns></returns>
        IDictionary<int, FMSCodDaily> GetCODVisitDaily(string startDate, string stopDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        /// <returns></returns>
        IDictionary<int, FMSCODDetails> GetCODVisitDailyByStationID(string startDate, string stopDate,int StationID);
    }
}
