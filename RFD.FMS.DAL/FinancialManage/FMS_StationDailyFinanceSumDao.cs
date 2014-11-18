using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using RFD.FMS.Model.UnionPay;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.FinancialManage
{
    public class FMS_StationDailyFinanceSumDao : SqlServerDao, IFMS_StationDailyFinanceSumDao
    {
        private const string TableName = @"LMS_RFD.dbo.FMS_StationDailyFinanceSum";

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(FMS_StationDailyFinanceSum model)
        {
            var strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" DayReceiveOrderCount , ");
            strSql.Append(" DayReceiveNeedInSum , ");
            strSql.Append(" DayReceiveNeedOutSum , ");
            strSql.Append(" DayReceiveGoodsCount , ");
            strSql.Append(" DayOutOrderCount , ");
            strSql.Append(" PreDayResortCount , ");
            strSql.Append(" PreDayResortNeedInSum , ");
            strSql.Append(" PreDayResortNeedOutSum , ");
            strSql.Append(" DayTransferOrderCount , ");
            strSql.Append(" DayInStationNeedInSum , ");
            strSql.Append(" DayInStationNeedOutSum , ");
            strSql.Append(" DayNeedDeliverOrderCount , ");
            strSql.Append(" DayNeedDeliverInSum , ");
            strSql.Append(" DayNeedDeliverOutSum , ");
            strSql.Append(" CashSuccOrderCount , ");
            strSql.Append(" CashRealInSum , ");
            strSql.Append(" CashRealOutSum , ");
            strSql.Append(" PosSuccOrderCount , ");
            strSql.Append(" PosRealInSum , ");
            strSql.Append(" DeliverSuccRate , ");
            strSql.Append(" RejectOrderCount , ");
            strSql.Append(" AllRejectNeedInSum , ");
            strSql.Append(" AllRejectNeedOutSum , ");
            strSql.Append(" DayResortCount , ");
            strSql.Append(" DayResortNeedInSum , ");
            strSql.Append(" DayResortNeedOutSum , ");
            strSql.Append(" DayOutStationOrderCount , ");
            strSql.Append(" DayOutStationNeedInSum , ");
            strSql.Append(" DayOutStationNeedOutSum , ");
            strSql.Append(" ResortRate , ");
            strSql.Append(" StationID , ");
            strSql.Append(" Sources , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" DailyTime , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" FinanceStatus , ");
            strSql.Append(" RealInCome , ");
            strSql.Append(" POSChecked , ");
            strSql.Append(" Remark , ");
            strSql.Append(" DayOutOrderSum, ");
            strSql.Append(" ExchangeOrderCount, ");
            strSql.Append(" ExchangeOrderSum,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" @DayReceiveOrderCount , ");
            strSql.Append(" @DayReceiveNeedInSum , ");
            strSql.Append(" @DayReceiveNeedOutSum , ");
            strSql.Append(" @DayReceiveGoodsCount , ");
            strSql.Append(" @DayOutOrderCount , ");
            strSql.Append(" @PreDayResortCount , ");
            strSql.Append(" @PreDayResortNeedInSum , ");
            strSql.Append(" @PreDayResortNeedOutSum , ");
            strSql.Append(" @DayTransferOrderCount , ");
            strSql.Append(" @DayInStationNeedInSum , ");
            strSql.Append(" @DayInStationNeedOutSum , ");
            strSql.Append(" @DayNeedDeliverOrderCount , ");
            strSql.Append(" @DayNeedDeliverInSum , ");
            strSql.Append(" @DayNeedDeliverOutSum , ");
            strSql.Append(" @CashSuccOrderCount , ");
            strSql.Append(" @CashRealInSum , ");
            strSql.Append(" @CashRealOutSum , ");
            strSql.Append(" @PosSuccOrderCount , ");
            strSql.Append(" @PosRealInSum , ");
            strSql.Append(" @DeliverSuccRate , ");
            strSql.Append(" @RejectOrderCount , ");
            strSql.Append(" @AllRejectNeedInSum , ");
            strSql.Append(" @AllRejectNeedOutSum , ");
            strSql.Append(" @DayResortCount , ");
            strSql.Append(" @DayResortNeedInSum , ");
            strSql.Append(" @DayResortNeedOutSum , ");
            strSql.Append(" @DayOutStationOrderCount , ");
            strSql.Append(" @DayOutStationNeedInSum , ");
            strSql.Append(" @DayOutStationNeedOutSum , ");
            strSql.Append(" @ResortRate , ");
            strSql.Append(" @StationID , ");
            strSql.Append(" @Sources , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @DailyTime , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @FinanceStatus , ");
            strSql.Append(" @RealInCome , ");
            strSql.Append(" @POSChecked , ");
            strSql.Append(" @Remark , ");
            strSql.Append(" @DayOutOrderSum,  ");
            strSql.Append(" @ExchangeOrderCount, ");
            strSql.Append(" @ExchangeOrderSum,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                            new SqlParameter(string.Format("@{0}", "DayReceiveOrderCount"),
                                                             model.DayReceiveOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayReceiveNeedInSum"),
                                                             model.DayReceiveNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayReceiveNeedOutSum"),
                                                             model.DayReceiveNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayReceiveGoodsCount"),
                                                             model.DayReceiveGoodsCount),
                                            new SqlParameter(string.Format("@{0}", "DayOutOrderCount"),
                                                             model.DayOutOrderCount),
                                            new SqlParameter(string.Format("@{0}", "PreDayResortCount"),
                                                             model.PreDayResortCount),
                                            new SqlParameter(string.Format("@{0}", "PreDayResortNeedInSum"),
                                                             model.PreDayResortNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "PreDayResortNeedOutSum"),
                                                             model.PreDayResortNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayTransferOrderCount"),
                                                             model.DayTransferOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayInStationNeedInSum"),
                                                             model.DayInStationNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayInStationNeedOutSum"),
                                                             model.DayInStationNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayNeedDeliverOrderCount"),
                                                             model.DayNeedDeliverOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayNeedDeliverInSum"),
                                                             model.DayNeedDeliverInSum),
                                            new SqlParameter(string.Format("@{0}", "DayNeedDeliverOutSum"),
                                                             model.DayNeedDeliverOutSum),
                                            new SqlParameter(string.Format("@{0}", "CashSuccOrderCount"),
                                                             model.CashSuccOrderCount),
                                            new SqlParameter(string.Format("@{0}", "CashRealInSum"), model.CashRealInSum)
                                            ,
                                            new SqlParameter(string.Format("@{0}", "CashRealOutSum"),
                                                             model.CashRealOutSum),
                                            new SqlParameter(string.Format("@{0}", "PosSuccOrderCount"),
                                                             model.PosSuccOrderCount),
                                            new SqlParameter(string.Format("@{0}", "PosRealInSum"), model.PosRealInSum),
                                            new SqlParameter(string.Format("@{0}", "DeliverSuccRate"),
                                                             model.DeliverSuccRate),
                                            new SqlParameter(string.Format("@{0}", "RejectOrderCount"),
                                                             model.RejectOrderCount),
                                            new SqlParameter(string.Format("@{0}", "AllRejectNeedInSum"),
                                                             model.AllRejectNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "AllRejectNeedOutSum"),
                                                             model.AllRejectNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayResortCount"),
                                                             model.DayResortCount),
                                            new SqlParameter(string.Format("@{0}", "DayResortNeedInSum"),
                                                             model.DayResortNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayResortNeedOutSum"),
                                                             model.DayResortNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayOutStationOrderCount"),
                                                             model.DayOutStationOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayOutStationNeedInSum"),
                                                             model.DayOutStationNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayOutStationNeedOutSum"),
                                                             model.DayOutStationNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "ResortRate"), model.ResortRate),
                                            new SqlParameter(string.Format("@{0}", "StationID"), model.StationID),
                                            new SqlParameter(string.Format("@{0}", "Sources"), model.Sources),
                                            new SqlParameter(string.Format("@{0}", "MerchantID"), model.MerchantID),
                                            new SqlParameter(string.Format("@{0}", "DailyTime"), model.DailyTime),
                                            new SqlParameter(string.Format("@{0}", "CreateTime"), model.CreateTime),
                                            new SqlParameter(string.Format("@{0}", "UpdateTime"), model.UpdateTime),
                                            new SqlParameter(string.Format("@{0}", "FinanceStatus"), model.FinanceStatus)
                                            ,
                                            new SqlParameter(string.Format("@{0}", "RealInCome"), model.RealInCome),
                                            new SqlParameter(string.Format("@{0}", "POSChecked"), model.POSChecked),
                                            new SqlParameter(string.Format("@{0}", "Remark"), model.Remark),
                                            new SqlParameter(string.Format("@{0}", "DayOutOrderSum"),model.DayOutOrderSum),
                                            new SqlParameter(string.Format("@{0}", "ExchangeOrderCount"),model.ExchangeOrderCount),
                                            new SqlParameter(string.Format("@{0}", "ExchangeOrderSum"),model.ExchangeOrderSum),
                                            new SqlParameter(string.Format("@{0}", "IsChange"),true)
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
        public int AddV2(FMS_StationDailyFinanceSum model)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into FMS_StationDailyFinanceSum(");
            strSql.Append(" DayReceiveOrderCount , ");
            strSql.Append(" DayReceiveNeedInSum , ");
            strSql.Append(" DayReceiveNeedOutSum , ");
            strSql.Append(" DayReceiveGoodsCount , ");
            strSql.Append(" DayOutOrderCount , ");
            strSql.Append(" PreDayResortCount , ");
            strSql.Append(" PreDayResortNeedInSum , ");
            strSql.Append(" PreDayResortNeedOutSum , ");
            strSql.Append(" DayTransferOrderCount , ");
            strSql.Append(" DayInStationNeedInSum , ");
            strSql.Append(" DayInStationNeedOutSum , ");
            strSql.Append(" DayNeedDeliverOrderCount , ");
            strSql.Append(" DayNeedDeliverInSum , ");
            strSql.Append(" DayNeedDeliverOutSum , ");
            strSql.Append(" CashSuccOrderCount , ");
            strSql.Append(" CashRealInSum , ");
            strSql.Append(" CashRealOutSum , ");
            strSql.Append(" PosSuccOrderCount , ");
            strSql.Append(" PosRealInSum , ");
            strSql.Append(" DeliverSuccRate , ");
            strSql.Append(" RejectOrderCount , ");
            strSql.Append(" AllRejectNeedInSum , ");
            strSql.Append(" AllRejectNeedOutSum , ");
            strSql.Append(" DayResortCount , ");
            strSql.Append(" DayResortNeedInSum , ");
            strSql.Append(" DayResortNeedOutSum , ");
            strSql.Append(" DayOutStationOrderCount , ");
            strSql.Append(" DayOutStationNeedInSum , ");
            strSql.Append(" DayOutStationNeedOutSum , ");
            strSql.Append(" ResortRate , ");
            strSql.Append(" StationID , ");
            strSql.Append(" Sources , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" DailyTime , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" FinanceStatus , ");
            strSql.Append(" RealInCome , ");
            strSql.Append(" POSChecked , ");
            strSql.Append(" Remark , ");
            strSql.Append(" DayOutOrderSum, ");
            strSql.Append(" ExchangeOrderCount, ");
            strSql.Append(" ExchangeOrderSum,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" @DayReceiveOrderCount , ");
            strSql.Append(" @DayReceiveNeedInSum , ");
            strSql.Append(" @DayReceiveNeedOutSum , ");
            strSql.Append(" @DayReceiveGoodsCount , ");
            strSql.Append(" @DayOutOrderCount , ");
            strSql.Append(" @PreDayResortCount , ");
            strSql.Append(" @PreDayResortNeedInSum , ");
            strSql.Append(" @PreDayResortNeedOutSum , ");
            strSql.Append(" @DayTransferOrderCount , ");
            strSql.Append(" @DayInStationNeedInSum , ");
            strSql.Append(" @DayInStationNeedOutSum , ");
            strSql.Append(" @DayNeedDeliverOrderCount , ");
            strSql.Append(" @DayNeedDeliverInSum , ");
            strSql.Append(" @DayNeedDeliverOutSum , ");
            strSql.Append(" @CashSuccOrderCount , ");
            strSql.Append(" @CashRealInSum , ");
            strSql.Append(" @CashRealOutSum , ");
            strSql.Append(" @PosSuccOrderCount , ");
            strSql.Append(" @PosRealInSum , ");
            strSql.Append(" @DeliverSuccRate , ");
            strSql.Append(" @RejectOrderCount , ");
            strSql.Append(" @AllRejectNeedInSum , ");
            strSql.Append(" @AllRejectNeedOutSum , ");
            strSql.Append(" @DayResortCount , ");
            strSql.Append(" @DayResortNeedInSum , ");
            strSql.Append(" @DayResortNeedOutSum , ");
            strSql.Append(" @DayOutStationOrderCount , ");
            strSql.Append(" @DayOutStationNeedInSum , ");
            strSql.Append(" @DayOutStationNeedOutSum , ");
            strSql.Append(" @ResortRate , ");
            strSql.Append(" @StationID , ");
            strSql.Append(" @Sources , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @DailyTime , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @FinanceStatus , ");
            strSql.Append(" @RealInCome , ");
            strSql.Append(" @POSChecked , ");
            strSql.Append(" @Remark , ");
            strSql.Append(" @DayOutOrderSum,  ");
            strSql.Append(" @ExchangeOrderCount, ");
            strSql.Append(" @ExchangeOrderSum,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                                            new SqlParameter(string.Format("@{0}", "DayReceiveOrderCount"),
                                                             model.DayReceiveOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayReceiveNeedInSum"),
                                                             model.DayReceiveNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayReceiveNeedOutSum"),
                                                             model.DayReceiveNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayReceiveGoodsCount"),
                                                             model.DayReceiveGoodsCount),
                                            new SqlParameter(string.Format("@{0}", "DayOutOrderCount"),
                                                             model.DayOutOrderCount),
                                            new SqlParameter(string.Format("@{0}", "PreDayResortCount"),
                                                             model.PreDayResortCount),
                                            new SqlParameter(string.Format("@{0}", "PreDayResortNeedInSum"),
                                                             model.PreDayResortNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "PreDayResortNeedOutSum"),
                                                             model.PreDayResortNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayTransferOrderCount"),
                                                             model.DayTransferOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayInStationNeedInSum"),
                                                             model.DayInStationNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayInStationNeedOutSum"),
                                                             model.DayInStationNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayNeedDeliverOrderCount"),
                                                             model.DayNeedDeliverOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayNeedDeliverInSum"),
                                                             model.DayNeedDeliverInSum),
                                            new SqlParameter(string.Format("@{0}", "DayNeedDeliverOutSum"),
                                                             model.DayNeedDeliverOutSum),
                                            new SqlParameter(string.Format("@{0}", "CashSuccOrderCount"),
                                                             model.CashSuccOrderCount),
                                            new SqlParameter(string.Format("@{0}", "CashRealInSum"), model.CashRealInSum)
                                            ,
                                            new SqlParameter(string.Format("@{0}", "CashRealOutSum"),
                                                             model.CashRealOutSum),
                                            new SqlParameter(string.Format("@{0}", "PosSuccOrderCount"),
                                                             model.PosSuccOrderCount),
                                            new SqlParameter(string.Format("@{0}", "PosRealInSum"), model.PosRealInSum),
                                            new SqlParameter(string.Format("@{0}", "DeliverSuccRate"),
                                                             model.DeliverSuccRate),
                                            new SqlParameter(string.Format("@{0}", "RejectOrderCount"),
                                                             model.RejectOrderCount),
                                            new SqlParameter(string.Format("@{0}", "AllRejectNeedInSum"),
                                                             model.AllRejectNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "AllRejectNeedOutSum"),
                                                             model.AllRejectNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayResortCount"),
                                                             model.DayResortCount),
                                            new SqlParameter(string.Format("@{0}", "DayResortNeedInSum"),
                                                             model.DayResortNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayResortNeedOutSum"),
                                                             model.DayResortNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "DayOutStationOrderCount"),
                                                             model.DayOutStationOrderCount),
                                            new SqlParameter(string.Format("@{0}", "DayOutStationNeedInSum"),
                                                             model.DayOutStationNeedInSum),
                                            new SqlParameter(string.Format("@{0}", "DayOutStationNeedOutSum"),
                                                             model.DayOutStationNeedOutSum),
                                            new SqlParameter(string.Format("@{0}", "ResortRate"), model.ResortRate),
                                            new SqlParameter(string.Format("@{0}", "StationID"), model.StationID),
                                            new SqlParameter(string.Format("@{0}", "Sources"), model.Sources),
                                            new SqlParameter(string.Format("@{0}", "MerchantID"), model.MerchantID),
                                            new SqlParameter(string.Format("@{0}", "DailyTime"), model.DailyTime),
                                            new SqlParameter(string.Format("@{0}", "CreateTime"), model.CreateTime),
                                            new SqlParameter(string.Format("@{0}", "UpdateTime"), model.UpdateTime),
                                            new SqlParameter(string.Format("@{0}", "FinanceStatus"), model.FinanceStatus)
                                            ,
                                            new SqlParameter(string.Format("@{0}", "RealInCome"), model.RealInCome),
                                            new SqlParameter(string.Format("@{0}", "POSChecked"), model.POSChecked),
                                            new SqlParameter(string.Format("@{0}", "Remark"), model.Remark),
                                            new SqlParameter(string.Format("@{0}", "DayOutOrderSum"),model.DayOutOrderSum),
                                            new SqlParameter(string.Format("@{0}", "ExchangeOrderCount"),model.ExchangeOrderCount),
                                            new SqlParameter(string.Format("@{0}", "ExchangeOrderSum"),model.ExchangeOrderSum),
                                            new SqlParameter(string.Format("@{0}", "IsChange"),true)
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
        /// 根据商家,站点和时间判断汇总是否存在
        /// </summary>
        public bool ExistsV2(FMS_StationDailyFinanceSum model)
        {
            string dateDaily = Convert.ToDateTime(model.DailyTime).ToString("yyyy-MM-dd");

            string whereStr = "";
            if (model.MerchantID != null)
            {
                whereStr = " AND MerchantID= " + model.MerchantID;
            }
            string sql = string.Format
                (@"SELECT COUNT(0)
            FROM   FMS_StationDailyFinanceSum(NOLOCK)
            WHERE  StationID = {0}
                   AND DailyTime = '{1}'
                   AND Sources={2} {3}",
                 model.StationID, dateDaily, model.Sources, whereStr);

            return Convert.ToInt32(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql)) > 0;
        }

        /// <summary>
        /// 根据商家,站点和时间判断汇总是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists(FMS_StationDailyFinanceSum model)
        {
            string dateDaily = Convert.ToDateTime(model.DailyTime).ToString("yyyy-MM-dd");
            string whereStr = "";
            if (model.MerchantID != null)
            {
                whereStr = " AND MerchantID= " + model.MerchantID;
            }
            string sql = string.Format
                (@"SELECT COUNT(0)
            FROM   LMS_RFD.dbo.FMS_StationDailyFinanceSum(NOLOCK)
            WHERE  StationID = {0}
                   AND DailyTime = '{1}'
                   AND Sources={2} {3}",
                 model.StationID, dateDaily, model.Sources, whereStr);
            return Convert.ToInt32(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql)) > 0;
        }

        /// <summary>
        /// 根据条件删除报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public bool Delete(SearchModel searchInfo)
        {
            string where = " where 1=1 ";
            string dailyDate = (searchInfo.dtDailyDate).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(searchInfo.StationId))
            {
                where += " And StationID= " + searchInfo.StationId;
            }
            if (!string.IsNullOrEmpty(searchInfo.Sources))
            {
                where += " And Sources= " + searchInfo.Sources;
            }
            if (searchInfo.Sources == "2")
            {
                if (!string.IsNullOrEmpty(searchInfo.MerchantId))
                {
                    where += " And MerchantID= " + searchInfo.MerchantId;
                }
            }
            if (!string.IsNullOrEmpty(dailyDate))
            {
                where += " And DailyTime = '" + dailyDate + "'";
            }
            string sql = "Delete LMS_RFD.dbo.FMS_StationDailyFinanceDetails  " + where;
            sql += ";Delete LMS_RFD.dbo.FMS_StationDailyFinanceSum  " + where;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }

        /// <summary>
        /// 根据条件删除报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public bool DeleteV2(SearchModel searchInfo)
        {
            string where = " where 1=1 ";
            string dailyDate = (searchInfo.dtDailyDate).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(searchInfo.StationId))
            {
                where += " And StationID= " + searchInfo.StationId;
            }
            if (!string.IsNullOrEmpty(searchInfo.Sources))
            {
                where += " And Sources= " + searchInfo.Sources;
            }
            if (searchInfo.Sources == "2")
            {
                if (!string.IsNullOrEmpty(searchInfo.MerchantId))
                {
                    where += " And MerchantID= " + searchInfo.MerchantId;
                }
            }

            if (!string.IsNullOrEmpty(dailyDate))
            {
                where += " And DailyTime = '" + dailyDate + "'";
            }

            string sql = "Delete FMS_StationDailyFinanceDetails  " + where;

            sql += ";Delete FMS_StationDailyFinanceSum  " + where;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }

        public DataTable GetWaybillByNo(string waybillNo)
        {
            string sql = string.Format(@"SELECT w.WaybillNO,
                           w.Sources,
                           w.DeliverStationID,
                           w.merchantId,
                           wbs.CreatTime
                    FROM   LMS_RFD.dbo.Waybill w(NOLOCK)
                           JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK)
                                ON  wsi.WaybillNO = w.WaybillNO
                           JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK)
                                ON  wbs.WaybillBackStationID = wsi.BackStationInofID
                    WHERE  w.WaybillNO = {0}", waybillNo);

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }
    }
}