using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ServiceForInComeFeeAcount
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
                new ServiceForInComeFeeAcount() 
            };
            ServiceBase.Run(ServicesToRun);

            //ServiceForInComeFeeAcount service = new ServiceForInComeFeeAcount();
            //service.OnStart();
        }
    }
}
