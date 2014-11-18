using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Common.Logging;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.WEBLOGIC.FinancialManage;
using System.Timers;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;

namespace ServiceIncomeAccount
{
    public partial class ServiceIncomeAccount : ServiceBase
    {
        IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
        private int merchantId;
        private DateTime servicedate;
        private Timer _time;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceIncomeAccount));

        public ServiceIncomeAccount()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        //public void OnStart()
        {
            Logger.Info("收入结算日统计服务启动");
            Common.SendFailedMail(Common.FailedSubject, "收入结算日统计服务启动");
            try
            {
                double sleeptime = Common.ServiceInterval;
                _time = new Timer { Interval = sleeptime };//实例化Timer类，设置间隔时间为10000毫秒； 
                _time.Elapsed += TimerElapsed;//到达时间的时候执行事件； 
                _time.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
                _time.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 
            }
            catch (Exception ex)
            {
                return;
            }
        }

        protected override void OnStop()
        {
            Common.SendFailedMail(Common.FailedSubject, "收入结算日统计服务停止");
            Logger.Info("收入结算日统计服务停止");
        }

        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        //public void TimerElapsed()
        {
            if (!Common.JudgeRunTime(Common.RunHour))
            {
                DateTime accountDate = DateTime.Now.Date.AddDays(-1);

                if (incomeAccountService.IsIncomeHistoryCount(accountDate) == 0)
                {
                    IncomeAccount(accountDate);
                }
            }
        }

        /// <summary>
        /// 收入结算服务入口
        /// </summary>
        /// <param name="accountDate"></param>
        public void IncomeAccount(DateTime accountDate)
        {
            DataTable merchantData = incomeAccountService.GetMerchantInfo("rfd");

            StringBuilder sb_d = new StringBuilder();
            StringBuilder sb_r = new StringBuilder();
            StringBuilder sb_v = new StringBuilder();
            StringBuilder sb_o = new StringBuilder();
            if (merchantData.Rows.Count > 0)
            {
                for (int i = 0; i < merchantData.Rows.Count; i++)
                {
                    merchantId = int.Parse(merchantData.Rows[i]["id"].ToString());
                    //收入结算--发货
                    int n_d=IncomeDeliveryCount(merchantId, accountDate);
                    if (n_d == (int)EnumResultType.T2) sb_d.Append(merchantId+",");

                    //收入结算--拒收
                    int n_r=IncomeReturnsCount(merchantId, accountDate);
                    if (n_r == (int)EnumResultType.T2) sb_r.Append(merchantId + ",");

                    //收入结算--上门
                    int n_v=IncomeVisitReturnsCount(merchantId, accountDate);
                    if (n_v == (int)EnumResultType.T2) sb_v.Append(merchantId + ",");

                    //收入结算--其它费用(代收货款)
                    int n_o=IncomeOtherFeeReceiveFee(merchantId, accountDate);
                    if (n_o == (int)EnumResultType.T2) sb_o.Append(merchantId + ",");
                }

            }

            if (!string.IsNullOrWhiteSpace(sb_d.ToString()))
            {
                Common.SendFailedMail(Common.FailedSubject, "收入发货日统计无数据，统计时间：" + accountDate.ToString() + "商家编号：" + sb_d.ToString().TrimEnd(',') + ";");
            }
            if (!string.IsNullOrWhiteSpace(sb_r.ToString()))
            {
                Common.SendFailedMail(Common.FailedSubject, "收入拒收日统计无数据，统计时间：" + accountDate.ToString() + "商家编号：" + sb_r.ToString().TrimEnd(',') + ";");
            }
            if (!string.IsNullOrWhiteSpace(sb_v.ToString()))
            {
                Common.SendFailedMail(Common.FailedSubject, "收入上门退日统计无数据，统计时间：" + accountDate.ToString() + "商家编号：" + sb_v.ToString().TrimEnd(',') + ";");
            }
            if (!string.IsNullOrWhiteSpace(sb_o.ToString()))
            {
                Common.SendFailedMail(Common.FailedSubject, "收入其他费用日统计无数据，统计时间：" + accountDate.ToString() + "商家编号：" + sb_o.ToString().TrimEnd(',') + ";");
            }

            //跑历史
            //IncomeHistoryCount(accountDate);
        }

        #region 跑当天
        /// <summary>
        /// 收入结算--发货
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="countdate"></param>
        /// <returns></returns>
        public int IncomeDeliveryCount(int merchantid, DateTime countdate)
        {
            StringBuilder sbMsg = new StringBuilder();
            try
            {
                if (merchantid < 0 || countdate == DateTime.MinValue)
                {
                    return (int)EnumResultType.T0;
                }

                DataTable dt = incomeAccountService.GetIncomeDeliveryAccountInfo(merchantid, countdate);
                
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int num = 0;
                        int warehouseid = int.Parse(dt.Rows[i]["warehouseid"].ToString());
                        int deliverstationid = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        num = incomeAccountService.CollateIncomeDeliveryAccountInfo(merchantid, countdate, warehouseid, deliverstationid);//收入结算中已经结算运费的订单数量
                        if (int.Parse(dt.Rows[i]["FormCount"].ToString()) == num)
                        {
                            IncomeStatLog modellog = new IncomeStatLog();
                            modellog.ExpressCompanyID = warehouseid;
                            modellog.Ip = Common.GetMachineIp();
                            modellog.MerchantID = merchantid;
                            modellog.StationID = deliverstationid;
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 1;
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;
                            bool isOk = incomeAccountService.AddIncomeDeliveryDetails(merchantid, countdate, deliverstationid, warehouseid);

                            if (isOk)
                            {
                                modellog.IsSuccess = 1;
                                modellog.Reasons = "成功";
                            }
                            else
                            {
                                modellog.IsSuccess = 0;
                                modellog.Reasons = "插入收入结算失败，需重新跑服务";
                            }

                            incomeAccountService.AddIncomeStatLog(modellog);

                        }
                        else
                        {
                            //插入日志表
                            IncomeStatLog modellog = new IncomeStatLog();

                            modellog.ExpressCompanyID = int.Parse(dt.Rows[i]["warehouseid"].ToString());
                            modellog.Ip = Common.GetMachineIp();
                            modellog.IsSuccess = 0;
                            modellog.MerchantID = merchantid;
                            modellog.Reasons = countdate.ToString() + "相差" + (int.Parse(dt.Rows[i]["FormCount"].ToString()) - num).ToString() + "单";
                            modellog.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 1;
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;

                            incomeAccountService.AddIncomeStatLog(modellog);
                        }
                    }
                }
                else
                {
                    return (int)EnumResultType.T2;
                }
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message);
                return (int)EnumResultType.T3;
            }

            return (int)EnumResultType.T1;
        }

        /// <summary>
        /// 收入结算--拒收
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="countdate"></param>
        /// <returns></returns>
        public int IncomeReturnsCount(int merchantid, DateTime countdate)
        {
            StringBuilder sbMsg = new StringBuilder();
            try
            {
                if (merchantid < 0 || countdate == DateTime.MinValue)
                {
                    return (int)EnumResultType.T0;
                }

                //当日拒收数据（商家、配送站、分拣中心）
                DataTable dt = incomeAccountService.GetIncomeReturnsCount(merchantid, countdate);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int num = 0;
                        int returnExpressId = int.Parse(dt.Rows[i]["ReturnExpressCompanyID"].ToString());
                        int deliverstationid = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        //校验
                        num = incomeAccountService.CollateIncomeReturnsAccountInfo(merchantid, countdate, returnExpressId, deliverstationid);//收入结算中已经拒收入库结算运费的订单数量
                        if (int.Parse(dt.Rows[i]["FormCount"].ToString()) == num)
                        {
                            IncomeStatLog modellog = new IncomeStatLog();
                            modellog.ExpressCompanyID = returnExpressId;
                            modellog.Ip = Common.GetMachineIp();
                            modellog.MerchantID = merchantid;
                            modellog.StationID = deliverstationid;
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 2;
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;

                            bool isOk = incomeAccountService.AddIncomeReturnsAccountDetails(merchantid, countdate, deliverstationid, returnExpressId);
                            if (isOk)
                            {
                                modellog.IsSuccess = 1;
                                modellog.Reasons = "成功";
                            }
                            else
                            {
                                modellog.IsSuccess = 0;
                                modellog.Reasons = "插入收入结算失败，需重新跑服务";
                            }

                            incomeAccountService.AddIncomeStatLog(modellog);

                        }
                        else
                        {
                            //插入日志表
                            IncomeStatLog modellog = new IncomeStatLog();

                            modellog.ExpressCompanyID = int.Parse(dt.Rows[i]["ReturnExpressCompanyID"].ToString());
                            modellog.Ip = Common.GetMachineIp();
                            modellog.IsSuccess = 0;
                            modellog.MerchantID = merchantid;
                            modellog.Reasons = countdate.ToString() + "相差" + (int.Parse(dt.Rows[i]["FormCount"].ToString()) - num).ToString() + "单";
                            modellog.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 2;
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;

                            incomeAccountService.AddIncomeStatLog(modellog);
                        }
                    }
                }
                else
                {
                    return (int)EnumResultType.T2;
                }
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message);
                return (int)EnumResultType.T3;
            }

            return (int)EnumResultType.T1;
        }

        /// <summary>
        /// 收入结算--上门
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="countdate"></param>
        /// <returns></returns>
        public int IncomeVisitReturnsCount(int merchantid, DateTime countdate)
        {
            StringBuilder sbMsg = new StringBuilder();
            try
            {
                if (merchantid < 0 || countdate == DateTime.MinValue)
                {
                    return (int)EnumResultType.T0;
                }

                //收入结算--上门（站点、返回的分拣中心）
                DataTable dt = incomeAccountService.GetIncomeVisitReturnsCount(merchantid, countdate);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int num = 0;
                        int returnExpressId = int.Parse(dt.Rows[i]["ReturnExpressCompanyID"].ToString());
                        int deliverstationid = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        //校验
                        num = incomeAccountService.CollateIncomeVisitReturnsAccountInfo(merchantid, countdate, returnExpressId, deliverstationid);//收入结算中已经拒收入库结算运费的订单数量
                        if (int.Parse(dt.Rows[i]["FormCount"].ToString()) == num)
                        {
                            IncomeStatLog modellog = new IncomeStatLog();
                            modellog.ExpressCompanyID = returnExpressId;
                            modellog.Ip = Common.GetMachineIp();
                            modellog.MerchantID = merchantid;
                            modellog.StationID = deliverstationid;
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 3;
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;

                            //插入收入结算上门退信息
                            bool isOk = incomeAccountService.AddIncomeVisitReturnsAccountDetails(merchantid, countdate, deliverstationid, returnExpressId);
                            if (isOk)
                            {
                                modellog.IsSuccess = 1;
                                modellog.Reasons = "成功";
                            }
                            else
                            {
                                modellog.IsSuccess = 0;
                                modellog.Reasons = "插入收入结算失败，需重新跑服务";
                            }

                            //插入日志
                            incomeAccountService.AddIncomeStatLog(modellog);

                        }
                        else
                        {
                            //插入日志表
                            IncomeStatLog modellog = new IncomeStatLog();

                            modellog.ExpressCompanyID = int.Parse(dt.Rows[i]["ReturnExpressCompanyID"].ToString());
                            modellog.Ip = Common.GetMachineIp();
                            modellog.IsSuccess = 0;
                            modellog.MerchantID = merchantid;
                            modellog.Reasons = countdate.ToString() + "相差" + (int.Parse(dt.Rows[i]["FormCount"].ToString()) - num).ToString() + "单";
                            modellog.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 3;
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;

                            //插入日志
                            incomeAccountService.AddIncomeStatLog(modellog);
                        }
                    }
                }
                else
                {
                    return (int)EnumResultType.T2;
                }
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message);
                return (int)EnumResultType.T3;
            }

            return (int)EnumResultType.T1;
        }

        /// <summary>
        /// 收入结算--其它费用(代收货款)
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="countdate"></param>
        /// <returns></returns>
        public int IncomeOtherFeeReceiveFee(int merchantId, DateTime countdate)
        {
            StringBuilder sbMsg = new StringBuilder();
            try
            {
                if (merchantId < 0 || countdate == DateTime.MinValue)
                {
                    return (int)EnumResultType.T0;
                }
                //收入结算--上门（站点、类型）
                DataTable dt = incomeAccountService.GetIncomeOtherReceiveFee(merchantId, countdate);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int num = 0;
                        int deliverstationid = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        //校验
                        num = incomeAccountService.CollateIncomeOtherReceiveFee(merchantId, countdate, deliverstationid);//收入结算中已经拒收入库结算运费的订单数量
                        if (int.Parse(dt.Rows[i]["FormCount"].ToString()) == num)
                        {
                            IncomeStatLog modellog = new IncomeStatLog();
                            modellog.Ip = Common.GetMachineIp();
                            modellog.MerchantID = merchantId;
                            modellog.StationID = deliverstationid;
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 4;//代收货款
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;

                            //插入收入结算上门退信息
                            bool isOk = incomeAccountService.AddIncomeOtherReceiveFee(merchantId, countdate, deliverstationid);
                            if (isOk)
                            {
                                modellog.IsSuccess = 1;
                                modellog.Reasons = "成功";
                            }
                            else
                            {
                                modellog.IsSuccess = 0;
                                modellog.Reasons = "插入收入结算失败，需重新跑服务";
                            }

                            //插入日志
                            incomeAccountService.AddIncomeStatLog(modellog);

                        }
                        else
                        {
                            //插入日志表
                            IncomeStatLog modellog = new IncomeStatLog();

                            modellog.Ip = Common.GetMachineIp();
                            modellog.IsSuccess = 0;
                            modellog.MerchantID = merchantId;
                            modellog.Reasons = countdate.ToString() + "相差" + (int.Parse(dt.Rows[i]["FormCount"].ToString()) - num).ToString() + "单";
                            modellog.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                            modellog.StatisticsDate = countdate;
                            modellog.StatisticsType = 4;//代收货款
                            modellog.UpdateTime = DateTime.Now;
                            modellog.CreateTime = DateTime.Now;

                            //插入日志
                            incomeAccountService.AddIncomeStatLog(modellog);
                        }
                    }
                }
                else
                {
                    return (int)EnumResultType.T2;
                }
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message);
                return (int)EnumResultType.T3;
            }

            return (int)EnumResultType.T1;
        }
        #endregion

        #region 跑历史

        public bool IncomeHistoryCount(DateTime countdate)
        {
            DataTable dtHistoryCount = incomeAccountService.GetIncomeHistoryCount(countdate);

            if (dtHistoryCount.Rows.Count > 0)
            {
                int StatisticsType = 0;
                int MerchantID = 0;
                int StationID = 0;
                int ExpressCompanyID = 0;
                string reasons = "";
                int LogId = 0;
                DateTime countdatehistory;
                for (int i = 0; i < dtHistoryCount.Rows.Count; i++)
                {
                    StatisticsType = int.Parse(dtHistoryCount.Rows[i]["StatisticsType"].ToString());
                    MerchantID = int.Parse(dtHistoryCount.Rows[i]["MerchantID"].ToString());
                    StationID = int.Parse(dtHistoryCount.Rows[i]["StationID"].ToString());

                    if (!string.IsNullOrEmpty(dtHistoryCount.Rows[i]["ExpressCompanyID"].ToString()))
                    { ExpressCompanyID = int.Parse(dtHistoryCount.Rows[i]["ExpressCompanyID"].ToString()); }

                    reasons = dtHistoryCount.Rows[i]["Reasons"].ToString();
                    LogId = int.Parse(dtHistoryCount.Rows[i]["LogID"].ToString());
                    countdatehistory = DateTime.Parse(dtHistoryCount.Rows[i]["StatisticsDate"].ToString());
                    //发货
                    if (StatisticsType == 1)
                    {
                        ReRunDeliverCountHistory(MerchantID, countdatehistory, ExpressCompanyID, StationID, LogId, reasons);
                    }

                    //拒收
                    if (StatisticsType == 2)
                    {
                        ReRunReturnsCountHistory(MerchantID, countdatehistory, ExpressCompanyID, StationID, LogId, reasons);

                    }

                    //上门
                    if (StatisticsType == 3)
                    {
                        ReRunVisitReturnsCountHistory(MerchantID, countdatehistory, ExpressCompanyID, StationID, LogId, reasons);

                    }

                    //代收货款
                    if (StatisticsType == 4)
                    {
                        ReRunReceiveFeeCountHistory(MerchantID, countdatehistory, ExpressCompanyID, StationID, LogId, reasons);

                    }

                }
            }
            return true;
        }

        /// <summary>
        /// 历史发货统计
        /// </summary>
        /// <param name="MerchantID"></param>
        /// <param name="countdate"></param>
        /// <param name="ExpressCompanyID"></param>
        /// <param name="StationID"></param>
        /// <param name="LogId"></param>
        /// <param name="reasons"></param>
        /// <returns></returns>
        public bool ReRunDeliverCountHistory(int MerchantID, DateTime countdate, int ExpressCompanyID, int StationID, int LogId, string reasons)
        {
            int num1 = incomeAccountService.IncomeDeliveryAccountHistory(MerchantID, countdate, ExpressCompanyID, StationID);
            int num2 = incomeAccountService.CollateIncomeDeliveryAccountInfo(MerchantID, countdate, ExpressCompanyID, StationID);
            bool isOk = false;
            if (num1 == 0 && num2 == 0)
            {
                return isOk;
            }
            if (num1 == num2)
            {
                IncomeStatLog modellog = new IncomeStatLog();
                modellog.LogID = LogId;
                modellog.UpdateTime = DateTime.Now;
                isOk = incomeAccountService.AddIncomeDeliveryDetails(MerchantID, countdate, StationID, ExpressCompanyID);
                if (isOk)
                {
                    modellog.IsSuccess = 1;
                    modellog.Reasons = reasons + "---重跑服务：成功";
                }
                else
                {
                    modellog.IsSuccess = 0;
                    modellog.Reasons = reasons + "---重跑服务：失败";
                }

                incomeAccountService.UpdateIncomeStatLog(modellog);
            }
            return isOk;
        }

        /// <summary>
        /// 历史拒收统计
        /// </summary>
        /// <param name="MerchantID"></param>
        /// <param name="countdate"></param>
        /// <param name="ExpressCompanyID"></param>
        /// <param name="StationID"></param>
        /// <param name="LogId"></param>
        /// <param name="reasons"></param>
        /// <returns></returns>
        public bool ReRunReturnsCountHistory(int MerchantID, DateTime countdate, int ExpressCompanyID, int StationID, int LogId, string reasons)
        {
            int num1 = incomeAccountService.IncomeReturnsAccountHistory(MerchantID, countdate, ExpressCompanyID, StationID);
            int num2 = incomeAccountService.CollateIncomeReturnsAccountInfo(MerchantID, countdate, ExpressCompanyID, StationID);
            bool isOk = false;
            if (num1 == 0 && num2 == 0)
            {
                return isOk;
            }
            if (num1 == num2)
            {
                IncomeStatLog modellog = new IncomeStatLog();
                modellog.LogID = LogId;
                modellog.UpdateTime = DateTime.Now;
                isOk = incomeAccountService.AddIncomeReturnsAccountDetails(MerchantID, countdate, StationID, ExpressCompanyID);
                if (isOk)
                {
                    modellog.IsSuccess = 1;
                    modellog.Reasons = reasons + "---重跑服务：成功";
                }
                else
                {
                    modellog.IsSuccess = 0;
                    modellog.Reasons = reasons + "---重跑服务：失败";
                }

                incomeAccountService.UpdateIncomeStatLog(modellog);
            }
            return isOk;
        }

        /// <summary>
        /// 历史上门统计
        /// </summary>
        /// <param name="MerchantID"></param>
        /// <param name="countdate"></param>
        /// <param name="ExpressCompanyID"></param>
        /// <param name="StationID"></param>
        /// <param name="LogId"></param>
        /// <param name="reasons"></param>
        /// <returns></returns>
        public bool ReRunVisitReturnsCountHistory(int MerchantID, DateTime countdate, int ExpressCompanyID, int StationID, int LogId, string reasons)
        {
            int num1 = incomeAccountService.IncomeVisitReturnsAccountHistory(MerchantID, countdate, ExpressCompanyID, StationID);
            int num2 = incomeAccountService.CollateIncomeVisitReturnsAccountInfo(MerchantID, countdate, ExpressCompanyID, StationID);
            bool isOk = false;
            if (num1 == 0 && num2 == 0)
            {
                return isOk;
            }
            if (num1 == num2)
            {
                IncomeStatLog modellog = new IncomeStatLog();
                modellog.LogID = LogId;
                modellog.UpdateTime = DateTime.Now;
                isOk = incomeAccountService.AddIncomeVisitReturnsAccountDetails(MerchantID, countdate, StationID, ExpressCompanyID);
                if (isOk)
                {
                    modellog.IsSuccess = 1;
                    modellog.Reasons = reasons + "---重跑服务：成功";
                }
                else
                {
                    modellog.IsSuccess = 0;
                    modellog.Reasons = reasons + "---重跑服务：失败";
                }

                incomeAccountService.UpdateIncomeStatLog(modellog);
            }
            return isOk;
        }

        /// <summary>
        /// 历史代收货款统计
        /// </summary>
        /// <param name="MerchantID"></param>
        /// <param name="countdate"></param>
        /// <param name="ExpressCompanyID"></param>
        /// <param name="StationID"></param>
        /// <param name="LogId"></param>
        /// <param name="reasons"></param>
        /// <returns></returns>
        public bool ReRunReceiveFeeCountHistory(int MerchantID, DateTime countdate, int ExpressCompanyID, int StationID, int LogId, string reasons)
        {
            int num1 = incomeAccountService.IncomeOtherReceiveFeeHistory(MerchantID, countdate, StationID);
            int num2 = incomeAccountService.CollateIncomeOtherReceiveFee(MerchantID, countdate, StationID);
            bool isOk = false;
            if (num1 == 0 && num2 == 0)
            {
                return isOk;
            }
            if (num1 == num2)
            {
                IncomeStatLog modellog = new IncomeStatLog();
                modellog.LogID = LogId;
                modellog.UpdateTime = DateTime.Now;
                isOk = incomeAccountService.AddIncomeOtherReceiveFee(merchantId, countdate, StationID);
                if (isOk)
                {
                    modellog.IsSuccess = 1;
                    modellog.Reasons = reasons + "---重跑服务：成功";
                }
                else
                {
                    modellog.IsSuccess = 0;
                    modellog.Reasons = reasons + "---重跑服务：失败";
                }

                incomeAccountService.UpdateIncomeStatLog(modellog);
            }
            return isOk;
        }
        #endregion
    }

    public enum EnumResultType
    {
        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        T0 = 0,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        T1 = 1,
        /// <summary>
        /// 没有返货结果
        /// </summary>
        [Description("没有返货结果")]
        T2 = 2,
        /// <summary>
        /// 出现异常
        /// </summary>
        [Description("出现异常")]
        T3 = 3,
        T4 = 4,
        T5 = 5
    }
}
