using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IWareHouseDao
    {

        DataTable GetWareHouseSortCenter(string distributionCode);

        DataTable GetWareHouse();

        DataTable GetSortCenter(string distributionCode);
    }
}
