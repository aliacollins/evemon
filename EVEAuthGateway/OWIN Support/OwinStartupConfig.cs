using System;

using Owin;

namespace EVEMon.Gateways.Extensions.Owin
{
	public class OwinStartupConfig
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseRedirectionEndPoints();
			app.UseStaticContent();
		}
	}
}