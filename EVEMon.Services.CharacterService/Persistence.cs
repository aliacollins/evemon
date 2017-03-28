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
			// Get the Account information:
			XmlAttributeCollection AccountInfo = GetAccountInformation(accountName);

			// return the specific element related to the Refresh token
            return AccountInfo["RefreshToken"].ToString();
        }

		public string GetAccountAuthorisationToken(string accountName)
		{
			// Get the Account information:
			XmlAttributeCollection AccountInfo = GetAccountInformation(accountName);

			// Return the required property
			return AccountInfo["AuthorisationToken"].ToString();
		}

		public string GetAccountAuthenticationToken(string accountName)
		{
			// Get the Account information
			XmlAttributeCollection AccountInfo = GetAccountInformation(accountName);

			// Return the required property
			return AccountInfo["AuthenticationToken"].ToString();
		}

		public List<string> GetEVEAccountList()
		{
			// We need to open the XML file which holds the account information, and return a collection of the AccountName elements.
			throw new NotImplementedException();
		}

		private XmlAttributeCollection GetAccountInformation(string accountName)
		{
			
			throw new NotImplementedException();
		}
    }
}
