using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Flurl;
using Flurl.Util;

using EVEMon.Gateways.EVEAuthGateway.Extensions.Owin;
using EVEMon.Gateways.EVEAuthGateway.Properties;

namespace EVEMon.Gateways.EVEAuthGateway.Controllers
{
	public class CallbackController : ApiController
	{
		public HttpResponseMessage Get()
		{
			var query = Request.GetQueryNameValuePairs().ToDictionary(k => k.Key);

			if (!query.ContainsKey("code")) return RedirectTo("failed");

			var code = query["code"].Value;

			TokenOperations.FromAuthorizationCode(code);

			return RedirectTo("success");
		}

		private HttpResponseMessage RedirectTo(string pathSegment)
		{
			var url = Settings.Default.InternalServerBaseURL
				.AppendPathSegment($"{pathSegment}.html");
			var response = Request.CreateResponse(HttpStatusCode.Moved);
			response.Headers.Location = new Uri(url.ToInvariantString());
			return response;
		}
	}
}