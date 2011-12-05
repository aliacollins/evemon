using System;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class IndustryJob
    {
        /// <summary>
        /// The maximum number of days after job ended. Beyond this limit, we do not import jobs anymore.
        /// </summary>
        public const int MaxEndedDays = 7;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal IndustryJob(SerializableJobListItem src)
        {
            State = GetState(src);
            ID = src.JobID;
            InstallerID = src.InstallerID;
            InstalledItemID = src.InstalledItemTypeID;
            InstalledItem = StaticBlueprints.GetBlueprintByID(src.InstalledItemTypeID);
            OutputItemID = src.OutputTypeID;
            OutputItem = GetOutputItem(src.OutputTypeID);
            Runs = src.Runs;
            Activity = (BlueprintActivity)Enum.ToObject(typeof(BlueprintActivity), src.ActivityID);
            BlueprintType = (BlueprintType)Enum.ToObject(typeof(BlueprintType), src.InstalledItemCopy);
            Installation = GetInstallation(src.OutputLocationID);
            SolarSystem = StaticGeography.GetSolarSystemByID(src.SolarSystemID);
            InstalledTime = src.InstallTime;
            InstalledME = src.InstalledItemMaterialLevel;
            InstalledPE = src.InstalledItemProductivityLevel;
            BeginProductionTime = src.BeginProductionTime;
            EndProductionTime = src.EndProductionTime;
            PauseProductionTime = src.PauseProductionTime;
            LastStateChange = DateTime.UtcNow;
            IssuedFor = src.IssuedFor;
            ActiveJobState = GetActiveJobState();
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        internal IndustryJob(SerializableJob src)
        {
            Ignored = src.Ignored;
            ID = src.JobID;
            State = src.State;
            InstalledItemID = src.InstalledItemID;
            InstalledItem = StaticBlueprints.GetBlueprintByID(src.InstalledItemID);
            OutputItemID = src.OutputItemID;
            OutputItem = GetOutputItem(src.OutputItemID);
            Runs = src.Runs;
            Activity = src.Activity;
            BlueprintType = src.BlueprintType;
            Installation = src.ItemLocation;
            SolarSystem = StaticGeography.GetSolarSystemByID(src.SolarSystemID);
            InstalledTime = src.InstalledTime;
            InstalledME = src.InstalledItemME;
            InstalledPE = src.InstalledItemPE;
            BeginProductionTime = src.BeginProductionTime;
            EndProductionTime = src.EndProductionTime;
            PauseProductionTime = src.PauseProductionTime;
            LastStateChange = src.LastStateChange;
            IssuedFor = (src.IssuedFor == IssuedFor.None ? IssuedFor.Character : src.IssuedFor);
            ActiveJobState = GetActiveJobState();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets true if we have notified the user.
        /// </summary>
        public bool NotificationSend { get; set; }

        /// <summary>
        /// When true, the job will be deleted unless it was found on the API feed.
        /// </summary>
        internal bool MarkedForDeletion { get; set; }

        /// <summary>
        /// Gets or sets whether an expired job has been deleted by the user.
        /// </summary>
        public bool Ignored { get; set; }

        /// <summary>
        /// Gets or sets the jobs state.
        /// </summary>
        public JobState State { get; private set; }

        /// <summary>
        /// Gets or sets the active jobs state.
        /// </summary>
        public ActiveJobState ActiveJobState { get; set; }

        /// <summary>
        /// Gets the last state change.
        /// </summary>
        public DateTime LastStateChange { get; private set; }

        /// <summary>
        /// Gets the estimated time to completion.
        /// </summary>
        public string TTC
        {
            get
            {
                if (State == JobState.Paused)
                {
                    return EndProductionTime.Subtract(PauseProductionTime).ToDescriptiveText(
                        DescriptiveTextOptions.SpaceBetween);
                }

                if (State == JobState.Active && EndProductionTime > DateTime.UtcNow)
                    return EndProductionTime.ToRemainingTimeShortDescription(DateTimeKind.Utc);

                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the job ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets or sets the installer ID.
        /// </summary>
        /// <value>The installer ID.</value>
        public long InstallerID { get; set; }

        /// <summary>
        /// Gets the installed item ID.
        /// </summary>
        public int InstalledItemID { get; private set; }

        /// <summary>
        /// Gets the installed item (can only be a blueprint).
        /// </summary>
        public Blueprint InstalledItem { get; private set; }

        /// <summary>
        /// Gets the putput item ID.
        /// </summary>
        public int OutputItemID { get; private set; }

        /// <summary>
        /// Gets the output item (can be a blueprint or item).
        /// </summary>
        public Item OutputItem { get; private set; }

        /// <summary>
        /// Gets the job runs.
        /// </summary>
        public int Runs { get; private set; }

        /// <summary>
        /// Gets the job activity.
        /// </summary>
        public BlueprintActivity Activity { get; private set; }

        /// <summary>
        /// Get the blueprint's type.
        /// </summary>
        public BlueprintType BlueprintType { get; private set; }

        /// <summary>
        /// Gets the job installed material efficiency level.
        /// </summary>
        public int InstalledME { get; private set; }

        /// <summary>
        /// Gets the job installed productivity level.
        /// </summary>
        public int InstalledPE { get; private set; }

        /// <summary>
        /// Gets the time the job was installed.
        /// </summary>
        public DateTime InstalledTime { get; private set; }

        /// <summary>
        /// Gets the time the job begins.
        /// </summary>
        public DateTime BeginProductionTime { get; private set; }

        /// <summary>
        /// Gets the time the job ends.
        /// </summary>
        public DateTime EndProductionTime { get; private set; }

        /// <summary>
        /// Gets the time the job was paused.
        /// </summary>
        public DateTime PauseProductionTime { get; private set; }

        /// <summary>
        /// Gets where this job is installed.
        /// </summary>
        public string Installation { get; private set; }

        /// <summary>
        /// Gets the solar system where this job is located.
        /// </summary>
        public SolarSystem SolarSystem { get; private set; }

        /// <summary>
        /// Gets the job installation full celestrial path.
        /// </summary>
        public string FullLocation
        {
            get { return String.Format(CultureConstants.DefaultCulture, "{0} > {1}", SolarSystem.FullLocation, Installation); }
        }

        /// <summary>
        /// Gets for which the job was issued.
        /// </summary>
        public IssuedFor IssuedFor { get; private set; }

        /// <summary>
        /// Gets true if the job is active.
        /// </summary>
        public bool IsActive
        {
            get { return State == JobState.Active; }
        }

        /// <summary>
        /// Checks whether the given API object matches with this job.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private bool MatchesWith(SerializableJobListItem src)
        {
            return src.JobID == ID;
        }

        /// <summary>
        /// Checks whether the given API object matches with this job.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private bool IsModified(SerializableJobListItem src)
        {
            return src.EndProductionTime != EndProductionTime
                   || src.PauseProductionTime != PauseProductionTime;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        internal SerializableJob Export()
        {
            return new SerializableJob
            {
                Ignored = Ignored,
                JobID = ID,
                State = State,
                InstalledItemID = InstalledItemID,
                InstalledItem = InstalledItem.Name,
                OutputItemID = OutputItemID,
                OutputItem = OutputItem.Name,
                Runs = Runs,
                Activity = Activity,
                BlueprintType = BlueprintType,
                ItemLocation = Installation,
                SolarSystemID = SolarSystem.ID,
                InstalledTime = InstalledTime,
                InstalledItemME = InstalledME,
                InstalledItemPE = InstalledPE,
                BeginProductionTime = BeginProductionTime,
                EndProductionTime = EndProductionTime,
                PauseProductionTime = PauseProductionTime,
                IssuedFor = IssuedFor,
                LastStateChange = LastStateChange,
            };
        }

        /// <summary>
        /// Try to update this job with a serialization object from the API.
        /// </summary>
        /// <param name="src">The serializable source.</param>
        /// <returns>True if import sucessful otherwise, false.</returns>
        internal bool TryImport(SerializableJobListItem src)
        {
            // Note that, before a match is found, all jobs have been marked for deletion : m_markedForDeletion == true

            // Checks whether ID is the same
            if (!MatchesWith(src))
                return false;

            // Prevent deletion
            MarkedForDeletion = false;

            // Update infos (if ID is the same it may have been modified)
            if (IsModified(src))
            {
                EndProductionTime = src.EndProductionTime;
                PauseProductionTime = src.PauseProductionTime;

                State = (PauseProductionTime == DateTime.MinValue ? JobState.Active : JobState.Paused);
                ActiveJobState = GetActiveJobState();
                LastStateChange = DateTime.UtcNow;
            }

            // Update state
            JobState state = GetState(src);
            if (State != JobState.Paused && state != State)
            {
                State = state;
                LastStateChange = DateTime.UtcNow;
            }

            return true;
        }

        /// <summary>
        /// Gets the output item by its ID (can be a blueprint or an item).
        /// </summary>
        /// <param name="id">The itemID of the blueprint.</param>
        /// <returns>The output item from the bluperint.</returns>
        private static Item GetOutputItem(int id)
        {
            // Is it a blueprint ? If not then it's an item
            return StaticBlueprints.GetBlueprintByID(id) ?? StaticItems.GetItemByID(id);
        }

        /// <summary>
        /// Gets the station.
        /// </summary>
        /// <param name="id">The ID of the installation.</param>
        /// <returns>Name of the installation.</returns>
        private string GetInstallation(int id)
        {
            // Look for the station in datafile, if not found check if it's a conquerable outpost station
            Station station = StaticGeography.GetStationByID(id) ?? ConquerableStation.GetStationByID(id);

            // Still nothing ? Then it's a starbase structure
            // and will be assigned manually based on activity
            if (station == null)
                return (Activity == BlueprintActivity.Manufacturing ? "POS - Assembly Array" : "POS - Laboratory");

            return station.Name;
        }

        /// <summary>
        /// Gets the state of a job.
        /// </summary>
        /// <param name="src">The serializable source.</param>
        /// <returns>State of the seriallzable job.</returns>
        private static JobState GetState(SerializableJobListItem src)
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
            if (State == JobState.Active)
            {
                if (BeginProductionTime > DateTime.UtcNow)
                    return ActiveJobState.Pending;

                return EndProductionTime > DateTime.UtcNow ? ActiveJobState.InProgress : ActiveJobState.Ready;
            }

            return ActiveJobState.None;
        }

        #endregion
    }
}