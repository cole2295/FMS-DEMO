using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class Province : IProvince
    {
        private IProvinceDao _provinceDao;

        /// <summary>
        ///重载构造方法， 实例化数据层
        /// </summary>
        /// <param name="provinceDao">数据层接口</param>
        public Province()
        {
            
        }

        /// <summary>
        /// 获取省份信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetProvinceList()
        {
            return _provinceDao.GetProvinceList();
        }

        /// <summary>
        /// 获取省份字典 add by wangyongc 2011-09-21
        /// </summary>
        /// <returns></returns>
        public DataTable GetProvinceList(string strDistrictID)
        {
            return _provinceDao.GetProvinceList(strDistrictID);
        }

        public string GetDistrictIdByProvinceId(string id)
        {
            return _provinceDao.GetDistrictId(id);
        }

        /// <summary>
        /// 获取省份信息
        /// </summary>
        /// <returns></returns>
        public string GetProvinceID(string name)
        {
            return _provinceDao.GetProvinceID(name);
        }

        /// <summary>
        /// 获取所有有效省市区
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPCA()
        {
            return _provinceDao.GetAllPCA();
        }

        ///<summary>
        ///</summary>
        ///<param name="expressCompanyId"></param>
        ///<returns></returns>
        public DataTable GetCityNoSiteNo(int expressCompanyId)
        {
            return _provinceDao.GetCityNoSiteNo(expressCompanyId);
        }
    }
}
