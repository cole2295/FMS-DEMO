using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.AudiMgmt;
using RFD.FMS.Domain.AudiMgmt;

namespace RFD.FMS.ServiceImpl.AudiMgmt
{
    [DataObject]
    public class BankPosCheckService : IBankPosCheckService
    {
        private IBankPosCheckDao _bankPosCheckDao;

        /// <summary>
        /// 查询出所有的分拣中心
        /// </summary>
        /// <returns></returns>
         public DataTable GetOrderMoneyStoreInfo()
         {
             return _bankPosCheckDao.GetOrderMoneyStoreInfo();
         }

         /// <summary>
        /// 查询需要核对的系统数据
        /// </summary>
        /// <param name="dtBegDate">开始时间</param>
        /// <param name="dtEndDate">结束时间</param>
        /// <param name="strStatioID">配送站</param>
        /// <returns></returns>
         public DataTable GetCheckData(SearchCondition condition)
         {
             return _bankPosCheckDao.GetCheckData(condition);
         }
    }
}
