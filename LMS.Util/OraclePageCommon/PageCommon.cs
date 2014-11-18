using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.ApplicationBlocks.Data;

namespace RFD.FMS.Util.OraclePageCommon
{
	public class PageCommon
	{
		private const string CommonPagingSql =
					@"with {4} as( SELECT ROWNUM AS ROW_NO,a.*
   FROM ( {0} {1})  {5} )  select * from {4} where ROW_NO between {2} and {3}";
		private const string commCountsql = "select count(1) from ({0}) a ";

		/// <summary>
		/// 通用获取分页数据
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="sql"></param>
		/// <param name="orderby"></param>
		/// <param name="paginator"></param>
		/// <returns></returns>
		public static IPagedDataTable GetPagedData(OracleConnection connection, string sql, string orderby, PaginatorDTO paginator, params OracleParameter[] commandParameters)
		{
			var pageTable = new PagedDataTable();
			//取得总行数和
			var countsql = string.Format(commCountsql, sql);
			object count = null;
			using(var con = new OracleConnection(connection.ConnectionString))
			{
				count = OracleHelper.ExecuteScalar(con, CommandType.Text, countsql, commandParameters);
			}

			OracleParameter[] commandParametersNew = ToParameters(commandParameters);
			if (count != null)
				pageTable.RecordCount = Convert.ToInt32(count);
			if (pageTable.RecordCount == 0)
			{
				pageTable.PageCount = 0;
				pageTable.ContentData = null;
				return pageTable;
			}
			//总页数
			pageTable.PageCount = Convert.ToInt32(Math.Ceiling(pageTable.RecordCount * 1.0 / paginator.PageSize));
			var startRow = (paginator.PageNo - 1) * paginator.PageSize + 1;
            var orderbyStr = string.IsNullOrEmpty(orderby) ? "" : " Order by " + orderby;
            var queryText = string.Format(CommonPagingSql, sql, orderbyStr, startRow,
				startRow + paginator.PageSize - 1, "t", "a");
			using (var con = new OracleConnection(connection.ConnectionString))
			{
				pageTable.ContentData = OracleHelper.ExecuteDataset(connection, CommandType.Text, queryText, commandParametersNew).Tables[0];
			}
			
			return pageTable;
		}

        /// <summary>
        /// 通用获取分页数据--查询总数SQL分离
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pageSql">明细SQL</param>
        /// <param name="countSql">统计SQL</param>
        /// <param name="orderby"></param>
        /// <param name="paginator"></param>
        /// <returns></returns>
        public static IPagedDataTable GetPagedData(OracleConnection connection, string pageSql, string countSql, string orderby, PaginatorDTO paginator, params OracleParameter[] commandParameters)
        {
            var pageTable = new PagedDataTable();
            //取得总行数和
            object count = null;
            
            using (var con = new OracleConnection(connection.ConnectionString))
            {
                count = OracleHelper.ExecuteScalar(con, CommandType.Text, countSql, commandParameters);
            }

            
            if (count != null)
                pageTable.RecordCount = Convert.ToInt32(count);
            if (pageTable.RecordCount == 0)
            {
                pageTable.PageCount = 0;
                pageTable.ContentData = null;
                return pageTable;
            }
            //总页数
            pageTable.PageCount = Convert.ToInt32(Math.Ceiling(pageTable.RecordCount * 1.0 / paginator.PageSize));
            var startRow = (paginator.PageNo - 1) * paginator.PageSize + 1;
            var orderbyStr = string.IsNullOrEmpty(orderby) ? "" : " Order by " + orderby;
            var queryText = string.Format(CommonPagingSql, pageSql, orderbyStr, startRow,
                startRow + paginator.PageSize - 1, "t", "a");

            OracleParameter[] commandParametersNew = ToParameters(commandParameters);
            using (var con = new OracleConnection(connection.ConnectionString))
            {
                pageTable.ContentData = OracleHelper.ExecuteDataset(connection, CommandType.Text, queryText, commandParametersNew).Tables[0];
            }

            return pageTable;
        }

        /// <summary>
        /// 通用获取分页数据--知道总数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="sql">查询SQL</param>
        /// <param name="orderby">排序</param>
        /// <param name="paginator"></param>
        /// <param name="commandParameters">查询参数</param>
        /// <param name="allCount">记录总数</param>
        /// <returns></returns>
        public static IPagedDataTable GetPagedDataByAllCount(OracleConnection connection, string sql, string orderby, PaginatorDTO paginator, params OracleParameter[] commandParameters)
        {
            var pageTable = new PagedDataTable();
            if (paginator.ItemCount <= 0)
                return null;

            OracleParameter[] commandParametersNew = { };
            if(commandParameters!=null)
                commandParametersNew = ToParameters(commandParameters);
            
            //总页数
            pageTable.PageCount = Convert.ToInt32(Math.Ceiling(paginator.ItemCount * 1.0 / paginator.PageSize));
            var startRow = (paginator.PageNo - 1) * paginator.PageSize + 1;
            var orderbyStr = string.IsNullOrEmpty(orderby) ? "" : " Order by " + orderby;
            var queryText = string.Format(CommonPagingSql, sql, orderbyStr, startRow,
                startRow + paginator.PageSize - 1, "t", "a");
            using (var con = new OracleConnection(connection.ConnectionString))
            {
                if (commandParametersNew != null)
                    pageTable.ContentData = OracleHelper.ExecuteDataset(connection, CommandType.Text, queryText, commandParametersNew).Tables[0];
                else
                    pageTable.ContentData = OracleHelper.ExecuteDataset(connection, CommandType.Text, queryText).Tables[0];
            }

            return pageTable;
        }

		public static OracleParameter[] ToParameters(DbParameter[] parameters)
		{
			OracleParameter[] dbParameters = new OracleParameter[parameters.Length];

			DbParameter temp = null;
			OracleParameter tempOracle = null;

			for (int i = 0; i < parameters.Length; i++)
			{
				temp = parameters[i];

				tempOracle = new OracleParameter();
				tempOracle.ParameterName = temp.ParameterName;
				tempOracle.DbType = temp.DbType;
				tempOracle.Size = temp.Size;
				tempOracle.Value = temp.Value;

				dbParameters[i] = tempOracle;
			}

			return dbParameters;
		}
	}
}
