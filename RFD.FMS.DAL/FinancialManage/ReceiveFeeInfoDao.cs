using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.Util;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.FinancialManage
{
    public class ReceiveFeeInfoDao : SqlServerDao, IReceiveFeeInfoDao
    {
        public FMS_IncomeReceiveFeeInfo GetProjectSendFeeInfoModel(long waybillno)
        {
            string sql = @"SELECT w.WaybillNO,w.WaybillType,w.DeliverManID,w.Sources,w.ComeFrom,w.MerchantID,
	            w.CustomerOrder,w.CreatTime WaybillCreateTime,w.DeliverStationID,wbs.AcceptType,wbs.CreatTime backStationCreateTime,
	            wbs.SignStatus,wbs.POSCode,wsi.FactBackAmount,wsi.FactAmount,wsi.FinancialStatus,
	            wsi.CreatTime SignInfoCreateTime,w.DistributionCode,wsi.NeedAmount,wsi.NeedBackAmount,wsi.accounttype,wsi.TransferPayType
            FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
            JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
            JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON w.BackStationID = wbs.WaybillBackStationID 
            where w.IsDelete=0 and wbs.IsDelete=0 and wbs.SignStatus='3' and w.WaybillNO=@WaybillNO";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybillno)
            };

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];

            if (table.Rows.Count == 0) return null;

            return ProjectDataRowToObject(table.Rows[0]);
        }

        public FMS_IncomeExpressReceiveFeeInfo GetExpressReceiveFeeInfoModel(long waybillno)
        {
            string sql = @"SELECT w.Sources,w.WaybillNO,w.MerchantID,w.DeliverManID,w.CreatTime WaybillCreateTime,
	            w.ReceiveStationID,w.DeliverStationID,w.DistributionCode,wsi.TransferFee,
	            wsi.DinsureFee,wsi.TransferPayType,wsi.AcceptType,null BackStationCreateTime,
                w.CustomerOrder,null POSCode 
            FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
            JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO               
            WHERE   w.IsDelete=0
                AND wsi.TransferPayType=1
                AND w.WaybillNO=@WaybillNO";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybillno)
            };

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];

            if (table.Rows.Count == 0) return null;

            return ExpressReceiveDataRowToObject(table.Rows[0]);
        }

        public FMS_IncomeExpressReceiveFeeInfo GetExpressSendFeeInfoModel(long waybillno)
        {
            string sql = @"SELECT w.Sources,w.WaybillNO,w.MerchantID,w.DeliverManID,w.CreatTime WaybillCreateTime,
	            w.ReceiveStationID,w.DeliverStationID,w.DistributionCode,wsi.TransferFee,
	            wsi.DinsureFee,wsi.TransferPayType,wbs.AcceptType,wbs.CreatTime BackStationCreateTime,w.CustomerOrder,wbs.POSCode
            FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
            JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
            JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID  
            WHERE  w.IsDelete=0
                AND wsi.TransferPayType=2
                AND w.WaybillNO=@WaybillNO ";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybillno)
            };

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];

            if (table.Rows.Count == 0) return null;

            return ExpressSendDataRowToObject(table.Rows[0]);
        }
       public FMS_IncomeExpressReceiveFeeInfo GetExpressModel(long waybillno)
        {
            string sql = @"SELECT w.Sources,w.WaybillNO,w.MerchantID,w.DeliverManID,w.CreatTime WaybillCreateTime,
	            w.ReceiveStationID,w.DeliverStationID,w.DistributionCode,wsi.TransferFee,
	            wsi.DinsureFee,wsi.TransferPayType,wsi.AcceptType,null BackStationCreateTime,
                w.CustomerOrder,null POSCode 
            FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
            JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO               
            WHERE   w.IsDelete=0
                AND w.WaybillNO=@WaybillNO";
            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybillno)
            };

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];

            if (table.Rows.Count == 0) return null;

            return ExpressSendDataRowToObject(table.Rows[0]);

        }


        public FMS_IncomeExpressReceiveFeeInfo GetExpressSendMonthlyModel(long waybillno)
        {
            string sql = @"SELECT w.Sources,w.WaybillNO,w.MerchantID,w.DeliverManID,w.CreatTime WaybillCreateTime,
	            w.ReceiveStationID,w.DeliverStationID,w.DistributionCode,wsi.TransferFee,
	            wsi.DinsureFee,wsi.TransferPayType,wbs.AcceptType,wbs.CreatTime BackStationCreateTime,w.CustomerOrder,wbs.POSCode
            FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
            JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
            JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID  
            WHERE  w.IsDelete=0
                AND wsi.TransferPayType=3
                AND w.WaybillNO=@WaybillNO ";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybillno)
            };

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];

            if (table.Rows.Count == 0) return null;

            return ExpressSendDataRowToObject(table.Rows[0]);
        }

        public DataTable GetSynInfoByWaybillNO(IList<long> waybillNos)
        {
            string sql = String.Format("select WaybillNO,AcceptType from LMS_RFD.dbo.WaybillSignInfo where WaybillNO in ({0})", DataConvert.ToDbIds(waybillNos));

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        public string GetCurrentData(long waybillNo)
        {
            string sql = @"SELECT wbs.AcceptType
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON w.BackStationID = wbs.WaybillBackStationID  
                where w.IsDelete=0 AND wbs.IsDelete=0 and wbs.SignStatus='3' AND w.WaybillNO=@WaybillNO";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybillNo)
            };

            return DataConvert.ToString(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql,parameters));
        }

        private FMS_IncomeReceiveFeeInfo ProjectDataRowToObject(DataRow row)
        {
            FMS_IncomeReceiveFeeInfo model = new FMS_IncomeReceiveFeeInfo();

            model.WaybillNO = DataConvert.ToLong(row["WaybillNo"]);
            model.WaybillType = DataConvert.ToString(row["WaybillType"]);
            model.DeliverManID = DataConvert.ToInt(row["DeliverManID"]);
            model.Sources = DataConvert.ToInt(row["Sources"]);
            model.ComeFrom = DataConvert.ToInt(row["ComeFrom"]);
            model.MerchantID = DataConvert.ToInt(row["MerchantID"]);
            model.CustomerOrder = DataConvert.ToString(row["CustomerOrder"]);
            model.WaybillCreateTime = DataConvert.ToDateTime(row["WaybillCreateTime"]);
            model.DeliverStationID = DataConvert.ToInt(row["DeliverStationID"]);
            if (DataConvert.ToString(row["AcceptType"]) == "pos机")
            {
                model.AcceptType = "POS机"; 
            }
            else
            {
                model.AcceptType = DataConvert.ToString(row["AcceptType"]); 
            }
            model.BackStationCreateTime = DataConvert.ToDateTime(row["BackStationCreateTime"]);
            model.SignStatus = DataConvert.ToString(row["SignStatus"]);
            model.POSCode = DataConvert.ToString(row["POSCode"]);
            model.FactBackAmount = DataConvert.ToDecimal(row["FactBackAmount"]);
            model.FactAmount = DataConvert.ToDecimal(row["FactAmount"]);
            model.FinancialStatus = DataConvert.ToInt(row["FinancialStatus"]);
            model.SignInfoCreateTime = DataConvert.ToDateTime(row["SignInfoCreateTime"]);
            model.DiscributionCode = DataConvert.ToString(row["DistributionCode"]);
            model.NeedAmount = DataConvert.ToDecimal(row["NeedAmount"]);
            model.NeedBackAmount = DataConvert.ToDecimal(row["NeedBackAmount"]);
            model.TransferPayType = DataConvert.ToInt(row["TransferPayType"]);
            model.AccountType = DataConvert.ToInt(row["accounttype"]);

            return model;
        }

        private FMS_IncomeExpressReceiveFeeInfo ExpressReceiveDataRowToObject(DataRow row)
        {
            FMS_IncomeExpressReceiveFeeInfo model = new FMS_IncomeExpressReceiveFeeInfo();

            model.WaybillNO = DataConvert.ToLong(row["WaybillNO"]);
            model.Sources = DataConvert.ToInt(row["Sources"]);
            model.MerchantID = DataConvert.ToInt(row["MerchantID"]);
            model.DeliverManID = DataConvert.ToInt(row["DeliverManID"]);
            model.WaybillCreatTime = DataConvert.ToDateTime(row["WaybillCreateTime"]);
            model.ReceiveStationID = DataConvert.ToInt(row["ReceiveStationID"]);
            model.DeliverStationID = DataConvert.ToInt(row["DeliverStationID"]);
            model.DistributionCode = DataConvert.ToString(row["DistributionCode"]);
            model.TransferFee = DataConvert.ToDecimal(row["TransferFee"]);
            model.DinsureFee = DataConvert.ToDecimal(row["DinsureFee"]);
            model.TransferPayType = DataConvert.ToInt(row["TransferPayType"]);
            model.AcceptType = DataConvert.ToString(row["AcceptType"]);
            model.BackStationCreateTime = DataConvert.ToDateTime(row["BackStationCreateTime"]);
            model.CustomerOrder = DataConvert.ToString(row["CustomerOrder"]);
            model.POSCode = DataConvert.ToString(row["POSCode"]);

            return model;
        }

        private FMS_IncomeExpressReceiveFeeInfo ExpressSendDataRowToObject(DataRow row)
        {
            FMS_IncomeExpressReceiveFeeInfo model = new FMS_IncomeExpressReceiveFeeInfo();

            model.WaybillNO = DataConvert.ToLong(row["WaybillNO"]);
            model.Sources = DataConvert.ToInt(row["Sources"]);
            model.MerchantID = DataConvert.ToInt(row["MerchantID"]);
            model.DeliverManID = DataConvert.ToInt(row["DeliverManID"]);
            model.WaybillCreatTime = DataConvert.ToDateTime(row["WaybillCreateTime"]);
            model.ReceiveStationID = DataConvert.ToInt(row["ReceiveStationID"]);
            model.DeliverStationID = DataConvert.ToInt(row["DeliverStationID"]);
            model.DistributionCode = DataConvert.ToString(row["DistributionCode"]);
            model.TransferFee = DataConvert.ToDecimal(row["TransferFee"]);
            model.DinsureFee = DataConvert.ToDecimal(row["DinsureFee"]);
            model.TransferPayType = DataConvert.ToInt(row["TransferPayType"]);
            model.AcceptType = DataConvert.ToString(row["AcceptType"]);
            model.BackStationCreateTime = DataConvert.ToDateTime(row["BackStationCreateTime"]);
            model.CustomerOrder = DataConvert.ToString(row["CustomerOrder"]);
            model.POSCode = DataConvert.ToString(row["POSCode"]);

            return model;
        }
    }
}
