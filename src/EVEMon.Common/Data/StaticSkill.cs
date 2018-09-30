using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a skill definition. Characters use their own representation, through <see cref="Skill"/>.
    /// </summary>
    public sealed class StaticSkill : IStaticSkill
    {
        private static StaticSkill s_unknownStaticSkill;


        #region Constructors

        /// <summary>
        /// Constructor for an unknown static skill.
        /// </summary>
        private StaticSkill()
        {
            ID = Int32.MaxValue;
            Name = $"{EveMonConstants.UnknownText} Skill";
            Description = "An unknown skill.";
            ArrayIndex = Int16.MaxValue;
            Prerequisites = new Collection<StaticSkillLevel>();
            PrimaryAttribute = EveAttribute.None;
            SecondaryAttribute = EveAttribute.None;
            Group = StaticSkillGroup.UnknownStaticSkillGroup;
            FormattedCost = Cost.ToNumericString(0);
            if (alphaLimit.ContainsKey(ID))
            {
                AlphaLimit = alphaLimit[ID];
            }
       }

        /// <summary>
        /// Deserialization constructor from datafiles.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="src"></param>
        /// <param name="arrayIndex"></param>
        internal StaticSkill(StaticSkillGroup group, SerializableSkill src, int arrayIndex)
        {
            ID = src.ID;
            Cost = src.Cost;
            Rank = src.Rank;
            IsPublic = src.Public;
            Name = src.Name;
            Description = src.Description;
            PrimaryAttribute = src.PrimaryAttribute;
            SecondaryAttribute = src.SecondaryAttribute;
            IsTrainableOnTrialAccount = src.CanTrainOnTrial;
            ArrayIndex = arrayIndex;
            Group = group;
            Prerequisites = new Collection<StaticSkillLevel>();
            FormattedCost = Cost.ToNumericString(0);
            if (alphaLimit.ContainsKey(ID))
            {
                AlphaLimit = alphaLimit[ID];
            }
        }

        #endregion


        #region Initialization

        /// <summary>
        /// Completes the initialization by updating the prequisites and checking trainability on trial account.
        /// </summary>
        internal void CompleteInitialization(IEnumerable<SerializableSkillPrerequisite> prereqs)
        {
            if (prereqs == null)
                return;

            // Create the prerequisites list
            Prerequisites.AddRange(prereqs.Select(x => new StaticSkillLevel(x.GetSkill(), x.Level)));

            if (!IsTrainableOnTrialAccount)
                return;

            // Check trainableOnTrialAccount on its childrens to be sure it's really trainable
            if (Prerequisites.All(prereq => prereq.Skill.IsTrainableOnTrialAccount))
                return;

            IsTrainableOnTrialAccount = false;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the ID of this skill.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets a zero-based index for skills (allow the use of arrays to optimize computations).
        /// </summary>
        public int ArrayIndex { get; }

        /// <summary>
        /// Gets the name of this skill (interned).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of this skill.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the rank of this skill.
        /// </summary>
        public Int64 Rank { get; }

        /// <summary>
        /// Gets the skill's cost.
        /// </summary>
        public Int64 Cost { get; }

        /// <summary>
        /// Gets the skill group this skill is part of.
        /// </summary>
        public StaticSkillGroup Group { get; }

        /// <summary>
        /// Gets false when the skill is not for sale by any NPC (CCP never published it or removed it from the game, it's inactive).
        /// </summary>
        public bool IsPublic { get; }

        /// <summary>
        /// Gets the primary attribute of this skill.
        /// </summary>
        public EveAttribute PrimaryAttribute { get; }

        /// <summary>
        /// Gets the secondary attribute of this skill.
        /// </summary>
        public EveAttribute SecondaryAttribute { get; }

        /// <summary>
        /// Get whether skill is trainable on a trial account.
        /// </summary>
        public bool IsTrainableOnTrialAccount { get; private set; }

        /// <summary>
        /// Get the level limit for an alpha clone.
        /// </summary>
        public long AlphaLimit { get; private set; }

        /// <summary>
        /// Gets the prerequisites a character must satisfy before it can be trained.
        /// </summary>
        public Collection<StaticSkillLevel> Prerequisites { get; }

        /// <summary>
        /// Gets a formatted representation of the price.
        /// </summary>
        public string FormattedCost { get; }

        /// <summary>
        /// Gets all the prerequisites. I.e, for eidetic memory, it will return <c>{ instant recall IV }</c>.
        /// The order matches the hierarchy but skills are not duplicated and are systematically trained to the highest required level.
        /// For example, if some skill is required to lv3 and, later, to lv4, this first time it is encountered, lv4 is returned.
        /// </summary>
        /// <value>All prerequisites.</value>
        /// <remarks>Please note they may be redundancies.</remarks>
        public IEnumerable<StaticSkillLevel> AllPrerequisites
        {
            get
            {
                Int64[] highestLevels = new Int64[StaticSkills.ArrayIndicesCount];
                List<StaticSkillLevel> list = new List<StaticSkillLevel>();

                // Fill the array
                foreach (StaticSkillLevel prereq in Prerequisites)
                {
                    StaticSkillEnumerableExtensions.FillPrerequisites(highestLevels, list, prereq, true);
                }

                // Return the result
                foreach (StaticSkillLevel newItem in list.Where(x => highestLevels[x.Skill.ArrayIndex] != 0))
                {
                    yield return new StaticSkillLevel(newItem.Skill, highestLevels[newItem.Skill.ArrayIndex]);
                    highestLevels[newItem.Skill.ArrayIndex] = 0;
                }
            }
        }

        /// <summary>
        /// Gets the unknown static skill.
        /// </summary>
        /// <value>
        /// The unknown static skill.
        /// </value>
        public static StaticSkill UnknownStaticSkill => s_unknownStaticSkill ?? (s_unknownStaticSkill = new StaticSkill());

        private static Dictionary<int, int> alphaLimit = new Dictionary<int, int>()
        {
            {3339, 4}, // Amarr Battleship
            {33095, 4}, // Amarr Battlecruiser
            {3335, 4}, // Amarr Cruiser
            {33091, 4}, // Amarr Destroyer
            {3331, 4}, // Amarr Frigate
            {3343, 1}, // Amarr Industrial
            {3338, 4}, // Caldari Battleship
            {33096, 4}, // Caldari Battlecruiser
            {3334, 4}, // Caldari Cruiser
            {33092, 4}, // Caldari Destroyer
            {3330, 4}, // Caldari Frigate
            {3342, 1}, // Caldari Industrial
            {3336, 4}, // Gallente Battleship
            {33097, 4}, // Gallente Battlecruiser
            {3332, 4}, // Gallente Cruiser
            {33093, 4}, // Gallente Destroyer
            {3328, 4}, // Gallente Frigate
            {3340, 1}, // Gallente Industrial
            {3337, 4}, // Minmatar Battleship
            {33098, 4}, // Minmatar Battlecruiser
            {3333, 4}, // Minmatar Cruiser
            {33094, 4}, // Minmatar Destroyer
            {3329, 4}, // Minmatar Frigate
            {3341, 1}, // Minmatar Industrial
            {32918, 4}, // Mining Frigate
            {3327, 4}, // Spaceship Command
            {3452, 3}, // Acceleration Control
            {3450, 3}, // Afterburner
            {3453, 3}, // Evasive Maneuvering
            {3454, 3}, // High Speed Maneuvering
            {3449, 4}, // Navigation
            {3455, 3}, // Warp Drive Operation
            {11207, 3}, // Advanced Weapon Upgrades
            {3426, 5}, // CPU Management
            {3423, 4}, // Capacitor Emission Systems
            {3418, 4}, // Capacitor Management
            {3417, 3}, // Capacitor Systems Operation
            {3432, 5}, // Electronics Upgrades
            {3424, 5}, // Energy Grid Upgrades
            {3421, 2}, // Energy Pulse Weapons
            {3413, 5}, // Power Grid Management
            {28164, 4}, // Thermodynamics
            {3318, 5}, // Weapon Upgrades
            {33078, 1}, // Armor Layering
            {22806, 2}, // EM Armor Compensation
            {22807, 2}, // Explosive Armor Compensation
            {3394, 5}, // Hull Upgrades
            {22808, 2}, // Kinetic Armor Compensation
            {3392, 5}, // Mechanics
            {16069, 3}, // Remote Armor Repair Systems
            {27902, 2}, // Remote Hull Repair Systems
            {3393, 5}, // Repair Systems
            {22809, 2}, // Thermal Armor Compensation
            {12365, 2}, // EM Shield Compensation
            {12367, 2}, // Explosive Shield Compensation
            {12366, 2}, // Kinetic Shield Compensation
            {3422, 3}, // Shield Emission Systems
            {21059, 4}, // Shield Compensation
            {3419, 4}, // Shield Management
            {3416, 4}, // Shield Operation
            {3425, 4}, // Shield Upgrades
            {3420, 4}, // Tactical Shield Manipulation
            {11566, 2}, // Thermal Shield Compensation
            {3428, 3}, // Long Range Targeting
            {3431, 3}, // Signature Analysis
            {3429, 4}, // Target Management
            {3316, 4}, // Controlled bursts
            {3300, 5}, // Gunnery
            {3309, 4}, // Large Energy Turret
            {3307, 4}, // Large Hybrid Turret
            {3308, 4}, // Large Projectile Turret
            {12202, 3}, // Medium Artillery Specialization
            {12208, 3}, // Medium Autocannon Specialization
            {12204, 3}, // Medium Beam Laser Specialization
            {12211, 3}, // Medium Blaster Specialization
            {3306, 5}, // Medium Energy Turret
            {3304, 5}, // Medium Hybrid Turret
            {3305, 5}, // Medium Projectile Turret
            {12214, 3}, // Medium Pulse Laser Specialization
            {12206, 3}, // Medium Rail Specialization
            {3312, 4}, // Motion Prediction
            {3310, 4}, // Rapid Firing
            {3311, 4}, // Sharpshooter
            {12201, 3}, // Small Artillery Specialization
            {11084, 3}, // Small Autocannon Specialization
            {11083, 3}, // Small Beam Laser Specialization
            {12210, 3}, // Small Blaster Specialization
            {3303, 5}, // Small Energy Turret
            {3301, 5}, // Small Hybrid Turret
            {3302, 5}, // Small Projectile Turret
            {12213, 3}, // Small Pulse Laser Specialization
            {11082, 3}, // Small Rail Specialization
            {3315, 4}, // Surgical Strike
            {3317, 4}, // Trajectory Analysis
            {3326, 4}, // Cruise Missiles
            {20312, 3}, // Guided Missile Precision
            {25718, 3}, // Heavy Assault Missile Specialization
            {25719, 5}, // Heavy Assault Missiles
            {20211, 3}, // Heavy Missile Specialization
            {3324, 5}, // Heavy Missiles
            {20210, 3}, // Light Missile Specialization
            {3321, 5}, // Light Missiles
            {12441, 4}, // Missile Bombardment
            {3319, 5}, // Missile Launcher Operation
            {12442, 2}, // Missile Projection
            {21071, 4}, // Rapid Launch
            {20209, 3}, // Rocket Specialization
            {3320, 5}, // Rockets
            {20314, 3}, // Target Navigation Prediction
            {3325, 4}, // Torpedoes
            {20315, 3}, // Warhead Upgrades
            {3437, 4}, // Drone Avionics
            {23618, 4}, // Drone Durability
            {3442, 4}, // Drone Interfacing
            {12305, 4}, // Drone Navigation
            {23606, 4}, // drone Sharpshooting
            {3436, 5}, // Drones
            {24241, 5}, // Light Drone Operation
            {33699, 5}, // Medium Drone Operation
            {3439, 2}, // Repair Drone Operation
            {3441, 4}, // Heavy Drone Operation
            {12484, 2}, // Amarr Drone Specialization
            {12487, 2}, // Caldari Drone Specialization
            {12486, 2}, // Gallente Drone Specialization
            {12485, 2}, // Minmatar Drone Specialization
            {3427, 4}, // Electronic Warfare
            {3435, 4}, // Propulsion Jamming
            {3434, 4}, // Weapon Disruption
            {3433, 3}, // Sensor Linking
            {19921, 3}, // Target Painting
            {13278, 3}, // Archaeology
            {25811, 2}, // Astrometric Acquisition
            {25739, 2}, // Astrometric Rangefinding
            {3412, 3}, // Astrometrics
            {21718, 3}, // Hacking
            {3551, 3}, // Survey
            {26253, 3}, // Armor Rigging
            {26254, 3}, // Astronautics Rigging
            {26259, 3}, // Hybrid Weapon Rigging
            {26260, 3}, // Launcher Rigging
            {26257, 3}, // Shield Rigging
            {26261, 3}, // Projectile Weapon Rigging
            {26255, 3}, // Drones Rigging
            {26256, 3}, // Electronic superiority Rigging
            {26258, 3}, // Energy Weapon Rigging
            {26252, 3}, // Jury Rigging
            {3359, 2}, // Connections
            {3361, 2}, // Criminal Connections
            {3357, 3}, // Diplomacy
            {3894, 2}, // Distribution Connections
            {3893, 2}, // Mining Connections
            {3356, 2}, // Negotiation
            {3895, 2}, // Security Connections
            {3355, 3}, // Social
            {3348, 3}, // Leadership
            {3363, 1}, // Corporation Management
            {3446, 2}, // Broker Relations
            {16598, 2}, // Marketing
            {3443, 3}, // Trade
            {3405, 3}, // Biology
            {3411, 3}, // Cybernetics
            {24242, 1}, // Infomorph Psychology
            {3380, 5}, // Industry
            {3387, 3}, // Mass Production
            {3402, 4}, // Science
            {3386, 4}, // Mining
            {16281, 2}, // Ice Harvesting
            {22578, 4}, // Mining Upgrades
            {3358, 3}, // Reprocessing
            {25544, 2}, // Gas Cloud Harvesting
            {25863, 3}, // Salvaging
            {11584, 1} // Anchoring
        };

        #endregion


        #region Public Methods - Computations

        /// <summary>
        /// Calculates the cumulative points required for a level of this skill (starting from a zero level).
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Int64 GetPointsRequiredForLevel(Int64 level)
        {
            // Much faster than the old formula. This one too may have 1pt difference here and there, only on the lv2 skills
            switch (level)
            {
                case -1:
                case 0:
                    return 0;
                case 1:
                    return 250 * Rank;
                case 2:
                    switch (Rank)
                    {
                        case 1:
                            return 1415;
                        default:
                            return (int)(Rank * 1414.3f + 0.5f);
                    }
                case 3:
                    return 8000 * Rank;
                case 4:
                    return Convert.ToInt32(Math.Ceiling(Math.Pow(2, 2.5 * level - 2.5) * 250 * Rank));
                case 5:
                    return 256000 * Rank;
                default:
                    throw new NotImplementedException($"One of our devs messed up. Skill level was {level} ?!");
            }
        }

        /// <summary>
        /// Calculates the cumulative points required for a level of this skill (starting from a zero level).
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        public Int64 GetPointsRequiredForLevelOnly(int level)
        {
            if (level == 0)
                return 0;

            return GetPointsRequiredForLevel(level) - GetPointsRequiredForLevelOnly(level - 1);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Gets this skill's representation for the provided character.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">character</exception>
        public Skill ToCharacter(Character character)
        {
            character.ThrowIfNull(nameof(character));

            return character.Skills.GetByArrayIndex(ArrayIndex);
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation for this skill (the name of the skill).
        /// </summary>
        /// <returns>Name of the Static Skill.</returns>
        public override string ToString() => Name;

        #endregion
    }
}
