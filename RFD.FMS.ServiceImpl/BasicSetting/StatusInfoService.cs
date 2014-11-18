using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using System.Data;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class StatusInfoService : IStatusInfoService
    {
        private IStatusInfoDao _statusInfoDao;

        public DataTable GetStatusInfoByTypeNo(int typeNo)
        {
            return _statusInfoDao.GetStatusInfoByTypeNo(typeNo);
        }

        public string GetStatusNameByTypeCode(int statusTypeNo, string statusNo)
        {
            return _statusInfoDao.GetStatusNameByTypeCode(statusTypeNo, statusNo);
        }
    }
}
