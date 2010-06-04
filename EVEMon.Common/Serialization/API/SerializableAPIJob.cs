using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIJobList
    {
        public SerializableAPIJobList()
        {
            this.Jobs = new List<SerializableAPIJob>();
        }

        [XmlArray("jobs")]
        [XmlArrayItem("job")]
        public List<SerializableAPIJob> Jobs
        {
            get;
            set;
        }
    }

    public sealed class SerializableAPIJob
    {
        [XmlAttribute("jobID")]
        public int JobID
        {
            get;
            set;
        }

        [XmlAttribute("installedItemLocationID")]
        public int InstalledItemLocationID
        {
            get;
            set;
        }

        [XmlAttribute("outputLocationID")]
        public int OutputLocationID
        {
            get;
            set;
        }

        [XmlAttribute("installedInSolarSystemID")]
        public int SolarSystemID
        {
            get;
            set;
        }

        [XmlAttribute("installedItemProductivityLevel")]
        public int InstalledItemProductivityLevel
        {
            get;
            set;
        }

        [XmlAttribute("installedItemMaterialLevel")]
        public int InstalledItemMaterialLevel
        {
            get;
            set;
        }

        [XmlAttribute("installerID")]
        public int InstallerID
        {
            get;
            set;
        }

        [XmlAttribute("runs")]
        public int Runs
        {
            get;
            set;
        }

        [XmlAttribute("installedItemTypeID")]
        public int InstalledItemTypeID
        {
            get;
            set;
        }

        [XmlAttribute("outputTypeID")]
        public int OutputTypeID
        {
            get;
            set;
        }

        [XmlAttribute("installedItemCopy")]
        public int InstalledItemCopy
        {
            get;
            set;
        }

        [XmlAttribute("completed")]
        public int Completed
        {
            get;
            set;
        }

        [XmlAttribute("activityID")]
        public int ActivityID
        {
            get;
            set;
        }

        [XmlAttribute("completedStatus")]
        public int CompletedStatus
        {
            get;
            set;
        }

        [XmlAttribute("installTime")]
        public string InstallTimeXml
        {
            get { return InstallTime.ToCCPTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    InstallTime = value.CCPTimeStringToDateTime();
            }
        }

        [XmlAttribute("beginProductionTime")]
        public string BeginProductionTimeXml
        {
            get { return BeginProductionTime.ToCCPTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    BeginProductionTime = value.CCPTimeStringToDateTime();
            }
        }

        [XmlAttribute("endProductionTime")]
        public string EndProductionTimeXml
        {
            get { return EndProductionTime.ToCCPTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    EndProductionTime = value.CCPTimeStringToDateTime();
            }
        }

        [XmlAttribute("pauseProductionTime")]
        public string PauseProductionTimeXml
        {
            get { return PauseProductionTime.ToCCPTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    PauseProductionTime = value.CCPTimeStringToDateTime();
            }
        }

        /// <summary>
        /// The time this job was installed.
        /// </summary>
        [XmlIgnore]
        public DateTime InstallTime
        {
            get;
            set;
        }

        /// <summary>
        /// The time this job began.
        /// </summary>
        [XmlIgnore]
        public DateTime BeginProductionTime
        {
            get;
            set;
        }

        /// <summary>
        /// The time this job will finish.
        /// </summary>
        [XmlIgnore]
        public DateTime EndProductionTime
        {
            get;
            set;
        }

        /// <summary>
        /// The time this job was paused.
        /// </summary>
        [XmlIgnore]
        public DateTime PauseProductionTime
        {
            get;
            set;
        }

        /// <summary>
        /// Which this job was issued for.
        /// </summary>
        [XmlIgnore]
        public IssuedFor IssuedFor
        {
            get;
            set;
        }
    }

    public enum CCPJobCompletedStatus
    {
        Failed = 0,
        Delivered = 1,
        Aborted = 2,
        GM_Aborted = 3,
        Inflight_Unanchored = 4,
        Destroyed = 5
    }
}
