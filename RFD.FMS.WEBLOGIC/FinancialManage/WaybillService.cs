using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.FinancialManage;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class WaybillService : IWaybillService
    {
        private IWaybillDao _waybillDao;
        public DataTable GetThirdWaybillDetails(ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref PageInfo pi, out DataTable amount)
        {
            if (conditions.IsQucikQuery)
            {
                return _waybillDao.GetThirdWaybillDetailsV2(conditions, pageOrNo, ref  pi, out amount);
            }
            return _waybillDao.GetThirdWaybillDetails(conditions, pageOrNo, ref  pi, out amount);
        }

        public DataTable GetThirdWaybillStat(ThirdPartyWaybillSearchConditons conditions, out DataTable amount)
        {
            amount = _waybillDao.StatisticsForDetails(conditions);
            if (amount == null || amount.Rows.Count <= 0)
            {
                return null;
            }

            return _waybillDao.GetThirdWaybillStat(conditions);
        }
    }
}
