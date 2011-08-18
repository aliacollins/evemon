using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
	/// <summary>
	/// Represents a certificate from the datafiles.
	/// Every category (i.e. "Business and Industry")
	/// contains certificate classes (i.e. "Production Manager"),
	/// which contain certificates (i.e. "Production Manager Basic").
	/// </summary>
    public sealed class StaticCertificate
    {
        private readonly List<StaticCertificate> m_prerequisiteCertificates = new List<StaticCertificate>();
        private readonly List<StaticSkillLevel> m_prerequisiteSkills = new List<StaticSkillLevel>();


        #region Constructor

        /// <summary>
        /// Constructor from XML.
        /// </summary>
        /// <param name="certClass"></param>
        /// <param name="src"></param>
        internal StaticCertificate(StaticCertificateClass certClass, SerializableCertificate src)
        {
            ID = src.ID;
            Class = certClass;
            Name = Class.Name;
            Grade = src.Grade;
            Description = src.Description;

            // Recommendations
            Recommendations = new StaticRecommendations<Item>();
            if (src.Recommendations != null)
            {
                foreach (SerializableCertificateRecommendation recommendation in src.Recommendations)
                {
                    Ship ship = StaticItems.ShipsMarketGroup.AllItems.FirstOrDefault(x => x.Name == recommendation.Ship) as Ship;
                    if (ship != null)
                    {
                        ship.Recommendations.Add(this);
                        Recommendations.Add(ship);
                    }
                }
            }
        }
        
        #endregion


        # region Public Properties

        /// <summary>
        /// Gets this certificate's ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets this certificate's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the certificate's class.
        /// </summary>
        public StaticCertificateClass Class { get; private set; }

        /// <summary>
        /// Gets this certificate's description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets this certificate's grade.
        /// </summary>
        public CertificateGrade Grade { get; private set; }

        /// <summary>
        /// Gets the ships this certificate is recommended for.
        /// </summary>
        public StaticRecommendations<Item> Recommendations { get; private set; }

        /// <summary>
        /// Gets the prerequisite skills.
        /// </summary>
        public IEnumerable<StaticSkillLevel> PrerequisiteSkills
        {
            get { return m_prerequisiteSkills; }
        }

        /// <summary>
        /// Gets the prerequisite certificates.
        /// </summary>
        public IEnumerable<StaticCertificate> PrerequisiteCertificates
        {
            get { return m_prerequisiteCertificates; }
        }

        /// <summary>
        /// Gets all the top-level prerequisite skills, including the ones from prerequisite certificates.
        /// </summary>
        public IEnumerable<StaticSkillLevel> AllTopPrerequisiteSkills
        {
            get
            {
                int[] highestLevels = new int[StaticSkills.ArrayIndicesCount];
                List<StaticSkillLevel> list = new List<StaticSkillLevel>();

                // Collect all top prerequisites from certificates
                foreach (StaticCertificate certPrereq in m_prerequisiteCertificates)
                {
                    foreach (StaticSkillLevel certSkilPrereq in certPrereq.AllTopPrerequisiteSkills)
                    {
                        if (highestLevels[certSkilPrereq.Skill.ArrayIndex] < certSkilPrereq.Level)
                        {
                            highestLevels[certSkilPrereq.Skill.ArrayIndex] = certSkilPrereq.Level;
                            list.Add(certSkilPrereq);
                        }
                    }
                }

                // Collect all prerequisites from skills
                foreach (StaticSkillLevel skillPrereq in m_prerequisiteSkills)
                {
                    if (highestLevels[skillPrereq.Skill.ArrayIndex] < skillPrereq.Level)
                    {
                        highestLevels[skillPrereq.Skill.ArrayIndex] = skillPrereq.Level;
                        list.Add(skillPrereq);
                    }
                }

                // Return the result
                foreach (StaticSkillLevel newItem in list)
                {
                    if (highestLevels[newItem.Skill.ArrayIndex] != 0)
                    {
                        yield return new StaticSkillLevel(newItem.Skill, highestLevels[newItem.Skill.ArrayIndex]);
                        highestLevels[newItem.Skill.ArrayIndex] = 0;
                    }
                }
            }
        }

        # endregion


        # region Helper Methods

        /// <summary>
        /// Completes the initialization by updating the prerequisites.
        /// </summary>
        internal void CompleteInitialization(IEnumerable<SerializableCertificatePrerequisite> prereqs)
        {
            foreach (SerializableCertificatePrerequisite prereq in prereqs)
            {
                // Skills
                if (prereq.Kind == SerializableCertificatePrerequisiteKind.Skill)
                {
                    m_prerequisiteSkills.Add(new StaticSkillLevel(prereq.Name, Int32.Parse(prereq.Level)));
                }
                // Certificates
                else
                {
                    CertificateGrade grade = GetGrade(prereq.Level);
                    m_prerequisiteCertificates.Add(StaticCertificates.GetCertificateClass(prereq.Name)[grade]);
                }
            }
        }

	    /// <summary>
	    /// Gets the grade from the provided grade key. No need to previously interns the key, it will be interned in this method.
	    /// </summary>
	    /// <param name="key"></param>
	    /// <exception cref="NotImplementedException"></exception>
	    /// <returns></returns>
	    private static CertificateGrade GetGrade(string key)
        {
            switch (key)
            {
                case "Basic":
                    return CertificateGrade.Basic;
                case "Standard":
                    return CertificateGrade.Standard;
                case "Improved":
                    return CertificateGrade.Improved;
                case "Elite":
                    return CertificateGrade.Elite;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this certificate.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} {1}", Name, Grade);
        }

        #endregion


        /// <summary>
        /// Gets the equivalent representation of this certificate for the given character.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Certificate ToCharacter(Character character)
        {
            return character.Certificates[ID];
        }
    }
}

