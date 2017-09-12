using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

using EVEMon.Entities.Events;
using EVEMon.Services.Properties;
using EVEMon.Services.Specification;

namespace EVEMon.Services
{
    /// <summary>
    /// A service class which provides functionality for persisting and retrieving EVE Character information
    /// </summary>
    public class EVECharacterManager : IEVECharacterManager
    {
		static Settings EVEMonSettings = Properties.Settings.Default;

		string CharacterDetailsXMLPath = EVEMonSettings["CharacterDataFilePath"].ToString();

		public bool SetCharacterTokens(SSOCompleteEventArgs args)
		{
			return true;
		}

        public string GetCharacterRefreshToken(string characterName)
        {
			// Get the Character information:
			XmlAttributeCollection CharacterInfo = GetCharacterInformation(characterName.ToLower());

			// return the specific element related to the Refresh token
            return CharacterInfo["RefreshToken"].ToString();
        }

		public string GetCharacterAuthorisationToken(string characterName)
		{
			// Get the Character information:
			XmlAttributeCollection CharacterInfo = GetCharacterInformation(characterName.ToLower());

			// Return the required property
			return CharacterInfo["AuthorisationToken"].ToString();
		}

		public string GetCharacterAuthenticationToken(string characterName)
		{
			// Get the Character information
			XmlAttributeCollection CharacterInfo = GetCharacterInformation(characterName.ToLower());

			// Return the required property
			return CharacterInfo["AuthenticationToken"].ToString();
		}

		public List<string> GetEVECharacterList()
		{
			// We need to open the XML file which holds the character information, and return a collection of the CharacterName elements.
			throw new NotImplementedException();
		}

		public List<object> GetEVECharacterDetails()
		{
			return new List<object>();
		}

		private XmlAttributeCollection GetCharacterInformation(string characterName)
		{
			
			throw new NotImplementedException();
		}

		private bool CheckCharacterFileExists()
		{
			DateTime FileLastWrite = File.GetLastWriteTime(CharacterDetailsXMLPath);
			if (FileLastWrite == new DateTime())
				return false;

			return true;
		}

		private bool CreateCharacterFile()
		{
			try
			{ File.Create(CharacterDetailsXMLPath); }
			catch(Exception ex)
			{
				// Something went very wrong. No characters doesn't, however, mean EVEMon can't be used - handle the exception and continue 
				// TODO - Ashilta - Handle more exceptions
				// TODO - Ashilta - Logging?

				return false;
			}

			return false;
		}
    }
}