using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class WareHouseService : IWareHouseService
	{
		public WareHouseService()
		{

		}

        private IWareHouseDao _wareHouseDao;

        public DataTable GetWareHouseSortCenter(string distributionCode)
		{
            return _wareHouseDao.GetWareHouseSortCenter(distributionCode);
		}

        public DataTable GetSortCenter(string distributionCode)
		{
            return _wareHouseDao.GetSortCenter(distributionCode);
		}

		public DataTable GetWareHouse()
		{
            return _wareHouseDao.GetWareHouse();
		}
	}
}
