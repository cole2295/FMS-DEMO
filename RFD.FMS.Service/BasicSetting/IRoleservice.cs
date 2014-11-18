using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IRoleservice
    {
        DataSet GetMenuListByUserID(string UserID);

        DataTable GetAllMenus();
    }
}
