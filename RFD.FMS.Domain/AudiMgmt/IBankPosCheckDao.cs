using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.AudiMgmt
{
    public interface IBankPosCheckDao
    {
        DataTable GetOrderMoneyStoreInfo();

        DataTable GetCheckData(SearchCondition condition);
    }
}
