using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Service.FinancialManage
{
    [ServiceContract]
    public interface IFMSSortingForLMSService
    {
        [OperationContract]
        int AddSortingTransfer(FMS_SortingTransferDetail model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waybillno"></param>
        /// <returns></returns>
        [OperationContract]
        bool ExistFMS_SortingTransferDetailByNo(Int64 waybillno);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waybillno"></param>
        /// <returns></returns>
        [OperationContract]
        string ExistOutBound(FMS_SortingTransferDetail model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waybillno"></param>
        /// <returns></returns>
        [OperationContract]
        string ExistIntoStation(FMS_SortingTransferDetail model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateFMS_SortingToCity(FMS_SortingTransferDetail model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateFMS_SortingToStation(FMS_SortingTransferDetail model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateFMS_ReturnToSortingCenter(FMS_SortingTransferDetail model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateFMS_MerchantToSortingCenter(FMS_SortingTransferDetail model);
    }
}
