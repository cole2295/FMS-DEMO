using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.COD;
using RFD.FMS.Domain.COD;
using System.Data;

namespace RFD.FMS.WEBLOGIC.COD
{
    public class AccountOperatorLogService : IAccountOperatorLogService
    {
        private IAccountOperatorLogDao _accountOperatorLogDao;
        private IAccountOperatorLogService OracleService;

        public bool AddOperatorLogLog(string PK_NO, int createBy, string logText, int logType)
        {
            if (OracleService != null)
            {
                return OracleService.AddOperatorLogLog(PK_NO, createBy, logText, logType);
            }

            return _accountOperatorLogDao.AddOperatorLogLog(PK_NO, createBy, logText, logType);
        }

        public DataTable GetOperatorLogLog(string pk_No, int logType)
        {
            if (OracleService != null)
            {
                return OracleService.GetOperatorLogLog(pk_No, logType);
            }
            return _accountOperatorLogDao.GetOperatorLogLog(pk_No, logType);
        }
    }
}
