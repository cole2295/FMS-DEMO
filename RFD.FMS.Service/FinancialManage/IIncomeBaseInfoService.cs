using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IIncomeBaseInfoService
    {
        /// <summary>
        /// 商家收入日报表统计
        /// </summary>
        /// <param name="beginTime">统计开始时间</param>
        /// <param name="endTime">统计结束时间</param>
        /// <returns>统计的数值</returns>
        DataTable GetIncomeDailyReport(string beginTime, string endTime, string merchantIds,string distributionCode);

        /// <summary>
        /// 商家收入日报表汇总
        /// </summary>
        /// <param name="beginTime">汇总开始时间</param>
        /// <param name="endTime">汇总结束时间</param>
        /// <returns>汇总的数据</returns>
        IDictionary<string, string> GetIncomeDailyReportSum(string beginTime, string endTime, string merchantIds, string distributionCode);

        /// <summary>
        /// 根据查询列表统计
        /// </summary>
        /// <param name="dtDetail"></param>
        /// <returns></returns>
        IDictionary<string, string> GetIncomeDailyReportSum(DataTable dtDetail);

        /// <summary>
        /// 查询已有配送费参数
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <returns>配送费</returns>
        DeliverFeeModel GetDeliverFeeParameter(long waybillNo, string distributionCode);

        /// <summary>
        /// 计算配送费
        /// </summary>
        /// <param name="model">配送费对象</param>
        /// <returns>配送费</returns>
        bool EvalDeliverFee(DeliverFeeModel model);

        /// <summary>
        /// 保存修改后的配送费
        /// </summary>
        /// <param name="model">配送费model</param>
        /// <returns>是否修改成功</returns>
        bool SaveDeliverFee(DeliverFeeModel model);

        /// <summary>
        /// 更新配送费是否已计算标志，重新计算配送费
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <returns>是否成功</returns>
        bool UpdateEvalStatus(long waybillNo);

        /// <summary>
        /// 更新配送费是否已计算标志，重新计算配送费
        /// </summary>
        /// <param name="waybillNo">运单号集合</param>
        /// <returns>是否成功</returns>
        bool UpdateEvalStatus(string waybillNos);

        DataTable GetIncomeDeliveryFeeInfo(DataTable ImportTable, int Type);

        bool ExsitIncomeFeeInfoByNo(long waybillNo, long incomeFeeID);
        DataTable ExsitIncomeFeeInfoByNo(long waybillNO);

        bool BatchSaveDeliverFee(DeliverFeeModel model);
    }
}
