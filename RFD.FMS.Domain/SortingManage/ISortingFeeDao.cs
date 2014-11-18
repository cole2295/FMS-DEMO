using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Domain.SortingManage
{
    public interface ISortingFeeDao
    {
        int AddSortingFee(FMS_SortingFeeModel model);

        DataTable GetSoringFee(FMS_SortingFeeModel model);

        DataTable GetSortingFeeModel(string SortingFeeID);

        int UpdateSortingFee(FMS_SortingFeeModel model);

        int Delete(FMS_SortingFeeModel model);

        DataTable GetSoringFeeWait(FMS_SortingFeeModel model);

        int AddSortingFeeWait(FMS_SortingFeeModel model);

        int UpdateSortingFeeWait(FMS_SortingFeeModel model);

        DataTable GetSmallSortingFeeModel(string SortingFeeID);

        int AuditSortingFee(FMS_SortingFeeModel model);

        int AuditSortingFeeWait(FMS_SortingFeeModel model);

        int DeleteWait(FMS_SortingFeeModel model);

        int BackSortingFee(FMS_SortingFeeModel model);

        int BackSortingFeeWait(FMS_SortingFeeModel model);

        DataTable GetSortingFeeWaitModel(string SortingFeeWaitID);

        DataTable GetSmallSortingFeeWaitModel(string SortingFeeWaitID);

        DataTable GetSoringFeeWait(int rowNum);

        int UpdateSortingFeeWaitForEffect(FMS_SortingFeeModel model);

        int UpdateSortingFeeForEffect(FMS_SortingFeeModel model);

        void AddSortingFeeHis(FMS_SortingFeeModel model);

        bool ExsitSortingFeeWait(FMS_SortingFeeModel model);

        string GetAccountFareByMerchant(int SortingMerchantID, int SortingCenterID, string CityID, int MerchantID, DateTime SDate,int Type);
    }
}
