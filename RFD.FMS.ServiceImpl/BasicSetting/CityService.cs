using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using RFD.FMS.Model;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class CityService : ICityService
    {
        /// <summary>
        /// 实例化数据层
        /// </summary>
        private ICityDao _cityDao;
        public CityService()
        {
            
        }

        /// <summary>
        /// 查询城市信息
        /// </summary>
        /// <param name="city">查询条件</param>
        /// <returns>城市信息DataTable类型</returns>
        public DataTable GetCityList(City city)
        {
            return _cityDao.GetCityList(city);
        }
    }
}
