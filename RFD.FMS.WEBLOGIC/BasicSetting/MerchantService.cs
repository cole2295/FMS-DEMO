using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class MerchantService : IMerchantService
	{
        private IMerchantDao _merchantDao;
        private IMerchantService OracleService;

		public DataTable GetMerchants(string disCode)
		{
           if(OracleService !=null)
           {
             return  OracleService.GetMerchants(disCode);
           }
            return _merchantDao.GetMerchants(disCode);
		}

        public DataTable GetAllMerchants(string disCode)
		{
            if (OracleService !=null)
            {
              return  OracleService.GetAllMerchants(disCode);
            }
            return _merchantDao.GetAllMerchants(disCode);
		}

        public DataTable GetMerchantsNoExpress(string disCode)
        {
            if(OracleService != null)
            {
               return OracleService.GetMerchantsNoExpress(disCode);
            }
            return _merchantDao.GetMerchantsNoExpress(disCode);
        }

        public string GetMerchantName(long waybillNo)
        {
            if(OracleService !=null)
            {
               return OracleService.GetMerchantName(waybillNo);
            }
            return _merchantDao.GetMerchantName(waybillNo);
        }


        public string GetMerchantCategory(int merchantId)
        {
            if(OracleService !=null)
            {
              return  OracleService.GetMerchantCategory(merchantId);
            }
            return _merchantDao.GetMerchantCategory(merchantId);
        }
    }
}
