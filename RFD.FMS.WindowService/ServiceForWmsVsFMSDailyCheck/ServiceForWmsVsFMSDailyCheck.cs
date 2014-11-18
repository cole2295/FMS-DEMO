using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;

namespace ServiceForWmsVsFMSDailyCheck
{
    public partial class ServiceForWmsVsFMSDailyCheck : ServiceBase
    {
        public ServiceForWmsVsFMSDailyCheck()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
       // public void OnStart()
        {
            try
            {
               
                Common.SendFailedMail(Common.FailedSubject, "凡客如风达财务核对服务启动");

                ServiceStart();
            }
            catch (Exception ex)
            {
                Common.WriteTest(DateTime.Now.ToString() + ": 启动异常" + "异常消息:"+ex.Message + "异常回溯:"+ ex.StackTrace
                    +"..."+ex.InnerException);
            }
        }

        protected override void OnStop()
        {
            Common.SendFailedMail(Common.FailedSubject, "凡客如风达财务核对服务停止");
        }

        public void ServiceStart()
        {
            var jobManager = new JobManager();

            jobManager.OnStart();
        }
    }
}
