using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVEMon.Common.Enumerations
{
    /// <summary>
    /// Represents a certificate's status from a character's point of view.
    /// </summary>
    public enum CertificateStatus
    {
        /// <summary>
        /// The certificate has been granted to this character
        /// </summary>
        Granted,

        /// <summary>
        /// The certificate can be claimed by the char, all prerequisites are met.
        /// </summary>
        Claimable, 

        /// <summary>
        /// The certificate is not claimable yet but at least one prerequisite is satisfied
        /// </summary>
        PartiallyTrained,

        /// <summary>
        /// The certificate is not claimable and none of its prerequisites are satisfied
        /// </summary>
        Untrained
    }
}
