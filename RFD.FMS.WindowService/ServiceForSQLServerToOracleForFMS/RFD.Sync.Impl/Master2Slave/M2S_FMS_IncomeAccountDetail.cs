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
    public class M2S_FMS_IncomeAccountDetail : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_IncomeAccountDetail");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlIncomeAccountDetail, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_IncomeAccountDetail as o join {0} as f on o.DetailID = f.DetailID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("DetailID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_IncomeAccountDetail", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("DetailID","bigint", true));
            t.Cols.Add(new SyncCol("AccountNO", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("AreaType", "int"));
            t.Cols.Add(new SyncCol("DeliveryNum", "int"));
            t.Cols.Add(new SyncCol("DeliveryVNum", "int"));
            t.Cols.Add(new SyncCol("ReturnsNum", "int"));
            t.Cols.Add(new SyncCol("ReturnsVNum", "int"));
            t.Cols.Add(new SyncCol("VisitReturnsNum", "int"));
            t.Cols.Add(new SyncCol("VisitReturnsVNum", "int"));
            t.Cols.Add(new SyncCol("DeliveryStandard", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("DeliveryFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DeliveryVStandard", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("DeliveryVFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("RetrunsStandard", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("RetrunsFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ReturnsVStandard", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("ReturnsVFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("VisitReturnsStandard", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("VisitReturnsFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("VisitReturnsVStandard", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("VisitReturnsVFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ProtectedStandard", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ProtectedFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ReceiveStandard", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ReceiveFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ReceivePOSStandard", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ReceivePOSFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("OtherFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("Fare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DataType", "int"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("IsDeleted", "tinyint"));
            t.Cols.Add(new SyncCol("OverAreaSubsidy", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("KPI", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("LostDeduction", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ResortDeduction", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("StationId", "int"));
            t.Cols.Add(new SyncCol("AccountCount", "int"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_IncomeAccountDetail";
        }
    }
}
