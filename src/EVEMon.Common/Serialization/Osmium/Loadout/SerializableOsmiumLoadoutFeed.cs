using System;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Serialization.Osmium.Loadout
{
    public sealed class SerializableOsmiumLoadoutFeed
    {
        public long ID
        {
            get
            {
                return Int64.Parse(
                    Uri.Replace(
                        String.Format(CultureConstants.InvariantCulture, "{0}/loadout/", NetworkConstants.OsmiumBaseUrl),
                        String.Empty));
            }
        }

        public string Uri { get; set; }

        public string Name { get; set; }

        public int ShipTypeID { get; set; }

        public string ShipTypeName { get; set; }

        public SerializableOsmiumLoadoutAuthor Author { get; set; }

        public long CreationDate { get; set; }

        public string RawDescription { get; set; }

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public int Rating
        {
            get { return UpVotes - DownVotes; }
        }
    }
}