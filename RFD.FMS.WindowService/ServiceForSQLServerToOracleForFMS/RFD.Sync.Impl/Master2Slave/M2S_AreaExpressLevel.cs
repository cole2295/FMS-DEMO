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
    public class M2S_AreaExpressLevel : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("AreaExpressLevel");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlAreaExpressLevel, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from AreaExpressLevel as o join {0} as f on o.AutoId = f.AutoId
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("AutoId", "int", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("AreaExpressLevel", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("AutoId", "int", true));
            t.Cols.Add(new SyncCol("AreaID", "nvarchar(100)"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("WareHouseID", "nvarchar(40)"));
            t.Cols.Add(new SyncCol("AreaType", "int"));
            t.Cols.Add(new SyncCol("Enable", "IsEnable", "int", false));
            t.Cols.Add(new SyncCol("EffectAreaType", "int"));
            t.Cols.Add(new SyncCol("DoDate", "datetime"));
            t.Cols.Add(new SyncCol("CreateBy", "nvarchar(100)"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateBy", "nvarchar(100)"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("AuditStatus", "int"));
            t.Cols.Add(new SyncCol("AuditBy", "nvarchar(100)"));
            t.Cols.Add(new SyncCol("AuditTime", "datetime"));
            t.Cols.Add(new SyncCol("WareHouseType", "int"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("ProductID", "int"));
            t.Cols.Add(new SyncCol("ProductKid", "varchar(20)"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_AreaExpressLevel";
        }
    }
}
