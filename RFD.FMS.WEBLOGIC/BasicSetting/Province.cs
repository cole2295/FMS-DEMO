using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class Province : IProvince
    {
        private IProvinceDao _provinceDao;
    	private IProvince OracleService;
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
			if (OracleService!=null)
			{
			  return OracleService.GetProvinceList();
			}
            return _provinceDao.GetProvinceList();
        }

        /// <summary>
        /// 获取省份字典 add by wangyongc 2011-09-21
        /// </summary>
        /// <returns></returns>
        public DataTable GetProvinceList(string strDistrictID)
        {
			if (OracleService != null)
			{
				return OracleService.GetProvinceList(strDistrictID);
			}
        	return _provinceDao.GetProvinceList(strDistrictID);
        }

        public string GetDistrictIdByProvinceId(string id)
        {
			if (OracleService != null)
			{
				return GetDistrictIdByProvinceId(id);
			}
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
			if (OracleService!=null)
			{
			  return OracleService.GetAllPCA();
			}
            return _provinceDao.GetAllPCA();
        }

        ///<summary>
        ///</summary>
        ///<param name="expressCompanyId"></param>
        ///<returns></returns>
        public DataTable GetCityNoSiteNo(int expressCompanyId)
        {
        	if (OracleService!=null)
        	{
        		return OracleService.GetCityNoSiteNo(expressCompanyId);
        	}
            return _provinceDao.GetCityNoSiteNo(expressCompanyId);
        }
    }
}
