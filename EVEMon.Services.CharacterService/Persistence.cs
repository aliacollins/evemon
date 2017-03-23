using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using EVEMon.Services.EVEAccountService.Specification;

namespace EVEMon.Services.EVEAccountService
{
    /// <summary>
    /// A service class which provides functionality for persisting and retrieving EVE Account information
    /// </summary>
    public class AccountPersister : IAccountPersister
    {
        // I was figuring out how to store and retrieve settings from a common location. THere must be a pattern about storing properties in a WinForms app without having
        // circular references, right?!
        // Settings EVEMonSettings = EVEMon

        string AccountInfoXML = $"{Environment.SpecialFolder.ApplicationData}/EVEMon/AccountInfo.xml";

        public string GetAccountRefreshToken(string accountName)
        {
            XmlReader Reader = new XmlReader(AccountInfoXML);


            return "";
        }
    }
}
