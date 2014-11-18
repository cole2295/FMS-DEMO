using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.AudiMgmt
{
    public interface IBankPosCheckService
    {
        DataTable GetOrderMoneyStoreInfo();

        DataTable GetCheckData(SearchCondition condition);
    }
}
