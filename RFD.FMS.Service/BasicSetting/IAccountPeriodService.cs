using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IAccountPeriodService
    {
        bool AddAccountPeriod(AccountPeriod ap);

        List<AccountPeriod> SearchAccountPeriod(AccountPeriodCondition apc);

        bool UpdateAccountPeriod(AccountPeriod ap);

        string ImitatePeriod(AccountPeriod ap);

        void GetImitatePeriod(ref AccountPeriod ap);
    }
}
