using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class IndustryJob
    {
        /// <summary>
        /// The maximum number of days after job ended. Beyond this limit, we do not import jobs anymore.
        /// </summary>
        public const int MaxEndedDays = 7;
        private List<IndustryJob> m_jobsCompleted = new List<IndustryJob>();

        private bool m_ignored;
        private bool m_markedForDeletion;
        private bool m_notificationSend;

        private JobState m_state;
        private ActiveJobState m_activeJobState;
        private DateTime m_lastStateChange;
        private DateTime m_endProductionTime;
        private DateTime m_pauseProductionTime;

        private readonly int m_jobID;
        private readonly int m_installedItemID;
        private readonly int m_outputItemID;
        private readonly int m_installedItemME;
        private readonly int m_installedItemPE;
        private readonly int m_runs;
        private readonly string m_installation;
        private readonly BlueprintType m_blueprintType;
        private readonly BlueprintActivity m_activity;
        private readonly Blueprint m_installedItem;
        private readonly Item m_outputItem;
        private readonly SolarSystem m_solarSystem;
        private readonly IssuedFor m_issuedFor;
        private readonly DateTime m_installedTime;
        private readonly DateTime m_beginProductionTime;

        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal IndustryJob(SerializableAPIJob src)
        {
            m_state = GetState(src);
            m_jobID = src.JobID;
            m_installedItemID = src.InstalledItemTypeID;
            m_installedItem = StaticBlueprints.GetBlueprintByID(src.InstalledItemTypeID);
            m_outputItemID = src.OutputTypeID;
            m_outputItem = GetOutputItem(src.OutputTypeID);
            m_runs = src.Runs;
            m_activity = (BlueprintActivity)Enum.ToObject(typeof(BlueprintActivity), src.ActivityID);
            m_blueprintType = (BlueprintType)Enum.ToObject(typeof(BlueprintType), src.InstalledItemCopy);
            m_installation = GetInstallation(src.OutputLocationID);
            m_solarSystem = StaticGeography.GetSystem(src.SolarSystemID);
            m_installedTime = src.InstallTime;
            m_installedItemME = src.InstalledItemMaterialLevel;
            m_installedItemPE = src.InstalledItemProductivityLevel;
            m_beginProductionTime = src.BeginProductionTime;
            m_endProductionTime = src.EndProductionTime;
            m_pauseProductionTime = src.PauseProductionTime;
            m_lastStateChange = DateTime.UtcNow;
            m_issuedFor = src.IssuedFor;
            m_activeJobState = GetActiveJobState();
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        internal IndustryJob(SerializableJob src)
        {
            m_ignored = src.Ignored;
            m_jobID = src.JobID;
            m_state = src.State;
            m_installedItemID = src.InstalledItemID;
            m_installedItem = StaticBlueprints.GetBlueprintByID(src.InstalledItemID);
            m_outputItemID = src.OutputItemID;
            m_outputItem = GetOutputItem(src.OutputItemID);
            m_runs = src.Runs;
            m_activity = src.Activity;
            m_blueprintType = src.BlueprintType;
            m_installation = src.ItemLocation;
            m_solarSystem = StaticGeography.GetSystem(src.SolarSystemID);
            m_installedTime = src.InstalledTime;
            m_installedItemME = src.InstalledItemME;
            m_installedItemPE = src.InstalledItemPE;
            m_beginProductionTime = src.BeginProductionTime;
            m_endProductionTime = src.EndProductionTime;
            m_pauseProductionTime = src.PauseProductionTime;
            m_lastStateChange = src.LastStateChange;
            m_issuedFor = src.IssuedFor;
            m_activeJobState = GetActiveJobState();
        }

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        internal SerializableJob Export()
        {
            var serial = new SerializableJob();

            serial.Ignored = m_ignored;
            serial.JobID = m_jobID;
            serial.State = m_state;
            serial.InstalledItemID = m_installedItemID;
            serial.InstalledItem = m_installedItem.Name;
            serial.OutputItemID = m_outputItemID;
            serial.OutputItem = m_outputItem.Name;
            serial.Runs = m_runs;
            serial.Activity = m_activity;
            serial.BlueprintType = m_blueprintType;
            serial.ItemLocation = m_installation;
            serial.SolarSystemID = m_solarSystem.ID;
            serial.InstalledTime = m_installedTime;
            serial.InstalledItemME = m_installedItemME;
            serial.InstalledItemPE = m_installedItemPE;
            serial.BeginProductionTime = m_beginProductionTime;
            serial.EndProductionTime = m_endProductionTime;
            serial.PauseProductionTime = m_pauseProductionTime;
            serial.LastStateChange = m_lastStateChange;
            serial.IssuedFor = m_issuedFor;

            return serial;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Try to update this job with a serialization object from the API.
        /// </summary>
        /// <param name="src">The serializable source.</param>
        /// <returns>True if import sucessful otherwise, false.</returns>
        internal bool TryImport(SerializableAPIJob src)
        {
            // Note that, before a match is found, all jobs have been marked for deletion : m_markedForDeletion == true

            // Checks whether ID is the same
            if (!this.MatchesWith(src))
                return false;

            // Prevent deletion
            m_markedForDeletion = false;

            // Update infos (if ID is the same it may have been modified)
            if (this.IsModified(src))
            {
                m_endProductionTime = src.EndProductionTime;
                m_pauseProductionTime = src.PauseProductionTime;
                
                m_state = (m_pauseProductionTime == DateTime.MinValue ? JobState.Active : JobState.Paused);
                m_activeJobState = GetActiveJobState();
            }

            // Update state
            JobState state = GetState(src);
            if (m_state != JobState.Paused && state != m_state)
                m_state = state;

            return true;
        }

        /// <summary>
        /// Gets the output item by its ID (can be a blueprint or an item).
        /// </summary>
        /// <param name="itemID">The itemID of the blueprint.</param>
        /// <returns>The output item from the bluperint.</returns>
        private static Item GetOutputItem(int id)
        {
            Item item = null;

            // Is it a blueprint ?
            item = StaticBlueprints.GetBlueprintByID(id) as Blueprint;

            // Then it's an item
            if (item == null)
                item = StaticItems.GetItemByID(id);

            return item;
        }

        /// <summary>
        /// Gets the station.
        /// </summary>
        /// <param name="id">The ID of the installation.</param>
        /// <returns>Name of the installation.</returns>
        internal string GetInstallation(int id)
        {
            // Look for the station in datafile
            Station station = StaticGeography.GetStation(id);

            // Is it a conquerable outpost station ?
            if (station == null)
                station = ConquerableStation.GetStation(id);

            // Still nothing ? Then it's a starbase structure
            // and will be assigned manually based on activity
            if (station == null)
                return (m_activity == BlueprintActivity.Manufacturing ? "POS - Assembly Array" : "POS - Laboratory");

            return station.Name;
        }

        /// <summary>
        /// Gets the state of a job.
        /// </summary>
        /// <param name="src">The serializable source.</param>
        /// <returns>State of the seriallzable job.</returns>
        private JobState GetState(SerializableAPIJob src)
        {
            if (src.Completed == (int)JobState.Delivered)
            {
                switch ((CCPJobCompletedStatus)src.CompletedStatus)
                {
                    // Canceled States
                    case CCPJobCompletedStatus.Aborted:
                    case CCPJobCompletedStatus.GM_Aborted:
                        return JobState.Canceled;
                    
                    // Failed States
                    case CCPJobCompletedStatus.Inflight_Unanchored:
                    case CCPJobCompletedStatus.Destroyed:
                    case CCPJobCompletedStatus.Failed:
                        return JobState.Failed;

                    // Delivered States
                    case CCPJobCompletedStatus.Delivered:
                        return JobState.Delivered;

                    default:
                        throw new NotImplementedException();
                }
            }

            return JobState.Active;
        }

        /// <summary>
        /// Gets the state of an active job.
        /// </summary>
        /// <returns>State of an active job.</returns>
        private ActiveJobState GetActiveJobState()
        {
            if (m_state == JobState.Active)
            {
                if (m_beginProductionTime > DateTime.UtcNow)
                    return ActiveJobState.Pending;

                if (m_endProductionTime > DateTime.UtcNow)
                    return ActiveJobState.InProgress;

                return ActiveJobState.Ready;
            }

            return ActiveJobState.None;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets true if we have notified the user.
        /// </summary>
        public bool NotificationSend
        {
            get { return m_notificationSend; }
            set { m_notificationSend = value; }
        }

        /// <summary>
        /// When true, the job will be deleted unless it was found on the API feed.
        /// </summary>
        internal bool MarkedForDeletion
        {
            get { return m_markedForDeletion; }
            set { m_markedForDeletion = value; }
        }

        /// <summary>
        /// Gets or sets whether an expired job has been deleted by the user.
        /// </summary>
        public bool Ignored
        {
            get { return m_ignored; }
            set { m_ignored = value; }
        }

        /// <summary>
        /// Gets or sets the jobs state.
        /// </summary>
        public JobState State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        /// <summary>
        /// Gets or sets the active jobs state.
        /// </summary>
        public ActiveJobState ActiveJobState
        {
            get { return m_activeJobState; }
            set { m_activeJobState = value; }
        }

        /// <summary>
        /// Gets the last state change.
        /// </summary>
        public DateTime LastStateChange
        {
            get { return m_lastStateChange; }
        }

        /// <summary>
        /// Gets the estimated time to completion.
        /// </summary>
        public string TTC
        {
            get
            {
                if (m_state == JobState.Paused)
                    return m_endProductionTime.Subtract(m_pauseProductionTime).ToDescriptiveText(DescriptiveTextOptions.SpaceBetween);

                if (m_state == JobState.Active && m_endProductionTime > DateTime.UtcNow)
                    return m_endProductionTime.ToLocalTime().ToRemainingTimeShortDescription();
                
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the job ID.
        /// </summary>
        public int ID
        {
            get { return m_jobID; }
        }

        /// <summary>
        /// Gets the installed item (can only be a blueprint).
        /// </summary>
        public Blueprint InstalledItem
        {
            get { return m_installedItem; }
        }

        /// <summary>
        /// Gets the output item (can be a blueprint of item).
        /// </summary>
        public Item OutputItem
        {
            get { return m_outputItem; }
        }

        /// <summary>
        /// Gets the job runs.
        /// </summary>
        public int Runs
        {
            get { return m_runs; }
        }

        /// <summary>
        /// Gets the job activity.
        /// </summary>
        public BlueprintActivity Activity
        {
            get { return m_activity; }
        }

        /// <summary>
        /// Get the blueprint's type.
        /// </summary>
        public BlueprintType BlueprintType
        {
            get { return m_blueprintType; }
        }

        /// <summary>
        /// Gets the job installed material efficiency level.
        /// </summary>
        public int InstalledME
        {
            get { return m_installedItemME; }
        }

        /// <summary>
        /// Gets the job installed productivity level.
        /// </summary>
        public int InstalledPE
        {
            get { return m_installedItemPE; }
        }

        /// <summary>
        /// Gets the time the job was installed.
        /// </summary>
        public DateTime InstalledTime
        {
            get { return m_installedTime; }
        }

        /// <summary>
        /// Gets the time the job begins.
        /// </summary>
        public DateTime BeginProductionTime
        {
            get { return m_beginProductionTime; }
        }

        /// <summary>
        /// Gets the time the job ends.
        /// </summary>
        public DateTime EndProductionTime
        {
            get { return m_endProductionTime; }
        }

        /// <summary>
        /// Gets the time the job was paused.
        /// </summary>
        public DateTime PauseProductionTime
        {
            get { return m_pauseProductionTime; }
        }

        /// <summary>
        /// Gets where this job is installed.
        /// </summary>
        public string Installation
        {
            get { return m_installation; }
        }

        /// <summary>
        /// Gets the solar system where this job is located.
        /// </summary>
        public SolarSystem SolarSystem
        {
            get { return m_solarSystem; }
        }

        /// <summary>
        /// Gets the job installation full celestrial path.
        /// </summary>
        public string FullLocation
        {
            get { return String.Format("{0} > {1}", m_solarSystem.FullLocation, m_installation); }
        }

        /// <summary>
        /// Gets for which the job was issued.
        /// </summary>
        public IssuedFor IssuedFor
        {
            get { return m_issuedFor; }
        }

        /// <summary>
        /// Gets true if the job is active.
        /// </summary>
        public bool IsActive
        {
            get { return m_state == JobState.Active; }
        }

        /// <summary>
        /// Checks whether the given API object matches with this job.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        internal bool MatchesWith(SerializableAPIJob src)
        {
            return src.JobID == m_jobID;
        }

        /// <summary>
        /// Checks whether the given API object matches with this job.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        internal bool IsModified(SerializableAPIJob src)
        {
            return src.EndProductionTime != m_endProductionTime
                || src.PauseProductionTime != m_pauseProductionTime;
        }

        #endregion

    }
}
