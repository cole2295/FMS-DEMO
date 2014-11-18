using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using RFD.FMS.Model.UnionPay;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class FMS_StationDailyFinanceSumDao : OracleDao
    {
        private const string TableName = @"FMS_StationDailyFinanceSum";

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(FMS_StationDailyFinanceSum model)
        {
            if (model.ID <= 0)
            {
                model.ID = GetIdNew("FMS_STATIONDAILYFINANCESUM");
            }

            var strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" FEEID, ");
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
            strSql.Append(" :FEEID, ");
            strSql.Append(" :DayReceiveOrderCount , ");
            strSql.Append(" :DayReceiveNeedInSum , ");
            strSql.Append(" :DayReceiveNeedOutSum , ");
            strSql.Append(" :DayReceiveGoodsCount , ");
            strSql.Append(" :DayOutOrderCount , ");
            strSql.Append(" :PreDayResortCount , ");
            strSql.Append(" :PreDayResortNeedInSum , ");
            strSql.Append(" :PreDayResortNeedOutSum , ");
            strSql.Append(" :DayTransferOrderCount , ");
            strSql.Append(" :DayInStationNeedInSum , ");
            strSql.Append(" :DayInStationNeedOutSum , ");
            strSql.Append(" :DayNeedDeliverOrderCount , ");
            strSql.Append(" :DayNeedDeliverInSum , ");
            strSql.Append(" :DayNeedDeliverOutSum , ");
            strSql.Append(" :CashSuccOrderCount , ");
            strSql.Append(" :CashRealInSum , ");
            strSql.Append(" :CashRealOutSum , ");
            strSql.Append(" :PosSuccOrderCount , ");
            strSql.Append(" :PosRealInSum , ");
            strSql.Append(" :DeliverSuccRate , ");
            strSql.Append(" :RejectOrderCount , ");
            strSql.Append(" :AllRejectNeedInSum , ");
            strSql.Append(" :AllRejectNeedOutSum , ");
            strSql.Append(" :DayResortCount , ");
            strSql.Append(" :DayResortNeedInSum , ");
            strSql.Append(" :DayResortNeedOutSum , ");
            strSql.Append(" :DayOutStationOrderCount , ");
            strSql.Append(" :DayOutStationNeedInSum , ");
            strSql.Append(" :DayOutStationNeedOutSum , ");
            strSql.Append(" :ResortRate , ");
            strSql.Append(" :StationID , ");
            strSql.Append(" :Sources , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :DailyTime , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateTime , ");
            strSql.Append(" :FinanceStatus , ");
            strSql.Append(" :RealInCome , ");
            strSql.Append(" :POSChecked , ");
            strSql.Append(" :Remark , ");
            strSql.Append(" :DayOutOrderSum,  ");
            strSql.Append(" :ExchangeOrderCount, ");
            strSql.Append(" :ExchangeOrderSum,  ");
            strSql.Append(" :IsChange  ");
            strSql.Append(") ");
            OracleParameter[] parameters = {
                                            new OracleParameter(string.Format(":{0}", "FEEID"),
                                                             model.ID),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveOrderCount"),
                                                             model.DayReceiveOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveNeedInSum"),
                                                             model.DayReceiveNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveNeedOutSum"),
                                                             model.DayReceiveNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveGoodsCount"),
                                                             model.DayReceiveGoodsCount),
                                            new OracleParameter(string.Format(":{0}", "DayOutOrderCount"),
                                                             model.DayOutOrderCount),
                                            new OracleParameter(string.Format(":{0}", "PreDayResortCount"),
                                                             model.PreDayResortCount),
                                            new OracleParameter(string.Format(":{0}", "PreDayResortNeedInSum"),
                                                             model.PreDayResortNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "PreDayResortNeedOutSum"),
                                                             model.PreDayResortNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayTransferOrderCount"),
                                                             model.DayTransferOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayInStationNeedInSum"),
                                                             model.DayInStationNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayInStationNeedOutSum"),
                                                             model.DayInStationNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayNeedDeliverOrderCount"),
                                                             model.DayNeedDeliverOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayNeedDeliverInSum"),
                                                             model.DayNeedDeliverInSum),
                                            new OracleParameter(string.Format(":{0}", "DayNeedDeliverOutSum"),
                                                             model.DayNeedDeliverOutSum),
                                            new OracleParameter(string.Format(":{0}", "CashSuccOrderCount"),
                                                             model.CashSuccOrderCount),
                                            new OracleParameter(string.Format(":{0}", "CashRealInSum"), model.CashRealInSum)
                                            ,
                                            new OracleParameter(string.Format(":{0}", "CashRealOutSum"),
                                                             model.CashRealOutSum),
                                            new OracleParameter(string.Format(":{0}", "PosSuccOrderCount"),
                                                             model.PosSuccOrderCount),
                                            new OracleParameter(string.Format(":{0}", "PosRealInSum"), model.PosRealInSum),
                                            new OracleParameter(string.Format(":{0}", "DeliverSuccRate"),
                                                             model.DeliverSuccRate),
                                            new OracleParameter(string.Format(":{0}", "RejectOrderCount"),
                                                             model.RejectOrderCount),
                                            new OracleParameter(string.Format(":{0}", "AllRejectNeedInSum"),
                                                             model.AllRejectNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "AllRejectNeedOutSum"),
                                                             model.AllRejectNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayResortCount"),
                                                             model.DayResortCount),
                                            new OracleParameter(string.Format(":{0}", "DayResortNeedInSum"),
                                                             model.DayResortNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayResortNeedOutSum"),
                                                             model.DayResortNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayOutStationOrderCount"),
                                                             model.DayOutStationOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayOutStationNeedInSum"),
                                                             model.DayOutStationNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayOutStationNeedOutSum"),
                                                             model.DayOutStationNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "ResortRate"), model.ResortRate),
                                            new OracleParameter(string.Format(":{0}", "StationID"), model.StationID),
                                            new OracleParameter(string.Format(":{0}", "Sources"), model.Sources),
                                            new OracleParameter(string.Format(":{0}", "MerchantID"), model.MerchantID),
                                            new OracleParameter(string.Format(":{0}", "DailyTime"), model.DailyTime),
                                            new OracleParameter(string.Format(":{0}", "CreateTime"), model.CreateTime),
                                            new OracleParameter(string.Format(":{0}", "UpdateTime"), model.UpdateTime),
                                            new OracleParameter(string.Format(":{0}", "FinanceStatus"), model.FinanceStatus)
                                            ,
                                            new OracleParameter(string.Format(":{0}", "RealInCome"), model.RealInCome),
                                            new OracleParameter(string.Format(":{0}", "POSChecked"), model.POSChecked),
                                            new OracleParameter(string.Format(":{0}", "Remark"), model.Remark),
                                            new OracleParameter(string.Format(":{0}", "DayOutOrderSum"),model.DayOutOrderSum),
                                            new OracleParameter(string.Format(":{0}", "ExchangeOrderCount"),model.ExchangeOrderCount),
                                            new OracleParameter(string.Format(":{0}", "ExchangeOrderSum"),model.ExchangeOrderSum),
                                            new OracleParameter(string.Format(":{0}", "IsChange"),true)
                                        };

            int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败");

            return model.ID;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddV2(FMS_StationDailyFinanceSum model)
        {
            if (model.ID <= 0)
            {
                model.ID = GetIdNew("FMS_STATIONDAILYFINANCESUM");
            }

            var strSql = new StringBuilder();
            strSql.Append("insert into FMS_StationDailyFinanceSum(");
            strSql.Append(" FEEID, ");
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
            strSql.Append(" :FEEID, ");
            strSql.Append(" :DayReceiveOrderCount , ");
            strSql.Append(" :DayReceiveNeedInSum , ");
            strSql.Append(" :DayReceiveNeedOutSum , ");
            strSql.Append(" :DayReceiveGoodsCount , ");
            strSql.Append(" :DayOutOrderCount , ");
            strSql.Append(" :PreDayResortCount , ");
            strSql.Append(" :PreDayResortNeedInSum , ");
            strSql.Append(" :PreDayResortNeedOutSum , ");
            strSql.Append(" :DayTransferOrderCount , ");
            strSql.Append(" :DayInStationNeedInSum , ");
            strSql.Append(" :DayInStationNeedOutSum , ");
            strSql.Append(" :DayNeedDeliverOrderCount , ");
            strSql.Append(" :DayNeedDeliverInSum , ");
            strSql.Append(" :DayNeedDeliverOutSum , ");
            strSql.Append(" :CashSuccOrderCount , ");
            strSql.Append(" :CashRealInSum , ");
            strSql.Append(" :CashRealOutSum , ");
            strSql.Append(" :PosSuccOrderCount , ");
            strSql.Append(" :PosRealInSum , ");
            strSql.Append(" :DeliverSuccRate , ");
            strSql.Append(" :RejectOrderCount , ");
            strSql.Append(" :AllRejectNeedInSum , ");
            strSql.Append(" :AllRejectNeedOutSum , ");
            strSql.Append(" :DayResortCount , ");
            strSql.Append(" :DayResortNeedInSum , ");
            strSql.Append(" :DayResortNeedOutSum , ");
            strSql.Append(" :DayOutStationOrderCount , ");
            strSql.Append(" :DayOutStationNeedInSum , ");
            strSql.Append(" :DayOutStationNeedOutSum , ");
            strSql.Append(" :ResortRate , ");
            strSql.Append(" :StationID , ");
            strSql.Append(" :Sources , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :DailyTime , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateTime , ");
            strSql.Append(" :FinanceStatus , ");
            strSql.Append(" :RealInCome , ");
            strSql.Append(" :POSChecked , ");
            strSql.Append(" :Remark , ");
            strSql.Append(" :DayOutOrderSum,  ");
            strSql.Append(" :ExchangeOrderCount, ");
            strSql.Append(" :ExchangeOrderSum,  ");
            strSql.Append(" :IsChange  ");
            strSql.Append(") ");
            OracleParameter[] parameters = {
                                            new OracleParameter(string.Format(":{0}", "FEEID"),
                                                             model.ID),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveOrderCount"),
                                                             model.DayReceiveOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveNeedInSum"),
                                                             model.DayReceiveNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveNeedOutSum"),
                                                             model.DayReceiveNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayReceiveGoodsCount"),
                                                             model.DayReceiveGoodsCount),
                                            new OracleParameter(string.Format(":{0}", "DayOutOrderCount"),
                                                             model.DayOutOrderCount),
                                            new OracleParameter(string.Format(":{0}", "PreDayResortCount"),
                                                             model.PreDayResortCount),
                                            new OracleParameter(string.Format(":{0}", "PreDayResortNeedInSum"),
                                                             model.PreDayResortNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "PreDayResortNeedOutSum"),
                                                             model.PreDayResortNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayTransferOrderCount"),
                                                             model.DayTransferOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayInStationNeedInSum"),
                                                             model.DayInStationNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayInStationNeedOutSum"),
                                                             model.DayInStationNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayNeedDeliverOrderCount"),
                                                             model.DayNeedDeliverOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayNeedDeliverInSum"),
                                                             model.DayNeedDeliverInSum),
                                            new OracleParameter(string.Format(":{0}", "DayNeedDeliverOutSum"),
                                                             model.DayNeedDeliverOutSum),
                                            new OracleParameter(string.Format(":{0}", "CashSuccOrderCount"),
                                                             model.CashSuccOrderCount),
                                            new OracleParameter(string.Format(":{0}", "CashRealInSum"), model.CashRealInSum)
                                            ,
                                            new OracleParameter(string.Format(":{0}", "CashRealOutSum"),
                                                             model.CashRealOutSum),
                                            new OracleParameter(string.Format(":{0}", "PosSuccOrderCount"),
                                                             model.PosSuccOrderCount),
                                            new OracleParameter(string.Format(":{0}", "PosRealInSum"), model.PosRealInSum),
                                            new OracleParameter(string.Format(":{0}", "DeliverSuccRate"),
                                                             model.DeliverSuccRate),
                                            new OracleParameter(string.Format(":{0}", "RejectOrderCount"),
                                                             model.RejectOrderCount),
                                            new OracleParameter(string.Format(":{0}", "AllRejectNeedInSum"),
                                                             model.AllRejectNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "AllRejectNeedOutSum"),
                                                             model.AllRejectNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayResortCount"),
                                                             model.DayResortCount),
                                            new OracleParameter(string.Format(":{0}", "DayResortNeedInSum"),
                                                             model.DayResortNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayResortNeedOutSum"),
                                                             model.DayResortNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "DayOutStationOrderCount"),
                                                             model.DayOutStationOrderCount),
                                            new OracleParameter(string.Format(":{0}", "DayOutStationNeedInSum"),
                                                             model.DayOutStationNeedInSum),
                                            new OracleParameter(string.Format(":{0}", "DayOutStationNeedOutSum"),
                                                             model.DayOutStationNeedOutSum),
                                            new OracleParameter(string.Format(":{0}", "ResortRate"), model.ResortRate),
                                            new OracleParameter(string.Format(":{0}", "StationID"), model.StationID),
                                            new OracleParameter(string.Format(":{0}", "Sources"), model.Sources),
                                            new OracleParameter(string.Format(":{0}", "MerchantID"), model.MerchantID),
                                            new OracleParameter(string.Format(":{0}", "DailyTime"), model.DailyTime),
                                            new OracleParameter(string.Format(":{0}", "CreateTime"), model.CreateTime),
                                            new OracleParameter(string.Format(":{0}", "UpdateTime"), model.UpdateTime),
                                            new OracleParameter(string.Format(":{0}", "FinanceStatus"), model.FinanceStatus)
                                            ,
                                            new OracleParameter(string.Format(":{0}", "RealInCome"), model.RealInCome),
                                            new OracleParameter(string.Format(":{0}", "POSChecked"), model.POSChecked),
                                            new OracleParameter(string.Format(":{0}", "Remark"), model.Remark),
                                            new OracleParameter(string.Format(":{0}", "DayOutOrderSum"),model.DayOutOrderSum),
                                            new OracleParameter(string.Format(":{0}", "ExchangeOrderCount"),model.ExchangeOrderCount),
                                            new OracleParameter(string.Format(":{0}", "ExchangeOrderSum"),model.ExchangeOrderSum),
                                            new OracleParameter(string.Format(":{0}", "IsChange"),true)
                                        };

            int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败");

            return model.ID;
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
            FROM   FMS_StationDailyFinanceSum
            WHERE  StationID = {0}
                   AND DailyTime = '{1}'
                   AND Sources={2} {3}",
                 model.StationID, dateDaily, model.Sources, whereStr);

            return Convert.ToInt32(OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql)) > 0;
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
            FROM   FMS_StationDailyFinanceSum
            WHERE  StationID = {0}
                   AND DailyTime = '{1}'
                   AND Sources={2} {3}",
                 model.StationID, dateDaily, model.Sources, whereStr);
            return Convert.ToInt32(OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql)) > 0;
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
            string sql = "Delete FMS_StationDailyFinanceDetails  " + where;
            sql += ";Delete FMS_StationDailyFinanceSum  " + where;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
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

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }

        public DataTable GetWaybillByNo(string waybillNo)
        {
            string sql = string.Format(@"SELECT w.WaybillNO,
                           w.Sources,
                           w.DeliverStationID,
                           w.merchantId,
                           wbs.CreatTime
                    FROM   Waybill w
                           JOIN WaybillSignInfo wsi
                                ON  wsi.WaybillNO = w.WaybillNO
                           JOIN WaybillBackStation wbs
                                ON  wbs.WaybillBackStationID = wsi.BackStationInofID
                    WHERE  w.WaybillNO = {0}", waybillNo);

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }
    }
}