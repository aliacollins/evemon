using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Owin;

namespace EVEMon.Gateways.EVEAuthGateway
{
	internal class OwinStartupConfig
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseRedirectionEndPoints();
		}
	}
}