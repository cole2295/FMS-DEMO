using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class MerchantService : IMerchantService
	{
        private IMerchantDao _merchantDao;

		public DataTable GetMerchants(string disCode)
		{
            return _merchantDao.GetMerchants(disCode);
		}

        public DataTable GetAllMerchants(string disCode)
		{
            return _merchantDao.GetAllMerchants(disCode);
		}

        public DataTable GetMerchantsNoExpress(string disCode)
        {
            return _merchantDao.GetMerchantsNoExpress(disCode);
        }

        public string GetMerchantName(long waybillNo)
        {
            return _merchantDao.GetMerchantName(waybillNo);
        }


        public string GetMerchantCategory(int merchantId)
        {
            return _merchantDao.GetMerchantCategory(merchantId);
        }
    }
}
