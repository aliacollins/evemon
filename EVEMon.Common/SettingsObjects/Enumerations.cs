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


}
