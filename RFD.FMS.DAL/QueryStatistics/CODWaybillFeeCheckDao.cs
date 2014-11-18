using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.QueryStatistics;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.QueryStatistics
{
    public class CODWaybillFeeCheckDao : SqlServerDao, ICODWaybillFeeCheckDao
    {
        public DataTable GetCODDeliveryDaily(string startDate, string stopDate)
        {
            string sql = @"SELECT ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,SUM(ISNULL(fci.NeedPayAmount,0)) Fee FROM LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
                    inner join RFD_PMS.dbo.ExpressCompany ecp(nolock) on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join RFD_PMS.dbo.ExpressCompany ecp1(nolock) on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.DeliverTime>=@Start 
	                AND fci.DeliverTime<@Stop
	                AND fci.WaybillType IN (0,1) 
	                AND fci.Flag=1 
	                AND fci.OperateType=1
                   -- AND ecp.IsDeleted=0
                group by ecp1.ExpressCompanyOldID,ecp1.CompanyName";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Start",startDate),
                new SqlParameter("@Stop",stopDate)
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetCODDeliveryDailyByStationID(string startDate, string stopDate, int StationID)
        {
            string sql = @"SELECT fci.WaybillNO ,ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,ISNULL(fci.NeedPayAmount,0) Fee FROM LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
                    inner join RFD_PMS.dbo.ExpressCompany ecp(nolock) on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join RFD_PMS.dbo.ExpressCompany ecp1(nolock) on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.DeliverTime>=@Start 
	                AND fci.DeliverTime<@Stop
	                AND fci.WaybillType IN (0,1) 
	                AND fci.Flag=1 
	                AND fci.OperateType=1
                   -- AND ecp.IsDeleted=0
                    AND ecp1.ExpressCompanyOldID=@StationID ";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Start",startDate),
                new SqlParameter("@Stop",stopDate),
                new SqlParameter("@StationID",StationID)
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetCODReturnsDaily(string startDate, string stopDate)
        {
            string sql = @"SELECT ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,SUM(ISNULL(fci.NeedPayAmount,0)) Fee FROM LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
	                inner join RFD_PMS.dbo.ExpressCompany ecp(nolock) on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join RFD_PMS.dbo.ExpressCompany ecp1(nolock) on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.ReturnTime>=@Start
	                AND fci.ReturnTime<@Stop
	                AND fci.WaybillType IN (0,1)
	                AND fci.Flag=0
	                AND fci.OperateType=4
                   -- AND ecp.IsDeleted=0
                group by ecp1.ExpressCompanyOldID,ecp1.CompanyName";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Start",startDate),
                new SqlParameter("@Stop",stopDate)
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetCODReturnsDailyByStationID(string startDate, string stopDate, int StationID)
        {
            string sql = @"SELECT fci.WaybillNo, ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,ISNULL(fci.NeedPayAmount,0) Fee FROM LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
	                inner join RFD_PMS.dbo.ExpressCompany ecp(nolock) on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join RFD_PMS.dbo.ExpressCompany ecp1(nolock) on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
                WHERE fci.MerchantID=8 
	                AND fci.ReturnTime>=@Start
	                AND fci.ReturnTime<@Stop
	                AND fci.WaybillType IN (0,1)
	                AND fci.Flag=0
	                AND fci.OperateType=4
                   -- AND ecp.IsDeleted=0
                    AND ecp1.ExpressCompanyOldID=@StationID";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Start",startDate),
                new SqlParameter("@Stop",stopDate),
                new SqlParameter("@StationID",StationID)
            };
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];

        }
        public DataTable GetCODVisitDaily(string startDate, string stopDate)
        {
            string sql = @"
	            SELECT ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,sum(fci.NeedBackAmount) Fee FROM LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
		            inner join RFD_PMS.dbo.ExpressCompany ecp(nolock) on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join RFD_PMS.dbo.ExpressCompany ecp1(nolock) on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
	            WHERE fci.MerchantID=8 
		            AND fci.ReturnTime>=@Start
		            AND fci.ReturnTime<@Stop
		            AND fci.WaybillType IN (1,2)
		            AND fci.Flag=1
		            AND fci.OperateType in (1,3)
		           -- AND ecp.IsDeleted=0
                group by ecp1.ExpressCompanyOldID,ecp1.CompanyName";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Start",startDate),
                new SqlParameter("@Stop",stopDate)
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
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
	            SELECT fci.WaybillNo,ecp1.ExpressCompanyOldID ID,ecp1.CompanyName Name,fci.NeedBackAmount Fee FROM LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
		            inner join RFD_PMS.dbo.ExpressCompany ecp(nolock) on fci.DeliverStationID = ecp.ExpressCompanyID
                    inner join RFD_PMS.dbo.ExpressCompany ecp1(nolock) on ecp.TopCODCompanyID = ecp1.ExpressCompanyID
	            WHERE fci.MerchantID=8 
		            AND fci.ReturnTime>=@Start
		            AND fci.ReturnTime<@Stop
		            AND fci.WaybillType IN (1,2)
		            AND fci.Flag=1
		            AND fci.OperateType in (1,3)
		           -- AND ecp.IsDeleted=0
                    AND ecp1.ExpressCompanyOldID=@StationID ";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Start",startDate),
                new SqlParameter("@Stop",stopDate),
                new SqlParameter("@StationID",StationID)
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
    }
}
