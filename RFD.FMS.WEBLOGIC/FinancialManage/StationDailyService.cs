using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Model.UnionPay;
using RFD.FMS.Service.FinancialManage;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.MODEL.Enumeration;
using System.IO;
using System.Configuration;
using System.Threading;
using RFD.FMS.Util;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class StationDailyService : IStationDailyService
    {
        private IFMS_StationDailyFinanceDetailsDao _fMS_StationDailyFinanceDetailsDao;
        private IFMS_StationDailyFinanceSumDao _fMS_StationDailyFinanceSumDao;
        private IDeliverFeeStatDao deliverFeeStatService = ServiceLocator.GetService<IDeliverFeeStatDao>();

        /// <summary>
        /// 添加报表明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddDetailsV2(FMS_StationDailyFinanceDetails model)
        {
            if (!_fMS_StationDailyFinanceDetailsDao.ExistsV2(model))
            {
                return _fMS_StationDailyFinanceDetailsDao.AddV2(model) > 0;
            }

            return false;
        }

        /// <summary>
        /// 添加报表明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddDetails(FMS_StationDailyFinanceDetails model)
        {
            if (!_fMS_StationDailyFinanceDetailsDao.Exists(model))
            {
                return _fMS_StationDailyFinanceDetailsDao.Add(model) > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 添加报表汇总
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddSumV2(FMS_StationDailyFinanceSum model)
        {
            if (!_fMS_StationDailyFinanceSumDao.ExistsV2(model))
            {
                return _fMS_StationDailyFinanceSumDao.AddV2(model) > 0;
            }

            return false;
        }

        /// <summary>
        /// 添加报表汇总
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddSum(FMS_StationDailyFinanceSum model)
        {
            if (!_fMS_StationDailyFinanceSumDao.Exists(model))
            {
                return _fMS_StationDailyFinanceSumDao.Add(model) > 0;
            }
            else
            {
                return false;
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
            return _fMS_StationDailyFinanceDetailsDao.GetOrderDetil(dtDailyDate, strStaionID, strSources, strMerchantID);
        }

        public DataTable GetOrderDetil(SearchModel searchInfo)
        {
            return _fMS_StationDailyFinanceDetailsDao.GetOrderDetil(searchInfo.dtDailyDate, searchInfo.StationId, searchInfo.Sources,
                                            searchInfo.MerchantId);
        }

        public IList<WaybillDistribution> ConvertToWaybillDistributionModelList(DataTable dataTable)
        {
            var waybillDistributions = new List<WaybillDistribution>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];
                    WaybillDistribution model = GetModel(row);
                    waybillDistributions.Add(model);
                }
            }
            return waybillDistributions;
        }

        public WaybillDistribution GetModel(DataRow dataRow)
        {
            var model = new WaybillDistribution();
            if (dataRow["IntoTime"].ToString() != "")
            {
                model.IntoTime = DateTime.Parse(dataRow["IntoTime"].ToString());
            }
            if (dataRow["WaybillNO"].ToString() != "")
            {
                model.WaybillNO = long.Parse(dataRow["WaybillNO"].ToString());
            }
            model.WaybillType = dataRow["WaybillType"].ToString();
            if (dataRow["NeedAmount"].ToString() != "")
            {
                model.NeedAmount = decimal.Parse(dataRow["NeedAmount"].ToString());
            }
            if (dataRow["NeedBackAmount"].ToString() != "")
            {
                model.NeedBackAmount = decimal.Parse(dataRow["NeedBackAmount"].ToString());
            }
            if (dataRow["FactAmount"].ToString() != "")
            {
                model.FactAmount = decimal.Parse(dataRow["FactAmount"].ToString());
            }
            if (dataRow["FactBackAmount"].ToString() != "")
            {
                model.FactBackAmount = decimal.Parse(dataRow["FactBackAmount"].ToString());
            }
            model.SignStatus = dataRow["SignStatus"].ToString();
            model.AcceptType = dataRow["AcceptType"].ToString();
            model.POSCode = dataRow["POSCode"].ToString();
            if (dataRow["SignTime"].ToString() != "")
            {
                model.SignTime = DateTime.Parse(dataRow["SignTime"].ToString());
            }
            model.EmployeeName = dataRow["EmployeeName"].ToString();
            model.DeductNotes = dataRow["DeductNotes"].ToString();
            return model;
        }

        /// <summary>
        /// 查询并添加报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        public void AddStationDailyDataV2(SearchModel searchInfo)
        {
            DataTable dt = GetOrderDetil(searchInfo);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    InsertDailyDetailV2(dr, searchInfo);
                }

                InsertDailySumV2(searchInfo, dt);
            }
        }

        /// <summary>
        /// 查询并添加报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        public void AddStationDailyData(SearchModel searchInfo)
        {
            DataTable dt = GetOrderDetil(searchInfo);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    InsertDailyDetail(dr, searchInfo);
                }

                InsertDailySum(searchInfo, dt);
            }
        }

        /// <summary>
        /// 添加报表明细
        /// </summary>
        /// <param name="dr"></param>
        private void InsertDailyDetailV2(DataRow dr, SearchModel searchInfo)
        {
            var model = new FMS_StationDailyFinanceDetails();

            try
            {
                model.WaybillNO = long.Parse(dr["WaybillNO"].ToString());
                if (!string.IsNullOrEmpty(dr["IntoTime"].ToString()))
                {
                    model.EnterTime = Convert.ToDateTime(dr["IntoTime"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["NeedAmount"].ToString()))
                {
                    model.NeedPrice = decimal.Parse(dr["NeedAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["NeedBackAmount"].ToString()))
                {
                    model.NeedReturnPrice = decimal.Parse(dr["NeedBackAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["FactAmount"].ToString()))
                {
                    model.PriceDiff = decimal.Parse(dr["FactAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["FactBackAmount"].ToString()))
                {
                    model.PriceReturnCash = decimal.Parse(dr["FactBackAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["WayBillInfoWeight"].ToString()))
                {
                    model.Weight = decimal.Parse(dr["WayBillInfoWeight"].ToString());
                }
                model.WaybillType = dr["WaybillType"].ToString();
                model.AcceptType = dr["AcceptType"].ToString();
                model.RejectReason = dr["Remark"].ToString();

                if (!string.IsNullOrEmpty(dr["SignTime"].ToString()))
                {
                    model.PostTime = Convert.ToDateTime(dr["SignTime"].ToString());
                }
                model.DeliverManName = dr["EmployeeName"].ToString();
                model.Status = dr["SignStatus"].ToString();
                model.CreateTime = DateTime.Now;
                model.DailyTime = searchInfo.dtDailyDate;
                model.StationID = int.Parse(searchInfo.StationId);
                model.Sources = int.Parse(dr["Sources"].ToString());

                if (int.Parse(dr["Sources"].ToString()) == (int)WaybillSourse.Other)
                {
                    if (!string.IsNullOrEmpty(dr["MerchantID"].ToString()))
                    {
                        model.MerchantID = int.Parse(dr["MerchantID"].ToString());
                    }
                    decimal ProtectedParmer = decimal.Zero;
                    model.DeliverFee = deliverFeeStatService.GetDeliverFee(model.MerchantID, model.Weight,
                                                                decimal.Parse(dr["WayBillInfoVolume"].ToString()), (WayBillStatus)Convert.ToInt32(model.Status), out ProtectedParmer);

                    if (!string.IsNullOrEmpty(dr["Protectedprice"].ToString()))
                    {
                        model.Protectedprice = decimal.Parse(dr["Protectedprice"].ToString()) * ProtectedParmer;
                    }
                }
                model.PosNum = dr["POSCode"].ToString();
                if (!string.IsNullOrEmpty(dr["DeductMoney"].ToString()))
                {
                    model.DeductMoney = decimal.Parse(dr["DeductMoney"].ToString());
                }
                model.CustomerOrder = dr["CustomerOrder"].ToString();
                if (!string.IsNullOrEmpty(dr["EmployeeID"].ToString()))
                {
                    model.DeliverManID = int.Parse(dr["EmployeeID"].ToString());
                }
                AddDetailsV2(model);
            }
            catch (Exception ex)
            {
                WriteTest(model.WaybillNO + "插入配送报表明细失败，错误信息：" + ex.Message + "---" + DateTime.Now);
            }
        }

        /// <summary>
        /// 添加报表明细
        /// </summary>
        /// <param name="dr"></param>
        private void InsertDailyDetail(DataRow dr, SearchModel searchInfo)
        {
            var model = new FMS_StationDailyFinanceDetails();
            try
            {
                model.WaybillNO = long.Parse(dr["WaybillNO"].ToString());
                if (!string.IsNullOrEmpty(dr["IntoTime"].ToString()))
                {
                    model.EnterTime = Convert.ToDateTime(dr["IntoTime"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["NeedAmount"].ToString()))
                {
                    model.NeedPrice = decimal.Parse(dr["NeedAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["NeedBackAmount"].ToString()))
                {
                    model.NeedReturnPrice = decimal.Parse(dr["NeedBackAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["FactAmount"].ToString()))
                {
                    model.PriceDiff = decimal.Parse(dr["FactAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["FactBackAmount"].ToString()))
                {
                    model.PriceReturnCash = decimal.Parse(dr["FactBackAmount"].ToString());
                }
                if (!string.IsNullOrEmpty(dr["WayBillInfoWeight"].ToString()))
                {
                    model.Weight = decimal.Parse(dr["WayBillInfoWeight"].ToString());
                }
                model.WaybillType = dr["WaybillType"].ToString();
                model.AcceptType = dr["AcceptType"].ToString();
                model.RejectReason = dr["Remark"].ToString();

                if (!string.IsNullOrEmpty(dr["SignTime"].ToString()))
                {
                    model.PostTime = Convert.ToDateTime(dr["SignTime"].ToString());
                }
                model.DeliverManName = dr["EmployeeName"].ToString();
                model.Status = dr["SignStatus"].ToString();
                model.CreateTime = DateTime.Now;
                model.DailyTime = searchInfo.dtDailyDate;
                model.StationID = int.Parse(searchInfo.StationId);
                model.Sources = int.Parse(dr["Sources"].ToString());

                if (int.Parse(dr["Sources"].ToString()) == (int)WaybillSourse.Other)
                {
                    if (!string.IsNullOrEmpty(dr["MerchantID"].ToString()))
                    {
                        model.MerchantID = int.Parse(dr["MerchantID"].ToString());
                    }
                    decimal ProtectedParmer = decimal.Zero;
                    model.DeliverFee = deliverFeeStatService.GetDeliverFee(model.MerchantID, model.Weight,
                                                                decimal.Parse(dr["WayBillInfoVolume"].ToString()), (WayBillStatus)Convert.ToInt32(model.Status), out ProtectedParmer);

                    if (!string.IsNullOrEmpty(dr["Protectedprice"].ToString()))
                    {
                        model.Protectedprice = decimal.Parse(dr["Protectedprice"].ToString()) * ProtectedParmer;
                    }
                }
                model.PosNum = dr["POSCode"].ToString();
                if (!string.IsNullOrEmpty(dr["DeductMoney"].ToString()))
                {
                    model.DeductMoney = decimal.Parse(dr["DeductMoney"].ToString());
                }
                model.CustomerOrder = dr["CustomerOrder"].ToString();
                if (!string.IsNullOrEmpty(dr["EmployeeID"].ToString()))
                {
                    model.DeliverManID = int.Parse(dr["EmployeeID"].ToString());
                }
                AddDetails(model);
            }
            catch (Exception ex)
            {
                WriteTest(model.WaybillNO + "插入配送报表明细失败，错误信息：" + ex.Message + "---" + DateTime.Now);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="tips"></param>
        public void WriteTest(string tips)
        {
            var fs = new FileStream(ConfigurationManager.AppSettings["LogFilePath"] + DateTime.Now.ToString("yyyy年MM月dd") + "logData.txt",
                                    FileMode.OpenOrCreate, FileAccess.Write);
            var mStreamWriter = new StreamWriter(fs);
            mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            mStreamWriter.WriteLine(tips);
            mStreamWriter.Flush();
            mStreamWriter.Close();
            fs.Close();
        }

        /// <summary>
        /// 添加报表汇总
        /// </summary>
        private void InsertDailySum(SearchModel searchInfo, DataTable dt)
        {
            try
            {
                var model = new FMS_StationDailyFinanceSum();
                int sourceType = Convert.ToInt32(searchInfo.Sources);
                DateTime dateTime = searchInfo.dtDailyDate;
                string stationCode = searchInfo.StationId;
                int? merchantID = null;
                if (!string.IsNullOrEmpty(searchInfo.MerchantId))
                {
                    merchantID = Convert.ToInt32(searchInfo.MerchantId);
                }
                IList<WaybillDistribution> waybillDistributions = ConvertToWaybillDistributionModelList(dt);
                model.MerchantID = merchantID;
                model.StationID = Convert.ToInt32(stationCode);
                model.Sources = sourceType;
                model.DailyTime = searchInfo.dtDailyDate;
                model.CreateTime = DateTime.Now;
                model.DayNeedDeliverInSum = AllNeedamountSum(waybillDistributions);
                model.AllRejectNeedInSum = AllRejectNeedamountSum(waybillDistributions);
                model.AllRejectNeedOutSum = AllRejectNeedBackAmountSum(waybillDistributions);
                model.CashRealInSum = CashFactAmountSum(waybillDistributions);
                model.CashRealOutSum = CashFactBackAmountSum(waybillDistributions);
                model.CashSuccOrderCount = CashSuccessCount(waybillDistributions);
                model.DeliverSuccRate = DistributionSuccessRate(waybillDistributions).ToString();
                model.PosRealInSum = PosFactAmountSum(waybillDistributions);
                model.PosSuccOrderCount = PosSuccessCount(waybillDistributions);
                model.RejectOrderCount = RejectWaybillCount(waybillDistributions);
                model.ResortRate = RetentionRate(waybillDistributions).ToString();
                model.DayOutOrderCount = TodayBackWaybillSuccessCount(waybillDistributions);
                model.DayReceiveOrderCount = TodayCount(waybillDistributions, dateTime);
                model.DayInStationNeedOutSum = TodayIntoStationMastRefund(waybillDistributions, sourceType,
                                                                          dateTime, stationCode, merchantID);
                model.DayReceiveNeedInSum = TodayIntoStationNeedamountSum(waybillDistributions, dateTime);
                model.DayReceiveNeedOutSum = TodayIntoStationNeedBackAmountSum(waybillDistributions, dateTime);
                model.DayNeedDeliverOutSum = TodayNeedBackAmountSum(waybillDistributions, dateTime);
                model.DayOutStationOrderCount = TodayOutStationCount(sourceType, dateTime, stationCode,
                                                                     merchantID);
                model.DayOutStationNeedInSum = TodayOutStationNeedAmoutSum(sourceType, dateTime, stationCode,
                                                                           merchantID);
                model.DayOutStationNeedOutSum = TodayOutStationNeedBackAmoutSum(sourceType, dateTime,
                                                                                stationCode, merchantID);
                model.DayResortCount = TodayRetentionCount(waybillDistributions);
                model.DayResortNeedInSum = TodayRetentionNeedAmountSum(waybillDistributions);
                model.DayResortNeedOutSum = TodayRetentionNeedBackAmountSum(waybillDistributions);
                model.DayTransferOrderCount = TodaySwitchIntoStationCount(waybillDistributions, sourceType,
                                                                          dateTime, stationCode, merchantID);
                model.DayInStationNeedInSum = TodaySwitchIntoStationSum(waybillDistributions, sourceType,
                                                                        dateTime, stationCode, merchantID);
                model.ExchangeOrderCount = TodayExchangeCount(waybillDistributions);
                model.ExchangeOrderSum = TodayExchangeSum(waybillDistributions);
                AddSum(model);
            }
            catch (Exception ex)
            {
                WriteTest("站点ID：" + searchInfo.StationId + "，来源：" + searchInfo.Sources + "，插入配送报表汇总失败，错误信息：" + ex.Message + "---" + DateTime.Now);
            }
        }

        /// <summary>
        /// 添加报表汇总
        /// </summary>
        private void InsertDailySumV2(SearchModel searchInfo, DataTable dt)
        {
            try
            {
                var model = new FMS_StationDailyFinanceSum();
                int sourceType = Convert.ToInt32(searchInfo.Sources);
                DateTime dateTime = searchInfo.dtDailyDate;
                string stationCode = searchInfo.StationId;
                int? merchantID = null;
                if (!string.IsNullOrEmpty(searchInfo.MerchantId))
                {
                    merchantID = Convert.ToInt32(searchInfo.MerchantId);
                }
                IList<WaybillDistribution> waybillDistributions = ConvertToWaybillDistributionModelList(dt);
                model.MerchantID = merchantID;
                model.StationID = Convert.ToInt32(stationCode);
                model.Sources = sourceType;
                model.DailyTime = searchInfo.dtDailyDate;
                model.CreateTime = DateTime.Now;
                model.DayNeedDeliverInSum = AllNeedamountSum(waybillDistributions);
                model.AllRejectNeedInSum = AllRejectNeedamountSum(waybillDistributions);
                model.AllRejectNeedOutSum = AllRejectNeedBackAmountSum(waybillDistributions);
                model.CashRealInSum = CashFactAmountSum(waybillDistributions);
                model.CashRealOutSum = CashFactBackAmountSum(waybillDistributions);
                model.CashSuccOrderCount = CashSuccessCount(waybillDistributions);
                model.DeliverSuccRate = DistributionSuccessRate(waybillDistributions).ToString();
                model.PosRealInSum = PosFactAmountSum(waybillDistributions);
                model.PosSuccOrderCount = PosSuccessCount(waybillDistributions);
                model.RejectOrderCount = RejectWaybillCount(waybillDistributions);
                model.ResortRate = RetentionRate(waybillDistributions).ToString();
                model.DayOutOrderCount = TodayBackWaybillSuccessCount(waybillDistributions);
                model.DayReceiveOrderCount = TodayCount(waybillDistributions, dateTime);
                model.DayInStationNeedOutSum = TodayIntoStationMastRefund(waybillDistributions, sourceType,
                                                                          dateTime, stationCode, merchantID);
                model.DayReceiveNeedInSum = TodayIntoStationNeedamountSum(waybillDistributions, dateTime);
                model.DayReceiveNeedOutSum = TodayIntoStationNeedBackAmountSum(waybillDistributions, dateTime);
                model.DayNeedDeliverOutSum = TodayNeedBackAmountSum(waybillDistributions, dateTime);
                model.DayOutStationOrderCount = TodayOutStationCount(sourceType, dateTime, stationCode,
                                                                     merchantID);
                model.DayOutStationNeedInSum = TodayOutStationNeedAmoutSum(sourceType, dateTime, stationCode,
                                                                           merchantID);
                model.DayOutStationNeedOutSum = TodayOutStationNeedBackAmoutSum(sourceType, dateTime,
                                                                                stationCode, merchantID);
                model.DayResortCount = TodayRetentionCount(waybillDistributions);
                model.DayResortNeedInSum = TodayRetentionNeedAmountSum(waybillDistributions);
                model.DayResortNeedOutSum = TodayRetentionNeedBackAmountSum(waybillDistributions);
                model.DayTransferOrderCount = TodaySwitchIntoStationCount(waybillDistributions, sourceType,
                                                                          dateTime, stationCode, merchantID);
                model.DayInStationNeedInSum = TodaySwitchIntoStationSum(waybillDistributions, sourceType,
                                                                        dateTime, stationCode, merchantID);
                model.ExchangeOrderCount = TodayExchangeCount(waybillDistributions);
                model.ExchangeOrderSum = TodayExchangeSum(waybillDistributions);

                AddSumV2(model);
            }
            catch (Exception ex)
            {
                WriteTest("站点ID：" + searchInfo.StationId + "，来源：" + searchInfo.Sources + "，插入配送报表汇总失败，错误信息：" + ex.Message + "---" + DateTime.Now);
            }
        }

        /// <summary>
        /// 换货单总金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        private decimal? TodayExchangeSum(IList<WaybillDistribution> waybillDistributions)
        {
            decimal _NeedAmount = waybillDistributions.Where(
                item =>
                item.WaybillType == Convert.ToString((int)WayBillType.Exchange) &&
                item.SignStatus == Convert.ToString((int)WayBillStatus.Success)).Sum(
                item => item.NeedAmount ?? 0);
            decimal _NeedBackAmount = waybillDistributions.Where(
                item => item.WaybillType == Convert.ToString((int)WayBillType.Exchange)).Sum(
                item => item.NeedBackAmount ?? 0);
            return _NeedAmount - _NeedBackAmount;
        }

        /// <summary>
        /// 换货单总数量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        private int? TodayExchangeCount(IList<WaybillDistribution> waybillDistributions)
        {
            return
                waybillDistributions.Where(
                    item =>
                    item.WaybillType == Convert.ToString((int)WayBillType.Exchange) &&
                    item.SignStatus == Convert.ToString((int)WayBillStatus.Success)).Count();
        }

        /// <summary>
        /// 重新生成配送报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        public bool ReloadStationDailyDataV2(SearchModel searchInfo)
        {
            if (!string.IsNullOrEmpty(searchInfo.StationId) && !string.IsNullOrEmpty(searchInfo.Sources) &&
                !string.IsNullOrEmpty(searchInfo.dtDailyDate.ToString()))
            {
                DeleteStationDailyDataV2(searchInfo);

                AddStationDailyData(searchInfo);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 重新生成配送报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        public bool ReloadStationDailyData(SearchModel searchInfo)
        {
            if (!string.IsNullOrEmpty(searchInfo.StationId) && !string.IsNullOrEmpty(searchInfo.Sources) &&
                !string.IsNullOrEmpty(searchInfo.dtDailyDate.ToString()))
            {
                DeleteStationDailyData(searchInfo);

                AddStationDailyData(searchInfo);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 删除报表明细及汇总
        /// </summary>
        /// <param name="searchInfo"></param>
        private void DeleteStationDailyDataV2(SearchModel searchInfo)
        {
            _fMS_StationDailyFinanceSumDao.DeleteV2(searchInfo);
        }

        /// <summary>
        /// 删除报表明细及汇总
        /// </summary>
        /// <param name="searchInfo"></param>
        private void DeleteStationDailyData(SearchModel searchInfo)
        {
            _fMS_StationDailyFinanceSumDao.Delete(searchInfo);
        }

        #region IWaybillDistribution 成员

        private DataTable _trunIntoStationSummaryData;
        private DataTable _trunOutStationSummaryData;

        /// <summary>
        /// 合计
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public int CountAll(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Count;
        }

        /// <summary>
        /// 当日接单量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public int TodayCount(IList<WaybillDistribution> waybillDistributions, DateTime dateTime)
        {
            return
                waybillDistributions.Where(
                    item => item.IntoTime != null &&
                        item.IntoTime.Value.ToString("yyyyMMdd") == dateTime.ToString("yyyyMMdd")).Count();
        }

        /// <summary>
        /// 当日转站入站应退金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public decimal TodayIntoStationMastRefund(IList<WaybillDistribution> waybillDistributions, int? type,
                                                  DateTime dateTime, string stationCode, int? merchantID)
        {
            DataTable dt = GetTrunIntoStationSummaryData(type, dateTime, stationCode, merchantID);
            if (dt != null && dt.Rows.Count == 1)
                return dt.Columns.Contains("NeedBackAmountSum")
                           ? Convert.ToDecimal(dt.Rows[0]["NeedBackAmountSum"].ToString())
                           : 0;
            return 0;
        }

        /// <summary>
        /// 拒收单量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public int RejectWaybillCount(IList<WaybillDistribution> waybillDistributions)
        {
            return
                waybillDistributions.Where(
                    item => item.SignStatus == ((int)WayBillStatus.Reject).ToString()).Count();
        }

        /// <summary>
        /// 当日接货应收金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public decimal TodayIntoStationNeedamountSum(IList<WaybillDistribution> waybillDistributions, DateTime dateTime)
        {
            return
                waybillDistributions.Where(
                    item => item.IntoTime != null &&
                    item.IntoTime.Value.ToString("yyyyMMdd") == dateTime.ToString("yyyyMMdd")).Sum(
                    item => item.NeedAmount ?? 0);
        }

        /// <summary>
        /// 全部拒收金额（应收）
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal AllRejectNeedamountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(
                item => item.SignStatus == ((int)WayBillStatus.Reject).ToString()).Sum(item => item.NeedAmount ?? 0);
        }

        /// <summary>
        /// 当日接货应退金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public decimal TodayIntoStationNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions,
                                                         DateTime dateTime)
        {
            return waybillDistributions.Where(
                item => item.IntoTime != null &&
                item.IntoTime.Value.ToString("yyyyMMdd") == dateTime.ToString("yyyyMMdd")).Sum(
                item => item.NeedBackAmount ?? 0);
        }

        /// <summary>
        /// 当日应配送总金额（应收）
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal AllNeedamountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Sum(item => item.NeedAmount ?? 0);
        }

        /// <summary>
        /// 全部拒收金额（应退）
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal AllRejectNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(
                item => item.SignStatus == ((int)WayBillStatus.Reject).ToString()).Sum(item => item.NeedBackAmount ?? 0);
        }

        /// <summary>
        /// 当日应配送总金额（应退）
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public decimal TodayNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions, DateTime dateTime)
        {
            return waybillDistributions.Where(
                item => item.WaybillType == ((int)WayBillType.Returned).ToString()
                        || item.WaybillType == ((int)WayBillType.Exchange).ToString()).Sum(
                item => item.NeedBackAmount ?? 0);
        }

        /// <summary>
        /// 当日滞留量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public int TodayRetentionCount(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(
                item => item.SignStatus != ((int)WayBillStatus.Reject).ToString()
                        && item.SignStatus != ((int)WayBillStatus.Success).ToString()).Count();
        }

        /// <summary>
        /// 当日上门退款单量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public int TodayBackWaybillSuccessCount(IList<WaybillDistribution> waybillDistributions)
        {
            return
                waybillDistributions.Where(
                    item => item.WaybillType == ((int)WayBillType.Returned).ToString()
                            && item.SignStatus == ((int)WayBillStatus.Success).ToString()).Count();
        }

        /// <summary>
        /// 现金成功单量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public int CashSuccessCount(IList<WaybillDistribution> waybillDistributions)
        {
            return
                waybillDistributions.Where(
                    item =>
                    item.AcceptType.Trim().Equals("现金") &&
                    item.SignStatus == ((int)WayBillStatus.Success).ToString()).Count();
        }

        /// <summary>
        /// 当日滞留金额(应收)
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal TodayRetentionNeedAmountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(
                item => item.SignStatus != ((int)WayBillStatus.Reject).ToString()
                        && item.SignStatus != ((int)WayBillStatus.Success).ToString()).Sum(item => item.NeedAmount ?? 0);
        }

        /// <summary>
        /// 现金实收金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal CashFactAmountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return
                waybillDistributions.Where(item => item.AcceptType.Trim().Equals("现金")).Sum(item => item.FactAmount ?? 0);
        }

        /// <summary>
        /// 当日滞留金额(应退)
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal TodayRetentionNeedBackAmountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(
                item => item.SignStatus != ((int)WayBillStatus.Reject).ToString()
                        && item.SignStatus != ((int)WayBillStatus.Success).ToString()).Sum(
                item => item.NeedBackAmount ?? 0);
        }

        /// <summary>
        /// 现金实退金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal CashFactBackAmountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(item =>
                                              item.AcceptType.Trim().Equals("现金") &&
                                              item.SignStatus == ((int)WayBillStatus.Success).ToString()).Sum(
                item => item.FactBackAmount ?? 0);
        }

        /// <summary>
        /// POS机成功单量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public int PosSuccessCount(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(item =>
                                              item.AcceptType.Trim().ToUpper().Equals("POS机") &&
                                              item.SignStatus == ((int)WayBillStatus.Success).ToString()).Count();
        }

        /// <summary>
        /// 缓存转站出站汇总信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        private DataTable GetTrunOutStationSummaryData(int? type, DateTime dateTime, string stationCode, int? merchantID)
        {
            if (_trunOutStationSummaryData == null)
                _trunOutStationSummaryData = _fMS_StationDailyFinanceDetailsDao.GetTrunOutStationSummaryByStationAndTime(type, stationCode,
                                                                                                 dateTime, merchantID);
            return _trunOutStationSummaryData;
        }

        /// <summary>
        /// 缓存转站入站汇总信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        private DataTable GetTrunIntoStationSummaryData(int? type, DateTime dateTime, string stationCode,
                                                        int? merchantID)
        {
            if (_trunIntoStationSummaryData == null)
                _trunIntoStationSummaryData = _fMS_StationDailyFinanceDetailsDao.GetTrunIntoStationSummaryByStationAndTime(type, stationCode,
                                                                                                   dateTime, merchantID);
            return _trunIntoStationSummaryData;
        }

        /// <summary>
        /// 当日转出订单量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public int TodayOutStationCount(int? type, DateTime dateTime, string stationCode, int? merchantID)
        {
            DataTable dt = GetTrunOutStationSummaryData(type, dateTime, stationCode, merchantID);
            if (dt != null && dt.Rows.Count == 1)
                return dt.Columns.Contains("WaybillCount") ? Convert.ToInt32(dt.Rows[0]["WaybillCount"].ToString()) : 0;
            return 0;
        }

        /// <summary>
        /// 当日转出订单金额(应收)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public decimal TodayOutStationNeedAmoutSum(int? type, DateTime dateTime, string stationCode, int? merchantID)
        {
            DataTable dt = GetTrunOutStationSummaryData(type, dateTime, stationCode, merchantID);
            if (dt != null && dt.Rows.Count == 1)
                return dt.Columns.Contains("NeedAmountSum")
                           ? Convert.ToDecimal(dt.Rows[0]["NeedAmountSum"].ToString())
                           : 0;
            return 0;
        }

        /// <summary>
        /// 当日转出订单金额(应退)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public decimal TodayOutStationNeedBackAmoutSum(int? type, DateTime dateTime, string stationCode, int? merchantID)
        {
            DataTable dt = GetTrunOutStationSummaryData(type, dateTime, stationCode, merchantID);
            if (dt != null && dt.Rows.Count == 1)
                return dt.Columns.Contains("NeedBackAmountSum")
                           ? Convert.ToDecimal(dt.Rows[0]["NeedBackAmountSum"].ToString())
                           : 0;
            return 0;
        }

        /// <summary>
        /// 当日转站入站订单量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public int TodaySwitchIntoStationCount(IList<WaybillDistribution> waybillDistributions, int? type,
                                               DateTime dateTime, string stationCode, int? merchantID)
        {
            DataTable dt = GetTrunIntoStationSummaryData(type, dateTime, stationCode, merchantID);
            if (dt != null && dt.Rows.Count == 1)
                return dt.Columns.Contains("WaybillCount") ? Convert.ToInt32(dt.Rows[0]["WaybillCount"].ToString()) : 0;
            return 0;
        }

        /// <summary>
        /// 当日转站入站应收金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public decimal TodaySwitchIntoStationSum(IList<WaybillDistribution> waybillDistributions, int? type,
                                                 DateTime dateTime, string stationCode, int? merchantID)
        {
            DataTable dt = GetTrunIntoStationSummaryData(type, dateTime, stationCode, merchantID);
            if (dt != null && dt.Rows.Count == 1)
                return dt.Columns.Contains("NeedAmountSum")
                           ? Convert.ToDecimal(dt.Rows[0]["NeedAmountSum"].ToString())
                           : 0;
            return 0;
        }

        /// <summary>
        /// POS实收金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal PosFactAmountSum(IList<WaybillDistribution> waybillDistributions)
        {
            return waybillDistributions.Where(item =>
                                              item.AcceptType.Trim().ToUpper().Equals("POS机")).Sum(
                item => item.FactAmount ?? 0);
        }

        /// <summary>
        /// 配送成功率（[现金成功单量+POS成功单量+零收款成功单量]/[现金成功单量+POS成功单量+零收款成功单量+拒收成功单量]）
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal DistributionSuccessRate(IList<WaybillDistribution> waybillDistributions)
        {
            var numerator = (decimal)waybillDistributions.Where(item =>
                                                                 (item.AcceptType.Trim().Equals("现金")
                                                                  || item.AcceptType.Trim().ToUpper().Equals("POS机")
                                                                  ||
                                                                  ((!item.FactAmount.HasValue ||
                                                                    item.FactAmount.Value == 0)
                                                                   &&
                                                                   (!item.FactBackAmount.HasValue ||
                                                                    item.FactBackAmount.Value == 0)
                                                                  )) &&
                                                                 item.SignStatus ==
                                                                 ((int)WayBillStatus.Success).ToString()).Count();
            decimal denominator = numerator + waybillDistributions.Where(
                                                  item => item.SignStatus == ((int)WayBillStatus.Reject).ToString()).
                                                  Count();
            return Math.Round(denominator == 0 ? 0 : numerator / denominator, 2) * 100;
        }

        /// <summary>
        /// 滞留率：（当日滞留量/当日应配总总单量）
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal RetentionRate(IList<WaybillDistribution> waybillDistributions)
        {
            var numerator = (decimal)waybillDistributions.Where(
                                          item => item.SignStatus != ((int)WayBillStatus.Reject).ToString()
                                                  && item.SignStatus != ((int)WayBillStatus.Success).ToString()).Count();

            int denominator = waybillDistributions.Count();

            return Math.Round(denominator == 0 ? 0 : numerator / denominator, 3) * 100;
        }

        /// <summary>
        /// 零收款单量
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public int ZoreDepositCount(IList<WaybillDistribution> waybillDistributions)
        {
            return
                waybillDistributions.Where(
                    item => item.SignStatus == ((int)WayBillStatus.Success).ToString()
                            && (!item.FactAmount.HasValue || item.FactAmount.Value == 0)).Count();
        }

        /// <summary>
        /// 拒收率（拒收单量/总单量）
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal RejectRate(IList<WaybillDistribution> waybillDistributions)
        {
            var numerator = (decimal)waybillDistributions.Where(
                                          item => item.SignStatus == ((int)WayBillStatus.Reject).ToString()).Count();

            int denominator = waybillDistributions.Count();

            return Math.Round(denominator == 0 ? 0 : numerator / denominator, 3);
        }

        /// <summary>
        /// 存款金额
        /// </summary>
        /// <param name="waybillDistributions"></param>
        /// <returns></returns>
        public decimal DepositSum(IList<WaybillDistribution> waybillDistributions)
        {
            decimal prefixExpression = waybillDistributions.Where(
                item =>
                item.AcceptType.Trim().Equals("现金") &&
                item.SignStatus == ((int)WayBillStatus.Success).ToString()).Sum(item => item.FactAmount ?? 0);

            decimal suffixExpression = waybillDistributions.Where(
                item =>
                item.AcceptType.Trim().Equals("现金") &&
                item.SignStatus == ((int)WayBillStatus.Success).ToString()).Sum(item => item.FactBackAmount ?? 0);

            return prefixExpression - suffixExpression;
        }

        #endregion

        public DataTable GetWaybillByNo(string waybillNo)
        {
            return _fMS_StationDailyFinanceSumDao.GetWaybillByNo(waybillNo);
        }

        /// <summary>
        /// 根据订单号重新生成报表
        /// </summary>
        /// <param name="waybillNos"></param>
        public int ReloadStationDailyByWaybillNosV2(string waybillNos)
        {
            int count = 0;
            var searchInfo = new SearchModel();
            waybillNos.Split(',').ToList().ForEach(waybillNo =>
            {
                DataTable dt = GetWaybillByNo(waybillNo);
                DataRow dr = dt.Rows[0];
                searchInfo.Sources = dr["sources"].ToString();
                searchInfo.StationId = dr["DeliverStationID"].ToString();
                searchInfo.MerchantId = dr["merchantId"].ToString();
                searchInfo.dtDailyDate = Convert.ToDateTime(dr["CreatTime"].ToString());
                ReloadStationDailyDataV2(searchInfo);
                Thread.Sleep(100);
                count++;
            });
            return count;
        }

        /// <summary>
        /// 根据订单号重新生成报表
        /// </summary>
        /// <param name="waybillNos"></param>
        public int ReloadStationDailyByWaybillNos(string waybillNos)
        {
            int count = 0;
            var searchInfo = new SearchModel();
            waybillNos.Split(',').ToList().ForEach(waybillNo =>
            {
                DataTable dt = GetWaybillByNo(waybillNo);
                DataRow dr = dt.Rows[0];
                searchInfo.Sources = dr["sources"].ToString();
                searchInfo.StationId = dr["DeliverStationID"].ToString();
                searchInfo.MerchantId = dr["merchantId"].ToString();
                searchInfo.dtDailyDate = Convert.ToDateTime(dr["CreatTime"].ToString());
                ReloadStationDailyData(searchInfo);
                Thread.Sleep(100);
                count++;
            });
            return count;
        }
    }
}
