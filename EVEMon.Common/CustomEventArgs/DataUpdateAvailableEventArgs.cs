using System;
using System.Collections.Generic;
using EVEMon.Common.Serialization.BattleClinic;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class DataUpdateAvailableEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataUpdateAvailableEventArgs"/> class.
        /// </summary>
        /// <param name="updateUrl">The update URL.</param>
        /// <param name="changedFiles">The changed files.</param>
        public DataUpdateAvailableEventArgs(string updateUrl, List<SerializableDatafile> changedFiles)
        {
            UpdateUrl = updateUrl;
            ChangedFiles = changedFiles;
        }

        /// <summary>
        /// Gets or sets the update URL.
        /// </summary>
        /// <value>The update URL.</value>
        public string UpdateUrl { get; private set; }

        /// <summary>
        /// Gets or sets the changed files.
        /// </summary>
        /// <value>The changed files.</value>
        public List<SerializableDatafile> ChangedFiles { get; private set; }
    }
}