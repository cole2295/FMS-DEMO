using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using System.Data;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
	public class StatusInfoService : IStatusInfoService
	{
		private IStatusInfoDao _statusInfoDao;
		private IStatusInfoService OracleService;

		public DataTable GetStatusInfoByTypeNo(int typeNo)
		{
			if (OracleService != null)
			{
				return OracleService.GetStatusInfoByTypeNo(typeNo);
			}
			return _statusInfoDao.GetStatusInfoByTypeNo(typeNo);
		}

		public string GetStatusNameByTypeCode(int statusTypeNo, string statusNo)
		{
			if (OracleService != null)
			{
				return OracleService.GetStatusNameByTypeCode(statusTypeNo, statusNo);
			}
			return _statusInfoDao.GetStatusNameByTypeCode(statusTypeNo, statusNo);
		}
	}
}
