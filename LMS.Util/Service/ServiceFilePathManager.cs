using System;
using Common.Logging;
using Microsoft.Win32;

namespace RFD.FMS.Util.Service
{
	public class ServiceFilePathManager
	{
		private const string SERVICE_REGISTRY_KEY_PREX = "SYSTEM\\CurrentControlSet\\Services\\";

		private static readonly ILog logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 当前服务安装路径
		/// </summary>
		public static string ServiceInstallPath { get; set; }

		/// <summary>
		/// 获取当前服务安装路径
		/// </summary>
		/// <param name="ServiceName"></param>
		/// <returns></returns>
		public static void InstallPathInit(string ServiceName)
		{
			string key = SERVICE_REGISTRY_KEY_PREX + ServiceName;
			RegistryKey openSubKey = Registry.LocalMachine.OpenSubKey(key);
			if (openSubKey == null)
			{
				throw new Exception(ServiceName + "没有找到安装路径");
			}
			string path = openSubKey.GetValue("ImagePath").ToString();
			ServiceInstallPath = path.Substring(1, path.LastIndexOf("\\") - 1);
			logger.Error(ServiceName + " ServiceInstallPath:" + ServiceInstallPath);
		}
	}
}