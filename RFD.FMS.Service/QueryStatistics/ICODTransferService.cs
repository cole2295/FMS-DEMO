using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.QueryStatistics
{
    public interface ICODTransferService
    {
        /// <summary>
        /// 交接明细
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pi"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        DataTable SearchCodDetailsV2(CodSearchCondition condition, ref PageInfo pi, out DataTable amount);

        /// <summary>
        /// 交接明细
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pi"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        DataTable SearchCodDetails(CodSearchCondition condition, ref PageInfo pi, out DataTable amount);

        /// <summary>
        /// 交接汇总
        /// </summary>
        /// <returns></returns>
        DataTable SearchCodStatV2(CodSearchCondition condition, out DataTable amount);

        /// <summary>
        /// 交接汇总
        /// </summary>
        /// <returns></returns>
        DataTable SearchCodStat(CodSearchCondition condition, out DataTable amount);

        DataTable SearchExprotDetailDataV2(CodSearchCondition condition);

        DataTable SearchExprotDetailData(CodSearchCondition condition);

        /// <summary>
        /// 导出数据查询
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        DataTable SearchExprotStatData(CodSearchCondition condition);

        /// <summary>
        /// 导出数据查询
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        DataTable SearchExprotStatDataV2(CodSearchCondition condition);
    }
}
