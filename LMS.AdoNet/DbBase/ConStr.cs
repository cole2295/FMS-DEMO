using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using LMS.Util.Security;

namespace LMS.AdoNet.DbBase
{
    /*
* (C)Copyright 2011-2012 如风达信息管理系统
* 
* 模块名称：数据库连接字符串
* 说明：取连接串等
* 作者：韩冰
* 创建日期：2011-02-30 13:42:00
* 修改人：
* 修改时间：
* 修改记录：
*/
    public static class ConStr
    {

        private static string rfdConStr;
        /// <summary>
        /// 如风达主库
        /// </summary>
        public static string RfdConStr
        {
            get
            {
                var cns = DbConnectionstringFromDb.Instance.GetConnStr(CurrentDbCode.DbCode, DatabaseType.Execute);
                if (!string.IsNullOrEmpty(cns))
                {
                    return DES.Decrypt3DES(cns.Trim());
                }


                if (string.IsNullOrEmpty(rfdConStr))
                {
                    rfdConStr = DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["ExecuteConnString"].ToString().Trim());
                }
                return rfdConStr;
            }
        }

        private static string readOnlyConnString;
        /// <summary>
        /// 如风达只读库
        /// </summary>
        public static string ReadOnlyConnString
        {
            get
            {
                if (string.IsNullOrEmpty(readOnlyConnString))
                {
                    readOnlyConnString = DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["ExecuteConnString"].ToString().Trim());
                }
                return readOnlyConnString;
            }
        }

        private static string _dbConfigConnString;
        public static string DbConfigConnString
        {
            get
            {
                if (string.IsNullOrEmpty(_dbConfigConnString))
                {
                    string confName = "DbConfigConnString";
                    if (ConfigurationManager.ConnectionStrings[confName] == null)
                    {
                        confName = "ExecuteConnString";
                    }

                    _dbConfigConnString = DES.Decrypt3DES(ConfigurationManager.ConnectionStrings[confName].ToString().Trim());
                }
                return _dbConfigConnString;
            }
        }
    }
}
