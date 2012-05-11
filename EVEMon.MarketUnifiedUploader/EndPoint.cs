using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common;

namespace EVEMon.MarketUnifiedUploader
{
    public sealed class EndPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPoint"/> class.
        /// </summary>
        internal EndPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPoint"/> class.
        /// </summary>
        /// <param name="endPoint">The endpoint.</param>
        internal EndPoint(Dictionary<string, object> endPoint)
        {
            Name = endPoint.Keys.Contains("name") ? endPoint["name"].ToString() : String.Empty;
            Enabled = endPoint.Keys.Contains("enabled") && Convert.ToBoolean(endPoint["enabled"].ToString());
            UploadKey = endPoint.Keys.Contains("key") ? endPoint["key"].ToString() : "0";

            if (endPoint.Keys.Contains("url"))
                URL = new Uri(endPoint["url"].ToString());

            if (endPoint.Keys.Contains("method") &&
                Enum.IsDefined(typeof(HttpMethod), endPoint["method"].ToString().ToTitleCase()))
                Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), endPoint["method"].ToString().ToTitleCase());

            if (endPoint.Keys.Contains("compression") &&
                Enum.IsDefined(typeof(Compression), endPoint["compression"].ToString().ToTitleCase()))
                Compression = (Compression)Enum.Parse(typeof(Compression), endPoint["compression"].ToString().ToTitleCase());
        }


        #region Properties

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
        /// Gets or sets the compression.
        /// </summary>
        /// <value>
        /// The compression.
        /// </value>
        internal Compression Compression { get; set; }

        /// <summary>
        /// Gets or sets the http method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        internal HttpMethod Method { get; set; }

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

        #endregion


        #region Overriden Methods

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

        #endregion
    }
}
