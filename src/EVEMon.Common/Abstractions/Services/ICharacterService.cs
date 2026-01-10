using System;
using System.Collections.Generic;
using EVEMon.Common.Models;

namespace EVEMon.Common.Abstractions.Services
{
    /// <summary>
    /// Service interface for character data access.
    /// Abstracts EveMonClient.Characters and EveMonClient.MonitoredCharacters.
    /// </summary>
    public interface ICharacterService
    {
        /// <summary>
        /// Gets all characters.
        /// </summary>
        IReadOnlyList<Character> Characters { get; }

        /// <summary>
        /// Gets the monitored characters.
        /// </summary>
        IReadOnlyList<Character> MonitoredCharacters { get; }

        /// <summary>
        /// Gets a character by its GUID.
        /// </summary>
        /// <param name="guid">The character's unique identifier.</param>
        /// <returns>The character, or null if not found.</returns>
        Character GetCharacter(Guid guid);

        /// <summary>
        /// Gets a character by its name.
        /// </summary>
        /// <param name="name">The character's name.</param>
        /// <returns>The character, or null if not found.</returns>
        Character GetCharacterByName(string name);

        /// <summary>
        /// Sets whether a character is monitored.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="monitored">Whether to monitor the character.</param>
        void SetMonitored(Character character, bool monitored);

        /// <summary>
        /// Gets all known character labels.
        /// </summary>
        IReadOnlyList<string> GetKnownLabels();

        /// <summary>
        /// Adds a new character to the collection.
        /// </summary>
        /// <param name="character">The character to add.</param>
        void Add(Character character);

        /// <summary>
        /// Removes a character from the collection.
        /// </summary>
        /// <param name="character">The character to remove.</param>
        void Remove(Character character);
    }
}
