using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Helpers
{
    public static class ListViewExporter
    {
        /// <summary>
        /// This method takes a list view and returns a multi-line string that represents the listview as a CSV (comma-delimited) file.
        /// </summary>
        /// <param name="listViewToExport">Thr listview to export.</param>
        /// <param name="withUnit">if set to <c>true</c> listView is exported with unit column.</param>
        /// <remarks>
        /// For delimiter we use the semicolon in order to support decimal and thousand seperator in different cultures.
        /// The major difference is that the list view assumes to contain units, so if the values in each column contain two values seperated by a space
        /// and the second value is the same in every column (1 and beyond), the unit is seperated out and placed in column "2". This allows the
        /// values to be imported into the spreadsheet software as a number, instead of a string enabling numerical analysis of the export.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">listViewToExport</exception>
        public static void CreateCSV(ListView listViewToExport, bool withUnit = false)
        {
            listViewToExport.ThrowIfNull(nameof(listViewToExport));

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = @"Comma Delimited Files (Semicolon) (*.csv)|*.csv";
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                StringBuilder sb = new StringBuilder();

                // Export the column headers
                bool ignoreSemicolon = true;
                for (int i = 0; i < listViewToExport.Columns.Count; i++)
                {
                    ColumnHeader myColumn = listViewToExport.Columns[i];
                    sb.Append(MakeCSVString(myColumn.Text, ignoreSemicolon));
                    ignoreSemicolon = false;

                    if (i == 0 && withUnit)
                        sb.Append(MakeCSVString("Unit"));
                }
                sb.AppendLine();

                for (int line = 0; line < listViewToExport.Items.Count; line++)
                {
                    string[] elements = listViewToExport.Items[line].SubItems[1].Text.Split(" ".ToCharArray());
                    string unit = string.Empty;
                    if (withUnit)
                    {
                        // Determine if the items have a unit description
                        string possibleUnit = string.Join(" ", elements.Skip(1));
                        bool hasUnit = elements.Length > 1 &&
                                       StaticProperties.AllProperties.Any(prop => prop.Unit == possibleUnit);
                        if (hasUnit)
                            unit = possibleUnit;
                    }

                    // Export the lines
                    ignoreSemicolon = true;
                    int maxElements = listViewToExport.Columns.Count;

                    if (listViewToExport.Items[line].SubItems.Count < maxElements)
                        maxElements = listViewToExport.Items[line].SubItems.Count;

                    for (int subitem = 0; subitem < maxElements; subitem++)
                    {
                        elements = listViewToExport.Items[line].SubItems[subitem].Text.Split(" ".ToCharArray());

                        // If the value is a number format it as so; as string otherwise
                        double number;
                        sb.Append(double.TryParse(elements[0], out number) ? MakeCSVNumber(
                            number.ToString(CultureConstants.DefaultCulture), ignoreSemicolon) :
                            MakeCSVString(listViewToExport.Items[line].SubItems[subitem].Text,
                            ignoreSemicolon));

                        ignoreSemicolon = false;

                        if (subitem == 0 && withUnit)
                            sb.Append(MakeCSVString(unit));
                    }
                    sb.AppendLine();
                }

                File.WriteAllText(saveFileDialog.FileName, sb.ToString());
            }
        }

        /// <summary>
        /// Makes the CSV string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="ignoreSemicolon">if set to <c>true</c> ignore semicolon.</param>
        /// <returns></returns>
        private static string MakeCSVString(string text, bool ignoreSemicolon = false)
            => MakeCSVNumber($"\"{text}\"", ignoreSemicolon);

        /// <summary>
        /// Makes the CSV number.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="ignoreSemicolon">if set to <c>true</c> ignore semicolon.</param>
        /// <returns></returns>
        private static string MakeCSVNumber(string text, bool ignoreSemicolon = false)
            => $"{(ignoreSemicolon ? string.Empty : ";")}{text}";
    }
}
