using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ServiceForLmsSynFms
{
	[TestFixture]
	public class unitTest
	{

		[Test]
		public void SCMToLMS()
		{
			try
			{
                //ServiceForLmsSynFms sc = new ServiceForLmsSynFms();
                //sc.OnStart();
                //sc.TimerElapsed2(null, null);
			}
			catch (Exception ex)
			{
				string s = ex.Message;
			}
			finally
			{

			}
		}
	}
}
