using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a certificate from a character's point of view.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class Certificate
    {
        private readonly Character m_character;
        private readonly CertificateLevel[] m_levels;               


        #region Initialization, importation, exportation and update

        /// <summary>
        /// Constructor at character's initialization time.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        /// <param name="certClass"></param>
        internal Certificate(Character character, StaticCertificate src, CertificateClass certClass)
        {            
            StaticData = src;
            Class = certClass;
            m_character = character;            

            m_levels = new CertificateLevel[5];
            
            foreach (var skill in src.PrerequisiteSkills)
            {
                m_levels[(int)skill.Key] = new CertificateLevel(skill.Key, this, character);
                m_levels[(int)skill.Key].CompleteInitialization(skill.Value.ToCharacter(character).ToList());               
            }           
        }

        /// <summary>
        /// Gets the static data associated with this certificate.
        /// </summary>
        public StaticCertificate StaticData { get; private set; }

        /// <summary>
        /// Try to update the certificate's status. 
        /// </summary>
        /// <returns>True if the status was updated, false otherwise.</returns>
        internal bool TryUpdateCertificateStatus()
        {
            bool updated = false;
            foreach (var level in m_levels)
            {
                updated |= level.TryUpdateCertificateStatus();
            }  
            
            return updated;
        }

        #endregion


        #region Core properties

        /// <summary>
        /// Gets this certificate's id.
        /// </summary>
        public int ID
        {
            get { return StaticData.ID; }
        }

        /// <summary>
        /// Gets this certificate's name.
        /// </summary>
        public string Name
        {
            get { return StaticData.Class.Name; }
        }

        /// <summary>
        /// Gets this certificate's description.
        /// </summary>
        public string Description
        {
            get { return StaticData.Description; }
        }

        /// <summary>
        /// Gets the class for this certificate.
        /// </summary>
        public CertificateClass Class { get; private set; }

        /// <summary>
        /// Gets the ships this certificate is recommended for.
        /// </summary>
        public IEnumerable<Item> Recommendations
        {
            get { return StaticData.Recommendations; }
        }
        
        public CertificateLevel LevelOne {  get { return m_levels[(int)CertificateGrade.LevelOne]; } }

        public CertificateLevel LevelTwo { get { return m_levels[(int)CertificateGrade.LevelTwo]; } }

        public CertificateLevel LevelThree { get { return m_levels[(int)CertificateGrade.LevelThree]; } }

        public CertificateLevel LevelFour { get { return m_levels[(int)CertificateGrade.LevelFour]; } }

        public CertificateLevel LevelFive { get { return m_levels[(int)CertificateGrade.LevelFive]; } } 

        public IEnumerable<CertificateLevel> AllLevel { get { return new List<CertificateLevel> { LevelOne, LevelTwo, LevelThree, LevelFour, LevelFive }; } }

        #endregion


        #region Helper methods and properties  

        /// <summary>
        /// Gets all the top-level prerequisite skills, including the ones from prerequisite certificates.
        /// </summary>
        public IEnumerable<SkillLevel> AllTopPrerequisiteSkills
        {
            get { return StaticData.AllTopPrerequisiteSkills.ToCharacter(m_character); }
        }

        /// <summary>
        /// Gets the required training time for the provided character to train this certificate.
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTrainingTime
        {
            get { return m_character.GetTrainingTimeToMultipleSkills(AllTopPrerequisiteSkills); }
        }

        /// <summary>
        /// Checks whether the provided skill is an immediate prerequisite.
        /// </summary>
        /// <param name="grade">The level of the certificate to check</param>
        /// <param name="skill">The skill to test</param>
        /// <param name="neededLevel">When this skill is an immediate prerequisite, this parameter will held the required level</param>
        /// <returns></returns>
        public bool HasAsImmediatePrerequisite(CertificateGrade grade, Skill skill, out Int64 neededLevel)
        {
            return m_levels[(int)grade].PrerequisiteSkills.Contains(skill, out neededLevel);
        }

        /// <summary>
        /// Gets the lowest untrained (neither granted nor claimable).
        /// Null if all certificates have been granted or are claimable.
        /// </summary>
        public CertificateLevel LowestUntrainedGrade
        {
            get
            {
                return m_levels.FirstOrDefault(cert => cert.Status != CertificateStatus.Granted);
            }
        }
        #endregion


        /// <summary>
        /// Gets a string representation of this certificate.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StaticData.ToString();
        }

        /// <summary>
        /// Implicit conversion operator to the static equivalent of this certificate.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static implicit operator StaticCertificate(Certificate cert)
        {
            return cert == null ? null : cert.StaticData;
        }
    }
}