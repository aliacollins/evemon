using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Owin;

namespace EVEMon.Gateways.EVEAuthGateway
{
	internal static class OwinExtensions
	{
		public static IAppBuilder UseRedirectionEndPoints(this IAppBuilder app)
		{
			// Configure Web API for self-host. 
			var config = new HttpConfiguration();
			config.Routes.MapHttpRoute(
				"Authentication",
				"{controller}"
			);

			return app.UseWebApi(config);
		}

	}
}
