using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.DAL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IStatusInfoService
    {
        DataTable GetStatusInfoByTypeNo(int typeNo);

        string GetStatusNameByTypeCode(int statusTypeNo, string statusNo);
    }
}
