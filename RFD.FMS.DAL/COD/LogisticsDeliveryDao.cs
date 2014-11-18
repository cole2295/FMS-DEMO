using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.ApplicationBlocks.Data.Extension;
using RFD.FMS.AdoNet.DbBase;
using System.Data.SqlClient;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.COD;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.COD
{
    public class LogisticsDeliveryDao : SqlServerDao, ILogisticsDeliveryDao
    {
        public DataTable SearchCodDetails(string condition, ref PageInfo pi, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;

            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSql(true);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSql(true);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSql(true);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            SqlParameter[] parameters =
            {
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

        public DataTable SearchCodDetailsV2(string condition, ref PageInfo pi, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSqlV2(true);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSqlV2(true);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSqlV2(true);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
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
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        private string GetDeliverSqlV2(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM  FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                --谁配送的
                UNION ALL
                SELECT  fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid) END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName ELSE ec3.CompanyName END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
             CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE
            	CASE WHEN isnull(t.FinalExpressCompanyID,0)=0 THEN
            	    CONVERT(NVARCHAR(20),ec3.expresscompanyid)
				 ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID) END
        	 END AS 末级发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
            	 ec3.CompanyName
				 ELSE ec4.CompanyName END
            END AS 末级发货仓名称 ,
            CASE t.WaybillType WHEN '1' THEN '是' ELSE '否' END 上门换 ,
            CASE wh.IsMain WHEN '0' THEN '是'  WHEN '1' THEN '否' END 主分仓
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
                sb.Append(") aa WHERE   序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

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
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM  LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                --谁接的
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                --谁配送的
                UNION ALL
                SELECT  fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE CONVERT(NVARCHAR(20),ec3.expresscompanyid) END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName ELSE ec3.CompanyName END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
             CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE
            	CASE WHEN isnull(t.FinalExpressCompanyID,0)=0 THEN
            	    CONVERT(NVARCHAR(20),ec3.expresscompanyid)
				 ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID) END
        	 END AS 末级发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
            	 ec3.CompanyName
				 ELSE ec4.CompanyName END
            END AS 末级发货仓名称 ,
            CASE t.WaybillType WHEN '1' THEN '是' ELSE '否' END 上门换 ,
            CASE wh.IsMain WHEN '0' THEN '是'  WHEN '1' THEN '否' END 主分仓
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
                sb.Append(") aa WHERE   序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            return sb.ToString();
        }

        private string GetReturnsSqlV2(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            if (isPage)
                sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
            --2012.05.22之前查询
            SELECT  fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
            UNION ALL
			SELECT	fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {2}
            UNION ALL
            --谁接的单
            SELECT  fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {0}
            UNION ALL
			SELECT	fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {2}
            UNION ALL
            --谁配送的单
            SELECT  fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
            UNION ALL
			SELECT	fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
	SELECT * FROM(
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
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
            --2012.05.22之前查询
            SELECT  fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
            UNION ALL
			SELECT	fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {2}
            UNION ALL
            --谁接的单
            SELECT  fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {0}
            UNION ALL
			SELECT	fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {2}
            UNION ALL
            --谁配送的单
            SELECT  fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
            UNION ALL
			SELECT	fcbi.ID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
	SELECT * FROM(
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            ISNULL(t.PaidAmount, 0) AS 已收款 ,
            ISNULL(t.NeedPayAmount, 0) AS 应收款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
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
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            0.00 AS 已退款 ,--ISNULL(t.BackAmount, 0)
            ISNULL(t.NeedBackAmount, 0) AS 应退款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
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
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )");
            if (isPage)
                sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
			mbi.MerchantName AS 商家 ,
            结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.TotalAmount, 0) AS 订单总价 ,
            0.00 AS 已退款 ,--ISNULL(t.BackAmount, 0)
            ISNULL(t.NeedBackAmount, 0) AS 应退款 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
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

        public DataTable StatCodV2(string condition, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = @"
WITH t AS (
                --2012.05.22之前
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		ISNULL(SUM(ISNULL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计,
        ISNULL(SUM(ISNULL(t.Fare, 0)),0) AS 配送费合计,
        ISNULL(SUM(ISNULL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t ";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = @"
WITH    t AS ( 
               --2012.05.22之前查询
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {2}
                UNION ALL
               --谁接的单
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {2}
                UNION ALL
               --谁配送的
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0{1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 结算重量合计,
            ISNULL(SUM(ISNULL(t.Fare, 0)),0) AS 配送费合计,
            ISNULL(SUM(ISNULL(t.ProtectedPrice, 0)),0) AS 保价金额合计
    FROM    t
";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            else
            {
                sql = @"
WITH t AS (
		        --2012.05.22之前
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--ISNULL(SUM(ISNULL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN ISNULL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计,
        ISNULL(SUM(ISNULL(t.Fare, 0)),0) AS 配送费合计,
        ISNULL(SUM(ISNULL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql.ToString()).Tables[0];
        }

        public DataTable StatCod(string condition, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = @"
WITH t AS (
                --2012.05.22之前
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		ISNULL(SUM(ISNULL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计,
        ISNULL(SUM(ISNULL(t.Fare, 0)),0) AS 配送费合计,
        ISNULL(SUM(ISNULL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t ";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = @"
WITH    t AS ( 
               --2012.05.22之前查询
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {2}
                UNION ALL
               --谁接的单
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {2}
                UNION ALL
               --谁配送的
               SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK ) 
                JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0{1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 结算重量合计,
            ISNULL(SUM(ISNULL(t.Fare, 0)),0) AS 配送费合计,
            ISNULL(SUM(ISNULL(t.ProtectedPrice, 0)),0) AS 保价金额合计
    FROM    t
";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            else
            {
                sql = @"
WITH t AS (
		        --2012.05.22之前
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		ISNULL(SUM(ISNULL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--ISNULL(SUM(ISNULL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN ISNULL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		ISNULL(SUM(ISNULL(t.AccountWeight, 0)),0) AS 结算重量合计,
        ISNULL(SUM(ISNULL(t.Fare, 0)),0) AS 配送费合计,
        ISNULL(SUM(ISNULL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql.ToString()).Tables[0];
        }

        public DataTable SearchCodStatV2(string condition, CodSearchCondition searchCondition)
        {
            string sqlDetail = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sqlDetail = GetDeliverStatSqlV2(searchCondition.DateType, searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sqlDetail = string.Format(sqlDetail, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sqlDetail = GetVisitReturnsStatSqlV2(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sqlDetail = string.Format(sqlDetail, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sqlDetail = GetReturnsStatSqlV2(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sqlDetail = string.Format(sqlDetail, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }


            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlDetail);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchCodStat(string condition, CodSearchCondition searchCondition)
        {
            string sqlDetail = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sqlDetail = GetDeliverStatSql(searchCondition.DateType, searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sqlDetail = string.Format(sqlDetail, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sqlDetail = GetVisitReturnsStatSql(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sqlDetail = string.Format(sqlDetail, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sqlDetail = GetReturnsStatSql(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sqlDetail = string.Format(sqlDetail, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }


            DataSet ds = Microsoft.ApplicationBlocks.Data.Extension.SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sqlDetail);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        private string GetDeliverStatSqlV2(string dateType, bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            if (dateType == "1")
            {
                sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.DeliverDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType 分配区域 ,");
                if (IsCod)
                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
                        	ELSE CONVERT(NVARCHAR(20),t.ExpressCompanyID) END
                        	ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID) END
                         AS 发货仓库 ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END AS 发货仓库名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
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
						t.Warehouseid,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType ,");
                if (IsCod)
                    sb.Append(@"t.IsCOD ,");
                sb.Append(@"                        
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
                        	ELSE CONVERT(NVARCHAR(20),t.ExpressCompanyID) END
                        	ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID) END,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END,
                        wh.IsMain ,
                        t.WaybillType");
            }
            else
            {
                sb.Append(@"
WITH    t AS ( 
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59'  {0}
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.RfdAcceptDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType 分配区域 ,");
                if (IsCod)
                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
							ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
						END AS 发货仓库 ,
						CASE WHEN t.MerchantID IN ( 8, 9 )
							 THEN wh.WarehouseName
							 ELSE ec3.CompanyName
						END AS 发货仓库名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
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
						t.Warehouseid,
                        t.FinalExpressCompanyID,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType ,");
                if (IsCod)
                    sb.Append(@"t.IsCOD ,");
                sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END ,
						CASE WHEN t.MerchantID IN ( 8, 9 )
                         THEN wh.WarehouseName
                         ELSE ec3.CompanyName
                        END,
                        wh.IsMain ,
                        t.WaybillType");
            }

            return sb.ToString();
        }

        private string GetDeliverStatSql(string dateType, bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            if (dateType == "1")
            {
                sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.DeliverDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType 分配区域 ,");
                if (IsCod)
                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
                        	ELSE CONVERT(NVARCHAR(20),t.ExpressCompanyID) END
                        	ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID) END
                         AS 发货仓库 ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END AS 发货仓库名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
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
						t.Warehouseid,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType ,");
                if (IsCod)
                    sb.Append(@"t.IsCOD ,");
                sb.Append(@"                        
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
                        	ELSE CONVERT(NVARCHAR(20),t.ExpressCompanyID) END
                        	ELSE CONVERT(NVARCHAR(20),t.FinalExpressCompanyID) END,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END,
                        wh.IsMain ,
                        t.WaybillType");
            }
            else
            {
                sb.Append(@"
WITH    t AS ( 
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59'  {0}
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.RfdAcceptDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType 分配区域 ,");
                if (IsCod)
                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
							ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
						END AS 发货仓库 ,
						CASE WHEN t.MerchantID IN ( 8, 9 )
							 THEN wh.WarehouseName
							 ELSE ec3.CompanyName
						END AS 发货仓库名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
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
						t.Warehouseid,
                        t.FinalExpressCompanyID,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType ,");
                if (IsCod)
                    sb.Append(@"t.IsCOD ,");
                sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END ,
						CASE WHEN t.MerchantID IN ( 8, 9 )
                         THEN wh.WarehouseName
                         ELSE ec3.CompanyName
                        END,
                        wh.IsMain ,
                        t.WaybillType");
            }

            return sb.ToString();
        }

        private string GetVisitReturnsStatSqlV2(bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
            --2012.05.22之前的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
            UNION ALL
            --谁接的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
            UNION ALL
            --谁配送的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType 分配区域 ,");
            if (IsCod)
                sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
            sb.Append(@"
                        COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        0.00 AS 已退款 ,--ISNULL(SUM(ISNULL(t.BackAmount, 0)), 0)
                        ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)), 0) AS 应退款 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) ELSE
								CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END END AS 发货仓库 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN ec4.CompanyName ELSE 
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END END AS 发货仓库名称 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END AS 入库仓 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
						t.Warehouseid,
                        t.FinalExpressCompanyID,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        t.ReturnWareHouseID,
                        ec4.CompanyName,
                        ec5.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType ,");
            if (IsCod)
                sb.Append(@"t.IsCOD ,");
            sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END ,
                        wh.IsMain ,
                        t.WaybillType");

            return sb.ToString();
        }

        private string GetVisitReturnsStatSql(bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
            --2012.05.22之前的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
            UNION ALL
            --谁接的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{2}' {0}
            UNION ALL
            --谁配送的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType 分配区域 ,");
            if (IsCod)
                sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
            sb.Append(@"
                        COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        0.00 AS 已退款 ,--ISNULL(SUM(ISNULL(t.BackAmount, 0)), 0)
                        ISNULL(SUM(ISNULL(t.NeedBackAmount, 0)), 0) AS 应退款 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) ELSE
								CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END END AS 发货仓库 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN ec4.CompanyName ELSE 
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END END AS 发货仓库名称 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END AS 入库仓 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
						t.Warehouseid,
                        t.FinalExpressCompanyID,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        t.ReturnWareHouseID,
                        ec4.CompanyName,
                        ec5.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType ,");
            if (IsCod)
                sb.Append(@"t.IsCOD ,");
            sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END ,
                        wh.IsMain ,
                        t.WaybillType");

            return sb.ToString();
        }

        private string GetReturnsStatSqlV2(bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
            --2012.05.22之前查询
            SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {2}
            UNION ALL
            --谁接的单
            SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {2}
            UNION ALL
            --谁配送的
            SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59'  AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,ael.AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo AS fcbi ( NOLOCK )
			           JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType 分配区域 ,");
            if (IsCod)
                sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
            sb.Append(@"
                        COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) ELSE
								CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END END AS 发货仓库 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN ec4.CompanyName ELSE 
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END END AS 发货仓库名称 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END AS 入库仓 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
						LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
						t.Warehouseid,
                        t.FinalExpressCompanyID,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        t.ReturnWareHouseID,
                        ec4.CompanyName,
                        ec5.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType ,");
            if (IsCod)
                sb.Append(@"t.IsCOD ,");
            sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END ,
                        wh.IsMain ,
                        t.WaybillType");

            return sb.ToString();
        }

        private string GetReturnsStatSql(bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
            --2012.05.22之前查询
            SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<='2012-05-22 23:59:59' {2}
            UNION ALL
            --谁接的单
            SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode='{3}' {2}
            UNION ALL
            --谁配送的
            SELECT   fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
				JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59'  AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
			UNION ALL
			SELECT		fcbi.ID,fcbi.MerchantID ,fcbi.WaybillNO ,CONVERT(NVARCHAR(20),fcbi.CreateTime,112) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN fcbi.FinalExpressCompanyID IS NULL THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE fcbi.ExpressCompanyID END    ELSE fcbi.FinalExpressCompanyID END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN ISNULL(fcbi.AreaType,0)>0 THEN fcbi.AreaType ELSE ael.AreaType END AreaType,fcbi.IsCOD
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			           JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.ReturnDate AS 日期 ,
                        结算单位=CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType 分配区域 ,");
            if (IsCod)
                sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
            sb.Append(@"
                        COUNT(t.WaybillNO) AS 订单数 ,
                        ISNULL(SUM(ISNULL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        ISNULL(SUM(ISNULL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        ISNULL(SUM(ISNULL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN ISNULL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        ISNULL(SUM(ISNULL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) ELSE
								CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END END AS 发货仓库 ,
                        CASE WHEN ISNULL(t.FinalExpressCompanyID,0)<>0 THEN ec4.CompanyName ELSE 
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END END AS 发货仓库名称 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END AS 入库仓 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        SUM(ISNULL(t.Fare,0)) AS 配送费,
                        SUM(ISNULL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN '0' THEN '是'
                          WHEN '1' THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
              FROM      t
                        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON t.MerchantID = mbi.ID
                        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN rfd_pms.dbo.Warehouse wh ( NOLOCK ) ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec3 ( NOLOCK ) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec4 ( NOLOCK ) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
						LEFT JOIN rfd_pms.dbo.Warehouse wh1 ( NOLOCK ) ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN rfd_pms.dbo.ExpressCompany ec5 ( NOLOCK ) ON ec5.expresscompanyid = t.ReturnExpressCompanyID
                                                              AND ec5.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.ReturnDate ,
						t.Warehouseid,
                        t.FinalExpressCompanyID,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        t.ReturnWareHouseID,
                        ec4.CompanyName,
                        ec5.expresscompanyid,
                        CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType ,");
            if (IsCod)
                sb.Append(@"t.IsCOD ,");
            sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec3.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.Warehouseid
                             ELSE CONVERT(NVARCHAR(20), ec5.expresscompanyid)
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END ,
                        wh.IsMain ,
                        t.WaybillType");

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

        private string GetAreaTypeSql(string areaType)
        {
            string sql = @"
LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = fcbi.AreaID
                                 AND ael.[Enable] IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ISNULL(ael.WareHouseID,'') = ''
								{0}
";
            //if (!string.IsNullOrEmpty(areaType))
            //    sql = string.Format(sql, " AND ael.AreaType=" + areaType);
            //else
            sql = string.Format(sql, "");
            return sql;
        }

        public DataTable SearchExprotDetailDataV2(string condition, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSqlV2(false);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSqlV2(false);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSqlV2(false);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public DataTable SearchExprotDetailData(string condition, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverSql(false);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsSql(false);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsSql(false);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelperEx.ExecuteDataset(ResetTimeOutString(ReadOnlyConnString, 180), CommandType.Text, sql).Tables[0];
            //return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public DataTable SearchExprotStatDataV2(string condition, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverStatSqlV2(searchCondition.DateType, searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsStatSqlV2(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsStatSqlV2(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public DataTable SearchExprotStatData(string condition, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = GetDeliverStatSql(searchCondition.DateType, searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = GetVisitReturnsStatSql(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = GetReturnsStatSql(searchCondition.IsAreaType, searchCondition.SummaryByCod);
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);//
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        /// <summary>
        /// 支出日报发货查询
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable SearchLogisticsDeliverDailyV2(string condition, string type, CODSearchCondition csc, string accountDateColumnName)
        {
            StringBuilder sqlStr = new StringBuilder();
            if (csc.IsAreaType)
            {
                sqlStr.Append(@"
                    SELECT 1 as RowNums,
                           {2} AccountDate,
                           mbi.MerchantName,
                            ec.ExpressCompanyID,
                           ec.AccountCompanyName,
                           ael.AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"isnull(COUNT(fci.WaybillNO),0) AS CountNum,
                           SUM(ISNULL(fci.Fare, 0)) CountFare,
                           fci.FareFormula,
                           '{0}' AS CountType
                    FROM   FMS_CODBaseInfo fci(NOLOCK)
                           JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
                                ON  ec.ExpressCompanyID = fci.TopCODCompanyID
                           JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
                                ON  mbi.ID = fci.MerchantID
                           LEFT JOIN AreaExpressLevel ael(NOLOCK)
                                ON  ael.AreaID = fci.AreaID
                                AND ael.[Enable] IN (1, 2)
                                AND ael.expresscompanyid = fci.TopCodCompanyID
                                AND ael.MerchantID = fci.MerchantID
                                AND ISNULL(ael.WareHouseID, '') = ''
                    WHERE  (1 = 1)
                           {1}
                    GROUP BY
                           mbi.MerchantName,
                           ec.ExpressCompanyID,
                           ec.AccountCompanyName,
                           ael.AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"fci.FareFormula,
                           {2}
                    ");
            }
            else
            {
                sqlStr.Append(@"
                    SELECT 1 as RowNums,
                           {2} AccountDate,
                           mbi.MerchantName,
                           ec.ExpressCompanyID,
                           ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"isnull(COUNT(fci.WaybillNO),0) AS CountNum,
                           SUM(ISNULL(fci.Fare, 0)) CountFare,
                           '{0}' AS CountType
                    FROM   FMS_CODBaseInfo fci(NOLOCK)
                           JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
                                ON  ec.ExpressCompanyID = fci.TopCODCompanyID
                           JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
                                ON  mbi.ID = fci.MerchantID
                    WHERE  (1 = 1)
                           {1}
                    GROUP BY
                           mbi.MerchantName,
                           ec.ExpressCompanyID,
                           ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"{2}");
            }
            string sql = string.Format(sqlStr.ToString(), type, condition, accountDateColumnName);

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql);

            if (ds == null || ds.Tables.Count <= 0) return null;

            return ds.Tables[0];
        }

        /// <summary>
        /// 支出日报发货查询
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable SearchLogisticsDeliverDaily(string condition, string type, CODSearchCondition csc, string accountDateColumnName)
        {
            StringBuilder sqlStr = new StringBuilder();
            if (csc.IsAreaType)
            {
                sqlStr.Append(@"
SELECT 1 as RowNums,
       {2} AccountDate,
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN ISNULL(fci.AreaType,0)>0 THEN fci.AreaType ELSE ael.AreaType END AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"
       isnull(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(ISNULL(fci.Fare, 0)) CountFare,
       fci.FareFormula,
       '{0}' AS CountType
FROM   LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.ID = fci.MerchantID
       LEFT JOIN AreaExpressLevel ael(NOLOCK)
            ON  ael.AreaID = fci.AreaID
            AND ael.[Enable] IN (1, 2)
            AND ael.expresscompanyid = fci.TopCodCompanyID
            AND ael.MerchantID = fci.MerchantID
            AND ISNULL(ael.WareHouseID, '') = ''
WHERE  (1 = 1)
       {1}
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN ISNULL(fci.AreaType,0)>0 THEN fci.AreaType ELSE ael.AreaType END,
       fci.FareFormula,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"
       {2}
");
            }
            else
            {
                sqlStr.Append(@"
SELECT 1 as RowNums,
       {2} AccountDate,
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"
       isnull(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(ISNULL(fci.Fare, 0)) CountFare,
       '{0}' AS CountType
FROM   LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.ID = fci.MerchantID
WHERE  (1 = 1)
       {1}
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"{2}");
            }
            string sql = string.Format(sqlStr.ToString(), type, condition, accountDateColumnName);

            DataSet ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        /// <summary>
        /// 支出日报拒收查询
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable SearchLogisticsReturnsDailyV2(string condition, string type, CODSearchCondition csc, string accountDateColumnName)
        {
            StringBuilder sqlStr = new StringBuilder();
            if (csc.IsAreaType)
            {
                sqlStr.Append(@"
with t as (
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.AreaID,
       fci.Fare,
       fci.FareFormula,
       fci.ReturnTime,
       fci.IsCOD
FROM   FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1)
       {1}
union all
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.AreaID,
       fci.Fare,
       fci.FareFormula,
       fci.CreateTime AS ReturnTime,
       fci.IsCOD
FROM   FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType in (2,5)
       {2}
       )
       
       select 
       {3} AccountDate,
       1 as RowNums,
	   mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       ael.AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       isnull(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(ISNULL(t.Fare, 0)) CountFare,
       t.FareFormula,
       '{0}' AS CountType
        from t
       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.ID = t.MerchantID
       LEFT JOIN AreaExpressLevel ael(NOLOCK)
            ON  ael.AreaID = t.AreaID
            AND ael.[Enable] IN (1, 2)
            AND ael.expresscompanyid = t.TopCodCompanyID
            AND ael.MerchantID = t.MerchantID
            AND ISNULL(ael.WareHouseID, '') = ''
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       ael.AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       t.FareFormula,
       {3}
");
            }
            else
            {
                sqlStr.Append(@"
with t as (
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.Fare,
       fci.ReturnTime,
       fci.IsCOD
FROM   FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1)
       {1}
union all
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.Fare,
       fci.CreateTime AS ReturnTime,
       fci.IsCOD
FROM   FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType in (2,5)
       {2}
       )
       
       select 
       1 as RowNums,
       {3} AccountDate,
	   mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       isnull(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(ISNULL(t.Fare, 0)) CountFare,
       '{0}' AS CountType
        from t
       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.ID = t.MerchantID
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       {3}
");
            }
            string sql = string.Format(sqlStr.ToString(), type, condition, condition.Replace("fci.ReturnTime", "fci.CreateTime"), accountDateColumnName);

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        /// <summary>
        /// 支出日报拒收查询
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable SearchLogisticsReturnsDaily(string condition, string type, CODSearchCondition csc, string accountDateColumnName)
        {
            StringBuilder sqlStr = new StringBuilder();
            if (csc.IsAreaType)
            {
                sqlStr.Append(@"
with t as (
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.AreaID,
       fci.Fare,
       fci.FareFormula,
       fci.ReturnTime,
       fci.IsCOD,
       fci.AreaType
FROM   LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1)
       {1}
union all
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.AreaID,
       fci.Fare,
       fci.FareFormula,
       fci.CreateTime AS ReturnTime,
       fci.IsCOD,
       fci.AreaType
FROM   LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType in (2,5)
       {2}
       )
       
       select 
       {3} AccountDate,
       1 as RowNums,
	   mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN ISNULL(t.AreaType,0)>0 THEN t.AreaType ELSE ael.AreaType END AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       isnull(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(ISNULL(t.Fare, 0)) CountFare,
       t.FareFormula,
       '{0}' AS CountType
        from t
       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.ID = t.MerchantID
       LEFT JOIN AreaExpressLevel ael(NOLOCK)
            ON  ael.AreaID = t.AreaID
            AND ael.[Enable] IN (1, 2)
            AND ael.expresscompanyid = t.TopCodCompanyID
            AND ael.MerchantID = t.MerchantID
            AND ISNULL(ael.WareHouseID, '') = ''
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN ISNULL(t.AreaType,0)>0 THEN t.AreaType ELSE ael.AreaType END,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       t.FareFormula,
       {3}
");
            }
            else
            {
                sqlStr.Append(@"
with t as (
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.Fare,
       fci.ReturnTime,
       fci.IsCOD
FROM   LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1)
       {1}
union all
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.Fare,
       fci.CreateTime AS ReturnTime,
       fci.IsCOD
FROM   LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK)
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType in (2,5)
       {2}
       )
       
       select 
       1 as RowNums,
       {3} AccountDate,
	   mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       isnull(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(ISNULL(t.Fare, 0)) CountFare,
       '{0}' AS CountType
        from t
       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.ID = t.MerchantID
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       {3}
");
            }
            string sql = string.Format(sqlStr.ToString(), type, condition, condition.Replace("fci.ReturnTime", "fci.CreateTime"), accountDateColumnName);

            DataSet ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }


        public DataTable SearchLogisticsSignedReturnDaily(string condition, string type, CODSearchCondition csc, string accountDateColumnName)
        {
            throw new NotImplementedException();
        }
         public DataTable StatCod<T>(string condition, CodSearchCondition searchCondition, List<T> parameterList)
         {
             throw new Exception("sql未实现");
         }
        public DataTable SearchCodDetails<T>(string condition, ref PageInfo pi, CodSearchCondition searchCondition, List<T> parameterList, bool isPage)
         {
             throw new Exception("sql未实现");
         }
        public DataTable SearchExprotDetailData<T>(string condition, CodSearchCondition searchCondition, List<T> parameterList)
        {
            throw new Exception("sql未实现");
        }
       public  DataTable SearchLogisticsReturnsDaily<T>(string condition, string type, CODSearchCondition csc, string accountDateColumnName, List<T> parameterList)
       {
           throw new Exception("sql未实现");
       }
      public  DataTable SearchLogisticsDeliverDaily<T>(string condition, string type, CODSearchCondition csc, string accountDateColumnName, List<T> parameterList)
       {
           throw new Exception("sql未实现");
       }
    }
}
