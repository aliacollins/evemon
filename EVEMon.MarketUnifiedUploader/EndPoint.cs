using System;

namespace EVEMon.MarketUnifiedUploader
{
    public sealed class EndPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPoint"/> class.
        /// </summary>
        internal EndPoint()
        {
            UploadInterval = TimeSpan.Zero;
            NextUploadTimeUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EndPoint"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        internal Uri URL { get; set; }

        /// <summary>
        /// Gets or sets the upload key.
        /// </summary>
        /// <value>
        /// The upload key.
        /// </value>
        internal string UploadKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gzip uploading is supported.
        /// </summary>
        /// <value>
        ///   <c>true</c> if gzip uploading is supported; otherwise, <c>false</c>.
        /// </value>
        internal bool GzipSupport { get; set; }

        /// <summary>
        /// Gets or sets the upload interval.
        /// </summary>
        /// <value>
        /// The upload interval.
        /// </value>
        internal TimeSpan UploadInterval { get; set; }

        /// <summary>
        /// Gets or sets the next upload time UTC.
        /// </summary>
        /// <value>
        /// The next upload time UTC.
        /// </value>
        internal DateTime NextUploadTimeUtc { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
