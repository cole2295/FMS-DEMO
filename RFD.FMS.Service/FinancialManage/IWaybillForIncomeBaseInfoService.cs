using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IWaybillForIncomeBaseInfoService
    {
        void AcountDeductFeeInfoDao(int Num);

        RFD.FMS.Model.FinancialManage.FMS_IncomeBaseInfo GetBaseInfoModel(System.Data.DataRow row);

        RFD.FMS.Model.FinancialManage.FMS_IncomeFeeInfo GetFeeInfoModel(System.Data.DataRow row);

        DataTable SearchDetails(ThirdPartyWaybillSearchConditons condition, ref PageInfo pi, out DataTable amount);

        DataTable SearchDetailsForExport(ThirdPartyWaybillSearchConditons condition);

        DataTable SearchDetailsForExportV2(ThirdPartyWaybillSearchConditons condition);

        DataTable SearchDetailsV2(ThirdPartyWaybillSearchConditons condition, ref PageInfo pi, out DataTable amount);

        DataTable SearchSummary(ThirdPartyWaybillSearchConditons condition, out DataTable amount);

        DataTable SearchSummaryForExport(ThirdPartyWaybillSearchConditons condition);

        void UpdateIncomeFeeInfoDao(List<string> WaybollNOList);

        void UpdateIncomeFeeInfoDao(int Num);

        string GetIncomeErrorLog();

        void ClearIncomeIsAccount(int rowCount);

        void AccountIncomeHistory(int rowCount);

        void DisposeEffect(int rowCount);
    }
}
