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
    public class M2S_FMS_StationDeliverFee : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_StationDeliverFee");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlStationDeliverFee, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_StationDeliverFee as o join {0} as f on o.ID = f.ID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("ID", "int", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_StationDeliverFee", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("ID", "FEEID","int", true));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("StationID", "int"));
            t.Cols.Add(new SyncCol("BasicDeliverFee", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateCode", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("AuditBy", "int"));
            t.Cols.Add(new SyncCol("AuditTime", "datetime"));
            t.Cols.Add(new SyncCol("AuditCode", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("AuditResult", "int"));
            t.Cols.Add(new SyncCol("Status", "char(5)"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("IsCenterSort", "int"));
            t.Cols.Add(new SyncCol("AreaType", "int"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("IsDeleted", "tinyint"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("GoodsCategoryCode", "nvarchar(10)"));
            t.Cols.Add(new SyncCol("DeliverFee", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("IsCod", "int"));
            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_StationDeliverFee";
        }
    }
}
