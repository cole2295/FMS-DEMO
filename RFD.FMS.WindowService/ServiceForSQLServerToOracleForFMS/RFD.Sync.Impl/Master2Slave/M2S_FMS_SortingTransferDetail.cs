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
    public class M2S_FMS_SortingTransferDetail : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_SortingTransferDetail");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlSortingTransferDetail, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_SortingTransferDetail as o join {0} as f on o.DetailKid = f.DetailKid
                                ";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("DetailKid", "varchar(20)", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_SortingTransferDetail", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("DetailKid", "varchar(20)", true));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("WaybillNo", "bigint"));
            t.Cols.Add(new SyncCol("WaybillType", "varchar(20)"));
            t.Cols.Add(new SyncCol("SortingCenterID", "int"));
            t.Cols.Add(new SyncCol("TSortingCenterID", "int"));
            t.Cols.Add(new SyncCol("SoringMerchantID", "int"));
            t.Cols.Add(new SyncCol("CreateCityID", "nvarchar(10)"));
            t.Cols.Add(new SyncCol("SortCityID", "nvarchar(10)"));
            t.Cols.Add(new SyncCol("DeliverStationID", "int"));
            t.Cols.Add(new SyncCol("TopCODCompanyID", "int"));
            t.Cols.Add(new SyncCol("ToStationTime", "datetime"));
            t.Cols.Add(new SyncCol("OutBoundTime", "datetime"));
            t.Cols.Add(new SyncCol("ReturnTime", "datetime"));
            t.Cols.Add(new SyncCol("InSortingTime", "datetime"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("IsAccount", "int"));
            t.Cols.Add(new SyncCol("AccountFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("AccountFormula", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("IsDeleted", "bit"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("ReturnSortingCenterID", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("OutType","int"));
            t.Cols.Add(new SyncCol("IntoType","int"));
            t.Cols.Add(new SyncCol("IsChange","bit"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_SortingTransferDetail";
        }
    }
}
