using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the characters list
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class GlobalMonitoredCharacterCollection : ReadonlyCollection<Character>
    {
        /// <summary>
        /// 
        /// </summary>
        internal GlobalMonitoredCharacterCollection()
        {
        }

        /// <summary>
        /// Update the order from the given list
        /// </summary>
        /// <param name="order"></param>
        public void Update(IEnumerable<Character> order)
        {
            Items.Clear();
            Items.AddRange(order);

            // Notify the change
            EveMonClient.OnMonitoredCharactersChanged();
        }

        /// <summary>
        /// Moves the given character to the target index.
        /// </summary>
        /// <remarks>
        /// When the item is located before the target index, it is decremented. 
        /// That way we ensures the item is actually inserted before the item that originally was at <c>targetindex</c>.
        /// </remarks>
        /// <param name="item"></param>
        /// <param name="targetIndex"></param>
        public void MoveTo(Character item, int targetIndex)
        {
            int oldIndex = Items.IndexOf(item);
            if (oldIndex == -1) throw new InvalidOperationException("The item was not found in the collection.");

            if (oldIndex < targetIndex) targetIndex--;
            Items.RemoveAt(oldIndex);
            Items.Insert(targetIndex, item);

            EveMonClient.OnMonitoredCharactersChanged();
        }

        /// <summary>
        /// When the <see cref="Character.IsMonitored"/> property changed, this collection is updated
        /// </summary>
        /// <param name="character">The character for which the property changed</param>
        internal void OnCharacterMonitoringChanged(Character character, bool value)
        {
            if (value)
            {
                if (!Items.Contains(character))
                {
                    Items.Add(character);
                    EveMonClient.OnMonitoredCharactersChanged();
                }
            }
            else
            {
                if (Items.Contains(character))
                {
                    Items.Remove(character);
                    EveMonClient.OnMonitoredCharactersChanged();
                }
            }
        }

        /// <summary>
        /// Imports the given characters
        /// </summary>
        /// <param name="characterGuids"></param>
        internal void Import(List<MonitoredCharacterSettings> monitoredCharacters)
        {
            Items.Clear();
            foreach (var characterSettings in monitoredCharacters)
            {
                var character = EveMonClient.Characters[characterSettings.CharacterGuid];
                if (character != null)
                {
                    Items.Add(character);
                    character.Monitored = true;
                    character.UISettings = characterSettings.Settings;
                }
            }

            EveMonClient.OnMonitoredCharactersChanged();
        }

        /// <summary>
        /// Updates the settings from <see cref="Settings.UI.MonitoredCharacters"/>. Adds and removes group as needed.
        /// </summary>
        /// <param name="order"></param>
        internal List<MonitoredCharacterSettings> Export()
        {
            List<MonitoredCharacterSettings> serial = new List<MonitoredCharacterSettings>();
            foreach (var character in Items)
            {
                serial.Add(new MonitoredCharacterSettings { CharacterGuid = character.Guid, Settings = character.UISettings.Clone() });
            }
            return serial;
        }
    }
}
