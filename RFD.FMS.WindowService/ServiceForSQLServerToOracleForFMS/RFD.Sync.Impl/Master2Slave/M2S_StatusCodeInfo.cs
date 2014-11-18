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
    public class M2S_StatusCodeInfo : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("StatusCodeInfo");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlStatusCodeInfo, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from StatusCodeInfo as o join {0} as f on o.CodeType = f.CodeType and o.CodeNo = f.CodeNo
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("CodeType", "varchar(20)", true));
            t.Cols.Add(new SyncCol("CodeNo", "varchar(20)", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("StatusCodeInfo", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("CodeType", "varchar(20)", true));
            t.Cols.Add(new SyncCol("CodeNo", "varchar(20)", true));
            t.Cols.Add(new SyncCol("CodeDesc", "varchar(20)"));
            t.Cols.Add(new SyncCol("OrderBy", "int"));
            t.Cols.Add(new SyncCol("Enabled", "int"));
            t.Cols.Add(new SyncCol("CreatBy", "varchar(20)"));
            t.Cols.Add(new SyncCol("CreatDate", "datetime"));
            t.Cols.Add(new SyncCol("UpdateBy", "varchar(20)"));
            t.Cols.Add(new SyncCol("UpdateDate", "datetime"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_StatusCodeInfo";
        }
    }
}
