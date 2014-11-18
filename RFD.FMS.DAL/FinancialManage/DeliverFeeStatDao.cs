using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;


namespace RFD.FMS.DAL.FinancialManage
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
    public class DeliverFeeStatDao : SqlServerDao, IDeliverFeeStatDao
    {
        private string strQuerySumDataSql = @"
SELECT  MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,SuccessDeliverFee,Refuse,RefuseDeliverFee,
       SuccessDeliverFee + RefuseDeliverFee+Protectedprice AS SumDeliverFee,MerchantID,ExpressCompanyID,Sources,Protectedprice
FROM   (
SELECT case when mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,mbi.ID AS MerchantID,ec.ExpressCompanyID,gg.Sources,
		                                                CONVERT(VARCHAR(10),@BegTime,120)+'~'+CONVERT(VARCHAR(10),@EndTime,120) AS CreateTime,
                                                       p.ProvinceName,c.CityName,ec.CompanyName,gg.success,
                                                              gg.success*(SELECT isnull(mdf.BasicDeliverFee,0) FROM RFD_FMS.dbo.FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=gg.MerchantID) AS SuccessDeliverFee,
                                                       gg.Refuse,gg.Refuse*(SELECT isnull(mdf.BasicDeliverFee,0)*mdf.RefuseFeeRate FROM RFD_FMS.dbo.FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=gg.MerchantID) AS RefuseDeliverFee,
                                                    gg.Protectedprice * (SELECT ISNULL(mdf.ProtectedParmer,1) FROM RFD_FMS.dbo.FMS_MerchantDeliverFee mdf(NOLOCK) WHERE  mdf.MerchantID = gg.MerchantID) AS Protectedprice
                                                FROM   (
                                                           SELECT wb.Sources,wb.MerchantID,wbs.DeliverStation,
                                                                  SUM(CASE WHEN wbs.SignStatus = '3' THEN 1 ELSE 0 END) AS success,
                                                                  SUM(CASE WHEN wbs.SignStatus = '5' THEN 1 ELSE 0 END) AS Refuse,
                                                                SUM(isnull(wsi.Protectedprice,0)) AS Protectedprice 
                                                FROM   LMS_RFD.dbo.Waybill wb(NOLOCK)
                                                                  INNER JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON  wsi.WaybillNO = wb.WaybillNO
                                                                  INNER JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON  wsi.BackStationInofID = wbs.WaybillBackStationID
                                                                  JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = wb.DeliverStationID
                                                           WHERE  wbs.SignStatus IN ('3', '5')
                                                                  AND wb.Sources=@Sources
                                                                  AND wbs.CreatTime BETWEEN @BegTime AND @EndTime {0}
                                                           GROUP BY
                                                                  wb.MerchantID, wbs.DeliverStation,wb.Sources
                                                       ) AS gg
                                                       LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON  gg.MerchantID = mbi.ID
                                                       INNER JOIN RFD_PMS.dbo.StatusInfo si(NOLOCK) ON si.StatusNO=gg.Sources AND si.StatusTypeNO=3
                                                       INNER JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = gg.DeliverStation
                                                       INNER JOIN RFD_PMS.dbo.City c(NOLOCK) ON  ec.CityID = c.CityID
                                                       INNER JOIN RFD_PMS.dbo.Province p(NOLOCK) ON  c.ProvinceID = p.ProvinceID
                                               WHERE 1=1 {1} ) as aa";

        private string strQueryDayDataSql = @"
SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,SuccessDeliverFee,Refuse,RefuseDeliverFee,
       SuccessDeliverFee + RefuseDeliverFee AS SumDeliverFee,MerchantID,ExpressCompanyID,Sources
FROM   (
SELECT case when MerchantName IS NULL THEN StatusName ELSE MerchantName END AS MerchantName,gg.CreateTime,p.ProvinceName,c.CityName,ec.CompanyName,gg.success,mbi.ID AS MerchantID,ec.ExpressCompanyID,gg.Sources,
                                                      gg.success*(SELECT isnull(mdf.BasicDeliverFee,0) FROM RFD_FMS.dbo.FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=gg.MerchantID) AS SuccessDeliverFee,gg.Refuse,
                                                              gg.Refuse*(SELECT isnull(mdf.BasicDeliverFee,0)*mdf.RefuseFeeRate 
                                                FROM RFD_FMS.dbo.FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=gg.MerchantID) AS RefuseDeliverFee
                                                FROM   (
                                                           SELECT wb.MerchantID,wb.Sources,wbs.DeliverStation,SUM(CASE WHEN wbs.SignStatus = '3' THEN 1 ELSE 0 END) AS success,
                                                                  SUM(CASE WHEN wbs.SignStatus = '5' THEN 1 ELSE 0 END) AS Refuse, CONVERT(CHAR(10),wbs.CreatTime,120) AS CreateTime
                                                           FROM   LMS_RFD.dbo.Waybill wb(NOLOCK)
                                                                  INNER JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON  wsi.WaybillNO = wb.WaybillNO
                                                                  INNER JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON  wsi.BackStationInofID = wbs.WaybillBackStationID
                                                                  JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = wb.DeliverStationID
                                                           WHERE  wbs.SignStatus IN ('3', '5') AND wb.Sources=@Sources AND wbs.CreatTime BETWEEN @BegTime AND @EndTime {0}
                                                           GROUP BY
                                                                  wb.MerchantID,wbs.DeliverStation,CONVERT(CHAR(10),wbs.CreatTime,120),wb.Sources
                                                       ) AS gg
                                                       LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON  gg.MerchantID = mbi.ID
                                                       INNER JOIN RFD_PMS.dbo.StatusInfo si(NOLOCK) ON si.StatusNO=gg.Sources AND si.StatusTypeNO=3
                                                       INNER JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = gg.DeliverStation
                                                       INNER JOIN RFD_PMS.dbo.City c(NOLOCK) ON  ec.CityID = c.CityID 
                                                       INNER JOIN RFD_PMS.dbo.Province p(NOLOCK) ON  c.ProvinceID = p.ProvinceID
                                                WHERE 1=1 {1} 
                                               ) as aaa  ORDER BY CreateTime ";

        private string strQueryDetailSql = @"SELECT top 1000 CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS 商家
                                            ,CONVERT(VARCHAR(10),wbs.CreatTime,120) AS 日期,wtsi.ReceiveProvince as 省份,wtsi.ReceiveCity as 城市,ec.CompanyName as 配送站
                                            ,wb.CustomerOrder AS 订单号,wb.WaybillNO as 运单号,wi.WayBillInfoWeight as 重量,wsi.Amount as 订单金额,wtsi.ReceiveBy as 收货人,wtsi.ReceiveAddress as 收货地址,
                                                   CASE WHEN wbs.SignStatus='3' THEN '妥投' ELSE '拒收' END AS 状态
                                            FROM   LMS_RFD.dbo.Waybill wb(NOLOCK)
		                                            INNER JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK)ON  wb.WaybillNO = wsi.WaybillNO
		                                            INNER JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK)ON  wsi.BackStationInofID = wbs.WaybillBackStationID
		                                            INNER JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON wb.WaybillNO=wi.WaybillNO		
		                                            INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) on wb.WaybillNO=wtsi.WaybillNO	
		                                            LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON wb.MerchantID=mbi.ID
		                                            INNER JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation=ec.ExpressCompanyID
		                                            LEFT JOIN RFD_PMS.dbo.StatusInfo si(NOLOCK) ON wb.Sources=si.StatusNO AND si.StatusTypeNO=3
                                            WHERE wbs.CreatTime BETWEEN @BegTime AND @EndTime
                                            AND wbs.SignStatus IN ('3', '5')AND wb.Sources =@Sources AND wbs.DeliverStation=@StationID {0}";



        private string strNewSumQueryFeeSql = @"SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,
                                   SuccessDeliverFee,Refuse,RefuseDeliverFee,SuccessDeliverFee + RefuseDeliverFee + Protectedprice AS SumDeliverFee,
                                   MerchantID,ExpressCompanyID,Sources,Protectedprice
                            FROM   (SELECT CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,
                                            mbi.ID AS MerchantID, ec.ExpressCompanyID,gg.Sources,
                                            CONVERT(VARCHAR(10), @BegTime, 120) + '~' + CONVERT(VARCHAR(10),@EndTime, 120) AS CreateTime,
                                              p.ProvinceName,c.CityName,ec.CompanyName,gg.success,gg.SuccessDeliverFee,gg.Refuse,gg.RefuseDeliverFee,gg.Protectedprice
                                       FROM   (
                                                  SELECT fsdfd.Sources,
							                               fsdfd.MerchantID,
							                               fsdfd.StationID AS DeliverStation,
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN isnull(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN isnull(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(ISNULL(fsdfd.Protectedprice, 0)) AS Protectedprice
						                            FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
						                            WHERE  fsdfd.[Status] IN ('3', '5')
							                               AND fsdfd.Sources = @Sources
							                               AND fsdfd.DailyTime BETWEEN @BegTime AND @EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources
                                              ) AS gg
                                              LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON  gg.MerchantID = mbi.ID
                                              INNER JOIN RFD_PMS.dbo.StatusInfo si(NOLOCK) ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN RFD_PMS.dbo.City c(NOLOCK) ON  ec.CityID = c.CityID
                                              INNER JOIN RFD_PMS.dbo.Province p(NOLOCK) ON  c.ProvinceID = p.ProvinceID
                                       WHERE  1 = 1 {1} ) AS aa";

        private string strNewSumQueryFeeSqlV2 = @"SELECT MerchantName,CreateTime,ProvinceName,CityName,CompanyName,success,
                                   SuccessDeliverFee,Refuse,RefuseDeliverFee,SuccessDeliverFee + RefuseDeliverFee + Protectedprice AS SumDeliverFee,
                                   MerchantID,ExpressCompanyID,Sources,Protectedprice
                            FROM   (SELECT CASE WHEN mbi.MerchantName IS NULL THEN si.StatusName ELSE mbi.MerchantName END AS MerchantName,
                                            mbi.ID AS MerchantID, ec.ExpressCompanyID,gg.Sources,
                                            CONVERT(VARCHAR(10), @BegTime, 120) + '~' + CONVERT(VARCHAR(10),@EndTime, 120) AS CreateTime,
                                              p.ProvinceName,c.CityName,ec.CompanyName,gg.success,gg.SuccessDeliverFee,gg.Refuse,gg.RefuseDeliverFee,gg.Protectedprice
                                       FROM   (
                                                  SELECT fsdfd.Sources,
							                               fsdfd.MerchantID,
							                               fsdfd.StationID AS DeliverStation,
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN isnull(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN isnull(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(ISNULL(fsdfd.Protectedprice, 0)) AS Protectedprice
						                            FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
						                            WHERE  fsdfd.[Status] IN ('3', '5')
							                               AND fsdfd.Sources = @Sources
							                               AND fsdfd.DailyTime BETWEEN @BegTime AND @EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources
                                              ) AS gg
                                              LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON  gg.MerchantID = mbi.ID
                                              INNER JOIN RFD_PMS.dbo.StatusInfo si(NOLOCK) ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN RFD_PMS.dbo.City c(NOLOCK) ON  ec.CityID = c.CityID
                                              INNER JOIN RFD_PMS.dbo.Province p(NOLOCK) ON  c.ProvinceID = p.ProvinceID
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
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN isnull(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN isnull(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(ISNULL(fsdfd.Protectedprice, 0)) AS Protectedprice,
                                                           CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)  AS CreateTime
						                            FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
						                            WHERE  fsdfd.[Status] IN ('3', '5')
							                               AND fsdfd.Sources = @Sources
							                               AND fsdfd.DailyTime BETWEEN @BegTime AND @EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources,CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)
                                              ) AS gg
                                              LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON  gg.MerchantID = mbi.ID
                                              INNER JOIN RFD_PMS.dbo.StatusInfo si(NOLOCK) ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN RFD_PMS.dbo.City c(NOLOCK) ON  ec.CityID = c.CityID
                                              INNER JOIN RFD_PMS.dbo.Province p(NOLOCK) ON  c.ProvinceID = p.ProvinceID
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
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN 1 ELSE 0 END) AS success,
							                               SUM(CASE WHEN fsdfd.[Status] = '3' THEN isnull(fsdfd.DeliverFee,0)  ELSE 0 END) AS SuccessDeliverFee,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN 1 ELSE 0 END) AS Refuse,
							                               SUM(CASE WHEN fsdfd.[Status] = '5' THEN isnull(fsdfd.DeliverFee,0) ELSE 0 END) AS RefuseDeliverFee,
							                               SUM(ISNULL(fsdfd.Protectedprice, 0)) AS Protectedprice,
                                                           CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)  AS CreateTime
						                            FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
						                            WHERE  fsdfd.[Status] IN ('3', '5')
							                               AND fsdfd.Sources = @Sources
							                               AND fsdfd.DailyTime BETWEEN @BegTime AND @EndTime {0}
						                            GROUP BY fsdfd.MerchantID,fsdfd.StationID,fsdfd.Sources,CONVERT(VARCHAR(10),fsdfd.DailyTime, 120)
                                              ) AS gg
                                              LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON  gg.MerchantID = mbi.ID
                                              INNER JOIN RFD_PMS.dbo.StatusInfo si(NOLOCK) ON  si.StatusNO = gg.Sources AND si.StatusTypeNO = 3 
                                              INNER JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = gg.DeliverStation
                                              INNER JOIN RFD_PMS.dbo.City c(NOLOCK) ON  ec.CityID = c.CityID
                                              INNER JOIN RFD_PMS.dbo.Province p(NOLOCK) ON  c.ProvinceID = p.ProvinceID
                                       WHERE  1 = 1 {1} ) AS aa";


        public DataTable GetStationList(string strCityID)
        {
            string strSql =
                @"SELECT ec.ExpressCompanyID,ec.CompanyName FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) WHERE ec.CompanyFlag=2 AND ec.CityID=@CityID";

            List<SqlParameter> sqlParaList = new List<SqlParameter>();
            sqlParaList.Add(new SqlParameter("@CityID", SqlDbType.VarChar) { Value = strCityID });
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
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
            List<SqlParameter> sqlParaList = new List<SqlParameter>();
            sqlParaList.Add(new SqlParameter("@Sources", SqlDbType.Int) { Value = ht["Sources"] });
            sqlParaList.Add(new SqlParameter("@BegTime", SqlDbType.DateTime) { Value = ht["BegTime"] });
            sqlParaList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = ht["EndTime"] });
            //sqlParaList.Add(new SqlParameter("@QueryS1", SqlDbType.VarChar) { Value = strS1 });
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
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
            List<SqlParameter> sqlParaList = new List<SqlParameter>();
            sqlParaList.Add(new SqlParameter("@Sources", SqlDbType.Int) { Value = ht["Sources"] });
            sqlParaList.Add(new SqlParameter("@BegTime", SqlDbType.DateTime) { Value = ht["BegTime"] });
            sqlParaList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = ht["EndTime"] });
            //sqlParaList.Add(new SqlParameter("@QueryS1", SqlDbType.VarChar) { Value = strS1 });
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
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

            List<SqlParameter> sqlParaList = new List<SqlParameter>();
            sqlParaList.Add(new SqlParameter("@Sources", SqlDbType.Int) { Value = ht["Sources"] });
            sqlParaList.Add(new SqlParameter("@BegTime", SqlDbType.DateTime) { Value = ht["BegTime"] });
            sqlParaList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = ht["EndTime"] });
            sqlParaList.Add(new SqlParameter("@StationID", SqlDbType.Int) { Value = ht["StationID"] });
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0];
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
                                            fmdf.RefuseFeeRate,fmdf.ReceiveFeeRate,fmdf.PaymentPeriodDate,fmdf.DeliverFeePeriodDate,isnull(fmdf.FirstWeight,0) AS FirstWeight,
                                            isnull(fmdf.StatPramer,1) AS StatPramer,isnull(fmdf.AddWeightPrice,0) AS AddWeightPrice,isnull(fmdf.FirstWeightPrice,0) AS FirstWeightPrice
                                            ,isnull(fmdf.VolumeParmer,1) AS VolumeParmer,isnull(fmdf.ProtectedParmer,0) AS ProtectedParmer
                                            FROM RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK)  WHERE fmdf.MerchantID=@MerchantID ";

            List<SqlParameter> sqlParaList = new List<SqlParameter>();
            sqlParaList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = MerchantID });

            DataTable dt = SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strMerchantFeeSql, sqlParaList.ToArray()).Tables[0];
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
