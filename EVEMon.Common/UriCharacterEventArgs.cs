using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the argument for the callback for uri characters addition
    /// </summary>
    public sealed class UriCharacterEventArgs : EventArgs
    {
        private Uri m_uri;
        private APIResult<SerializableAPICharacter> m_result;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="character"></param>
        /// <param name="error"></param>
        public UriCharacterEventArgs(Uri uri, APIResult<SerializableAPICharacter> result)
        {
            m_uri = uri;
            m_result = result;
        }

        /// <summary>
        /// Gets the created Uri character
        /// </summary>
        public Uri Uri
        {
            get { return m_uri; }
        }

        /// <summary>
        /// Gets true if there was an error.
        /// </summary>
        public bool HasError
        {
            get { return m_result.HasError; }
        }

        /// <summary>
        /// Gets the error which occured
        /// </summary>
        public string Error
        {
            get { return m_result.ErrorMessage; }
        }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string CharacterName
        {
            get { return m_result.Result.Name; }
        }

        /// <summary>
        /// Creates the character
        /// </summary>
        public UriCharacter CreateCharacter()
        {
            // Retrieve the identity and create one if needed
            var identity = EveClient.CharacterIdentities[m_result.Result.ID];
            if (identity == null)
            {
                identity = EveClient.CharacterIdentities.Add(m_result.Result.ID, m_result.Result.Name);
            }

            // Instantiates characters, adds, notify
            var uriCharacter = new UriCharacter(identity, m_uri, m_result);
            EveClient.Characters.Add(uriCharacter, true);
            return uriCharacter;
        }

        /// <summary>
        /// Updates the given character.
        /// </summary>
        /// <param name="character"></param>
        public void UpdateCharacter(UriCharacter character)
        {
            // Retrieve the identity and create one if needed
            var identity = EveClient.CharacterIdentities[m_result.Result.ID];
            if (identity == null)
            {
                identity = EveClient.CharacterIdentities.Add(m_result.Result.ID, m_result.Result.Name);
            }

            // Updates
            character.Update(identity, m_uri, m_result);
        }
    }
}
