using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using RFD.FMS.AdoNet.Exceptions;
using RFD.FMS.Util.Security;
using RFD.FMS.Util;

namespace RFD.FMS.AdoNet
{
    [Serializable]
    public abstract class DaoBase<T>
    {
        ///<summary>
        /// RFD主库连接字符串
        ///</summary>
        public virtual string ConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["ExecuteConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// RFD只读连接字符串
        ///</summary>
        public virtual string ReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["RedOnlyConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// LMS_RFD主库连接字符串 wangyongc 2011-08-18
        ///</summary>
        public virtual string LMSConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["ExecuteConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// LMS_RFD只读连接字符串 wangyongc 2011-08-18
        ///</summary>
        public virtual string LMSReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["RedOnlyConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// 平台库
        ///</summary>
        public virtual string VanclLMSQueryString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["VanclLMSQueryString"].ToString().Trim()); }
        }

        /// <summary>
        /// SCM只读
        /// </summary>
        public virtual string SCMReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["SCMRedOnlyConnString"].ToString().Trim()); }
        }

        /// <summary>
        /// SCM主库
        /// </summary>
        public virtual string SCMExecuteConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["SCMExecuteConnString"].ToString().Trim()); }
        }

        public T SCMExecuteConn
        {
            get
            {
                return GetConnection(SCMExecuteConnString);
            }
        }

        public T Connection
        {
            get
            {
                return GetConnection(ConnString);
            }
        }

        public T ReadOnlyConnection
        {
            get
            {
                return GetConnection(ReadOnlyConnString);
            }
        }

        public T LMSConnection
        {
            get
            {
                return GetConnection(LMSConnString);
            }
        }

        public T LMSReadOnlyConnection
        {
            get
            {
                return GetConnection(LMSReadOnlyConnString);
            }
        }

        public string ResetTimeOutString(string connectionString, int timeOutSecond)
        {
            if (timeOutSecond > 180)
                throw new AdoNetException("超时时间过长，建议重新设置，请低于180秒！");
            return string.Format("Connection Timeout={1};{0}", connectionString, timeOutSecond);
        }

        public abstract T GetConnection(string connstr);
    }
}