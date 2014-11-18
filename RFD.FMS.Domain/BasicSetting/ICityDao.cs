using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Model;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface ICityDao
    {
        /// <summary>
        /// 查询城市信息
        /// </summary>
        /// <param name="city">查询条件</param>
        /// <returns>城市信息DataTable类型</returns>
        DataTable GetCityList(City city);
    }
}
