using System.ServiceProcess;
using Quartz;

namespace ServiceForWmsVsFMSDailyCheck
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        private static void Main()
       {
           ServiceBase[] ServicesToRun;

           ServicesToRun = new ServiceBase[]
            {
                new ServiceForWmsVsFMSDailyCheck()
            };

           ServiceBase.Run(ServicesToRun);

           //JobForWmsVsFMSDailyCheck job = new JobForWmsVsFMSDailyCheck();

           //for (int i = 0; i < 1; i++)
           //{
           //    job.Execute(null);
           //}

           //ServiceForWmsVsFMSDailyCheck service = new ServiceForWmsVsFMSDailyCheck();
           //service.OnStart();
        }
    }
}