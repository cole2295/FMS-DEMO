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
    public class M2S_FMS_IncomeDeliveryCount : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_IncomeDeliveryCount");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlIncomeDeliveryCount, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_IncomeDeliveryCount as o join {0} as f on o.CountID = f.CountID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("CountID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_IncomeDeliveryCount", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("CountID","bigint", true));
            t.Cols.Add(new SyncCol("AccountNO", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("AreaType", "int"));
            t.Cols.Add(new SyncCol("Weight", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("CountType", "int"));
            t.Cols.Add(new SyncCol("CountDate", "date"));
            t.Cols.Add(new SyncCol("CountNum", "int"));
            t.Cols.Add(new SyncCol("Fare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("Formula", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("IsDeleted", "tinyint"));
            t.Cols.Add(new SyncCol("StationID", "int"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_IncomeDeliveryCount";
        }
    }
}
