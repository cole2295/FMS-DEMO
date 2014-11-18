using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.AudiMgmt
{
    public interface IFinanceDao
    {
        System.Data.DataTable GetAllDetailsFinanceData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetAllDetailsFinanceDataNew(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetAllDetailsFinanceDataNewV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetDetailsFinanceDailyData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetDetailsFinanceDailyDataV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetDetailsFinanceData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetDetailsFinanceDataNew(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetDetailsFinanceDataNewV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetSystemWaybillInfo(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetTotalFinanceDailyData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetTotalFinanceData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetTotalFinanceData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition, bool displayTotalCount);
        System.Data.DataTable GetTotalFinanceDataNew(RFD.FMS.MODEL.BasicSetting.SearchCondition condition, bool displayTotalCount);
        System.Data.DataTable GetTotalFinanceDataNewDis(RFD.FMS.MODEL.BasicSetting.SearchCondition condition, bool displayTotalCount);
        System.Data.DataTable GetTotalFinanceDataNewDisV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition, bool displayTotalCount);
        System.Data.DataTable GetTotalFinanceDataNewV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition, bool displayTotalCount);
        System.Data.DataTable GetTransferFinanceDetailData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetTransferFinanceSumData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        System.Data.DataTable GetWaybillListByOrderNo(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        bool UpdateDailyTotalAmount(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        bool UpdateDailyTotalAmountV2(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        bool UpdateWaybillbackstationBackMount(string WaybillNO, decimal NeedBackAmount, decimal FactBackAmount);
        bool UpdateWaybillbackstationMount(string WaybillNO, decimal needamount, decimal factamount);
        bool UpdateWaybillsigninfoBackMount(string WaybillNO, decimal NeedBackAmount, decimal FactBackAmount, decimal ReturnCash);
        bool UpdateWaybillsigninfoMount(string WaybillNO, decimal needamount, decimal factamount);
    }
}
