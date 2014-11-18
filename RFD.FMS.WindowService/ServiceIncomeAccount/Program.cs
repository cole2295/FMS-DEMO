using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ServiceIncomeAccount
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
                new ServiceIncomeAccount() 
            };

            ServiceBase.Run(ServicesToRun);

            #region 调试
            //ServiceIncomeAccount incomeAccount = new ServiceIncomeAccount();
            //incomeAccount.TimerElapsed();
            #endregion
        }
    }
}
