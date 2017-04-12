using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EVEMon.Entities.Events;

namespace EVEMon.Services.Specification
{
	public interface IEVEAccountManager
	{
		bool SetAccountTokens(string accountName, SSOCompleteEventArgs args);
		string GetAccountRefreshToken(string accountName);
		string GetAccountAuthenticationToken(string accountName);
		string GetAccountAuthorisationToken(string accountName);

		List<string> GetEVEAccountList();
		bool CheckAccountFileExists();
		bool CreateAccountFile();
	}
}
