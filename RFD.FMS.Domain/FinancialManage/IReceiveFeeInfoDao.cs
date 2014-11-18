using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Model.FinancialManage;
using System.Data;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IReceiveFeeInfoDao
    {
        FMS_IncomeReceiveFeeInfo GetProjectSendFeeInfoModel(long waybillno);

        FMS_IncomeExpressReceiveFeeInfo GetExpressReceiveFeeInfoModel(long waybillno);

        FMS_IncomeExpressReceiveFeeInfo GetExpressSendFeeInfoModel(long waybillno);

        DataTable GetSynInfoByWaybillNO(IList<long> waybillNos);

        string GetCurrentData(long waybillNo);

        FMS_IncomeExpressReceiveFeeInfo GetExpressSendMonthlyModel(long waybillno);
        FMS_IncomeExpressReceiveFeeInfo GetExpressModel(long waybillno);

    }
}
