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
    public class M2S_FMS_CODAccountDetail : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_CODAccountDetail");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlCODAccountDetailcs, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_CODAccountDetail as o join {0} as f on o.AccountDetailID = f.AccountDetailID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("AccountDetailID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_CODAccountDetail", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("AccountDetailID", "bigint", true));
            t.Cols.Add(new SyncCol("AccountNO", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("WareHouseID", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("AreaType", "smallint"));
            t.Cols.Add(new SyncCol("DeliveryNum", "int"));
            t.Cols.Add(new SyncCol("DeliveryVNum", "int"));
            t.Cols.Add(new SyncCol("ReturnsNum", "int"));
            t.Cols.Add(new SyncCol("ReturnsVNum", "int"));
            t.Cols.Add(new SyncCol("VisitReturnsNum", "int"));
            t.Cols.Add(new SyncCol("Formula", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("DatumFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("Allowance", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("KPI", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("POSPrice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("StrandedPrice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("IntercityLose", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("OtherCost", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("Fare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DataType", "smallint"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("DeleteFlag", "bit"));
            t.Cols.Add(new SyncCol("WareHouseType", "int"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("AccountNum", "int"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_CODAccountDetail";
        }
    }
}
