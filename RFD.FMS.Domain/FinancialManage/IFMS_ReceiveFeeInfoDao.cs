using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.MODEL.BasicSetting;
using System.Data;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IFMS_ReceiveFeeInfoDao
    {
        bool ExistsExpressReceiveFeeInfo(long waybillNo);

        bool AddExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel);

        bool UpdateExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel);

        bool DeleteExpressFeeInfo(long waybillNo);

        bool ExistsProjectReceiveFeeInfo(long waybillNo);

        bool UpdateProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel);

        bool InsertProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel);

        bool DeleteProjectReceiveFeeInfo(long waybillNo);

        DataTable GetTransferFinanceSumDataV2(SearchCondition condition);

        DataTable GetTransferFinanceDetailDataV2(SearchCondition condition);

        DataTable GetAllDetailsFinanceDataV2(SearchCondition condition);

        DataTable GetDetailsFinanceDataV2(SearchCondition condition);

        DataTable GetTotalFinanceDataV2(SearchCondition condition, bool displayTotalCount);

        void UpdateSynInfo(long waybillNo,string acceptType);

        DataTable GetNullAcceptTypeValues();

        DataTable GetRepairData();

        void UpdateRepairData(long waybillNo, string acceptType);

        DataTable GetErrorBackStationTimeData();

        void UpdateBackStationTime(long waybillNo,DateTime backStationTime);
    }
}
