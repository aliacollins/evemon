using System;

using EVEMon.Entities.Accounts;

namespace EVEMon.Entities.Events
{
	public static class GlobalEvents
	{
		public static event EventHandler<SSOCompleteEventArgs> SSOComplete;

		public static void Complete(object sender, SSOCompleteEventArgs e)
		{
			SSOComplete(sender, e);
		}
	}
}