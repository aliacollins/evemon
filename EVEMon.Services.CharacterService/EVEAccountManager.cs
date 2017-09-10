using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

using EVEMon.Entities.Events;
using EVEMon.Services.Specification;

namespace EVEMon.Services
{
    /// <summary>
    /// A service class which provides functionality for persisting and retrieving EVE Account information
    /// </summary>
    public class EVEAccountManager : IEVEAccountManager
    {
        // TODO - Ashilta - I was figuring out how to store and retrieve settings from a common location. There must be a pattern about storing properties in a WinForms app without having
        // circular references, right?!
        // Settings EVEMonSettings = EVEMon

        string AccountDetailsXMLPath = $"{Environment.SpecialFolder.ApplicationData}/EVEMon/AccountInfo.xml";

		public bool SetAccountTokens(string accountName, SSOCompleteEventArgs args)
		{
			return true;
		}

        public string GetAccountRefreshToken(string accountName)
        {
			// Get the Account information:
			XmlAttributeCollection AccountInfo = GetAccountInformation(accountName.ToLower());

			// return the specific element related to the Refresh token
            return AccountInfo["RefreshToken"].ToString();
        }

		public string GetAccountAuthorisationToken(string accountName)
		{
			// Get the Account information:
			XmlAttributeCollection AccountInfo = GetAccountInformation(accountName.ToLower());

			// Return the required property
			return AccountInfo["AuthorisationToken"].ToString();
		}

		public string GetAccountAuthenticationToken(string accountName)
		{
			// Get the Account information
			XmlAttributeCollection AccountInfo = GetAccountInformation(accountName.ToLower());

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

		public bool CheckAccountFileExists()
		{
			DateTime FileLastWrite = File.GetLastWriteTime(AccountDetailsXMLPath);
			if (FileLastWrite == new DateTime())
				return false;

			return true;
		}

		public bool CreateAccountFile()
		{
			try
			{ File.Create(AccountDetailsXMLPath); }
			catch(Exception ex)
			{
				// Something went very wrong. No accounts doesn't, however, mean EVEMon can't be used - handle the exception and continue 
				// TODO - Ashilta - Handle more exceptiosn
				// TODO - Ashilta - Logging?

				return false;
			}

			return false;
		}
    }
}
