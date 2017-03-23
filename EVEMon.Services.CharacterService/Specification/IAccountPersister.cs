using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVEMon.Services.EVEAccountService.Specification
{
    public interface IAccountPersister
    {
        bool SetAccountTokens(string accountName, Dictionary<string,string> tokens);
        string GetAccountRefreshToken(string accountName);
        string GetAccountBearerTOken(string accountName);
    }
}
