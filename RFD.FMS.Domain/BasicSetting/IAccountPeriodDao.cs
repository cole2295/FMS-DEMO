using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;
using System.Data;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IAccountPeriodDao
    {
        bool AddAccountPeriod(AccountPeriod ap);

        DataTable SearchAccountPeriod(AccountPeriodCondition apc);

        bool UpdateAccountPeriod(AccountPeriod ap);
    }
}
