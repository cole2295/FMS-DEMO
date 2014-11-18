using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IAreaService
    {
        /// <summary>
        /// 查询区县信息
        /// </summary>
        /// <param name="area">查询条件</param>
        /// <returns>区县信息DataTable类型</returns>
        DataTable GetAreaList(Area area);

    }
}
