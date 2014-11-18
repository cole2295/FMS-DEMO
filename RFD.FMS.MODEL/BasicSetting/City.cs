using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Model
{
    /*
 * (C)Copyright 2011-2012 如风达信息管理系统
 * 
 * 模块名称：城市字典实体类
 * 说明：城市实体对应数据库City表
 * 作者：杨来旺
 * 创建日期：2011-02-30 13:42:00
 * 修改人：
 * 修改时间：
 * 修改记录：
 */
    public class City
    {
        /// <summary>
        /// 城市ID
        /// </summary>
        public string CityID { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary>
        public string ProvinceID { get; set; }
        /// <summary>
        /// 所属分检中心
        /// </summary>
        public int ExpressCompanyID { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 城市级别
        /// </summary>
        public string Level { get; set; }
    }
}
