using System;

namespace EVEMon.Entities.Events
{
	public class SSOCompleteEventArgs : EventArgs
	{
		public int CharacterID { get; set; }
		public string CharacterName { get; set; }
		public string CharacterOwnerHash { get; set; }

		public string AuthorizationToken { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public int Expires { get; set; }
		public bool SignOnSuccess { get; set; }
	}
}