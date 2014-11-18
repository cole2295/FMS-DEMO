using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class AreaService : IAreaService
    {
        private IAreaDao _areaDao;

        #region IAreaService 成员

        public System.Data.DataTable GetAreaList(MODEL.BasicSetting.Area area)
        {
            return _areaDao.GetAreaList(area);
        }

        #endregion
    }
}
