using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Describes the target platform to allow EVEMon to apply different tweaks at runtime
    /// </summary>
    public enum CompatibilityMode
    {
        /// <summary>
        /// Windows and Linux + Wine
        /// </summary>
        Default = 0,
        /// <summary>
        /// Wine
        /// </summary>
        Wine = 1
    }

    public enum ObsoleteEntryRemovalBehaviour
    {
        /// <summary>
        /// Never remove entries from the plan, always ask the user.
        /// </summary>
        AlwaysAsk = 0,
        /// <summary>
        /// Only remove confirmed completed (by API) entries from the plan, ask about unconfirmed entries.
        /// </summary>
        RemoveConfirmed = 1,
        /// <summary>
        /// Always remove all entries automatically.
        /// </summary>
        RemoveAll = 2
    }
}
