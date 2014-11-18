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
    public class M2S_FMS_IncomeBaseInfo : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_IncomeBaseInfo");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlIncomeBaseInfo, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from LMS_RFD.dbo.FMS_IncomeBaseInfo as o join {0} as f on o.IncomeID = f.IncomeID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("IncomeID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_IncomeBaseInfo", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("IncomeID", "IncomeID", "bigint", true));
            t.Cols.Add(new SyncCol("WaybillNo", "bigint"));
            t.Cols.Add(new SyncCol("WaybillType", "varchar(20)"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("FinalExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("DeliverStationID", "int"));
            t.Cols.Add(new SyncCol("TopCODCompanyID", "int"));
            t.Cols.Add(new SyncCol("RfdAcceptTime", "datetime"));
            t.Cols.Add(new SyncCol("DeliverTime", "datetime"));
            t.Cols.Add(new SyncCol("ReturnTime", "datetime"));
            t.Cols.Add(new SyncCol("ReturnExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("BackStationTime", "datetime"));
            t.Cols.Add(new SyncCol("BackStationStatus", "varchar(20)"));
            t.Cols.Add(new SyncCol("ProtectedAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("TotalAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("PaidAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("NeedPayAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("BackAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("NeedBackAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("AccountWeight", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("AreaID", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("ReceiveAddress", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("SignType", "int"));
            t.Cols.Add(new SyncCol("InefficacyStatus", "int"));
            t.Cols.Add(new SyncCol("createtime", "datetime") { OnlyInsert = OnlyINSERT.OnlyInsert });
            t.Cols.Add(new SyncCol("updatetime", "datetime"));
            t.Cols.Add(new SyncCol("ReceiveStationID", "int"));
            t.Cols.Add(new SyncCol("ReceiveDeliverManID", "int"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("CurrentDistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("WayBillInfoWeight", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("SubStatus", "int"));
            t.Cols.Add(new SyncCol("AcceptType", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("CustomerOrder", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("OriginDepotNo", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("PeriodAccountCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("DeliverCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("ISDELETED", "bit"));
            t.Cols.Add(new SyncCol("waybillcategory", "nvarchar(10)"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_IncomeBaseInfo";
        }
    }
}
