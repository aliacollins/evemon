namespace EVEMon.Common.Net
{
    /// <summary>
    /// This interface allows implementers to register to the <see cref="NetworkMonitor"/> class to track network avilability changes.
    /// </summary>
    public interface INetworkChangeSubscriber
    {
        /// <summary>
        /// Notifies the network availability changed.
        /// </summary>
        /// <param name="isAvailable"></param>
        void SetNetworkStatus(bool isAvailable);
    }
}