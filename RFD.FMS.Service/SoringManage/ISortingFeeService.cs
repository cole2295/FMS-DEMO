using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Service.SoringManage
{
    public interface ISortingFeeService
    {
        int AddSortingFee(FMS_SortingFeeModel model);

        DataTable GetSortingFee(FMS_SortingFeeModel model);

        DataTable GetSortingFeeModel(string SortingFeeID);

        int UpdateSortingFee(FMS_SortingFeeModel model);

        int Delete(FMS_SortingFeeModel model);

        DataTable GetSortingFeeWait(FMS_SortingFeeModel model);

        int AddSortingFeeWait(FMS_SortingFeeModel model);

        int UpdateSortingFeeWait(FMS_SortingFeeModel model);

        FMS_SortingFeeModel GetSmallSortingFeeModel(string SortingFeeID);

        int AuditSortingFee(FMS_SortingFeeModel model);

        int AuditSortingFeeWait(FMS_SortingFeeModel model);

        int BackSortingFee(FMS_SortingFeeModel model);

        int BackSortingFeeWait(FMS_SortingFeeModel model);

        DataTable GetSortingFeeWaitModel(string ID);

        FMS_SortingFeeModel GetSmallSortingFeeWaitModel(string SortingFeeWaitID);

        string Dispposed(int rowNum);

        bool ExsitSortingFeeWait(FMS_SortingFeeModel model);

        string GetAccountFareByMerchant(int SortingMerchantID, int SortingCenterID, string CityID, int MerchantID,DateTime SDate,int Type);
    }
}
