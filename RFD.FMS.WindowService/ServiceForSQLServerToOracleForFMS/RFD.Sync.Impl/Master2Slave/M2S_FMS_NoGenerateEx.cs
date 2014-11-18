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
    public class M2S_FMS_NoGenerateEx : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_NoGenerateEx");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlNoGenerateEx, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_NoGenerateEx as o join {0} as f on o.CurrentDate = f.CurrentDate 
                                    and o.TabColCode = f.TabColCode
                                    and o.LatestNo = f.LatestNo
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("CurrentDate", "date", true));
            t.Cols.Add(new SyncCol("TabColCode", "varchar(3)", true));
            t.Cols.Add(new SyncCol("LatestNo", "int", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_NoGenerateEx", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("CurrentDate", "date", true));
            t.Cols.Add(new SyncCol("TabColCode", "varchar(3)", true));
            t.Cols.Add(new SyncCol("LatestNo", "int", true));
            t.Cols.Add(new SyncCol("IsChange", "bit"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_NoGenerateEx";
        }
    }
}
