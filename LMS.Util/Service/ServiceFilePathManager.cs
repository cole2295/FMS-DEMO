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
		/// ��ǰ����װ·��
		/// </summary>
		public static string ServiceInstallPath { get; set; }

		/// <summary>
		/// ��ȡ��ǰ����װ·��
		/// </summary>
		/// <param name="ServiceName"></param>
		/// <returns></returns>
		public static void InstallPathInit(string ServiceName)
		{
			string key = SERVICE_REGISTRY_KEY_PREX + ServiceName;
			RegistryKey openSubKey = Registry.LocalMachine.OpenSubKey(key);
			if (openSubKey == null)
			{
				throw new Exception(ServiceName + "û���ҵ���װ·��");
			}
			string path = openSubKey.GetValue("ImagePath").ToString();
			ServiceInstallPath = path.Substring(1, path.LastIndexOf("\\") - 1);
			logger.Error(ServiceName + " ServiceInstallPath:" + ServiceInstallPath);
		}
	}
}