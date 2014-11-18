using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IProvince
    {
        /// <summary>
        /// 获取省份信息
        /// </summary>
        /// <returns></returns>
        DataTable GetProvinceList();

        DataTable GetProvinceList(string strDistrictID);

        string GetDistrictIdByProvinceId(string id);

        // <summary>
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
