using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Service.FinancialManage
{
    public interface ISortingTransferSearchingService
    {
        DataTable SortingTransferAndToStationDetail(SortingDetail Model);
        DataTable SortingToCityDetail(SortingDetail Model);
        DataTable ReturnToSortingCenterDetail(SortingDetail Model);
        DataTable MerchantToSortingCenterDetail(SortingDetail Model);

        DataTable SortingTransferAndToStationDaily(SortingDetail Model);
        DataTable SortingToCityDaily(SortingDetail Model);
        DataTable ReturnToSortingCenterDaily(SortingDetail Model);
        DataTable MerchantToSortingCenterDaily(SortingDetail Model);


        DataTable SortingTransferAndToStationMerchant(SortingDetail Model);
        DataTable SortingToCityMerchant(SortingDetail Model);
        DataTable ReturnToSortingCenterMerchant(SortingDetail Model);
        DataTable MerchantToSortingCenterMerchant(SortingDetail Model);

        DataTable SortingTransferAndToStationAll(SortingDetail Model);
        DataTable SortingToCityAll(SortingDetail Model);
        DataTable ReturnToSortingCenterAll(SortingDetail Model);
        DataTable MerchantToSortingCenterAll(SortingDetail Model);
    }
}
