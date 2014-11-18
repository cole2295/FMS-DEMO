using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.Oracle.BasicSetting
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
    public class StatusInfoDao : OracleDao, IStatusInfoDao
	{
		/// <summary>
		/// ����״̬���ͱ�Ż�ȡ����״̬
		/// </summary>
		/// <param name="statusTypeNo"></param>
		/// <returns></returns>
		public DataTable GetStatusInfoByTypeNo(int statusTypeNo)
		{
			string strSql =
                string.Format(@"select statusNo,statusName from StatusInfo   where statusTypeNo={0} order by orderby", statusTypeNo);
			DataTable dataTable = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];
			return dataTable;	
		}

		public string GetStatusNameByTypeCode(int statusTypeNo, string statusNo)
		{
            string sql = string.Format(@"select statusName from StatusInfo   where statusTypeNo={0} and StatusNO='{1}'",
			                           statusTypeNo, statusNo);
			object data = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql);
			return data==null ? "" : data.ToString();
		}
	}
}