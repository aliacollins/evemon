using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EVEMon.Entities.Events;

namespace EVEMon.Services.Specification
{
	public interface IEVECharacterManager
	{
		bool SetCharacterTokens(SSOCompleteEventArgs args);
		string GetCharacterRefreshToken(string characterName);
		string GetCharacterAuthenticationToken(string characterName);
		string GetCharacterAuthorisationToken(string characterName);

		List<string> GetEVECharacterList();
		List<object> GetEVECharacterDetails();
	}
}
