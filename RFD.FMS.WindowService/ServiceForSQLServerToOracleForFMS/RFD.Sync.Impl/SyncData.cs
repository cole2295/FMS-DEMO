using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Quartz;
using RFD.Message;
using RFD.Sync.AdoNet;
using RFD.SyncSQL;
using Microsoft.ApplicationBlocks.Data;
using RFD.Sync.Impl.Tool;
using Oracle.ApplicationBlocks.Data;

namespace RFD.Sync.Impl
{
    public abstract class SyncBase : IStatefulJob
    {
        public int ExecuteSync()
        {
            string logKey = GetLogKey();

            try
            {
                DataSet ds = GetSyncData();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    MessageCollector.Instance.Collect(logKey, string.Format("取得{0}条数据", ds.Tables[0].Rows.Count));

                    List<DataRow> srcDr = ds.Tables[0].Select().ToList();
                    SyncTable toSyncT = GetSynStructTo();
                    SyncTable fromSyncT = GetSynStructFrom();
                    SyncSqlFactory syn = new SyncSqlFactory();

                    var toSqlList = syn.CreateSqlList("ORA", toSyncT, srcDr, NumOfEachUpdate());

                    bool ok = ExcSql("ORA",GetConnStrTo(), toSqlList);

                    if (!ok) throw new Exception("同步异常，写入目标数据库失败！");

                    var fromSqlList = syn.CreateSqlList("MS", fromSyncT, srcDr, NumOfEachUpdate());

                    ok = ExcSql("MS",GetConnStrFrom(), fromSqlList);

                    if (!ok) throw new Exception("同步异常，更新原数据库同步状态失败！");

                    return ds.Tables[0].Rows.Count;
                }

                MessageCollector.Instance.Collect(logKey, "未取得待同步数据");

                return 0;
            }
            catch (Exception ex)
            {
                MessageCollector.Instance.Collect(logKey, ex.ToString());

                Mail mail = new Mail();

                mail.SendMailToUser("服务运行异常", ex.Message, "gaopengxiang@vancl.cn;zengwei@vancl.cn");

                throw ex;
            }

            return -1;
        }

        protected bool ExcSql(string dbtype, string connstr, List<KeyValuePair<string, string>> sqlList)
        {
            bool succeed = false;

            for (int i = 0; i < sqlList.Count; i++)
            {
                succeed = ExcMsSql(dbtype, connstr, sqlList[i].Value);

                MessageCollector.Instance.Collect(GetType(), string.Format("n执行状态:{0}\r\n执行数据{1}",
                                                                (succeed ? "成功" : "失败"), sqlList[i].Key));
            }

            return succeed;
        }

        private bool ExcMsSql(string dbtype, string connstr, string sql)
        {
            try
            {
                var cons = DbDES.Decrypt3DES(connstr, Encoding.UTF8);

                if (dbtype.ToUpper() == "MS")
                {
                    return SqlHelper.ExecuteNonQuery(cons, CommandType.Text, sql, null) > 0;
                }
                else if (dbtype.ToUpper() == "ORA")
                {
                    return OracleHelper.ExecuteNonQuery(cons, CommandType.Text, sql) > 0;
                }
            }
            catch (Exception ex)
            {
                MessageCollector.Instance.Collect(GetType(), ex.ToString() + "\r\n MS_SQL:" + sql);

                throw ex;
            }

            return false;
        }

        protected abstract DataSet GetSyncData();
        protected abstract SyncTable GetSynStructFrom();
        protected abstract SyncTable GetSynStructTo();
        /// <summary>
        /// 得到子类的类名
        /// </summary>
        /// <returns></returns>
        protected abstract string GetClassName();

        protected virtual string GetConnStrFrom()
        {
            return DBStr.DbFromConnString;
        }

        protected virtual string GetConnStrTo()
        {
            return DBStr.DbToConnString;
        }


        /// <summary>
        /// 每次更新记录数
        /// </summary>
        /// <returns></returns>
        protected virtual int NumOfEachUpdate()
        {
            return SyncSelectParamByConfig.Instance.GetParam("default").Record;
        }

        /// <summary>
        /// 得到日志的键值
        /// </summary>
        /// <returns></returns>
        protected virtual string GetLogKey()
        {
            return "default";
        }

        #region IJob 成员

        public void Execute(JobExecutionContext context)
        {
            ExecuteSync();
        }

        #endregion
    }
}
