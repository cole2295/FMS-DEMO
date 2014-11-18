using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.BasicSetting
{
	/*
 * (C)Copyright 2011-2012 ������Ϣ����ϵͳ
 * 
 * ģ�����ƣ�״̬��Ϣ��ѯ�����ݲ㣩
 * ˵������ѯ״̬��Ϣ
 * ���ߣ�������
 * �������ڣ�2011-03-02 12:11:00
 * �޸��ˣ�
 * �޸�ʱ�䣺
 * �޸ļ�¼��
 */
    public class StatusInfoDao : SqlServerDao, IStatusInfoDao
	{
		/// <summary>
		/// ����״̬���ͱ�Ż�ȡ����״̬
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