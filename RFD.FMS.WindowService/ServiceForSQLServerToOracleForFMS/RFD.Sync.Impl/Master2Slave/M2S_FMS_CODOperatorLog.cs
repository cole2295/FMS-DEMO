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
    public class M2S_FMS_CODOperatorLog : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_CODOperatorLog");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlCODOperatorLog, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_CODOperatorLog as o join {0} as f on o.LogID = f.LogID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("LogID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_CODOperatorLog", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("LogID", "bigint", true));
            t.Cols.Add(new SyncCol("PK_NO", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("LogText", "nvarchar(250)"));
            t.Cols.Add(new SyncCol("LogType", "int"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_CODOperatorLog";
        }
    }
}
