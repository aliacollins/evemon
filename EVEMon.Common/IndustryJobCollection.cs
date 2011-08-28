using System;
using System.Collections.Generic;
using System.Linq;

using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// A collection of industry jobs.
    /// </summary>
    public sealed class IndustryJobCollection : ReadonlyCollection<IndustryJob>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal IndustryJobCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src"></param>
        internal void Import(IEnumerable<SerializableJob> src)
        {
            Items.Clear();
            foreach (SerializableJob srcJob in src)
            {
                Items.Add(new IndustryJob(srcJob));
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable jobs from the API.</param>
        internal void Import(IEnumerable<SerializableJobListItem> src)
        {
            // Mark all jobs for deletion 
            // If they are found again on the API feed, they won't be deleted
            // and those set as ignored will be left as ignored
            foreach (IndustryJob job in Items)
            {
                job.MarkedForDeletion = true;
            }

            // Import the jobs from the API
            List<IndustryJob> newJobs = (src.Select(
                srcJob => new
                              {
                                  srcJob,
                                  limit = srcJob.EndProductionTime.AddDays(IndustryJob.MaxEndedDays)
                              }).Where(
                                  job => job.limit >= DateTime.UtcNow).Where(
                                      job => !Items.Any(x => x.TryImport(job.srcJob))).Select(
                                          job => new IndustryJob(job.srcJob)).Where(
                                              job => job.InstalledItem != null)).ToList();

            // Add the items that are no longer marked for deletion
            newJobs.AddRange(Items.Where(x => !x.MarkedForDeletion));

            // Replace the old list with the new one
            Items.Clear();
            Items.AddRange(newJobs);

            // Fires the event regarding industry jobs update
            EveMonClient.OnCharacterIndustryJobsUpdated(m_character);
        }

        /// <summary>
        /// Exports the orders to a serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable jobs.</returns>
        internal List<SerializableJob> Export()
        {
            List<SerializableJob> serial = new List<SerializableJob>(Items.Count);
            serial.AddRange(Items.Select(job => job.Export()));

            return serial;
        }

        /// <summary>
        /// Notify the user on a job completion.
        /// </summary>
        internal void UpdateOnTimerTick()
        {
            // We exit if there are no jobs
            if (Items.IsEmpty())
                return;

            List<IndustryJob> jobsCompleted = new List<IndustryJob>();

            // Add the not notified "Ready" jobs to the completed list
            foreach (IndustryJob job in Items.Where(x => x.ActiveJobState == ActiveJobState.Ready && !x.NotificationSend))
            {
                jobsCompleted.Add(job);
                job.NotificationSend = true;
            }

            // We exit if no jobs have been completed
            if (jobsCompleted.IsEmpty())
                return;

            // Sends a notification
            EveMonClient.Notifications.NotifyIndustryJobCompletion(m_character, jobsCompleted);

            // Fires the event regarding industry jobs completed
            EveMonClient.OnCharacterIndustryJobsCompleted(m_character, jobsCompleted);
        }
    }
}
