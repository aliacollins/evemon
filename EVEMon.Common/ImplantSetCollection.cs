using System.Linq;
using System.Collections.Generic;

using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a collection of implants sets.
    /// </summary>
    public sealed class ImplantSetCollection : ReadonlyVirtualCollection<ImplantSet>
    {
        private readonly Character m_owner;
        private readonly List<ImplantSet> m_customSets;
        private ImplantSet m_current;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="owner"></param>
        internal ImplantSetCollection(Character owner)
        {
            m_owner = owner;
            m_customSets = new List<ImplantSet>();
            OldAPI = new ImplantSet(owner, "Previous implants from the API");
            API = new ImplantSet(owner, "Implants from API");
            None = new ImplantSet(owner, "<None>");
            m_current = API;
        }

        /// <summary>
        /// Gets the implant set by its index. First items are <see cref="API"/>, <see cref="OldAPI"/>, then the custom sets.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ImplantSet this[int index]
        {
            get { return Enumerate().ElementAt(index); }
        }

        /// <summary>
        /// Gets the none im
        /// </summary>
        public ImplantSet None { get; private set; }

        /// <summary>
        /// Gets the implants retrieved from the API.
        /// </summary>
        public ImplantSet API { get; private set; }

        /// <summary>
        /// Gets the implants previously retrieved from the API.
        /// </summary>
        public ImplantSet OldAPI { get; private set; }

        /// <summary>
        /// Gets or sets the current implant set.
        /// </summary>
        public ImplantSet Current
        {
            get { return m_current; }
            set
            {
                if (m_current == value)
                    return;

                m_current = value;
                EveMonClient.OnCharacterUpdated(m_owner);
            }
        }

        /// <summary>
        /// Adds a new implant set.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ImplantSet Add(string name)
        {
            ImplantSet set = new ImplantSet(m_owner, name);
            m_customSets.Add(set);
            EveMonClient.OnCharacterUpdated(m_owner);
            return set;
        }

        /// <summary>
        /// Removes the given set.
        /// </summary>
        /// <param name="set"></param>
        public void Remove(ImplantSet set)
        {
            m_customSets.Remove(set);
            EveMonClient.OnCharacterUpdated(m_owner);
        }

        /// <summary>
        /// Core method on which everything else relies.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ImplantSet> Enumerate()
        {
            yield return API;
            yield return OldAPI;
            foreach (ImplantSet set in m_customSets)
            {
                yield return set;
            }
        }

        /// <summary>
        /// Imports data from a deserialization object
        /// </summary>
        /// <param name="serial"></param>
        public void Import(SerializableImplantSetCollection serial)
        {
            API.Import(serial.API, false);
            OldAPI.Import(serial.API, false);

            m_customSets.Clear();
            foreach (SerializableSettingsImplantSet serialSet in serial.CustomSets)
            {
                ImplantSet set = new ImplantSet(m_owner, serialSet.Name);
                set.Import(serialSet, true);
                m_customSets.Add(set);
            }

            // Imports selection
            m_current = Enumerate().ElementAt(serial.SelectedIndex);

            EveMonClient.OnSettingsChanged();
        }

        /// <summary>
        /// Exports this collection to a serialization object.
        /// </summary>
        /// <returns></returns>
        public SerializableImplantSetCollection Export()
        {
            SerializableImplantSetCollection serial = new SerializableImplantSetCollection
                                                          { API = API.Export(), OldAPI = OldAPI.Export() };
            serial.CustomSets.AddRange(m_customSets.Select(x => x.Export()));
            serial.SelectedIndex = Enumerate().IndexOf(m_current);
            return serial;
        }

        /// <summary>
        /// Imports data from an API serialization object provided by CCP
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(SerializableImplantSet serial)
        {
            // Search whether the api infos are different from the ones currently stored
            ImplantSet newSet = new ImplantSet(m_owner, "temp");
            newSet.Import(serial);

            bool isDifferent = false;
            Implant[] oldArray = API.ToArray();
            Implant[] newArray = newSet.ToArray();
            for (int i = 0; i < oldArray.Length; i++)
            {
                isDifferent |= (oldArray[i] != newArray[i]);
            }

            // Imports the API and make a backup
            if (isDifferent)
                OldAPI.Import(API.Export(), false);

            API.Import(serial);

            EveMonClient.OnSettingsChanged();
        }
    }
}
