using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.COD;

namespace RFD.FMS.Domain.COD
{
    public interface IDistributionFeeDao 
    {
        DataTable GetFare(DistributionFeeDTO dto);

        DataTable GetFareV2(DistributionFeeDTO dto);
    }
}
