using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.Message;
using RFD.Sync.AdoNet;

namespace RFD.Sync.Impl.Dao
{
    public class SyncDao
    {
        public DataSet GetDataSet(string connstr, string sql)
        {
            var cons = DbDES.Decrypt3DES(connstr, Encoding.UTF8);

            DataSet ds = SqlHelper.ExecuteDataset(cons, CommandType.Text, sql);

            return ds;
        }


        public bool Execute(string connstr, string sql, SqlParameter[] parms)
        {
            try
            {
                var cons = DbDES.Decrypt3DES(connstr, Encoding.UTF8);

                int i = SqlHelper.ExecuteNonQuery(cons, CommandType.Text, sql, parms);

                return i > 0;
            }
            catch (Exception ex)
            {
                MessageCollector.Instance.Collect("SQL脚本执行异常", ex.ToString(), true);
            }

            return false;
        }
    }
}
