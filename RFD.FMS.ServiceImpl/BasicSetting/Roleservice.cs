using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
	[DataObject]
    public class Roleservice : IRoleservice
	{
        private IMenuDao _menuDao;
		public DataSet GetMenuListByUserID(string UserID)
		{
            return _menuDao.GetMenuListByUserID(UserID);
		}

       public DataTable GetAllMenus()
        {
            return _menuDao.GetAllMenus();
        }
	}
}