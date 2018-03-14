using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Base class for the ESI character sheet.
    /// </summary>
    [DataContract]
    public sealed class EsiAPICharacterSheet
    {
        private DateTime birthday;

        public EsiAPICharacterSheet()
        {
            birthday = DateTime.MinValue;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description", EmitDefaultValue = false, IsRequired = false)]
        public string Description { get; set; }

        [DataMember(Name = "birthday")]
        private string BirthdayJson
        {
            get { return birthday.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    birthday = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "race_id")]
        public int RaceID { get; set; }

        [DataMember(Name = "bloodline_id")]
        public int BloodLineID { get; set; }

        [DataMember(Name = "ancestry_id", EmitDefaultValue = false, IsRequired = false)]
        public int AncestryID { get; set; }

        // One of: female, male
        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "corporation_id")]
        public long CorporationID { get; set; }

        [DataMember(Name = "alliance_id", EmitDefaultValue = false, IsRequired = false)]
        public long AllianceID { get; set; }

        [DataMember(Name = "faction_id", EmitDefaultValue = false, IsRequired = false)]
        public int FactionID { get; set; }

        [DataMember(Name = "security_status")]
        public double SecurityStatus { get; set; }

        /// <summary>
        /// The date and time the character was created.
        /// </summary>
        [IgnoreDataMember]
        public DateTime Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                if (value > DateTime.MinValue)
                    birthday = value;
            }
        }

        public SerializableAPICharacterSheet ToXMLItem(long characterID, EsiAPIClones clones,
                                                       EsiAPIJumpFatigue fatigue, EsiAPISkills skills,
                                                       EsiAPIEmploymentHistory history,
                                                       EsiAPIAttributes attribs, decimal balance,
                                                       EsiAPIShip ship, IEnumerable<int> implants)
        {
            clones.ThrowIfNull(nameof(clones));
            fatigue.ThrowIfNull(nameof(fatigue));
            skills.ThrowIfNull(nameof(skills));
            history.ThrowIfNull(nameof(history));
            attribs.ThrowIfNull(nameof(attribs));
            ship.ThrowIfNull(nameof(ship));

            // Convert integers to string names
            Race myRace = (Race)RaceID;
            Bloodline myBloodline = (Bloodline)BloodLineID;
            Ancestry myAncestry = (Ancestry)AncestryID;

            // If there is a timed respec date, subtract a year for the last one
            // CCP year = 365.0 days
            DateTime lastRespec = DateTime.MinValue, nextRespec = attribs.RemapCooldownDate;
            if (nextRespec > DateTime.MinValue)
                lastRespec = nextRespec.Subtract(TimeSpan.FromDays(365.0));

            var ret = new SerializableAPICharacterSheet()
            {
                ID = characterID,
                Name = Name,
                Birthday = Birthday,
                BloodLine = myBloodline.ToString().Replace('_', '-'),
                Race = myRace.ToString(),
                Ancestry = myAncestry.ToString().Replace('_', ' '),
                Gender = Gender.ToTitleCase(),
                CorporationID = CorporationID,
                AllianceID = AllianceID,
                FactionID = FactionID,
                FreeSkillPoints = skills.UnallocatedSP,
                FreeRespecs = attribs.BonusRemaps,
                LastRespecDate = attribs.LastRemap,
                JumpLastUpdateDate = fatigue.LastUpdate,
                JumpFatigueDate = fatigue.FatigueExpires,
                JumpActivationDate = fatigue.LastJump,
                LastTimedRespec = lastRespec,
                Balance = balance,
                CloneJumpDate = clones.LastCloneJump,
                SecurityStatus = SecurityStatus,
                // LastKnownLocation is not used since we have a better way now
                ShipTypeName = StaticItems.GetItemName(ship.ShipTypeID),
                ShipName = ship.ShipName,
                RemoteStationDate = clones.LastStationChange,
                HomeStationID = clones.HomeLocation.LocationID,
            };

            // Skills
            foreach (var skill in skills.Skills)
                ret.Skills.Add(skill.ToXMLItem());

            // Jump clones
            foreach (var clone in clones.JumpClones)
            {
                int cloneID = clone.JumpCloneID;
                ret.JumpClones.Add(clone.ToXMLItem());

                // Jump clone implants
                foreach (int implant in clone.Implants)
                    ret.JumpCloneImplants.Add(new SerializableCharacterJumpCloneImplant()
                    {
                        JumpCloneID = cloneID,
                        TypeID = implant,
                        TypeName = StaticItems.GetItemName(implant)
                    });
            }

            // Attributes
            ret.Attributes = new SerializableCharacterAttributes()
            {
                Charisma = attribs.Charisma,
                Intelligence = attribs.Intelligence,
                Memory = attribs.Memory,
                Perception = attribs.Perception,
                Willpower = attribs.Willpower
            };

            // Implants
            foreach (int implant in implants)
                ret.Implants.Add(new SerializableNewImplant()
                {
                    ID = implant,
                    Name = StaticItems.GetItemName(implant)
                });

            return ret;
        }
    }
}
