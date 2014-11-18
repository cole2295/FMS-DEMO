using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using RFD.FMS.AdoNet;
using RFD.FMS.Util.Context;

namespace RFD.FMS.AdoNet
{
    [Serializable]
    public abstract class SqlServerDao : DaoBase<SqlConnection>
    {
        public override SqlConnection GetConnection(string connstr)
        {
            var connection = NeutralContext.Get(Constants.NcUnitofworkTransactionConnectionThread) as SqlConnection;

            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            else
            {
                connection = new SqlConnection(connstr);
            }

            return connection;
        }

        public virtual SqlParameter[] ToParameters(DbParameter[] parameters)
        {
            SqlParameter[] dbParameters = new SqlParameter[parameters.Length];

            DbParameter temp = null;
            SqlParameter tempSqlServer = null;

            for (int i = 0; i < parameters.Length; i++)
            {
                temp = parameters[i];

                tempSqlServer = new SqlParameter();
                tempSqlServer.ParameterName = temp.ParameterName;
                tempSqlServer.DbType = temp.DbType;
                tempSqlServer.Size = temp.Size;
                tempSqlServer.Value = temp.Value;

                dbParameters[i] = tempSqlServer;
            }

            return dbParameters;
        }

        public virtual SqlParameter[] ToParameters<T>(T parameters) where T : IEnumerable
        {
            List<SqlParameter> list = new List<SqlParameter>();

            DbParameter temp = null;
            SqlParameter tempSqlServer = null;

            foreach (var item in parameters)
            {
                temp = item as DbParameter;

                tempSqlServer = new SqlParameter();
                tempSqlServer.ParameterName = temp.ParameterName;
                tempSqlServer.DbType = temp.DbType;
                tempSqlServer.Size = temp.Size;
                tempSqlServer.Value = temp.Value;

                list.Add(tempSqlServer);
            }

            return list.ToArray();
        }
    }
}