using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Domain.QueryStatistics
{
    public interface ICODWaybillFeeCheckDao
    {
        /// <summary>
        /// 发货日统计
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="stopDate">结束时间</param>
        /// <returns></returns>
        DataTable GetCODDeliveryDaily(string startDate, string stopDate);
        /// <summary>
        /// 日发货站点明细
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        /// <returns></returns>
        DataTable GetCODDeliveryDailyByStationID(string startDate, string stopDate, int StationID);
        /// <summary>
        /// 退货日统计(拒收等)
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="stopDate">结束时间</param>
        /// <returns></returns>
        DataTable GetCODReturnsDaily(string startDate, string stopDate);
        
        /// <summary>
        /// 上门退日统计
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="stopDate">结束时间</param>
        /// <returns></returns>
        DataTable GetCODVisitDaily(string startDate, string stopDate);
        /// <summary>
        /// 退货（拒收）明细
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        /// <returns></returns>
        DataTable GetCODReturnsDailyByStationID(string startDate, string stopDate, int StationID);
        /// <summary>
        /// 上门退货明细
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        /// <returns></returns>
        DataTable GetCODVisitDailyByStationID(string startDate, string stopDate,int StationID);
    }
}
