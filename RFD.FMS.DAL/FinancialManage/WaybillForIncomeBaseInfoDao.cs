using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using RFD.FMS.MODEL;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;
using Microsoft.ApplicationBlocks.Data.Extension;

namespace RFD.FMS.DAL.FinancialManage
{
    public class WaybillForIncomeBaseInfoDao : SqlServerDao, IWaybillForIncomeBaseInfoDao
    {
        public DataTable SearchDetails<T>(string conditionStr,List<T> parameterList, ref PageInfo pi, bool isPage)
        {
            StringBuilder sql = new StringBuilder();
            if (isPage)
                sql.Append(@"
IF ( @records IS NULL
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
SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1 ;");

            sql.Append(@"
create table #tmp_1
(RowNum BIGINT, IncomeID bigint primary key,AccountFare decimal(18,2),
AccountStandard nvarchar(150),MerchantName nvarchar(100),CompanyName nvarchar(100),
ProtectedFee decimal(18,2),CashReceiveServiceFee decimal(18,2),POSReceiveServiceFee decimal(18,2),
ReceiveFee decimal(18,2),POSReceiveFee decimal(18,2),AreaType int)
;WITH T AS (
select  ROW_NUMBER() OVER(ORDER BY fibi.MerchantID DESC) AS RowNum,
		IncomeID,AccountFare,AccountStandard,MerchantName,CompanyName,
        ProtectedFee,CashReceiveServiceFee,POSReceiveServiceFee,
        ReceiveFee,POSReceiveFee,fifi.AreaType
FROM   LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
       JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
			ON fifi.WaybillNo = fibi.WaybillNo
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.id = fibi.merchantid
       JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK)
            ON  ec2.expresscompanyid = fibi.ExpressCompanyID
WHERE  (1 = 1) {0})
insert  into #tmp_1 SELECT * FROM T ");
            if(isPage)
                sql.Append(" WHERE  RowNum BETWEEN @rowStart AND @rowStart + @pageSize - 1");

            sql.Append(@"
SELECT b.RowNum AS 序号,
       fibi.WaybillNo 运单号,
       fibi.CustomerOrder 订单号,
       fibi.DeliverCode 配送单号,
       s3.StatusName 运单类型,
       mbi2.MerchantName 月结商家名称,
       b.MerchantName 商家,
       mbi1.MerchantName 子公司,
       mbi1.MerchantCode 子公司编码,
       d.DistributionName 配送商,
       ec.CompanyName 配送站,
       fibi.RfdAcceptTime 接单时间,
       b.CompanyName 发货分拣中心,
       fibi.BackStationTime 归班时间,
       s2.StatusName 运单状态,
       fibi.ReturnTime 拒收入库时间,
       s.StatusName 返货状态,
       fibi.AccountWeight 结算重量,
       CASE WHEN gc.GoodsCategoryName IS NULL THEN fibi.WaybillCategory ELSE gc.GoodsCategoryName END 货物品类,
       b.AccountFare 配送费,
       b.AccountStandard 配送费计算公式,
       fibi.NeedPayAmount 应收款,
       ISNULL(fibi.NeedBackAmount, 0) 应退金额,
       ISNULL(fibi.ProtectedAmount, 0) 保价金额,
       s1.StatusName 无效状态,
       fibi.AcceptType 支付方式,
       b.AreaType 收入区域类型,
       PCA.ProvinceName 省,
       PCA.CityName 市,
       PCA.AreaName 区,
       fibi.ReceiveAddress 地址,
       b.ProtectedFee 保价费,
       b.CashReceiveServiceFee 现金服务费,
       b.ReceiveFee 现金手续费,
       b.POSReceiveServiceFee POS服务费,
       b.POSReceiveFee POS手续费
FROM   LMS_RFD.dbo.FMS_IncomeBaseInfo fibi with(NOLOCK,forceseek)
		join #tmp_1 b on fibi.IncomeID=b.IncomeID

       LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi1(NOLOCK)
            ON  mbi1.MerchantCode = fibi.OriginDepotNo and mbi1.IsSubMerchant=1
       LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi2(NOLOCK)
            ON  mbi2.PeriodAccountCode = fibi.PeriodAccountCode  AND ISNULL(fibi.PeriodAccountCode,'')<>''
       LEFT JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)--未分配无效时
            ON  ec.ExpressCompanyID = fibi.DeliverStationID
	   LEFT JOIN RFD_PMS.dbo.Distribution d(NOLOCK)--未分配无效时
            ON  d.DistributionCode = ec.DistributionCode
       LEFT JOIN RFD_PMS.dbo.StatusInfo s(NOLOCK)
             ON s.statusTypeNO = 5 AND CAST(s.StatusNO AS INT) = fibi.SubStatus
       LEFT JOIN RFD_PMS.dbo.StatusInfo s1(NOLOCK)
            ON s1.statusTypeNO = 308 AND CAST(s1.StatusNO AS INT) = fibi.InefficacyStatus
       LEFT JOIN RFD_PMS.dbo.StatusInfo s2(NOLOCK)
            ON s2.statusTypeNO = 1 AND s2.StatusNO = fibi.BackStationStatus
       LEFT JOIN RFD_PMS.dbo.StatusInfo s3(NOLOCK)
			ON s3.statusTypeNO = 2 AND s3.StatusNO = fibi.WaybillType
       LEFT JOIN RFD_PMS.dbo.GoodsCategory gc(NOLOCK) ON gc.GoodsCategoryCode=fibi.WaybillCategory
       LEFT JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName
                    FROM RFD_PMS.dbo.Area AS a(nolock)  JOIN RFD_PMS.dbo.City AS c(NOLOCK)
                    ON a.CityID=c.CityID JOIN RFD_PMS.dbo.Province AS p(NOLOCK)  ON c.ProvinceID = p.ProvinceID
                    WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0) PCA ON PCA.AreaID=fibi.AreaID

drop table #tmp_1;
");
            string sqlStr = string.Format(sql.ToString(), conditionStr);

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
            DataSet ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sqlStr, parameters);
            if (isPage)
                pi.PageCount = (int)parameters[1].Value;
            
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchDetailsV2<T>(string conditionStr, List<T> parameterList, ref PageInfo pi, bool isPage)
        {
            StringBuilder sql = new StringBuilder();
            if (isPage)
                sql.Append(@"
IF ( @records IS NULL
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
SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1 ;
WITH    t AS (");

            sql.Append(@"
SELECT ROW_NUMBER() OVER(ORDER BY fibi.MerchantID DESC) AS 序号,
       fibi.WaybillNo 运单号,
       fibi.CustomerOrder 订单号,
       s3.StatusName 运单类型,
       mbi2.MerchantName 月结商家名称,
       mbi.MerchantName 商家,
       mbi1.MerchantName 子公司,
       mbi1.MerchantCode 子公司编码,
       d.DistributionName 结算单位,
       ec.CompanyName 配送站,
       fibi.RfdAcceptTime 接单时间,
       ec2.CompanyName 发货分拣中心,
       fibi.BackStationTime 归班时间,
       s2.StatusName 运单状态,
       fibi.ReturnTime 拒收入库时间,
       s.StatusName 返货状态,
       fibi.AccountWeight 结算重量,
       CASE WHEN gc.GoodsCategoryName IS NULL THEN fibi.WaybillCategory ELSE gc.GoodsCategoryName END 货物品类,
       fifi.AccountFare 配送费,
       fifi.AccountStandard 配送费计算公式,
       fibi.NeedPayAmount 应收款,
       ISNULL(fibi.NeedBackAmount, 0) 应退金额,
       ISNULL(fibi.ProtectedAmount, 0) 保价金额,
       s1.StatusName 无效状态,
       fibi.AcceptType 支付方式,
       ael1.AreaType 收入区域类型,
       PCA.ProvinceName 省,
       PCA.CityName 市,
       PCA.AreaName 区,
       fibi.ReceiveAddress 地址
FROM   FMS_IncomeBaseInfo fibi(NOLOCK)
       JOIN FMS_IncomeFeeInfo fifi(NOLOCK)
			ON fifi.WaybillNo = fibi.WaybillNo
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.id = fibi.merchantid
       JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK)
            ON  ec2.expresscompanyid = fibi.ExpressCompanyID
       LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi1(NOLOCK)
            ON  mbi1.MerchantCode = fibi.OriginDepotNo and mbi1.IsSubMerchant=1
       LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi2(NOLOCK)
            ON  mbi2.PeriodAccountCode = fibi.PeriodAccountCode  AND ISNULL(fibi.PeriodAccountCode,'')<>''
       LEFT JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)--未分配无效时
            ON  ec.ExpressCompanyID = fibi.DeliverStationID
	   LEFT JOIN RFD_PMS.dbo.Distribution d(NOLOCK)--未分配无效时
            ON  d.DistributionCode = ec.DistributionCode
       LEFT JOIN RFD_PMS.dbo.StatusInfo s(NOLOCK)
             ON s.statusTypeNO = 5 AND CAST(s.StatusNO AS INT) = fibi.SubStatus
       LEFT JOIN RFD_PMS.dbo.StatusInfo s1(NOLOCK)
            ON s1.statusTypeNO = 308 AND CAST(s1.StatusNO AS INT) = fibi.InefficacyStatus
       LEFT JOIN RFD_PMS.dbo.StatusInfo s2(NOLOCK)
            ON s2.statusTypeNO = 1 AND s2.StatusNO = fibi.BackStationStatus
       LEFT JOIN RFD_PMS.dbo.StatusInfo s3(NOLOCK)
			ON s3.statusTypeNO = 2 AND s3.StatusNO = fibi.WaybillType
       LEFT JOIN RFD_PMS.dbo.GoodsCategory gc(NOLOCK) ON gc.GoodsCategoryCode=fibi.WaybillCategory
       LEFT JOIN AreaExpressLevelIncome ael1(NOLOCK)
            ON  ael1.AreaID = fibi.AreaID
            AND ael1.[Enable] IN (1, 2)
            AND ael1.MerchantID = fibi.MerchantID
            AND ael1.WareHouseID = fibi.ExpressCompanyID
       LEFT JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName
                    FROM RFD_PMS.dbo.Area AS a  JOIN RFD_PMS.dbo.City AS c(NOLOCK)
                    ON a.CityID=c.CityID JOIN RFD_PMS.dbo.Province AS p(NOLOCK)  ON c.ProvinceID = p.ProvinceID
                    WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0) PCA ON PCA.AreaID=fibi.AreaID
WHERE  (1 = 1) {0}
");
            if (isPage)
                sql.Append(") SELECT * FROM t WHERE  序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1");
            string sqlStr = string.Format(sql.ToString(), conditionStr);

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
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);
            if (isPage)
                pi.PageCount = (int)parameters[1].Value;

            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchStat<T>(string conditionStr, List<T> parameterList)
        {
            string sql = @"
                SELECT COUNT(fibi.WaybillNO) 订单量合计,
                       COUNT(
                           CASE 
                                WHEN ISNULL(fibi.NeedPayAmount, 0) > 0 THEN ISNULL(fibi.NeedPayAmount, 0)
                           END
                       ) AS 应收订单量合计,
                       SUM(ISNULL(fibi.NeedPayAmount, 0)) 应收款合计,
                       COUNT(
                           CASE 
                                WHEN ISNULL(fibi.NeedBackAmount, 0) > 0 THEN ISNULL(fibi.NeedBackAmount, 0)
                           END
                       ) AS 应退订单量合计,
                       SUM(ISNULL(fibi.NeedBackAmount, 0)) 应退款合计,
                       SUM(ISNULL(fibi.ProtectedAmount, 0)) 保价金额合计,
                       SUM(ISNULL(fibi.AccountWeight, 0)) 结算重量合计,
                       SUM(ISNULL(fifi.AccountFare, 0)) 配送费合计
                FROM   LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
                       JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
			                ON fifi.WaybillNo = fibi.WaybillNo
                WHERE  (1 = 1) {0}";

            sql = string.Format(sql, conditionStr);

            DataSet ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql);

            if (ds == null || ds.Tables.Count <= 0) return null;
                
            return ds.Tables[0];
        }

        public DataTable SearchStatV2<T>(string conditionStr, List<T> parameterList)
        {
            string sql = @"
                SELECT COUNT(fibi.WaybillNO) 订单量合计,
                       COUNT(
                           CASE 
                                WHEN ISNULL(fibi.NeedPayAmount, 0) > 0 THEN ISNULL(fibi.NeedPayAmount, 0)
                           END
                       ) AS 应收订单量合计,
                       SUM(ISNULL(fibi.NeedPayAmount, 0)) 应收款合计,
                       COUNT(
                           CASE 
                                WHEN ISNULL(fibi.NeedBackAmount, 0) > 0 THEN ISNULL(fibi.NeedBackAmount, 0)
                           END
                       ) AS 应退订单量合计,
                       SUM(ISNULL(fibi.NeedBackAmount, 0)) 应退款合计,
                       SUM(ISNULL(fibi.ProtectedAmount, 0)) 保价金额合计,
                       SUM(ISNULL(fibi.AccountWeight, 0)) 结算重量合计,
                       SUM(ISNULL(fifi.AccountFare, 0)) 配送费合计
                FROM   FMS_IncomeBaseInfo fibi(NOLOCK)
                       JOIN FMS_IncomeFeeInfo fifi(NOLOCK)
			                ON fifi.WaybillNo = fibi.WaybillNo
                       LEFT JOIN AreaExpressLevelIncome ael1(NOLOCK)
                            ON  ael1.AreaID = fibi.AreaID
                            AND ael1.[Enable] IN (1, 2)
                            AND ael1.MerchantID = fibi.MerchantID
                            AND ael1.WareHouseID = fibi.ExpressCompanyID
                WHERE  (1 = 1) {0}";

            sql = string.Format(sql, conditionStr);

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql);

            if (ds == null || ds.Tables.Count <= 0) return null;

            return ds.Tables[0];
        }

        public DataTable SearchSummary<T>(string conditionStr, List<T> parameterList, ThirdPartyWaybillSearchConditons condition)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
    SELECT 
       CONVERT(NVARCHAR(20),{1},111) {2}日期,
       mbi.MerchantName 商家,
       d.DistributionName 配送商,
       ec.CompanyName 配送站,
       s3.StatusName 运单类型,
       s2.StatusName 运单状态,
       fifi.AreaType 收入区域类型,
       COUNT(fibi.WaybillNo) 订单总量,
       SUM(ISNULL(fibi.AccountWeight,0)) 结算重量,
       SUM(ISNULL(fibi.NeedPayAmount,0)) 应收款,
       SUM(ISNULL(fibi.NeedBackAmount, 0)) 应退金额,
       SUM(ISNULL(fibi.ProtectedAmount, 0)) 保价金额,
       fibi.AcceptType 支付方式,
       ec2.CompanyName 发货分拣中心
FROM   LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
       JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
			ON fifi.WaybillNo = fibi.WaybillNo
       JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK)
            ON  mbi.id = fibi.merchantid
       JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK)
            ON  ec2.expresscompanyid = fibi.ExpressCompanyID
       LEFT JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)--未分配无效时
            ON  ec.ExpressCompanyID = fibi.DeliverStationID
	   LEFT JOIN RFD_PMS.dbo.Distribution d(NOLOCK)--未分配无效时
            ON  d.DistributionCode = ec.DistributionCode
       LEFT JOIN RFD_PMS.dbo.StatusInfo s2(NOLOCK)
            ON s2.statusTypeNO = 1 AND s2.StatusNO = fibi.BackStationStatus
       LEFT JOIN RFD_PMS.dbo.StatusInfo s3(NOLOCK)
			ON s3.statusTypeNO = 2 AND s3.StatusNO = fibi.WaybillType
WHERE  (1 = 1)  {0}
  
  GROUP BY CONVERT(NVARCHAR(20),{1},111),
			mbi.MerchantName,
			d.DistributionName,
			ec.CompanyName,
			s3.StatusName,
			s2.StatusName,
			fifi.AreaType,
			fibi.AcceptType,
			ec2.CompanyName
");
            string summaryDateColumn = string.Empty;
            string summaryDateShow = string.Empty;
            if (condition.DateType == 0)
            {
                summaryDateColumn = "fibi.RfdAcceptTime";
                summaryDateShow = "接单";
            }
            else if (condition.DateType == 1)
            {
                summaryDateColumn = "fibi.BackStationTime";
                summaryDateShow = "归班";
            }
            else
            {
                summaryDateColumn = "fibi.ReturnTime";
                summaryDateShow = "拒收入库";
            }

            string sqlStr = string.Format(sql.ToString(), conditionStr, summaryDateColumn, summaryDateShow);
            DataSet ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sqlStr);

            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        /// <summary>
        /// 通过运单号查询收入结算所需信息 add by wangyongc 2012-04-12
        /// </summary>
        /// <param name="waybillNO">运单号</param>
        /// <returns></returns>
        public DataTable GetWaybillInfoByNOForIncomeBaseInfo(long waybillNO)
        {
            //天猫的单号独立取，规则：对个订单对一个运单，只取其1，多个运单对一个订单，取重复
            string strSql =
                @"SELECT w.WaybillNO,
                    CASE WHEN w.MerchantID=4596 THEN (SELECT TOP 1 OrderNO FROM LMS_RFD.dbo.OrderThirdPartyRelation WHERE WaybillNO=w.WaybillNO) ELSE w.CustomerOrder END CustomerOrder,
                    w.WaybillType,w.WarehouseId,w.[Status],w.MerchantID,w.CreatStation,ob.OutBoundStation AS FinalExpressCompanyID,w.DeliverStationID,w.CreatTime,w.DeliverTime,
                    w.ReturnTime,w.ReturnWareHouse,
                    CASE WHEN ec1.CompanyFlag=2 THEN ec2.ExpressCompanyID ELSE w.ReturnExpressCompanyId END ReturnExpressCompanyId,
                    wbs.CreatTime AS BackStationTime,wbs.SignStatus AS BackStationStatus,wsi.ProtectedPrice AS ProtectedAmount,
                    CASE 
                        WHEN w.MerchantID IN (8, 9) THEN (
                                ISNULL(Amount, 0) + ISNULL(additionalprice, 0) + 
                                ISNULL(TransferFee, 0)
                            )
                        WHEN (
                                ISNULL(Amount, 0) + ISNULL(additionalprice, 0) + 
                                ISNULL(TransferFee, 0)
                            ) >=isnull(wsi.NeedAmount,0)+isnull(wsi.PaidAmount,0) THEN (
                                ISNULL(Amount, 0) + ISNULL(additionalprice, 0) + 
                                ISNULL(TransferFee, 0)
                            )
                        ELSE ISNULL(wsi.NeedAmount, 0) + ISNULL(wsi.PaidAmount, 0)
                    END AS TotalAmount,wsi.PaidAmount ,wsi.NeedAmount AS NeedPayAmount,wsi.FactBackAmount AS BackAmount,wsi.NeedBackAmount AS NeedBackAmount,
                    pca.AreaID,
                    (case wtsi.ReceiveProvince WHEN '北京' THEN '' WHEN '天津' THEN '' WHEN '上海' THEN '' WHEN '重庆' THEN '' else wtsi.ReceiveProvince end)+wtsi.ReceiveCity+wtsi.ReceiveArea+wtsi.ReceiveAddress as ReceiveAddress,
                    wsi.SignType,w.InefficacyStatus,w.ReceiveStationID,w.ReceiveDeliverManID,wsi.TransferPayType,wsi.DeputizeAmount,w.DistributionCode,w.CurrentDistributionCode,wsi.TransferFee,wbs.AcceptType,
                    (SELECT ec.TopCODCompanyID FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) JOIN RFD_PMS.dbo.ExpressCompany ec1(NOLOCK) ON ec.TopCODCompanyID=ec1.ExpressCompanyID WHERE ec.IsDeleted=0 AND ec.ExpressCompanyID=w.DeliverStationID) AS TopCODCompanyID,
                    w.BackStatus,w.CustomerOrder,wee.OriginDepotNo,wsi.PeriodAccountCode,wi.WaybillCategory,
                    w.DeliverCode,wi.WayBillInfoWeight,wi.MerchantWeight,wi.FinancialWeight

                    FROM LMS_RFD.dbo.Waybill w(NOLOCK) 
                    INNER JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO=wsi.WaybillNO
                    LEFT JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON w.BackStationID=wbs.WaybillBackStationID
                    INNER JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON w.WaybillNO=wi.WaybillNO
                    INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) ON w.WaybillNO=wtsi.WaybillNO
                    LEFT JOIN LMS_RFD.dbo.OutBound ob(NOLOCK) ON w.OutBoundID=ob.OutBoundID
                    left join LMS_RFD.dbo.WaybillExpressExtend wee(nolock) on w.WaybillNO=wee.WaybillNo
                    LEFT JOIN RFD_PMS.dbo.ExpressCompany ec1(NOLOCK) ON ec1.ExpressCompanyID=w.ReturnExpressCompanyId AND ec1.CompanyFlag=2
					LEFT JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK) ON ec2.ExpressCompanyID=ec1.ParentID AND ec2.CompanyFlag=1
                    LEFT JOIN (
					            SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a2.AreaID,a2.AreaName 
					            FROM RFD_PMS.dbo.Province p(NOLOCK) 
                                JOIN RFD_PMS.dbo.City c(NOLOCK) ON p.ProvinceID = c.ProvinceID
					            JOIN RFD_PMS.dbo.Area a2(NOLOCK) ON c.CityID = a2.CityID AND a2.IsDeleted=0
				              ) pca ON pca.ProvinceName=wtsi.ReceiveProvince AND pca.CityName=wtsi.ReceiveCity AND pca.AreaName=wtsi.ReceiveArea
                    WHERE w.WaybillNO=@WaybillNO";

            SqlParameter[] parameters = { new SqlParameter("@WaybillNO", waybillNO) };

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
        }
    }
}
