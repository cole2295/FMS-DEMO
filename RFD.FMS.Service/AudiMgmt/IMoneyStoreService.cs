using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Service.AudiMgmt
{
    public interface IMoneyStoreService
    {
        System.Data.DataTable GetBatchQuery(string strWaybillNOs);
        System.Data.DataTable GetBatchQueryV2(RFD.FMS.MODEL.BasicSetting.SearchCondition searchCondition);
        System.Data.DataTable GetOrderMoneyStoreInfo(System.Collections.Hashtable ht);
        bool UpdateFinancialStatus(string strSignID);
    }
}
