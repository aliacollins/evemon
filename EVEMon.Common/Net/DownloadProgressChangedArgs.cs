namespace EVEMon.Common.Net
{
    /// <summary>
    /// Event args class to return download progress information
    /// </summary>
    public class DownloadProgressChangedArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadProgressChangedArgs"/> class.
        /// </summary>
        /// <param name="totalBytesToReceive">The total bytes to receive.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <param name="progressPercentage">The progress percentage.</param>
        internal DownloadProgressChangedArgs(long totalBytesToReceive, long bytesReceived, int progressPercentage)
        {
            TotalBytesToReceive = totalBytesToReceive;
            ProgressPercentage = progressPercentage;
            BytesReceived = bytesReceived;
        }

        /// <summary>
        /// Gets or sets the total bytes to receive.
        /// </summary>
        /// <value>The total bytes to receive.</value>
        public long TotalBytesToReceive { get; private set; }

        /// <summary>
        /// Gets or sets the bytes received.
        /// </summary>
        /// <value>The bytes received.</value>
        public long BytesReceived { get; private set; }

        /// <summary>
        /// Gets or sets the progress percentage.
        /// </summary>
        /// <value>The progress percentage.</value>
        public int ProgressPercentage { get; private set; }
    }
}