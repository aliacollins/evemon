using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EVEMon {

    public static class ListViewExporter {

        private static SaveFileDialog _SaveFileDialog = new SaveFileDialog();

        /// <summary>
        /// This method takes a list view and returns a multi-line string that represents the listview as a CSV (comma-delimited) file.  The major
        /// difference is that the list view assumes to contain units, so if the values in each column contain two values seperated by a space
        /// and the second value is the same in every column (1 and beyond), the unit is seperated out and placed in column "2".  This allows the
        /// values to be imported into the spreadsheet software as a number, instead of a string enabling numerical analysis of the export.
        /// </summary>
        /// <param name="aListViewToExport">as noted.</param>
        /// <returns>A CSV text file.</returns>
        public static void CreateCSV(ListView aListViewToExport) {
            _SaveFileDialog.Filter = "Comma Delimited Files (*.csv)|*.csv";
            if ( _SaveFileDialog.ShowDialog() == DialogResult.OK ) {
                StringBuilder SB = new StringBuilder();

                // Export the column headers
                bool aFirst = true;
                for (int I = 0; I < aListViewToExport.Columns.Count; I++) {
                    ColumnHeader myColumn = aListViewToExport.Columns[I];
                    SB.Append(MakeCSVString(aFirst, myColumn.Text));
                    aFirst = false;

                    if (I == 0)
                        SB.Append(MakeCSVString(aFirst, "Unit"));
                }
                SB.AppendLine();

                for (int LINE = 0; LINE < aListViewToExport.Items.Count; LINE++) {
                    // Test to see if each value of the line column 1 to N contains one space and the unit is the same throughout
                    bool hasCommonUnit = true;
                    string UNIT = "";
                    for (int COLUMN = 1; (COLUMN < aListViewToExport.Items[LINE].SubItems.Count) && hasCommonUnit; COLUMN++) {
                        string[] ELEMENTS = aListViewToExport.Items[LINE].SubItems[COLUMN].Text.Split(" ".ToCharArray());
                        hasCommonUnit = ELEMENTS.Length == 2;
                        if (hasCommonUnit)
                            if (UNIT == "")
                                UNIT = ELEMENTS[1];
                            else
                                hasCommonUnit = (ELEMENTS[1] == UNIT);
                    }

                    // Export the lines
                    aFirst = true;
                    int MAX_ELEMENTS = aListViewToExport.Columns.Count;
                    if (aListViewToExport.Items[LINE].SubItems.Count < MAX_ELEMENTS)
                        MAX_ELEMENTS = aListViewToExport.Items[LINE].SubItems.Count;

                    for (int SUBITEM = 0; (SUBITEM < MAX_ELEMENTS); SUBITEM++) {
                        try {
                            string[] ELEMENTS = aListViewToExport.Items[LINE].SubItems[SUBITEM].Text.Split(" ".ToCharArray());
                            if (hasCommonUnit && ELEMENTS.Length == 2 && ELEMENTS[1] == UNIT) {
                                SB.Append(MakeCSVNumber(aFirst, Convert.ToDouble(ELEMENTS[0].Replace(",", "")).ToString()));
                            }
                            else {
                                SB.Append(MakeCSVNumber(aFirst, Convert.ToDouble(aListViewToExport.Items[LINE].SubItems[SUBITEM].Text).ToString()));
                            }
                        }
                        catch {
                            SB.Append(MakeCSVString(aFirst, aListViewToExport.Items[LINE].SubItems[SUBITEM].Text));
                        }
                        aFirst = false;
                        if (SUBITEM == 0) {
                            SB.Append(MakeCSVString(aFirst, UNIT));
                        }
                    }
                    SB.AppendLine();
                }

                File.WriteAllText(_SaveFileDialog.FileName,SB.ToString());
            }
        }


        private static string MakeCSVString(bool aIgnoreComma, string aWhat) {
            return MakeCSVNumber(aIgnoreComma, @"""" + aWhat.Replace("\"", "\"\"") + @"""");
        }

        private static string MakeCSVNumber(bool aIgnoreComma, string aWhat) {
            return ((aIgnoreComma) ? "" : ",") + aWhat;

        }
    }
}
