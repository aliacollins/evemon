using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    public static class ListViewExporter
    {
        private static readonly SaveFileDialog s_saveFileDialog = new SaveFileDialog();

        /// <summary>
        /// This method takes a list view and returns a multi-line string that represents the listview as a CSV (comma-delimited) file.  The major
        /// difference is that the list view assumes to contain units, so if the values in each column contain two values seperated by a space
        /// and the second value is the same in every column (1 and beyond), the unit is seperated out and placed in column "2".  This allows the
        /// values to be imported into the spreadsheet software as a number, instead of a string enabling numerical analysis of the export.
        /// </summary>
        /// <param name="listViewToExport">as noted.</param>
        /// <returns>A CSV text file.</returns>
        public static void CreateCSV(ListView listViewToExport)
        {
            s_saveFileDialog.Filter = "Comma Delimited Files (*.csv)|*.csv";
            if (s_saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            StringBuilder sb = new StringBuilder();

            // Export the column headers
            bool ignoreComma = true;
            for (int i = 0; i < listViewToExport.Columns.Count; i++)
            {
                ColumnHeader myColumn = listViewToExport.Columns[i];
                sb.Append(MakeCSVString(ignoreComma, myColumn.Text));
                ignoreComma = false;

                if (i == 0)
                    sb.Append(MakeCSVString(false, "Unit"));
            }
            sb.AppendLine();

            for (int line = 0; line < listViewToExport.Items.Count; line++)
            {
                // Determine if the items have a unit description
                string[] elements = listViewToExport.Items[line].SubItems[1].Text.Split(" ".ToCharArray());
                string possibleUnit = String.Join(" ", elements.Skip(1));
                bool hasUnit = elements.Length > 1 && StaticProperties.AllProperties.Any(prop => prop.Unit == possibleUnit);
                string unit = (hasUnit ? possibleUnit : String.Empty);

                // Export the lines
                ignoreComma = true;
                int maxElements = listViewToExport.Columns.Count;

                if (listViewToExport.Items[line].SubItems.Count < maxElements)
                    maxElements = listViewToExport.Items[line].SubItems.Count;

                for (int subitem = 0; (subitem < maxElements); subitem++)
                {
                    elements = listViewToExport.Items[line].SubItems[subitem].Text.Split(" ".ToCharArray());

                    double number;
                    // If the value is a number format it as so; as string otherwise
                    sb.Append(Double.TryParse(elements[0], out number)
                                  ? MakeCSVNumber(ignoreComma, number.ToString(CultureConstants.InvariantCulture))
                                  : MakeCSVString(ignoreComma, listViewToExport.Items[line].SubItems[subitem].Text));

                    ignoreComma = false;

                    if (subitem == 0)
                        sb.Append(MakeCSVString(false, unit));
                }
                sb.AppendLine();
            }

            File.WriteAllText(s_saveFileDialog.FileName, sb.ToString());
        }

        /// <summary>
        /// Makes the CSV string.
        /// </summary>
        /// <param name="ignoreComma">if set to <c>true</c> ignore comma.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static String MakeCSVString(bool ignoreComma, string text)
        {
            return MakeCSVNumber(ignoreComma, String.Format("\"{0}\"", text));
        }

        /// <summary>
        /// Makes the CSV number.
        /// </summary>
        /// <param name="ignoreComma">if set to <c>true</c> ignore comma.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static String MakeCSVNumber(bool ignoreComma, string text)
        {
            return String.Format("{0}{1}", ignoreComma ? String.Empty : ",", text);
        }
    }
}