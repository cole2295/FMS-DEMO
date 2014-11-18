using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using System.Data.SqlClient;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.COD;

namespace RFD.FMS.DAL.Oracle.COD
{
    public class LogisticsDeliveryDao : OracleDao, ILogisticsDeliveryDao
    {
        public DataTable SearchCodDetails<T>(string condition, ref PageInfo pi, CodSearchCondition searchCondition, List<T> parameterList,bool isPage)
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
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            List<OracleParameter> parameterListTmp = new List<OracleParameter>();
            if (isPage)
            {
                parameterListTmp.Add(new OracleParameter(":pageStr", OracleDbType.Decimal) { Value = pi.CurrentPageStartRowNum });
                parameterListTmp.Add(new OracleParameter(":pageEnd", OracleDbType.Decimal) { Value = pi.CurrentPageEndRowNum });
            }
            var oracleParameterList = parameterList as List<OracleParameter>;
            parameterListTmp.AddRange(oracleParameterList);

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql, ToParameters(parameterListTmp.ToArray()));
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }
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
                sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            //else if(searchCondition.ReportType == "3")
            //{
            //    sql = GetSignedReturnSql(true);
            //    sql = string.Format(sql,condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            //}
            OracleParameter[] parameters ={
										   new OracleParameter(":pageStr",OracleDbType.Decimal),
										   new OracleParameter(":pageEnd",OracleDbType.Decimal)
									  };
            parameters[0].Value = pi.CurrentPageStartRowNum;
            parameters[1].Value = pi.CurrentPageEndRowNum;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchCodDetailsV2(string condition, ref PageInfo pi, CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 没有查询V2");
        }

        private string GetDeliverSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            //if (isPage)
            //    sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前的
                SELECT   fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM  FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                --谁接的
                UNION ALL
                SELECT   fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
                --谁配送的
                UNION ALL
                SELECT  fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )");
            //if (isPage)
            //    sb.Append("select * from (");ROW_NUMBER() OVER ( ORDER BY t.MerchantID DESC ) AS 序号,
            sb.Append(@"
    SELECT  ROWNUM AS 序号,
            t.InfoID AS 唯一编号,
            t.InfoID AS 明细,
			mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.TotalAmount, 0) AS 订单总价 ,
            NVL(t.PaidAmount, 0) AS 已收款 ,
            NVL(t.NeedPayAmount, 0) AS 应收款 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE cast(ec3.expresscompanyid as VARCHAR2(20)) END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName ELSE ec3.CompanyName END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
             CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE
            	CASE WHEN NVL(t.FinalExpressCompanyID,0)=0 THEN
            	    cast(ec3.expresscompanyid as VARCHAR2(20))
				 ELSE cast(t.FinalExpressCompanyID as VARCHAR2(20)) END
        	 END AS 末级发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
            	 ec3.CompanyName
				 ELSE ec4.CompanyName END
            END AS 末级发货仓名称 ,
            CASE t.WaybillType WHEN '1' THEN '是' ELSE '否' END 上门换 ,
            CASE wh.IsMain WHEN 0 THEN '是'  WHEN 1 THEN '否' END 主分仓
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
                sb = new StringBuilder("SELECT * FROM (").Append(sb.Append(") WHERE   序号 BETWEEN :pageStr AND :pageEnd"));


            return sb.ToString();
        }

        private string GetReturnsSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            //if (isPage)
            //    sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
            --2012.05.22之前查询
            SELECT  fcbi.INFOID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,cast(fcbi.ReturnExpressCompanyID AS varchar2(20)) AS ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
            UNION ALL
			SELECT	fcbi.INFOID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN  NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID AS varchar2(20)) END    ELSE cast(fcbi.FinalExpressCompanyID AS varchar2(20)) END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo fcbi
			            {1}
			   WHERE    ( 1 = 1 )  AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {2}
            UNION ALL
            --谁接的单
            SELECT  fcbi.INFOID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,cast(fcbi.ReturnExpressCompanyID AS varchar2(20)) AS ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {0}
            UNION ALL
			SELECT	fcbi.INFOID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN  NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID AS varchar2(20)) END    ELSE cast(fcbi.FinalExpressCompanyID AS varchar2(20)) END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo fcbi
			            {1}
			   WHERE    ( 1 = 1 )  AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {2}
            UNION ALL
            --谁配送的单
            SELECT  fcbi.INFOID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,cast(fcbi.ReturnExpressCompanyID AS varchar2(20)) AS ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
               JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.CreateTime > trunc(sysdate-90) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
            UNION ALL
			SELECT	fcbi.INFOID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.CreateTime AS ReturnTime,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID AS varchar2(20)) END    ELSE cast(fcbi.FinalExpressCompanyID AS varchar2(20)) END AS ReturnExpressCompanyID,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo fcbi
               JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0
			            {1}
			   WHERE    ( 1 = 1 )  AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
    SELECT  ROWNUM AS 序号,
            t.InfoID AS 唯一编号,
			mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.TotalAmount, 0) AS 订单总价 ,
            NVL(t.PaidAmount, 0) AS 已收款 ,
            NVL(t.NeedPayAmount, 0) AS 应收款 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE cast(ec5.expresscompanyid as VARCHAR2(20))
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
                sb = new StringBuilder("SELECT * FROM (").Append(sb.Append(") WHERE   序号 BETWEEN :pageStr AND :pageEnd"));


            return sb.ToString();
        }

        private string GetVisitReturnsSql(bool isPage)
        {
            StringBuilder sb = new StringBuilder();
            //if (isPage)
            //    sb.Append(GetPageSql());
            sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.ReturnTime,fcbi.ReturnExpressCompanyID,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
				JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )");
            //if (isPage)
            //    sb.Append("select * from (");
            sb.Append(@"
    SELECT  ROWNUM AS 序号,
            t.InfoID AS 唯一编号,
			mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
            ec2.CompanyName AS 实际配送站 ,
            t.AreaType 分配区域 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.TotalAmount, 0) AS 订单总价 ,
            0.00 AS 已退款 ,--NVL(t.BackAmount, 0)
            NVL(t.NeedBackAmount, 0) AS 应退款 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
            t.Fare AS 配送费,
            t.FareFormula 配送费公式,
            t.ProtectedPrice AS 保价金额,
            t.BoxsNo AS 包装箱 ,
            t.Address AS 收货人地址 ,
            t.RfdAcceptTime AS 始发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                 ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
            END AS 初始发货仓 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
                 ELSE ec3.CompanyName
            END AS 初始发货仓名称 ,
            t.DeliverTime AS 最终发日期 ,
            t.FinalExpressCompanyID AS 末级发货仓 ,
            ec4.CompanyName AS 末级发货仓名称 ,
            t.ReturnTime AS 入库日期,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                 ELSE cast(ec5.expresscompanyid as VARCHAR2(20))
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
                sb = new StringBuilder("SELECT * FROM (").Append(sb.Append(") WHERE   序号 BETWEEN :pageStr AND :pageEnd"));


            return sb.ToString();
        }

        /// <summary>
        /// 签单返回查询明细sql
        /// </summary>
        /// <param name="isPage"></param>
        /// <returns></returns>
//       private string GetSignedReturnSql(bool isPage)
//        {
//            StringBuilder sb = new StringBuilder();
//            //if (isPage)
//            //    sb.Append(GetPageSql());
//            sb.Append(@"
//            WITH    t AS ( 
//                --2012.05.22之前的
//                SELECT   fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//                ，fcbi.ReturnTime
//               FROM  FMS_CODBaseInfo fcbi
//						{1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
//                --谁接的
//                UNION ALL
//                SELECT   fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//                ,fcbi.ReturnTime
//                FROM     FMS_CODBaseInfo fcbi
//						{1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
//                --谁配送的
//                UNION ALL
//                SELECT  fcbi.InfoID,fcbi.MerchantID ,fcbi.TopCODCompanyID ,fcbi.DeliverStationID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.AreaID ,fcbi.WaybillNO ,fcbi.Address ,fcbi.BoxsNo ,fcbi.RfdAcceptTime ,fcbi.DeliverTime ,fcbi.WaybillType,fcbi.Fare,fcbi.FareFormula,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//                ，fcbi.ReturnTime
//                FROM     FMS_CODBaseInfo fcbi
//						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
//             )");
//            sb.Append(@"
//    SELECT  ROWNUM AS 序号,
//            t.InfoID AS 唯一编号,
//			mbi.MerchantName AS 商家 ,
//            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
//            ec2.CompanyName AS 实际配送站 ,
//            t.AreaType 分配区域 ,
//            t.WaybillNO AS 订单号 ,
//            NVL(t.TotalAmount, 0) AS 订单总价 ,
//            NVL(t.PaidAmount, 0) AS 已收款 ,
//            NVL(t.NeedPayAmount, 0) AS 应收款 ,
//            NVL(t.AccountWeight, 0) AS 结算重量 ,
//            CASE WHEN t.IsCOD IS NULL or t.IsCOD=0 THEN '否' ELSE '是' END 业务类型,
//            t.Fare AS 配送费,
//            t.FareFormula 配送费公式,
//            t.ProtectedPrice AS 保价金额,
//            t.BoxsNo AS 包装箱 ,
//            t.Address AS 收货人地址 ,
//            t.RfdAcceptTime AS 始发日期 ,
//            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE cast(ec3.expresscompanyid as VARCHAR2(20)) END AS 初始发货仓 ,
//            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName ELSE ec3.CompanyName END AS 初始发货仓名称 ,
//            t.DeliverTime AS 最终发日期 ,
//            t.ReturnTime AS 签单返回时间，
//             CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid ELSE
//            	CASE WHEN NVL(t.FinalExpressCompanyID,0)=0 THEN
//            	    cast(ec3.expresscompanyid as VARCHAR2(20))
//				 ELSE cast(t.FinalExpressCompanyID as VARCHAR2(20)) END
//        	 END AS 末级发货仓 ,
//            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
//            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
//            	 ec3.CompanyName
//				 ELSE ec4.CompanyName END
//            END AS 末级发货仓名称 ,
//            CASE t.WaybillType WHEN '1' THEN '是' ELSE '否' END 上门换 ,
//            CASE wh.IsMain WHEN 0 THEN '是'  WHEN 1 THEN '否' END 主分仓
//    FROM    t
//            JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
//            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
//            JOIN ExpressCompany ec2 ON ec2.ExpressCompanyID = t.DeliverStationID
//            LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
//            LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
//                                                              AND ec3.CompanyFlag = 1
//            LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
//                                                              AND ec4.CompanyFlag = 1
//            ");
//            if (isPage)
//                sb=new StringBuilder("SELECT * FROM (").Append(sb.Append(") WHERE   序号 BETWEEN :pageStr AND :pageEnd"));

//            return sb.ToString();
//        }

        public DataTable StatCodV2(string condition, CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 没有查询V2");
        }

        public DataTable StatCod<T>(string condition, CodSearchCondition searchCondition, List<T> parameterList)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = @"
WITH t AS (
                --2012.05.22之前
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi 
               JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		NVL(SUM(NVL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		NVL(SUM(NVL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计,
        NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
        NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t ";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = @"
WITH    t AS ( 
               --2012.05.22之前查询
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 )  AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi {1}
			   WHERE    ( 1 = 1 )  AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {2}
                UNION ALL
               --谁接的单
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 )  AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi {1}
			   WHERE    ( 1 = 1 )  AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {2}
                UNION ALL
               --谁配送的
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi 
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 )  AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi 
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0{1}
			   WHERE    ( 1 = 1 )  AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 结算重量合计,
            NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
            NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
    FROM    t
";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "6")
            {
                sql = @"
WITH t AS (
		        --2012.05.22之前
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 )  AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
				JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 )   AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--NVL(SUM(NVL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN NVL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		NVL(SUM(NVL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计,
        NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
        NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql.ToString(),ToParameters(parameterList.ToArray())).Tables[0];
        }
        public DataTable StatCod(string condition, CodSearchCondition searchCondition)
        {
            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = @"
WITH t AS (
                --2012.05.22之前
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi 
               JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		NVL(SUM(NVL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		NVL(SUM(NVL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计,
        NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
        NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t ";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = @"
WITH    t AS ( 
               --2012.05.22之前查询
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {2}
                UNION ALL
               --谁接的单
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {2}
                UNION ALL
               --谁配送的
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi 
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi 
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0{1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 结算重量合计,
            NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
            NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
    FROM    t
";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
            }
            else if(searchCondition.ReportType == "6")
            {
                sql = @"
WITH t AS (
		        --2012.05.22之前
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
				JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--NVL(SUM(NVL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN NVL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		NVL(SUM(NVL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计,
        NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
        NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t";
                sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            }
                // 签单返回类型
//            else if (searchCondition.ReportType == "3")
//            {
//                sql = @"
//WITH t AS (
//                --2012.05.22之前
//               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
//               FROM     FMS_CODBaseInfo fcbi {1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
//               UNION ALL
//                --谁接的
//			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
//               FROM     FMS_CODBaseInfo fcbi {1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
//               UNION ALL
//                --谁配送的
//               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
//               FROM     FMS_CODBaseInfo fcbi 
//               JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
//	)
//SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
//		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
//		NVL(SUM(NVL(t.PaidAmount, 0)),0) AS 已收款合计 ,
//		COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
//				   THEN t.NeedPayAmount
//			  END) AS 应收订单量合计 ,
//		NVL(SUM(NVL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
//		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计,
//        NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
//        NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
//FROM    t ";
            //    sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
            //}

            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql.ToString()).Tables[0];
        }

        public DataTable SearchCodStatV2(string condition, CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 没有查询V2");
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
            //else if(searchCondition.ReportType == "3")
            //{
            //    sqlDetail = GetSignedReturnStatSql(searchCondition.DateType, searchCondition.IsAreaType,
            //                                       searchCondition.SummaryByCod);
            //    sqlDetail = string.Format(sqlDetail, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);
            //}


            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sqlDetail);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        private string GetDeliverStatSql(string dateType, bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            if (dateType == "1")
            {
                sb.Append(@"
WITH    t AS ( 
                --2012.05.22之前
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.CreateTime > trunc(sysdate-90) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.CreateTime > trunc(sysdate-90) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.CreateTime > trunc(sysdate-90) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.DeliverDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType 分配区域 ,");
                if (IsCod)
                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
                        NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN NVL(t.FinalExpressCompanyID,0)=0 THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
                        	ELSE cast(t.ExpressCompanyID as VARCHAR2(20)) END
                        	ELSE cast(t.FinalExpressCompanyID as VARCHAR2(20)) END
                         AS 发货仓库 ,
                        CASE WHEN ec4.CompanyName IS NULL THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
                        	ELSE ec3.CompanyName END
                        	ELSE ec4.CompanyName END AS 发货仓库名称 ,
                        SUM(NVL(t.Fare,0)) AS 配送费,
                        SUM(NVL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN 0 THEN '是'
                          WHEN 1 THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.DeliverDate ,
						t.Warehouseid,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType ,");
                if (IsCod)
                    sb.Append(@"t.IsCOD ,");
                sb.Append(@"                        
                        CASE WHEN NVL(t.FinalExpressCompanyID,0)=0 THEN
                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
                        	ELSE cast(t.ExpressCompanyID as VARCHAR2(20)) END
                        	ELSE cast(t.FinalExpressCompanyID as VARCHAR2(20)) END,
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
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.CreateTime > trunc(sysdate-90) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss')  {0}
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.CreateTime > trunc(sysdate-90) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
                UNION ALL
                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.CreateTime >trunc(sysdate-90) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.RfdAcceptDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType 分配区域 ,");
                if (IsCod)
                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
                        NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
							ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
						END AS 发货仓库 ,
						CASE WHEN t.MerchantID IN ( 8, 9 )
							 THEN wh.WarehouseName
							 ELSE ec3.CompanyName
						END AS 发货仓库名称 ,
                        SUM(NVL(t.Fare,0)) AS 配送费,
                        SUM(NVL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN 0 THEN '是'
                          WHEN 1 THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
              GROUP BY  mbi.MerchantName ,
                        t.RfdAcceptDate ,
						t.Warehouseid,
                        t.FinalExpressCompanyID,
                        t.MerchantID,
                        ec3.expresscompanyid,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
                if (IsAreaType)
                    sb.Append(@"t.AreaType ,");
                if (IsCod)
                    sb.Append(@"t.IsCOD ,");
                sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
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

        private string GetVisitReturnsStatSql(bool IsAreaType, bool IsCod)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
WITH    t AS ( 
            --2012.05.22之前的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
            UNION ALL
            --谁接的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
            UNION ALL
            --谁配送的
            SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.BackAmount ,fcbi.NeedBackAmount ,fcbi.AccountWeight ,fcbi.WaybillType ,fcbi.ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.ReturnDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,");
            if (IsAreaType)
                sb.Append(@"t.AreaType 分配区域 ,");
            if (IsCod)
                sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
            sb.Append(@"
                        COUNT(t.WaybillNO) AS 订单数 ,
                        NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        0.00 AS 已退款 ,--NVL(SUM(NVL(t.BackAmount, 0)), 0)
                        NVL(SUM(NVL(t.NeedBackAmount, 0)), 0) AS 应退款 ,
                        CASE WHEN NVL(t.FinalExpressCompanyID,0)<>0 THEN cast(t.FinalExpressCompanyID as VARCHAR2(20)) ELSE
								CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                             ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
                        END END AS 发货仓库 ,
                        CASE WHEN NVL(t.FinalExpressCompanyID,0)<>0 THEN ec4.CompanyName ELSE 
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END END AS 发货仓库名称 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                             ELSE cast(ec5.expresscompanyid as VARCHAR2(20))
                        END AS 入库仓 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        SUM(NVL(t.Fare,0)) AS 配送费,
                        SUM(NVL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN 0 THEN '是'
                          WHEN 1 THEN '否'
                        END 主分仓
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
                        LEFT JOIN Warehouse wh1 ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN ExpressCompany ec5 ON ec5.expresscompanyid = t.ReturnExpressCompanyID
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
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType ,");
            if (IsCod)
                sb.Append(@"t.IsCOD ,");
            sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE cast(ec3.expresscompanyid as NVARCHAR2(20))
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.Warehouseid
                             ELSE cast(ec5.expresscompanyid as NVARCHAR2(20))
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
            SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,cast(fcbi.ReturnExpressCompanyID  AS varchar(20)) AS ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
			UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,trunc(fcbi.CreateTime) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID AS VARCHAR2(20)) END    ELSE cast(fcbi.FinalExpressCompanyID AS VARCHAR2(20)) END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo fcbi
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {2}
            UNION ALL
            --谁接的单
            SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,cast(fcbi.ReturnExpressCompanyID  AS varchar(20)) AS ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {0}
			UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,trunc(fcbi.CreateTime) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID AS VARCHAR2(20)) END    ELSE cast(fcbi.FinalExpressCompanyID AS VARCHAR2(20)) END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo fcbi
			            {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{3}' {2}
            UNION ALL
            --谁配送的
            SELECT   fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,cast(fcbi.ReturnExpressCompanyID  AS varchar(20)) AS ReturnExpressCompanyID ,fcbi.ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
               FROM     FMS_CODBaseInfo fcbi
				JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss')  AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {0}
			UNION ALL
			SELECT		fcbi.INFOID as ID,fcbi.MerchantID ,fcbi.WaybillNO ,trunc(fcbi.CreateTime) as ReturnDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN     CASE WHEN fcbi.MerchantID IN (8,9) THEN fcbi.Warehouseid ELSE cast(fcbi.ExpressCompanyID AS VARCHAR2(20)) END    ELSE cast(fcbi.FinalExpressCompanyID AS VARCHAR2(20)) END AS ReturnExpressCompanyID ,fcbi.Warehouseid AS ReturnWareHouseID,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
			   FROM     FMS_CODBaseInfo fcbi
			           JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{3}' AND ec.DistributionCode='{3}' {2}
             )
SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
                        mbi.MerchantName AS 商家 ,
                        t.ReturnDate AS 日期 ,
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,");
            if (IsAreaType)
                sb.Append(@"t.AreaType 分配区域 ,");
            if (IsCod)
                sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
            sb.Append(@"
                        COUNT(t.WaybillNO) AS 订单数 ,
                        NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 订单重量 ,
                        NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 订单总价 ,
                        NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款 ,
                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                                   THEN t.NeedPayAmount
                              END) AS 应收订单量 ,
                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
                        CASE WHEN NVL(t.FinalExpressCompanyID,0)<>0 THEN cast(t.FinalExpressCompanyID as VARCHAR2(20)) ELSE
								CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
                             ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
                        END END AS 发货仓库 ,
                        CASE WHEN NVL(t.FinalExpressCompanyID,0)<>0 THEN ec4.CompanyName ELSE 
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END END AS 发货仓库名称 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.ReturnWareHouseID
                             ELSE cast(ec5.expresscompanyid as VARCHAR2(20))
                        END AS 入库仓 ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END AS 入库仓名称 ,
                        SUM(NVL(t.Fare,0)) AS 配送费,
                        SUM(NVL(t.ProtectedPrice,0)) AS 保价金额,
                        CASE wh.IsMain
                          WHEN 0 THEN '是'
                          WHEN 1 THEN '否'
                        END 主分仓 ,
                        CASE t.WaybillType
                          WHEN '1' THEN '是'
                          ELSE '否'
                        END 上门换
              FROM      t
                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
						LEFT JOIN Warehouse wh1 ON wh1.Warehouseid = t.ReturnWareHouseID
                        LEFT JOIN ExpressCompany ec5 ON ec5.expresscompanyid = t.ReturnExpressCompanyID
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
                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
            if (IsAreaType)
                sb.Append(@"t.AreaType ,");
            if (IsCod)
                sb.Append(@"t.IsCOD ,");
            sb.Append(@" 
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
                             ELSE cast(ec3.expresscompanyid as NVARCHAR2(20))
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh.WarehouseName
                             ELSE ec3.CompanyName
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh1.Warehouseid
                             ELSE cast(ec5.expresscompanyid as NVARCHAR2(20))
                        END ,
                        CASE WHEN t.MerchantID IN ( 8, 9 )
                             THEN wh1.WarehouseName
                             ELSE ec5.CompanyName
                        END ,
                        wh.IsMain ,
                        t.WaybillType");

            return sb.ToString();
        }

//        private  string GetSignedReturnStatSql(string dateType,bool IsAreaType, bool IsCod)
//        {
//            StringBuilder sb = new StringBuilder();
//            if (dateType == "1")
//            {
//                sb.Append(@"
//WITH    t AS ( 
//                --2012.05.22之前
//                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//               FROM     FMS_CODBaseInfo fcbi
//						{1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
//                UNION ALL
//                --谁接的
//                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//               FROM     FMS_CODBaseInfo fcbi
//						{1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
//                UNION ALL
//                --谁配送的
//                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.DeliverDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//               FROM     FMS_CODBaseInfo fcbi
//						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
//             )
//    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
//                        mbi.MerchantName AS 商家 ,
//                        t.DeliverDate AS 日期 ,
//                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,");
//                if (IsAreaType)
//                    sb.Append(@"t.AreaType 分配区域 ,");
//                if (IsCod)
//                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
//                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
//                        NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 订单重量 ,
//                        NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 订单总价 ,
//                        NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款 ,
//                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
//                                   THEN t.NeedPayAmount
//                              END) AS 应收订单量 ,
//                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
//                        CASE WHEN NVL(t.FinalExpressCompanyID,0)=0 THEN
//                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
//                        	ELSE cast(t.ExpressCompanyID as VARCHAR2(20)) END
//                        	ELSE cast(t.FinalExpressCompanyID as VARCHAR2(20)) END
//                         AS 发货仓库 ,
//                        CASE WHEN ec4.CompanyName IS NULL THEN
//                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
//                        	ELSE ec3.CompanyName END
//                        	ELSE ec4.CompanyName END AS 发货仓库名称 ,
//                        SUM(NVL(t.Fare,0)) AS 配送费,
//                        SUM(NVL(t.ProtectedPrice,0)) AS 保价金额,
//                        CASE wh.IsMain
//                          WHEN 0 THEN '是'
//                          WHEN 1 THEN '否'
//                        END 主分仓 ,
//                        CASE t.WaybillType
//                          WHEN '1' THEN '是'
//                          ELSE '否'
//                        END 上门换
//              FROM      t
//                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
//                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
//                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
//                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
//                                                              AND ec3.CompanyFlag = 1
//                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
//                                                              AND ec4.CompanyFlag = 1
//              GROUP BY  mbi.MerchantName ,
//                        t.DeliverDate ,
//						t.Warehouseid,
//                        t.MerchantID,
//                        ec3.expresscompanyid,
//                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
//                if (IsAreaType)
//                    sb.Append(@"t.AreaType ,");
//                if (IsCod)
//                    sb.Append(@"t.IsCOD ,");
//                sb.Append(@"                        
//                        CASE WHEN NVL(t.FinalExpressCompanyID,0)=0 THEN
//                        	CASE WHEN t.MerchantID IN (8,9) THEN t.Warehouseid
//                        	ELSE cast(t.ExpressCompanyID as VARCHAR2(20)) END
//                        	ELSE cast(t.FinalExpressCompanyID as VARCHAR2(20)) END,
//                        CASE WHEN ec4.CompanyName IS NULL THEN
//                        	CASE WHEN t.MerchantID IN (8,9) THEN wh.WarehouseName
//                        	ELSE ec3.CompanyName END
//                        	ELSE ec4.CompanyName END,
//                        wh.IsMain ,
//                        t.WaybillType");
//            }
//            else
//            {
//                sb.Append(@"
//WITH    t AS ( 
//                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//                FROM     FMS_CODBaseInfo fcbi
//						{1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss')  {0}
//                UNION ALL
//                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//               FROM     FMS_CODBaseInfo fcbi
//						{1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode='{2}' {0}
//                UNION ALL
//                SELECT   fcbi.MerchantID ,fcbi.WaybillNO ,fcbi.RfdAcceptDate ,fcbi.TopCODCompanyID ,fcbi.AreaID ,fcbi.Warehouseid ,fcbi.ExpressCompanyID ,fcbi.FinalExpressCompanyID ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight ,fcbi.WaybillType,fcbi.Fare,fcbi.ProtectedPrice,CASE WHEN fcbi.AreaType IS NULL OR fcbi.AreaType=0 THEN ael.AreaType ELSE fcbi.AreaType END AreaType,fcbi.IsCOD
//               FROM     FMS_CODBaseInfo fcbi
//						JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
//               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>'{2}' AND ec.DistributionCode='{2}' {0}
//             )
//    SELECT    ROW_NUMBER() OVER ( ORDER BY mbi.MerchantName DESC ) AS 序号 ,
//                        mbi.MerchantName AS 商家 ,
//                        t.RfdAcceptDate AS 日期 ,
//                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,");
//                if (IsAreaType)
//                    sb.Append(@"t.AreaType 分配区域 ,");
//                if (IsCod)
//                    sb.Append(@"CASE WHEN t.IsCOD IS NULL OR t.IsCOD=0 THEN '否' ELSE '是' END 业务类型 ,");
//                sb.Append(@"COUNT(t.WaybillNO) AS 订单数 ,
//                        NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 订单重量 ,
//                        NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 订单总价 ,
//                        NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款 ,
//                        COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
//                                   THEN t.NeedPayAmount
//                              END) AS 应收订单量 ,
//                        NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款 ,
//                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN t.Warehouseid
//							ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
//						END AS 发货仓库 ,
//						CASE WHEN t.MerchantID IN ( 8, 9 )
//							 THEN wh.WarehouseName
//							 ELSE ec3.CompanyName
//						END AS 发货仓库名称 ,
//                        SUM(NVL(t.Fare,0)) AS 配送费,
//                        SUM(NVL(t.ProtectedPrice,0)) AS 保价金额,
//                        CASE wh.IsMain
//                          WHEN 0 THEN '是'
//                          WHEN 1 THEN '否'
//                        END 主分仓 ,
//                        CASE t.WaybillType
//                          WHEN '1' THEN '是'
//                          ELSE '否'
//                        END 上门换
//              FROM      t
//                        JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
//                        JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
//                        LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
//                        LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
//                                                              AND ec3.CompanyFlag = 1
//                        LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
//                                                              AND ec4.CompanyFlag = 1
//              GROUP BY  mbi.MerchantName ,
//                        t.RfdAcceptDate ,
//						t.Warehouseid,
//                        t.FinalExpressCompanyID,
//                        t.MerchantID,
//                        ec3.expresscompanyid,
//                        CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END ,");
//                if (IsAreaType)
//                    sb.Append(@"t.AreaType ,");
//                if (IsCod)
//                    sb.Append(@"t.IsCOD ,");
//                sb.Append(@" 
//                        CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.Warehouseid
//                             ELSE cast(ec3.expresscompanyid as VARCHAR2(20))
//                        END ,
//						CASE WHEN t.MerchantID IN ( 8, 9 )
//                         THEN wh.WarehouseName
//                         ELSE ec3.CompanyName
//                        END,
//                        wh.IsMain ,
//                        t.WaybillType");
//            }

//            return sb.ToString();
//        }

        private string GetAreaTypeSql(string areaType)
        {
            string sql = @"
LEFT JOIN AreaExpressLevel ael ON ael.AreaID = fcbi.AreaID
                                 AND ael.IsEnable IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND (ael.WareHouseID= '' or ael.WareHouseID is null)
								{0}
";
            //if (!string.NVLOrEmpty(areaType))
            //    sql = string.Format(sql, " AND ael.AreaType=" + areaType);
            //else
            sql = string.Format(sql, "");
            return sql;
        }

        public DataTable SearchExprotDetailDataV2(string condition, CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 没有导出V2");
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
            //else if (searchCondition.ReportType == "3")
            //{
            //    sql = GetSignedReturnSql(false);
            //    sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType),
            //                        searchCondition.DistributionCode);
            //}

            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql).Tables[0];
        }
      public   DataTable SearchExprotDetailData<T>(string condition, CodSearchCondition searchCondition, List<T> parameterList)
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

          return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql,ToParameters(parameterList.ToArray())).Tables[0];
      }

        public DataTable SearchExprotStatDataV2(string condition, CodSearchCondition searchCondition)
        {
            throw new Exception("oracle 没有导出V2");
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
            //else if(searchCondition.ReportType == "3")
            //{
            //    sql = GetSignedReturnStatSql(searchCondition.DateType, searchCondition.IsAreaType,
            //                                 searchCondition.SummaryByCod);
            //    sql = string.Format(sql, condition, GetAreaTypeSql(searchCondition.AreaType), searchCondition.DistributionCode);

            //}

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        /// <summary>
        /// 支出日报发货查询
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable SearchLogisticsDeliverDailyV2(string condition, string type, CODSearchCondition csc, string accountDateColumnName)
        {
            throw new Exception("oracle 没有查询V2");
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
       CASE WHEN fci.AreaType IS NULL OR fci.AreaType=0 THEN ael.AreaType ELSE fci.AreaType END AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"
       NVL(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(NVL(fci.Fare, 0)) CountFare,
       fci.FareFormula,
       '{0}' AS CountType
FROM   FMS_CODBaseInfo fci
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
            ON  mbi.ID = fci.MerchantID
       LEFT JOIN AreaExpressLevel ael
            ON  ael.AreaID = fci.AreaID
            AND ael.IsEnable IN (1, 2)
            AND ael.expresscompanyid = fci.TopCodCompanyID
            AND ael.MerchantID = fci.MerchantID
            AND (ael.WareHouseID = '' or ael.WareHouseID is null)
WHERE  (1 = 1) AND fci.createtime > trunc(sysdate-90)
       {1}
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN fci.AreaType IS NULL OR fci.AreaType=0 THEN ael.AreaType ELSE fci.AreaType END,
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
       NVL(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(NVL(fci.Fare, 0)) CountFare,
       '{0}' AS CountType
FROM   FMS_CODBaseInfo fci
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
            ON  mbi.ID = fci.MerchantID
WHERE  (1 = 1) AND fci.createtime > trunc(sysdate-90)
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

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }
        public DataTable SearchLogisticsDeliverDaily<T>(string condition, string type, CODSearchCondition csc, string accountDateColumnName, List<T> parameterList)
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
       CASE WHEN fci.AreaType IS NULL OR fci.AreaType=0 THEN ael.AreaType ELSE fci.AreaType END AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"
       NVL(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(NVL(fci.Fare, 0)) CountFare,
       fci.FareFormula,
       '{0}' AS CountType
FROM   FMS_CODBaseInfo fci
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
            ON  mbi.ID = fci.MerchantID
       LEFT JOIN AreaExpressLevel ael
            ON  ael.AreaID = fci.AreaID
            AND ael.IsEnable IN (1, 2)
            AND ael.expresscompanyid = fci.TopCodCompanyID
            AND ael.MerchantID = fci.MerchantID
            AND (ael.WareHouseID = '' or ael.WareHouseID is null)
WHERE  (1 = 1) AND fci.createtime > trunc(sysdate-90)
       {1}
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN fci.AreaType IS NULL OR fci.AreaType=0 THEN ael.AreaType ELSE fci.AreaType END,
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
       NVL(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(NVL(fci.Fare, 0)) CountFare,
       '{0}' AS CountType
FROM   FMS_CODBaseInfo fci
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
            ON  mbi.ID = fci.MerchantID
WHERE  (1 = 1) AND fci.createtime > trunc(sysdate-90)
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

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql,ToParameters(parameterList.ToArray()));
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
            throw new Exception("oracle 没有查询V2");
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
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType not in (2,5)
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
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
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
       CASE WHEN t.AreaType IS NULL OR t.AreaType=0 THEN ael.AreaType ELSE t.AreaType END AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       NVL(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(NVL(t.Fare, 0)) CountFare,
       t.FareFormula,
       '{0}' AS CountType
        from t
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
            ON  mbi.ID = t.MerchantID
       LEFT JOIN AreaExpressLevel ael
            ON  ael.AreaID = t.AreaID
            AND ael.IsEnable IN (1, 2)
            AND ael.expresscompanyid = t.TopCodCompanyID
            AND ael.MerchantID = t.MerchantID
            AND (ael.WareHouseID = '' or ael.WareHouseID is null)
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN t.AreaType IS NULL OR t.AreaType=0 THEN ael.AreaType ELSE t.AreaType END,");
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
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType not in (2,5)
       {1}
union all
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.Fare,
       fci.CreateTime AS ReturnTime,
       fci.IsCOD
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
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
       NVL(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(NVL(t.Fare, 0)) CountFare,
       '{0}' AS CountType
        from t
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
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

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchLogisticsReturnsDaily<T>(string condition, string type, CODSearchCondition csc, string accountDateColumnName, List<T> parameterList)
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
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType not in (2,5)
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
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
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
       CASE WHEN t.AreaType IS NULL OR t.AreaType=0 THEN ael.AreaType ELSE t.AreaType END AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"t.IsCOD, ");
                sqlStr.Append(@"
       NVL(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(NVL(t.Fare, 0)) CountFare,
       t.FareFormula,
       '{0}' AS CountType
        from t
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
            ON  mbi.ID = t.MerchantID
       LEFT JOIN AreaExpressLevel ael
            ON  ael.AreaID = t.AreaID
            AND ael.IsEnable IN (1, 2)
            AND ael.expresscompanyid = t.TopCodCompanyID
            AND ael.MerchantID = t.MerchantID
            AND (ael.WareHouseID = '' or ael.WareHouseID is null)
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN t.AreaType IS NULL OR t.AreaType=0 THEN ael.AreaType ELSE t.AreaType END,");
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
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
WHERE  (1 = 1) AND fci.OperateType not in (2,5)
       {1}
union all
SELECT fci.WaybillNO,
	   fci.MerchantID,
       fci.TopCODCompanyID,
       fci.Fare,
       fci.CreateTime AS ReturnTime,
       fci.IsCOD
FROM   FMS_CODBaseInfo fci
JOIN ExpressCompany ec
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
       NVL(COUNT(t.WaybillNO),0) AS CountNum,
       SUM(NVL(t.Fare, 0)) CountFare,
       '{0}' AS CountType
        from t
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = t.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
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

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql,ToParameters(parameterList.ToArray()));
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }
        public DataTable SearchLogisticsSignedReturnDaily(string condition, string type, CODSearchCondition csc, string accountDateColumnName)
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
       CASE WHEN fci.AreaType IS NULL OR fci.AreaType=0 THEN ael.AreaType ELSE fci.AreaType END AreaType,");
                if (csc.IsCOD)
                    sqlStr.Append(@"fci.IsCOD, ");
                sqlStr.Append(@"
       NVL(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(NVL(fci.Fare, 0)) CountFare,
       fci.FareFormula,
       '{0}' AS CountType
FROM   FMS_CODBaseInfo fci
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
            ON  mbi.ID = fci.MerchantID
       LEFT JOIN AreaExpressLevel ael
            ON  ael.AreaID = fci.AreaID
            AND ael.IsEnable IN (1, 2)
            AND ael.expresscompanyid = fci.TopCodCompanyID
            AND ael.MerchantID = fci.MerchantID
            AND (ael.WareHouseID = '' or ael.WareHouseID is null)
WHERE  (1 = 1)
       {1}
GROUP BY
       mbi.MerchantName,
       ec.ExpressCompanyID,
       ec.AccountCompanyName,
       CASE WHEN fci.AreaType IS NULL OR fci.AreaType=0 THEN ael.AreaType ELSE fci.AreaType END,
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
       NVL(COUNT(fci.WaybillNO),0) AS CountNum,
       SUM(NVL(fci.Fare, 0)) CountFare,
       '{0}' AS CountType
FROM   FMS_CODBaseInfo fci
       JOIN ExpressCompany ec
            ON  ec.ExpressCompanyID = fci.TopCODCompanyID
       JOIN MerchantBaseInfo mbi
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

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable StatCods(string condition, CodSearchCondition searchCondition,  List<OracleParameter> parameters)
        {

            string sql = string.Empty;
            if (searchCondition.ReportType == "")
            {
                sql = @"
WITH t AS (
                --2012.05.22之前
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
               UNION ALL
                --谁接的
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode=':DistributionCode' {0}
               UNION ALL
                --谁配送的
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi 
               JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>':DistributionCode' AND ec.DistributionCode=':DistributionCode' {0}
	)
SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		NVL(SUM(NVL(t.PaidAmount, 0)),0) AS 已收款合计 ,
		COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
				   THEN t.NeedPayAmount
			  END) AS 应收订单量合计 ,
		NVL(SUM(NVL(t.NeedPayAmount, 0)),0) AS 应收款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计,
        NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
        NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t ";
                //sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
                sql = string.Format(sql, condition, GetAreaTypeSql(""));
                parameters.Add(new OracleParameter(":DistributionCode", searchCondition.DistributionCode));
            }
            else if (searchCondition.ReportType == "7")
            {
                sql = @"
WITH    t AS ( 
               --2012.05.22之前查询
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {2}
                UNION ALL
               --谁接的单
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode=':DistributionCode' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi {1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode=':DistributionCode' {2}
                UNION ALL
               --谁配送的
               SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi 
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.OperateType not in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>':DistributionCode' AND ec.DistributionCode=':DistributionCode' {0}
                UNION ALL
			   SELECT   fcbi.INFOID as ID,fcbi.WaybillNO ,fcbi.TotalAmount ,fcbi.PaidAmount ,fcbi.NeedPayAmount ,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
			   FROM     FMS_CODBaseInfo fcbi 
                JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0{1}
			   WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>':DistributionCode' AND ec.DistributionCode=':DistributionCode' {2}
             )
    SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
            NVL(SUM(NVL(t.TotalAmount, 0)), 0) AS 总价合计 ,
            NVL(SUM(NVL(t.PaidAmount, 0)), 0) AS 已收款合计 ,
            COUNT(CASE WHEN NVL(t.NeedPayAmount, 0) > 0
                       THEN t.NeedPayAmount
                  END) AS 应收订单量合计 ,
            NVL(SUM(NVL(t.NeedPayAmount, 0)), 0) AS 应收款合计 ,
            NVL(SUM(NVL(t.AccountWeight, 0)), 0) AS 结算重量合计,
            NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
            NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
    FROM    t
";
                //sql = string.Format(sql, condition, GetAreaTypeSql(""), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"), searchCondition.DistributionCode);
                sql = string.Format(sql, condition, GetAreaTypeSql(""), condition.Replace("fcbi.ReturnTime", "fcbi.CreateTime"));
                parameters.Add(new OracleParameter(":DistributionCode", searchCondition.DistributionCode));
            }
            else if(searchCondition.ReportType == "6")
            {
                sql = @"
WITH t AS (
		        --2012.05.22之前
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime<=to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') {0}
                UNION ALL
                --谁接的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
						{1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode=':DistributionCode' {0}
                UNION ALL
                --谁配送的
                SELECT   fcbi.WaybillNO,fcbi.TotalAmount,fcbi.NeedBackAmount,fcbi.AccountWeight,fcbi.Fare,fcbi.ProtectedPrice
               FROM     FMS_CODBaseInfo fcbi
				JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 {1}
               WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') AND fcbi.DistributionCode<>':DistributionCode' AND ec.DistributionCode=':DistributionCode' {0}
	
	)

SELECT  COUNT(t.WaybillNO) AS 订单量合计 ,
		NVL(SUM(NVL(t.TotalAmount, 0)),0) AS 总价合计 ,
		0.00 AS 已退款合计 ,--NVL(SUM(NVL(fcbi.BackAmount, 0)),0)
		COUNT(CASE WHEN NVL(t.NeedBackAmount, 0) > 0
				   THEN t.NeedBackAmount
			  END) AS 应退订单量合计 ,
		NVL(SUM(NVL(t.NeedBackAmount, 0)),0) AS 应退款合计 ,
		NVL(SUM(NVL(t.AccountWeight, 0)),0) AS 结算重量合计,
        NVL(SUM(NVL(t.Fare, 0)),0) AS 配送费合计,
        NVL(SUM(NVL(t.ProtectedPrice, 0)),0) AS 保价金额合计
FROM    t";
                //sql = string.Format(sql, condition, GetAreaTypeSql(""), searchCondition.DistributionCode);
                sql = string.Format(sql, condition, GetAreaTypeSql(""));
                parameters.Add(new OracleParameter(":DistributionCode", searchCondition.DistributionCode));
            }
                
            //return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql.ToString()).Tables[0];
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql.ToString(),parameters.ToArray()).Tables[0];
        }
    }

}
