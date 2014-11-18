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
    public class M2S_FMS_CodStatsLog : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_CodStatsLog");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlCodStatsLog, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_CodStatsLog as o join {0} as f on o.CodStatsID = f.CodStatsID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("CodStatsID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_CodStatsLog", HowTableUpdate.TableInsertAndUpdate);

            t.Cols.Add(new SyncCol("CodStatsID","bigint", true));
            t.Cols.Add(new SyncCol("StatisticsType", "smallint"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("WareHouseID", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("StatisticsDate", "date"));
            t.Cols.Add(new SyncCol("IsSuccess", "smallint"));
            t.Cols.Add(new SyncCol("Reasons", "nvarchar(500)"));
            t.Cols.Add(new SyncCol("Ip", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("WareHouseType", "int"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_CodStatsLog";
        }
    }
}
