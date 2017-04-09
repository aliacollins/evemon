using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EVEMon.Entities.Accounts;

namespace EVEMon.Services.EVEAccountService.Specification
{
	public interface IAccountPersister
	{
		//bool SetAccountTokens(string accountName, Dictionary<string, string> tokens);
		string GetAccountRefreshToken(string accountName);
		string GetAccountAuthenticationToken(string accountName);
		string GetAccountAuthorisationToken(string accountName);

		//void SaveAccountData(EVEAccountInfo accountInfo);
		List<string> GetEVEAccountList();
		bool CheckAccountFileExists();
		bool CreateAccountFile();
	}
}
