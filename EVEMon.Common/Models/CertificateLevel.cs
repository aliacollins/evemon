using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Wraps the informations of a specific level of a certificate
    /// </summary>
    public sealed class CertificateLevel
    {
        /// <summary>
        /// 
        /// </summary>
        private List<SkillLevel> m_skills;

        private bool m_initialized;
        private Character m_character;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="skills"></param>
        public CertificateLevel(CertificateGrade grade, Certificate cert, Character characater)
        {
            Grade = grade;
            Certificate = cert;
            m_character = characater;
            m_initialized = false;
            Status = CertificateStatus.Untrained;            
        }

        /// <summary>
        /// 
        /// </summary>
        public CertificateGrade Grade { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Certificate Certificate { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CertificateStatus Status { get; private set; }

        /// <summary>                                                                                                             
        /// Gets the immediate prerequisite skills.                                                                                                            
        /// </summary>
        public IEnumerable<SkillLevel> PrerequisiteSkills
        {
            get { return m_skills.ToList(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skills"></param>
        public void CompleteInitialization(IEnumerable<SkillLevel> skills)
        {
            m_skills = skills.ToList();
        }

        /// <summary>
        /// Gets true whether the certificate is granted.
        /// </summary>
        public bool IsGranted
        {
            get { return Status == CertificateStatus.Granted; }
        }

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
        /// Gets the required training time for the provided character to train this certificate.
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTrainingTime
        {
            get { return m_character.GetTrainingTimeToMultipleSkills(m_skills); }
        }

        /// <summary>
        /// Try to update the certificate's status. 
        /// </summary>
        /// <returns>True if the status was updated, false otherwise.</returns>
        internal bool TryUpdateCertificateStatus()
        {
            if (m_initialized)
                return false;
            
            bool noPrereq = true;
            bool claimable = true;

            // Scan prerequisite skills
            foreach (SkillLevel prereqSkill in m_skills)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureConstants.DefaultCulture, "Level {0}", Skill.GetRomanFromInt(((int)Grade) + 1));
        }
    }
}
