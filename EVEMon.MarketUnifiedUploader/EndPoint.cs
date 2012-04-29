using System;

namespace EVEMon.MarketUnifiedUploader
{
    public sealed class EndPoint
    {
        public EndPoint()
        {
            UploadInterval = TimeSpan.Zero;
            NextUploadTimeUtc = DateTime.UtcNow;
        }

        public bool Enabled { get; set; }

        public string Name { get; set; }

        public Uri URL { get; set; }

        public string UploadKey { get; set; }

        public bool GzipSupport { get; set; }

        public TimeSpan UploadInterval { get; set; }

        public DateTime NextUploadTimeUtc { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
