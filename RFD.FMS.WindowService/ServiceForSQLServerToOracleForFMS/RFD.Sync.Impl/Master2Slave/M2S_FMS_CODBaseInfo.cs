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
    public class M2S_FMS_CODBaseInfo : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_CODBaseInfo");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlCodBaseInfo, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from LMS_RFD.dbo.FMS_CODBaseInfo as o join {0} as f on o.ID = f.ID
                                ";

            SyncTable t = new SyncTable(userSql);
            t.Cols.Add(new SyncCol("ID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_CODBaseInfo", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("ID","INFOID", "bigint", true));
            t.Cols.Add(new SyncCol("MediumID", "bigint"));
            t.Cols.Add(new SyncCol("WaybillNO", "bigint"));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("WaybillType", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("Flag", "int"));
            t.Cols.Add(new SyncCol("DeliverStationID", "int"));
            t.Cols.Add(new SyncCol("TopCODCompanyID", "int"));
            t.Cols.Add(new SyncCol("WarehouseId", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("ExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("RfdAcceptTime", "datetime"));
            t.Cols.Add(new SyncCol("RfdAcceptDate", "date"));
            t.Cols.Add(new SyncCol("FinalExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("DeliverTime", "datetime"));
            t.Cols.Add(new SyncCol("DeliverDate", "date"));
            t.Cols.Add(new SyncCol("ReturnWareHouseID", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("ReturnExpressCompanyID", "int"));
            t.Cols.Add(new SyncCol("TotalAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("PaidAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("NeedPayAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("BackAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("NeedBackAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("AccountWeight", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("AreaID", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("AreaType", "int"));
            t.Cols.Add(new SyncCol("BoxsNo", "nvarchar(30)"));
            t.Cols.Add(new SyncCol("Address", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime") { OnlyInsert = OnlyINSERT.OnlyInsert });
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("IsDeleted", "bit"));
            t.Cols.Add(new SyncCol("ReturnTime", "datetime"));
            t.Cols.Add(new SyncCol("ReturnDate", "date"));
            t.Cols.Add(new SyncCol("IsFare", "int"));
            t.Cols.Add(new SyncCol("Fare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("FareFormula", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("OperateType", "int"));
            t.Cols.Add(new SyncCol("ProtectedPrice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("CurrentDistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("ComeFrom", "int"));
            t.Cols.Add(new SyncCol("IsCOD", "int"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_CODBaseInfo";
        }
    }
}
