namespace EVEMon.Common.Enumerations.CCPAPI
{
    public enum CCPJobCompletedStatus
    {
        Installed = 1,
        Paused = 2,
        Ready = 3,

        Delivered = 101,
        Canceled = 102,
        Reverted = 103,
    }
}