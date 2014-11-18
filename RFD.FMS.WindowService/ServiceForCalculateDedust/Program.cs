using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using RFD.FMS.DAL.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;

namespace ServiceForCalculateDedust
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
                new ServiceForCalculateDedust() 
            };

            ServiceBase.Run(ServicesToRun);

            //ServiceForCalculateDedust service = new ServiceForCalculateDedust();
            //service.CalculateDedust();


            //var deductService = ServiceLocator.GetService<IDeductService>();
            //deductService.DealExpressSendByWaybillNO(9120948141674);

            //var deductService = ServiceLocator.GetService<IDeductService>();
            //deductService.DealProjectSendByWaybillNO(213052496656);
        }
    }
}
