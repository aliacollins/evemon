using System.ComponentModel;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents the available column types.
    /// </summary>
    public enum EveNotificationColumn
    {
        None = -1,

        [Header("Received")]
        [Description("Received Date")]
        SentDate = 0,

        [Header("From")]
        [Description("From ( Sender )")]
        SenderName = 1,

        [Header("Subject")]
        [Description("Subject")]
        Type = 2,
    }
}