using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;

namespace EVEMon.Common.Notifications
{
    /// <summary>
    /// Provides notification services for PlanetaryPins.
    /// </summary>
    public sealed class PlanetaryPinsNotificationEventArgs: NotificationEventArgs
    {
        public PlanetaryPinsNotificationEventArgs(Object sender, IEnumerable<PlanetaryPin> pins)
            : base(sender, NotificationCategory.PlanetaryPinsCompleted)
        {
            if (pins == null)
                throw new ArgumentNullException("pins");

            PlanetaryPins = new Collection<PlanetaryPin>();
            foreach (PlanetaryPin pin in pins)
            {
                PlanetaryPins.Add(pin);
            }
            UpdateDescription();
        }
   
        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public Collection<PlanetaryPin> PlanetaryPins { get; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails => true;

        /// <summary>
        /// Enqueue the jobs from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            foreach (PlanetaryPin pin in ((PlanetaryPinsNotificationEventArgs)other).PlanetaryPins)
            {
                PlanetaryPins.Add(pin);
            }

            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = String.Format(CultureConstants.DefaultCulture, "{0} planetary work{1} completed.", PlanetaryPins.Count,
                                        PlanetaryPins.Count > 1 ? "s" : String.Empty);
        }
    }
 }
