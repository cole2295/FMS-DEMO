using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class DistributionService : IDistributionService
    {
        private IDistributionDao distributionDao;
        public DataTable GetDistribution(Distribution model)
        {
            return distributionDao.GetDistribution(model);
        }


        public string GetDistributionCodeByID(string Ids)
        {
            var ds = distributionDao.GetDistributionCodeByID(Ids);
            string distributionCode = "";
            foreach (DataRow datarow in ds.Rows)
            {
                distributionCode += "'" + datarow["DistributionCode"].ToString() + "',";
            }

            return distributionCode.Trim(',');
        }
    }
}
