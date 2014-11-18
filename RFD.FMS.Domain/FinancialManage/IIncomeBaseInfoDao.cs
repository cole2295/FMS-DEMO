using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Domain.FinancialManage
{
	public interface IIncomeBaseInfoDao
	{
		/// <summary>
        /// 是否存在该记录
        /// </summary>
		bool Exists(Int64 incomeid);
		
		/// <summary>
		/// 增加一条数据
		/// </summary>
		int Add(FMS_IncomeBaseInfo model);
		
		/// <summary>
		/// 更新一条数据
		/// </summary>
		bool Update(FMS_IncomeBaseInfo model);

	    /// <summary>
	    /// 更新一条数据
	    /// </summary>
	    bool UpdateStatus(FMS_IncomeBaseInfo model);
        /// <summary>
        /// 更新一条数据
        /// </summary>
        bool UpdateBackStatus(FMS_IncomeBaseInfo model);
		
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		FMS_IncomeBaseInfo GetModel(Int64 incomeid);
		
        /// <summary>
		/// 得到一个对象实体 根据waybillNo
		/// </summary>
        FMS_IncomeBaseInfo GetModelByWaybillNO(Int64 waybillNo);
		
		/// <summary>
		/// 根据条件得到一个对象实体集
		/// </summary>
		List<FMS_IncomeBaseInfo> GetModelList(Dictionary<string, object> searchParams);
		
		/// <summary>
        /// 获取指定条件的结果集行数
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
		int GetDataTableCount(string searchString,Dictionary<string, object> searchParams);
		
		/// <summary>
        /// 获取指定条件结果集
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
		DataTable GetDataTable(Dictionary<string, object> searchParams);
		
		/// <summary>
        /// 根据指定条件指定排序获取结果集
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortColumn"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
		DataTable GetDataTable(string searchString,string sortColumn,Dictionary<string, object> searchParams);
		
		/// <summary>
        /// 根据指定条件指定排序获取结果集带分页
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortColumn"></param>
        /// <param name="searchParams"></param>
        /// <param name="rowStart"></param>
        /// <param name="rowEnd"></param>
        /// <returns></returns>
		DataTable GetPageDataTable(string searchString,string sortColumn,Dictionary<string, object> searchParams, int rowStart, int rowEnd);

	    /// <summary>
	    /// 通过运单号查询收入结算所需信息 add by wangyongc 2012-04-12
	    /// </summary>
	    /// <param name="waybillNO">运单号</param>
	    /// <returns></returns>
	    DataTable GetWaybillInfoByNO(long waybillNO);

        /// <summary>
        /// 更新金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateAmount(FMS_IncomeBaseInfo info);

        /// <summary>
        /// 更新无效状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateInefficacyStatus(Int64 waybillNo, int inefficacyStatus);

        /// <summary>
        /// 商家收入日报表统计
        /// </summary>
        /// <param name="beginTime">统计开始时间</param>
        /// <param name="endTime">统计结束时间</param>
        /// <returns>统计的数值</returns>
        DataTable GetIncomeDailyReport(string beginTime, string endTime, string merchantIds, string distributionCode);

        /// <summary>
        /// 商家收入日报表汇总
        /// </summary>
        /// <param name="beginTime">汇总开始时间</param>
        /// <param name="endTime">汇总结束时间</param>
        /// <returns>汇总的数据</returns>
        DataTable GetIncomeDailyReportSum(string beginTime, string endTime, string merchantIds, string distributionCode);

        /// <summary>
        /// 查询运单配送费计算参数
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <returns>参数</returns>
        DataTable GetDeliverFeeParameter(long waybillNo, string distributionCode);

        /// <summary>
        /// 保存修改的配送费信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
	}
}
