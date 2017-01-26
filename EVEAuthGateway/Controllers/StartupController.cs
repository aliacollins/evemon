using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Flurl;
using Flurl.Util;

using EVEMon.Gateways.EVEAuthGateway.Properties;

namespace EVEMon.Gateways.EVEAuthGateway.Controllers
{
	internal class StartupController : ApiController
	{
		Settings AppSettings = Settings.Default;

		public HttpResponseMessage Get()
		{
			string CallbackURL = AppSettings.InternalServerBaseURL.AppendPathSegment("callback");

			// Create a Guid to use as an insurance policy. The Guid prevents man-in-the-middle attacks.
			Guid State = new Guid();

			// Create the redirect URL for our local server
			string RedirectURL = AppSettings.LoginServerBaseUrl
				.AppendPathSegment("authorize")
				.SetQueryParam("response_type", "code")
				.SetQueryParam("redirect_uri", CallbackURL)
				.SetQueryParam("client_id", AppSettings.ClientID)
				.SetQueryParam("state", State)
				.SetQueryParam("scope", string.Join(" ", AppSettings.Scopes));

			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Moved);
			response.Headers.Location = new Uri(RedirectURL.ToInvariantString());
			return response;
		}
	}
}