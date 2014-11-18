using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IExpressPriceDao
    {
        DataTable GetAreaInfo(string strCityID, string strCityName);
    }
}
