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
using Microsoft.ApplicationBlocks.Data.Extension;

namespace RFD.FMS.DAL.QueryStatistics
{
    public class CODTransferDao : SqlServerDao, ICODTransferDao
    {
        #region ICODTransferDao 成员

        public System.Data.DataTable StatCodV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                #region 发货
                sql = @"
WITH t AS (
                --2012.05.22之前
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		ISNULL(SUM(ISNULL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计
FROM    t 
";
            #endregion
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                #region 拒收
                sql = @"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {1}
                --谁接的
                UNION ALL
                SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {1}
                --谁配送的
                UNION ALL
                SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 结算重量合计
    FROM    t
";
                #endregion
                sql = string.Format(sql, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            else
            {
                #region 上门退
                sql = @"
WITH t AS (
		        --2012.05.22之前
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--ISNULL(SUM(ISNULL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN ISNULL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计
FROM    t
";
                #endregion
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql.ToString()).Tables[0];
        }

        public System.Data.DataTable StatCod(string condition, MODEL.CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                #region 发货
                sql = @"
WITH t AS (
                --2012.05.22之前
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		ISNULL(SUM(ISNULL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计
FROM    t 

";
                #endregion
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                #region 拒收
                sql = @"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {1}
                --谁接的
                UNION ALL
                SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {1}
                --谁配送的
                UNION ALL
                SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 结算重量合计
    FROM    t
";
                #endregion
                sql = string.Format(sql, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            else
            {
                #region 上门退
                sql = @"
WITH t AS (
		        --2012.05.22之前
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--ISNULL(SUM(ISNULL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN ISNULL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计
FROM    t
";
                #endregion
                sql = string.Format(sql, condition,searchCondition.DistributionCode);
            }

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql.ToString()).Tables[0];
        }

        public System.Data.DataTable SearchCodDetailsV2(string condition, ref MODEL.PageInfo pi, MODEL.CodSearchCondition searchCondition)
        {
            string sql = string.Empty;

            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSqlV2(true);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSqlV2(true);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSqlV2(true);
                sql = string.Format(sql, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }
            SqlParameter[] parameters ={
										   new SqlParameter("@records",SqlDbType.Int),
										   new SqlParameter("@pages",SqlDbType.Int),
										   new SqlParameter("@pageSize",SqlDbType.Int),
										   new SqlParameter("@pageNo",SqlDbType.Int)
									  };
            parameters[0].Value = pi.ItemCount;
            parameters[1].Direction = ParameterDirection.Output;
            parameters[2].Value = pi.PageSize;
            parameters[3].Value = pi.CurrentPageIndex;

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters);

            pi.PageCount = (int)parameters[1].Value;

            if (ds == null || ds.Tables.Count <= 0) return null;
            
            return ds.Tables[0];
        }

        public System.Data.DataTable SearchCodDetails(string condition, ref MODEL.PageInfo pi, MODEL.CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSql(true);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSql(true);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSql(true);
                sql = string.Format(sql, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }
            SqlParameter[] parameters ={
										   new SqlParameter("@records",SqlDbType.Int),
										   new SqlParameter("@pages",SqlDbType.Int),
										   new SqlParameter("@pageSize",SqlDbType.Int),
										   new SqlParameter("@pageNo",SqlDbType.Int)
									  };
            parameters[0].Value = pi.ItemCount;
            parameters[1].Direction = ParameterDirection.Output;
            parameters[2].Value = pi.PageSize;
            parameters[3].Value = pi.CurrentPageIndex;
            DataSet ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters);
            pi.PageCount = (int)parameters[1].Value;
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public System.Data.DataTable SearchCodStatV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            string sqlDetail = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sqlDetail = GetDeliverStatSqlV2(searchCondition.DateType);
                sqlDetail = string.Format(sqlDetail, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sqlDetail = GetVisitReturnsStatSqlV2();
                sqlDetail = string.Format(sqlDetail, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sqlDetail = GetReturnsStatSqlV2();
                sqlDetail = string.Format(sqlDetail, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlDetail);

            if (ds == null || ds.Tables.Count <= 0) return null;
            
            return ds.Tables[0];
        }

        public System.Data.DataTable SearchCodStat(string condition, MODEL.CodSearchCondition searchCondition)
        {
            string sqlDetail = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sqlDetail = GetDeliverStatSql(searchCondition.DateType);
                sqlDetail = string.Format(sqlDetail, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sqlDetail = GetVisitReturnsStatSql();
                sqlDetail = string.Format(sqlDetail, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sqlDetail = GetReturnsStatSql();
                sqlDetail = string.Format(sqlDetail, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }


            DataSet ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120 ,CommandType.Text, sqlDetail);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public System.Data.DataTable SearchExprotDetailDataV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSqlV2(false);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSqlV2(false);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSqlV2(false);
                sql = string.Format(sql, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public System.Data.DataTable SearchExprotDetailData(string condition, MODEL.CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSql(false);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSql(false);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSql(false);
                sql = string.Format(sql, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql).Tables[0];
        }

        public System.Data.DataTable SearchExprotStatDataV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverStatSqlV2(searchCondition.DateType);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsStatSqlV2();
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsStatSqlV2();
                sql = string.Format(sql, condition , condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public System.Data.DataTable SearchExprotStatData(string condition, MODEL.CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverStatSql(searchCondition.DateType);
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsStatSql();
                sql = string.Format(sql, condition, searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsStatSql();
                sql = string.Format(sql, condition, condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql).Tables[0];
        }

        #endregion

        private string GetDeliverSqlV2(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            CASE WHEN isnull(t.FinalExpressCompanyID,0)=0 THEN 
            	CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
            	ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
				END            	
				ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID)
        	 END
        	 AS 末级发货仓 ,
            CASE WHEN ec4.CompanyName IS NULL  THEN
            	CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE ec3.CompanyName
				END            	
				ELSE ec4.CompanyName
            END
            AS 末级发货仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN '0' THEN '是'
              WHEN '1' THEN '否'
            END 主分仓
    FROM    t
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
");
            if (isPage)
                sb.Append(")aa WHERE   序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            return sb.ToString();
        }

        private string GetDeliverSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            CASE WHEN isnull(t.FinalExpressCompanyID,0)=0 THEN 
            	CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
            	ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
				END            	
				ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID)
        	 END
        	 AS 末级发货仓 ,
            CASE WHEN ec4.CompanyName IS NULL  THEN
            	CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE ec3.CompanyName
				END            	
				ELSE ec4.CompanyName
            END
            AS 末级发货仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN '0' THEN '是'
              WHEN '1' THEN '否'
            END 主分仓
    FROM    t
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
");
            if (isPage)
                sb.Append(")aa WHERE   序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            return sb.ToString();
        }

        private string GetReturnsSqlV2(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
            UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {1}
                --谁接的
                UNION ALL
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
            UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {1}
                --谁配送的
                UNION ALL
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
            UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
	SELECT * FROM(
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE CONVERT(NVARCHAR(20),ec5.expresscompanyid)
            END AS 入库仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.WarehouseName
                 ELSE ec5.CompanyName
            END AS 入库仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN '0' THEN '是'
              WHEN '1' THEN '否'
            END 主分仓
    FROM    t
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
			LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
	) aa
");

            if (isPage)
                sb.Append(" WHERE  序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            return sb.ToString();
        }

        private string GetReturnsSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
            UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {1}
                --谁接的
                UNION ALL
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
            UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {1}
                --谁配送的
                UNION ALL
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
            UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
	SELECT * FROM(
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE CONVERT(NVARCHAR(20),ec5.expresscompanyid)
            END AS 入库仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.WarehouseName
                 ELSE ec5.CompanyName
            END AS 入库仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN '0' THEN '是'
              WHEN '1' THEN '否'
            END 主分仓
    FROM    t
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
			LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
	) aa
");

            if (isPage)
                sb.Append(" WHERE  序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            return sb.ToString();
        }

        private string GetVisitReturnsSqlV2(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
               --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            0.00 AS 已退款 ,--ISNULL(t.BackAmount, 0)
            ISNULL(t.NeedBackAmount, 0) AS 应退款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE CONVERT(NVARCHAR(20),ec5.expresscompanyid)
            END AS 入库仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.WarehouseName
                 ELSE ec5.CompanyName
            END AS 入库仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN '0' THEN '是'
              WHEN '1' THEN '否'
            END 主分仓
    FROM    t
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
			LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
");
            if (isPage)
                sb.Append(")aa WHERE   序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            return sb.ToString();
        }

        private string GetVisitReturnsSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
               --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            0.00 AS 已退款 ,--ISNULL(t.BackAmount, 0)
            ISNULL(t.NeedBackAmount, 0) AS 应退款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid)
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE CONVERT(NVARCHAR(20),ec5.expresscompanyid)
            END AS 入库仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.WarehouseName
                 ELSE ec5.CompanyName
            END AS 入库仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN '0' THEN '是'
              WHEN '1' THEN '否'
            END 主分仓
    FROM    t
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
			LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
            LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
");
            if (isPage)
                sb.Append(")aa WHERE   序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            return sb.ToString();
        }

        private string GetDeliverStatSqlV2(string dateType)
        {
            StringBuilder sb = new StringBuilder();
            if (dateType == "1")
            {
                #region 出库时间查询
                sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                 --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.DeliverDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END AS 发货仓库名称 ,
                        mbi.MerchantName AS 商家
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.DeliverDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END");
                #endregion
            }
            else
            {
                #region 接单时间查询
                sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                 --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.RfdAcceptDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
							 THEN wh.WarehouseName
							 ELSE ec3.CompanyName
						END AS 发货仓库名称 ,
                        mbi.MerchantName AS 商家 
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.RfdAcceptDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
						CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName ELSE ec3.CompanyName END");
                #endregion
            }

            return sb.ToString();
        }

        private string GetDeliverStatSql(string dateType)
        {
            StringBuilder sb = new StringBuilder();
            if (dateType == "1")
            {
                #region 出库时间查询
                sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                 --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.DeliverDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END AS 发货仓库名称 ,
                        mbi.MerchantName AS 商家
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.DeliverDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END");
                #endregion
            }
            else
            {
                #region 接单时间查询
                sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                 --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.RfdAcceptDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
							 THEN wh.WarehouseName
							 ELSE ec3.CompanyName
						END AS 发货仓库名称 ,
                        mbi.MerchantName AS 商家 
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.RfdAcceptDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
						CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName ELSE ec3.CompanyName END");
                #endregion
            }

            return sb.ToString();
        }

        private string GetVisitReturnsStatSqlV2()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59'  {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        COUNT(CASE WHEN ISNULL(t.NeedBackAmount, 0) > 0
                                   THEN t.NeedBackAmount
                              END) AS 应退订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)), 0) AS 应退款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        mbi.MerchantName AS 商家
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END");

            return sb.ToString();
        }

        private string GetVisitReturnsStatSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59'  {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{1}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        COUNT(CASE WHEN ISNULL(t.NeedBackAmount, 0) > 0
                                   THEN t.NeedBackAmount
                              END) AS 应退订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)), 0) AS 应退款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        mbi.MerchantName AS 商家 
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END");

            return sb.ToString();
        }

        private string GetReturnsStatSqlV2()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi (NOLOCK)
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {1}
                UNION ALL
                --谁接的
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {1}
                UNION ALL
                --谁配送的
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        mbi.MerchantName AS 商家 
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
						LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END");

            return sb.ToString();
        }

        private string GetReturnsStatSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (NOLOCK)
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {1}
                UNION ALL
                --谁接的
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {1}
                UNION ALL
                --谁配送的
                SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        mbi.MerchantName AS 商家 
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
						LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END");

            return sb.ToString();
        }

        private string GetPageSql()
        {
            return @"IF ( @records IS NULL
     OR @records = 0
   ) 
    BEGIN
        SET @records = 0
        SET @pages = 0
        RETURN
    END
SET @pages = @records / @pageSize
IF ( @records % @pageSize > 0 ) 
    SET @pages = @pages + 1
																	
DECLARE @rowStart INT
SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1 ;";
        }
    }
}
