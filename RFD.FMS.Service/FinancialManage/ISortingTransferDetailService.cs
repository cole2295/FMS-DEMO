using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Service.FinancialManage
{
   public interface ISortingTransferDetailService
    {
        DataTable SortingTransferAndToStationDetail(SortingDetail Model);
        DataTable SortingToCityDetail(SortingDetail Model);
        DataTable ReturnToSortingCenterDetail(SortingDetail Model);
        DataTable MerchantToSortingCenterDetail(SortingDetail Model);
       int CountSortingTransferAndToStationDetail(SortingDetail Model);
       int CountSortingToCityDetail(SortingDetail Model);
       int CountReturnToSortingCenterDetail(SortingDetail Model);
       int CountMerchantToSortingCenterDetail(SortingDetail Model);

       DataTable ExportSortingTransferAndToStationDetail(SortingDetail Model);
       DataTable ExportSortingToCityDetail(SortingDetail Model);
       DataTable ExportReturnToSortingCenterDetail(SortingDetail Model);
       DataTable ExportMerchantToSortingCenterDetail(SortingDetail Model);

    }
}
