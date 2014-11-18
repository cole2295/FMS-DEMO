using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using LMS.AdoNet.DbBase;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using LMS.Util;
using System.Configuration;

namespace RFD.FMS.DAL.BasicSetting
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
	public class FinanceDao : DaoBase
	{
		private static readonly string CashForPayment = EnumHelper.GetDescription(PaymentType.Cash);
		private static readonly string POSForPayment = EnumHelper.GetDescription(PaymentType.POS);
		private static readonly string UnreceivedStatus = EnumHelper.GetDescription(AcceptStatus.UnReceived);
		private static readonly string ReceivedStatus = EnumHelper.GetDescription(AcceptStatus.Received);
		private static readonly int BatchCount = ConfigurationManager.AppSettings["BatchCount"] == null ? 1000 : int.Parse(ConfigurationManager.AppSettings["BatchCount"]);

		/// <summary>
		/// 根据查询条件获取成功的运单汇总数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetTotalFinanceData(SearchCondition condition)
		{
			var strSql = new StringBuilder();
			//拼装必选条件
			strSql.Append(@"
                SELECT (ROW_NUMBER() OVER (ORDER BY CONVERT(CHAR(10),wbs.CreatTime,120))) AS 'ID',
                       ec.ExpressCompanyID,
                       ec.CompanyName, 
                       mbi.MerchantName,
                       SUM(CASE WHEN wbs.AcceptType = @Cash THEN 1 ELSE 0 END) AS 'CashWaybillCount',  
                       SUM(CASE WHEN wbs.AcceptType = @POS THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
                       SUM(wsi.FactAmount) AS 'AcceptAmount',
                       SUM(CASE WHEN w.WaybillType = @Returned THEN 1 ELSE 0 END) AS 'BackWaybillCount',
                       SUM(wsi.FactBackAmount) AS 'BackAmount',
                       (SUM(wsi.FactAmount) - SUM(wsi.FactBackAmount)) AS 'SaveAmount',
                       CONVERT(CHAR(10),wbs.CreatTime,120) as 'CreateTime'
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON w.WaybillNO = wbs.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO AND wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
           LEFT JOIN   MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID   
                WHERE  wbs.CreatTime BETWEEN @BeginTime AND @EndTime
                AND    wbs.SignStatus = @SignStatus
            ");

			IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
			//添加参数
			paramList.Add(new SqlParameter("@Cash", SqlDbType.NVarChar, 20) { Value = CashForPayment });
			paramList.Add(new SqlParameter("@POS", SqlDbType.NVarChar, 20) { Value = POSForPayment });
			paramList.Add(new SqlParameter("@Returned", SqlDbType.NVarChar, 20) { Value = ((int)WayBillType.Returned).ToString() });
			paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
			paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
			paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });

			//拼装可选条件
			//wangyongc 2011-08-19 站点条件
			if (condition.DeliverStation != 0)
			{
				strSql.Append(" AND wbs.DeliverStation = @DeliverStation ");
				paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
			}
			//hemingyu 2011-08-24 商家条件
			if (condition.MerchantID != 0)
			{
				strSql.Append(" AND mbi.ID = @MerchantID ");
				paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
			}
			//支付方式
			if (condition.PayType != -1)
			{
				strSql.Append(" AND wbs.AcceptType = @AcceptType ");
				paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
			}
			else
			{
				strSql.Append(" AND wbs.AcceptType IN(@Cash, @POS) ");
			}
			//拼装来源
			if (condition.Source != -1)
			{
				strSql.Append(" AND w.Sources = @Source ");
				paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
			}
			//拼装分组条件
			strSql.Append(@"  
                GROUP BY CONVERT(CHAR(10),wbs.CreatTime,120), ec.CompanyName, ec.ExpressCompanyID, mbi.MerchantName");
			//返回结果
			return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
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
			var strSql = new StringBuilder();
			//拼装必选条件
			strSql.Append(@"
                SELECT (ROW_NUMBER() OVER (ORDER BY wbs.CreatTime)) AS 'ID', ec.CompanyName, 
                       w.WaybillNO, wsi.NeedAmount, wsi.NeedBackAmount, wsi.AcceptType,
                       (CASE WHEN wsi.FinancialStatus = @Unreceived THEN @UnreceivedStatus 
                          WHEN wsi.FinancialStatus = @Received THEN @ReceivedStatus 
                          ELSE @UnreceivedStatus END) AS FinancialStatus, 
                       POSCode, w.CreatTime as IntoTime, wsi.CreatTime AS CreateTime
                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
                JOIN   LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON w.WaybillNO = wbs.WaybillNO AND wsi.BackStationInofID = wbs.WaybillBackStationID
                JOIN   ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
                JOIN   Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
                WHERE  wbs.CreatTime BETWEEN @BeginTime AND @EndTime
                 AND   wbs.SignStatus = @SignStatus
            ");

			IList<SqlParameter> paramList = new List<SqlParameter>();//参数列表
			//添加参数
			paramList.Add(new SqlParameter("@Unreceived", SqlDbType.Int) { Value = (int)AcceptStatus.UnReceived });
			paramList.Add(new SqlParameter("@UnreceivedStatus", SqlDbType.NVarChar, 20) { Value = UnreceivedStatus });
			paramList.Add(new SqlParameter("@Received", SqlDbType.Int) { Value = (int)AcceptStatus.Received });
			paramList.Add(new SqlParameter("@ReceivedStatus", SqlDbType.NVarChar, 20) { Value = ReceivedStatus });
			paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
			paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
			paramList.Add(new SqlParameter("@SignStatus", SqlDbType.NVarChar, 20) { Value = ((int)WayBillStatus.Success).ToString() });
			//拼装可选条件
			//支付方式
			if (condition.PayType != -1)
			{
				strSql.Append(" AND wbs.AcceptType = @AcceptType ");
				paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
			}
			else
			{
				strSql.Append(" AND wbs.AcceptType IN(@Cash, @POS) ");
				paramList.Add(new SqlParameter("@Cash", SqlDbType.NVarChar, 20) { Value = CashForPayment });
				paramList.Add(new SqlParameter("@POS", SqlDbType.NVarChar, 20) { Value = POSForPayment });
			}
			//拼装配送站
			if (condition.DeliverStation != 0)
			{
				strSql.Append(" AND wbs.DeliverStation = @DeliverStation ");
				paramList.Add(new SqlParameter("@DeliverStation", SqlDbType.Int) { Value = condition.DeliverStation });
			}
			//拼装来源
			if (condition.Source != -1)
			{
				strSql.Append(" AND w.Sources = @Source ");
				paramList.Add(new SqlParameter("@Source", SqlDbType.Int) { Value = condition.Source });
			}
			//商家条件
			if (condition.MerchantID != 0)
			{
				strSql.Append(" AND w.MerchantID = @MerchantID ");
				paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
			}
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
                JOIN   Employee e(NOLOCK) ON e.EmployeeID = wbs.DeliverMan
                WHERE  wbs.DeliverStation = @DeliverStation
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
			var data = new DataTable();
			if (condition.OrderNoList == null || condition.OrderNoList.Count == 0) 
				return null;
			var orders = condition.OrderNoList.ToArray<string>();
			var times = orders.Length / BatchCount;
			for (var i = 0; i <= times; i++)
			{
				var temp = String.Empty;
				for (var j = i * BatchCount; j < (i + 1) * BatchCount; j++)
				{
					if (orders.Length <= j) break;
					temp += String.Format(" SELECT {0} UNION ALL ", orders[j]);
				}
				temp = temp.TrimEnd("UNION ALL".ToCharArray());
				var strSql = new StringBuilder();
				strSql.AppendFormat(@"
                    --定义表变量
					IF OBJECT_ID('tempdb.dbo.#t_order') IS NOT NULL
					   DROP TABLE #t_order
					CREATE TABLE #t_order(WaybillNo BIGINT)
					INSERT INTO #t_order(WaybillNo) {0}
					--将临时表与数据库相关联
					SELECT (ROW_NUMBER() OVER (ORDER BY w.CreatTime DESC)) AS 'ID',
                           w.[WaybillNo],wbs.[FactAmount],wbs.[NeedAmount],wi.[WayBillInfoWeight],wi.[WayBillBoxNo],ec.[CompanyName],
                           wh.[WarehouseName],wsi.[ReceiveAddress],w.[CreatTime], w.[Status],w.[BackStatus], 
                           (CASE WHEN (w.[Status] = {1} AND w.[BackStatus] IS NULL) THEN 1 
                                 WHEN w.[BackStatus] IN({2},{3}) THEN 1 
                                 ELSE 0 END) AS [IsDelay]  
					FROM LMS_RFD.dbo.Waybill w(NOLOCK)
					JOIN #t_order t ON w.WaybillNo = t.WaybillNo
					   INNER JOIN LMS_RFD.dbo.WaybillSignInfo wbi(NOLOCK)
                            ON  w.WaybillNO = wbi.WaybillNO
                       LEFT JOIN LMS_RFD.dbo.WaybillTakeSendInfo wsi(NOLOCK)
                            ON  w.WaybillNO = wsi.WaybillNO
                       LEFT JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK)
                            ON  wbi.BackStationInofID = wbs.WaybillBackStationID
                       LEFT JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK)
                            ON  w.WaybillNO = wi.WaybillNO
					LEFT JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation = ec.ExpressCompanyID
					LEFT JOIN RFD_PMS.dbo.Warehouse wh(NOLOCK) ON w.WarehouseId = wh.WarehouseId 
                ", temp, (int)WayBillStatus.Success, 
				   (int)BackStatus.RefusedBounded, 
				   (int)BackStatus.ReturnBounded);
				var ds = SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql.ToString());
				if (ds != null)
				{
					if (data.Columns.Count == 0)
					{
						data = ds.Tables[0].Clone();
					}
					foreach (DataRow dr in ds.Tables[0].Rows)
					{
						data.ImportRow(dr);
					}
				}
			}
			return data;
		}

        /// <summary>
        /// 服务调用接口
        /// </summary>
        /// <param name="dt">统计日期</param>
        /// <returns></returns>
        public DataTable GetServiveSumData(DateTime dt)
        {
            string strBegTime = dt.ToString("yyyy-MM-dd") + " 00:00:00";
            string strEndTime = dt.ToString("yyyy-MM-dd") + " 23:59:59";
            string strSql = @"SELECT wbs.DeliverStation,w.Sources,
                                       mbi.ID AS MerchantID,
                                       COUNT(w.WaybillNO) AS Recivecount,
                                       SUM(CASE WHEN wbs.SignStatus='3' AND wbs.AcceptType = '现金' THEN 1 ELSE 0 END) AS 'CashWaybillCount',
                                       SUM(CASE WHEN wbs.SignStatus='3' AND wbs.AcceptType = '现金' THEN wsi.FactAmount ELSE 0 END) AS 'CashAmount',  
                                       SUM(CASE WHEN wbs.SignStatus='3' AND wbs.AcceptType = 'POS机' THEN 1 ELSE 0 END) AS 'POSWaybillCount', 
                                       SUM(CASE WHEN wbs.SignStatus='3' AND wbs.AcceptType = '现金' THEN wsi.FactAmount ELSE 0 END) AS 'CashAmount',
                                       SUM(CASE WHEN wbs.SignStatus='3' THEN wsi.FactAmount ELSE 0 END ) AS 'AcceptAmount',
                                       SUM(CASE WHEN wbs.SignStatus='3' and w.WaybillType=2 THEN 1 ELSE 0 END) AS 'BackWaybillCount',
                                       SUM(CASE WHEN wbs.SignStatus='3' THEN wsi.FactBackAmount ELSE 0 END) AS 'BackAmount',
                                       sum(CASE WHEN wbs.SignStatus='4' OR wbs.SignStatus IS NULL THEN 1 ELSE 0 END) AS 'DelayWaybikllCount',
                                       sum(CASE WHEN wbs.SignStatus='4' OR wbs.SignStatus IS NULL THEN wsi.FactAmount ELSE 0 END) AS 'DelayWaybikllAmout',
                                       SUM(CASE WHEN wbs.SignStatus='5' THEN 1 ELSE 0 END) AS 'AllRejectNeedCount',
                                       SUM(CASE WHEN wbs.SignStatus='5' THEN wsi.FactAmount ELSE 0 END) AS 'AllRejectNeedAmount',
                                       (SUM(wsi.FactAmount) - SUM(wsi.FactBackAmount)) AS 'SaveAmount',
                                       CONVERT(CHAR(10),wbs.CreatTime,120) as 'DailyTime'
                                FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                                JOIN   LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO = wsi.WaybillNO
                                INNER JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON  wsi.BackStationInofID = wbs.WaybillBackStationID               
				                LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON w.MerchantID = mbi.ID   
                                WHERE  wbs.CreatTime BETWEEN @BegTime AND @EndTime 
                                AND wbs.SignStatus IN ('3','4','5')               
                            GROUP BY CONVERT(CHAR(10),wbs.CreatTime,120),wbs.DeliverStation,w.Sources,mbi.ID";
            List<SqlParameter> sqlParaList = new List<SqlParameter>();
            sqlParaList.Add(new SqlParameter("@BegTime", SqlDbType.DateTime) { Value = strBegTime });
            sqlParaList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = strEndTime });
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql, sqlParaList.ToArray()).Tables[0]; 
        }
	}
}
