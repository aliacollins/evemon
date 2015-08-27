using System;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.XmlGenerator.Xmlfiles.Serialization;

namespace EVEMon.XmlGenerator.Xmlfiles
{
    public static class Flags
    {
        internal static void GenerateXmlfile()
        {
            DateTime startTime = DateTime.Now;

            Console.WriteLine();
            Console.Write("Generating Flags Xml file ");

            SerializableRoot<SerializableInvFlagsRow> flags = new SerializableRoot<SerializableInvFlagsRow>
            {
                Rowset = new SerialiazableRowset<SerializableInvFlagsRow>
                {
                    Name = "flags",
                    Key = "flagID",
                    Columns = "flagID,flagName,flagText"
                }
            };

            flags.Rowset.Rows.AddRange(Database.InvFlagsTable.Select(
                flag => new SerializableInvFlagsRow
                {
                    ID = flag.ID,
                    Name = flag.Name,
                    Text = flag.Text
                }));

            Util.DisplayEndTime(startTime);

            // Serialize
            Util.SerializeXMLTo(flags, "invFlags", "Flags.xml");
        }
    }
}