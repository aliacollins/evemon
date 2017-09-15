using System.Collections.Generic;
using System.Net.Http;

using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;

using EVEMon.Entities.Events;

namespace EVEMon.Gateways.Extensions.Owin
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
			var settings = EVEMon.Gateways.Properties.Settings.Default;
			var content = new FormUrlEncodedContent(body);
			var result = settings.LoginServerBaseUrl
				.AppendPathSegment("token")
				.WithBasicAuth(settings.ClientID, settings.ClientSecret)
				.PostAsync(content)
				.ReceiveString()
				.Result;

			var obj = JObject.Parse(result);
			var args = new SSOCompleteEventArgs
			{
				AccessToken = obj.SelectToken("access_token").Value<string>(),
				Expires = obj.SelectToken("expires_in").Value<int>(),
				RefreshToken = obj.SelectToken("refresh_token").Value<string>()
			};

			if (body["grant_type"] == "authorization_code") args.AuthorizationToken = authenticationArtifact;

			// Before we finish, we need to get details of the character we're dealing with:
			GetCharacterDetailsFromSSO(ref args);

			// An event to signal that the SSO process has finished.
			GlobalEvents.Complete(null, args);
		}

		private static void GetCharacterDetailsFromSSO(ref SSOCompleteEventArgs args)
		{
			var Settings = Properties.Settings.Default;
			var Character = Settings.LoginServerBaseUrl.AppendPathSegment("verify")
				.WithOAuthBearerToken(args.AccessToken)
				.GetAsync()
				.ReceiveJson()
				.Result;

			args.CharacterName = Character.CharacterName;
			args.CharacterID = (int)Character.CharacterID;
			args.CharacterOwnerHash = Character.CharacterOwnerHash;
		}
	}
}