using System;
using System.Collections.ObjectModel;
using EVEMon.Common.Serialization.BattleClinic;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class DataUpdateAvailableEventArgs : EventArgs
    {
        private readonly Collection<SerializableDatafile> m_changedFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataUpdateAvailableEventArgs"/> class.
        /// </summary>
        /// <param name="changedFiles">The changed files.</param>
        public DataUpdateAvailableEventArgs(Collection<SerializableDatafile> changedFiles)
        {
            m_changedFiles = changedFiles;
        }

        /// <summary>
        /// Gets or sets the changed files.
        /// </summary>
        /// <value>The changed files.</value>
        public Collection<SerializableDatafile> ChangedFiles { get { return m_changedFiles; } }
    }
}