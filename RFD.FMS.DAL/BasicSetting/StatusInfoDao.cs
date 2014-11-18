using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.BasicSetting
{
	/*
 * (C)Copyright 2011-2012 如风达信息管理系统
 * 
 * 模块名称：状态信息查询（数据层）
 * 说明：查询状态信息
 * 作者：高毅勤
 * 创建日期：2011-03-02 12:11:00
 * 修改人：
 * 修改时间：
 * 修改记录：
 */
    public class StatusInfoDao : SqlServerDao, IStatusInfoDao
	{
		/// <summary>
		/// 根据状态类型编号获取所有状态
		/// </summary>
		/// <param name="statusTypeNo"></param>
		/// <returns></returns>
		public DataTable GetStatusInfoByTypeNo(int statusTypeNo)
		{
			string strSql =
                string.Format(@"select statusNo,statusName from RFD_PMS.dbo.StatusInfo (nolock)  where statusTypeNo={0} order by orderby", statusTypeNo);
			DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];
			return dataTable;	
		}

		public string GetStatusNameByTypeCode(int statusTypeNo, string statusNo)
		{
            string sql = string.Format(@"select statusName from RFD_PMS.dbo.StatusInfo (nolock)  where statusTypeNo={0} and StatusNO='{1}'",
			                           statusTypeNo, statusNo);
			object data = SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql);
			return data==null ? "" : data.ToString();
		}
	}
}