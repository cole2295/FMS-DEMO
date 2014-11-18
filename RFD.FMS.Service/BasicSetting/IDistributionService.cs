using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IDistributionService
    {
        DataTable GetDistribution(Distribution model );

        string GetDistributionCodeByID(string Ids);
    }
}
