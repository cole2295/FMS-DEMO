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
        /// RFD���������ַ���
        ///</summary>
        public virtual string ConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["ExecuteConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// RFDֻ�������ַ���
        ///</summary>
        public virtual string ReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["RedOnlyConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// LMS_RFD���������ַ��� wangyongc 2011-08-18
        ///</summary>
        public virtual string LMSConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["ExecuteConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// LMS_RFDֻ�������ַ��� wangyongc 2011-08-18
        ///</summary>
        public virtual string LMSReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["RedOnlyConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// ƽ̨��
        ///</summary>
        public virtual string VanclLMSQueryString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["VanclLMSQueryString"].ToString().Trim()); }
        }

        /// <summary>
        /// SCMֻ��
        /// </summary>
        public virtual string SCMReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["SCMRedOnlyConnString"].ToString().Trim()); }
        }

        /// <summary>
        /// SCM����
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
                throw new AdoNetException("��ʱʱ������������������ã������180�룡");
            return string.Format("Connection Timeout={1};{0}", connectionString, timeOutSecond);
        }

        public abstract T GetConnection(string connstr);
    }
}