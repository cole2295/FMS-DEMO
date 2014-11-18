using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ServiceForSortingFee
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
                new ServiceForSortingFee() 
            };
            ServiceBase.Run(ServicesToRun);

            //ServiceForSortingFee sortingFeesrv = new ServiceForSortingFee();
            //sortingFeesrv.OnStart();
        }
    }
}
