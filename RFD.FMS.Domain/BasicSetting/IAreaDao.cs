using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IAreaDao
    {
        /// <summary>
        /// 获取区县信息
        /// </summary>
        /// <param name="area">查询条件</param>
        /// <returns>区县信息dataTable类型</returns>
        DataTable GetAreaList(Area area);
    }
}
