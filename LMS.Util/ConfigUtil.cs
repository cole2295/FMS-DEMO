using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace RFD.FMS.Util
{
    /// <summary>
    /// 配置工具类
    /// </summary>
    public static class ConfigUtil
    {
        /// <summary>
        /// 根据配置节名称获取对应的值
        /// </summary>
        /// <typeparam name="T">获取值的数据类型</typeparam>
        /// <param name="configKey">配置节名称</param>
        /// <param name="defaultValue">未找到配置节则返回默认值</param>
        /// <returns>返回配置节对应的值</returns>
        public static T GetConfigValue<T>(string configKey, T defaultValue)
        {
            var config = ConfigurationManager.AppSettings[configKey];
            if (config.IsNullData())
            {
                return defaultValue;
            }
            return DataConvert.ToValue<T>(config);
        }
        /// <summary>
        /// 根据配置节名称获取对应的连接字符串的值
        /// </summary>
        /// <param name="configKey">配置节名称</param>
        /// <returns>连接字符串的值</returns>
        public static string GetConnectionString(string configKey)
        {
            var config = ConfigurationManager.ConnectionStrings[configKey];
            if (config.IsNullData())
            {
                return String.Empty;
            }
            return config.ConnectionString.Trim();
        }
    }
}
