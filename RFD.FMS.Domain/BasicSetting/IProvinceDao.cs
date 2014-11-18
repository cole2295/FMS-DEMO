using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IProvinceDao
    {
        /// <summary>
        /// 获取省份字典
        /// </summary>
        /// <returns></returns>
        DataTable GetProvinceList();

        DataTable GetProvinceList(string strDistrictID);
        /// <summary>
        /// 获取省份字典
        /// </summary>
        /// <returns></returns>
        string GetProvinceID(string Name);

        string GetDistrictId(string ProvinceID);

        /// <summary>
        /// 获取所有有效省市区
        /// </summary>
        /// <returns></returns>
        DataTable GetAllPCA();

        ///<summary>
        ///</summary>
        ///<param name="expressCompanyId"></param>
        ///<returns></returns>
        DataTable GetCityNoSiteNo(int expressCompanyId);
    }
}
