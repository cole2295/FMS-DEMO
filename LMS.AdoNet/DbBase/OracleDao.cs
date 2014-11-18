using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.Util;
using RFD.FMS.Util.Security;
using Oracle.DataAccess.Client;
using RFD.FMS.Util.Context;
using Oracle.DataAccess.Types;

namespace RFD.FMS.AdoNet
{
    public abstract class OracleDao : DaoBase<OracleConnection>
    {
        ///<summary>
        /// RFD主库连接字符串
        ///</summary>
        public override string ConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["OracleExecuteConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// RFD只读连接字符串
        ///</summary>
        public override string ReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["OracleRedOnlyConnString"].ToString().Trim()); }
        }

        public override OracleConnection GetConnection(string connstr)
        {
            OracleConnection connection = NeutralContext.Get(RFD.FMS.AdoNet.Constants.NcUnitofworkTransactionConnectionThread) as OracleConnection;

            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            else
            {
                connection = new OracleConnection(connstr);
            }

            return connection;
        }

		public virtual int GetIdNew(string seqName)
		{
			//throw new Exception("Oracle获取序列异常,SQLServer和Oracle同时运行的时候不允许Oracle生成序列!");

			StringBuilder builder = new StringBuilder();

			builder.Append("select ");
			builder.Append(seqName);
			builder.Append(".Nextval from dual");

			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, builder.ToString());

			return DataConvert.ToInt(obj);
		}

        public virtual int GetId(string seqName)
        {
            throw new Exception("Oracle获取序列异常,SQLServer和Oracle同时运行的时候不允许Oracle生成序列!");

            //StringBuilder builder = new StringBuilder();

            //builder.Append("select ");
            //builder.Append(seqName);
            //builder.Append(".Nextval from dual");

            //object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, builder.ToString());

            //return DataConvert.ToInt(obj);
        }

        public virtual OracleParameter[] ToParameters(DbParameter[] parameters)
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

        public virtual OracleParameter[] ToParameters<T>(T parameters) where T : IEnumerable
        {
            List<OracleParameter> list = new List<OracleParameter>();

            DbParameter temp = null;
            OracleParameter tempOracleServer = null;

            foreach (var item in parameters)
            {
                temp = item as DbParameter;

                tempOracleServer = new OracleParameter();
                tempOracleServer.ParameterName = temp.ParameterName;
                tempOracleServer.DbType = temp.DbType;
                tempOracleServer.Size = temp.Size;
                tempOracleServer.Value = temp.Value;

                list.Add(tempOracleServer);
            }

            return list.ToArray();
        }

        /// <summary>
        /// The need for this method is highly annoying.
        /// When Oracle sets its output parameters, the OracleParameter.Value property
        /// is set to an internal Oracle type, not its equivelant System type.
        /// For example, strings are returned as OracleString, DBNull is returned
        /// as OracleNull, blobs are returned as OracleBinary, etc...
        /// So these Oracle types need unboxed back to their normal system types.
        /// </summary>
        /// <param name="oracleType">Oracle type to unbox.</param>
        /// <returns></returns>
        protected static object UnBoxOracleType(object oracleType)
        {
            if (oracleType == null)
                return null;

            Type T = oracleType.GetType();
            if (T == typeof(OracleString))
            {
                if (((OracleString)oracleType).IsNull)
                    return null;
                return ((OracleString)oracleType).Value;
            }
            else if (T == typeof(OracleDecimal))
            {
                if (((OracleDecimal)oracleType).IsNull)
                    return null;
                return ((OracleDecimal)oracleType).Value;
            }
            else if (T == typeof(OracleBinary))
            {
                if (((OracleBinary)oracleType).IsNull)
                    return null;
                return ((OracleBinary)oracleType).Value;
            }
            else if (T == typeof(OracleBlob))
            {
                if (((OracleBlob)oracleType).IsNull)
                    return null;
                return ((OracleBlob)oracleType).Value;
            }
            else if (T == typeof(OracleDate))
            {
                if (((OracleDate)oracleType).IsNull)
                    return null;
                return ((OracleDate)oracleType).Value;
            }
            else if (T == typeof(OracleTimeStamp))
            {
                if (((OracleTimeStamp)oracleType).IsNull)
                    return null;
                return ((OracleTimeStamp)oracleType).Value;
            }
            else // not sure how to handle these.
                return oracleType;
        }
    }
}