using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using RFD.FMS.MODEL;
using Oracle.ApplicationBlocks.Data;
using System.Data.SqlClient;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class WaybillForIncomeBaseInfoDao : OracleDao,IWaybillForIncomeBaseInfoDao
    {
        public DataTable SearchDetails<T>(string conditionStr,List<T> parameterList, ref PageInfo pi, bool isPage)
        {
            StringBuilder sql = new StringBuilder();
            #region sql
            sql.Append(@"
SELECT RowNum AS 序号,
       fifi.IncomeFeeID Fee唯一编号,
       fibi.WaybillNo 运单号,
       fibi.CustomerOrder 订单号,
       fibi.DeliverCode 配送单号,
       s3.StatusName 运单类型,
       mbi2.MerchantName 月结商家名称,
       mbi.MerchantName 商家,
       mbi1.MerchantName 子公司,
       mbi1.MerchantCode 子公司编码,
       d.DistributionName 配送商,
       ec.CompanyName 配送站,
       fibi.RfdAcceptTime 接单时间,
       ec2.CompanyName 发货分拣中心,
       fibi.BackStationTime 归班时间,
       s2.StatusName 运单状态,
       fibi.ReturnTime 拒收入库时间,
       s.StatusName 返货状态,
       fibi.AccountWeight 结算重量,
       CASE WHEN NVL(gc.GoodsCategoryName,'')='' THEN fibi.WaybillCategory ELSE gc.GoodsCategoryName END 货物品类,
       fifi.AccountFare 配送费,
       fifi.AccountStandard 配送费计算公式,
       fibi.NeedPayAmount 应收款,
       NVL(fibi.NeedBackAmount, 0) 应退金额,
       NVL(fibi.ProtectedAmount, 0) 保价金额,
       s1.StatusName 无效状态,
       fibi.AcceptType 支付方式,
       fifi.AreaType 收入区域类型,
       PCA.ProvinceName 省,
       PCA.CityName 市,
       PCA.AreaName 区,
       fibi.ReceiveAddress 地址,
       fifi.ProtectedFee 保价费,
       fifi.CashReceiveServiceFee 现金服务费,
       fifi.ReceiveFee 现金手续费,
       fifi.POSReceiveServiceFee POS服务费,
       fifi.POSReceiveFee POS手续费
FROM   FMS_IncomeBaseInfo fibi
       JOIN FMS_IncomeFeeInfo fifi
            ON fifi.WaybillNo = fibi.WaybillNo
       JOIN MerchantBaseInfo mbi
            ON  mbi.id = fibi.merchantid
       JOIN ExpressCompany ec2
            ON  ec2.expresscompanyid = fibi.ExpressCompanyID
       LEFT JOIN MerchantBaseInfo mbi1
            ON  mbi1.MerchantCode = fibi.OriginDepotNo and mbi1.IsSubMerchant=1
       LEFT JOIN MerchantBaseInfo mbi2
            ON  mbi2.PeriodAccountCode = fibi.PeriodAccountCode  AND NVL(fibi.PeriodAccountCode,'')<>''
       LEFT JOIN ExpressCompany ec--未分配无效时
            ON  ec.ExpressCompanyID = fibi.DeliverStationID
     LEFT JOIN Distribution d--未分配无效时
            ON  d.DistributionCode = ec.DistributionCode
       LEFT JOIN StatusInfo s
             ON s.statusTypeNO = 5 AND CAST(s.StatusNO AS INT) = fibi.SubStatus
       LEFT JOIN StatusInfo s1
            ON s1.statusTypeNO = 308 AND CAST(s1.StatusNO AS INT) = fibi.InefficacyStatus
       LEFT JOIN StatusInfo s2
            ON s2.statusTypeNO = 1 AND s2.StatusNO = fibi.BackStationStatus
       LEFT JOIN StatusInfo s3
      ON s3.statusTypeNO = 2 AND s3.StatusNO = fibi.WaybillType
       LEFT JOIN GoodsCategory gc ON gc.GoodsCategoryCode=fibi.WaybillCategory
       LEFT JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName
                    FROM Area a  JOIN City c
                    ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID
                    WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0) PCA ON PCA.AreaID=fibi.AreaID
WHERE  (1 = 1) AND fibi.CreateTime > trunc(sysdate-90) {0}
");
            if (isPage)
                sql = new StringBuilder("SELECT * FROM ( ").Append(sql.Append(" ) where 序号 BETWEEN :pageStr AND :pageEnd"));
           
            #endregion
            string sqlStr = string.Format(sql.ToString(), conditionStr);
            List<OracleParameter> parameterListTmp = new List<OracleParameter>();
            if (isPage)
            {
                parameterListTmp.Add(new OracleParameter(":pageStr", OracleDbType.Decimal) { Value = pi.CurrentPageStartRowNum });
                parameterListTmp.Add(new OracleParameter(":pageEnd", OracleDbType.Decimal) { Value = pi.CurrentPageEndRowNum });
            }
            var oracleParameterList = parameterList as List<OracleParameter>;
            parameterListTmp.AddRange(oracleParameterList);

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sqlStr, ToParameters(parameterListTmp.ToArray()));
            
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchDetailsV2<T>(string conditionStr, List<T> parameterList, ref PageInfo pi, bool isPage)
        {
            throw new Exception("oracle 中无查询v2");
        }

        public DataTable SearchStat<T>(string conditionStr, List<T> parameterList)
        {
            string sql = @"
                SELECT COUNT(fibi.WaybillNO) 订单量合计,
                       COUNT(
                           CASE 
                                WHEN NVL(fibi.NeedPayAmount, 0) > 0 THEN NVL(fibi.NeedPayAmount, 0)
                           END
                       ) AS 应收订单量合计,
                       SUM(NVL(fibi.NeedPayAmount, 0)) 应收款合计,
                       COUNT(
                           CASE 
                                WHEN NVL(fibi.NeedBackAmount, 0) > 0 THEN NVL(fibi.NeedBackAmount, 0)
                           END
                       ) AS 应退订单量合计,
                       SUM(NVL(fibi.NeedBackAmount, 0)) 应退款合计,
                       SUM(NVL(fibi.ProtectedAmount, 0)) 保价金额合计,
                       SUM(NVL(fibi.AccountWeight, 0)) 结算重量合计,
                       SUM(NVL(fifi.AccountFare, 0)) 配送费合计,
                       SUM(NVL(fifi.POSReceiveServiceFee, 0)) POS服务费合计,
                       SUM(NVL(fifi.CashReceiveServiceFee, 0)+NVL(fifi.ReceiveFee, 0)+NVL(fifi.POSReceiveFee, 0)) 代收手续费合计
                FROM   FMS_IncomeBaseInfo fibi
                       JOIN FMS_IncomeFeeInfo fifi
			                ON fifi.WaybillNo = fibi.WaybillNo
                WHERE  (1 = 1) AND fibi.CreateTime > trunc(sysdate-90) {0}";

            sql = string.Format(sql, conditionStr);

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql,ToParameters(parameterList.ToArray()));

            if (ds == null || ds.Tables.Count <= 0) return null;
                
            return ds.Tables[0];
        }

        public DataTable SearchStatV2<T>(string conditionStr, List<T> parameterList)
        {
            throw new Exception("oracle 中无查询V2");
        }

        public DataTable SearchSummary<T>(string conditionStr, List<T> parameterList, ThirdPartyWaybillSearchConditons condition)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
    SELECT 
       trunc({1}) {2}日期,
       mbi.MerchantName 商家,
       d.DistributionName 配送商,
       ec.CompanyName 配送站,
       s3.StatusName 运单类型,
       s2.StatusName 运单状态,
       fifi.AreaType 收入区域类型,
       COUNT(fibi.WaybillNo) 订单总量,
       SUM(NVL(fibi.AccountWeight,0)) 结算重量,
       SUM(NVL(fifi.AccountFare, 0)) 配送费,
       SUM(NVL(fibi.NeedPayAmount,0)) 应收款,
       SUM(NVL(fibi.NeedBackAmount, 0)) 应退金额,
       SUM(NVL(fibi.ProtectedAmount, 0)) 保价金额,
       fibi.AcceptType 支付方式,
       ec2.CompanyName 发货分拣中心,
       SUM(NVL(fifi.ProtectedFee, 0)) 保价费,
       SUM(NVL(fifi.CashReceiveServiceFee, 0)) 现金服务费,
       SUM(NVL(fifi.ReceiveFee, 0)) 现金手续费,
       SUM(NVL(fifi.POSReceiveServiceFee, 0)) POS服务费,
       SUM(NVL(fifi.POSReceiveFee, 0)) POS手续费
FROM   FMS_IncomeBaseInfo fibi
       JOIN FMS_IncomeFeeInfo fifi
			ON fifi.WaybillNo = fibi.WaybillNo
       JOIN MerchantBaseInfo mbi
            ON  mbi.id = fibi.merchantid
       JOIN ExpressCompany ec2
            ON  ec2.expresscompanyid = fibi.ExpressCompanyID
       LEFT JOIN ExpressCompany ec--未分配无效时
            ON  ec.ExpressCompanyID = fibi.DeliverStationID
	   LEFT JOIN Distribution d--未分配无效时
            ON  d.DistributionCode = ec.DistributionCode
       LEFT JOIN StatusInfo s2
            ON s2.statusTypeNO = 1 AND s2.StatusNO = fibi.BackStationStatus
       LEFT JOIN StatusInfo s3
			ON s3.statusTypeNO = 2 AND s3.StatusNO = fibi.WaybillType
WHERE  (1 = 1) AND fibi.CreateTime > trunc(sysdate-90)  {0}
  
  GROUP BY trunc({1}),
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
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sqlStr,ToParameters(parameterList.ToArray()));

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
                @"SELECT              
                    w.WaybillNO,
                    CASE 
                     WHEN w.MerchantID=4596 
                     THEN (SELECT OrderNO FROM OrderThirdPartyRelation WHERE WaybillNO=w.WaybillNO and RowNum=1)
                     ELSE w.CustomerOrder END CustomerOrder,
                    w.WaybillType,
                    w.WarehouseId,
                    w.Status  ,
                    w.MerchantID,
                    w.CreatStation,
                    ob.OutBoundStation AS FinalExpressCompanyID,
                    w.DeliverStationID,
                    w.CreatTime,
                    w.DeliverTime,
                    w.ReturnTime,
                    w.ReturnWareHouse,
                    CASE WHEN ec1.CompanyFlag=2 THEN ec2.ExpressCompanyID ELSE w.ReturnExpressCompanyId END ReturnExpressCompanyId,
                    wbs.CreatTime AS BackStationTime,
                    wbs.SignStatus AS BackStationStatus,
                    wsi.ProtectedPrice AS ProtectedAmount,
                    CASE 
                        WHEN w.MerchantID IN (8, 9) 
                        THEN (
                                NVL(Amount, 0) + NVL(additionalprice, 0) + 
                                NVL(TransferFee, 0)
                            )
                        WHEN (
                                NVL(Amount, 0) + NVL(additionalprice, 0) + 
                                NVL(TransferFee, 0)
                            ) >=NVL(wsi.NeedAmount,0)+NVL(wsi.PaidAmount,0) THEN (
                                NVL(Amount, 0) + NVL(additionalprice, 0) + 
                                NVL(TransferFee, 0)
                            )
                        ELSE NVL(wsi.NeedAmount, 0) + NVL(wsi.PaidAmount, 0)
                    END AS TotalAmount,
                    wsi.PaidAmount ,
                    wsi.NeedAmount AS NeedPayAmount,
                    wsi.FactBackAmount AS BackAmount,
                    wsi.NeedBackAmount AS NeedBackAmount,
                    CASE fmdf.WeightType
                        WHEN 0 THEN NVL(wi.MerchantWeight, 0)
                        WHEN 1 THEN NVL(wi.WayBillInfoWeight, 0)
                        WHEN 2 THEN CASE 
                                        WHEN NVL(wi.MerchantWeight, 0) > NVL(wi.WayBillInfoVolumeWeight, 0) THEN 
                                            NVL(wi.MerchantWeight, 0)
                                        ELSE NVL(wi.WayBillInfoVolumeWeight, 0)
                                    END
			            WHEN 3 THEN NVL(wi.MerchantWeight, 0)
			            WHEN 4 THEN 0
                        ELSE 0
                    END AS AccountWeight,
                    pca.AreaID,
                    (case wtsi.ReceiveProvince WHEN '北京' THEN '' WHEN '天津' THEN '' WHEN '上海' THEN '' WHEN '重庆' THEN '' else wtsi.ReceiveProvince end)+wtsi.ReceiveCity+wtsi.ReceiveArea+wtsi.ReceiveAddress as ReceiveAddress,
                    wsi.SignType,
                    w.InefficacyStatus,
                    w.ReceiveStationID,
                    w.ReceiveDeliverManID,
                    wsi.TransferPayType,
                    wsi.DeputizeAmount,
                    w.DistributionCode,
                    w.CurrentDistributionCode,
                    wsi.TransferFee,wbs.AcceptType,
                    (SELECT ec.TopCODCompanyID FROM ps_PMS.ExpressCompany ec JOIN ps_PMS.ExpressCompany ec1 ON ec.TopCODCompanyID=ec1.ExpressCompanyID WHERE ec.IsDeleted=0 AND ec.ExpressCompanyID=w.DeliverStationID) AS TopCODCompanyID,
                    w.BackStatus,
                    wi.WayBillInfoWeight,
                    NVL(wi.MerchantWeight, 0) AS MerchantWeight,
                    NVL(wi.FinancialWeight,0) AS FinancialWeight,
                    w.CustomerOrder,
                    wee.OriginDepotNo,
                    wsi.PeriodAccountCode,
                    wi.WaybillCategory,
                    w.DeliverCode,
                    fmdf.IsCategory

                    FROM Waybill w 
                    INNER JOIN WaybillSignInfo wsi ON w.WaybillNO=wsi.WaybillNO
                    LEFT JOIN WaybillBackStation wbs ON w.BackStationID=wbs.WaybillBackStationID
                    INNER JOIN WaybillInfo wi ON w.WaybillNO=wi.WaybillNO
                    INNER JOIN WaybillTakeSendInfo wtsi ON w.WaybillNO=wtsi.WaybillNO
                    LEFT JOIN OutBound ob ON w.OutBoundID=ob.OutBoundID
                    left join WaybillExpressExtend wee on w.WaybillNO=wee.WaybillNo
                    LEFT JOIN ps_PMS.ExpressCompany ec1 ON ec1.ExpressCompanyID=w.ReturnExpressCompanyId AND ec1.CompanyFlag=2
					LEFT JOIN ps_PMS.ExpressCompany ec2 ON ec2.ExpressCompanyID=ec1.ParentID AND ec2.CompanyFlag=1
                    LEFT JOIN (
					            SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a2.AreaID,a2.AreaName 
					            FROM Province p 
                                JOIN City c ON p.ProvinceID = c.ProvinceID
					            JOIN Area a2 ON c.CityID = a2.CityID AND a2.IsDeleted=0
				              ) pca ON pca.ProvinceName=wtsi.ReceiveProvince AND pca.CityName=wtsi.ReceiveCity AND pca.AreaName=wtsi.ReceiveArea
                    WHERE w.WaybillNO=:WaybillNO";

            OracleParameter[] parameters = { new OracleParameter(":WaybillNO", waybillNO) };

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
        }
    }
}
