using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Model.UnionPay;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.AudiMgmt;
using Microsoft.ApplicationBlocks.Data.Extension;

namespace RFD.FMS.DAL.AudiMgmt
{
	/*
	* (C)Copyright 2011-2012 如风达信息管理系统
	* 
	* 模块名称：财务收款查询（数据层）
	* 说明：查询指定时间段内站点的财务汇总数据和明细数据
	* 作者：何名宇
	* 创建日期：2011/07/13
	* 修改人：
	* 修改时间：
	* 修改记录：      
	*/
    public class FinanceDao : SqlServerDao, IFinanceDao
	{
        private static readonly string CashForPayment = EnumHelper.GetDescription(PaymentType.Cash);
        private static readonly string POSForPayment = EnumHelper.GetDescription(PaymentType.POS);
        private static readonly int BatchCount = ConfigurationManager.AppSettings["BatchCount"] == null ? 10000 : int.Parse(ConfigurationManager.AppSettings["BatchCount"]);

        /// <summary>GetTotalFinanceData
        /// 根据查询条件获取成功的运单汇总数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceData(SearchCondition condition, bool displayTotalCount)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表

            //拼装可选条件
            var strWhere = new StringBuilder();

            //wangyongc 2011-08-19 站点条件
            if (condition.DeliverStation != 0)
            {
                strWhere.Append(" AND wbs.DeliverStation = @DeliverStation ");

                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }

            //hemingyu 2011-08-24 商家条件 update by zengwei 20120509 20120817
            if (condition.Source == 0)
            {
                if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
                {
                    switch (condition.MerchantIDs)
                    {
                        case "1":
                            {
                                strWhere.Append(" AND ISNULL(w.ComeFrom,0) NOT IN (18,19) ");//避免NULL的查询差异

                                break;
                            }
                        case "2":
                            {
                                strWhere.Append(" AND w.ComeFrom IN (18,19) ");

                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            else if (condition.Source == 2)
            {
                if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
                {
                    strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
                }
            }

            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND w.Sources = @Source ");

                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }

            int MerchantIDs = 0;

            int.TryParse(condition.MerchantIDs, out MerchantIDs);

            paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = MerchantIDs });

            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");

                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }

            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");

                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }

            //账期中的配送公司的查询
            if (!string.IsNullOrEmpty(condition.PeriodExpressNexus))
            {
                if (int.Parse(condition.PeriodExpressNexus) == (int)EnumPeriodExpressNexus.N1)
                {
                    strWhere.Append(" AND wbs.DeliverStation in (" + condition.PeriodExpressIds + ")");
                }

                if (int.Parse(condition.PeriodExpressNexus) == (int)EnumPeriodExpressNexus.N2)
                {
                    strWhere.Append(" AND wbs.DeliverStation not in (" + condition.PeriodExpressIds + ")");
                }
            }

            var strSql = new StringBuilder();

            //拼装必选条件
            if (displayTotalCount)
            {
                #region 总汇总
                //选择来源时
                if (condition.Source != -1)
                {
                    #region 没有选择来源
                    strSql.AppendFormat(
                    @"SELECT t.CashWaybillCount, t.POSWaybillCount, (t.CashWaybillCount + t.POSWaybillCount) 'SucessWaybillCount',
							t.AcceptAmount, t.BackWaybillCount, t.CashRealOutSum, (t.AcceptAmount - t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT  
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.FactAmount,0)) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(isnull(wsi.FactBackAmount,0)) AS 'CashRealOutSum'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID   
                        JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID 
                        JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID               
					   WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
						 AND   wbs.SignStatus = @SignStatus  AND w.IsDelete = 0 and w.DistributionCode = @DistributionCode
                         AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2                  
						 {0}
						 {1}   --来源为vancl,vjia时筛选配送商为如风达
					 ) t 
					", strWhere.ToString(), condition.Source > 1 ? "" : " AND ec.DistributionCode = @DistributionCode");
                    #endregion
                }
                else
                {
                    #region 选择来源
                    strSql.AppendFormat(@"    
					 SELECT SUM(t.CashWaybillCount) 'CashWaybillCount', 
                            SUM(t.POSWaybillCount)  'POSWaybillCount', 
                            SUM(t.CashWaybillCount) + SUM(t.POSWaybillCount) 'SucessWaybillCount',
							SUM(t.AcceptAmount) 'AcceptAmount', 
                            SUM(t.BackWaybillCount) 'BackWaybillCount', 
                            SUM(t.CashRealOutSum) 'CashRealOutSum', 
                            SUM(t.AcceptAmount) - SUM(t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT  
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.FactAmount,0)) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(isnull(wsi.FactBackAmount,0)) AS 'CashRealOutSum'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID 
JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID 
JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID               
					   WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
						 AND   wbs.SignStatus = @SignStatus  AND w.IsDelete = 0                   
						 {0}
						 AND   w.Sources IN(0,1)  AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
                         AND  ec.DistributionCode = @DistributionCode
				   UNION ALL
                     SELECT  
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.FactAmount,0)) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(isnull(wsi.FactBackAmount,0)) AS 'CashRealOutSum'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID 
                        JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID 
                        INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID               
					   WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
						 AND   wbs.SignStatus = @SignStatus  AND w.IsDelete = 0                   
						 {0}
						 AND   w.Sources =2  AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
                    AND   ec.DistributionCode <> @DistributionCode and w.DistributionCode = @DistributionCode 
                    UNION ALL
                     SELECT  
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.FactAmount,0)) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(isnull(wsi.FactBackAmount,0)) AS 'CashRealOutSum'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID 
                        JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID 
                        INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID               
					   WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
						 AND   wbs.SignStatus = @SignStatus  AND w.IsDelete = 0                   
						 {0}
						 AND   w.Sources =2  AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
                    AND   ec.DistributionCode=@DistributionCode and w.DistributionCode = @DistributionCode
                     ) t 
					", strWhere.ToString());
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 站点汇总
                strSql.AppendFormat(@"      
				SELECT (ROW_NUMBER() OVER (ORDER BY {0} {1})) AS 'ID', 
                       CONVERT(VARCHAR(20), t.ExpressCompanyID) + '&' + t.CompanyName + '&' 
					   + CASE WHEN t.Sources=0 THEN CONVERT(VARCHAR(20),@MerchantID) else CONVERT(VARCHAR(20), t.MerchantID) end  + '&' + t.SourceName + '&'
                       + CONVERT(VARCHAR(20), t.Sources) + '&' + t.CreateTime+'&'+t.DistributionCode AS 'QueryParams',
                        DistributionName, CompanyName, MerchantCode,SourceName, 
                        isnull(CashWaybillCount,0) as CashWaybillCount , 
                        isnull(POSWaybillCount,0) as POSWaybillCount,
                        isnull(AcceptAmount,0) as AcceptAmount,
                        isnull(BackWaybillCount,0) as BackWaybillCount,
                        isnull(CashRealOutSum,0) as  CashRealOutSum, 
                        isnull(SaveAmount,0) as SaveAmount, 
                        isnull(CreateTime,0) as CreateTime 
                 FROM(
					   SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                               MAX(DistributionName) 'DistributionName',
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName) AS 'CompanyName', 
							   w.Sources,
							   MAX(si.StatusName) AS 'SourceName',
							   0 AS 'MerchantID',
                               '' AS 'MerchantCode',
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(wsi.FactAmount) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(wsi.FactBackAmount) AS 'CashRealOutSum',
							   (SUM(isnull(wsi.FactAmount,0)) - SUM(isnull(wsi.FactBackAmount,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				   LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID
                   LEFT JOIN   RFD_PMS.dbo.StatusInfo si(NOLOCK) ON w.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                   LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
               LEFT  JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                       WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime                       
						{2}
						AND    w.Sources IN(0,1) AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
						AND    wbs.SignStatus = @SignStatus AND ec.DistributionCode=@DistributionCode  AND w.IsDelete=0
				     GROUP BY  CONVERT(CHAR(10),wbs.CreatTime,120), ec.ExpressCompanyID, w.Sources        
				    UNION ALL
					   SELECT  MAX(ec.DistributionCode), 
                               MAX(DistributionName),
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName), 
							   MAX(w.Sources),
							   MAX(mbi.MerchantName),
							   mbi.ID,
                               Max(mbi.MerchantCode) AS 'MerchantCode',
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(wsi.FactAmount) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(wsi.FactBackAmount) AS 'CashRealOutSum',
							   (SUM(isnull(wsi.FactAmount,0)) - SUM(isnull(wsi.FactBackAmount,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				   LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID   
				   LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                       WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                        AND    w.Sources = 2
						{2} 
                         AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
                        AND    wbs.SignStatus = @SignStatus AND ec.DistributionCode=@DistributionCode  AND w.IsDelete=0
			       GROUP BY    CONVERT(CHAR(10),wbs.CreatTime,120), ec.ExpressCompanyID, mbi.ID
                    --add by wangyongc 2011-10-25 增加了配送商的查询
                    UNION ALL
                    SELECT     MAX(ec.DistributionCode) 'DistributionCode', 
                               MAX(DistributionName) 'DistributionName',
                                0 as 'ExpressCompanyID',
							   MAX(DistributionName) AS 'CompanyName', 
							   w.Sources,							   
							   CASE WHEN w.Sources=2 THEN MAX(mbi.MerchantName) else							   
							   MAX(si.StatusName) END AS 'SourceName',
                               mbi.ID AS 'MerchantID',
                               Max(mbi.MerchantCode) AS 'MerchantCode',
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.FactAmount,0)) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(isnull(wsi.FactBackAmount,0)) AS 'CashRealOutSum',
							   (SUM(isnull(wsi.FactAmount,0)) - SUM(isnull(wsi.FactBackAmount,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				   LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID
                   LEFT JOIN   RFD_PMS.dbo.StatusInfo si(NOLOCK) ON w.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                   LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                       WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime 
                         AND   w.Sources = 2   						
						 {2}
                          AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
						 AND   wbs.SignStatus = @SignStatus AND w.DistributionCode=@DistributionCode AND ec.DistributionCode<>@DistributionCode AND w.IsDelete=0
				     GROUP BY  CONVERT(CHAR(10),wbs.CreatTime,120), ec.DistributionCode, w.Sources, mbi.ID
				 ) t ORDER BY  {0} {1}
            ", condition.OrderBy, condition.Direction, strWhere.ToString());
                #endregion
            }
            //添加参数
            paramList.Add(new SqlParameter("@Cash", SqlDbType.NVarChar, 20) { Value = CashForPayment });
            paramList.Add(new SqlParameter("@POS", SqlDbType.NVarChar, 20) { Value = POSForPayment });
            paramList.Add(new SqlParameter("@Returned", SqlDbType.NVarChar, 20) { Value = ((int)WayBillType.Returned).ToString() });
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //返回结果
            return SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNew(SearchCondition condition, bool displayTotalCount)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            var strVanclAndvjiaWhere = new StringBuilder();
            //wangyongc 2011-08-19 站点条件
            if (condition.DeliverStation != 0)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.StationID=@DeliverStation ");
                strWhere.Append(" AND wbs.DeliverStation = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //hemingyu 2011-08-24 商家条件 update by zengwei 20120509
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strVanclAndvjiaWhere.Append(string.Format(" AND fsdfd.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
                strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }
            //支付方式
            if (condition.PayType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.AcceptType = @AcceptType ");
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }

            //拼装来源
            if (condition.Source != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.Sources = @Source ");
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }


            //if(!String.IsNullOrEmpty(condition.DistributionCode))
            //{
            //    strVanclAndvjiaWhere.Append(" AND fsdfd.DistributionCode = @Source ");
            //    strWhere.Append(" AND w.DistributionCode = @Source ");
            //    paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //}
            var strSql = new StringBuilder();
            //拼装必选条件
            if (displayTotalCount)
            {
                #region
                //选择来源时
                if (condition.Source != -1)
                {
                    strSql.AppendFormat(@"    
					 SELECT t.CashWaybillCount, t.POSWaybillCount, (t.CashWaybillCount + t.POSWaybillCount) 'SucessWaybillCount',
							t.AcceptAmount, t.BackWaybillCount, t.CashRealOutSum, (t.AcceptAmount - t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {0}
						 {1}   --来源为vancl,vjia时筛选配送商为如风达
					 ) t 
					", strVanclAndvjiaWhere.ToString(), condition.Source > 1 ? "" : " AND ec.DistributionCode = @DistributionCode");
                }
                else
                {
                    strSql.AppendFormat(@"    
					 SELECT SUM(t.CashWaybillCount) 'CashWaybillCount', 
                            SUM(t.POSWaybillCount)  'POSWaybillCount', 
                            SUM(t.CashWaybillCount) + SUM(t.POSWaybillCount) 'SucessWaybillCount',
							SUM(t.AcceptAmount) 'AcceptAmount', 
                            SUM(t.BackWaybillCount) 'BackWaybillCount', 
                            SUM(t.CashRealOutSum) 'CashRealOutSum', 
                            SUM(t.AcceptAmount) - SUM(t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
                        AND fsdfd.Sources in (0,1)
                         AND   ec.DistributionCode = @DistributionCode
				   UNION ALL
                     SELECT  
                            SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
						 AND   fsdfd.Sources =2 AND ec.DistributionCode=@DistributionCode 
                    UNION ALL
                     SELECT  
                            SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
						 AND   fsdfd.Sources =2  AND ec.DistributionCode<>@DistributionCode
                     ) t 
					", strWhere.ToString(), strVanclAndvjiaWhere);
                }
                #endregion
            }
            else
            {
                strSql.AppendFormat(@"      
				SELECT (ROW_NUMBER() OVER (ORDER BY {0} {1})) AS 'ID', 
                       CONVERT(VARCHAR(20), t.ExpressCompanyID) + '&' + t.CompanyName + '&' 
					   + CONVERT(VARCHAR(20), t.MerchantID) + '&' + t.SourceName + '&'
                       + CONVERT(VARCHAR(20), t.Sources) + '&' + t.CreateTime+'&'+t.DistributionCode AS 'QueryParams',
                        DistributionName, CompanyName, MerchantCode,SourceName, 
                        isnull(CashWaybillCount,0) as CashWaybillCount , 
                        isnull(POSWaybillCount,0) as POSWaybillCount,
                        isnull(AcceptAmount,0) as AcceptAmount,
                        isnull(BackWaybillCount,0) as BackWaybillCount,
                        isnull(CashRealOutSum,0) as  CashRealOutSum, 
                        isnull(SaveAmount,0) as SaveAmount, 
                        isnull(CreateTime,0) as CreateTime 
                 FROM(
					   SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                               MAX(d.DistributionName) as  'DistributionName',
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName) AS 'CompanyName', 
							   fsdfd.Sources,
							   CASE WHEN fsdfd.Sources=1 THEN 'Vjia' ELSE 'VANCL' END AS SourceName,
							   --MAX(si.StatusName) AS 'SourceName',
							   0 AS 'MerchantID',
                               '' AS 'MerchantCode',
							   SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
							   SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
							   (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
				        FROM LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
						JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
						JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
                       WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
						AND    fsdfd.Sources IN(0,1)
                        {3}
						AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode=@DistributionCode -- AND w.IsDelete=0
				     GROUP BY  fsdfd.DailyTime, ec.ExpressCompanyID, fsdfd.Sources      	          
				    UNION ALL
					   SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                            MAX(d.DistributionName) as  'DistributionName',
                            ec.ExpressCompanyID,
		                    MAX(ec.CompanyName) AS 'CompanyName', 
		                    fsdfd.Sources,
		                    MAX(mbi.MerchantName) AS 'SourceName',
		                    --MAX(si.StatusName) AS 'SourceName',
		                    mbi.ID AS 'MerchantID',
                            Max(mbi.MerchantCode) AS 'MerchantCode',
		                    SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
		                    SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
		                    SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
		                    SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
		                    SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
		                    (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
		                    CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
                    FROM LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
	                    JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
	                    JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
	                    JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
	                     JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON fsdfd.MerchantID = mbi.ID   
                        WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
						                    AND    fsdfd.Sources=2
                                            {3}
						                    AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode=@DistributionCode --AND w.IsDelete=0                    
                    GROUP BY    CONVERT(CHAR(10),fsdfd.DailyTime,120), ec.ExpressCompanyID,fsdfd.Sources, mbi.ID
                    --add by wangyongc 2011-10-25 增加了配送商的查询
                    UNION ALL
                    SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                            MAX(d.DistributionName) as  'DistributionName',
                            0 as ExpressCompanyID,
		                    MAX(DistributionName) AS 'CompanyName', 
                            fsdfd.Sources,
                           MAX(mbi.MerchantName) AS 'SourceName',
                            mbi.ID AS 'MerchantID',
                            Max(mbi.MerchantCode) AS 'MerchantCode',
		                    SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
		                    SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
		                    SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
		                    SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
		                    SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
		                    (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
		                    CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
                    FROM LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
	                    JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
	                    JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
	                    JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
	                     JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON fsdfd.MerchantID = mbi.ID   
                        WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
					AND    fsdfd.Sources=2
                    {3}
					AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode<>@DistributionCode --AND w.IsDelete=0
				     GROUP BY  CONVERT(CHAR(10),fsdfd.DailyTime,120), ec.DistributionCode, fsdfd.Sources, mbi.ID
				 ) t ORDER BY  {0} {1}
            ", condition.OrderBy, condition.Direction, strWhere.ToString(), strVanclAndvjiaWhere);
            }
            //添加参数
            paramList.Add(new SqlParameter("@Cash", SqlDbType.NVarChar, 20) { Value = CashForPayment });
            paramList.Add(new SqlParameter("@POS", SqlDbType.NVarChar, 20) { Value = POSForPayment });
            paramList.Add(new SqlParameter("@Returned", SqlDbType.NVarChar, 20) { Value = ((int)WayBillType.Returned).ToString() });
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //返回结果
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNewV2(SearchCondition condition, bool displayTotalCount)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            var strVanclAndvjiaWhere = new StringBuilder();
            //wangyongc 2011-08-19 站点条件
            if (condition.DeliverStation != 0)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.StationID=@DeliverStation ");
                strWhere.Append(" AND wbs.DeliverStation = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //hemingyu 2011-08-24 商家条件 update by zengwei 20120509
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strVanclAndvjiaWhere.Append(string.Format(" AND fsdfd.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
                strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }
            //支付方式
            if (condition.PayType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.AcceptType = @AcceptType ");
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }

            //拼装来源
            if (condition.Source != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.Sources = @Source ");
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }


            //if(!String.IsNullOrEmpty(condition.DistributionCode))
            //{
            //    strVanclAndvjiaWhere.Append(" AND fsdfd.DistributionCode = @Source ");
            //    strWhere.Append(" AND w.DistributionCode = @Source ");
            //    paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //}
            var strSql = new StringBuilder();
            //拼装必选条件
            if (displayTotalCount)
            {
                #region
                //选择来源时
                if (condition.Source != -1)
                {
                    strSql.AppendFormat(@"    
					 SELECT t.CashWaybillCount, t.POSWaybillCount, (t.CashWaybillCount + t.POSWaybillCount) 'SucessWaybillCount',
							t.AcceptAmount, t.BackWaybillCount, t.CashRealOutSum, (t.AcceptAmount - t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {0}
						 {1}   --来源为vancl,vjia时筛选配送商为如风达
					 ) t 
					", strVanclAndvjiaWhere.ToString(), condition.Source > 1 ? "" : " AND ec.DistributionCode = @DistributionCode");
                }
                else
                {
                    strSql.AppendFormat(@"    
					 SELECT SUM(t.CashWaybillCount) 'CashWaybillCount', 
                            SUM(t.POSWaybillCount)  'POSWaybillCount', 
                            SUM(t.CashWaybillCount) + SUM(t.POSWaybillCount) 'SucessWaybillCount',
							SUM(t.AcceptAmount) 'AcceptAmount', 
                            SUM(t.BackWaybillCount) 'BackWaybillCount', 
                            SUM(t.CashRealOutSum) 'CashRealOutSum', 
                            SUM(t.AcceptAmount) - SUM(t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
                        AND fsdfd.Sources in (0,1)
                         AND   ec.DistributionCode = @DistributionCode
				   UNION ALL
                     SELECT  
                            SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
						 AND   fsdfd.Sources =2 AND ec.DistributionCode=@DistributionCode 
                    UNION ALL
                     SELECT  
                            SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
						 AND   fsdfd.Sources =2  AND ec.DistributionCode<>@DistributionCode
                     ) t 
					", strWhere.ToString(), strVanclAndvjiaWhere);
                }
                #endregion
            }
            else
            {
                strSql.AppendFormat(@"      
				SELECT (ROW_NUMBER() OVER (ORDER BY {0} {1})) AS 'ID', 
                       CONVERT(VARCHAR(20), t.ExpressCompanyID) + '&' + t.CompanyName + '&' 
					   + CONVERT(VARCHAR(20), t.MerchantID) + '&' + t.SourceName + '&'
                       + CONVERT(VARCHAR(20), t.Sources) + '&' + t.CreateTime+'&'+t.DistributionCode AS 'QueryParams',
                        DistributionName, CompanyName, MerchantCode,SourceName, 
                        isnull(CashWaybillCount,0) as CashWaybillCount , 
                        isnull(POSWaybillCount,0) as POSWaybillCount,
                        isnull(AcceptAmount,0) as AcceptAmount,
                        isnull(BackWaybillCount,0) as BackWaybillCount,
                        isnull(CashRealOutSum,0) as  CashRealOutSum, 
                        isnull(SaveAmount,0) as SaveAmount, 
                        isnull(CreateTime,0) as CreateTime 
                 FROM(
					   SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                               MAX(d.DistributionName) as  'DistributionName',
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName) AS 'CompanyName', 
							   fsdfd.Sources,
							   CASE WHEN fsdfd.Sources=1 THEN 'Vjia' ELSE 'VANCL' END AS SourceName,
							   --MAX(si.StatusName) AS 'SourceName',
							   0 AS 'MerchantID',
                               '' AS 'MerchantCode',
							   SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
							   SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
							   (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
				        FROM FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
						JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
						JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
                       WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
						AND    fsdfd.Sources IN(0,1)
                        {3}
						AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode=@DistributionCode -- AND w.IsDelete=0
				     GROUP BY  fsdfd.DailyTime, ec.ExpressCompanyID, fsdfd.Sources      	          
				    UNION ALL
					   SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                            MAX(d.DistributionName) as  'DistributionName',
                            ec.ExpressCompanyID,
		                    MAX(ec.CompanyName) AS 'CompanyName', 
		                    fsdfd.Sources,
		                    MAX(mbi.MerchantName) AS 'SourceName',
		                    --MAX(si.StatusName) AS 'SourceName',
		                    mbi.ID AS 'MerchantID',
                            Max(mbi.MerchantCode) AS 'MerchantCode',
		                    SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
		                    SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
		                    SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
		                    SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
		                    SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
		                    (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
		                    CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
                    FROM FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
	                    JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
	                    JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
	                    JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
	                     JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON fsdfd.MerchantID = mbi.ID   
                        WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
						                    AND    fsdfd.Sources=2
                                            {3}
						                    AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode=@DistributionCode --AND w.IsDelete=0                    
                    GROUP BY    CONVERT(CHAR(10),fsdfd.DailyTime,120), ec.ExpressCompanyID,fsdfd.Sources, mbi.ID
                    --add by wangyongc 2011-10-25 增加了配送商的查询
                    UNION ALL
                    SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                            MAX(d.DistributionName) as  'DistributionName',
                            0 as ExpressCompanyID,
		                    MAX(DistributionName) AS 'CompanyName', 
                            fsdfd.Sources,
                           MAX(mbi.MerchantName) AS 'SourceName',
                            mbi.ID AS 'MerchantID',
                            Max(mbi.MerchantCode) AS 'MerchantCode',
		                    SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
		                    SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
		                    SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
		                    SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
		                    SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
		                    (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
		                    CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
                    FROM FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
	                    JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
	                    JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
	                    JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
	                     JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON fsdfd.MerchantID = mbi.ID   
                        WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
					AND    fsdfd.Sources=2
                    {3}
					AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode<>@DistributionCode --AND w.IsDelete=0
				     GROUP BY  CONVERT(CHAR(10),fsdfd.DailyTime,120), ec.DistributionCode, fsdfd.Sources, mbi.ID
				 ) t ORDER BY  {0} {1}
            ", condition.OrderBy, condition.Direction, strWhere.ToString(), strVanclAndvjiaWhere);
            }
            //添加参数
            paramList.Add(new SqlParameter("@Cash", SqlDbType.NVarChar, 20) { Value = CashForPayment });
            paramList.Add(new SqlParameter("@POS", SqlDbType.NVarChar, 20) { Value = POSForPayment });
            paramList.Add(new SqlParameter("@Returned", SqlDbType.NVarChar, 20) { Value = ((int)WayBillType.Returned).ToString() });
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //返回结果
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据 (其他配送商使用)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNewDis(SearchCondition condition, bool displayTotalCount)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            var strVanclAndvjiaWhere = new StringBuilder();
            //wangyongc 2011-08-19 站点条件
            if (condition.DeliverStation != 0)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.StationID=@DeliverStation ");
                strWhere.Append(" AND wbs.DeliverStation = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //hemingyu 2011-08-24 商家条件 update by zengwei 20120509
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strVanclAndvjiaWhere.Append(string.Format(" AND fsdfd.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
                strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }
            //支付方式
            if (condition.PayType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.AcceptType = @AcceptType ");
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }

            //拼装来源
            if (condition.Source != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.Sources = @Source ");
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }


            //if(!String.IsNullOrEmpty(condition.DistributionCode))
            //{
            //    strVanclAndvjiaWhere.Append(" AND fsdfd.DistributionCode = @Source ");
            //    strWhere.Append(" AND w.DistributionCode = @Source ");
            //    paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //}
            var strSql = new StringBuilder();
            //拼装必选条件
            if (displayTotalCount)
            {
                #region
                //选择来源时
                if (condition.Source != -1)
                {
                    strSql.AppendFormat(@"    
					 SELECT t.CashWaybillCount, t.POSWaybillCount, (t.CashWaybillCount + t.POSWaybillCount) 'SucessWaybillCount',
							t.AcceptAmount, t.BackWaybillCount, t.CashRealOutSum, (t.AcceptAmount - t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {0}
						 {1}   --来源为vancl,vjia时筛选配送商为如风达
					 ) t 
					", strVanclAndvjiaWhere.ToString(), condition.Source > 1 ? "" : " AND ec.DistributionCode = @DistributionCode");
                }
                else
                {
                    strSql.AppendFormat(@"    
					 SELECT SUM(t.CashWaybillCount) 'CashWaybillCount', 
                            SUM(t.POSWaybillCount)  'POSWaybillCount', 
                            SUM(t.CashWaybillCount) + SUM(t.POSWaybillCount) 'SucessWaybillCount',
							SUM(t.AcceptAmount) 'AcceptAmount', 
                            SUM(t.BackWaybillCount) 'BackWaybillCount', 
                            SUM(t.CashRealOutSum) 'CashRealOutSum', 
                            SUM(t.AcceptAmount) - SUM(t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
                         AND   ec.DistributionCode = @DistributionCode
				   UNION ALL
                     SELECT  
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(wsi.FactAmount) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(wsi.FactBackAmount) AS 'CashRealOutSum'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID 
                        INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID               
					   WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
						 AND   wbs.SignStatus = @SignStatus  AND w.IsDelete = 0                   
						 {0}
						 AND   w.Sources =2 AND   w.DistributionCode = @DistributionCode
                     ) t 
					", strWhere.ToString(), strVanclAndvjiaWhere);
                }
                #endregion
            }
            else
            {
                strSql.AppendFormat(@"      
				SELECT (ROW_NUMBER() OVER (ORDER BY {0} {1})) AS 'ID', 
                       CONVERT(VARCHAR(20), t.ExpressCompanyID) + '&' + t.CompanyName + '&' 
					   + CONVERT(VARCHAR(20), t.MerchantID) + '&' + t.SourceName + '&'
                       + CONVERT(VARCHAR(20), t.Sources) + '&' + t.CreateTime+'&'+t.DistributionCode AS 'QueryParams',
                        DistributionName, CompanyName, MerchantCode,SourceName, 
                        isnull(CashWaybillCount,0) as CashWaybillCount , 
                        isnull(POSWaybillCount,0) as POSWaybillCount,
                        isnull(AcceptAmount,0) as AcceptAmount,
                        isnull(BackWaybillCount,0) as BackWaybillCount,
                        isnull(CashRealOutSum,0) as  CashRealOutSum, 
                        isnull(SaveAmount,0) as SaveAmount, 
                        isnull(CreateTime,0) as CreateTime 
                 FROM(
					   SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                               MAX(d.DistributionName) as  'DistributionName',
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName) AS 'CompanyName', 
							   fsdfd.Sources,
							   CASE WHEN fsdfd.Sources=1 THEN 'Vjia' ELSE 'VANCL' END AS SourceName,
							   --MAX(si.StatusName) AS 'SourceName',
							   0 AS 'MerchantID',
                               '' AS 'MerchantCode',
							   SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
							   SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
							   (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
				        FROM LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
						JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
						JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
                       WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
						AND    fsdfd.Sources IN(0,1)
                        {3}
						AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode=@DistributionCode -- AND w.IsDelete=0
				     GROUP BY  fsdfd.DailyTime, ec.ExpressCompanyID, fsdfd.Sources      	          
				    UNION ALL
					   SELECT  MAX(ec.DistributionCode), 
                               MAX(DistributionName),
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName), 
							   MAX(w.Sources),
							   MAX(mbi.MerchantName),
							   mbi.ID,
                               Max(mbi.MerchantCode) AS 'MerchantCode',
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(wsi.FactAmount) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(wsi.FactBackAmount) AS 'CashRealOutSum',
							   (SUM(isnull(wsi.FactAmount,0)) - SUM(isnull(wsi.FactBackAmount,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				   LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID   
				   LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                    INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                       WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                        AND    w.Sources = 2
						{2} 
                        AND    wbs.SignStatus = @SignStatus AND ec.DistributionCode=@DistributionCode  AND w.IsDelete=0                     
			       GROUP BY    CONVERT(CHAR(10),wbs.CreatTime,120), ec.ExpressCompanyID, mbi.ID
                    --add by wangyongc 2011-10-25 增加了配送商的查询
                    UNION ALL
                    SELECT     MAX(ec.DistributionCode) 'DistributionCode', 
                               MAX(DistributionName) 'DistributionName',
                                0 as 'ExpressCompanyID',
							   MAX(DistributionName) AS 'CompanyName', 
							   w.Sources,							   
							   CASE WHEN w.Sources=2 THEN MAX(mbi.MerchantName) else							   
							   MAX(si.StatusName) END AS 'SourceName',
                               mbi.ID AS 'MerchantID',
                               Max(mbi.MerchantCode) AS 'MerchantCode',
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.FactAmount,0)) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(isnull(wsi.FactBackAmount,0)) AS 'CashRealOutSum',
							   (SUM(isnull(wsi.FactAmount,0)) - SUM(isnull(wsi.FactBackAmount,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				   LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID
                   LEFT JOIN   RFD_PMS.dbo.StatusInfo si(NOLOCK) ON w.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                   LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                    INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                       WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime 
                         AND   w.Sources = 2   						
						 {2}
						 AND   wbs.SignStatus = @SignStatus AND w.DistributionCode=@DistributionCode AND ec.DistributionCode<>@DistributionCode AND w.IsDelete=0
				     GROUP BY  CONVERT(CHAR(10),wbs.CreatTime,120), ec.DistributionCode, w.Sources, mbi.ID
				 ) t ORDER BY  {0} {1}
            ", condition.OrderBy, condition.Direction, strWhere.ToString(), strVanclAndvjiaWhere);
            }
            //添加参数
            paramList.Add(new SqlParameter("@Cash", SqlDbType.NVarChar, 20) { Value = CashForPayment });
            paramList.Add(new SqlParameter("@POS", SqlDbType.NVarChar, 20) { Value = POSForPayment });
            paramList.Add(new SqlParameter("@Returned", SqlDbType.NVarChar, 20) { Value = ((int)WayBillType.Returned).ToString() });
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //返回结果
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据 (其他配送商使用)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNewDisV2(SearchCondition condition, bool displayTotalCount)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            var strVanclAndvjiaWhere = new StringBuilder();
            //wangyongc 2011-08-19 站点条件
            if (condition.DeliverStation != 0)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.StationID=@DeliverStation ");
                strWhere.Append(" AND wbs.DeliverStation = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //hemingyu 2011-08-24 商家条件 update by zengwei 20120509
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strVanclAndvjiaWhere.Append(string.Format(" AND fsdfd.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
                strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }
            //支付方式
            if (condition.PayType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.AcceptType = @AcceptType ");
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }

            //拼装来源
            if (condition.Source != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.Sources = @Source ");
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }


            //if(!String.IsNullOrEmpty(condition.DistributionCode))
            //{
            //    strVanclAndvjiaWhere.Append(" AND fsdfd.DistributionCode = @Source ");
            //    strWhere.Append(" AND w.DistributionCode = @Source ");
            //    paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //}
            var strSql = new StringBuilder();
            //拼装必选条件
            if (displayTotalCount)
            {
                #region
                //选择来源时
                if (condition.Source != -1)
                {
                    strSql.AppendFormat(@"    
					 SELECT t.CashWaybillCount, t.POSWaybillCount, (t.CashWaybillCount + t.POSWaybillCount) 'SucessWaybillCount',
							t.AcceptAmount, t.BackWaybillCount, t.CashRealOutSum, (t.AcceptAmount - t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {0}
						 {1}   --来源为vancl,vjia时筛选配送商为如风达
					 ) t 
					", strVanclAndvjiaWhere.ToString(), condition.Source > 1 ? "" : " AND ec.DistributionCode = @DistributionCode");
                }
                else
                {
                    strSql.AppendFormat(@"    
					 SELECT SUM(t.CashWaybillCount) 'CashWaybillCount', 
                            SUM(t.POSWaybillCount)  'POSWaybillCount', 
                            SUM(t.CashWaybillCount) + SUM(t.POSWaybillCount) 'SucessWaybillCount',
							SUM(t.AcceptAmount) 'AcceptAmount', 
                            SUM(t.BackWaybillCount) 'BackWaybillCount', 
                            SUM(t.CashRealOutSum) 'CashRealOutSum', 
                            SUM(t.AcceptAmount) - SUM(t.CashRealOutSum) 'SaveAmount' 
					   FROM ( 
					  SELECT SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 
                               'CashWaybillCount',
                               SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 
                               'POSWaybillCount',
                               SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
                               SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 
                               'BackWaybillCount',
                               SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum'
                        FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)       
                               JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON  fsdfd.DeliverManID = e.EmployeeID
                               JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON  fsdfd.StationID = ec.ExpressCompanyID
                        WHERE  fsdfd.DailyTime >= @BeginTime
                               AND fsdfd.DailyTime < @EndTime
                               AND fsdfd.[Status] = @SignStatus                 
						 {1}
                         AND   ec.DistributionCode = @DistributionCode
				   UNION ALL
                     SELECT  
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(wsi.FactAmount) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(wsi.FactBackAmount) AS 'CashRealOutSum'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID 
                        INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID               
					   WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
						 AND   wbs.SignStatus = @SignStatus  AND w.IsDelete = 0                   
						 {0}
						 AND   w.Sources =2 AND   w.DistributionCode = @DistributionCode
                     ) t 
					", strWhere.ToString(), strVanclAndvjiaWhere);
                }
                #endregion
            }
            else
            {
                strSql.AppendFormat(@"      
				SELECT (ROW_NUMBER() OVER (ORDER BY {0} {1})) AS 'ID', 
                       CONVERT(VARCHAR(20), t.ExpressCompanyID) + '&' + t.CompanyName + '&' 
					   + CONVERT(VARCHAR(20), t.MerchantID) + '&' + t.SourceName + '&'
                       + CONVERT(VARCHAR(20), t.Sources) + '&' + t.CreateTime+'&'+t.DistributionCode AS 'QueryParams',
                        DistributionName, CompanyName, MerchantCode,SourceName, 
                        isnull(CashWaybillCount,0) as CashWaybillCount , 
                        isnull(POSWaybillCount,0) as POSWaybillCount,
                        isnull(AcceptAmount,0) as AcceptAmount,
                        isnull(BackWaybillCount,0) as BackWaybillCount,
                        isnull(CashRealOutSum,0) as  CashRealOutSum, 
                        isnull(SaveAmount,0) as SaveAmount, 
                        isnull(CreateTime,0) as CreateTime 
                 FROM(
					   SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                               MAX(d.DistributionName) as  'DistributionName',
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName) AS 'CompanyName', 
							   fsdfd.Sources,
							   CASE WHEN fsdfd.Sources=1 THEN 'Vjia' ELSE 'VANCL' END AS SourceName,
							   --MAX(si.StatusName) AS 'SourceName',
							   0 AS 'MerchantID',
                               '' AS 'MerchantCode',
							   SUM(CASE WHEN fsdfd.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN fsdfd.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(fsdfd.PriceDiff) AS 'AcceptAmount',
							   SUM(CASE WHEN fsdfd.WaybillType = 2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(fsdfd.PriceReturnCash) AS 'CashRealOutSum',
							   (SUM(isnull(fsdfd.PriceDiff,0)) - SUM(isnull(fsdfd.PriceReturnCash,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),fsdfd.DailyTime,120) as 'CreateTime'
				        FROM FMS_StationDailyFinanceDetails fsdfd(NOLOCK)             
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID	
						JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON fsdfd.DeliverManID=e.EmployeeID
						JOIN RFD_PMS.dbo.Distribution d(NOLOCK) ON ec.DistributionCode=d.DistributionCode
                       WHERE   fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
						AND    fsdfd.Sources IN(0,1)
                        {3}
						AND    fsdfd.[Status] = @SignStatus AND ec.DistributionCode=@DistributionCode -- AND w.IsDelete=0
				     GROUP BY  fsdfd.DailyTime, ec.ExpressCompanyID, fsdfd.Sources      	          
				    UNION ALL
					   SELECT  MAX(ec.DistributionCode), 
                               MAX(DistributionName),
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName), 
							   MAX(w.Sources),
							   MAX(mbi.MerchantName),
							   mbi.ID,
                               Max(mbi.MerchantCode) AS 'MerchantCode',
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(wsi.FactAmount) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(wsi.FactBackAmount) AS 'CashRealOutSum',
							   (SUM(isnull(wsi.FactAmount,0)) - SUM(isnull(wsi.FactBackAmount,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				   LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID   
				   LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                    INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                       WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                        AND    w.Sources = 2
						{2} 
                        AND    wbs.SignStatus = @SignStatus AND ec.DistributionCode=@DistributionCode  AND w.IsDelete=0                     
			       GROUP BY    CONVERT(CHAR(10),wbs.CreatTime,120), ec.ExpressCompanyID, mbi.ID
                    --add by wangyongc 2011-10-25 增加了配送商的查询
                    UNION ALL
                    SELECT     MAX(ec.DistributionCode) 'DistributionCode', 
                               MAX(DistributionName) 'DistributionName',
                                0 as 'ExpressCompanyID',
							   MAX(DistributionName) AS 'CompanyName', 
							   w.Sources,							   
							   CASE WHEN w.Sources=2 THEN MAX(mbi.MerchantName) else							   
							   MAX(si.StatusName) END AS 'SourceName',
                               mbi.ID AS 'MerchantID',
                               Max(mbi.MerchantCode) AS 'MerchantCode',
							   SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
							   SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.FactAmount,0)) AS 'AcceptAmount',
							   SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
							   SUM(isnull(wsi.FactBackAmount,0)) AS 'CashRealOutSum',
							   (SUM(isnull(wsi.FactAmount,0)) - SUM(isnull(wsi.FactBackAmount,0))) AS 'SaveAmount',
							   CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				   LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID
                   LEFT JOIN   RFD_PMS.dbo.StatusInfo si(NOLOCK) ON w.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                   LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                    INNER JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                       WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime 
                         AND   w.Sources = 2   						
						 {2}
						 AND   wbs.SignStatus = @SignStatus AND w.DistributionCode=@DistributionCode AND ec.DistributionCode<>@DistributionCode AND w.IsDelete=0
				     GROUP BY  CONVERT(CHAR(10),wbs.CreatTime,120), ec.DistributionCode, w.Sources, mbi.ID
				 ) t ORDER BY  {0} {1}
            ", condition.OrderBy, condition.Direction, strWhere.ToString(), strVanclAndvjiaWhere);
            }
            //添加参数
            paramList.Add(new SqlParameter("@Cash", SqlDbType.NVarChar, 20) { Value = CashForPayment });
            paramList.Add(new SqlParameter("@POS", SqlDbType.NVarChar, 20) { Value = POSForPayment });
            paramList.Add(new SqlParameter("@Returned", SqlDbType.NVarChar, 20) { Value = ((int)WayBillType.Returned).ToString() });
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            //返回结果
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceData(SearchCondition condition)
        {
            return GetTotalFinanceData(condition, false);
        }

        /// <summary>
        /// 根据查询条件获取每天的统计数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDailyData(SearchCondition condition)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            var strWhere = new StringBuilder();
            //拼装可选条件
            if (!condition.BeginTime.IsNullData() && !condition.EndTime.IsNullData())
            {
                strWhere.Append(" AND sdf.DailyTime >= @BeginTime AND sdf.DailyTime < @EndTime ");
                paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
                paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            }
            if (condition.DeliverStation != 0)
            {
                strWhere.Append(" AND sdf.StationID = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            if (condition.MerchantID != 0)
            {
                strWhere.Append(" AND sdf.MerchantID = @MerchantID ");
                paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND sdf.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            var strSql = new StringBuilder();
            //拼装必选条件
            strSql.AppendFormat(@" 
               SELECT ROW_NUMBER() OVER (ORDER BY {0} {1}) AS 'ID', TotalDataID,
                      CONVERT(VARCHAR(20), t.ExpressCompanyID) + '&' + t.CompanyName + '&' 
					  + CONVERT(VARCHAR(20), t.MerchantID) + '&' + t.SourceName + '&'
                      + CONVERT(VARCHAR(20), t.Sources) + '&' + t.DailyTime AS 'QueryParams',
                      {3} FROM 
                      (SELECT sdf.ID AS 'TotalDataID',
                              ec.ExpressCompanyID,
                              ec.CompanyName, 
							  ISNULL(mbi.MerchantName, si.StatusName) 'SourceName',
							  ISNULL(mbi.ID, 0) 'MerchantID',
							  sdf.Sources,
							  ISNULL(CashSuccOrderCount, 0) 'CashSuccOrderCount',
							  ISNULL(PosSuccOrderCount, 0) 'PosSuccOrderCount',
							  (CASE WHEN @AcceptType = @Cash THEN ISNULL(CashSuccOrderCount, 0) 
                                    WHEN @AcceptType = @POS THEN ISNULL(PosSuccOrderCount, 0)
                                    ELSE ISNULL(CashSuccOrderCount, 0)+ISNULL(PosSuccOrderCount, 0) END) 'TotalOrderCount',
							  ISNULL(CashRealInSum, 0) 'CashRealInSum',
							  ISNULL(PosRealInSum, 0) 'PosRealInSum',
					          (CASE WHEN @AcceptType = @Cash THEN ISNULL(CashRealInSum, 0) 
                                    WHEN @AcceptType = @POS THEN ISNULL(PosRealInSum, 0)
                                    ELSE ISNULL(CashRealInSum, 0)+ISNULL(PosRealInSum, 0) END) 'AcceptAmount',
                              ISNULL(ExchangeOrderCount, 0) 'ExchangeOrderCount',
							  ISNULL(DayOutOrderCount, 0) 'DayOutOrderCount',
							  ISNULL(ExchangeOrderCount, 0) + ISNULL(DayOutOrderCount, 0) 'TotalBackCount',
							  ISNULL(ExchangeOrderSum, 0) 'ExchangeOrderSum',
							  ISNULL(CashRealOutSum, 0) 'CashRealOutSum',
							  ISNULL(ExchangeOrderSum, 0) + ISNULL(CashRealOutSum, 0) 'TotalBackAmount',
							  RealInCome,
						      (CASE WHEN @AcceptType = @Cash THEN ISNULL(CashRealInSum, 0) - ISNULL(CashRealOutSum, 0)
                                    WHEN @AcceptType = @POS THEN ISNULL(PosRealInSum, 0) - ISNULL(CashRealOutSum, 0)
                                    ELSE ISNULL(CashRealInSum, 0) + ISNULL(PosRealInSum, 0) - ISNULL(CashRealOutSum, 0) END) 'SaveAmount',
						   	  (CASE WHEN FinanceStatus IS NULL THEN '未收款'
                                WHEN FinanceStatus = 0 THEN '未收款'
                                ELSE '已收款' END) 'FinanceStatus', 
							  CONVERT(CHAR(10), sdf.DailyTime,120) 'DailyTime'
						FROM  LMS_RFD.dbo.FMS_StationDailyFinanceSum sdf(NOLOCK)
						JOIN  RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON sdf.StationID = ec.ExpressCompanyID
				   LEFT JOIN  RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON sdf.MerchantID = mbi.ID 
				   LEFT JOIN  RFD_PMS.dbo.StatusInfo si(NOLOCK) ON sdf.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
					   WHERE  1=1 {2}) t", condition.OrderBy, condition.Direction, strWhere.ToString(),
                                             condition.IsRawData ? @"CompanyName,SourceName,CashSuccOrderCount,CashRealInSum,PosSuccOrderCount,
                      PosRealInSum,TotalOrderCount,AcceptAmount,DayOutOrderCount,ExchangeOrderCount,TotalBackCount,ExchangeOrderSum,CashRealOutSum,TotalBackAmount,
                      SaveAmount,FinanceStatus,RealInCome,DailyTime" : @"CompanyName,SourceName,CashSuccOrderCount,PosSuccOrderCount,
                      CashRealInSum,PosRealInSum,AcceptAmount,DayOutOrderCount,CashRealOutSum,
                      SaveAmount,FinanceStatus,RealInCome,DailyTime");
            //支付方式
            paramList.Add(new SqlParameter("@AcceptType", SqlDbType.Int) { Value = condition.PayType });
            paramList.Add(new SqlParameter("@Cash", SqlDbType.Int) { Value = (int)PaymentType.Cash });
            paramList.Add(new SqlParameter("@POS", SqlDbType.Int) { Value = (int)PaymentType.POS });
            //返回结果
            return SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }
        /// <summary>
        /// 根据查询条件获取成功的运单明细数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceData(SearchCondition condition)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }

            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }
            //拼装配送站
            if (condition.DeliverStation != 0)
            {
                strWhere.Append(" AND wbs.DeliverStation = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //账期中的配送公司的查询
            if (!string.IsNullOrEmpty(condition.PeriodExpressNexus))
            {
                if (int.Parse(condition.PeriodExpressNexus) == (int)EnumPeriodExpressNexus.N1)
                {
                    strWhere.Append(" AND wbs.DeliverStation in (" + condition.PeriodExpressIds + ")");
                }

                if (int.Parse(condition.PeriodExpressNexus) == (int)EnumPeriodExpressNexus.N2)
                {
                    strWhere.Append(" AND wbs.DeliverStation not in (" + condition.PeriodExpressIds + ")");
                }
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (condition.Source == 0)
            {
                if (!string.IsNullOrEmpty(condition.MerchantID.ToString()))
                {
                    switch (condition.MerchantID)
                    {
                        case 1:
                            strWhere.Append(" AND ISNULL(w.ComeFrom,0) NOT IN (18,19) ");//避免null 的差异
                            break;
                        case 2:
                            strWhere.Append(" AND w.ComeFrom IN (18,19) ");
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (condition.Source == 2)
            {
                if (condition.MerchantID != 0)
                {
                    strWhere.Append(" AND w.MerchantID = @MerchantID ");
                    paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
                }
            }
            //add by wangyongc 2011-10-25 增加配送商条件的判断
            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                strWhere.Append(" AND ec.DistributionCode = @DistributionCode ");
                paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = condition.DistributionCode });
            }
            //拼装必选条件
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
                SELECT (ROW_NUMBER() OVER (ORDER BY wbs.CreatTime)) AS 'ID', ec.CompanyName,
                       CHAR(9) + CONVERT(VARCHAR(20),w.WaybillNO) 'WaybillNO', w.CustomerOrder, wsi.NeedAmount, wsi.NeedBackAmount, 
                       wsi.AcceptType,(CASE WHEN wsi.FinancialStatus IS NULL THEN '未收款'
                                            WHEN wsi.FinancialStatus = 0 THEN '未收款'
                                            ELSE '已收款' END) 'FinancialStatus',
                       CASE WHEN wbs.POSCode IS NULL OR wbs.POSCode='' THEN e.POSCode 
                            ELSE wbs.POSCode END 'POSCode',s.STATUSNAME, w.CreatTime as IntoTime, wsi.CreatTime AS CreateTime,wbs.CreatTime as BackStationTime
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
                JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
                LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK) ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
                WHERE  wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                 AND   wbs.SignStatus = @SignStatus  AND w.IsDelete=0 AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
                 {0}   
            ", strWhere.ToString());

            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });

            return SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取成功的运单明细数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceDataNew(SearchCondition condition)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND fsdfd.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }

            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }
            //拼装配送站
            if (condition.DeliverStation != 0)
            {
                strWhere.Append(" AND fsdfd.StationID = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND fsdfd.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (condition.MerchantID != 0)
            {
                strWhere.Append(" AND fsdfd.MerchantID = @MerchantID ");
                paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
            }
            //add by wangyongc 2011-10-25 增加配送商条件的判断
            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                strWhere.Append(" AND ec.DistributionCode = @DistributionCode ");
                paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = condition.DistributionCode });
            }
            //拼装必选条件
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
               SELECT (ROW_NUMBER() OVER (ORDER BY fsdfd.DailyTime)) AS 'ID', ec.CompanyName,
                   CHAR(9) + CONVERT(VARCHAR(20),fsdfd.WaybillNO) 'WaybillNO', fsdfd.CustomerOrder, fsdfd.NeedPrice,fsdfd.NeedReturnPrice, 
                   fsdfd.AcceptType,'未收款' as FinancialStatus,
                   CASE WHEN fsdfd.PosNum IS NULL OR fsdfd.PosNum='' THEN e.POSCode 
                        ELSE fsdfd.PosNum END 'POSCode',s.STATUSNAME,fsdfd.EnterTime as IntoTime, fsdfd.DailyTime AS CreateTime
            FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
            JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID
            JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = fsdfd.DeliverManID
            LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK) ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
            WHERE  fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
             AND   fsdfd.[Status] = @SignStatus 
                 {0}   
            ", strWhere.ToString());

            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });

            return SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取成功的运单明细数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceDataNewV2(SearchCondition condition)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND fsdfd.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }

            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }
            //拼装配送站
            if (condition.DeliverStation != 0)
            {
                strWhere.Append(" AND fsdfd.StationID = @DeliverStation ");
                paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND fsdfd.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (condition.MerchantID != 0)
            {
                strWhere.Append(" AND fsdfd.MerchantID = @MerchantID ");
                paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
            }
            //add by wangyongc 2011-10-25 增加配送商条件的判断
            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                strWhere.Append(" AND ec.DistributionCode = @DistributionCode ");
                paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = condition.DistributionCode });
            }
            //拼装必选条件
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
               SELECT (ROW_NUMBER() OVER (ORDER BY fsdfd.DailyTime)) AS 'ID', ec.CompanyName,
                   CHAR(9) + CONVERT(VARCHAR(20),fsdfd.WaybillNO) 'WaybillNO', fsdfd.CustomerOrder, fsdfd.NeedPrice,fsdfd.NeedReturnPrice, 
                   fsdfd.AcceptType,'未收款' as FinancialStatus,
                   CASE WHEN fsdfd.PosNum IS NULL OR fsdfd.PosNum='' THEN e.POSCode 
                        ELSE fsdfd.PosNum END 'POSCode',s.STATUSNAME,fsdfd.EnterTime as IntoTime, fsdfd.DailyTime AS CreateTime
            FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
            JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON fsdfd.StationID = ec.ExpressCompanyID
            JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = fsdfd.DeliverManID
            LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK) ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
            WHERE  fsdfd.DailyTime >= @BeginTime AND  fsdfd.DailyTime < @EndTime
             AND   fsdfd.[Status] = @SignStatus 
                 {0}   
            ", strWhere.ToString());

            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });

            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 导出所有明细(根据查询条件)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetAllDetailsFinanceData(SearchCondition condition)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }
            //add by wangyongc 2011-10-25 增加配送商条件的判断
            //if (!String.IsNullOrEmpty(condition.DistributionCode))
            //{
            //    strWhere.Append(" AND ec.DistributionCode = @DistributionCode ");
            //    paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = condition.DistributionCode });
            //}
            //拼装必选条件
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
                SELECT (ROW_NUMBER() OVER (ORDER BY wbs.CreatTime)) AS 'ID', ec.CompanyName,
                       CHAR(9) + CONVERT(VARCHAR(20),w.WaybillNO) 'WaybillNO', w.CustomerOrder, wsi.NeedAmount, wsi.NeedBackAmount, 
                       wsi.AcceptType,(CASE WHEN wsi.FinancialStatus IS NULL THEN '未收款'
                                            WHEN wsi.FinancialStatus = 0 THEN '未收款'
                                            ELSE '已收款' END) 'FinancialStatus',
                       CASE WHEN wbs.POSCode IS NULL OR wbs.POSCode='' THEN e.POSCode 
                            ELSE wbs.POSCode END 'POSCode',s.STATUSNAME, wbs.CreatTime as BackStationTime,
                       mbi.MerchantName
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
                JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID = w.MerchantID
                JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK) ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
               WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                 AND   wbs.SignStatus = @SignStatus   AND w.IsDelete=0
                 {0}
                 AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
                 AND   ec.DistributionCode = @DistributionCode
                 AND   w.Sources IN(0,1)
           UNION ALL   --第三方配送商的明细数据
                SELECT (ROW_NUMBER() OVER (ORDER BY wbs.CreatTime)) AS 'ID', 
				       CASE WHEN ec.DistributionCode=@DistributionCode THEN ec.CompanyName
                       ELSE dis.DistributionName END AS 'CompanyName',
                       CHAR(9) + CONVERT(VARCHAR(20),w.WaybillNO) 'WaybillNO', w.CustomerOrder, wsi.NeedAmount, wsi.NeedBackAmount, 
                       wsi.AcceptType,(CASE WHEN wsi.FinancialStatus IS NULL THEN '未收款'
                                            WHEN wsi.FinancialStatus = 0 THEN '未收款'
                                            ELSE '已收款' END) 'FinancialStatus',
                       CASE WHEN wbs.POSCode IS NULL OR wbs.POSCode='' THEN e.POSCode 
                            ELSE wbs.POSCode END 'POSCode',s.STATUSNAME,wbs.CreatTime as BackStationTime,
                       mbi.MerchantName
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON dis.DistributionCode = ec.DistributionCode
                JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID = w.MerchantID
                JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK) ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
               WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                 AND   wbs.SignStatus = @SignStatus   AND w.IsDelete=0
                 {0}
                 AND ISNULL(wsi.TransferPayType,0)<>2 AND ISNULL(wsi.accountType,0)<>2
                 AND   w.Sources=2 AND (w.DistributionCode=@DistributionCode OR ec.DistributionCode= @DistributionCode)
            ", strWhere.ToString());

            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            return SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 导出所有明细(根据查询条件)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetAllDetailsFinanceDataNew(SearchCondition condition)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();


            var strVanclAndvjiaWhere = new StringBuilder();
            //支付方式
            if (condition.PayType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.AcceptType =@AcceptType ");
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.Sources = @Sources ");
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strVanclAndvjiaWhere.Append(string.Format(" AND fsdfd.MerchantID = IN ({0})", condition.MerchantIDs.Trim()));
                strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }

            //拼装来源
            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.DistributionCode = @DistributionCode ");
                strWhere.Append(" AND w.DistributionCode = @DistributionCode ");
                paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            }
            //add by wangyongc 2011-10-25 增加配送商条件的判断
            //if (!String.IsNullOrEmpty(condition.DistributionCode))
            //{
            //    strWhere.Append(" AND ec.DistributionCode = @DistributionCode ");
            //    paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = condition.DistributionCode });
            //}
            //拼装必选条件
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
                SELECT (ROW_NUMBER() OVER(ORDER BY fsdfd.DailyTime)) AS 'ID',
                       ec.CompanyName,
                       CHAR(9) + CONVERT(VARCHAR(20), fsdfd.WaybillNO) 'WaybillNO',
                       fsdfd.CustomerOrder,
                       fsdfd.NeedPrice,
                       fsdfd.NeedReturnPrice,
                       fsdfd.AcceptType,
                       '未收款' as  'FinancialStatus',
                       CASE 
                            WHEN fsdfd.PosNum IS NULL OR fsdfd.PosNum = '' THEN e.POSCode
                            ELSE fsdfd.PosNum
                       END 'POSCode',
                       s.STATUSNAME,
                       fsdfd.EnterTime AS IntoTime,
                       fsdfd.PostTime AS CreateTime
                FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
                       JOIN RFD_PMS.dbo.Employee e(NOLOCK)
                            ON  fsdfd.DeliverManID = e.EmployeeID
                       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
                            ON  fsdfd.StationID = ec.ExpressCompanyID
                       LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK)
                            ON  e.POSBANKID = s.STATUSNO
                            AND s.STATUSTYPENO = 401
                WHERE  fsdfd.DailyTime >= @BeginTime
                       AND fsdfd.DailyTime < @EndTime
                       AND fsdfd.[Status] = @SignStatus
                       AND e.DistributionCode=@DistributionCode
                        {1}
                       AND fsdfd.Sources IN (0, 1)
           UNION ALL   --第三方配送商的明细数据
                SELECT (ROW_NUMBER() OVER (ORDER BY wbs.CreatTime)) AS 'ID', 
				       CASE WHEN ec.DistributionCode=@DistributionCode THEN ec.CompanyName
                       ELSE dis.DistributionName END AS 'CompanyName',
                       CHAR(9) + CONVERT(VARCHAR(20),w.WaybillNO) 'WaybillNO', w.CustomerOrder, wsi.NeedAmount, wsi.NeedBackAmount, 
                       wsi.AcceptType,(CASE WHEN wsi.FinancialStatus IS NULL THEN '未收款'
                                            WHEN wsi.FinancialStatus = 0 THEN '未收款'
                                            ELSE '已收款' END) 'FinancialStatus',
                       CASE WHEN wbs.POSCode IS NULL OR wbs.POSCode='' THEN e.POSCode 
                            ELSE wbs.POSCode END 'POSCode',s.STATUSNAME, w.CreatTime as IntoTime, wsi.CreatTime AS CreateTime
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON dis.DistributionCode = ec.DistributionCode
                JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK) ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
               WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                 AND   wbs.SignStatus = @SignStatus   AND w.IsDelete=0
                 {0}
                 AND   w.Sources =2 and w.DistributionCode=@DistributionCode
            ", strWhere.ToString(), strVanclAndvjiaWhere);

            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });

            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 导出所有明细(根据查询条件)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetAllDetailsFinanceDataNewV2(SearchCondition condition)
        {
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();

            var strVanclAndvjiaWhere = new StringBuilder();

            //支付方式
            if (condition.PayType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.AcceptType =@AcceptType ");
                strWhere.Append(" AND wbs.AcceptType = @AcceptType ");
                paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strVanclAndvjiaWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                strWhere.Append(" AND e.POSBANKID = @POSBANKID ");
                paramList.Add(new SqlParameter("@POSBANKID", SqlDbType.NVarChar, 20) { Value = condition.POSType });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.Sources = @Sources ");
                strWhere.Append(" AND w.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strVanclAndvjiaWhere.Append(string.Format(" AND fsdfd.MerchantID = IN ({0})", condition.MerchantIDs.Trim()));
                strWhere.Append(string.Format(" AND w.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }

            //拼装来源
            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                strVanclAndvjiaWhere.Append(" AND fsdfd.DistributionCode = @DistributionCode ");
                strWhere.Append(" AND w.DistributionCode = @DistributionCode ");
                paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });
            }
            //add by wangyongc 2011-10-25 增加配送商条件的判断
            //if (!String.IsNullOrEmpty(condition.DistributionCode))
            //{
            //    strWhere.Append(" AND ec.DistributionCode = @DistributionCode ");
            //    paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = condition.DistributionCode });
            //}
            //拼装必选条件
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
                SELECT (ROW_NUMBER() OVER(ORDER BY fsdfd.DailyTime)) AS 'ID',
                       ec.CompanyName,
                       CHAR(9) + CONVERT(VARCHAR(20), fsdfd.WaybillNO) 'WaybillNO',
                       fsdfd.CustomerOrder,
                       fsdfd.NeedPrice,
                       fsdfd.NeedReturnPrice,
                       fsdfd.AcceptType,
                       '未收款' as  'FinancialStatus',
                       CASE 
                            WHEN fsdfd.PosNum IS NULL OR fsdfd.PosNum = '' THEN e.POSCode
                            ELSE fsdfd.PosNum
                       END 'POSCode',
                       s.STATUSNAME,
                       fsdfd.EnterTime AS IntoTime,
                       fsdfd.PostTime AS CreateTime
                FROM   FMS_StationDailyFinanceDetails fsdfd(NOLOCK)
                       JOIN RFD_PMS.dbo.Employee e(NOLOCK)
                            ON  fsdfd.DeliverManID = e.EmployeeID
                       JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK)
                            ON  fsdfd.StationID = ec.ExpressCompanyID
                       LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK)
                            ON  e.POSBANKID = s.STATUSNO
                            AND s.STATUSTYPENO = 401
                WHERE  fsdfd.DailyTime >= @BeginTime
                       AND fsdfd.DailyTime < @EndTime
                       AND fsdfd.[Status] = @SignStatus
                       AND e.DistributionCode=@DistributionCode
                        {1}
                       AND fsdfd.Sources IN (0, 1)
           UNION ALL   --第三方配送商的明细数据
                SELECT (ROW_NUMBER() OVER (ORDER BY wbs.CreatTime)) AS 'ID', 
				       CASE WHEN ec.DistributionCode=@DistributionCode THEN ec.CompanyName
                       ELSE dis.DistributionName END AS 'CompanyName',
                       CHAR(9) + CONVERT(VARCHAR(20),w.WaybillNO) 'WaybillNO', w.CustomerOrder, wsi.NeedAmount, wsi.NeedBackAmount, 
                       wsi.AcceptType,(CASE WHEN wsi.FinancialStatus IS NULL THEN '未收款'
                                            WHEN wsi.FinancialStatus = 0 THEN '未收款'
                                            ELSE '已收款' END) 'FinancialStatus',
                       CASE WHEN wbs.POSCode IS NULL OR wbs.POSCode='' THEN e.POSCode 
                            ELSE wbs.POSCode END 'POSCode',s.STATUSNAME, w.CreatTime as IntoTime, wsi.CreatTime AS CreateTime
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON dis.DistributionCode = ec.DistributionCode
                JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
LEFT JOIN RFD_PMS.dbo.STATUSINFO s(NOLOCK) ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
               WHERE   wbs.CreatTime >= @BeginTime AND  wbs.CreatTime < @EndTime
                 AND   wbs.SignStatus = @SignStatus   AND w.IsDelete=0
                 {0}
                 AND   w.Sources =2 and w.DistributionCode=@DistributionCode
            ", strWhere.ToString(), strVanclAndvjiaWhere);

            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 20) { Value = condition.DistributionCode });

            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取每天的统计数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceDailyData(SearchCondition condition)
        {
            var strSql = new StringBuilder();
            //拼装必选条件
            strSql.AppendFormat(@"
                SELECT {0}
                FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails sdf(NOLOCK)   
                JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON sdf.StationID = ec.ExpressCompanyID
           LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON sdf.MerchantID = mbi.ID 
		   LEFT JOIN   RFD_PMS.dbo.StatusInfo sif(NOLOCK) ON sdf.Sources = sif.StatusNO AND sif.StatusTypeNO = 3 --订单来源  
           LEFT JOIN   RFD_PMS.dbo.StatusInfo sis(NOLOCK) ON sdf.WaybillType = sis.StatusNO AND sis.StatusTypeNO = 2 --订单类型  
           LEFT JOIN   RFD_PMS.dbo.StatusInfo sir(NOLOCK) ON sdf.[Status] = sir.StatusNO AND sir.StatusTypeNO = 1 --配送结果  
               WHERE   sdf.DailyTime >= @BeginTime AND sdf.DailyTime < @EndTime  AND sdf.[Status]='3'  
            ", condition.IsRawData ? @"(ROW_NUMBER() OVER (ORDER BY sdf.CreateTime DESC)) AS 'ID', ec.CompanyName, ISNULL(mbi.MerchantName, sif.StatusName) 'SourceName', 
					   CHAR(9) + CONVERT(VARCHAR(20),WaybillNO) 'WaybillNO', EnterTime, sis.StatusName 'WaybillType', NeedPrice, NeedReturnPrice, PriceDiff, PriceReturnCash,
					   sir.StatusName, AcceptType, PosNum, PostTime, DeliverManName, DeductMoney"
                   : @"
                       (ROW_NUMBER() OVER (ORDER BY sdf.CreateTime DESC)) AS 'ID', ec.CompanyName, ISNULL(mbi.MerchantName, sif.StatusName) 'SourceName', 
					   CHAR(9) + CONVERT(VARCHAR(20),WaybillNO) 'WaybillNO', NeedPrice, NeedReturnPrice, AcceptType, PosNum, EnterTime, PostTime
			");

            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            //拼装可选条件
            if (condition.DeliverStation != 0)
            {
                strSql.Append(" AND sdf.StationID = @StationID ");
                paramList.Add(new SqlParameter("@StationID", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strSql.Append(" AND sdf.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (condition.MerchantID != 0)
            {
                strSql.Append(" AND sdf.MerchantID = @MerchantID ");
                paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
            }
            ////支付方式
            //if (condition.PayType != -1)
            //{
            //    strSql.Append(" AND sdf.AcceptType = @AcceptType ");
            //    paramList.Add(new SqlParameter("@AcceptType", SqlDbType.VarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            //}
            //else
            //{
            //    strSql.Append(" AND sdf.AcceptType IN (@Cash, @POS) ");
            //    paramList.Add(new SqlParameter("@Cash", SqlDbType.VarChar, 20) { Value = CashForPayment });
            //    paramList.Add(new SqlParameter("@POS", SqlDbType.VarChar, 20) { Value = POSForPayment });
            //}
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 根据查询条件获取每天的统计数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceDailyDataV2(SearchCondition condition)
        {
            var strSql = new StringBuilder();
            //拼装必选条件
            strSql.AppendFormat(@"
                SELECT {0}
                FROM   FMS_StationDailyFinanceDetails sdf(NOLOCK)   
                JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON sdf.StationID = ec.ExpressCompanyID
           LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON sdf.MerchantID = mbi.ID 
		   LEFT JOIN   RFD_PMS.dbo.StatusInfo sif(NOLOCK) ON sdf.Sources = sif.StatusNO AND sif.StatusTypeNO = 3 --订单来源  
           LEFT JOIN   RFD_PMS.dbo.StatusInfo sis(NOLOCK) ON sdf.WaybillType = sis.StatusNO AND sis.StatusTypeNO = 2 --订单类型  
           LEFT JOIN   RFD_PMS.dbo.StatusInfo sir(NOLOCK) ON sdf.[Status] = sir.StatusNO AND sir.StatusTypeNO = 1 --配送结果  
               WHERE   sdf.DailyTime >= @BeginTime AND sdf.DailyTime < @EndTime  AND sdf.[Status]='3'  
            ", condition.IsRawData ? @"(ROW_NUMBER() OVER (ORDER BY sdf.CreateTime DESC)) AS 'ID', ec.CompanyName, ISNULL(mbi.MerchantName, sif.StatusName) 'SourceName', 
					   CHAR(9) + CONVERT(VARCHAR(20),WaybillNO) 'WaybillNO', EnterTime, sis.StatusName 'WaybillType', NeedPrice, NeedReturnPrice, PriceDiff, PriceReturnCash,
					   sir.StatusName, AcceptType, PosNum, PostTime, DeliverManName, DeductMoney"
                   : @"
                       (ROW_NUMBER() OVER (ORDER BY sdf.CreateTime DESC)) AS 'ID', ec.CompanyName, ISNULL(mbi.MerchantName, sif.StatusName) 'SourceName', 
					   CHAR(9) + CONVERT(VARCHAR(20),WaybillNO) 'WaybillNO', NeedPrice, NeedReturnPrice, AcceptType, PosNum, EnterTime, PostTime
			");

            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            //添加参数
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            //拼装可选条件
            if (condition.DeliverStation != 0)
            {
                strSql.Append(" AND sdf.StationID = @StationID ");
                paramList.Add(new SqlParameter("@StationID", SqlDbType.Int) { Value = condition.DeliverStation });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strSql.Append(" AND sdf.Sources = @Source ");
                paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
            }
            //商家条件
            if (condition.MerchantID != 0)
            {
                strSql.Append(" AND sdf.MerchantID = @MerchantID ");
                paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
            }
            ////支付方式
            //if (condition.PayType != -1)
            //{
            //    strSql.Append(" AND sdf.AcceptType = @AcceptType ");
            //    paramList.Add(new SqlParameter("@AcceptType", SqlDbType.VarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            //}
            //else
            //{
            //    strSql.Append(" AND sdf.AcceptType IN (@Cash, @POS) ");
            //    paramList.Add(new SqlParameter("@Cash", SqlDbType.VarChar, 20) { Value = CashForPayment });
            //    paramList.Add(new SqlParameter("@POS", SqlDbType.VarChar, 20) { Value = POSForPayment });
            //}
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        ///  根据查询条件获取系统财务收款报表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetSystemWaybillInfo(SearchCondition condition)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
                SELECT (ROW_NUMBER() OVER (ORDER BY wbs.CreatTime)) AS '序号', w.WaybillNo AS '运单号',
                       w.CreatTime '发货时间',wis.IntoTime '入站时间', wbs.CreatTime '归班时间',w.CustomerOrder '客户订单号',
                       w.WaybillType '订单类型', wbs.SignStatus '订单状态',wbs.NeedAmount '应收金额',wbs.FactBackAmount '应退金额',
                       wgd.Weight '订单重量',wbs.AcceptType '付款方式',e.EmployeeName '配送员',e.POSCode 'POS机终端号' 
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillIntoStation wis(NOLOCK) ON w.WaybillNO = wis.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO=wsi.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillGoodsDetails wgd(NOLOCK) ON w.WaybillNO = wgd.WayBillNO
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON w.WaybillNO = wbs.WayBillNO AND wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
                WHERE  wbs.DeliverStation = @DeliverStation   AND w.IsDelete=0
                AND    wbs.CreatTime BETWEEN @BeginTime AND @EndTime
            ");

            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.SearchDate });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.SearchDate.AddDays(1) });

            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<SqlParameter>()).Tables[0];
        }
        /// <summary>
        /// 按指定的订单号获取所有的订单列表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetWaybillListByOrderNo(SearchCondition condition)
        {
            var result = new DataTable();
            var orders = condition.OrderNoList;
            if (orders.IsEmpty()) return null;
            var count = orders.Rows.Count;
            var list = new List<string>();
            var times = (count - 1) / BatchCount;
            for (var i = 0; i <= times; i++)
            {
                var temp = new StringBuilder();
                for (var j = i * BatchCount; j < (i + 1) * BatchCount; j++)
                {
                    if (count <= j) break;
                    var orderNo = orders.Rows[j]["订单号"].ToString().Trim();
                    if (String.IsNullOrEmpty(orderNo) ||
                        !StringUtil.IsValidFormCode(orderNo) ||
                        list.Contains(orderNo))
                    {
                        continue;
                    }
                    list.Add(orderNo);
                    temp.AppendFormat(" SELECT {0} UNION ALL ", orderNo);
                }
                var strSql = new StringBuilder();
                strSql.AppendFormat(@"
                    --定义表变量
					IF OBJECT_ID('tempdb.dbo.#t_order') IS NOT NULL
					   DROP TABLE #t_order
					CREATE TABLE #t_order(WaybillNo BIGINT)
					INSERT INTO #t_order(WaybillNo) {0}
					SELECT (ROW_NUMBER() OVER (ORDER BY temp.CreatTime DESC)) AS 'ID', 
						   CompanyName '配送站',WaybillNO '订单号',Amount '订单总价',
                           CASE WHEN  FactAmount=0 THEN PaidAmount ELSE FactAmount END AS '已收款',
                           CASE WHEN (NeedAmount-FactAmount)<0 THEN 0 ELSE (NeedAmount-FactAmount) END '应收款',
						   WayBillInfoWeight '订单重量',WayBillBoxNo '包装箱型号',
                           WarehouseName '出库仓',ReceiveAddress '收货人地址',
                           CreatTime '发货日期', WaybillStatus '订单状态',MerchantName '商家',
						   BackStatus, [Status] ,IsDelay 
					FROM(
							--将临时表与数据库相关联
							SELECT ec.[CompanyName],
                                   w.WaybillType,ISNULL(wbi.[NeedAmount],0) 'NeedAmount',
                                   w.[WaybillNO] 'WaybillNO', 
								   ISNULL(wbi.[Amount],0) 'Amount',
                                   isnull(wbi.PaidAmount,0) PaidAmount,
								   CASE WHEN wbi.SignStatus IN(@Resort,@Reject) THEN 0 --仅为修复错误数据
							       WHEN w.WaybillType = @Common THEN( 
                                        CASE WHEN ISNULL(wbi.[FactAmount],0) > 0 THEN wbi.[FactAmount] ELSE 0 END
                                   )
								   WHEN w.WaybillType = @Exchange THEN (
										CASE WHEN ISNULL(wbi.[FactAmount],0) > 0 THEN wbi.[FactAmount]
											 WHEN ISNULL(wbi.[FactBackAmount],0) > 0 THEN wbi.[FactBackAmount]
										     ELSE 0 END
								   )
								   ELSE ISNULL(wbi.[FactBackAmount],0) END 'FactAmount',
								   wi.[WayBillInfoWeight],wi.[WayBillBoxNo],wh.[WarehouseName],wsi.[ReceiveAddress],
                                   w.[CreatTime], w.[Status],w.[BackStatus],ISNULL(sib.[StatusName],sis.[StatusName]) [WaybillStatus],
								   (CASE WHEN (w.[Status] = {1} AND w.[BackStatus] IS NULL) THEN 1  
										 WHEN w.[BackStatus] IN({2},{3}) THEN 1 
										 ELSE 0 END) AS [IsDelay] ,mbi.MerchantName
							FROM LMS_RFD.dbo.Waybill w(NOLOCK)
							JOIN #t_order t ON w.WaybillNo = t.WaybillNo  
							JOIN LMS_RFD.dbo.WaybillSignInfo wbi(NOLOCK) ON  w.WaybillNO = wbi.WaybillNO
							LEFT JOIN LMS_RFD.dbo.WaybillTakeSendInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
							LEFT JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wbi.BackStationInofID = wbs.WaybillBackStationID
							LEFT JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON  w.WaybillNO = wi.WaybillNO
							LEFT JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON w.DeliverStationID = ec.ExpressCompanyID
							LEFT JOIN RFD_PMS.dbo.Warehouse wh(NOLOCK) ON w.WarehouseId = wh.WarehouseId 
							LEFT JOIN RFD_PMS.dbo.StatusInfo sis(NOLOCK) ON sis.StatusTypeNo = 1 AND sis.StatusNO = w.[Status]
							LEFT JOIN RFD_PMS.dbo.StatusInfo sib(NOLOCK) ON sib.StatusTypeNo = 5 AND sib.StatusNO = w.[BackStatus]
                            JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID=w.MerchantID
                           WHERE w.[CreatTime] >= '2011-11-03'  AND w.IsDelete=0 --单独处理2011-11-03之后的数据
                       
					) temp", temp.ToString().TrimEnd("UNION ALL".ToCharArray()),
                            (int)WayBillStatus.Success,
                            (int)BackStatus.RefusedBounded,
                            (int)BackStatus.ReturnBounded);
                IList<SqlParameter> paramList = new List<SqlParameter>();
                paramList.Add(new SqlParameter("@Common", SqlDbType.Int) { Value = (int)WayBillType.Common });
                paramList.Add(new SqlParameter("@Exchange", SqlDbType.Int) { Value = (int)WayBillType.Exchange });
                paramList.Add(new SqlParameter("@Resort", SqlDbType.Int) { Value = (int)WayBillStatus.Resort });
                paramList.Add(new SqlParameter("@Reject", SqlDbType.Int) { Value = (int)WayBillStatus.Reject });
                var ds = SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>());
                if (ds != null)
                {
                    if (result.Columns.Count == 0)
                    {
                        result = ds.Tables[0].Clone();
                    }
                    //data.Merge(ds.Tables[0]);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.ImportRow(dr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 更新当前站点的汇总数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateDailyTotalAmount(FMS_StationDailyFinanceSum model)
        {
            var strSql = new StringBuilder();

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                strSql.Append(@"
					UPDATE  LMS_RFD.dbo.FMS_StationDailyFinanceSum s
					   SET  s.RealInCome = @RealInCome,
							s.FinanceStatus = @FinanceStatus,
                            s.IsChange=1
					 WHERE  s.ID = @ID
					--更新明细表的财务收款状态(来源为vancl,vjia)
					UPDATE  d
					   SET  d.FinanceStatus = @FinanceStatus,
                            d.FinanceConfirmTime = GETDATE(),
                            d.IsChange=1
					  FROM
						 (
							SELECT StationID,Sources,DailyTime
							FROM LMS_RFD.dbo.FMS_StationDailyFinanceSum s WITH(NOLOCK)
							WHERE ID = @ID 
						 )  t 
					 JOIN   LMS_RFD.dbo.FMS_StationDailyFinanceDetails d WITH(NOLOCK)
					   ON   t.StationID = d.StationID 
					  AND   t.Sources = d.Sources 
					  AND   t.DailyTime = d.DailyTime 
                      AND   d.[Status] = @Status
				      AND   d.Sources IN (0,1)
                    --更新明细表的财务收款状态(来源为其他)
					UPDATE  d
					   SET  d.FinanceStatus = @FinanceStatus,
                            d.FinanceConfirmTime = GETDATE(),
                            d.IsChange=1
					  FROM
						 (
							SELECT StationID,Sources,DailyTime,MerchantID
							FROM LMS_RFD.dbo.FMS_StationDailyFinanceSum s WITH(NOLOCK)
							WHERE ID = @ID 
						 ) t 
					 JOIN   LMS_RFD.dbo.FMS_StationDailyFinanceDetails d WITH(NOLOCK)
					   ON   t.StationID = d.StationID 
					  AND   t.Sources = d.Sources 
					  AND   t.DailyTime = d.DailyTime 
                      AND   t.MerchantID = d.MerchantID
                      AND   d.[Status] = @Status
                      AND   d.Sources > 1
				");
                IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
                paramList.Add(new SqlParameter("@RealInCome", SqlDbType.Int) { Value = model.RealInCome });
                paramList.Add(new SqlParameter("@FinanceStatus", SqlDbType.Int) { Value = model.FinanceStatus });
                paramList.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID });
                paramList.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
                var rows = SqlHelper.ExecuteNonQuery(LMSConnection, CommandType.Text, strSql.ToString(), paramList.ToArray());
                work.Complete();
                return rows > 0;
            }
        }

        /// <summary>
        /// 更新当前站点的汇总数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateDailyTotalAmountV2(FMS_StationDailyFinanceSum model)
        {
            var strSql = new StringBuilder();

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                strSql.Append(@"
					UPDATE  FMS_StationDailyFinanceSum s
					   SET  s.RealInCome = @RealInCome,
							s.FinanceStatus = @FinanceStatus,
                            s.IsChange=1
					 WHERE  s.ID = @ID
					--更新明细表的财务收款状态(来源为vancl,vjia)
					UPDATE  d
					   SET  d.FinanceStatus = @FinanceStatus,
                            d.FinanceConfirmTime = GETDATE(),
                            d.IsChange=1
					  FROM
						 (
							SELECT StationID,Sources,DailyTime
							FROM FMS_StationDailyFinanceSum s WITH(NOLOCK)
							WHERE ID = @ID 
						 )  t 
					 JOIN   FMS_StationDailyFinanceDetails d WITH(NOLOCK)
					   ON   t.StationID = d.StationID 
					  AND   t.Sources = d.Sources 
					  AND   t.DailyTime = d.DailyTime 
                      AND   d.[Status] = @Status
				      AND   d.Sources IN (0,1)
                    --更新明细表的财务收款状态(来源为其他)
					UPDATE  d
					   SET  d.FinanceStatus = @FinanceStatus,
                            d.FinanceConfirmTime = GETDATE(),
                            d.IsChange=1
					  FROM
						 (
							SELECT StationID,Sources,DailyTime,MerchantID
							FROM FMS_StationDailyFinanceSum s WITH(NOLOCK)
							WHERE ID = @ID 
						 ) t 
					 JOIN   FMS_StationDailyFinanceDetails d WITH(NOLOCK)
					   ON   t.StationID = d.StationID 
					  AND   t.Sources = d.Sources 
					  AND   t.DailyTime = d.DailyTime 
                      AND   t.MerchantID = d.MerchantID
                      AND   d.[Status] = @Status
                      AND   d.Sources > 1
				");
                IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
                paramList.Add(new SqlParameter("@RealInCome", SqlDbType.Int) { Value = model.RealInCome });
                paramList.Add(new SqlParameter("@FinanceStatus", SqlDbType.Int) { Value = model.FinanceStatus });
                paramList.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID });
                paramList.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
                var rows = SqlHelper.ExecuteNonQuery(LMSConnection, CommandType.Text, strSql.ToString(), paramList.ToArray());
                work.Complete();
                return rows > 0;
            }
        }

        public bool UpdateWaybillsigninfoMount(string WaybillNO, decimal needamount, decimal factamount)
        {
            string strSql = @"update LMS_RFD.dbo.waybillsigninfo set needamount=@needamount,factamount=@factamount,IsChange=2 where WaybillNO=@WaybillNO";
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            paramList.Add(new SqlParameter("@needamount", needamount));
            paramList.Add(new SqlParameter("@factamount", factamount));
            paramList.Add(new SqlParameter("@WaybillNO", WaybillNO));
            return SqlHelper.ExecuteNonQuery(LMSConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
        }

        public bool UpdateWaybillbackstationMount(string WaybillNO, decimal needamount, decimal factamount)
        {
            string strSql = @"update LMS_RFD.dbo.waybillbackstation set needamount=@needamount,factamount=@factamount,IsChange=2 where WaybillNO=@WaybillNO";
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            paramList.Add(new SqlParameter("@needamount", needamount));
            paramList.Add(new SqlParameter("@factamount", factamount));
            paramList.Add(new SqlParameter("@WaybillNO", WaybillNO));
            return SqlHelper.ExecuteNonQuery(LMSConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
        }

        public bool UpdateWaybillsigninfoBackMount(string WaybillNO, decimal NeedBackAmount, decimal FactBackAmount, decimal ReturnCash)
        {
            string strSql = @"update LMS_RFD.dbo.waybillsigninfo set NeedBackAmount=@NeedBackAmount,FactBackAmount=@FactBackAmount,ReturnCash=@ReturnCash,IsChange=2 where WaybillNO=@WaybillNO";
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            paramList.Add(new SqlParameter("@FactBackAmount", FactBackAmount));
            paramList.Add(new SqlParameter("@NeedBackAmount", NeedBackAmount));
            paramList.Add(new SqlParameter("@ReturnCash", ReturnCash));
            paramList.Add(new SqlParameter("@WaybillNO", WaybillNO));
            return SqlHelper.ExecuteNonQuery(LMSConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
        }

        public bool UpdateWaybillbackstationBackMount(string WaybillNO, decimal NeedBackAmount, decimal FactBackAmount)
        {
            string strSql = @"update LMS_RFD.dbo.waybillbackstation set NeedBackAmount=@NeedBackAmount,FactBackAmount=@FactBackAmount,IsChange=2 where WaybillNO=@WaybillNO";
            IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
            paramList.Add(new SqlParameter("@NeedBackAmount", NeedBackAmount));
            paramList.Add(new SqlParameter("@FactBackAmount", FactBackAmount));
            paramList.Add(new SqlParameter("@WaybillNO", WaybillNO));
            return SqlHelper.ExecuteNonQuery(LMSConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
        }

        /// <summary>
        /// 通过查询条件得到快递模式配送费汇总信息 add by wangyongc 2012-03-27 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable GetTransferFinanceSumData(SearchCondition condition)
        {
            string strSql1 = @"SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                                           MAX(DistributionName) 'DistributionName',
                                           ec.ExpressCompanyID,
							               MAX(ec.CompanyName) AS 'CompanyName', 
							               w.Sources,
							               MAX(si.StatusName) AS 'SourceName',
							               MAX(w.MerchantID) AS 'MerchantID',
							               MAX(mbi.MerchantCode) AS 'MerchantCode',
							               MAX(mbi.MerchantName) AS 'MerchantName',
							               SUM(CASE WHEN isnull(wbs.AcceptType,'') <> 'POS机' THEN 1 ELSE 0 END) AS 'CashWaybillCount', 
                                           SUM(CASE WHEN wbs.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							               SUM(isnull(wsi.TransferFee,0)) AS 'TransferFeeSum',   
							               SUM(isnull(wsi.DinsureFee,0)) AS 'ProtectedPriceSum',
                                           (SUM(isnull(wsi.TransferFee,0))+ SUM(isnull(wsi.DinsureFee,0))) as SaveAmount,  
							               CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
						       FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						       JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
						       JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
						       JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				               LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID
                               LEFT JOIN   RFD_PMS.dbo.StatusInfo si(NOLOCK) ON w.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                               LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                               LEFT  JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                                   WHERE w.DistributionCode=@DistributionCode  
                                    AND w.[Status]='3'					
						           -- AND w.MerchantID=30
						            AND w.IsDelete=0
                                    AND wsi.TransferPayType=2";

            string strSql2 = @"SELECT  MAX(ec.DistributionCode) as 'DistributionCode', 
                               MAX(DistributionName) 'DistributionName',
                               ec.ExpressCompanyID,
							   MAX(ec.CompanyName) AS 'CompanyName', 
							   w.Sources,
							   MAX(si.StatusName) AS 'SourceName',
							   MAX(w.MerchantID) AS 'MerchantID',
							   MAX(mbi.MerchantCode) AS 'MerchantCode',
							   MAX(mbi.MerchantName) AS 'MerchantName',
							   SUM(CASE WHEN isnull(wsi.AcceptType,'') <> 'POS机' THEN 1 ELSE 0 END) AS 'CashWaybillCount', 
                               SUM(CASE WHEN wsi.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
							   SUM(isnull(wsi.TransferFee,0)) AS 'TransferFeeSum',  
							   SUM(isnull(wsi.DinsureFee,0)) AS 'ProtectedPriceSum',
                               (SUM(isnull(wsi.TransferFee,0))+ SUM(isnull(wsi.DinsureFee,0))) as SaveAmount, 
							   CONVERT(CHAR(10),w.CreatTime,120) as 'CreateTime'
						FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
						JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO               
					    JOIN   RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON w.ReceiveStationID = ec.ExpressCompanyID
				        LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID
                        LEFT JOIN   RFD_PMS.dbo.StatusInfo si(NOLOCK) ON w.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                        LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                        LEFT  JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                           WHERE w.DistributionCode=@DistributionCode  
                           -- AND w.MerchantID=30
						    AND w.IsDelete=0
                            AND w.[Status]<>'-5' 
                            AND w.DeliverStationID<>'-1'
						    AND (wsi.TransferPayType=1) ";

            List<SqlParameter> parameters = new List<SqlParameter>();

            string where = "";
            string where1 = "";

            if (condition.DeliverStation != 0)
            {
                where += " and w.DeliverStationID=@DeliverStation";
                where1 += " and w.ReceiveStationID=@DeliverStation ";
                parameters.Add(new SqlParameter("@DeliverStation", condition.DeliverStation));
            }

            if (condition.BeginTime != null && condition.EndTime != null)
            {
                where += " and wbs.CreatTime BETWEEN @CreatTime AND @EndTime";
                where1 += " and w.CreatTime BETWEEN @CreatTime AND @EndTime";
                parameters.Add(new SqlParameter("@CreatTime", condition.BeginTime));
                parameters.Add(new SqlParameter("@EndTime", condition.EndTime));
            }

            //商家选择
            if (!string.IsNullOrEmpty(condition.MerchantIDs))
            {
                where += string.Format(" and w.MerchantID in ({0})", condition.MerchantIDs);
                where1 += string.Format(" and w.MerchantID in ({0})", condition.MerchantIDs);
            }

            //支付方式
            if (condition.PayType != -1)
            {
                where += " AND wbs.AcceptType = @AcceptType ";
                where1 += " AND wsi.AcceptType = @AcceptType ";

                parameters.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }

            strSql1 += where;
            strSql2 += where1;
            strSql1 += " GROUP BY  CONVERT(CHAR(10),wbs.CreatTime,120), ec.ExpressCompanyID, w.Sources ";
            strSql2 += " GROUP BY  CONVERT(CHAR(10),w.CreatTime,120), ec.ExpressCompanyID, w.Sources  ";

            string strSql = "";

            if (condition.TransferPayType == 0)
            {
                strSql = strSql1 + " UNION ALL " + strSql2;
            }
            else if (condition.TransferPayType != 2)
            {
                strSql = strSql2;
            }
            else
            {
                strSql = strSql1;
            }

            strSql = " SELECT (ROW_NUMBER() OVER (ORDER BY CreateTime)) AS 'ID',* from (" + strSql + " ) a ";

            parameters.Add(new SqlParameter("@DistributionCode", condition.DistributionCode));

            return SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120, CommandType.Text, strSql.ToString(), parameters.ToArray<SqlParameter>()).Tables[0];
        }

        /// <summary>
        /// 通过查询条件得到快递模式配送费汇总信息 add by wangyongc 2012-03-27 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable GetTransferFinanceDetailData(SearchCondition condition)
        {
            string strSql1 =
                @"SELECT  
                        ec.CompanyName as 配送站,
                        CHAR(9) + CONVERT(VARCHAR(20),w.WaybillNO) as  运单号, 
                        w.CustomerOrder as 订单号, 
                        wsi.TransferFee as 配送费,
                        wsi.DinsureFee as 保价费, 
                        mbi.MerchantName 商家,
                        '现金' as 支付方式,
                        CASE WHEN wbs.POSCode IS NULL OR wbs.POSCode='' THEN e.POSCode 
                            ELSE wbs.POSCode END 'POS终端号',      
                        wbs.CreatTime as 统计时间,
                        w.CreatTime as 提交时间
                    FROM LMS_RFD.dbo.Waybill w(NOLOCK)
					JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
					JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID = wbs.WaybillBackStationID                
					JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
				    LEFT JOIN   RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID  
                    LEFT JOIN   RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                    LEFT JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                        WHERE w.DistributionCode=@DistributionCode  
                         AND w.[Status]='3'					
					     --AND w.MerchantID=30
					     AND w.IsDelete=0
                         AND wsi.TransferPayType=2 ";
            string strSql2 =
                @"SELECT  
                    ec.CompanyName as 配送站,
                    CHAR(9) + CONVERT(VARCHAR(20),w.WaybillNO) as  运单号, 
                    w.CustomerOrder as 订单号, 
                    wsi.TransferFee as 配送费,
                    wsi.DinsureFee as 保价费, 
                    mbi.MerchantName 商家,
                    '现金' as 支付方式,
                    e.POSCode as  'POS终端号',
                    w.CreatTime as 统计时间,
                    w.CreatTime as 提交时间
                    FROM LMS_RFD.dbo.Waybill w(NOLOCK)
						 JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO               
					     LEFT JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON w.ReceiveStationID = ec.ExpressCompanyID
				         LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID  
                         LEFT JOIN RFD_PMS.dbo.Distribution dis(NOLOCK) ON ec.DistributionCode = dis.DistributionCode
                         LEFT JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON w.DeliverManID=e.EmployeeID
                           WHERE w.DistributionCode=@DistributionCode  
						   --AND w.MerchantID=30
                            AND w.IsDelete=0
                            AND w.[Status]<>'-5' 
                            AND w.DeliverStationID<>'-1'
						    AND (wsi.TransferPayType=1 ) ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            string where = "";
            string where1 = "";
            if (condition.DeliverStation != 0)
            {
                where += " and w.DeliverStationID=@DeliverStation";
                where1 += " and w.ReceiveStationID=@DeliverStation ";
                parameters.Add(new SqlParameter("@DeliverStation", condition.DeliverStation));
            }
            if (condition.BeginTime != null && condition.EndTime != null)
            {
                where += " and wbs.CreatTime BETWEEN @CreatTime AND @EndTime";
                where1 += " and w.CreatTime BETWEEN @CreatTime AND @EndTime";
                parameters.Add(new SqlParameter("@CreatTime", condition.BeginTime));
                parameters.Add(new SqlParameter("@EndTime", condition.EndTime));
            }

            //支付方式
            if (condition.PayType != -1)
            {
                where += " AND wbs.AcceptType = @AcceptType ";
                where1 += " AND wsi.AcceptType = @AcceptType ";
                parameters.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }

            //if (!string.IsNullOrEmpty(condition.MerchantIDs) && condition.MerchantIDs.Trim().Length != 0)
            //{
            //    where += String.Format(" AND mbi.ID in ({0})", condition.MerchantIDs);
            //    where1 += String.Format(" AND mbi.ID in ({0})", condition.MerchantIDs);
            //}

            // 商家

            if (!string.IsNullOrEmpty(condition.MerchantIDs))
            {
                where += string.Format(" and w.MerchantID in ({0})", condition.MerchantIDs);
                where1 += string.Format(" and w.MerchantID in ({0})", condition.MerchantIDs);
            }

            strSql1 += where;
            strSql2 += where1;

            string strSql = "";

            if (condition.TransferPayType == 0)
            {
                strSql = strSql1 + " UNION ALL " + strSql2;
            }
            else if (condition.TransferPayType != 2)
            {
                strSql = strSql2;
            }
            else
            {
                strSql = strSql1;
            }

            strSql = " SELECT (ROW_NUMBER() OVER (ORDER BY 提交时间)) AS '序号',* from (" + strSql + " ) a ";

            parameters.Add(new SqlParameter("@DistributionCode", condition.DistributionCode));

            return SqlHelperEx.ExecuteDataset(LMSReadOnlyConnection,120, CommandType.Text, strSql.ToString(), parameters.ToArray<SqlParameter>()).Tables[0];

        }
    }
}
