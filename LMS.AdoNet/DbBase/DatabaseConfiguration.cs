using System.Collections.Generic;
using System.Configuration;
using Common.Logging;
using RFD.FMS.Util.Service;

namespace RFD.FMS.AdoNet.DbBase
{
	public abstract class DatabaseConfiguration : IDatabaseConfiguration
	{
		protected const string DatabaseTagName = "Database";
		protected const string EnumValue = "EnumValue";
		protected const string DatabaseName = "DatabaseName";
		protected const string WarehouseId = "WarehouseId";
		protected const string WebCookieValue = "WebCookieValue";
		protected const string ConnectionString = "ConnectionString";
		protected const string DbType = "DbType";
		protected const string CompanySource = "CompanySource";
		protected readonly ILog Logger = LogManager.GetCurrentClassLogger();
		protected string DbConfigFileName;

		protected DatabaseConfiguration()
		{
#if DEBUG
			bool isUnitDebug; //是否单元测试
			bool.TryParse(ConfigurationManager.AppSettings["IsUnitDebug"].Trim(), out isUnitDebug);
			if (!isUnitDebug) //如果不是单元测试，则取服务安装路径+配置路径
			{
				DbConfigFileName = ServiceFilePathManager.ServiceInstallPath +
				                   ConfigurationManager.AppSettings["dbConfigFileNameTest"].Trim();
			}
			else
			{
				DbConfigFileName = ConfigurationManager.AppSettings["dbConfigFileNameTest"].Trim();
			}
			Logger.Info("Debug DbConfigFilePath:" + DbConfigFileName);
#else
			DbConfigFileName = ServiceFilePathManager.ServiceInstallPath +
			                   ConfigurationManager.AppSettings["dbConfigFileName"].Trim();
			Logger.Info("Release DbConfigFilePath:" + DbConfigFileName);
#endif
		}

		#region IDatabaseConfiguration Members

		public virtual IList<DatabaseModel> GetDatabases()
		{
			return null;
		}

		#endregion
	}
}