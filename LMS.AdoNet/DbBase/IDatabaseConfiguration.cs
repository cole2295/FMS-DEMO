using System.Collections.Generic;

namespace RFD.FMS.AdoNet.DbBase
{
	public interface IDatabaseConfiguration
	{
		IList<DatabaseModel> GetDatabases();
	}
}