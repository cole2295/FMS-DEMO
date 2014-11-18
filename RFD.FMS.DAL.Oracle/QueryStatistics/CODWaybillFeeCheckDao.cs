using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.QueryStatistics;
using System.Data.SqlClient;
using System.Data;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.Oracle.QueryStatistics
{
    public class CODWaybillFeeCheckDao : OracleDao, ICODWaybillFeeCheckDao
    {
        public DataTable GetCODDeliveryDaily(string startDate, string stopDate)
        {
            string sql = @"SELECT ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,SUM(NVL(fci.NeedPayAmount,0)) Fee FROM FMS_CODBaseInfo fci
                    inner join PS_PMS.ExpressCompany ecp on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join PS_PMS.ExpressCompany ecp1 on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.DeliverTime>=:Start1 
	                AND fci.DeliverTime<:Stop1
	                AND fci.WaybillType IN (0,1) 
	                AND fci.Flag=1 
	                AND fci.OperateType=1
                    --AND ecp.IsDeleted=0
                group by ecp1.ExpressCompanyOldID,ecp1.CompanyName";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":Start1",OracleDbType.Date){Value = DataConvert.ToDateTime(startDate)},
                new OracleParameter(":Stop1",OracleDbType.Date){Value = DataConvert.ToDateTime(stopDate)}
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetCODDeliveryDailyByStationID(string startDate, string stopDate, int StationID)
        {
            string sql = @"SELECT fci.WaybillNO ,ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,NVL(fci.NeedPayAmount,0) Fee FROM FMS_CODBaseInfo fci
                    inner join PS_PMS.ExpressCompany ecp on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join PS_PMS.ExpressCompany ecp1 on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.DeliverTime>=:Start1 
	                AND fci.DeliverTime<:Stop1
	                AND fci.WaybillType IN (0,1) 
	                AND fci.Flag=1 
	                AND fci.OperateType=1
                   -- AND ecp.IsDeleted=0
                    AND ecp1.ExpressCompanyOldID=:StationID ";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":Start1",OracleDbType.Date){Value = DataConvert.ToDateTime(startDate)},
                new OracleParameter(":Stop1",OracleDbType.Date){Value = DataConvert.ToDateTime(stopDate)},
                new OracleParameter(":StationID",OracleDbType.Int32){Value = StationID}
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetCODReturnsDaily(string startDate, string stopDate)
        {
            string sql = @"SELECT ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,SUM(NVL(fci.NeedPayAmount,0)) Fee FROM FMS_CODBaseInfo fci
	                inner join PS_PMS.ExpressCompany ecp on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join PS_PMS.ExpressCompany ecp1 on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.ReturnTime>=:Start1
	                AND fci.ReturnTime<:Stop1
	                AND fci.WaybillType IN (0,1)
	                AND fci.Flag=0
	                AND fci.OperateType=4
                    --AND ecp.IsDeleted=0
                group by ecp1.ExpressCompanyOldID,ecp1.CompanyName";

            OracleParameter[] parameters = 
            {
               new OracleParameter(":Start1",OracleDbType.Date){Value = DataConvert.ToDateTime(startDate)},
                new OracleParameter(":Stop1",OracleDbType.Date){Value = DataConvert.ToDateTime(stopDate)}
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetCODReturnsDailyByStationID(string startDate, string stopDate, int StationID)
        {
            string sql = @"SELECT fci.WaybillNo, ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,NVL(fci.NeedPayAmount,0) Fee FROM FMS_CODBaseInfo fci
	                inner join PS_PMS.ExpressCompany ecp on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join PS_PMS.ExpressCompany ecp1 on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.ReturnTime>=:Start1
	                AND fci.ReturnTime<:Stop1
	                AND fci.WaybillType IN (0,1)
	                AND fci.Flag=0
	                AND fci.OperateType=4
                    --AND ecp.IsDeleted=0
                    AND ecp1.ExpressCompanyOldID=:StationID";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":Start1",OracleDbType.Date){Value = DataConvert.ToDateTime(startDate)},
                new OracleParameter(":Stop1",OracleDbType.Date){Value = DataConvert.ToDateTime(stopDate)},
                new OracleParameter(":StationID",OracleDbType.Int32){Value = StationID}
            };
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];

        }
        public DataTable GetCODVisitDaily(string startDate, string stopDate)
        {
            string sql = @"
	            SELECT ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,sum(fci.NeedBackAmount) Fee FROM FMS_CODBaseInfo fci
		            inner join PS_PMS.ExpressCompany ecp on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join PS_PMS.ExpressCompany ecp1 on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
	            WHERE fci.MerchantID=8 
		            AND fci.ReturnTime>=:Start1
		            AND fci.ReturnTime<:Stop1
		            AND fci.WaybillType IN (1,2)
		            AND fci.Flag=1
		            AND fci.OperateType in (1,3)
		            --AND ecp.IsDeleted=0
                group by ecp1.ExpressCompanyOldID,ecp1.CompanyName";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":Start1",OracleDbType.Date){Value = DataConvert.ToDateTime(startDate)},
                new OracleParameter(":Stop1",OracleDbType.Date){Value = DataConvert.ToDateTime(stopDate)}
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        public DataTable GetCODVisitDailyByStationID(string startDate, string stopDate,int StationID)
        {
             string sql = @"
	            SELECT fci.WaybillNo,ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,fci.NeedBackAmount Fee FROM FMS_CODBaseInfo fci
		            inner join PS_PMS.ExpressCompany ecp on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join PS_PMS.ExpressCompany ecp1 on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
	            WHERE fci.MerchantID=8 
		            AND fci.ReturnTime>=:Start1
		            AND fci.ReturnTime<:Stop1
		            AND fci.WaybillType IN (1,2)
		            AND fci.Flag=1
		            AND fci.OperateType in (1,3)
		            --AND ecp.IsDeleted=0
                    AND ecp1.ExpressCompanyOldID=:StationID ";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":Start1",OracleDbType.Date){Value = DataConvert.ToDateTime(startDate)},
                new OracleParameter(":Stop1",OracleDbType.Date){Value = DataConvert.ToDateTime(stopDate)},
                new OracleParameter(":StationID",OracleDbType.Int32){Value = StationID}
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
    }
}
