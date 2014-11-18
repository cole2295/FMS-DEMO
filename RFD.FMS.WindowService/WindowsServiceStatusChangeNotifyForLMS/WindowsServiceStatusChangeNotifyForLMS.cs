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
using WindowsServiceStatusChangeNotifyForLMS;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace WindowsServiceStatusChangeNotifyForFMS
{
    /*
     * (C)Copyright 2011-2012 如风达信息管理系统
     * 
     * 模块名称：运单状态变更通知
     * 说明：
     * 作者：高朋祥
     * 创建日期：2012-04-08 10:19:00
     * 修改人：
     * 修改时间：
     * 修改记录：
     */
    public partial class WindowsServiceStatusChangeNotifyForFMS : ServiceBase
    {
        public WindowsServiceStatusChangeNotifyForFMS()
        {
            InitializeComponent();
        }

        readonly IWaybillStatusDeal service = ServiceLocator.GetService<IWaybillStatusDeal>();

        /// <summary>
        /// 定义时间
        /// </summary>
        private Timer _time;

        private static bool isRuning = false;

        protected override void OnStart(string[] args)
        {
            Common.SendFailedMail(Common.SuccedSubject, "服务已启动");

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
                Common.SendFailedMail(Common.FailedSubject, ex.Message + ex.Source + ex.StackTrace);

                return;
            }
        }

        /// <summary>
        /// 服务停止
        /// </summary>
        protected override void OnStop()
        {
            Common.SendFailedMail(Common.SuccedSubject, "服务停止");
        }

        /// <summary>
        /// 业务处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (isRuning)
            {
                return;
            }

            isRuning = true;

            try
            {
                service.DoDeal();
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }

            isRuning = false;
        }
    }
}
