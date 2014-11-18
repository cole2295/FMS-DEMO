using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class FMS_DeductBaseInfoDao : OracleDao, IFMS_DeductBaseInfoDao
    {
        /// <summary>
        /// 添加提成信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long Add(FMS_DeductSubBaseInfo model)
        {
            string sql = @"insert into FMS_DeductSubBaseInfo (WaybillNO,Formula,DeductType,StationId,DeliverUser,BasicCommission,
                    AreaCommission,WeightCommission,CreateTime,UpdateTime,IsDeleted,IsChange)
                Values(:WaybillNO,:Formula,:DeductType,:StationId,:DeliverUser,:BasicCommission,
                    :AreaCommission,:WeightCommission,:CreateTime,:UpdateTime,:IsDeleted,3)
                ;select @@IDENTITY";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":WaybillNO",model.WaybillNO),
                new OracleParameter(":Formula",model.Formula),
                new OracleParameter(":DeductType",model.DeductType),
                new OracleParameter(":StationId",model.StationId),
                new OracleParameter(":DeliverUser",model.DeliverUser),
                new OracleParameter(":BasicCommission",model.BasicCommission),
                new OracleParameter(":AreaCommission",model.AreaCommission),
                new OracleParameter(":WeightCommission",model.WeightCommission),
                new OracleParameter(":CreateTime",model.CreateTime),
                new OracleParameter(":UpdateTime",model.UpdateTime),
                new OracleParameter(":IsDeleted",model.IsDeleted)
            };

            return DataConvert.ToLong(OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql, parameters));
        }

        public bool IsCalculateOK(long id)
        {
            string sql = "update FMS_DeductBaseInfo set IsDeductAcount=1,UpdateTime=sysdate,IsChange=3 where DeductID=:DeductID";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":DeductID",id)
            };

            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters.ToArray());

            return rows == 1;
        }

        /// <summary>
        /// 查询快递取件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetExpressReceiveModel(string DistributionCodeList)
        {
            string sql =
                @"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,bInfo.ReceiveStationID,
                    bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,bInfo.IsDeductAcount,
                    bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,bInfo.MerchantID,bInfo.BackStationTime
                from FMS_DeductBaseInfo bInfo 
                    inner join MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
                    inner join ExpressCompany ecp on bInfo.ReceiveStationID=ecp.ExpressCompanyID
                where bInfo.IsDeductAcount=0 and bInfo.IsDeleted=0
                    and mInfo.IsExpressDelivery=1 
                    and bInfo.Status in ('-3','-5','0')
                    and bInfo.CreateTime > :SearchDate
                    and ecp.DistributionCode='rfd'";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":SearchDate",DateTime.Now.AddDays(-2))
            };

            List<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> list = new List<MODEL.FinancialManage.FMS_DeductBaseInfo>();

            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }

        /// <summary>
        /// 查询快递派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetExpressSendModel(string DistributionCodeList)
        {
            string sql = @"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,
                    bInfo.ReceiveStationID,bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,
                    bInfo.IsDeductAcount,bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,bInfo.MerchantID,bInfo.BackStationTime
                from FMS_DeductBaseInfo bInfo 
                    inner join MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
                    inner join ExpressCompany ecp on bInfo.DeliverStationID=ecp.ExpressCompanyID
                where bInfo.IsDeductAcount=0 and bInfo.IsDeleted=0
                    and mInfo.IsExpressDelivery=1 
                    and bInfo.Status='3'
                    and bInfo.CreateTime > :SearchDate
                    and ecp.DistributionCode='rfd'";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":SearchDate",DateTime.Now.AddDays(-2))
            };

            List<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> list = new List<MODEL.FinancialManage.FMS_DeductBaseInfo>();

            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql,parameters);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }

        /// <summary>
        /// 查询项目单派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetProjectSendModel(string DistributionCodeList)
        {
            string sql = @"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,
                    bInfo.ReceiveStationID,bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,
                    bInfo.IsDeductAcount,bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,bInfo.MerchantID,bInfo.BackStationTime
                from FMS_DeductBaseInfo bInfo 
                    inner join MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
                    inner join ExpressCompany ecp on bInfo.DeliverStationID=ecp.ExpressCompanyID
                where bInfo.IsDeductAcount=0 and bInfo.IsDeleted=0 
                    and bInfo.Status='3'
                    and mInfo.IsExpressDelivery!=1 
                    and bInfo.CreateTime > :SearchDate
                    and ecp.DistributionCode='rfd'";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":SearchDate",DateTime.Now.AddDays(-2))
            };

            List<FMS_DeductBaseInfo> list = new List<FMS_DeductBaseInfo>();

            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }

        /// <summary>
        /// 获取站点快递派件公式
        /// </summary>
        /// <param name="stationIds">站点编号集合</param>
        /// <returns>站点公式集合</returns>
        public DataTable GetExpressSendFormula(string stationIds)
        {
            string sql = String.Format(@"select ExpressCompanyID StationId,SendFormula Formula from DeductStationBaseInfo
            where ExpressCompanyID in ({0}) and ReductType=2 and IsDeleted=0 and UseStatus=1", stationIds);

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        /// <summary>
        /// 获取站点快递取件公式
        /// </summary>
        /// <param name="stationIds">站点编号集合</param>
        /// <returns>站点公式集合</returns>
        public DataTable GetExpressReceiveFormula(string stationIds)
        {
            string sql = String.Format(@"select ExpressCompanyID StationId,ReceiveFormula Formula from DeductStationBaseInfo
            where ExpressCompanyID in ({0}) and ReductType=2 and IsDeleted=0 and UseStatus=1",stationIds);

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        /// <summary>
        /// 获取站点项目派件公式
        /// </summary>
        /// <param name="stationIds">站点集合</param>
        /// <returns>站点公式集合</returns>
        public DataTable GetProjectSendFormula(string stationIds, string categoryCodes)
        {
            string sql = String.Format(@"select ExpressCompanyID StationId,GoodsCategoryCode,SendFormula Formula from DeductStationBaseInfo
            where ExpressCompanyID in ({0}) and GoodsCategoryCode in ({1}) and ReductType=1 and IsDeleted=0 and UseStatus=1", stationIds,categoryCodes);

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        private FMS_DeductBaseInfo DataRowToObject(DataRow row)
        {
            FMS_DeductBaseInfo model = new FMS_DeductBaseInfo();

            model.DeductID = DataConvert.ToLong(row["DeductID"]);
            model.WaybillNO = DataConvert.ToLong(row["WaybillNO"]);
            model.DeliverStationID = DataConvert.ToInt(row["DeliverStationID"]);
            model.ReceiveStationID = DataConvert.ToInt(row["ReceiveStationID"]);
            model.DeliverManID = DataConvert.ToInt(row["DeliverManID"]);
            model.ReceiveDeliverManID = DataConvert.ToInt(row["ReceiveDeliverManID"]);
            model.WayBillInfoWeight = DataConvert.ToDecimal(row["WayBillInfoWeight"]);
            model.IsDeductAcount = DataConvert.ToInt(row["IsDeductAcount"]);
            model.WaybillCategory = DataConvert.ToString(row["WaybillCategory"]);
            model.AreaSendDeduct = DataConvert.ToDecimal(row["DeductMoney"], -1000);
            model.AreaReceiveDeduct = DataConvert.ToDecimal(row["DeductNotes"],-1000);
            model.MerchantID = DataConvert.ToInt(row["MerchantID"]);
            if (!string.IsNullOrEmpty(row["BackStationTime"].ToString().Trim()))
            {
                model.BackStationTime = DataConvert.ToDateTime(row["BackStationTime"]);
            }
            return model;
        }

        public decimal GetStationDefaultAreaDeduct(int stationId)
        {
            string sql = @"select DeductValue from DeductStationAreaCommission dsac where IsDeleted=0 and StationId=:StationId and IsDefault=1";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":StationId",stationId)
            };

            return DataConvert.ToDecimal(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        public void DeleteOldDeduct(long waybillNO, int deductType)
        {
            string sql = "update FMS_DeductSubBaseInfo set IsDeleted=1,UpdateTime=sysdate ,IsChange=3 where waybillno=:waybillno and DeductType=:DeductType and IsDeleted=0";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":waybillno",waybillNO),
                new OracleParameter(":DeductType",deductType)
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        }

        public IList<long> CheckRepeatDeduct()
        {
            throw new Exception("未实现此方法!");
        }

        public IList<long> ClearNotEvalDeduct()
        {
            throw new Exception("未实现此方法!");
        }

        public IList<long> RepairExpressDelete()
        {
            throw new Exception("未实现此方法!");
        }

        public IList<long> RepairDeductDate()
        {
            throw new Exception("未实现此方法!");
        }

        private void ClearNotEvalDeduct(long id, long waybillNO, int deductType)
        {
            string sql = "select count(1) from FMS_DeductSubBaseInfo where WaybillNO=:WaybillNO and DeductType=:DeductType and IsDeleted=0";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":waybillno",waybillNO),
                new OracleParameter(":DeductType",deductType)
            };

            int count = DataConvert.ToInt(OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql, parameters));

            if (count > 0) return;

            string sqlUpdate = "update FMS_DeductSubBaseInfo set IsDeleted=0,UpdateTime=sysdate,IsChange=3  where ID=:ID";

            OracleParameter[] parameters1 = 
            {
                new OracleParameter(":ID",id)
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlUpdate, parameters1);
        }

        private void ClearErrorDeduct(long waybill, int deduct)
        {
            string sql = "select ID from FMS_DeductSubBaseInfo where WaybillNO=:WaybillNO and DeductType=:DeductType and IsDeleted=0";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":WaybillNO",waybill),
                new OracleParameter(":DeductType",deduct)
            };

            DataTable table = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];

            DataRow row = null;

            string sqlUpdate = "update FMS_DeductSubBaseInfo set IsDeleted=1,UpdateTime=sysdate,IsChange=3  where ID=";

            for (int i = 0; i < table.Rows.Count - 1; i++)
            {
                row = table.Rows[i];

                OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlUpdate + DataConvert.ToLong(row["ID"]));
            }
        }

        public IList<long> RepairExpressDate()
        {
            throw new NotImplementedException();
        }

        public IList<long> RepairFalseDelete()
        {
            throw new NotImplementedException();
        }


        public bool IsDeleted(long deductId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询项目单派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetProjectSendModelByWaybillNO(long WaybillNO)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询项目单派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetExpressSendModelByWaybillNO(long WaybillNO)
        {
            throw new NotImplementedException();
        }

        public void UpdateSubAreaDeduct(FMS_DeductSubBaseInfo model)
        {
            string sql = "update FMS_DeductSubBaseInfo SET AREACOMMISSION=:AREACOMMISSION where waybillno=:waybillno and DeductType=:DeductType and IsDeleted=0";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":AREACOMMISSION",model.AreaCommission),
                new OracleParameter(":waybillno",model.WaybillNO),
                new OracleParameter(":DeductType",model.DeductType)
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        }
    }
}
