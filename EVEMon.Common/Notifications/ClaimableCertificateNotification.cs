using System;
using System.Collections.Generic;

namespace EVEMon.Common.Notifications
{
    public sealed class ClaimableCertificateNotification : Notification
    { 
        private readonly List<Certificate> m_claimableCertificates;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="skills"></param>
        public ClaimableCertificateNotification(Object sender, IEnumerable<Certificate> certificates)
            : base(NotificationCategory.ClaimableCertificate, sender)
        {
            m_claimableCertificates = new List<Certificate>();
            foreach (Certificate cert in certificates)
            {
                m_claimableCertificates.Add(cert);
            }

            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public IEnumerable<Certificate> Certificates
        {
            get
            {
                foreach (Certificate cert in m_claimableCertificates)
                {
                    yield return cert;
                }
            }
        }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails
        {
            get
            {
                if (m_claimableCertificates.Count == 1)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            if (m_claimableCertificates.Count == 1)
            {
                m_description = String.Format(CultureConstants.DefaultCulture, "{0} {1} is claimable.",
                    m_claimableCertificates[0].Name, m_claimableCertificates[0].Grade) ;
            }
            else
            {
                m_description = String.Format(CultureConstants.DefaultCulture,
                    "{0} certificates are claimable.", m_claimableCertificates.Count);
            }
        }
}}
