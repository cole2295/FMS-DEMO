using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Util;

namespace RFD.FMS.ServiceImpl
{
    public class LogService
    {
        private static LogHelper log = new LogHelper("应用程序异常");

        public static LogHelper Instance
        {
            get { return log; }
        }
    }
}
