using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.DAL.Oracle.FinancialManage;

namespace WindowsServiceStatusChangeNotifyForFMS
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
                new WindowsServiceStatusChangeNotifyForFMS() 
            };

            ServiceBase.Run(ServicesToRun);
        }

        //static void Main()
        //{
        //    //while (true)
        //    //{
        //        var service = ServiceLocator.GetService<IWaybillStatusDeal>();

        //        service.DoDeal();
        //    //}
        //}
    }
}
