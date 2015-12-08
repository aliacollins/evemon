using System;

namespace EVEMon.Common.CloudStorageServices
{
    public sealed class SerializableAPIResult<T>
    { 
        public T Result { get; set; }

        public CloudStorageServiceAPIError Error { get; set; }

        public DateTime CacheExpires { get; set; }

        public bool HasError
        {
            get { return Error != null; }
        }
    }
}