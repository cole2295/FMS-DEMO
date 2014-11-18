using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IDistributionDao
    {
        DataTable GetDistribution(Distribution model);

        DataTable GetDistributionCodeByID(string Ids);
    }
}
