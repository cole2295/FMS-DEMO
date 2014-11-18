using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using System.Data;

namespace RFD.FMS.DAL.BasicSetting
{
    public class AccountPeriodDao : SqlServerDao,IAccountPeriodDao
    {
        public bool AddAccountPeriod(AccountPeriod ap)
        {
            throw new Exception("sql没有实现");
        }

        public DataTable SearchAccountPeriod(AccountPeriodCondition apc)
        {
            throw new Exception("sql没有实现");
        }

        public bool UpdateAccountPeriod(AccountPeriod ap)
        {
            throw new Exception("sql没有实现");
        }
    }
}
