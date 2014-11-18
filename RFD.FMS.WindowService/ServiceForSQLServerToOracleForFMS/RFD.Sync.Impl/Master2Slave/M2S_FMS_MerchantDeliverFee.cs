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
    public class M2S_FMS_MerchantDeliverFee : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_MerchantDeliverFee");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlMerchantDeliverFee, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_MerchantDeliverFee as o join {0} as f on o.ID = f.ID
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("ID", "int", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_MerchantDeliverFee", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("ID", "FeeID","int", true));
            t.Cols.Add(new SyncCol("MerchantID", "int"));
            t.Cols.Add(new SyncCol("PaymentType", "int"));
            t.Cols.Add(new SyncCol("PaymentPeriod", "int"));
            t.Cols.Add(new SyncCol("DeliverFeeType", "int"));
            t.Cols.Add(new SyncCol("DeliverFeePeriod", "int"));
            t.Cols.Add(new SyncCol("FeeFactors", "varchar(50)"));
            t.Cols.Add(new SyncCol("IsUniformedFee", "bit"));
            t.Cols.Add(new SyncCol("BasicDeliverFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("FormulaID", "int"));
            t.Cols.Add(new SyncCol("FormulaParamters", "varchar(100)"));
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateCode", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("AuditBy", "int"));
            t.Cols.Add(new SyncCol("AuditTime", "datetime"));
            t.Cols.Add(new SyncCol("AuditCode", "nvarchar(20)"));
            t.Cols.Add(new SyncCol("AuditResult", "int"));
            t.Cols.Add(new SyncCol("Status", "char(5)"));
            t.Cols.Add(new SyncCol("RefuseFeeRate", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ReceiveFeeRate", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("PaymentPeriodDate", "datetime"));
            t.Cols.Add(new SyncCol("DeliverFeePeriodDate", "datetime"));
            t.Cols.Add(new SyncCol("FirstWeight", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("StatPramer", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("AddWeightPrice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("FirstWeightPrice", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("VolumeParmer", "decimal(18, 2)"));
            t.Cols.Add(new SyncCol("ProtectedParmer", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("VisitReturnsFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("VisitChangeFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ReceivePOSFeeRate", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("VisitReturnsVFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("CashServiceFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("POSServiceFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ReceiveFeeType", "int"));
            t.Cols.Add(new SyncCol("ReceivePOSFeeType", "int"));
            t.Cols.Add(new SyncCol("CashServiceType", "int"));
            t.Cols.Add(new SyncCol("POSServiceType", "int"));
            t.Cols.Add(new SyncCol("WeightType", "int"));
            t.Cols.Add(new SyncCol("WeightValueRule", "int"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));

            t.Cols.Add(new SyncCol("ExtraProtected", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraRefuseFeeRate", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraVisitReturnsFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraVisitChangeFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraVisitReturnsVFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraReceiveFeeRate", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraCashServiceFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraPOSServiceFee", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("ExtraReceivePOSFeeRate", "decimal(18, 3)"));
            t.Cols.Add(new SyncCol("IsCategory", "int"));
            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_MerchantDeliverFee";
        }
    }
}
