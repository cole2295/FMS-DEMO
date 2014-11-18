using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;


namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    /*
  * (C)Copyright 2011-2012 如风达财务管理系统
  * 
  * 模块名称：配送费结算报表（数据层）
  * 说明：查询指定条件下的配送费结算信息
  * 作者：王勇
  * 创建日期：2011/07/18
  * 修改人：
  * 修改时间：
  * 修改记录：
  */
    public class DeliverFeeStatDao : OracleDao, IDeliverFeeStatDao
    {
        private string strQuerySumDataSql = @"
SELECT  MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,SuccessDeliverFee,Refuse,RefuseDeliverFee,
       SuccessDeliverFee + RefuseDeliverFee+Protectedprice AS SumDeliverFee,MerchantID,ExpressCompanyID,Sources,Protectedprice
FROM   (
SELECT case when mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,mbi.ID AS MerchantID,ec.ExpressCompanyID,gg.Sources,
		                                                CONVERT(VARCHAR(10),:BegTime,120)+'~'+CONVERT(VARCHAR(10),:EndTime,120) AS CreateTime,
                                                       p.ProvinceName,c.CityName,ec.CompanyName,gg.success,
                                                              gg.success*(SELECT NVL(mdf.BasicDeliverFee,0) FROM FMS_MerchantDeliverFee mdf WHERE mdf.MerchantID=gg.MerchantID) AS SuccessDeliverFee,
                                                       gg.Refuse,gg.Refuse*(SELECT NVL(mdf.BasicDeliverFee,0)*mdf.RefuseFeeRate FROM FMS_MerchantDeliverFee mdf WHERE mdf.MerchantID=gg.MerchantID) AS RefuseDeliverFee,
                                                    gg.Protectedprice * (SELECT NVL(mdf.ProtectedParmer,1) FROM FMS_MerchantDeliverFee mdf WHERE  mdf.MerchantID = gg.MerchantID) AS Protectedprice
                                                FROM   (
                                                           SELECT wb.Sources,wb.MerchantID,wbs.DeliverStation,
                                                                  SUM(CASE WHEN wbs.SignStatus = '3' THEN 1 ELSE 0 END) AS success,
                                                                  SUM(CASE WHEN wbs.SignStatus = '5' THEN 1 ELSE 0 END) AS Refuse,
                                                                SUM(NVL(wsi.Protectedprice,0)) AS Protectedprice 
                                                FROM   Waybill wb
                                                                  INNER JOIN WaybillSignInfo wsi ON  wsi.WaybillNO = wb.WaybillNO
                                                                  INNER JOIN WaybillBackStation wbs ON  wsi.BackStationInofID = wbs.WaybillBackStationID
                                                                  JOIN ExpressCompany ec ON  ec.ExpressCompanyID = wb.DeliverStationID
                                                           WHERE  wbs.SignStatus IN ('3', '5')
                                                                  AND wb.Sources=:Sources
                                                                  AND wbs.CreatTime BETWEEN :BegTime AND :EndTime {0}
                                                           GROUP BY
                                                                  wb.MerchantID, wbs.DeliverStation,wb.Sources
                                                       ) AS gg
                                                       LEFT JOIN MerchantBaseInfo mbi ON  gg.MerchantID = mbi.ID
                                                       INNER JOIN StatusInfo si ON si.StatusNO=gg.Sources AND si.StatusTypeNO=3
                                                       INNER JOIN ExpressCompany ec ON  ec.ExpressCompanyID = gg.DeliverStation
                                                       INNER JOIN City c ON  ec.CityID = c.CityID
                                                       INNER JOIN Province p ON  c.ProvinceID = p.ProvinceID
                                               WHERE 1=1 {1} ) as aa";

        private string strQueryDayDataSql = @"
SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,SuccessDeliverFee,Refuse,RefuseDeliverFee,
       SuccessDeliverFee + RefuseDeliverFee AS SumDeliverFee,MerchantID,ExpressCompanyID,Sources
FROM   (
SELECT case when MerchantName IS NULL THEN StatusName ELSE MerchantName END AS MerchantName,gg.CreateTime,p.ProvinceName,c.CityName,ec.CompanyName,gg.success,mbi.ID AS MerchantID,ec.ExpressCompanyID,gg.Sources,
                                                      gg.success*(SELECT NVL(mdf.BasicDeliverFee,0) FROM FMS_MerchantDeliverFee mdf WHERE mdf.MerchantID=gg.MerchantID) AS SuccessDeliverFee,gg.Refuse,
                                                              gg.Refuse*(SELECT NVL(mdf.BasicDeliverFee,0)*mdf.RefuseFeeRate 
                                                FROM FMS_MerchantDeliverFee mdf WHERE mdf.MerchantID=gg.MerchantID) AS RefuseDeliverFee
                                                FROM   (
                                                           SELECT wb.MerchantID,wb.Sources,wbs.DeliverStation,SUM(CASE WHEN wbs.SignStatus = '3' THEN 1 ELSE 0 END) AS success,
                                                                  SUM(CASE WHEN wbs.SignStatus = '5' THEN 1 ELSE 0 END) AS Refuse, CONVERT(CHAR(10),wbs.CreatTime,120) AS CreateTime
                                                           FROM   Waybill wb
                                                                  INNER JOIN WaybillSignInfo wsi ON  wsi.WaybillNO = wb.WaybillNO
                                                                  INNER JOIN WaybillBackStation wbs ON  wsi.BackStationInofID = wbs.WaybillBackStationID
                                                                  JOIN ExpressCompany ec ON  ec.ExpressCompanyID = wb.DeliverStationID
                                                           WHERE  wbs.SignStatus IN ('3', '5') AND wb.Sources=:Sources AND wbs.CreatTime BETWEEN :BegTime AND :EndTime {0}
                                                           GROUP BY
                                                                  wb.MerchantID,wbs.DeliverStation,CONVERT(CHAR(10),wbs.CreatTime,120),wb.Sources
                                                       ) AS gg
                                                       LEFT JOIN MerchantBaseInfo mbi ON  gg.MerchantID = mbi.ID
                                                       INNER JOIN StatusInfo si ON si.StatusNO=gg.Sources AND si.StatusTypeNO=3
                                                       INNER JOIN ExpressCompany ec ON  ec.ExpressCompanyID = gg.DeliverStation
                                                       INNER JOIN City c ON  ec.CityID = c.CityID 
                                                       INNER JOIN Province p ON  c.ProvinceID = p.ProvinceID
                                                WHERE 1=1 {1} 
                                               ) as aaa  ORDER BY CreateTime ";

        private string strQueryDetailSql = @"SELECT top 1000 CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS 商家
                                            ,CONVERT(VARCHAR(10),wbs.CreatTime,120) AS 日期,wtsi.ReceiveProvince as 省份,wtsi.ReceiveCity as 城市,ec.CompanyName as 配送站
                                            ,wb.CustomerOrder AS 订单号,wb.WaybillNO as 运单号,wi.WayBillInfoWeight as 重量,wsi.Amount as 订单金额,wtsi.ReceiveBy as 收货人,wtsi.ReceiveAddress as 收货地址,
                                                   CASE WHEN wbs.SignStatus='3' THEN '妥投' ELSE '拒收' END AS 状态
                                            FROM   Waybill wb
		                                            INNER JOIN WaybillSignInfo wsiON  wb.WaybillNO = wsi.WaybillNO
		                                            INNER JOIN WaybillBackStation wbsON  wsi.BackStationInofID = wbs.WaybillBackStationID
		                                            INNER JOIN WaybillInfo wi ON wb.WaybillNO=wi.WaybillNO		
		                                            INNER JOIN WaybillTakeSendInfo wtsi on wb.WaybillNO=wtsi.WaybillNO	
		                                            LEFT JOIN MerchantBaseInfo mbi ON wb.MerchantID=mbi.ID
		                                            INNER JOIN ExpressCompany ec ON wbs.DeliverStation=ec.ExpressCompanyID
		                                            LEFT JOIN StatusInfo si ON wb.Sources=si.StatusNO AND si.StatusTypeNO=3
                                            WHERE wbs.CreatTime BETWEEN :BegTime AND :EndTime
                                            AND wbs.SignStatus IN ('3', '5')AND wb.Sources =:Sources AND wbs.DeliverStation=:StationID {0}";



        private string strNewSumQueryFeeSql = @"SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,
                                   SuccessDeliverFee,Refuse,RefuseDeliverFee,SuccessDeliverFee + RefuseDeliverFee + Protectedprice AS SumDeliverFee,
                                   MerchantID,ExpressCompanyID,Sources,Protectedprice
                            FROM   (SELECT CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,
                                            mbi.ID AS MerchantID, ec.ExpressCompanyID,gg.Sources,
                                            CONVERT(VARCHAR(10), :BegTime, 120) + '~' + CONVERT(VARCHAR(10),:EndTime, 120) AS CreateTime,
                                              p.ProvinceName,c.CityName,ec.CompanyName,gg.success,gg.SuccessDeliverFee,gg.Refuse,gg.RefuseDeliverFee,gg.Protectedprice
                                       FROM   (
                                                  SELECT fsdfd.Sources,
							                               fsdfd.MerchantID,
							                               fsdfd.StationID AS DeliverStation,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN NVL(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN NVL(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(NVL(fsdfd.Protectedprice, 0)) AS Protectedprice
						                            FROM   FMS_StationDailyFinanceDetails fsdfd
						                            WHERE  fsdfd.Status IN ('3', '5')
							                               AND fsdfd.Sources = :Sources
							                               AND fsdfd.DailyTime BETWEEN :BegTime AND :EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources
                                              ) AS gg
                                              LEFT JOIN MerchantBaseInfo mbi ON  gg.MerchantID = mbi.ID
                                              INNER JOIN StatusInfo si ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN ExpressCompany ec ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN City c ON  ec.CityID = c.CityID
                                              INNER JOIN Province p ON  c.ProvinceID = p.ProvinceID
                                       WHERE  1 = 1 {1} ) AS aa";

        private string strNewSumQueryFeeSqlV2 = @"SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,
                                   SuccessDeliverFee,Refuse,RefuseDeliverFee,SuccessDeliverFee + RefuseDeliverFee + Protectedprice AS SumDeliverFee,
                                   MerchantID,ExpressCompanyID,Sources,Protectedprice
                            FROM   (SELECT CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,
                                            mbi.ID AS MerchantID, ec.ExpressCompanyID,gg.Sources,
                                            CONVERT(VARCHAR(10), :BegTime, 120) + '~' + CONVERT(VARCHAR(10),:EndTime, 120) AS CreateTime,
                                              p.ProvinceName,c.CityName,ec.CompanyName,gg.success,gg.SuccessDeliverFee,gg.Refuse,gg.RefuseDeliverFee,gg.Protectedprice
                                       FROM   (
                                                  SELECT fsdfd.Sources,
							                               fsdfd.MerchantID,
							                               fsdfd.StationID AS DeliverStation,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN NVL(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN NVL(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(NVL(fsdfd.Protectedprice, 0)) AS Protectedprice
						                            FROM   FMS_StationDailyFinanceDetails fsdfd
						                            WHERE  fsdfd.Status IN ('3', '5')
							                               AND fsdfd.Sources = :Sources
							                               AND fsdfd.DailyTime BETWEEN :BegTime AND :EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources
                                              ) AS gg
                                              LEFT JOIN MerchantBaseInfo mbi ON  gg.MerchantID = mbi.ID
                                              INNER JOIN StatusInfo si ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN ExpressCompany ec ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN City c ON  ec.CityID = c.CityID
                                              INNER JOIN Province p ON  c.ProvinceID = p.ProvinceID
                                       WHERE  1 = 1 {1} ) AS aa";

        private string strNewDayQueryFeeSql = @"SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,
                                   SuccessDeliverFee,Refuse,RefuseDeliverFee,SuccessDeliverFee + RefuseDeliverFee + Protectedprice AS SumDeliverFee,
                                   MerchantID,ExpressCompanyID,Sources,Protectedprice
                            FROM   (SELECT CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,
                                            mbi.ID AS MerchantID, ec.ExpressCompanyID,gg.Sources,
                                            CONVERT(VARCHAR(10),gg.CreateTime, 120)  AS CreateTime,
                                              p.ProvinceName,c.CityName,ec.CompanyName,gg.success,gg.SuccessDeliverFee,gg.Refuse,gg.RefuseDeliverFee,gg.Protectedprice
                                       FROM   (
                                                  SELECT fsdfd.Sources,
							                               fsdfd.MerchantID,
							                               fsdfd.StationID AS DeliverStation,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN NVL(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN NVL(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(NVL(fsdfd.Protectedprice, 0)) AS Protectedprice,
                                                           CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)  AS CreateTime
						                            FROM   FMS_StationDailyFinanceDetails fsdfd
						                            WHERE  fsdfd.Status IN ('3', '5')
							                               AND fsdfd.Sources = :Sources
							                               AND fsdfd.DailyTime BETWEEN :BegTime AND :EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources,CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)
                                              ) AS gg
                                              LEFT JOIN MerchantBaseInfo mbi ON  gg.MerchantID = mbi.ID
                                              INNER JOIN StatusInfo si ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN ExpressCompany ec ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN City c ON  ec.CityID = c.CityID
                                              INNER JOIN Province p ON  c.ProvinceID = p.ProvinceID
                                       WHERE  1 = 1 {1} ) AS aa";

        private string strNewDayQueryFeeSqlV2 = @"SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,
                                   SuccessDeliverFee,Refuse,RefuseDeliverFee,SuccessDeliverFee + RefuseDeliverFee + Protectedprice AS SumDeliverFee,
                                   MerchantID,ExpressCompanyID,Sources,Protectedprice
                            FROM   (SELECT CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,
                                            mbi.ID AS MerchantID, ec.ExpressCompanyID,gg.Sources,
                                            CONVERT(VARCHAR(10),gg.CreateTime, 120)  AS CreateTime,
                                              p.ProvinceName,c.CityName,ec.CompanyName,gg.success,gg.SuccessDeliverFee,gg.Refuse,gg.RefuseDeliverFee,gg.Protectedprice
                                       FROM   (
                                                  SELECT fsdfd.Sources,
							                               fsdfd.MerchantID,
							                               fsdfd.StationID AS DeliverStation,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.Status = '3' THEN NVL(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.Status = '5' THEN NVL(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(NVL(fsdfd.Protectedprice, 0)) AS Protectedprice,
                                                           CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)  AS CreateTime
						                            FROM   FMS_StationDailyFinanceDetails fsdfd
						                            WHERE  fsdfd.Status IN ('3', '5')
							                               AND fsdfd.Sources = :Sources
							                               AND fsdfd.DailyTime BETWEEN :BegTime AND :EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources,CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)
                                              ) AS gg
                                              LEFT JOIN MerchantBaseInfo mbi ON  gg.MerchantID = mbi.ID
                                              INNER JOIN StatusInfo si ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN ExpressCompany ec ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN City c ON  ec.CityID = c.CityID
                                              INNER JOIN Province p ON  c.ProvinceID = p.ProvinceID
                                       WHERE  1 = 1 {1} ) AS aa";


        public DataTable GetStationList(string strCityID)
        {
            string strSql =
                @"SELECT ec.ExpressCompanyID,ec.CompanyName FROM ExpressCompany ec WHERE ec.CompanyFlag=2 AND ec.CityID=:CityID";

            List<OracleParameter> sqlParaList = new List<OracleParameter>();
            sqlParaList.Add(new OracleParameter(":CityID", SqlDbType.VarChar) { Value = strCityID });
            return OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
        }

        public DataTable GetQueryDataV2(Hashtable ht)
        {
            string strWhere = "";
            string strS1 = "";
            string strSql = string.Empty;
            if (Convert.ToString(ht["QueryType"]) == "0")
            {
                //strSql=strQuerySumDataSql;
                strSql = strNewSumQueryFeeSqlV2;
            }
            else
            {
                //strSql = strQueryDayDataSql;
                strSql = strNewDayQueryFeeSqlV2;
            }
            if (Convert.ToString(ht["Sources"]) == "2")
            {
                if (!String.IsNullOrEmpty(Convert.ToString(ht["MerchantID"])))
                {
                    strWhere += " and fsdfd.MerchantID=" + Convert.ToString(ht["MerchantID"]) + " ";
                }
            }
            if (!String.IsNullOrEmpty(Convert.ToString(ht["StationID"])))
            {
                strWhere += " and  fsdfd.StationID=" + Convert.ToString(ht["StationID"]) + " ";
            }
            else
            {
                if (!String.IsNullOrEmpty(Convert.ToString(ht["ProvinceID"])))
                {
                    strS1 += " and p.ProvinceID='" + Convert.ToString(ht["ProvinceID"]) + "' ";
                    if (!String.IsNullOrEmpty(Convert.ToString(ht["CityID"])))
                    {
                        strS1 += " and c.CityID='" + Convert.ToString(ht["CityID"]) + "' ";
                    }
                }
            }
            strSql = String.Format(strSql, strWhere, strS1);
            List<OracleParameter> sqlParaList = new List<OracleParameter>();
            sqlParaList.Add(new OracleParameter(":Sources", OracleDbType.Decimal) { Value = ht["Sources"] });
            sqlParaList.Add(new OracleParameter(":BegTime", SqlDbType.DateTime) { Value = ht["BegTime"] });
            sqlParaList.Add(new OracleParameter(":EndTime", SqlDbType.DateTime) { Value = ht["EndTime"] });
            //sqlParaList.Add(new OracleParameter(":QueryS1", SqlDbType.VarChar) { Value = strS1 });
            return OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
        }

        public DataTable GetQueryData(Hashtable ht)
        {
            string strWhere = "";
            string strS1 = "";
            string strSql = string.Empty;
            if (Convert.ToString(ht["QueryType"]) == "0")
            {
                //strSql=strQuerySumDataSql;
                strSql = strNewSumQueryFeeSql;
            }
            else
            {
                //strSql = strQueryDayDataSql;
                strSql = strNewDayQueryFeeSql;
            }
            if (Convert.ToString(ht["Sources"]) == "2")
            {
                if (!String.IsNullOrEmpty(Convert.ToString(ht["MerchantID"])))
                {
                    strWhere += " and fsdfd.MerchantID=" + Convert.ToString(ht["MerchantID"]) + " ";
                }
            }
            if (!String.IsNullOrEmpty(Convert.ToString(ht["StationID"])))
            {
                strWhere += " and  fsdfd.StationID=" + Convert.ToString(ht["StationID"]) + " ";
            }
            else
            {
                if (!String.IsNullOrEmpty(Convert.ToString(ht["ProvinceID"])))
                {
                    strS1 += " and p.ProvinceID='" + Convert.ToString(ht["ProvinceID"]) + "' ";
                    if (!String.IsNullOrEmpty(Convert.ToString(ht["CityID"])))
                    {
                        strS1 += " and c.CityID='" + Convert.ToString(ht["CityID"]) + "' ";
                    }
                }
            }
            strSql = String.Format(strSql, strWhere, strS1);
            List<OracleParameter> sqlParaList = new List<OracleParameter>();
            sqlParaList.Add(new OracleParameter(":Sources", OracleDbType.Decimal) { Value = ht["Sources"] });
            sqlParaList.Add(new OracleParameter(":BegTime", SqlDbType.DateTime) { Value = ht["BegTime"] });
            sqlParaList.Add(new OracleParameter(":EndTime", SqlDbType.DateTime) { Value = ht["EndTime"] });
            //sqlParaList.Add(new OracleParameter(":QueryS1", SqlDbType.VarChar) { Value = strS1 });
            return OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
        }

        public DataTable GetDetail(Hashtable ht)
        {
            string strWhere = "";
            string strSql = strQueryDetailSql;

            if (Convert.ToString(ht["Sources"]) == "2")
            {
                if (!String.IsNullOrEmpty(Convert.ToString(ht["Sources"])))
                {
                    strWhere += " and wb.MerchantID=" + Convert.ToString(ht["MerchantID"]) + " ";
                }
            }
            strSql = String.Format(strSql, strWhere);

            List<OracleParameter> sqlParaList = new List<OracleParameter>();
            sqlParaList.Add(new OracleParameter(":Sources", OracleDbType.Decimal) { Value = ht["Sources"] });
            sqlParaList.Add(new OracleParameter(":BegTime", SqlDbType.DateTime) { Value = ht["BegTime"] });
            sqlParaList.Add(new OracleParameter(":EndTime", SqlDbType.DateTime) { Value = ht["EndTime"] });
            sqlParaList.Add(new OracleParameter(":StationID", OracleDbType.Decimal) { Value = ht["StationID"] });
            return OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
        }
        /// <summary>
        /// add by wangyongc 通过商家来源查询计算出配送费信息
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public decimal? GetDeliverFee(int? MerchantID, decimal? Weight, decimal? Volume, WayBillStatus status, out decimal Protectedprice)
        {
            string strMerchantFeeSql = @"  SELECT fmdf.ID,fmdf.MerchantID,fmdf.PaymentType,fmdf.PaymentPeriod,fmdf.DeliverFeeType,fmdf.DeliverFeePeriod,
                                            fmdf.FeeFactors,fmdf.IsUniformedFee,fmdf.BasicDeliverFee,fmdf.FormulaID,fmdf.FormulaParamters,fmdf.UpdateBy,
                                            fmdf.UpdateTime,fmdf.UpdateCode,fmdf.AuditBy,fmdf.AuditTime,fmdf.AuditCode,fmdf.AuditResult,fmdf.[Status],
                                            fmdf.RefuseFeeRate,fmdf.ReceiveFeeRate,fmdf.PaymentPeriodDate,fmdf.DeliverFeePeriodDate,NVLfmdf.FirstWeight,0) AS FirstWeight,
                                            NVL(fmdf.StatPramer,1) AS StatPramer,NVL(fmdf.AddWeightPrice,0) AS AddWeightPrice,NVL(fmdf.FirstWeightPrice,0) AS FirstWeightPrice
                                            ,NVL(fmdf.VolumeParmer,1) AS VolumeParmer,NVL(fmdf.ProtectedParmer,0) AS ProtectedParmer
                                            FROM FMS_MerchantDeliverFee fmdf  WHERE fmdf.MerchantID=:MerchantID ";

            List<OracleParameter> sqlParaList = new List<OracleParameter>();
            sqlParaList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = MerchantID });

            DataTable dt = OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strMerchantFeeSql, sqlParaList.ToArray()).Tables[0];
            decimal? dResult = decimal.Zero;
            decimal? VolumeWeight = decimal.Zero;
            //decimal Weight = Convert.ToDecimal(ht["Weight"]);
            //decimal Volume = Convert.ToDecimal(ht["Volume"]);

            if (dt.Rows.Count == 1)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToString(dr["FeeFactors"]) == "0")//计费因素为固定费用
                    {
                        dResult = Convert.ToDecimal(dr["BasicDeliverFee"]);
                    }
                    else
                    {
                        if (Convert.ToDecimal(dr["VolumeParmer"]) != 0)
                        {
                            VolumeWeight = Volume / Convert.ToDecimal(dr["VolumeParmer"]);
                        }
                        else
                        {
                            VolumeWeight = Volume;
                        }
                        decimal statP = Convert.ToDecimal(dr["StatPramer"]);
                        if (statP == 0)
                        {
                            statP = 1;
                        }
                        if (VolumeWeight > Weight)
                        {
                            dResult = ((VolumeWeight - Convert.ToDecimal(dr["FirstWeight"])) / statP) * Convert.ToDecimal(dr["AddWeightPrice"]) +
                                      Convert.ToDecimal(dr["FirstWeightPrice"]);
                        }
                        else
                        {

                            dResult = ((Weight - Convert.ToDecimal(dr["FirstWeight"])) / statP) * Convert.ToDecimal(dr["AddWeightPrice"]) +
                                      Convert.ToDecimal(dr["FirstWeightPrice"]);

                        }
                    }
                    if (status == WayBillStatus.Reject)
                    {
                        dResult = dResult * Convert.ToDecimal(dr["RefuseFeeRate"]);
                    }

                }
                Protectedprice = Convert.ToDecimal(dt.Rows[0]["ProtectedParmer"]);
            }
            else
            {
                dResult = 0;
                Protectedprice = 0;
            }

            return dResult;
        }
    }
}
