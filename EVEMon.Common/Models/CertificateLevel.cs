using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Wraps the informations of a specific level of a certificate
    /// </summary>
    public sealed class CertificateLevel
    {
        private List<SkillLevel> m_skills;

        private bool m_initialized;

        private readonly Character m_character;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateLevel"/> class.
        /// </summary>
        /// <param name="grade">The grade.</param>
        /// <param name="cert">The cert.</param>
        /// <param name="characater">The characater.</param>
        public CertificateLevel(CertificateGrade grade, Certificate cert, Character characater)
        {
            Grade = grade;
            Certificate = cert;
            m_character = characater;
            m_initialized = false;
            Status = CertificateStatus.Untrained;            
        }

        /// <summary>
        /// Gets the grade.
        /// </summary>
        /// <value>
        /// The grade.
        /// </value>
        public CertificateGrade Grade { get; private set; }

        /// <summary>
        /// Gets the certificate.
        /// </summary>
        /// <value>
        /// The certificate.
        /// </value>
        public Certificate Certificate { get; private set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public CertificateStatus Status { get; private set; }

        /// <summary>                                                                                                             
        /// Gets the immediate prerequisite skills.                                                                                                            
        /// </summary>
        public IEnumerable<SkillLevel> PrerequisiteSkills
        {
            get { return m_skills; }
        }

        /// <summary>
        /// Completes the initialization.
        /// </summary>
        /// <param name="skills">The skills.</param>
        public void CompleteInitialization(IEnumerable<SkillLevel> skills)
        {
            m_skills = skills.ToList();
        }

        /// <summary>
        /// Gets true whether the certificate is granted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is granted; otherwise, <c>false</c>.
        /// </value>
        public bool IsTrained
        {
            get { return Status == CertificateStatus.Trained; }
        }

        /// <summary>
        /// Marks the certificate as trained.
        /// </summary>
        internal void MarkAsTrained()
        {
            Status = CertificateStatus.Trained;
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
            bool trained = true;

            // Scan prerequisite skills
            foreach (SkillLevel prereqSkill in m_skills)
            {
                // Trained only if the skill's level is greater or equal than the minimum level
                trained &= (prereqSkill.Skill.Level >= prereqSkill.Level);

                // Untrainable if no prereq is satisfied
                noPrereq &= prereqSkill.AllDependencies.All(x => !x.IsTrained);
            }

            // Updates status
            if (trained)
                Status = CertificateStatus.Trained;
            else if (noPrereq)
                Status = CertificateStatus.Untrained;
            else
                Status = CertificateStatus.PartiallyTrained;
            m_initialized = true;
            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureConstants.DefaultCulture, "Level {0}", Skill.GetRomanFromInt(((int)Grade)));
        }
    }
}
