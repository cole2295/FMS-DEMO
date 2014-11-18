using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.SyncSQL;
using RFD.Sync.Impl.Dao;
using RFD.Sync.Impl.Tool;
using System.Data;
using RFD.Sync.AdoNet;

namespace RFD.Sync.Impl.Master2Slave
{
    public class M2S_FMS_IncomeFeeInfo_History : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_IncomeFeeInfo_History");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlIncomeFeeInfo_History, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from LMS_RFD.dbo.FMS_IncomeFeeInfo as o join {0} as f on o.IncomeFeeID = f.IncomeFeeID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);
            t.Cols.Add(new SyncCol("IncomeFeeID", "bigint", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_IncomeFeeInfo", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("IncomeFeeID", "bigint", true));
            t.Cols.Add(new SyncCol("WaybillNO", "bigint"));
            t.Cols.Add(new SyncCol("IsAccount", "int"));
            t.Cols.Add(new SyncCol("AccountStandard", "nvarchar(150)"));
            t.Cols.Add(new SyncCol("AccountFare", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("IsProtected", "int"));
            t.Cols.Add(new SyncCol("ProtectedStandard", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ProtectedFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("IsReceive", "int"));
            t.Cols.Add(new SyncCol("ReceiveStandard", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ReceiveFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime") { OnlyInsert = OnlyINSERT.OnlyInsert });
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("IsDeleted", "tinyint"));
            t.Cols.Add(new SyncCol("TransferPayType", "int"));
            t.Cols.Add(new SyncCol("DeputizeAmount", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("POSReceiveStandard", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("POSReceiveFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("CashReceiveServiceStandard", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("CashReceiveServiceFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("POSReceiveServiceStandard", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("POSReceiveServiceFee", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ExpressReceiveBasicDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ExpressSendBasicDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ExpressAreaDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ExpressWeightDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ProgramReceiveBasicDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ProgramSendBasicDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ProgramAreaDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ProgramWeightDeduct", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("IsDeductAcount", "int"));
            t.Cols.Add(new SyncCol("AreaType", "int"));
            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_IncomeFeeInfo_History";
        }
    }
}
