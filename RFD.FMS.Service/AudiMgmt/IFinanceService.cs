using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.AudiMgmt
{
    public interface IFinanceService
    {
        void BindOrderSource(System.Web.UI.WebControls.ListControl bindControl);
        System.Collections.Generic.Dictionary<string, System.Data.DataTable> CheckDelayOrders(RFD.FMS.MODEL.BasicSetting.SearchCondition condition, out string exportResult);
        bool ConfirmDailyTotalAmount(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        bool ConfirmDailyTotalAmountV2(RFD.FMS.Model.UnionPay.FMS_StationDailyFinanceSum model);
        void ExportDelayReports(System.Collections.Generic.Dictionary<string, System.Data.DataTable> reports, RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetAllDetailsFinanceData(SearchCondition condidtion);
        DataTable GetAllDetailsFinanceDataV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condidtion);
        DataTable GetDetailsFinanceDailyData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetDetailsFinanceDailyDataV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetDetailsFinanceData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetDetailsFinanceDataV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetDetailsFinanceDataNew(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetDetailsFinanceDataNewV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetSystemWaybillInfo(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTotalFinanceDailyData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTotalFinanceData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTotalFinanceDataV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTotalFinanceDataNew(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTotalFinanceDataNewDis(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTotalFinanceDataNewDisV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTotalFinanceDataNewV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        string GetTotalFinanceDataTip(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        string GetTotalFinanceDataTipV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        string GetTotalFinanceDataTipNew(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        string GetTotalFinanceDataTipNewV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTransferFinanceDetailData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTransferFinanceDetailDataV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTransferFinanceSumData(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        DataTable GetTransferFinanceSumDataV2(RFD.FMS.MODEL.BasicSetting.SearchCondition condition);
        bool UpdateMount(string WaybillNO, decimal Mount, int type);

        DataTable GetAccountTotalData(SearchCondition condidtion);

        DataTable GetAccountDetailsData(SearchCondition condidtion);
    }
}
