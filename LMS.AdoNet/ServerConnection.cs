using System.Collections;
using System.Threading;
using Common.Logging;

namespace RFD.FMS.AdoNet
{
	public static class ServerConnection
	{
		private static readonly Hashtable serverConnections = new Hashtable();
		private static readonly ILog logger = LogManager.GetCurrentClassLogger();

		public static DatabaseSource CurrentDatabase
		{
			get
			{
				object currentDatabase = serverConnections[Thread.CurrentThread.ManagedThreadId];
				if (currentDatabase == null)
				{
					return DatabaseSource.Unspecified;
				}
				return (DatabaseSource) currentDatabase;
			}
			set
			{
				if (serverConnections.Contains(Thread.CurrentThread.ManagedThreadId))
				{
					serverConnections.Remove(Thread.CurrentThread.ManagedThreadId);
				}
				if (value != DatabaseSource.Unspecified)
				{
					serverConnections.Add(Thread.CurrentThread.ManagedThreadId, value);
					logger.Info(string.Format("Thread:{0} Set CurrentDatabase:{1}", Thread.CurrentThread.ManagedThreadId, value));
				}
			}
		}
	}

	public static class WMSServerConnection
	{
		private static readonly Hashtable ServerConnections = new Hashtable();
		private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

		public static WMSDatabaseSource CurrentWMSDatabase
		{
			get
			{
				object currentDatabase = ServerConnections[Thread.CurrentThread.ManagedThreadId];
				if (currentDatabase == null)
				{
					return WMSDatabaseSource.Unspecified;
				}
				return (WMSDatabaseSource) currentDatabase;
			}
			set
			{
				if (ServerConnections.Contains(Thread.CurrentThread.ManagedThreadId))
				{
					ServerConnections.Remove(Thread.CurrentThread.ManagedThreadId);
				}
				if (value != WMSDatabaseSource.Unspecified)
				{
					ServerConnections.Add(Thread.CurrentThread.ManagedThreadId, value);
					Logger.Info(string.Format("Thread:{0} Set CurrentWMSDatabase:{1}", Thread.CurrentThread.ManagedThreadId, value));
				}
			}
		}
	}
}