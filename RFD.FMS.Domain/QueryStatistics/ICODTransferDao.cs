using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Domain.QueryStatistics
{
    public interface ICODTransferDao
    {
        /// <summary>
        /// COD查询统计
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        DataTable StatCodV2(string condition, CodSearchCondition searchCondition);

        /// <summary>
        /// COD查询统计
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        DataTable StatCod(string condition, CodSearchCondition searchCondition);

        /// <summary>
        /// COD查询明细
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pi"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        DataTable SearchCodDetailsV2(string condition, ref PageInfo pi, CodSearchCondition searchCondition);

        /// <summary>
        /// COD查询明细
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pi"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        DataTable SearchCodDetails(string condition, ref PageInfo pi, CodSearchCondition searchCondition);

        DataTable SearchCodStatV2(string condition, CodSearchCondition searchCondition);

        DataTable SearchCodStat(string condition, CodSearchCondition searchCondition);

        DataTable SearchExprotDetailDataV2(string condition, CodSearchCondition searchCondition);

        DataTable SearchExprotDetailData(string condition, CodSearchCondition searchCondition);

        DataTable SearchExprotStatDataV2(string condition, CodSearchCondition searchCondition);

        DataTable SearchExprotStatData(string condition, CodSearchCondition searchCondition);
    }
}
