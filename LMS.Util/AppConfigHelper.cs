using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;

namespace RFD.FMS.Util
{
    public class AppConfigHelper
    {
        public static string AppSettings(string key)
        {
            try
            {
                return DataConvert.ToString(ConfigurationManager.AppSettings[key]);
            }
            catch
            {
                throw new Exception("Key为" + key + "的AppSettings没有设置!");
            }
        }

        public static string TryGetAppSettings(string key)
        {
            try
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                    return ConfigurationManager.AppSettings[key].ToString();
                return string.Empty;
            }
            catch { return string.Empty; }
        }

        public static string _ipAddress;
        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            try
            {
                if (string.IsNullOrEmpty(_ipAddress))
                {
                    var hostname = Dns.GetHostName();
                    var localhost = Dns.GetHostByName(hostname);
                    var localaddr = localhost.AddressList[0]; //localaddr中就是本机ip地址
                    _ipAddress = localaddr.ToString();
                }
                return _ipAddress;
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        }
    }
}
