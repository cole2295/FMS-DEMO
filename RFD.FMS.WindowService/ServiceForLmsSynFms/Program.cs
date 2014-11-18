using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ServiceForLmsSynFms
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ServiceForLmsSynFms() 
            };
            ServiceBase.Run(ServicesToRun);

            //ServiceForLmsSynFms s = new ServiceForLmsSynFms();
            //s.OnStart();

            //var test = new unitTest();
            //test.SCMToLMS();
            //while (true)
            //{
            //    Console.Write(DateTime.Now);
            //}
        }

    }
}
