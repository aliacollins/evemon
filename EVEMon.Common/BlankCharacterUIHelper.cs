using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public static class BlankCharacterUIHelper
    {
        #region Fields

        private static readonly Dictionary<int, int> s_allRaceSkills = new Dictionary<int, int>
        {
            { DBConstants.ElectronicsSkillID, 3 },
            { DBConstants.EngineeringSkillID, 3 },
            { DBConstants.ShieldOperationSkillID, 2 },
            { DBConstants.GunnerySkillID, 2 },
            { DBConstants.MiningSkillID, 2 },
            { DBConstants.MechanicSkillID, 2 },
            { DBConstants.NavigationSkillID, 3 },
            { DBConstants.ScienceSkillID, 3 },
            { DBConstants.SpaceshipCommandSkillID, 3 }
        };

        private static readonly Dictionary<int, int> s_amarrRaceSkills = new Dictionary<int, int>
        {
            { DBConstants.SmallEnergyTurretSkillID, 3 },
            { DBConstants.AmarrFrigateSkillID, 2 }
        };

        private static readonly Dictionary<int, int> s_caldariRaceSkills = new Dictionary<int, int>
        {
            { DBConstants.SmallHybridTurretSkillID, 3 },
            { DBConstants.CaldariFrigateSkillID, 2 }
        };

        private static readonly Dictionary<int, int> s_gallenteRaceSkills = new Dictionary<int, int>
        {
            { DBConstants.SmallHybridTurretSkillID, 3 },
            { DBConstants.GallenteFrigateSkillID, 2 }
        };

        private static readonly Dictionary<int, int> s_minmatarRaceSkills =
            new Dictionary<int, int>
            {
                { DBConstants.SmallProjectileTurretSkillID, 3 },
                { DBConstants.MinmatarFrigateSkillID, 2 }
            };

        private static string s_filename;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the name of the character.
        /// </summary>
        /// <value>
        /// The name of the character.
        /// </value>
        public static string CharacterName { get; set; }

        /// <summary>
        /// Gets or sets the race.
        /// </summary>
        /// <value>
        /// The race.
        /// </value>
        public static Race Race { get; set; }

        /// <summary>
        /// Sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public static Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the bloodline.
        /// </summary>
        /// <value>
        /// The bloodline.
        /// </value>
        public static Bloodline Bloodline { get; set; }

        /// <summary>
        /// Sets the ancestry.
        /// </summary>
        /// <value>
        /// The ancestry.
        /// </value>
        public static Ancestry Ancestry { get; set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Creates the character.
        /// </summary>
        /// <returns></returns>
        private static SerializableCCPCharacter CreateCharacter()
        {
            SerializableCCPCharacter serial = new SerializableCCPCharacter
            {
                ID = UriCharacter.BlankCharacterID,
                Name = CharacterName,
                Birthday = DateTime.UtcNow,
                Race = Race.ToString(),
                BloodLine = Bloodline.ToString().Replace("_", "-"),
                Ancestry = Ancestry.ToString().Replace("_", " "),
                Gender = Gender.ToString(),
                CorporationName = "Blank Character's Corp",
                CorporationID = 9999999,
                Balance = 0,
                Attributes = new SerializableCharacterAttributes
                {
                    Intelligence =
                        EveConstants.CharacterBaseAttributePoints + 3,
                    Memory = EveConstants.CharacterBaseAttributePoints + 3,
                    Perception =
                        EveConstants.CharacterBaseAttributePoints + 3,
                    Willpower =
                        EveConstants.CharacterBaseAttributePoints + 3,
                    Charisma =
                        EveConstants.CharacterBaseAttributePoints + 2
                },
                ImplantSets = new SerializableImplantSetCollection
                {
                    API =
                        new SerializableSettingsImplantSet
                        { Name = "Implants from API" },
                    OldAPI = new SerializableSettingsImplantSet
                    {
                        Name = "Previous implants from the API"
                    },
                },
            };

            serial.Skills.AddRange(GetSkillsForRace());

            return serial;
        }

        /// <summary>
        /// Gets the skills for each race.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<SerializableCharacterSkill> GetSkillsForRace()
        {
            Dictionary<int, int> startingSkills = GetStartingSkills();

            return (startingSkills.Select(
                raceSkill => new
                {
                    raceSkill,
                    staticSkill = StaticSkills.GetSkillByID(raceSkill.Key)
                }).Where(raceSkill => raceSkill.staticSkill != null).Select(
                    skill => new SerializableCharacterSkill
                    {
                        ID = skill.raceSkill.Key,
                        Level = skill.raceSkill.Value,
                        Name = StaticSkills.GetSkillByID(skill.raceSkill.Key).Name,
                        Skillpoints =
                            StaticSkills.GetSkillByID(skill.raceSkill.Key).GetPointsRequiredForLevel
                                (skill.raceSkill.Value),
                        IsKnown = true,
                        OwnsBook = false,
                    }));
        }

        /// <summary>
        /// Gets the starting skills.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, int> GetStartingSkills()
        {
            Dictionary<int, int> startingSkills = new Dictionary<int, int>();

            switch (Race)
            {
                case Race.Amarr:
                    startingSkills = s_allRaceSkills.Concat(s_amarrRaceSkills).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case Race.Caldari:
                    startingSkills = s_allRaceSkills.Concat(s_caldariRaceSkills).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case Race.Gallente:
                    startingSkills = s_allRaceSkills.Concat(s_gallenteRaceSkills).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case Race.Minmatar:
                    startingSkills = s_allRaceSkills.Concat(s_minmatarRaceSkills).ToDictionary(x => x.Key, x => x.Value);
                    break;
            }
            return startingSkills;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Saves the blank character.
        /// </summary>
        public static void Save(Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            SerializableCCPCharacter serial = CreateCharacter();

            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Title = "Save Blank Character";
                fileDialog.Filter = "Blank Character CCPXML (*.xml) | *.xml";
                fileDialog.FileName = String.Format(CultureConstants.DefaultCulture, "{0}.xml", serial.Name);
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                DialogResult result = fileDialog.ShowDialog();
                if (result != DialogResult.OK)
                    return;

                XmlDocument xmlDoc = (XmlDocument)Util.SerializeToXmlDocument(typeof(SerializableCCPCharacter), serial);
                string content = Util.GetXmlStringRepresentation(xmlDoc);
                FileHelper.OverwriteOrWarnTheUser(fileDialog.FileName,
                    fs =>
                    {
                        using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                        {
                            writer.Write(content);
                            writer.Flush();
                            fs.Flush();
                        }
                        return true;
                    });

                s_filename = fileDialog.FileName;
                callback.Invoke();
            }
        }

        /// <summary>
        /// Adds the blank character.
        /// </summary>
        public static void AddBlankCharacter(Action callback)
        {
            // Add blank character
            GlobalCharacterCollection.TryAddOrUpdateFromUriAsync(new Uri(s_filename),
                (sender, e) =>
                {
                    if (e == null || e.HasError)
                        return;

                    UriCharacter character = e.CreateCharacter();
                    character.Monitored = true;

                    callback.Invoke();
                });
        }

        #endregion
    }
}