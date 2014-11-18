using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IStatusInfoDao
    {
        DataTable GetStatusInfoByTypeNo(int statusTypeNo);

        string GetStatusNameByTypeCode(int statusTypeNo, string statusNo);
    }
}
