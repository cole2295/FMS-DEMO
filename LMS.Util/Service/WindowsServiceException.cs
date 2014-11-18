using System;

namespace RFD.FMS.Util.Service
{
	public class WindowsServiceException : Exception
	{
		public WindowsServiceException()
		{
		}

		public WindowsServiceException(string message)
			: base(message)
		{
		}
	}
}