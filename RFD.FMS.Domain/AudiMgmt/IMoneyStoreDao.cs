using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;
using System.Collections;
using System.Data;

namespace RFD.FMS.Domain.AudiMgmt
{
    public interface IMoneyStoreDao
    {
        DataTable GetOrderMoneyStoreInfo(Hashtable ht);

        DataTable GetBatchQuery(string strWaybillNOs);

        DataTable GetBatchQueryV2(SearchCondition searchCondition);

        bool UpdateFinancialStatus(string strSignID);
    }
}
