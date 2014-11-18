using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Common.Logging;
using System.Configuration;
using RFD.FMS.Util;
using RFD.FMS.Service.FinancialManage;

namespace ServiceForCalculateDedust
{
    public partial class ServiceForCalculateDedust : ServiceBase
    {
        private static bool isRuningExpend = false;
        public ServiceForCalculateDedust()
        {
            InitializeComponent();
        }

        private Timer _time;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceForCalculateDedust));

        //public void OnStart()
        protected override void OnStart(string[] args)
        {
            Logger.Info("配送员提成计算服务启动");

            Common.SendFailedMail(Common.FailedSubject, "配送员提成计算服务启动");

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
            Common.SendFailedMail(Common.FailedSubject, "配送员提成计算服务停止");

            Logger.Info("配送员提成计算服务停止");
        }

        /// <summary>
        /// 业务处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (isRuningExpend) return;
            isRuningExpend = true;

            CalculateDedust();
        }

        public void CalculateDedust()
        {
            var deductService = ServiceLocator.GetService<IDeductService>();

            try
            {
                deductService.DealDetail();
            }
            catch (Exception ex)
            {
                Common.SendFailedMail("配送员提成计算服务异常", ex.Message);

                return;
            }
        }
    }
}