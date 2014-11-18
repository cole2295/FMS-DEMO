using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IWareHouseService
    {
        DataTable GetWareHouseSortCenter(string distributionCode);

        DataTable GetSortCenter(string distributionCode);

        DataTable GetWareHouse();
    }
}
