using System;
using System.Collections.Generic;

namespace EVEMon.Common.Notifications
{
    public sealed class ClaimableCertificateNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificates">The certificates.</param>
        public ClaimableCertificateNotificationEventArgs(Object sender, IEnumerable<Certificate> certificates)
            : base(sender, NotificationCategory.ClaimableCertificate)
        {
            Certificates = new List<Certificate>();
            foreach (Certificate cert in certificates)
            {
                Certificates.Add(cert);
            }

            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public List<Certificate> Certificates { get; private set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails
        {
            get { return Certificates.Count != 1; }
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = (Certificates.Count == 1
                               ? String.Format(CultureConstants.DefaultCulture, "{0} {1} is claimable.",
                                               Certificates[0].Name, Certificates[0].Grade)
                               : String.Format(CultureConstants.DefaultCulture,
                                               "{0} certificates are claimable.", Certificates.Count));
        }
    }
}