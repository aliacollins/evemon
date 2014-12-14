using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Collections;
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
        private readonly List<ImplantSet> m_cloneSets;
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
            m_cloneSets = new List<ImplantSet>();
            ActiveClone = new ImplantSet(owner, "Active Clone");
            None = new ImplantSet(owner, "None");
            m_current = ActiveClone;
        }

        /// <summary>
        /// Gets the implant set by its index.
        /// First items are <see cref="ActiveClone"/> then the jump clones then the custom sets.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ImplantSet this[int index]
        {
            get { return Enumerate().ElementAt(index); }
        }

        /// <summary>
        /// Gets the none implant.
        /// </summary>
        public ImplantSet None { get; private set; }

        /// <summary>
        /// Gets the implants retrieved from the API.
        /// </summary>
        public ImplantSet ActiveClone { get; private set; }

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
            yield return ActiveClone;

            foreach (ImplantSet set in m_cloneSets)
            {
                yield return set;
            }

            foreach (ImplantSet set in m_customSets)
            {
                yield return set;
            }
        }

        /// <summary>
        /// Imports data from a deserialization object.
        /// </summary>
        /// <param name="serial"></param>
        public void Import(SerializableImplantSetCollection serial)
        {
            if (serial == null)
                return;

            ActiveClone.Import(serial.ActiveClone);

            m_cloneSets.Clear();
            foreach (SerializableSettingsImplantSet serialSet in serial.JumpClones)
            {
                ImplantSet set = new ImplantSet(m_owner, serialSet.Name);
                set.Import(serialSet);
                m_cloneSets.Add(set);
            }

            m_customSets.Clear();
            foreach (SerializableSettingsImplantSet serialSet in serial.CustomSets)
            {
                ImplantSet set = new ImplantSet(m_owner, serialSet.Name);
                set.Import(serialSet);
                m_customSets.Add(set);
            }

            // Imports selection
            m_current = this[serial.SelectedIndex];

            EveMonClient.OnCharacterImplantSetCollectionChanged();
        }

        /// <summary>
        /// Imports data from an API serialization object provided by CCP.
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(SerializableAPICharacterSheet serial)
        {
            if (serial == null)
                return;

            // Import the active clone implants
            ActiveClone.Import(serial.Implants);

            m_cloneSets.Clear();
            foreach (SerializableCharacterJumpClone jumpClone in serial.JumpClones)
            {
                List<SerializableNewImplant> cloneImplants =
                    serial.JumpCloneImplants.Where(x => x.JumpCloneID == jumpClone.JumpCloneID)
                        .Select(cloneImplant => new SerializableNewImplant
                        {
                            ID = cloneImplant.TypeID,
                            Name = cloneImplant.TypeName
                        }).ToList();

                ImplantSet set = new ImplantSet(m_owner, jumpClone.CloneName);
                set.Import(cloneImplants);
                m_cloneSets.Add(set);
            }

            EveMonClient.OnCharacterImplantSetCollectionChanged();
        }

        /// <summary>
        /// Exports this collection to a serialization object.
        /// </summary>
        /// <returns></returns>
        public SerializableImplantSetCollection Export()
        {
            SerializableImplantSetCollection serial = new SerializableImplantSetCollection { ActiveClone = ActiveClone.Export() };
            serial.JumpClones.AddRange(m_cloneSets.Select(x => x.Export()));
            serial.CustomSets.AddRange(m_customSets.Select(x => x.Export()));
            serial.SelectedIndex = Enumerate().IndexOf(m_current);
            return serial;
        }
    }
}