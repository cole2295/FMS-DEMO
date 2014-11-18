using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using RFD.FMS.MODEL.COD;

namespace RFD.FMS.Service.COD
{
    [ServiceContract]
    public interface IDistributionFeeContractService
    {
        /// <summary>
        /// 根据订单号返回配送费
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [OperationContract]
        DistributionFeeDTO GetistributionFee(DistributionFeeDTO searchModel);
    }
}
