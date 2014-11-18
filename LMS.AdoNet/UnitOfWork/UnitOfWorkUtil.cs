using RFD.FMS.Util;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	public class UnitOfWorkUtil
	{
		/// <summary>
		/// ��ȡ<see cref="IUnitOfWorkFactory"/>����
		/// </summary>
		/// <returns></returns>
		public static IUnitOfWorkFactory GetUnitOfWorkFactory()
		{
			return ServiceLocator.GetObject<IUnitOfWorkFactory>("adoNetUnitOfWorkFactory");
		}
	}
}