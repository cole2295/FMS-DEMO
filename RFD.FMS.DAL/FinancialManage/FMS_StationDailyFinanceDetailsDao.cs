using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Model.UnionPay;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.FinancialManage
{
    public class FMS_StationDailyFinanceDetailsDao : SqlServerDao, IFMS_StationDailyFinanceDetailsDao
    {
        private const string TableName = @"LMS_RFD.dbo.FMS_StationDailyFinanceDetails";

        #region 查询报表数据SQL

        private const string strQueryDayData =
            @"SELECT  wis.IntoTime ,--接货时间
                                            wsi.WaybillNO ,--订单号        
                                            mbi.MerchantName,
                                            w.WaybillType ,--订单类型
                                            wbs.NeedAmount ,--应收金额
                                            wbs.NeedBackAmount ,--应退金额
                                            wbs.FactAmount ,--实收金额
                                            wbs.FactBackAmount ,--实退金额
                                            wi.WayBillInfoWeight,--重量
                                            ISNULL(wi.WayBillInfoVolume,0) as WayBillInfoVolume,--体积
                                            wbs.SignStatus ,--配送结果
                                            wbs.AcceptType ,--付款方式
                                            wbs.Remark,
                                            wbs.DeliverMan,
                                            w.Sources,
                                            w.MerchantID,
                                            e.POSCode ,--POS机终端号
                                            wsi.SignTime ,--签收时间
                                            e.EmployeeID ,--配送员编号
                                            e.EmployeeName ,--配送员
                                            wsi.DeductNotes,  --提成批注
                                            wbs.DeductMoney,  --提成标准
                                            wsi.protectedprice, --保价费
                                            w.CustomerOrder   --订单号
                                    FROM    LMS_RFD.dbo.WaybillBackStation wbs ( NOLOCK )
                                            JOIN LMS_RFD.dbo.WaybillSignInfo wsi ( NOLOCK ) ON wsi.BackStationInofID = wbs.WaybillBackStationID                                                   
                                            JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON wsi.WaybillNO = w.WaybillNO                                                                                       
                                            LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID = w.MerchantID        
                                            JOIN LMS_RFD.dbo.WaybillIntoStation wis ( NOLOCK ) ON wis.WaybillIntoStationID = w.WaybillIntoStationID
                                            JOIN RFD_PMS.dbo.Employee e ( NOLOCK ) ON e.EmployeeID = wbs.DeliverMan
                                            INNER JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON w.WaybillNO=wi.WaybillNO
                                    WHERE wbs.CreatTime >= @BegTime AND wbs.CreatTime < @Endtime AND wbs.DeliverStation = @StationID
                                          AND wbs.SignStatus IN ( '3','5' ) AND wbs.IsDelete = 0 AND w.IsDelete = 0 AND w.[Status] <> '-9'  
                                          AND w.Sources=@Sources {0}
                                    UNION ALL  

                                    SELECT  T1.IntoTime ,--接货时间
                                            wsi.WaybillNO ,--订单号
                                            mbi.MerchantName,--商家名称
                                            w.WaybillType ,--订单类型
                                            wsi.NeedAmount ,--应收金额
                                            wsi.NeedBackAmount ,--应退金额
                                            0 AS FactAmount ,--实收金额
                                            0 AS FactBackAmount ,--实退金额
                                            wi.WayBillInfoWeight, 
                                            ISNULL(wi.WayBillInfoVolume,0) as WayBillInfoVolume,--体积                
                                            T1.SignStatus ,--配送结果
                                            '' AS AcceptType ,--付款方式
                                            '' as Remark,
                                            '' as DeliverMan,
                                            w.Sources,
                                            w.MerchantID,
                                            '' AS POSCode ,--POS机终端号
                                            NULL AS SignTime ,--签收时间
                                            NULL AS EmployeeID ,--配送员编号
                                            '' AS EmployeeName ,--配送员 
                                            '' AS DeductNotes,  --提成批注 
                                            NULL AS DeductMoney,   --提成标准
                                            wsi.protectedprice, --保价费
                                            w.CustomerOrder --订单号
                                    FROM    ( SELECT    wis.WaybillNO ,
                                                        wis.IntoStation ,
                                                        wis.IntoTime ,
                                                        wis.CreatStation ,
                                                        wis.CreatTime ,
                                                        '4' AS SignStatus
                                              FROM      ( SELECT    WaybillNO ,
                                                                    IntoStation ,
                                                                    IntoTime ,
                                                                    CreatStation ,
                                                                    CreatTime
                                                          FROM      LMS_RFD.dbo.WaybillIntoStation (NOLOCK)
                                                          WHERE     CreatTime >= @BegTime
                                                                    AND CreatTime < @Endtime
                                                                    AND CreatStation = @StationID
                                                        ) wis
                                              WHERE     NOT EXISTS ( SELECT 1
                                                                     FROM   LMS_RFD.dbo.WaybillBackStation wbs ( NOLOCK )
                                                                     WHERE  wis.WaybillNO = wbs.WaybillNO
                                                                            AND wis.IntoStation = wbs.CreatStation
                                                                            AND wbs.CreatTime > wis.CreatTime
                                                                            AND wbs.CreatTime < @Endtime
                                                                            AND wbs.SignStatus IN ( '3', '5' )
                                                                            AND wbs.IsDelete = 0 )
                                                        AND NOT EXISTS ( SELECT 1
                                                                         FROM   LMS_RFD.dbo.TurnStation ts1 ( NOLOCK )
                                                                         WHERE  ts1.ApplyStation = wis.CreatStation
                                                                                AND ts1.WaybillNO = wis.WaybillNO
                                                                                AND ts1.RecipTime > wis.CreatTime
                                                                                AND ts1.RecipTime < @Endtime
                                                                                AND ts1.Status = '3'                                            
                                                                                )
                                            ) T1
                                            JOIN LMS_RFD.dbo.WaybillSignInfo wsi ( NOLOCK ) ON T1.WaybillNO = wsi.WaybillNO
                                            JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON T1.WaybillNO = w.WaybillNO                                                                                                                   
                                            LEFT JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID = w.MerchantID 
                                            INNER JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON w.WaybillNO=wi.WaybillNO 
                                            WHERE w.IsDelete = 0 AND w.Status <> '-9' AND w.Sources=@Sources {0}
                                            ";

        #endregion

        #region 转站入站汇总信息提取SQL

        /// <summary>
        /// 转站入站汇总信息提取SQL
        /// </summary>
        private const string TrunIntoStationSummaryByStationAndTimeSqlStr =
            @"SELECT  ( SELECT    COUNT(0) WaybillCount
          FROM      LMS_RFD.dbo.TurnStation (NOLOCK) ts
                    JOIN LMS_RFD.dbo.WaybillSignInfo (NOLOCK) wsi ON ts.WaybillNO = wsi.WaybillNO
                                                             AND ts.IsDeleted = 0
                                                             AND ts.RecipTime >= '{1}'
                                                             AND ts.RecipTime < '{2}'
                                                             AND ts.RecipcorpStation = {0}
                                                             AND ts.Status = '3'
                                                             AND ( wsi.IsDelete IS NULL
                                                              OR wsi.IsDelete = 0
                                                              )
                     JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON wsi.WaybillNO = w.WaybillNO
                                         AND w.IsDelete = 0
                                         {3}
        ) WaybillCount ,
        ( SELECT    ISNULL(SUM(ISNULL(wsi.NeedAmount, 0)), 0) NeedAmountSum
          FROM      LMS_RFD.dbo.TurnStation (NOLOCK) ts
                   JOIN LMS_RFD.dbo.WaybillSignInfo (NOLOCK) wsi ON ts.WaybillNO = wsi.WaybillNO
                                                             AND ts.IsDeleted = 0
                                                             AND ts.RecipTime >= '{1}'
                                                             AND ts.RecipTime < '{2}'
                                                             AND ts.RecipcorpStation = {0}
                                                             AND ts.Status = '3'
                                                             AND ( wsi.IsDelete IS NULL
                                                              OR wsi.IsDelete = 0
                                                              )
                 JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON wsi.WaybillNO = w.WaybillNO
                                                         AND w.IsDelete = 0
                                                         {3}
        ) NeedAmountSum ,
        ( SELECT    ISNULL(SUM(ISNULL(wsi.NeedBackAmount, 0)), 0) NeedBackAmountSum
          FROM      LMS_RFD.dbo.TurnStation (NOLOCK) ts
                    JOIN LMS_RFD.dbo.WaybillSignInfo (NOLOCK) wsi ON ts.WaybillNO = wsi.WaybillNO
                                                             AND ts.IsDeleted = 0
                                                             AND ts.RecipTime >= '{1}'
                                                             AND ts.RecipTime < '{2}'
                                                             AND ts.RecipcorpStation = {0}
                                                             AND ts.Status = '3'
                                                             AND ( wsi.IsDelete IS NULL
                                                              OR wsi.IsDelete = 0
                                                              )
                     JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON wsi.WaybillNO = w.WaybillNO
                                         AND w.IsDelete = 0
                                         {3}
        ) NeedBackAmountSum";

        #endregion

        #region 转站出站汇总信息提取SQL

        /// <summary>
        /// 转站出站汇总信息提取SQL
        /// </summary>
        private const string TrunOutStationSummaryByStationAndTimeSqlStr =
            @"SELECT  ( SELECT    COUNT(0) WaybillCount
          FROM      LMS_RFD.dbo.TurnStation (NOLOCK) ts
                    JOIN LMS_RFD.dbo.WaybillSignInfo (NOLOCK) wsi ON ts.WaybillNO = wsi.WaybillNO
                                                             AND ts.IsDeleted = 0
                                                             AND ts.RecipTime >= '{1}'
                                                             AND ts.RecipTime < '{2}'
                                                             AND ts.ApplyStation = {0}
                                                             AND ts.Status = '3'
                                                             AND ( wsi.IsDelete IS NULL
                                                              OR wsi.IsDelete = 0
                                                              )
                     JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON wsi.WaybillNO = w.WaybillNO
                                         AND w.IsDelete = 0
                                         {3}
        ) WaybillCount ,
        ( SELECT    ISNULL(SUM(ISNULL(wsi.NeedAmount, 0)), 0) NeedAmountSum
          FROM     LMS_RFD.dbo.TurnStation (NOLOCK) ts
                   JOIN LMS_RFD.dbo.WaybillSignInfo (NOLOCK) wsi ON ts.WaybillNO = wsi.WaybillNO
                                                             AND ts.IsDeleted = 0
                                                             AND ts.RecipTime >= '{1}'
                                                             AND ts.RecipTime < '{2}'
                                                             AND ts.ApplyStation = {0}
                                                             AND ts.Status = '3'
                                                             AND ( wsi.IsDelete IS NULL
                                                              OR wsi.IsDelete = 0
                                                              )
                 JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON wsi.WaybillNO = w.WaybillNO
                                                         AND w.IsDelete = 0
                                                         {3}
        ) NeedAmountSum ,
        ( SELECT    ISNULL(SUM(ISNULL(wsi.NeedBackAmount, 0)), 0) NeedBackAmountSum
          FROM      LMS_RFD.dbo.TurnStation (NOLOCK) ts
                    JOIN LMS_RFD.dbo.WaybillSignInfo (NOLOCK) wsi ON ts.WaybillNO = wsi.WaybillNO
                                                             AND ts.IsDeleted = 0
                                                             AND ts.RecipTime >= '{1}'
                                                             AND ts.RecipTime < '{2}'
                                                             AND ts.ApplyStation = {0}
                                                             AND ts.Status = '3'
                                                             AND ( wsi.IsDelete IS NULL
                                                              OR wsi.IsDelete = 0
                                                              )
                 JOIN LMS_RFD.dbo.Waybill w ( NOLOCK ) ON wsi.WaybillNO = w.WaybillNO
                                         AND w.IsDelete = 0
                                         {3}
        ) NeedBackAmountSum";

        #endregion

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(FMS_StationDailyFinanceDetails model)
        {
            var strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" EnterTime , ");
            strSql.Append(" WaybillNO , ");
            strSql.Append(" ReplaceTypeName , ");
            strSql.Append(" NeedPrice , ");
            strSql.Append(" NeedReturnPrice , ");
            strSql.Append(" PriceDiff , ");
            strSql.Append(" PriceReturnCash , ");
            strSql.Append(" Weight , ");
            strSql.Append(" WaybillType , ");
            strSql.Append(" StatusName , ");
            strSql.Append(" AcceptType , ");
            strSql.Append(" RejectReason , ");
            strSql.Append(" ResortReason , ");
            strSql.Append(" PostTime , ");
            strSql.Append(" DeliverManName , ");
            strSql.Append(" Comment , ");
            strSql.Append(" Status , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" StationID , ");
            strSql.Append(" Sources , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" OPType , ");
            strSql.Append(" PosNum , ");
            strSql.Append(" LmsId , ");
            strSql.Append(" DeductMoney,  ");
            strSql.Append(" CustomerOrder,  ");
            strSql.Append(" DailyTime,  ");
            strSql.Append(" DeliverFee,  ");
            strSql.Append(" Protectedprice,");
            strSql.Append(" DeliverManID, ");
            strSql.Append(" IsChange ");
            strSql.Append(") values (");
            strSql.Append(" @EnterTime , ");
            strSql.Append(" @WaybillNO , ");
            strSql.Append(" @ReplaceTypeName , ");
            strSql.Append(" @NeedPrice , ");
            strSql.Append(" @NeedReturnPrice , ");
            strSql.Append(" @PriceDiff , ");
            strSql.Append(" @PriceReturnCash , ");
            strSql.Append(" @Weight , ");
            strSql.Append(" @WaybillType , ");
            strSql.Append(" @StatusName , ");
            strSql.Append(" @AcceptType , ");
            strSql.Append(" @RejectReason , ");
            strSql.Append(" @ResortReason , ");
            strSql.Append(" @PostTime , ");
            strSql.Append(" @DeliverManName , ");
            strSql.Append(" @Comment , ");
            strSql.Append(" @Status , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @StationID , ");
            strSql.Append(" @Sources , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @OPType , ");
            strSql.Append(" @PosNum , ");
            strSql.Append(" @LmsId , ");
            strSql.Append(" @DeductMoney, ");
            strSql.Append(" @CustomerOrder,");
            strSql.Append(" @DailyTime,  ");
            strSql.Append(" @DeliverFee,  ");
            strSql.Append(" @Protectedprice, ");
            strSql.Append(" @DeliverManID,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(" ) ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                            new SqlParameter(string.Format("@{0}", "EnterTime"), model.EnterTime),
                                            new SqlParameter(string.Format("@{0}", "WaybillNO"), model.WaybillNO),
                                            new SqlParameter(string.Format("@{0}", "ReplaceTypeName"),
                                                             model.ReplaceTypeName),
                                            new SqlParameter(string.Format("@{0}", "NeedPrice"), model.NeedPrice),
                                            new SqlParameter(string.Format("@{0}", "NeedReturnPrice"),
                                                             model.NeedReturnPrice),
                                            new SqlParameter(string.Format("@{0}", "PriceDiff"), model.PriceDiff),
                                            new SqlParameter(string.Format("@{0}", "PriceReturnCash"),
                                                             model.PriceReturnCash),
                                            new SqlParameter(string.Format("@{0}", "Weight"), model.Weight),
                                            new SqlParameter(string.Format("@{0}", "WaybillType"), model.WaybillType),
                                            new SqlParameter(string.Format("@{0}", "StatusName"), model.StatusName),
                                            new SqlParameter(string.Format("@{0}", "AcceptType"), model.AcceptType),
                                            new SqlParameter(string.Format("@{0}", "RejectReason"), model.RejectReason),
                                            new SqlParameter(string.Format("@{0}", "ResortReason"), model.ResortReason),
                                            new SqlParameter(string.Format("@{0}", "PostTime"), model.PostTime),
                                            new SqlParameter(string.Format("@{0}", "DeliverManName"),
                                                             model.DeliverManName),
                                            new SqlParameter(string.Format("@{0}", "Comment"), model.Comment),
                                            new SqlParameter(string.Format("@{0}", "Status"), model.Status),
                                            new SqlParameter(string.Format("@{0}", "CreateTime"), model.CreateTime),
                                            new SqlParameter(string.Format("@{0}", "StationID"), model.StationID),
                                            new SqlParameter(string.Format("@{0}", "Sources"), model.Sources),
                                            new SqlParameter(string.Format("@{0}", "MerchantID"), model.MerchantID),
                                            new SqlParameter(string.Format("@{0}", "OPType"), model.OPType),
                                            new SqlParameter(string.Format("@{0}", "PosNum"), model.PosNum),
                                            new SqlParameter(string.Format("@{0}", "LmsId"), model.LmsId),
                                            new SqlParameter(string.Format("@{0}", "DeductMoney"), model.DeductMoney),
                                            new SqlParameter(string.Format("@{0}", "CustomerOrder"), model.CustomerOrder)
                                            ,
                                            new SqlParameter(string.Format("@{0}", "DailyTime"), model.DailyTime),
                                            new SqlParameter(string.Format("@{0}", "DeliverFee"), model.DeliverFee),
                                            new SqlParameter(string.Format("@{0}", "Protectedprice"),
                                                             model.Protectedprice),
                                            new SqlParameter(string.Format("@{0}", "DeliverManID"), model.DeliverManID),
                                            new SqlParameter(string.Format("@{0}", "IsChange"), true)
                                        };
            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddV2(FMS_StationDailyFinanceDetails model)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into FMS_StationDailyFinanceDetails(");
            strSql.Append(" EnterTime , ");
            strSql.Append(" WaybillNO , ");
            strSql.Append(" ReplaceTypeName , ");
            strSql.Append(" NeedPrice , ");
            strSql.Append(" NeedReturnPrice , ");
            strSql.Append(" PriceDiff , ");
            strSql.Append(" PriceReturnCash , ");
            strSql.Append(" Weight , ");
            strSql.Append(" WaybillType , ");
            strSql.Append(" StatusName , ");
            strSql.Append(" AcceptType , ");
            strSql.Append(" RejectReason , ");
            strSql.Append(" ResortReason , ");
            strSql.Append(" PostTime , ");
            strSql.Append(" DeliverManName , ");
            strSql.Append(" Comment , ");
            strSql.Append(" Status , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" StationID , ");
            strSql.Append(" Sources , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" OPType , ");
            strSql.Append(" PosNum , ");
            strSql.Append(" LmsId , ");
            strSql.Append(" DeductMoney,  ");
            strSql.Append(" CustomerOrder,  ");
            strSql.Append(" DailyTime,  ");
            strSql.Append(" DeliverFee,  ");
            strSql.Append(" Protectedprice,");
            strSql.Append(" DeliverManID, ");
            strSql.Append(" IsChange ");
            strSql.Append(") values (");
            strSql.Append(" @EnterTime , ");
            strSql.Append(" @WaybillNO , ");
            strSql.Append(" @ReplaceTypeName , ");
            strSql.Append(" @NeedPrice , ");
            strSql.Append(" @NeedReturnPrice , ");
            strSql.Append(" @PriceDiff , ");
            strSql.Append(" @PriceReturnCash , ");
            strSql.Append(" @Weight , ");
            strSql.Append(" @WaybillType , ");
            strSql.Append(" @StatusName , ");
            strSql.Append(" @AcceptType , ");
            strSql.Append(" @RejectReason , ");
            strSql.Append(" @ResortReason , ");
            strSql.Append(" @PostTime , ");
            strSql.Append(" @DeliverManName , ");
            strSql.Append(" @Comment , ");
            strSql.Append(" @Status , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @StationID , ");
            strSql.Append(" @Sources , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @OPType , ");
            strSql.Append(" @PosNum , ");
            strSql.Append(" @LmsId , ");
            strSql.Append(" @DeductMoney, ");
            strSql.Append(" @CustomerOrder,");
            strSql.Append(" @DailyTime,  ");
            strSql.Append(" @DeliverFee,  ");
            strSql.Append(" @Protectedprice, ");
            strSql.Append(" @DeliverManID,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(" ) ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                            new SqlParameter(string.Format("@{0}", "EnterTime"), model.EnterTime),
                                            new SqlParameter(string.Format("@{0}", "WaybillNO"), model.WaybillNO),
                                            new SqlParameter(string.Format("@{0}", "ReplaceTypeName"),
                                                             model.ReplaceTypeName),
                                            new SqlParameter(string.Format("@{0}", "NeedPrice"), model.NeedPrice),
                                            new SqlParameter(string.Format("@{0}", "NeedReturnPrice"),
                                                             model.NeedReturnPrice),
                                            new SqlParameter(string.Format("@{0}", "PriceDiff"), model.PriceDiff),
                                            new SqlParameter(string.Format("@{0}", "PriceReturnCash"),
                                                             model.PriceReturnCash),
                                            new SqlParameter(string.Format("@{0}", "Weight"), model.Weight),
                                            new SqlParameter(string.Format("@{0}", "WaybillType"), model.WaybillType),
                                            new SqlParameter(string.Format("@{0}", "StatusName"), model.StatusName),
                                            new SqlParameter(string.Format("@{0}", "AcceptType"), model.AcceptType),
                                            new SqlParameter(string.Format("@{0}", "RejectReason"), model.RejectReason),
                                            new SqlParameter(string.Format("@{0}", "ResortReason"), model.ResortReason),
                                            new SqlParameter(string.Format("@{0}", "PostTime"), model.PostTime),
                                            new SqlParameter(string.Format("@{0}", "DeliverManName"),
                                                             model.DeliverManName),
                                            new SqlParameter(string.Format("@{0}", "Comment"), model.Comment),
                                            new SqlParameter(string.Format("@{0}", "Status"), model.Status),
                                            new SqlParameter(string.Format("@{0}", "CreateTime"), model.CreateTime),
                                            new SqlParameter(string.Format("@{0}", "StationID"), model.StationID),
                                            new SqlParameter(string.Format("@{0}", "Sources"), model.Sources),
                                            new SqlParameter(string.Format("@{0}", "MerchantID"), model.MerchantID),
                                            new SqlParameter(string.Format("@{0}", "OPType"), model.OPType),
                                            new SqlParameter(string.Format("@{0}", "PosNum"), model.PosNum),
                                            new SqlParameter(string.Format("@{0}", "LmsId"), model.LmsId),
                                            new SqlParameter(string.Format("@{0}", "DeductMoney"), model.DeductMoney),
                                            new SqlParameter(string.Format("@{0}", "CustomerOrder"), model.CustomerOrder)
                                            ,
                                            new SqlParameter(string.Format("@{0}", "DailyTime"), model.DailyTime),
                                            new SqlParameter(string.Format("@{0}", "DeliverFee"), model.DeliverFee),
                                            new SqlParameter(string.Format("@{0}", "Protectedprice"),
                                                             model.Protectedprice),
                                            new SqlParameter(string.Format("@{0}", "DeliverManID"), model.DeliverManID),
                                            new SqlParameter(string.Format("@{0}", "IsChange"), true)
                                        };
            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 通过查询条件得到报表明细 add by wangyongc 2011-09-05
        /// </summary>
        /// <param name="dtDailyDate"></param>
        /// <param name="strStaionID"></param>
        /// <param name="strSources"></param>
        /// <param name="strMerchantID"></param>
        /// <returns></returns>
        public DataTable GetOrderDetil(DateTime dtDailyDate, string strStaionID, string strSources, string strMerchantID)
        {
            string strSql = strQueryDayData;
            string strS3 = "";
            if (strSources == Convert.ToString((int) WaybillSourse.Other) && !String.IsNullOrEmpty(strMerchantID))
            {
                strS3 += " AND w.MerchantID=" + strMerchantID;
            }
            strSql = String.Format(strSql, strS3);

            IList<SqlParameter> paramList = new List<SqlParameter>(); //参数列表
            //添加参数
            paramList.Add(new SqlParameter("@BegTime", SqlDbType.DateTime, 20)
                              {Value = dtDailyDate.ToString("yyyy-MM-dd 00:00:00")});
            paramList.Add(new SqlParameter("@Endtime", SqlDbType.DateTime, 20)
                              {Value = dtDailyDate.ToString("yyyy-MM-dd 23:59:59")});
            paramList.Add(new SqlParameter("@StationID", SqlDbType.Int, 20) {Value = strStaionID});
            paramList.Add(new SqlParameter("@Sources", SqlDbType.Int) {Value = strSources});

            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection,
                                            CommandType.Text, strSql,
                                            paramList.ToArray()).Tables[0];
        }

        /// <summary>
        /// 转站出站汇总信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stationCode"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DataTable GetTrunOutStationSummaryByStationAndTime(int? type, string stationCode, DateTime dateTime,int? merchantID)
        {
            string strSql = string.Format(TrunOutStationSummaryByStationAndTimeSqlStr,
                                          stationCode,
                                          dateTime.ToString("yyyy-MM-dd 00:00:00"),
                                          dateTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"),
                                          (type == null ? string.Empty : string.Format("And w.Sources={0}", type)) +
                                          (merchantID == null
                                               ? string.Empty
                                               : string.Format("And w.MerchantID={0}", merchantID))
                );
            DataSet ds = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, strSql);
            return ds.Tables[0];
        }

        /// <summary>
        /// 转站入站汇总信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stationCode"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DataTable GetTrunIntoStationSummaryByStationAndTime(int? type, string stationCode, DateTime dateTime,int? merchantID)
        {
            string strSql = string.Format(TrunIntoStationSummaryByStationAndTimeSqlStr,
                                          stationCode,
                                          dateTime.ToString("yyyy-MM-dd 00:00:00"),
                                          dateTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"),
                                          (type == null ? string.Empty : string.Format("And w.Sources={0}", type)) +
                                          (merchantID == null
                                               ? string.Empty
                                               : string.Format("And w.MerchantID={0}", merchantID))
                );
            DataSet ds = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, strSql);
            return ds.Tables[0];
        }

        /// <summary>
        /// 判断该订单是否已经生成报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ExistsV2(FMS_StationDailyFinanceDetails model)
        {
            string dateDaily = ((DateTime)model.DailyTime).ToString("yyyy-MM-dd");
            string sql =
                string.Format(
                    @"SELECT COUNT(0)
            FROM   FMS_StationDailyFinanceDetails(NOLOCK)
            WHERE  WaybillNO = {0}
                   AND DailyTime='{1}'  ",
                    model.WaybillNO, dateDaily);

            return Convert.ToInt32(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql)) > 0;
        }

        /// <summary>
        /// 判断该订单是否已经生成报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists(FMS_StationDailyFinanceDetails model)
        {
            string dateDaily = ((DateTime) model.DailyTime).ToString("yyyy-MM-dd");
            string sql =
                string.Format(
                    @"SELECT COUNT(0)
            FROM   LMS_RFD.dbo.FMS_StationDailyFinanceDetails(NOLOCK)
            WHERE  WaybillNO = {0}
                   AND DailyTime='{1}'  ",
                    model.WaybillNO, dateDaily);
            return Convert.ToInt32(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql)) > 0;
        }
    }
}