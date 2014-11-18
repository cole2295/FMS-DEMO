using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.Model.FinancialManage;

namespace RFD.FMS.Domain.FinancialManage
{
	public partial interface IIncomeFeeInfoDao
	{
		/// <summary>
        /// 是否存在该记录
        /// </summary>
		bool Exists(Int64 waybillno);
		
		/// <summary>
		/// 增加一条数据
		/// </summary>
		int Add(FMS_IncomeFeeInfo model);
		
		/// <summary>
		/// 更新一条数据
		/// </summary>
		bool Update(FMS_IncomeFeeInfo model);
		
        /// <summary>
		/// 归班更新一条数据
		/// </summary>
        bool UpdateByBackStation(FMS_IncomeFeeInfo model);

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		FMS_IncomeFeeInfo GetModel(Int64 waybillno);
		
		
		/// <summary>
		/// 根据条件得到一个对象实体集
		/// </summary>
		List<FMS_IncomeFeeInfo> GetModelList(Dictionary<string, object> searchParams);
		
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
	    /// 查询出未计算费用的
	    /// </summary>
	    /// <param name="TopNum"></param>
	    /// <returns></returns>
	    DataTable GetInComeFeeInfo(int TopNum);

	    /// <summary>
	    /// 查询出未计算费用的订单
	    /// </summary>
	    DataTable GetInComeFeeInfo(string WaybillNOList);

        DataTable GetAccountError9();

        DataTable GetAccountError45(int errorType);

        DataTable GetAccountError3();

        DataTable GetClearDatalist();

        bool UpdateIncomeFeeIsAccount(Int64 incomeFeeId);

        DataTable GetHistoryInComeFeeInfoDeliver(int topNum);

        DataTable GetHistoryInComeFeeInfoReturn(int topNum);

        DataTable GetHistoryInComeFeeInfoVisit(int topNum);

        bool UpdateEvalStatusByIncomeFeeID(List<long> incomeFeeIDs);
        DataTable GetIncomeDeliveryFeeInfo(List<long> incomeFeeIDs);

        bool ExsitIncomeFeeInfoByNo(long waybillNo, long incomeFeeID);

        DataTable ExsitIncomeFeeInfoByNo(long waybillNO);
    }
}
