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
    public class M2S_FMSTableColumnDic : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMSTableColumnDic");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlFMSTableColumnDic, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMSTableColumnDic as o join {0} as f on o.TabColCode = f.TabColCode
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("TabColCode", "varchar(3)", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMSTableColumnDic", HowTableUpdate.TableInsertAndUpdate);

            t.Cols.Add(new SyncCol("TabColCode", "varchar(3)", true));
            t.Cols.Add(new SyncCol("TableName", "varchar(50)"));
            t.Cols.Add(new SyncCol("ColumnName", "varchar(50)"));
            t.Cols.Add(new SyncCol("Remark", "nvarchar(50)"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMSTableColumnDic";
        }
    }
}
