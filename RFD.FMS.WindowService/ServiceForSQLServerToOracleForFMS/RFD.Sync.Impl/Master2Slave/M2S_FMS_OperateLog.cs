using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Quartz;
using RFD.Message;
using RFD.Sync.AdoNet;
using RFD.Sync.Impl.Dao;
using RFD.Sync.Impl.Tool;
using RFD.SyncSQL;

namespace RFD.Sync.Impl.Master2Slave
{
    public class M2S_FMS_OperateLog : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_OperateLog");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlOperateLog, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_OperateLog as o join {0} as f on o.OperateLogID = f.OperateLogID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("OperateLogID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_OperateLog", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("OperateLogID","bigint", true));
            t.Cols.Add(new SyncCol("BizOrderNo", "bigint"));
            t.Cols.Add(new SyncCol("LogType", "int"));
            t.Cols.Add(new SyncCol("Operation", "nvarchar(100)"));
            t.Cols.Add(new SyncCol("Operator", "OPERUSER","nvarchar(20)",false));
            t.Cols.Add(new SyncCol("OperatorId", "int"));
            t.Cols.Add(new SyncCol("OperatorDept", "int"));
            t.Cols.Add(new SyncCol("OperateTime", "datetime"));
            t.Cols.Add(new SyncCol("Description", "nvarchar(400)"));
            t.Cols.Add(new SyncCol("Reasult", "nvarchar(400)"));
            t.Cols.Add(new SyncCol("Status", "int"));
            t.Cols.Add(new SyncCol("IsSyn", "bit"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_OperateLog";
        }
    }
}
