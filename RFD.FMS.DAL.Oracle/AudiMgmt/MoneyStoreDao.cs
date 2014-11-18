using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using System.Configuration;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.AudiMgmt;

namespace RFD.FMS.DAL.Oracle.AudiMgmt
{
	/*
	* (C)Copyright 2011-2012 如风达信息管理系统
	* 
	* 模块名称：订单收款和入库状态查询（数据层）
	* 说明：查询指定条件下的订单收款和入库状态信息
	* 作者：王勇
	* 创建日期：2011/07/18
	* 修改人：
	* 修改时间：
	* 修改记录：
	*/
    public class MoneyStoreDao : OracleDao, IMoneyStoreDao
	{
        private static readonly int BatchCount = ConfigurationManager.AppSettings["BatchCount"] == null ? 5000 : int.Parse(ConfigurationManager.AppSettings["BatchCount"]);

        //hemingyu 2011-08-31 加分页
        /// <summary>
        /// 订单收款和入库状态查询SQL
        /// </summary>
        private const string strOrderMoneyStoreInfoSQL = @"
           WITH r AS(           
             SELECT ROW_NUMBER() OVER(ORDER BY wb.WaybillNo) AS RowNumber, COUNT(1) OVER() RowCount,--用于分页
                    wsi.WaybillSignInfoID,wb.CreatTime AS SendTime,wis.IntoTime,wbs.CreatTime AS BackStationTime,wb.WaybillNO,wb.CustomerOrder,
					sit.StatusName AS WaybillTypeName,wbs.SignStatus,si.StatusName,					
                    NVL(wsi.NeedAmount,0) AS NeedAmount
					,NVL(wsi.NeedBackAmount,0) AS NeedBackAmount,wi.WayBillInfoWeight,wbs.AcceptType,
					e.EmployeeName,e.POSCode,ec.CompanyName,mi.MerchantName,case when wsi.FinancialStatus=1 then '已收款' else '未收款' end AS FinancialStatus
					,wsi.FinancialTime,wsi.FinancialStatus AS FinancialStatusID,wb.Status,wb.WaybillType,wb.BackStatus,sib.StatusName as BackStatusName
					,CASE WHEN wb.BackStatus IN (6,7) THEN '是' ELSE '否' end as ISBackBound
					FROM Waybill wb                
					INNER JOIN WaybillSignInfo wsi ON wb.WaybillNO=wsi.WaybillNO
					LEFT JOIN WaybillBackStation wbs ON wsi.BackStationInofID=wbs.WaybillBackStationID
					LEFT JOIN WaybillIntoStation wis ON wb.WaybillIntoStationID=wis.WaybillIntoStationID
					LEFT JOIN WaybillInfo wi ON wb.WaybillNO=wi.WayBillNO
					LEFT JOIN Employee e ON wbs.DeliverMan=e.EmployeeID
					LEFT JOIN ExpressCompany ec ON wb.DeliverStationID=ec.ExpressCompanyID
					INNER JOIN StatusInfo sit ON  wb.WaybillType=sit.StatusNO AND sit.StatusTypeNO=2
					INNER JOIN StatusInfo si ON wb.Status=si.StatusNO AND si.StatusTypeNO=1
					LEFT JOIN StatusInfo sib ON wb.BackStatus=sib.StatusNO AND sib.StatusTypeNO=5
					LEFT JOIN MerchantBaseInfo mi ON wb.MerchantID = mi.ID
              WHERE 1=1 {0} 
            )
            SELECT * FROM r {1}";

        private const string strBatchQueryOrderMoneyStore = @"if object_id('tempdb.dbo.#t_orderform') is not null
									drop table #t_orderform

									create table #t_orderform(formcode bigint)
									{0}

					    SELECT ROW_NUMBER() OVER(ORDER BY wb.WaybillNo) AS RowNumber,wb.CreatTime AS SendTime,wis.IntoTime,wbs.CreatTime AS BackStationTime,wb.WaybillNO,
						sit.StatusName AS WaybillTypeName,si.StatusName AS StatusName,wsi.NeedAmount,wsi.NeedBackAmount,wi.WayBillInfoWeight,wsi.AcceptType,
						e.EmployeeName,case when wsi.FinancialStatus=1 then '已收款' else '未收款' end AS FinancialStatus,wsi.FinancialTime,e.POSCode,
						sib.StatusName as BackStatusName,CASE WHEN wb.BackStatus IN (6,7) THEN '是' ELSE '否' end as ISBackBound,ec.CompanyName,mi.MerchantName,
						wsi.FinancialStatus AS FinancialStatusID,wb.Status,wb.WaybillType,wb.BackStatus,wsi.WaybillSignInfoID,wbs.SignStatus
						FROM  #t_orderform ord 
                        INNER JOIN Waybill wb on ord.formcode=wb.WaybillNO
						INNER JOIN WaybillSignInfo wsi ON wb.WaybillNO=wsi.WaybillNO
						LEFT JOIN WaybillBackStation wbs ON wsi.BackStationInofID=wbs.WaybillBackStationID
						LEFT JOIN WaybillIntoStation wis ON wb.WaybillIntoStationID=wis.WaybillIntoStationID
						LEFT JOIN WaybillInfo wi ON wb.WaybillNO=wi.WayBillNO
						LEFT JOIN Employee e ON wb.DeliverManID=e.EmployeeID
						LEFT JOIN ExpressCompany ec ON wb.DeliverStationID=ec.ExpressCompanyID
						INNER JOIN StatusInfo sit ON  wb.WaybillType=sit.StatusNO AND sit.StatusTypeNO=2
						INNER JOIN StatusInfo si ON wb.Status=si.StatusNO AND si.StatusTypeNO=1
						LEFT JOIN StatusInfo sib ON wb.BackStatus=sib.StatusNO AND sib.StatusTypeNO=5
						LEFT JOIN MerchantBaseInfo mi ON wb.MerchantID = mi.ID";

        private const string strUpdateFinancialStatus = @"UPDATE WaybillIntoStationWaybillSignInfo SET FinancialStatus = 1,FinancialTime = SysDate WHERE WaybillSignInfoID={0} AND SignStatus='{1}'";

        public DataTable GetBatchQuery(string strWaybillNOs)
        {
            DataTable dt = new DataTable();
            string strSql = String.Format(strBatchQueryOrderMoneyStore, strWaybillNOs);

            dt = OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql).Tables[0];
            return dt;
        }

        public DataTable GetBatchQueryV2(SearchCondition searchCondition)
        {
            var result = new DataTable();
            var orders = searchCondition.OrderNoList;
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
                    if (searchCondition.SearchWaybillType == 0)
                    {
                        var orderNo = orders.Rows[j]["运单号"].ToString().Trim();
                        if (String.IsNullOrEmpty(orderNo) ||
                            !StringUtil.IsValidFormCode(orderNo) ||
                            list.Contains(orderNo))
                        {
                            continue;
                        }
                        list.Add(orderNo);
                        temp.AppendFormat(" SELECT {0} UNION ALL ", orderNo);
                    }
                    else
                    {
                        var customerNo = orders.Rows[j]["订单号"].ToString().Trim();
                        if (list.Contains(customerNo))
                        {
                            continue;
                        }
                        list.Add(customerNo);
                        temp.AppendFormat(" SELECT {0} UNION ALL ", "'"+customerNo+"'");
                    }
                }
                if (string.IsNullOrEmpty(temp.ToString()))
                    continue;

                var strSql = new StringBuilder();
                strSql.AppendFormat(@"
                    --定义表变量
					IF OBJECT_ID('tempdb.dbo.#t_orderform') IS NOT NULL
					   DROP TABLE #t_orderform
					CREATE TABLE #t_orderform(WaybillNo {1})
                    CREATE INDEX ix_tmp_t_orderform_WaybillNO ON #t_orderform(WaybillNO)
					INSERT INTO #t_orderform(WaybillNo) {0}
                    
					SELECT ROW_NUMBER() OVER(ORDER BY wb.WaybillNo) AS RowNumber,wb.CreatTime AS SendTime,wis.IntoTime,wbs.CreatTime AS BackStationTime,wb.WaybillNO,wb.CustomerOrder,
						sit.StatusName AS WaybillTypeName,si.StatusName AS StatusName,wsi.NeedAmount,wsi.NeedBackAmount,wi.WayBillInfoWeight,wsi.AcceptType,
						e.EmployeeName,case when wsi.FinancialStatus=1 then '已收款' else '未收款' end AS FinancialStatus,wsi.FinancialTime,e.POSCode,
						sib.StatusName as BackStatusName,CASE WHEN wb.BackStatus IN (6,7) THEN '是' ELSE '否' end as ISBackBound,ec.CompanyName,mi.MerchantName,
						wsi.FinancialStatus AS FinancialStatusID,wb.Status,wb.WaybillType,wb.BackStatus,wsi.WaybillSignInfoID,wbs.SignStatus
						FROM  #t_orderform ord 
                        INNER JOIN Waybill wb on ord.WaybillNO={2}
						INNER JOIN WaybillSignInfo wsi ON wb.WaybillNO=wsi.WaybillNO
						LEFT JOIN WaybillBackStation wbs ON wsi.BackStationInofID=wbs.WaybillBackStationID
						LEFT JOIN WaybillIntoStation wis ON wb.WaybillIntoStationID=wis.WaybillIntoStationID
						LEFT JOIN WaybillInfo wi ON wb.WaybillNO=wi.WayBillNO
						LEFT JOIN Employee e ON wb.DeliverManID=e.EmployeeID
						LEFT JOIN ExpressCompany ec ON wb.DeliverStationID=ec.ExpressCompanyID
						INNER JOIN StatusInfo sit ON  wb.WaybillType=sit.StatusNO AND sit.StatusTypeNO=2
						INNER JOIN StatusInfo si ON wb.Status=si.StatusNO AND si.StatusTypeNO=1
						LEFT JOIN StatusInfo sib ON wb.BackStatus=sib.StatusNO AND sib.StatusTypeNO=5
						LEFT JOIN MerchantBaseInfo mi ON wb.MerchantID = mi.ID", temp.ToString().TrimEnd("UNION ALL".ToCharArray()),
                searchCondition.SearchWaybillType == 0 ? "BIGINT" : "NVARCHAR(50)",
                searchCondition.SearchWaybillType == 0 ? "wb.WaybillNO" : "wb.CustomerOrder");
                var ds = OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql.ToString());
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
        /// 财务收款确认
        /// </summary>
        /// <param name="strSignID">签收ID</param>
        /// <returns></returns>
        public bool UpdateFinancialStatus(string strSignID)
        {
            string strSql = String.Format(strUpdateFinancialStatus, strSignID,
                                          (int)WayBillStatus.Success);

            int i = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql);
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 通过查询条件得到查询结果
        /// </summary>
        /// <param name="ht">条件列表</param>
        /// <returns></returns>
        public DataTable GetOrderMoneyStoreInfo(Hashtable ht)
        {
            DataTable dt = new DataTable();
            string strWhere = "";
            string strPageWhere = "";
            if (ht.Count > 0)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(ht["StationID"])))
                {
                    strWhere += " and wbs.DeliverStation=" + Convert.ToString(ht["StationID"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(ht["IntoBound"])))
                {
                    //if (Convert.ToString(ht["IntoBound"])=="0")
                    //{
                    //    strWhere += String.Format(" and (wb.[BackStatus] not in ({0},{1}) or wb.[BackStatus] is null) ", (int)WayBillStatus.ReturnBounded, (int)WayBillStatus.RefusedBounded);
                    //}
                    //else
                    //{
                    //    strWhere += String.Format(" and wb.[BackStatus] in ({0},{1}) ", (int)WayBillStatus.ReturnBounded, (int)WayBillStatus.RefusedBounded); //" and wb.Status in ('6','7') ";
                    //}

                    if (Convert.ToString(ht["IntoBound"]) == "0")
                    {
                        strWhere += " AND (NOT EXISTS (SELECT StatusNO FROM StatusInfo si WHERE si.StatusTypeNO=5 AND si.StatusNO=wb.BackStatus)) ";
                    }
                    else
                    {
                        strWhere += " AND (EXISTS (SELECT StatusNO FROM StatusInfo si WHERE si.StatusTypeNO=5 AND si.StatusNO=wb.BackStatus)) "; //" and wb.Status in ('6','7') ";
                    }

                }
                if (Convert.ToString(ht["Source"]) != "")
                {
                    strWhere += " and wb.Sources=" + Convert.ToString(ht["Source"]);
                }
                if (Convert.ToString(ht["MerchantID"]) != "")
                {
                    strWhere += " and wb.MerchantID=" + Convert.ToString(ht["MerchantID"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(ht["MoneyStatus"])))
                {
                    if (Convert.ToString(ht["MoneyStatus"]) == "0")
                    {
                        strWhere += " and wsi.FinancialStatus<>1 ";
                    }
                    else
                    {
                        strWhere += " and wsi.FinancialStatus=1 ";
                    }

                }
                if (!String.IsNullOrEmpty(Convert.ToString(ht["IntoTime"])))
                {
                    strWhere += " and wis.IntoTime BETWEEN '" + Convert.ToString(ht["IntoTime"]) + " 00:00:00' AND '" + Convert.ToString(ht["IntoTime"]) + " 23:59:59'";
                }

                if (!String.IsNullOrEmpty(Convert.ToString(ht["SignTime"])))
                {
                    strWhere += " and wsi.SignTime BETWEEN '" + Convert.ToString(ht["SignTime"]) + " 00:00:00' AND '" + Convert.ToString(ht["SignTime"]) + " 23:59:59'";
                }

                if (!String.IsNullOrEmpty(Convert.ToString(ht["BegTime"])) && !String.IsNullOrEmpty(Convert.ToString(ht["EndTime"])))
                {
                    strWhere += " and wb.CreatTime BETWEEN '" + Convert.ToString(ht["BegTime"]) + "' AND '" + Convert.ToString(ht["EndTime"]) + "'";
                }

                if (!String.IsNullOrEmpty(Convert.ToString(ht["WaybillNO"])))
                {
                    strWhere = " and wb.WaybillNO in (" + Convert.ToString(ht["WaybillNO"]) + ") ";
                }
                if (ht["IsPageing"].Equals("1"))
                {
                    strPageWhere = String.Format(" WHERE r.RowNumber BETWEEN {0} AND {1}", ht["StartIndex"], ht["EndIndex"]);
                }
            }
            var strSql = String.Format(strOrderMoneyStoreInfoSQL, strWhere, strPageWhere);
            if (!ht["IsPageing"].Equals("1"))
            {
                strSql = strSql.Replace("*", @"RowNumber AS 序号,SendTime AS 发货时间,IntoTime AS 入站时间,BackStationTime AS 归班结算时间,WaybillNO AS 运单号,CustomerOrder AS 订单号
                    WaybillTypeName AS 订单类型,StatusName AS 状态,NeedAmount AS 应收金额,NeedBackAmount AS 应退金额,WayBillInfoWeight AS 订单重量,
                    AcceptType AS 付款方式,EmployeeName AS 配送员,FinancialStatus AS 财务收款状态,FinancialTime AS 收款确认时间,POSCode AS POS机终端号,
                    BackStatusName AS 入库状态,ISBackBound AS 是否入库,CompanyName AS 站点, MerchantName AS 商家名称");
            }
            dt = OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql).Tables[0];
            return dt;
        }

    }
}
