using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using RFD.FMS.Util;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using System.Windows.Forms;

namespace ServiceForInComeFeeAcount
{
    public partial class ServiceForInComeFeeAcount : ServiceBase
    {
        public ServiceForInComeFeeAcount()
        {
            InitializeComponent();
        }

        IWaybillForIncomeBaseInfoService IncomeBaseService;

        /// <summary>
        /// 定义时间
        /// </summary>
        private System.Timers.Timer _time;
        private static bool isRuning = false;

        private System.Timers.Timer _timeClear;
        private static bool isRuningClear = false;

        private System.Timers.Timer _timeHistory;
        private static bool isRuningHistory = false;

        private System.Timers.Timer _timeEffect;
        private static bool isRuningEffect = false;

        protected override void OnStart(string[] args)
        //public void OnStart()
        {
            StringBuilder sbService = new StringBuilder();
            try
            {
                IncomeBaseService = ServiceLocator.GetService<IWaybillForIncomeBaseInfoService>();
                sbService.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"：");
                if (Common.AccountIsRun)
                {
                    sbService.AppendLine("收入配送费计算服务");
                    double sleeptime = Common.ServiceInterval;
                    _time = new System.Timers.Timer { Interval = sleeptime };//实例化Timer类，设置间隔时间为10000毫秒； 
                    _time.Elapsed += TimerElapsed;//到达时间的时候执行事件； 
                    _time.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
                    _time.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 
                }

                //日清除、统计服务
                if (Common.ClearIsAccount)
                {
                    sbService.AppendLine("日清除、统计服务");
                    _timeClear = new System.Timers.Timer { Interval = Common.ClearInterval };
                    _timeClear.Elapsed += ClearTimerElapsed;
                    _timeClear.AutoReset = true;
                    _timeClear.Enabled = true;
                }

                //历史计算服务
                if (Common.HistoryIsRun)
                {
                    sbService.AppendLine("历史计算服务");
                    _timeHistory = new System.Timers.Timer { Interval = Common.HistoryInterval };
                    _timeHistory.Elapsed += HistoryTimerElapsed;
                    _timeHistory.AutoReset = true;
                    _timeHistory.Enabled = true;
                }

                //生效服务
                if (Common.EffectIsRun)
                {
                    sbService.AppendLine("生效服务");
                    _timeEffect = new System.Timers.Timer { Interval = Common.EffectInterval };
                    _timeEffect.Elapsed += EffectTimerElapsed;
                    _timeEffect.AutoReset = true;
                    _timeEffect.Enabled = true;
                }

                sbService.AppendLine("服务已启动");
                WriteText(sbService.ToString());
                Common.SendFailedMail(Common.SuccedSubject, sbService.ToString());
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message + ex.Source + ex.StackTrace);

                WriteText(ex.Message + ex.Source + ex.StackTrace);

                return;
            }
        }

        /// <summary>
        /// 业务处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (isRuning) return;

            isRuning = true;
            try
            {
                IncomeBaseService.UpdateIncomeFeeInfoDao(Common.RowCount);
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message + "\n" + ex);
            }
            finally
            {
                isRuning = false;
            }
        }

        protected void ClearTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (isRuningClear) return;

            isRuningClear = true;

            try
            {
                string hmNow = DateTime.Now.ToString("HH:mm:ss");
                if (hmNow == Common.ClearStartTime)
                {
                    //WriteText("Clear test");
                    string log = IncomeBaseService.GetIncomeErrorLog();
                    if (!string.IsNullOrEmpty(log))
                    {
                        WriteText(log);
                        Common.SendFailedMail(Common.FailedSubject,log);
                    }
                    IncomeBaseService.ClearIncomeIsAccount(Common.RowCount);
                }
            }
            catch (Exception ex)
            {
                //WriteText("Clear test Exception"+ex.Message+ex.StackTrace);
                Common.SendFailedMail(Common.FailedSubject, ex.Message + "\n" + ex);
            }
            finally
            {
                isRuningClear = false;
            }
        }

        protected void HistoryTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Common.JudgeRunTime(Common.HistoryRunHour))
                return;

            if (isRuningHistory) return;

            isRuningHistory = true;

            try
            {
                //string hmNow = DateTime.Now.ToString("HH:mm:ss");
                //if (hmNow == Common.HistoryStartTime)
                //{
                    //WriteText("History test");
                    IncomeBaseService.AccountIncomeHistory(Common.RowCount);
                //}
            }
            catch (Exception ex)
            {
                //WriteText("History test Exception");
                Common.SendFailedMail(Common.FailedSubject, ex.Message + "\n" + ex);
            }
            finally
            {
                isRuningHistory = false;
            }
        }

        protected void EffectTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Common.JudgeRunTime(Common.EffectRunHour))
                return;

            if (isRuningEffect) return;

            isRuningEffect = true;

            try
            {
                string hmNow = DateTime.Now.ToString("HH:mm:ss");
                if (hmNow == Common.EffectStartTime)
                {
                    //WriteText("Effect test");
                    IncomeBaseService.DisposeEffect(Common.RowCount);
                }
            }
            catch (Exception ex)
            {
                //WriteText("Effect test Exception");
                Common.SendFailedMail(Common.FailedSubject, ex.Message + "\n" + ex);
            }
            finally
            {
                isRuningEffect = false;
            }
        }

        protected override void OnStop()
        {
            WriteText(Common.FailedSubject);

            Common.SendFailedMail(Common.FailedSubject, "收入结算费用数据计算服务停止");
        }

        public void WriteText(string tips)
        {
            string path = Application.StartupPath + "/日志";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fs = new FileStream(path + "/" + DateTime.Now.ToString("yyyy年MM月dd") + "logData.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var mStreamWriter = new StreamWriter(fs);
            mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            mStreamWriter.WriteLine(tips);
            mStreamWriter.Flush();
            mStreamWriter.Close();
            fs.Close();
        }
    }
}
