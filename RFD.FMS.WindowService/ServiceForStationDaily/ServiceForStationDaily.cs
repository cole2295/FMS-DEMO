using System;
using System.ServiceProcess;


namespace ServiceForStationDaily
{
    public partial class ServiceForStationDaily : ServiceBase
    {
        public ServiceForStationDaily()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Common.SendFailedMail(Common.FailedSubject, "如风达配送报表数据服务启动");
                Common.WriteTest("如风达配送报表数据服务启动");
                ServiceStart();
            }
            catch (Exception ex)
            {
                Common.WriteTest(ex.Message);
            }
        }

        protected override void OnStop()
        {
            Common.SendFailedMail(Common.FailedSubject, "如风达配送报表数据服务停止");
            Common.WriteTest("如风达配送报表数据服务停止");
        }

        public void ServiceStart()
        {
            var jobManager = new JobManager();
            jobManager.OnStart();
        }
    }
}