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
    public class M2S_FMS_CODLine : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_CODLine");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlCODLine, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_CODLine as o join {0} as f on o.CODLineNO = f.CODLineNO
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("CODLineNO", "nvarchar(20)", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_CODLine", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("CODLineNO", "nvarchar(20)", true));
            t.Cols.Add(new SyncCol("LineID", "bigint"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("IsEchelon", "smallint"));
            t.Cols.Add(new SyncCol("WareHouseID", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("AreaType", "smallint"));
            t.Cols.Add(new SyncCol("PriceFormula", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("LineStatus", "smallint"));
            t.Cols.Add(new SyncCol("AuditStatus", "smallint"));
            t.Cols.Add(new SyncCol("AuditBy", "int"));
            t.Cols.Add(new SyncCol("AuditTime", "datetime"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("DeleteFlag", "bit"));
            t.Cols.Add(new SyncCol("WareHouseType", "int"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("ProductID", "int"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("IsCOD", "int"));
            t.Cols.Add(new SyncCol("Formula", "nvarchar(150)"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_CODLine";
        }
    }
}
