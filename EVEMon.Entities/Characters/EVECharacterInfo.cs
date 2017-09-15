using System;

namespace EVEMon.Entities.Accounts
{
    [Serializable]
    public class EVEAccountInfo
    {
		public EVEAccountInfo(string characterName)
		{
			CharacterName = characterName;
		}

        public string CharacterName { get; private set; }


        public string AuthorisationToken { get; private set; }
        public string AuthenticationToken { get; private set; }
        public string RefreshToken { get; private set; }

		public void SetAuthorisationTokn(string authorisationToken)
		{
			AuthorisationToken = authorisationToken;
		}

		public void SetAuthenticationToken(string authenticationToken)
		{
			AuthorisationToken = authenticationToken;
		}

		public void SetRefreshToken(string refreshToken)
		{
			RefreshToken = refreshToken;
		}
    }
}