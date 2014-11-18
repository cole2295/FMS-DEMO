using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Common.Logging;
using RFD.FMS.Util;
using RFD.FMS.Util.Context;

namespace RFD.FMS.AdoNet.DbBase
{
	public class ConnectionStrings
	{
		private static readonly ILog logger = LogManager.GetCurrentClassLogger();
		private static readonly IList<DatabaseModel> databaseLst;
		private static string serverConnection;
		private static string webSite;

		static ConnectionStrings()
		{
			var databaseConfiguration = ServiceLocator.GetObject<IDatabaseConfiguration>("databaseConfiguration");
			databaseLst = databaseConfiguration.GetDatabases();
		}

		/// <summary>
		/// 判断是网站还是后台 true:网站;false:后台
		/// </summary>
		public static bool ReadOnlyWebConnString
		{
			get
			{
				try
				{
					webSite = ConfigurationManager.AppSettings["website"].Trim();
				}
				catch (Exception ex)
				{
					logger.Info("website没有读取到", ex);
					return false;
				}
				return string.IsNullOrEmpty(webSite) || webSite == 1.ToString();
			}
		}

		/// <summary>
		/// 根据配置文件看是Vjia还是Vancl
		/// </summary>
		public static bool IsVjia
		{
			get
			{
				try
				{
					serverConnection = ConfigurationManager.AppSettings["serverConnection"].Trim();
				}
				catch (Exception ex)
				{
					logger.Info("serverConnection没有读取到", ex);
				}
				if (!string.IsNullOrEmpty(serverConnection))
				{
					return serverConnection == "2" ? true : false;
				}
				return false;
			}
		}

		#region ConnectionStrings

		///<summary>
		///SCM 主库连接字符串
		///</summary>
		public static string ConnStringOfSCM
		{
			get
			{
				try
				{
					serverConnection = ConfigurationManager.AppSettings["serverConnection"].Trim();
				}
				catch (Exception ex)
				{
					logger.Info("serverConnection没有读取到", ex);
				}
				if (string.IsNullOrEmpty(serverConnection))
				{
					return GetConnectionString(ServerConnection.CurrentDatabase, DatabaseType.Execute);
				}
				return IsVjia
				       	? GetConnectionString(DatabaseSource.Vjia, DatabaseType.Execute)
				       	: GetConnectionString(DatabaseSource.Vancl, DatabaseType.Execute);
			}
		}

		/// <summary>
		/// WMS 主库连接字符串
		/// </summary>
		public static string ConnStringOfWMS
		{
			get
			{
				if (HttpContext.Current != null)
				{
					HttpCookie httpCookie = HttpContext.Current.Request.Cookies["WarehouseId_Conn"];
					if (httpCookie == null)
						return null;
					return (from database in databaseLst
					        where database.WebCookieValue == httpCookie.Value && database.DatabaseType == DatabaseType.Execute
					        select database.ConnectionString).FirstOrDefault();
				}
				return GetConnectionString(GetDbSourceByWMSDbSource(WMSServerConnection.CurrentWMSDatabase),
				                           DatabaseType.Execute);
			}
		}

		/// <summary>
		/// FanKu(凡库)数据库连接字符串
		/// </summary>
		public static string ConnStringOfFanKu
		{
			get { return GetConnectionString(DatabaseSource.FanKu, DatabaseType.Execute); }
		}

		/// <summary>
		/// 客户数据库连接字符串
		/// </summary>
		public static string ConnStringOfCustomer
		{
			get { return GetConnectionString(DatabaseSource.Customer, DatabaseType.Execute); }
		}
        /// <summary>
        /// 客户数据库连接字符串
        /// </summary>
        public static string ConnStringOfRfd
        {
            get { return GetConnectionString(DatabaseSource.LMS_RFD, DatabaseType.Execute); }
        }

		///<summary>
		///订单数据库连接字符串
		///</summary>
		public static string ConnStringOfOrder
		{
			get { return GetConnectionString(DatabaseSource.Order, DatabaseType.Execute); }
		}

		#endregion

		#region ReadonlyConnectionStrings

		/// <summary>
		/// 当前登陆SCM 只读库连接字符串
		/// </summary>
		public static string ReadOnlyConnStringOfSCM
		{
			get { return GetConnectionString(ServerConnection.CurrentDatabase, DatabaseType.Readonly); }
		}

		/// <summary>
		/// 当前登陆WMS 只读库连接字符串
		/// </summary>
		public static string ReadOnlyConnStringOfWMS
		{
			get
			{
				if (HttpContext.Current != null)
				{
					HttpCookie httpCookie = HttpContext.Current.Request.Cookies["WarehouseId_Conn"];
					if (httpCookie == null)
						return null;
					return (from database in databaseLst
					        where database.WebCookieValue == httpCookie.Value && database.DatabaseType == DatabaseType.Readonly
					        select database.ConnectionString).FirstOrDefault();
				}
				return GetConnectionString(GetDbSourceByWMSDbSource(WMSServerConnection.CurrentWMSDatabase),
				                           DatabaseType.Readonly);
			}
		}

		/// <summary>
		/// Customer数据库只读链接字符串
		/// </summary>
		public static string ReadOnlyConnStringOfCustomer
		{
			get { return GetConnectionString(DatabaseSource.Customer, DatabaseType.Readonly); }
		}

		/// <summary>
		/// Order 数据库只读链接字符串
		/// </summary>
		public static string ReadOnlyConnStringOfOrder
		{
			get { return GetConnectionString(DatabaseSource.Order, DatabaseType.Readonly); }
		}

		/// <summary>
		/// FanKu 数据库只读链接字符串
		/// </summary>
		public static string ReadOnlyConnStringOfFanKu
		{
			get { return GetConnectionString(DatabaseSource.FanKu, DatabaseType.Readonly); }
		}

		#endregion

		#region CurrentConnection

		/// <summary>
		/// 获取当前使用的连接
		/// </summary>
		/// <param name="connectionString">当前使用的连接字符串</param>
		/// <returns>返回当前连接</returns>
		public static SqlConnection GetCurrentConnection(string connectionString)
		{
			var connection = NeutralContext.Get(Constants.NcUnitofworkTransactionConnectionThread) as SqlConnection;
			if (connection != null)
			{
				logger.Debug("CurrentConnection is in UnitOfWork.");
				if (connection.State != ConnectionState.Open)
				{
					logger.Warn("CurrentConnection is not opened, open it!");
					connection.ConnectionString = connectionString;
					connection.Open();
				}
			}
			else
			{
				logger.Debug("CurrentConnection is not in UnitOfWork.");
				connection = new SqlConnection(connectionString);
				logger.Warn("New connection create in CurrentConnection!");
			}
			logger.Debug(string.Format(
				"Get CurrentConnection:connString={0};state={1}.",
				connection.ConnectionString, connection.State));
			return connection;
		}

		#endregion

		/// <summary>
		/// 获取指定仓库指定类型的连接数据库字符串
		/// </summary>
		/// <param name="databaseSource"></param>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string GetConnectionString(DatabaseSource databaseSource, DatabaseType dbType)
		{
			foreach (DatabaseModel database in databaseLst)
			{
				if (database.DatabaseSource == databaseSource && database.DatabaseType == dbType)
				{
					logger.Trace(
						m => m("{0}ConnectionString:{1}", databaseSource.ToString() + dbType.ToString(), database.ConnectionString));
					return database.ConnectionString;
				}
			}
			logger.Trace(m => m("{0}ConnString:{1}", databaseSource.ToString() + dbType.ToString(), string.Empty));
			return string.Empty;
		}

		/// <summary>
		/// 把WMSDatabaseSource转化成DatabaseSource
		/// </summary>
		/// <param name="wmsDatabaseSource"></param>
		/// <returns></returns>
		public static DatabaseSource GetDbSourceByWMSDbSource(WMSDatabaseSource wmsDatabaseSource)
		{
			return (DatabaseSource) Enum.Parse(typeof (DatabaseSource), wmsDatabaseSource.ToString());
		}

		/// <summary>
		/// 把DatabaseSource转化成WMSDatabaseSource
		/// </summary>
		/// <param name="databaseSource"></param>
		/// <returns></returns>
		public static WMSDatabaseSource GetWMSDbSourceByDbSource(DatabaseSource databaseSource)
		{
			return (WMSDatabaseSource) Enum.Parse(typeof (WMSDatabaseSource), databaseSource.ToString());
		}

		/// <summary>
		/// 根据数据库类型获取数据库名称
		/// </summary>
		/// <param name="databaseSource"></param>
		/// <returns></returns>
		public static string GetDbNameByDbSource(DatabaseSource databaseSource)
		{
			return (from database in databaseLst
			        where database.DatabaseSource == databaseSource
			        select database.DatabaseName).FirstOrDefault();
		}

		/// <summary>
		/// 根据WMS数据库类型获取数据库名称
		/// </summary>
		/// <param name="wmsDatabaseSource"></param>
		/// <returns></returns>
		public static string GetDbNameByWMSDbSource(WMSDatabaseSource wmsDatabaseSource)
		{
			DatabaseSource databaseSource = GetDbSourceByWMSDbSource(wmsDatabaseSource);
			return GetDbNameByDbSource(databaseSource);
		}

		/// <summary>
		/// 根据仓库ID获取仓库枚举
		/// </summary>
		/// <param name="warehouseId"></param>
		/// <returns></returns>
		public static WMSDatabaseSource GetWMSDbSourceByWarehouseId(string warehouseId)
		{
			return
				(from item in databaseLst where item.WarehouseId == warehouseId select GetWMSDbSourceByDbSource(item.DatabaseSource))
					.FirstOrDefault();
		}

		/// <summary>
		/// 获取指定仓库相关联的SCM库的数据库名称
		/// </summary>
		/// <param name="wmsDatabaseSource"></param>
		/// <returns></returns>
		public static string GetRelationDbNameByWMSDbSource(WMSDatabaseSource wmsDatabaseSource)
		{
			DatabaseSource databaseSource = GetDbSourceByWMSDbSource(wmsDatabaseSource);
			return (from database in databaseLst
			        where database.DatabaseSource == databaseSource
			        select (DatabaseSource) Enum.Parse(typeof (DatabaseSource), database.CompanySource.ToString())
			        into dbSource select GetDbNameByDbSource(dbSource)).FirstOrDefault();
		}

		/// <summary>
		/// 获取指定仓库的仓库Id
		/// </summary>
		/// <param name="wmsDatabaseSource"></param>
		/// <returns></returns>
		public static string GetWarehouseIdByWMSDbSource(WMSDatabaseSource wmsDatabaseSource)
		{
			DatabaseSource databaseSource = GetDbSourceByWMSDbSource(wmsDatabaseSource);
			return (from database in databaseLst
			        where database.DatabaseSource == databaseSource
			        select database.WarehouseId).FirstOrDefault();
		}
	}
}