using System;
using System.IO;
using System.Reflection;

namespace EVEMon.SQLiteToSql
{
    internal static class Util
    {
        private static string s_text;
        private static int s_counter;
        private static int s_percentOld;

        /// <summary>
        /// Updates the percent done.
        /// </summary>
        /// <param name="total">The total.</param>
        internal static void UpdatePercentDone(int total)
        {
            s_counter++;
            int percent = (s_counter * 100 / total);

            if (s_counter != 1 && s_percentOld >= percent)
                return;

            if (!String.IsNullOrEmpty(s_text))
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);

            s_text = String.Format("{0}%", percent);
            Console.Write(s_text);
            s_percentOld = percent;
        }

        /// <summary>
        /// Displays the end time.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        internal static void DisplayEndTime(DateTime startTime)
        {
            Console.WriteLine(@" in {0}", DateTime.Now.Subtract(startTime).ToString("g"));
        }

        /// <summary>
        /// Resets the counters.
        /// </summary>
        internal static void ResetCounters()
        {
            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
        }

        /// <summary>
        /// Gets the script for the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        internal static string GetScriptFor(string tableName)
        {
            var resourceName = String.Format(@"EVEMon.SQLiteToSql.Scripts.{0}.table.sql", tableName);

            string result = null;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(result))
                return result;

            Console.WriteLine(@"{0}.table.sql resource file does not exists!", tableName);
            return String.Empty;
        }

        /// <summary>
        /// Checks the yaml file exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        internal static string CheckYamlFileExists(string filename)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                String.Format(@"YamlFiles{0}{1}", Path.DirectorySeparatorChar, filename));

            if (File.Exists(filePath))
                return filePath;

            Console.WriteLine(@"{0} file does not exists!", filename);
            return String.Empty;
        }
    }
}