using System.Threading;

namespace RFD.FMS.AdoNet
{
	public static class Constants
	{
		public const string NC_UNITOFWORK_TRANSACTION_CONNECTION = "NC_UNITOFWORK_TRANSACTION_CONNECTION";

		public const string NC_UNITOFWORK_STACK = "NC_UNITOFWORK_STACK";

		/// <summary>
		/// NC_UNITOFWORK_TRANSACTION_CONNECTION Thread
		/// </summary>
		public static object NcUnitofworkTransactionConnectionThread
		{
			get { return Thread.CurrentThread.ManagedThreadId; }
		}

		public static object NcUnitofworkStackThread
		{
			get { return NC_UNITOFWORK_STACK + Thread.CurrentThread.ManagedThreadId; }
		}
	}
}