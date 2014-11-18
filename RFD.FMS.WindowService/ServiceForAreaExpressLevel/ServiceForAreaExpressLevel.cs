using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Common.Logging;
using System.Timers;
using System.Configuration;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;


namespace ServiceForAreaExpressLevel
{
    public partial class ServiceForAreaExpressLevel : ServiceBase
    {
        public ServiceForAreaExpressLevel()
        {
            InitializeComponent();
        }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceForAreaExpressLevel));
        private IAreaExpressLevelService _areaExpressLevel;
        private IAreaExpressLevelIncomeService _areaExpressLevelIncome;

        //支出区域类型属性字段
        private string areaid = null;
        private int expressCompanyid = 0;
        private int affectareatype = 0;
        private string warewhouseid = null;
        private int autoid = 0;
        private int warehousetype;
        private int merchant;
        private int productid;

        //收入区域类型属性字段
        private string areaidIncome = null;
        private int merchantid = 0;
        private int affectareatypeIncome = 0;
        private string warehouseidIncome = null;
        private int autoidIncome = 0;
        private int expressIdIncome=0;

        //错误信息
        private string errorAreasIncome;
        private string errorAreas;

        private System.Timers.Timer _timeExpend;
        private static bool isRuningExpend = false;

        private System.Timers.Timer _timeIncome;
        private static bool isRuningIncome = false;

        protected override void OnStart(string[] args)
        //public void OnStart()
        {

            Logger.Info("区域类型生效服务启动");
            
            StringBuilder sbService = new StringBuilder();
            try
            {
                _areaExpressLevel = ServiceLocator.GetService<IAreaExpressLevelService>();
                _areaExpressLevelIncome =ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                sbService.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"：");
                if (Common.ExpendIsRun)
                {
                    sbService.AppendLine("支出区域类型生效");
                    _timeExpend = new Timer { Interval = Common.ExpendInterval };//实例化Timer类，设置间隔时间为10000毫秒； 
                    _timeExpend.Elapsed += TimerElapsedExpend;//到达时间的时候执行事件； 
                    _timeExpend.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
                    _timeExpend.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 
                }

                if (Common.IncomeIsRun)
                {
                    sbService.AppendLine("收入区域类型生效");
                    _timeIncome = new Timer { Interval = Common.IncomeInterval };//实例化Timer类，设置间隔时间为10000毫秒； 
                    _timeIncome.Elapsed += TimerElapsedIncome;//到达时间的时候执行事件； 
                    _timeIncome.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
                    _timeIncome.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 
                }

                Common.WriteText(sbService.ToString() + "服务启动");

                Common.SendFailedMail(Common.FailedSubject, sbService.ToString()+"服务启动");
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message + ex.Source + ex.StackTrace);

                Common.WriteText(ex.Message + ex.Source + ex.StackTrace);

                return;
            }
        }

        protected override void OnStop()
        {
            Common.SendFailedMail(Common.FailedSubject, "区域类型生效服务停止");
            Logger.Info("区域类型生效服务停止");
        }

        /// <summary>
        /// 业务处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TimerElapsedExpend(object sender, ElapsedEventArgs e)
        {
            if (Common.JudgeRunTime(Common.ExpendRunHour))
                return;
            if (isRuningExpend) return;

            isRuningExpend = true;
            try
            {
                errorAreas = null;
                AreaExpressCompanyLevelAudit();
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message + "\n" + ex);
            }
            finally
            {
                isRuningExpend = false;
            }
        }

        /// <summary>
        /// 业务处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TimerElapsedIncome(object sender, ElapsedEventArgs e)
        {
            if (Common.JudgeRunTime(Common.IncomeRunHour))
                return;
            if (isRuningIncome) return;

            isRuningIncome = true;
            try
            {
                errorAreasIncome = null;
                //AreaMerchantLevelAudit();
                _areaExpressLevelIncome.IncomeAreaLevelToEffect(Common.RowCount);
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message + "\n" + ex);
            }
            finally
            {
                isRuningIncome = false;
            }
        }

        //支出区域类型生效方法
        protected void AreaExpressCompanyLevelAudit()
        {
            DateTime nowdate = DateTime.Now;

            try
            {
                DataTable dt = _areaExpressLevel.AreaExpressCompanyLevelNum(int.Parse(ConfigurationManager.AppSettings["RowCount"].ToString()), nowdate);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            autoid = int.Parse(dt.Rows[i]["autoid"].ToString());
                            areaid = dt.Rows[i]["areaid"].ToString();
                            expressCompanyid = int.Parse(dt.Rows[i]["expresscompanyid"].ToString());
                            affectareatype = int.Parse(dt.Rows[i]["effectareatype"].ToString());
                            warewhouseid = dt.Rows[i]["warehouseid"].ToString();

                            warehousetype = string.IsNullOrEmpty(dt.Rows[i]["warehousetype"].ToString())
                                                ? 0
                                                : int.Parse(dt.Rows[i]["warehousetype"].ToString());
                            merchant = string.IsNullOrEmpty(dt.Rows[i]["merchantid"].ToString())
                                           ? 0
                                           : int.Parse(dt.Rows[i]["merchantid"].ToString());
                            productid = string.IsNullOrEmpty(dt.Rows[i]["productid"].ToString())
                                            ? 0
                                            : int.Parse(dt.Rows[i]["productid"].ToString());


                            if (dt.Rows[i]["IsEnable"].ToString() == "1")//更新Areatype
                            {
                                _areaExpressLevel.AreaExpressCompanyLevelUpdate(autoid, areaid, expressCompanyid,
                                                                                affectareatype, warewhouseid,warehousetype,merchant,productid);
                            }
                            else if (dt.Rows[i]["IsEnable"].ToString() == "3")//新增Areatype
                            {
                                _areaExpressLevel.AreaExpressCompanyLevelAdd(autoid, areaid, expressCompanyid, affectareatype,
                                                                             warewhouseid,warehousetype,merchant,productid);
                            }
                            else if (dt.Rows[i]["IsEnable"].ToString() == "2")//删除Areatype
                            {
                                _areaExpressLevel.AreaExpressCompanyLevelDel(autoid, areaid, expressCompanyid, affectareatype,
                                                                             warewhouseid,warehousetype,merchant,productid);
                            }
                        }
                        catch (Exception ex)
                        {
                            errorAreas = errorAreas + areaid + ",";
                            continue;
                        }
                    }
                }
                else
                {
                    return;
                }

                if (!string.IsNullOrEmpty(errorAreas))
                {
                    Common.SendFailedMail("区域类型生效失败", "支出区域类型无法生效的地区：" + errorAreas);
                }
            }
            catch (Exception ex)
            {
                return;
            }

        }

        //收入区域类型生效方法
        protected void AreaMerchantLevelAudit()
        {
            DateTime nowdate = DateTime.Now;

            try
            {
                DataTable dt = _areaExpressLevelIncome.AreaMerchantLevelNum(int.Parse(ConfigurationManager.AppSettings["RowCount"].ToString()), nowdate);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            autoidIncome = int.Parse(dt.Rows[i]["autoid"].ToString());
                            areaidIncome = dt.Rows[i]["areaid"].ToString();
                            merchantid = int.Parse(dt.Rows[i]["merchantid"].ToString());
                            affectareatypeIncome = int.Parse(dt.Rows[i]["effectareatype"].ToString());
                            warehouseidIncome = dt.Rows[i]["warehouseid"].ToString();

                            if (!string.IsNullOrEmpty(dt.Rows[i]["expresscompanyid"].ToString()))
                            {
                                expressIdIncome = int.Parse(dt.Rows[i]["expresscompanyid"].ToString());
                            }
                            else
                            {
                                expressIdIncome = 0;
                            }
                            
                            if (dt.Rows[i]["IsEnable"].ToString() == "1")//更新Areatype
                            {
                                _areaExpressLevelIncome.AreaMerchantLevelUpdate(autoidIncome, areaidIncome, merchantid, affectareatypeIncome, warehouseidIncome,expressIdIncome);
                            }
                            else if (dt.Rows[i]["IsEnable"].ToString() == "3")//新增Areatype
                            {
                                _areaExpressLevelIncome.AreaMerchantLevelAdd(autoidIncome, areaidIncome, merchantid, affectareatypeIncome, warehouseidIncome,expressIdIncome);
                            }
                            else if (dt.Rows[i]["IsEnable"].ToString() == "2")//删除Areatype
                            {
                                _areaExpressLevelIncome.AreaMerchantLevelDel(autoidIncome, areaidIncome, merchantid, affectareatypeIncome, warehouseidIncome,expressIdIncome);
                            }
                        }
                        catch (Exception ex)
                        {
                            errorAreasIncome = errorAreasIncome + areaidIncome+",";      
                           continue;
                        }
                    }
                }
                else
                {
                    return;
                }

                if(!string.IsNullOrEmpty(errorAreasIncome))
                {
                    Common.SendFailedMail("区域类型生效失败", "收入区域类型无法生效的地区：" + errorAreasIncome);
                }
            }
            catch (Exception ex)
            {
               return;
            }

        }
    }
}
