using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

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

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            IQueryMonitor charIndustryJobsMonitor = m_character.QueryMonitors[APICharacterMethods.IndustryJobs];
            IQueryMonitor corpIndustryJobsMonitor = m_character.QueryMonitors[APICorporationMethods.CorporationIndustryJobs];
            if ((charIndustryJobsMonitor == null || !charIndustryJobsMonitor.Enabled) &&
                (corpIndustryJobsMonitor == null || !corpIndustryJobsMonitor.Enabled))
                return;

            UpdateOnTimerTick();
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
                Items.Add(new IndustryJob(srcJob) { InstallerID = m_character.CharacterID });
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
            List<IndustryJob> newJobs = src.Select(
                srcJob => new
                              {
                                  srcJob,
                                  limit = srcJob.EndProductionTime.AddDays(IndustryJob.MaxEndedDays)
                              }).Where(
                                  job => job.limit >= DateTime.UtcNow).Where(
                                      job => !Items.Any(x => x.TryImport(job.srcJob))).Select(
                                          job => new IndustryJob(job.srcJob)).Where(
                                              job => job.InstalledItem != null).ToList();

            // Add the items that are no longer marked for deletion
            newJobs.AddRange(Items.Where(x => !x.MarkedForDeletion));

            // Replace the old list with the new one
            Items.Clear();
            Items.AddRange(newJobs);
        }

        /// <summary>
        /// Exports only the character issued jobs to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used to export only the corporation jobs issued by the character.</remarks>
        internal IEnumerable<SerializableJob> ExportOnlyIssuedByCharacter()
        {
            return Items.Where(job => job.InstallerID == m_character.CharacterID).Select(job => job.Export());
        }

        /// <summary>
        /// Exports the jobs to a serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable jobs.</returns>
        /// <remarks>Used to export all jobs of the collection.</remarks>
        internal IEnumerable<SerializableJob> Export()
        {
            return Items.Select(job => job.Export());
        }

        /// <summary>
        /// Notify the user on a job completion.
        /// </summary>
        private void UpdateOnTimerTick()
        {
            // We exit if there are no jobs
            if (Items.IsEmpty())
                return;

            // Add the not notified "Ready" jobs to the completed list
            List<IndustryJob> jobsCompleted = Items.Where(
                job => job.ActiveJobState == ActiveJobState.Ready && !job.NotificationSend).ToList();

            jobsCompleted.ForEach(job => job.NotificationSend = true);

            // We exit if no jobs have been completed
            if (jobsCompleted.IsEmpty())
                return;

            // Sends a notification
            if (Items.All(job => job.IssuedFor == IssuedFor.Corporation))
            {
                // Fires the event regarding the corporation industry jobs completion, issued by the character
                IEnumerable<IndustryJob> characterJobs = jobsCompleted.Where(job => job.InstallerID == m_character.CharacterID);
                if (characterJobs.Count() > 0)
                    EveMonClient.OnCharacterIndustryJobsCompleted(m_character, characterJobs);

                // Fires the event regarding the corporation industry jobs completion
                EveMonClient.OnCorporationIndustryJobsCompleted(m_character, jobsCompleted);
            }
            else
                // Fires the event regarding the character's industry jobs completion
                EveMonClient.OnCharacterIndustryJobsCompleted(m_character, jobsCompleted);
        }
    }
}