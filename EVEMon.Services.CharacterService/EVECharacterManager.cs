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

		/// <summary>
		/// Fetches and returns the refresh token for a specified character
		/// </summary>
		/// <param name="characterName">A string; the name of the character</param>
		/// <returns>A string; the SSO refresh token for the character</returns>
        public string GetCharacterRefreshToken(string characterName)
        {
			if (characterName == null)
				throw new ArgumentNullException("The provided character name was null.");

			if (characterName.Equals(string.Empty, StringComparison.InvariantCultureIgnoreCase))
				throw new ArgumentException("The provided character name was empty.");

			// Get the Character information:
			XmlAttributeCollection CharacterInfo = GetCharacterInformation(characterName.ToLower());

			// return the specific element related to the Refresh token
			return CharacterInfo["RefreshToken"]?.ToString() ?? string.Empty;
        }

		/// <summary>
		/// Fetches and returns the authorisation token for a specified character
		/// </summary>
		/// <param name="characterName">A string; the name of the character</param>
		/// <returns>A string; the authorisation token for the character</returns>
		public string GetCharacterAuthorisationToken(string characterName)
		{
			if (characterName == null)
				throw new ArgumentNullException("The provided character name was null.");

			if (characterName.Equals(string.Empty, StringComparison.InvariantCultureIgnoreCase))
				throw new ArgumentException("The provided character name was empty.");

			// Get the Character information:
			XmlAttributeCollection CharacterInfo = GetCharacterInformation(characterName.ToLower());

			// Return the required property
			return CharacterInfo["AuthorisationToken"]?.ToString();
		}

		/// <summary>
		/// Fetches and returns the Authentication token for a specified character.
		/// </summary>
		/// <param name="characterName">A string; the character name</param>
		/// <returns>A string; the authentication token</returns>
		public string GetCharacterAuthenticationToken(string characterName)
		{
			if (characterName == null)
				throw new ArgumentNullException("The provided character name was null.");

			if (characterName.Equals(string.Empty, StringComparison.InvariantCultureIgnoreCase))
				throw new ArgumentException("The provided character name was empty.");

			// Get the Character information
			XmlAttributeCollection CharacterInfo = GetCharacterInformation(characterName.ToLower());

			// Return the required property
			return CharacterInfo["AuthenticationToken"]?.ToString() ?? string.Empty;
		}

		/// <summary>
		/// Fetches a list of the character names stored by EVEMon
		/// </summary>
		/// <returns></returns>
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