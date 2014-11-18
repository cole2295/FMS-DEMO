using RFD.FMS.Util.Service;

namespace RFD.FMS.AdoNet.Exceptions
{
	public class AdoNetException : WindowsServiceException
	{
		public AdoNetException()
		{
		}

		public AdoNetException(string message)
			: base(message)
		{
		}
	}
}