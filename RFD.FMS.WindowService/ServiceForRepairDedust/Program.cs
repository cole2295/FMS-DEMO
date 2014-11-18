using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace ServiceForRepairDedust
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;

            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new ServiceForRepairDedust() 
            //};

            //ServiceBase.Run(ServicesToRun);

            while (true)
            {
                ServiceForRepairDedust deduct = new ServiceForRepairDedust();

                deduct.RepairDedust();
            }
        }
    }
}
