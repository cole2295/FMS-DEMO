using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class AreaService : IAreaService
    {
        private IAreaDao _areaDao;
    	private IAreaService OracleService;
        #region IAreaService 成员

        public System.Data.DataTable GetAreaList(MODEL.BasicSetting.Area area)
        {
			if (OracleService!=null)
			{
				return OracleService.GetAreaList(area);
			}
            return _areaDao.GetAreaList(area);
        }

        #endregion
    }
}
