using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;

namespace EVEMon.XmlGenerator.Helpers
{
    public static class Util
    {
        private static string s_text;
        private static string s_solutionDir;
        private static string s_outputPath;
        private static string s_projectDir;
        private static int s_counter;
        private static int s_tablesCount;
        private static double s_percentOld;


        /// <summary>
        /// Serializes a XML file to EVEMon.Common\Resources.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datafile">The datafile.</param>
        /// <param name="filename">The filename.</param>
        internal static void SerializeXml<T>(T datafile, string filename)
        {
            string path = Path.Combine(GetSolutionDirectory(), @"EVEMon.Common\Resources", filename);

            FileStream stream = Common.Util.GetFileStream(path, FileMode.Create, FileAccess.Write);

            using (GZipStream zstream = new GZipStream(stream, CompressionMode.Compress))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(zstream, datafile);
                zstream.Flush();
            }

            Console.WriteLine(@"-----------------------------------------------");
            Console.WriteLine("Updated : {0}", filename);
            Console.WriteLine(@"-----------------------------------------------");

            // As long as EVEMon.Common is not rebuilt, files are not updated in output directories
            Copy(path, Path.Combine(GetSolutionDirectory(), @"EVEMon.Common\", GetOutputPath(), "Resources", filename));

            // Update the file in the settings directory
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Copy(path, Path.Combine(appData, "EVEMon", filename));

            Console.WriteLine();
        }

        /// <summary>
        /// Serializes a XML file to EVEMon.Common\Serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serial">The serial.</param>
        /// <param name="xmlRootName">Name of the xml root.</param>
        /// <param name="filename">The filename.</param>
        internal static void SerializeXmlTo<T>(T serial, string xmlRootName, string filename)
        {
            string path = Path.Combine(GetSolutionDirectory(), @"EVEMon.Common\Serialization", filename);
            using (FileStream stream = Common.Util.GetFileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootName));
                serializer.Serialize(stream, serial);
                stream.Flush();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Creates one file alongside the resources file containing
        /// the MD5 sums for each resource.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        internal static void CreateMD5SumsFile(string filename)
        {
            ResetCounters();

            Console.WriteLine();

            string resourcesPath = Path.Combine(GetSolutionDirectory(), @"EVEMon.Common\Resources");
            string md5SumsFileFullPath = Path.Combine(resourcesPath, filename);

            using (StreamWriter md5SumsFile = File.CreateText(md5SumsFileFullPath))
            {
                foreach (string file in Datafile.GetFilesFrom(resourcesPath, Datafile.DatafilesExtension))
                {
                    FileInfo datafile = new FileInfo(file);
                    if (!datafile.Exists)
                        throw new FileNotFoundException(String.Format(CultureConstants.DefaultCulture, "{0} not found!", file));

                    string line = String.Format(CultureConstants.DefaultCulture, "{0} *{1}", Common.Util.CreateMD5From(file),
                        datafile.Name);
                    md5SumsFile.WriteLine(line);
                }
            }

            Console.WriteLine(@"MD5Sums file created successfully");
            Console.WriteLine();
        }

        /// <summary>
        /// Copies a file from source to destination.
        /// </summary>
        /// <param name="srcFile">Fully qualified source filename</param>
        /// <param name="destFile">Fully quallified destination filename</param>
        private static void Copy(string srcFile, string destFile)
        {
            try
            {
                FileInfo fi = new FileInfo(destFile);
                if (fi.Directory == null)
                    return;

                if (fi.Directory.Exists && fi.Directory.Parent != null && fi.Directory.Parent.Parent != null)
                {
                    File.Copy(srcFile, destFile, true);
                    Console.WriteLine(@"*** {0}\{1}\{2}", fi.Directory.Parent.Parent.Name, fi.Directory.Parent.Name,
                        fi.Directory.Name);
                }
                else
                {
                    Trace.WriteLine(String.Format(CultureConstants.DefaultCulture, "{0} doesn't exist, copy failed",
                        fi.Directory.FullName));
                }
            }
            catch (IOException exc)
            {
                Trace.WriteLine(exc);
            }
            catch (UnauthorizedAccessException exc)
            {
                Trace.WriteLine(exc);
            }
        }


        #region Helper Methods

        /// <summary>
        /// Gets the solution directory.
        /// </summary>
        /// <returns></returns>
        private static String GetSolutionDirectory()
        {
            if (String.IsNullOrWhiteSpace(s_solutionDir))
                s_solutionDir = Regex.Match(Directory.GetCurrentDirectory(), @"[a-zA-Z]+:.*\\(?=Tools)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase).ToString();
            return s_solutionDir;
        }

        /// <summary>
        /// Gets the project directory.
        /// </summary>
        /// <returns></returns>
        private static string GetProjectDirectory()
        {
            if (String.IsNullOrWhiteSpace(s_projectDir))
            {
                s_projectDir = Regex.Match(Directory.GetCurrentDirectory(), @"[a-zA-Z]+:.*\\(?=bin)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase).ToString();
            }
            return s_projectDir;
        }

        /// <summary>
        /// Gets the output path.
        /// </summary>
        private static String GetOutputPath()
        {
            if (String.IsNullOrWhiteSpace(s_outputPath))
            {
                s_outputPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Remove(0, GetProjectDirectory().Length);
            }
            return s_outputPath;
        }

        /// <summary>
        /// Resets the counters.
        /// </summary>
        internal static void ResetCounters()
        {
            if (Debugger.IsAttached)
                Console.WriteLine(s_counter);

            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
        }

        /// <summary>
        /// Updates the percantage done of the datafile generating procedure.
        /// </summary>
        /// <param name="totalCount">The total count.</param>
        internal static void UpdatePercentDone(int totalCount)
        {
            s_counter++;
            double percent = totalCount > 0 ? (s_counter / (double)totalCount) : 0;

            if (s_counter != 1 && s_percentOld >= percent)
                return;

            if (!String.IsNullOrEmpty(s_text))
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);

            s_text = String.Format(CultureConstants.DefaultCulture, "{0:P0}", percent);
            Console.Write(s_text);
            s_percentOld = percent;
        }

        /// <summary>
        /// Updates the progress of data loaded from SQL server.
        /// </summary>
        /// <param name="totalCount">The total count.</param>
        internal static void UpdateProgress(int totalCount)
        {
            if (!String.IsNullOrEmpty(s_text))
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);

            s_tablesCount++;
            s_text = String.Format(CultureConstants.DefaultCulture, "{0:P0}", (s_tablesCount / (double)totalCount));
            Console.Write(s_text);
        }

        /// <summary>
        /// Gets the count of types in a specified namespace.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <returns></returns>
        internal static int GetCountOfTypesInNamespace(string nameSpace)
        {
            return typeof(Program).Assembly.GetTypes().Count(type => type.Namespace == nameSpace);
        }

        /// <summary>
        /// Displays the end time.
        /// </summary>
        /// <param name="stopwatch">The stopwatch.</param>
        internal static void DisplayEndTime(Stopwatch stopwatch)
        {
            Console.WriteLine(@" in {0:g}", stopwatch.Elapsed);
        }

        #endregion
    }
}