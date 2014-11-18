using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ServiceForAreaExpressLevel
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
                new ServiceForAreaExpressLevel() 
            };
            ServiceBase.Run(ServicesToRun);

            //ServiceForAreaExpressLevel areaExpressLevel = new ServiceForAreaExpressLevel();
            //areaExpressLevel.OnStart();
        }
    }
}
