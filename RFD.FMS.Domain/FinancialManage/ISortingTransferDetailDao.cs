using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface ISortingTransferDetailDao
    {
        int Add(FMS_SortingTransferDetail model);

        bool ExistFMS_SortingTransferDetailByNo(Int64 waybillno);

        string ExistOutBound(FMS_SortingTransferDetail model);

        string ExistIntoStation(FMS_SortingTransferDetail model);

        string ExsitInSorting(FMS_SortingTransferDetail model);

        DataTable GetTableByNo(long waybillno);

        bool UpdateFMS_SortingToCity(FMS_SortingTransferDetail model);

        bool UpdateFMS_SortingToStation(FMS_SortingTransferDetail model);

        bool UpdateFMS_ReturnToSortingCenter(FMS_SortingTransferDetail model);

        bool UpdateFMS_MerchantToSortingCenter(FMS_SortingTransferDetail model);

        DataTable SortingTransferAndToStationDetail(SortingDetail Model);
        DataTable SortingToCityDetail(SortingDetail Model);
        DataTable ReturnToSortingCenterDetail(SortingDetail Model);
        DataTable MerchantToSortingCenterDetail(SortingDetail Model);
        DataTable ReverseMerchantToSortingCenterDetail(SortingDetail Model);
        //MethodTest

        //DataTable GetAllSortingData(Int64 waybillNo);
        //DataTable GetSortingToCity(long waybillNo);

        int CountSortingTransferAndStationDetail(SortingDetail Model);

        int CountSortingToCityDetail(SortingDetail Model);

        int CountReturnToSortingCenterDetail(SortingDetail Model);

        int CountMerchantToSortingCenterDetail(SortingDetail Model);

        DataTable ExportSortingTransferAndToStationDetail(SortingDetail Model);

        DataTable ExportSortingToCityDetail(SortingDetail Model);

        DataTable ExportReturnToSortingCenterDetail(SortingDetail Model);

        DataTable ExportMerchantToSortingCenterDetail(SortingDetail Model);
    }
}
