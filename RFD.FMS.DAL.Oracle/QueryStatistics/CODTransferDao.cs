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

namespace RFD.FMS.DAL.Oracle.QueryStatistics
{
    public class CODTransferDao : OracleDao, ICODTransferDao
    {
        #region ICODTransferDao 成员

        public System.Data.DataTable StatCodV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 中没有查询V2");
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
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{1}' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi 
               JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		NVL(SUM(NVL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		NVL(SUM(NVL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计
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
                SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     FMS_CODBaseInfo fcbi
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {1}
                --谁接的
                UNION ALL
                SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     FMS_CODBaseInfo fcbi
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {1}
                --谁配送的
                UNION ALL
                SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5)  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight
			   FROM     FMS_CODBaseInfo fcbi 
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 结算重量合计
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
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{1}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight
               FROM     FMS_CODBaseInfo fcbi
				JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--NVL(SUM(NVL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN NVL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		NVL(SUM(NVL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计
FROM    t
";
                #endregion
                sql = string.Format(sql, condition,searchCondition.DistributionCode);
            }

            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql.ToString()).Tables[0];
        }

        public System.Data.DataTable SearchCodDetailsV2(string condition, ref MODEL.PageInfo pi, MODEL.CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 中没有查询V2");
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
            OracleParameter[] parameters ={
										   new OracleParameter(":pageStr",OracleDbType.Decimal),
										   new OracleParameter(":pageEnd",OracleDbType.Decimal)
									  };
            parameters[0].Value = pi.CurrentPageStartRowNum;
            parameters[1].Value = pi.CurrentPageEndRowNum;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql, parameters);
            pi.PageCount = (int)parameters[1].Value;
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public System.Data.DataTable SearchCodStatV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 中没有查询V2");
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


            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sqlDetail);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public System.Data.DataTable SearchExprotDetailDataV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 中没有导出V2");
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

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql).Tables[0];
        }

        public System.Data.DataTable SearchExprotStatDataV2(string condition, MODEL.CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 中没有导出V2");
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

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql).Tables[0];
        }

        #endregion

        private string GetDeliverSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{1}' {0}
                --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )");
            sb.Append(@"
    SELECT  ROWNUM AS 序号,
			mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位 ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.TotalAmount, 0) AS 订单总价 ,
            NVL(t.PaidAmount, 0) AS 已收款 ,
            NVL(t.NeedPayAmount, 0) AS 应收款 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE cast(ec3.expresscompanyid as varchar2(20))
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            CASE WHEN NVL(t.FinalExpressCompanyID,0)=0 THEN 
            	CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
            	ELSE cast(ec3.expresscompanyid as varchar2(20))
				END            	
				ELSE cast(t.FinalExpressCompanyID as varchar2(20))
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
              WHEN 0 THEN '是'
              WHEN 1 THEN '否'
            END 主分仓
    FROM    t
            JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN ExpressCompany ec2 ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
");
            if (isPage)
                sb.Append(" WHERE   ROWNUM BETWEEN :pageStr AND :pageEnd");

            return sb.ToString();
        }

        private string GetReturnsSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,cast(fcbi.ReturnExpressCompanyID as varchar(20)) as ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
            UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID as varchar(20)) END    ELSE cast(fcbi.FinalExpressCompanyID as varchar(20)) END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo fcbi
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {1}
                --谁接的
                UNION ALL
                SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,cast(fcbi.ReturnExpressCompanyID as varchar(20)) as ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
            UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID as varchar(20)) END    ELSE cast(fcbi.FinalExpressCompanyID as varchar(20)) END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo fcbi
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {1}
                --谁配送的
                UNION ALL
                SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,cast(fcbi.ReturnExpressCompanyID as varchar(20)) as ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
            UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID as varchar(20)) END    ELSE cast(fcbi.FinalExpressCompanyID as varchar(20)) END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo fcbi
			            JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
	SELECT * FROM(
    SELECT  ROWNUM AS 序号,
			mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位  ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.TotalAmount, 0) AS 订单总价 ,
            NVL(t.PaidAmount, 0) AS 已收款 ,
            NVL(t.NeedPayAmount, 0) AS 应收款 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE cast(ec3.expresscompanyid as varchar2(20))
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE cast(ec5.expresscompanyid as varchar2(20))
            END AS 入库仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.WarehouseName
                 ELSE ec5.CompanyName
            END AS 入库仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN 0 THEN '是'
              WHEN 1 THEN '否'
            END 主分仓
    FROM    t
            JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN ExpressCompany ec2 ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
			LEFT JOIN Warehouse wh1 ON wh1.Warehouseid = t.ReturnWareHouseID
            LEFT JOIN ExpressCompany ec5 ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
	) aa
");

            if (isPage)
                sb.Append(" WHERE   ROWNUM BETWEEN :pageStr AND :pageEnd");

            return sb.ToString();
        }

        private string GetVisitReturnsSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
               --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{1}' {0}
                --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )");
            sb.Append(@"
    SELECT  ROWNUM AS 序号,
			mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位  ,
            ec2.CompanyName AS 实际配送站 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.TotalAmount, 0) AS 订单总价 ,
            0.00 AS 已退款 ,--NVL(t.BackAmount, 0)
            NVL(t.NeedBackAmount, 0) AS 应退款 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE cast(ec3.expresscompanyid as varchar2(20))
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE cast(ec5.expresscompanyid as varchar2(20))
            END AS 入库仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.WarehouseName
                 ELSE ec5.CompanyName
            END AS 入库仓名称 ,
            CASE t.WaybillType
              WHEN '1' THEN '是'
              ELSE '否'
            END 上门换 ,
            CASE wh.IsMain
              WHEN 0 THEN '是'
              WHEN 1 THEN '否'
            END 主分仓
    FROM    t
            JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN ExpressCompany ec2 ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
			LEFT JOIN Warehouse wh1 ON wh1.Warehouseid = t.ReturnWareHouseID
            LEFT JOIN ExpressCompany ec5 ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
");
            if (isPage)
                sb.Append(" WHERE   ROWNUM BETWEEN :pageStr AND :pageEnd");

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
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{1}' {0}
                 --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY t.DeliverDate DESC ) AS 序号 ,
                        t.DeliverDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位  ,
                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END AS 发货仓库名称
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  t.DeliverDate ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
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
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                 --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{1}' {0}
                 --谁配送的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.NeedPayAmount
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY t.RfdAcceptDate DESC ) AS 序号 ,
                        t.RfdAcceptDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位 ,
                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
							 THEN wh.WarehouseName
							 ELSE ec3.CompanyName
						END AS 发货仓库名称
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  t.RfdAcceptDate ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END  ,
						CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName ELSE ec3.CompanyName END");
                #endregion
            }

            return sb.ToString();
        }

        private string GetVisitReturnsStatSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss')  {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{1}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedBackAmount,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{1}' AND ec.DistributionCode='{1}' {0}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY t.ReturnDate DESC ) AS 序号 ,
                        t.ReturnDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位 ,
                        COUNT(CASE WHEN NVL(t.NeedBackAmount, 0) > 0
                                   THEN t.NeedBackAmount
                              END) AS 应退订单量 ,
                        NVL(SUM(NVL(t.NeedBackAmount, 0)), 0) AS 应退款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN Warehouse wh1 ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN ExpressCompany ec5 ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  t.ReturnDate ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
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
                SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,cast(fcbi.ReturnExpressCompanyID as varchar(20)) as ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
			UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,trunc(fcbi.CreateTime) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID as varchar(20)) END    ELSE cast(fcbi.FinalExpressCompanyID as varchar(20)) END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo fcbi
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {1}
                UNION ALL
                --谁接的
                SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,cast(fcbi.ReturnExpressCompanyID as varchar(20)) as ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
			UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,trunc(fcbi.CreateTime) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID as varchar(20)) END    ELSE cast(fcbi.FinalExpressCompanyID as varchar(20)) END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo fcbi
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {1}
                UNION ALL
                --谁配送的
                SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,cast(fcbi.ReturnExpressCompanyID as varchar(20)) as ReturnExpressCompanyID ,fcbi.ReturnWareHouseID
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               WHERE    ( 1 = 1 ) AND fcbi.OperateType NOT in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
			UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,trunc(fcbi.CreateTime) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.NeedPayAmount ,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID as varchar(20)) END    ELSE cast(fcbi.FinalExpressCompanyID as varchar(20)) END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID
			   FROM     FMS_CODBaseInfo fcbi
			            JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {1}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY t.ReturnDate DESC ) AS 序号 ,
                        t.ReturnDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位  ,
                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
						LEFT JOIN Warehouse wh1 ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN ExpressCompany ec5 ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  t.ReturnDate ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END");

            return sb.ToString();
        }
    }
}
