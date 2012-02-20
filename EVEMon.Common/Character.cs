using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a base class for character
    /// </summary>
    [EnforceUIThreadAffinity]
    public abstract class Character : BaseCharacter
    {
        // Character
        private string m_name;

        // Attributes
        private readonly CharacterAttribute[] m_attributes = new CharacterAttribute[5];

        // Skill Point Caching
        private DateTime m_skillPointTotalUpdated = DateTime.MinValue;
        private int m_lastSkillPointTotal;


        #region Initialization

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="identity">The identitiy for this character</param>
        /// <param name="guid"></param>
        protected Character(CharacterIdentity identity, Guid guid)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");

            CharacterID = identity.CharacterID;
            m_name = identity.CharacterName;
            CorporationID = identity.CorporationID;
            CorporationName = identity.CorporationName;

            Identity = identity;
            Guid = guid;

            Corporation = new Corporation(this);

            SkillGroups = new SkillGroupCollection(this);
            Skills = new SkillCollection(this);

            EmploymentHistory = new EmploymentRecordCollection(this);
            CertificateCategories = new CertificateCategoryCollection(this);
            CertificateClasses = new CertificateClassCollection(this);
            Certificates = new CertificateCollection(this);
            ImplantSets = new ImplantSetCollection(this);
            Plans = new PlanCollection(this);

            for (int i = 0; i < m_attributes.Length; i++)
            {
                m_attributes[i] = new CharacterAttribute(this, (EveAttribute)i);
            }

            UISettings = new CharacterUISettings();

        }

        #endregion


        #region Bio

        /// <summary>
        /// Gets a global identifier for this character.
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// Gets the identity for this character.
        /// </summary>
        public CharacterIdentity Identity { get; protected set; }

        /// <summary>
        /// Gets or sets true if the character is monitored and displayed on the main window.
        /// </summary>
        public bool Monitored
        {
            get { return EveMonClient.MonitoredCharacters.Contains(this); }
            set { EveMonClient.MonitoredCharacters.OnCharacterMonitoringChanged(this, value); }
        }

        /// <summary>
        /// Gets the ID for this character.
        /// </summary>
        public long CharacterID { get; protected set; }

        /// <summary>
        /// Gets or sets the source's name.
        /// By default, it's the character's name but it may be overriden to help distinct tabs on the main window.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                if (m_name == value)
                    return;

                m_name = value;
                EveMonClient.OnCharacterUpdated(this);
            }
        }

        /// <summary>
        /// Gets an adorned name, with (file), (url) or (cached) labels.
        /// </summary>
        public virtual string AdornedName
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the character's birthday.
        /// </summary>
        public DateTime Birthday { get; private set; }

        /// <summary>
        /// Gets the character's race.
        /// </summary>
        public string Race { get; private set; }

        /// <summary>
        /// Gets the character's bloodline.
        /// </summary>
        public string Bloodline { get; private set; }

        /// <summary>
        /// Gets the character's ancestry.
        /// </summary>
        public string Ancestry { get; private set; }

        /// <summary>
        /// Gets the character's gender.
        /// </summary>
        public string Gender { get; private set; }

        /// <summary>
        /// Gets or sets the corporation.
        /// </summary>
        /// <value>The corporation.</value>
        public Corporation Corporation { get; private set; }

        /// <summary>
        /// Gets the id of the character's corporation.
        /// </summary>
        public long CorporationID { get; set; }

        /// <summary>
        /// Gets the name of the character's corporation.
        /// </summary>
        public string CorporationName { get; set; }

        /// <summary>
        /// Gets the name of the character's alliance.
        /// </summary>
        public string AllianceName { get; private set; }

        /// <summary>
        /// Gets the id of the character's alliance.
        /// </summary>
        public int AllianceID { get; private set; }

        /// <summary>
        /// Gets the name of the clone.
        /// </summary>
        /// <value>The name of the clone.</value>
        public string CloneName { get; private set; }

        /// <summary>
        /// Gets the clone's capacity.
        /// </summary>
        /// <value>The clone skill points.</value>
        public int CloneSkillPoints { get; private set; }

        /// <summary>
        /// Gets the current character's wallet balance.
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// Gets true whether the portrait is currently updating.
        /// </summary>
        internal bool IsUpdatingPortrait { get; set; }

        /// <summary>
        /// Gets true when the character is in a NPC corporation, false otherwise.
        /// </summary>
        public bool IsInNPCCorporation
        {
            get { return StaticGeography.AllStations.Any(x => x.CorporationID == CorporationID); }
        }

        #endregion


        #region Info

        /// <summary>
        /// Gets the character's ship name.
        /// </summary>
        public string ShipName { get; private set; }

        /// <summary>
        /// Gets the character's shipType name.
        /// </summary>
        public string ShipTypeName { get; private set; }

        /// <summary>
        /// Gets the character's last known location.
        /// </summary>
        public string LastKnownLocation { get; private set; }

        /// <summary>
        /// Gets the character's security status.
        /// </summary>
        public double SecurityStatus { get; private set; }

        /// <summary>
        /// Gets or sets the character's  employment history.
        /// </summary>
        public EmploymentRecordCollection EmploymentHistory { get; private set; }

        /// <summary>
        /// Gets the character's last known station location.
        /// </summary>
        public Station LastKnownStation
        {
            get { return Station.GetByName(LastKnownLocation); }
        }

        /// <summary>
        /// Gets the character's last known solar system location.
        /// </summary>
        public SolarSystem LastKnownSolarSystem
        {
            get { return StaticGeography.GetSolarSystemByName(LastKnownLocation); }
        }


        #endregion


        #region Attributes

        /// <summary>
        /// Gets the base attribute value for the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to retrieve.</param>
        /// <returns></returns>
        protected override ICharacterAttribute GetAttribute(EveAttribute attribute)
        {
            return m_attributes[(int)attribute];
        }

        #endregion


        #region Implants

        /// <summary>
        /// Gets the implants sets of the character and its clones.
        /// </summary>
        public ImplantSetCollection ImplantSets { get; private set; }

        /// <summary>
        /// Gets the current implants' bonuses.
        /// </summary>
        public ImplantSet CurrentImplants
        {
            get { return ImplantSets.Current; }
        }

        #endregion


        #region Skills

        /// <summary>
        /// Gets the collection of skills.
        /// </summary>
        public SkillCollection Skills { get; private set; }

        /// <summary>
        /// Gets the collection of skill groups.
        /// </summary>
        public SkillGroupCollection SkillGroups { get; private set; }

        /// <summary>
        /// Gets the total skill points for this character.
        /// </summary>
        /// <returns></returns>
        protected override int TotalSkillPoints
        {
            get
            {
                // We only do the calculation once every second to avoid
                // excessive CPU utilization.
                if (m_skillPointTotalUpdated > DateTime.UtcNow.AddSeconds(-1))
                    return m_lastSkillPointTotal;

                m_lastSkillPointTotal = Skills.Sum(skill => skill.SkillPoints);
                m_skillPointTotalUpdated = DateTime.UtcNow;

                return m_lastSkillPointTotal;
            }
        }

        /// <summary>
        /// Gets the number of skills this character knows.
        /// </summary>
        public int KnownSkillCount
        {
            get { return Skills.Count(skill => skill.IsKnown); }
        }

        /// <summary>
        /// Gets the number of skills currently known at the same level than the one specified.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetSkillCountAtLevel(int level)
        {
            return Skills.Where(x => x.IsKnown).Count(skill => skill.LastConfirmedLvl == level);
        }

        /// <summary>
        /// Gets the level of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public override int GetSkillLevel(StaticSkill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            return Skills[skill.ID].Level;
        }

        /// <summary>
        /// Gets the level of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public override int GetSkillPoints(StaticSkill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            return Skills[skill.ID].SkillPoints;
        }

        #endregion


        #region Certificates

        /// <summary>
        /// Gets the collection of certificate categories.
        /// </summary>
        public CertificateCategoryCollection CertificateCategories { get; private set; }

        /// <summary>
        /// Gets the collection of certificate classes.
        /// </summary>
        public CertificateClassCollection CertificateClasses { get; private set; }

        /// <summary>
        /// Gets the collection of certificates.
        /// </summary>
        public CertificateCollection Certificates { get; private set; }

        #endregion


        #region Plans

        /// <summary>
        /// Gets the collection of plans.
        /// </summary>
        public PlanCollection Plans { get; private set; }

        #endregion


        #region Training

        /// <summary>
        /// Gets true when the character is currently training, false otherwise.
        /// </summary>
        public virtual bool IsTraining
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the skill currently in training.
        /// </summary>
        public virtual QueuedSkill CurrentlyTrainingSkill
        {
            get { return null; }
        }

        #endregion


        #region Importation / exportation

        /// <summary>
        /// Create a serializable character sheet for this character.
        /// </summary>
        /// <returns></returns>
        public abstract SerializableSettingsCharacter Export();

        /// <summary>
        /// Fetches the data to the given serialization object, used by inheritors.
        /// </summary>
        /// <param name="serial">The serial.</param>
        protected void Export(SerializableSettingsCharacter serial)
        {
            if (serial == null)
                throw new ArgumentNullException("serial");

            serial.Guid = Guid;
            serial.ID = Identity.CharacterID;
            serial.Name = m_name;
            serial.Birthday = Birthday;
            serial.Race = Race;
            serial.Ancestry = Ancestry;
            serial.Gender = Gender;
            serial.BloodLine = Bloodline;
            serial.CorporationName = CorporationName;
            serial.CorporationID = CorporationID;
            serial.AllianceName = AllianceName;
            serial.AllianceID = AllianceID;
            serial.CloneSkillPoints = CloneSkillPoints;
            serial.CloneName = CloneName;
            serial.Balance = Balance;

            // Info
            serial.ShipName = ShipName;
            serial.ShipTypeName = ShipTypeName;
            serial.SecurityStatus = SecurityStatus;
            serial.LastKnownLocation = LastKnownLocation;

            // Employment History
            serial.EmploymentHistory.AddRange(EmploymentHistory.Export());

            // Attributes
            serial.Attributes.Intelligence = Intelligence.Base;
            serial.Attributes.Perception = Perception.Base;
            serial.Attributes.Willpower = Willpower.Base;
            serial.Attributes.Charisma = Charisma.Base;
            serial.Attributes.Memory = Memory.Base;

            // Implants sets
            serial.ImplantSets = ImplantSets.Export();

            // Skills
            serial.Skills.AddRange(Skills.Export());

            // Certificates
            serial.Certificates.AddRange(Certificates.Export());
        }

        /// <summary>
        /// Imports data from the given character sheet informations.
        /// </summary>
        /// <param name="serial">The serialized character sheet</param>
        internal void Import(APIResult<SerializableAPICharacterSheet> serial)
        {
            Import(serial.Result);
            EveMonClient.OnCharacterUpdated(this);
        }

        /// <summary>
        /// Imports data from the given character sheet informations.
        /// </summary>
        /// <param name="serial">The serialized character sheet</param>
        protected void Import(SerializableSettingsCharacter serial)
        {
            if (serial == null)
                throw new ArgumentNullException("serial");

            Import((SerializableCharacterSheetBase)serial);

            // Implants
            ImplantSets.Import(serial.ImplantSets);
        }

        /// <summary>
        /// Imports data from the given character sheet informations.
        /// </summary>
        /// <param name="serial">The serialized character sheet</param>
        private void Import(SerializableAPICharacterSheet serial)
        {
            Import((SerializableCharacterSheetBase)serial);

            // Implants
            ImplantSets.Import(serial.Implants);
        }

        /// <summary>
        /// Imports data from the given character info.
        /// </summary>
        /// <param name="serial">The serialized character info</param>
        internal void Import(SerializableAPICharacterInfo serial)
        {
            ShipName = serial.ShipName;
            ShipTypeName = serial.ShipTypeName;
            SecurityStatus = serial.SecurityStatus;
            LastKnownLocation = serial.LastKnownLocation;

            EmploymentHistory.Import(serial.EmploymentHistory);

            EveMonClient.OnCharacterInfoUpdated(this);
        }

        /// <summary>
        /// Imports data from the given character sheet informations.
        /// </summary>
        /// <param name="serial">The serialized character sheet</param>
        private void Import(SerializableCharacterSheetBase serial)
        {
            // Bio
            m_name = serial.Name;
            Balance = serial.Balance;
            Gender = serial.Gender;
            Race = serial.Race;
            Bloodline = serial.BloodLine;
            Ancestry = serial.Ancestry;
            Birthday = serial.Birthday;
            CorporationName = serial.CorporationName;
            CorporationID = serial.CorporationID;
            AllianceName = serial.AllianceName;
            AllianceID = serial.AllianceID;
            CloneName = serial.CloneName;
            CloneSkillPoints = serial.CloneSkillPoints;

            if (serial is SerializableSettingsCharacter)
            {
                // Info
                ShipName = serial.ShipName;
                ShipTypeName = serial.ShipTypeName;
                SecurityStatus = serial.SecurityStatus;
                LastKnownLocation = serial.LastKnownLocation;

                // Employment History
                EmploymentHistory.Import(serial.EmploymentHistory);
            }

            // Attributes
            m_attributes[(int)EveAttribute.Intelligence].Base = serial.Attributes.Intelligence;
            m_attributes[(int)EveAttribute.Perception].Base = serial.Attributes.Perception;
            m_attributes[(int)EveAttribute.Willpower].Base = serial.Attributes.Willpower;
            m_attributes[(int)EveAttribute.Charisma].Base = serial.Attributes.Charisma;
            m_attributes[(int)EveAttribute.Memory].Base = serial.Attributes.Memory;

            // Skills
            Skills.Import(serial.Skills, serial is SerializableAPICharacterSheet);

            // Certificates
            Certificates.Import(serial.Certificates);
        }

        /// <summary>
        /// Imports the given plans.
        /// </summary>
        /// <param name="plans"></param>
        internal void ImportPlans(IEnumerable<SerializablePlan> plans)
        {
            Plans.Import(plans);
        }

        /// <summary>
        /// Export the plans to the given list.
        /// </summary>
        /// <param name="list"></param>
        internal void ExportPlans(List<SerializablePlan> list)
        {
            list.AddRange(Plans.Select(plan => plan.Export()));
        }

        #endregion


        /// <summary>
        /// Gets the UI settings for this character.
        /// </summary>
        public CharacterUISettings UISettings { get; internal set; }

        /// <summary>
        /// Gets a unique hashcode for this character.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }
    }
}