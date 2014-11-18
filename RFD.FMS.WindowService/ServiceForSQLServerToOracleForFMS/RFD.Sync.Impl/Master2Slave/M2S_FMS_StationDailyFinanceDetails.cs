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
    public class M2S_FMS_StationDailyFinanceDetails : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_StationDailyFinanceDetails");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlStationDailyFinanceDetails, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from LMS_RFD.dbo.FMS_StationDailyFinanceDetails as o join {0} as f on o.ID = f.ID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);
            t.Cols.Add(new SyncCol("ID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_StationDailyFinanceDetails", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("ID", "DETAILID", "bigint", true));
            t.Cols.Add(new SyncCol("EnterTime", "datetime"));
            t.Cols.Add(new SyncCol("WaybillNO", "bigint"));
            t.Cols.Add(new SyncCol("ReplaceTypeName", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("NeedPrice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("NeedReturnPrice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("PriceDiff", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("PriceReturnCash", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("Weight", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("WaybillType", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("StatusName", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("AcceptType", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("RejectReason", "nvarchar(500)"));
            t.Cols.Add(new SyncCol("ResortReason", "nvarchar(500)"));
            t.Cols.Add(new SyncCol("PostTime", "datetime"));
            t.Cols.Add(new SyncCol("DeliverManName", "varchar(20)"));
            t.Cols.Add(new SyncCol("Comment","Description", "varchar(20)", false));
            t.Cols.Add(new SyncCol("Status", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("StationID", "int"));
            t.Cols.Add(new SyncCol("Sources", "int"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("OPType", "tinyint"));
            t.Cols.Add(new SyncCol("PosNum", "nvarchar(200)"));
            t.Cols.Add(new SyncCol("LmsId", "int"));
            t.Cols.Add(new SyncCol("DeductMoney", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("CustomerOrder", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("DailyTime", "date"));
            t.Cols.Add(new SyncCol("ExpandField", "nvarchar(200)"));
            t.Cols.Add(new SyncCol("DeliverFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("Protectedprice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DeliverManID", "int"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("CurrentDistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("ComeFrom", "int"));
            t.Cols.Add(new SyncCol("IsDelete", "bit"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_StationDailyFinanceDetails";
        }
    }
}
