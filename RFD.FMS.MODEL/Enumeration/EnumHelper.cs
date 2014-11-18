/*
* (C)Copyright 2011-2012 如风达信息管理系统
* 
* 模块名称：枚举工具类
* 说明：根据当前的枚举进行相应的操作
* 作者：何名宇
* 创建日期：2011-07-26
* 修改人：
* 修改时间：
* 修改记录：
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace RFD.FMS.MODEL.Enumeration
{
    public static partial class EnumHelper
    {
        /// <summary>
        /// 获取枚举变量的描述信息
        /// </summary>
        /// <param name="e">枚举变量</param>
        /// <returns></returns>
        public static String GetEnumDesc(Enum e)
        {
            FieldInfo EnumInfo = e.GetType().GetField(e.ToString());
            DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.
            GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (EnumAttributes.Length > 0)
            {
                return EnumAttributes[0].Description;
            }
            return e.ToString();
        }
    }
}
