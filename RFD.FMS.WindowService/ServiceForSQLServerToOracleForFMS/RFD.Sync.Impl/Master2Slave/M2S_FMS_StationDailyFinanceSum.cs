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
    public class M2S_FMS_StationDailyFinanceSum : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_StationDailyFinanceSum");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlStationDailyFinanceSum, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from LMS_RFD.dbo.FMS_StationDailyFinanceSum as o join {0} as f on o.ID = f.ID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("ID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_StationDailyFinanceSum", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("ID", "SumID","bigint", true));
            t.Cols.Add(new SyncCol("DayReceiveOrderCount", "int"));
            t.Cols.Add(new SyncCol("DayReceiveNeedInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayReceiveNeedOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayReceiveGoodsCount", "int"));
            t.Cols.Add(new SyncCol("DayOutOrderCount", "int"));
            t.Cols.Add(new SyncCol("PreDayResortCount", "int"));
            t.Cols.Add(new SyncCol("PreDayResortNeedInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("PreDayResortNeedOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayTransferOrderCount", "int"));
            t.Cols.Add(new SyncCol("DayInStationNeedInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayInStationNeedOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayNeedDeliverOrderCount", "int"));
            t.Cols.Add(new SyncCol("DayNeedDeliverInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayNeedDeliverOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("CashSuccOrderCount", "int"));
            t.Cols.Add(new SyncCol("CashRealInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("CashRealOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("PosSuccOrderCount", "int"));
            t.Cols.Add(new SyncCol("PosRealInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DeliverSuccRate", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("RejectOrderCount", "int"));
            t.Cols.Add(new SyncCol("AllRejectNeedInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("AllRejectNeedOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayResortCount", "int"));
            t.Cols.Add(new SyncCol("DayResortNeedInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayResortNeedOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayOutStationOrderCount", "int"));
            t.Cols.Add(new SyncCol("DayOutStationNeedInSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DayOutStationNeedOutSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ResortRate", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("StationID", "int"));
            t.Cols.Add(new SyncCol("Sources", "int"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("DailyTime", "date"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("FinanceStatus", "int"));
            t.Cols.Add(new SyncCol("RealInCome", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("POSChecked", "int"));
            t.Cols.Add(new SyncCol("Remark", "nvarchar(1024)"));
            t.Cols.Add(new SyncCol("DayOutOrderSum", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ExchangeOrderCount", "int"));
            t.Cols.Add(new SyncCol("ExchangeOrderSum", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("UpdateBy", "nvarchar(20)"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_StationDailyFinanceSum";
        }
    }
}
