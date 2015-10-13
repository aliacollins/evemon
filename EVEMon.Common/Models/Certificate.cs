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
        private readonly Dictionary<CertificateGrade, List<SkillLevel>> m_prereqSkills;        

        private bool m_initialized;


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
            Status = CertificateStatus.Untrained;

            m_prereqSkills = new Dictionary<CertificateGrade, List<SkillLevel>>();

            foreach (var skill in src.PrerequisiteSkills)
            {
                m_prereqSkills.Add(skill.Key, skill.Value.ToCharacter(character).ToList());
            }           
        }

        /// <summary>
        /// Gets the static data associated with this certificate.
        /// </summary>
        public StaticCertificate StaticData { get; private set; }

        /// <summary>
        /// Marks the certificate as granted.
        /// </summary>
        internal void MarkAsGranted()
        {
            Status = CertificateStatus.Granted;
            m_initialized = true;
        }

        /// <summary>
        /// Resets the data before we import a deserialization object.
        /// </summary>
        internal void Reset()
        {
            Status = CertificateStatus.Untrained;
            m_initialized = false;
        }

        /// <summary>
        /// Try to update the certificate's status. 
        /// </summary>
        /// <returns>True if the status was updated, false otherwise.</returns>
        internal bool TryUpdateCertificateStatus()
        {
            if (m_initialized)
                return false;

            bool claimable = true;
            bool noPrereq = true;

            // Scan prerequisite skills
            foreach (SkillLevel prereqSkill in PrerequisiteSkills)
            {
                Skill skill = prereqSkill.Skill;

                // Claimable only if the skill's level is grater or equal than the minimum level
                claimable &= (skill.Level >= prereqSkill.Level);

                // Untrainable if no prereq is satisfied
                noPrereq &= (skill.Level < prereqSkill.Level);
            }

            // Updates status
            if (claimable)
                Status = CertificateStatus.Claimable;
            else if (noPrereq)
                Status = CertificateStatus.Untrained;
            else
                Status = CertificateStatus.PartiallyTrained;
            m_initialized = true;
            return true;
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

        /// <summary>
        /// Gets the current certificate status this character has.
        /// </summary>
        public CertificateStatus Status { get; private set; }

        /// <summary>
        /// Gets the immediate prerequisite skills.
        /// </summary>
        public IEnumerable<SkillLevel> PrerequisiteSkills
        {
            get { return m_prereqSkills.SelectMany(prereq => prereq.Value).ToList(); }
        }

        #endregion


        #region Helper methods and properties

        /// <summary>
        /// Gets true whether the certificate is granted.
        /// </summary>
        public bool IsGranted
        {
            get { return Status == CertificateStatus.Granted; }
        }

        /// <summary>
        /// Gets all the top-level prerequisite skills, including the ones from prerequisite certificates.
        /// </summary>
        public IEnumerable<SkillLevel> AllTopPrerequisiteSkills
        {
            get { return StaticData.AllTopPrerequisiteSkills.ToCharacter(m_character); }
        }

        /// <summary>
        /// Gets true when the certificate can be claimed.
        /// </summary>
        public bool CanBeClaimed
        {
            get
            {
                return Status == CertificateStatus.Claimable |
                       (Status == CertificateStatus.PartiallyTrained &&
                        AllTopPrerequisiteSkills.All(x => x.Skill.Level >= x.Level));
            }
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
        /// <param name="skill">The skill to test</param>
        /// <param name="neededLevel">When this skill is an immediate prerequisite, this parameter will held the required level</param>
        /// <returns></returns>
        public bool HasAsImmediatePrerequisite(Skill skill, out Int64 neededLevel)
        {
            return PrerequisiteSkills.Contains(skill, out neededLevel);
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