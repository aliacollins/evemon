using System;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.XmlGenerator.Xmlfiles.Serialization;

namespace EVEMon.XmlGenerator.Xmlfiles
{
    public static class Flags
    {
        internal static void GenerateXMLfile()
        {
            DateTime startTime = DateTime.Now;

            Console.WriteLine();
            Console.Write("Generating Flags XML file ");

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

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, "in {0}",
                                            DateTime.Now.Subtract(startTime)).TrimEnd('0'));

            // Serialize
            Util.SerializeXMLTo(flags, "invFlags", "Flags.xml");
        }
    }
}