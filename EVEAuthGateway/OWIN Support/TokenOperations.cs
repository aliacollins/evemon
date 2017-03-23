using System.Collections.Generic;
using System.Net.Http;

using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;

using EVEMon.Gateways.EVEAuthGateway.Entities;

namespace EVEMon.Gateways.EVEAuthGateway.Extensions.Owin
{
	public static class TokenOperations
	{
		public static void FromAuthorizationCode(string code)
		{
			var body = new Dictionary<string, string>
			{
				{"grant_type", "authorization_code"},
				{"code", code}
			};

			GetToken(code, body);
		}

		public static void FromRefreshToken(string refreshToken)
		{
			var body = new Dictionary<string, string>
			{
				{"grant_type", "refresh_token"},
				{"refresh_token", refreshToken}
			};
			GetToken(refreshToken, body);
		}

		private static void GetToken(string authenticationArtifact, Dictionary<string, string> body)
		{
			var settings = EVEMon.Gateways.EVEAuthGateway.Properties.Settings.Default;
			var content = new FormUrlEncodedContent(body);
			var result = settings.LoginServerBaseUrl
				.AppendPathSegment("token")
				.WithBasicAuth(settings.ClientID, settings.ClientSecret)
				.PostAsync(content)
				.ReceiveString()
				.Result;

			var obj = JObject.Parse(result);
			var args = new SignOnCompleteEventArgs
			{
				AccessToken = obj.SelectToken("access_token").Value<string>(),
				Expires = obj.SelectToken("expires_in").Value<int>(),
				RefreshToken = obj.SelectToken("refresh_token").Value<string>()
			};

			if (body["grant_type"] == "authorization_code") args.AuthorizationToken = authenticationArtifact;

			// An event to signal that the SSO process has finished. AJA - Need to determine how we tie this up into the bigger process.
			//GlobalEvents.Complete(null, args);
		}
	}
}