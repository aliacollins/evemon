using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using Flurl;
using Flurl.Util;

using EVEMon.Gateways.Properties;

namespace EVEMon.Gateways.Controllers
{
	public class StartupController : ApiController
	{
		Settings AppSettings = EVEMon.Gateways.Properties.Settings.Default;

		public HttpResponseMessage Get()
		{
			string CallbackURL = AppSettings.InternalServerBaseURL.AppendPathSegment("callback");

			// Create a Guid to use as an insurance policy. The Guid prevents man-in-the-middle attacks.
			Guid State = Guid.NewGuid();

			// Create the redirect URL for our local server
			string RedirectURL = AppSettings.LoginServerBaseUrl
				.AppendPathSegment("authorize")
				.SetQueryParam("response_type", "code")
				.SetQueryParam("redirect_uri", CallbackURL)
				.SetQueryParam("client_id", AppSettings.ClientID)
				.SetQueryParam("state", State)
				.SetQueryParam("scope", AssembleRequestScopes());

			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Redirect);
			response.Headers.Location = new Uri(RedirectURL.ToInvariantString());
			return response;
		}

		/// <summary>
		/// Gets and returns the collection of scopes defined in the Settings file
		/// </summary>
		/// <returns>A single space delimeted string of scopes</returns>
		private string AssembleRequestScopes()
		{
			string[] ScopeList = new string[AppSettings.Scopes.Count];
			AppSettings.Scopes.CopyTo(ScopeList, 0);
			StringBuilder Builder = new StringBuilder();

			foreach (string Scope in ScopeList)
			{
				Builder.Append($" {Scope}");
			}

			return Builder.ToString();
		}
	}
}