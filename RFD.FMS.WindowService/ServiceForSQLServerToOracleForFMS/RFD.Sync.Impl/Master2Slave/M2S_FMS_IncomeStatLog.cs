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
    public class M2S_FMS_IncomeStatLog : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_IncomeStatLog");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlIncomeStatLog, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_IncomeStatLog as o join {0} as f on o.LogID = f.LogID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("LogID", "int", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_IncomeStatLog", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("LogID","bigint", true));
            t.Cols.Add(new SyncCol("StatisticsType", "int"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("StationID", "int"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("StatisticsDate", "date"));
            t.Cols.Add(new SyncCol("IsSuccess", "int"));
            t.Cols.Add(new SyncCol("Reasons", "varchar(150)"));
            t.Cols.Add(new SyncCol("Ip", "varchar(50)"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_IncomeStatLog";
        }
    }
}
