using System.ComponentModel;

using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    /// Represents the available column types
    /// </summary>
    public enum EveNotificationsColumn
    {
        None = -1,

        [Header("Send at")]
        [Description("Send Date")]
        SentDate = 0,

        [Header("From")]
        [Description("From ( Sender )")]
        SenderName = 1,

        [Header("Type")]
        [Description("Type")]
        Type = 2,
    }
}
