using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiEmploymentHistoryListItem
    {
        private DateTime startDate;

        public EsiEmploymentHistoryListItem()
        {
            startDate = DateTime.MinValue;
        }

        [DataMember(Name = "record_id")]
        public int RecordID { get; set; }

        [DataMember(Name = "corporation_id")]
        public long CorporationID { get; set; }

        [DataMember(Name = "is_deleted", IsRequired = false)]
        public bool Closed { get; set; }

        [DataMember(Name = "start_date")]
        public string StartDateJson
        {
            get { return startDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    startDate = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
            }
        }
    }
}
