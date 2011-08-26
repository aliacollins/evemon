namespace EVEMon.Common.SettingsObjects
{
    public interface IColumnSettings
    {
        bool Visible { get; set; }
        int Key { get; }
    }
}