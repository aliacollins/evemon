using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Abstractions.Services;
using EVEMon.Common.Models;

namespace EVEMon.Common.Services
{
    /// <summary>
    /// Implementation of <see cref="ICharacterService"/> that wraps EveMonClient.
    /// </summary>
    public sealed class CharacterService : ICharacterService
    {
        /// <inheritdoc />
        public IReadOnlyList<Character> Characters =>
            EveMonClient.Characters?.ToList().AsReadOnly() ?? new List<Character>().AsReadOnly();

        /// <inheritdoc />
        public IReadOnlyList<Character> MonitoredCharacters =>
            EveMonClient.MonitoredCharacters?.ToList().AsReadOnly() ?? new List<Character>().AsReadOnly();

        /// <inheritdoc />
        public Character GetCharacter(Guid guid)
        {
            return EveMonClient.Characters?.FirstOrDefault(c => c.Guid == guid);
        }

        /// <inheritdoc />
        public Character GetCharacterByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return EveMonClient.Characters?.FirstOrDefault(c =>
                string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public void SetMonitored(Character character, bool monitored)
        {
            if (character == null)
                return;

            character.Monitored = monitored;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetKnownLabels()
        {
            return EveMonClient.Characters?.GetKnownLabels()?.ToList().AsReadOnly()
                ?? new List<string>().AsReadOnly();
        }

        /// <inheritdoc />
        public void Add(Character character)
        {
            if (character == null)
                return;

            EveMonClient.Characters?.Add(character);
        }

        /// <inheritdoc />
        public void Remove(Character character)
        {
            if (character == null)
                return;

            EveMonClient.Characters?.Remove(character);
        }
    }
}
