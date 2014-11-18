using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using Oracle.DataAccess.Client;

namespace RFD.Sync.AdoNet
{
    public class DBConn
    {
        public static DbConnection GetConnection(string dbflag, string connstr)
        {
            DbConnection connection = null;

            connection = NeutralContext.Get(GetConnThread(dbflag));

            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            else
            {
                connection = NewConn(dbflag, connstr);

                NeutralContext.Put(GetConnThread(dbflag), connection);
            }

            return connection;
        }

        private static string GetConnThread(string dbflag)
        {
            return dbflag + Thread.CurrentThread.ManagedThreadId.ToString();
        }

        private static DbConnection NewConn(string dbflag, string connstr)
        {
            if (dbflag.ToUpper() == "MS")
            {
                return new SqlConnection(connstr);
            }
            else if (dbflag.ToUpper() == "ORA")
            {
                return new OracleConnection(connstr);
            }
            
            throw new Exception("未知的数据库类型");
        }
    }

    public class DBStr
    {
        private static string _dbFromConnString;
        public static string DbFromConnString
        {
            get
            {
                if (string.IsNullOrEmpty(_dbFromConnString))
                {
                    _dbFromConnString = ConfigurationManager.ConnectionStrings["dbfrom"].ToString().Trim();
                }
                return _dbFromConnString;
            }
        }

        private static string _dbToConnString;
        public static string DbToConnString
        {
            get
            {
                if (string.IsNullOrEmpty(_dbToConnString))
                {
                    _dbToConnString = ConfigurationManager.ConnectionStrings["dbto"].ToString().Trim();
                }
                return _dbToConnString;
            }
        }
    }


    public class NeutralContext
    {
        private static readonly Dictionary<string, DbConnection> contexts = new Dictionary<string, DbConnection>();

        /// <summary>
        /// 获取key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DbConnection Get(string key)
        {
            if (contexts.ContainsKey(key))
            {
                return contexts[key];
            }
            return null;
        }

        /// <summary>
        /// 设置指定key的值为value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Put(string key, DbConnection value)
        {
            if (contexts.ContainsKey(key))
            {
                contexts[key] = value;
            }
            else
            {
                contexts.Add(key, value);
            }
        }

        /// <summary>
        /// 移除指定key值
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (contexts.ContainsKey(key))
            {
                contexts.Remove(key);
            }
        }
    }
}
