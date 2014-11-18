using RFD.FMS.Util;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	public class UnitOfWorkUtil
	{
		/// <summary>
		/// 获取<see cref="IUnitOfWorkFactory"/>对象
		/// </summary>
		/// <returns></returns>
		public static IUnitOfWorkFactory GetUnitOfWorkFactory()
		{
			return ServiceLocator.GetObject<IUnitOfWorkFactory>("adoNetUnitOfWorkFactory");
		}
	}
}