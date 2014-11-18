using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class DistributionService :IDistributionService
    {
        private IDistributionDao distributionDao;
        private IDistributionService OracleService;
        public DataTable GetDistribution(Distribution model)
        {
            if(OracleService !=null)
            {
                return OracleService.GetDistribution(model);
            }
            else
            {
                return distributionDao.GetDistribution(model);
            }
        }


        public string GetDistributionCodeByID(string Ids)
        {
            if(OracleService !=null)
            {
                return OracleService.GetDistributionCodeByID(Ids);
            }
            else
            {
                var ds=distributionDao.GetDistributionCodeByID(Ids);
                string distributionCode = "";
                foreach (DataRow datarow in ds.Rows)
                {
                    distributionCode += "'" + datarow["DistributionCode"].ToString() + "',";
                }

                return distributionCode.Trim(',');
            }
        }
    }
}
