using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Model
{
    /*
* (C)Copyright 2011-2012 如风达信息管理系统
* 
* 模块名称：省份字典实体类
* 说明：省份字典实体类
* 作者：杨来旺
* 创建日期：2011-02-30 13:42:00
* 修改人：
* 修改时间：
* 修改记录：
*/
    public class Province
    {
        /// <summary>
        /// 省份ID
        /// </summary>
        public string ProvinceID { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string DistrictID { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
