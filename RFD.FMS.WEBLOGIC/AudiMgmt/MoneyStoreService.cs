using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Domain.AudiMgmt;
using RFD.FMS.Service.AudiMgmt;

namespace RFD.FMS.WEBLOGIC.AudiMgmt
{
    [DataObject]
    public class MoneyStoreService : IMoneyStoreService
    {
        private IMoneyStoreDao _moneyStoreDao;

        /// <summary>
        /// 通过查询条件得到查询结果
        /// </summary>
        /// <param name="ht">条件列表</param>
        /// <returns></returns>
        public DataTable GetOrderMoneyStoreInfo(Hashtable ht)
        {
            return _moneyStoreDao.GetOrderMoneyStoreInfo(ht);
        }

         public DataTable GetBatchQuery(string strWaybillNOs)
         {
             return _moneyStoreDao.GetBatchQuery(strWaybillNOs);
         }

         public DataTable GetBatchQueryV2(SearchCondition searchCondition)
         {
             return _moneyStoreDao.GetBatchQueryV2(searchCondition);
         }

        /// <summary>
        /// 财务收款确认
        /// </summary>
        /// <param name="strSignID">签收ID</param>
        /// <returns></returns>
        public bool UpdateFinancialStatus(string strSignID)
        {
            return _moneyStoreDao.UpdateFinancialStatus(strSignID);
        }
    }
}
