using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Service.COD
{
    public interface IAccountOperatorLogService
    {
        bool AddOperatorLogLog(string PK_NO, int createBy, string logText, int logType);

        DataTable GetOperatorLogLog(string pk_No, int logType);
    }
}
