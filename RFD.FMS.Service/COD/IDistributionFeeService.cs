using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.COD;

namespace RFD.FMS.Service.COD
{
    public interface IDistributionFeeService
    {
        /// <summary>
        /// 根据订单号返回配送费
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        DistributionFeeDTO GetistributionFee(DistributionFeeDTO searchModel);
    }
}
