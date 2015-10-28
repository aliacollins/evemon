using System;
using System.Linq;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a mastery certificate.
    /// </summary>
    public sealed class MasteryCertificate
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="masteryLevel">The mastery level.</param>
        /// <param name="src">The source.</param>
        internal MasteryCertificate(Mastery masteryLevel, SerializableMasteryCertificate src)
        {
            MasteryLevel = masteryLevel;
            Certificate = StaticCertificates.GetCertificateByID(src.ID);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the mastery level.
        /// </summary>
        public Mastery MasteryLevel { get; private set; }

        /// <summary>
        /// Gets or sets the certificate.
        /// </summary>
        public StaticCertificate Certificate { get; set; }

        /// <summary>
        /// Gets this certificate's representation for the provided character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">character</exception>
        public Certificate ToCharacter(Character character)
        {
            if (character == null)
                throw new ArgumentNullException("character");

            return character.Certificates.FirstOrDefault(x => x.ID == Certificate.ID);
        }

        #endregion

    }
}