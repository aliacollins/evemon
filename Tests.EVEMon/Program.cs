using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tests.EVEMon
{
    internal static class Program
    {
        // Some constants we are going to use
        private static readonly string[] s_programsPath = new[]
                                                              {
                                                                  Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                                                  Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                                                              };
        private const string NUnitExecutableName = "nunit-x86.exe"; // must be the x86 version of nUnit because this is a x86 Project
        private const string SearchPattern = "nunit *";
        private const string BinFolder = "bin";
        private const string Arguments = "/run \"{0}\"";

        [STAThread]
        public static void Main()
        {
            string executable = GetNUnitExecutable();

            if (String.IsNullOrEmpty(executable) || !File.Exists(executable))
            {
                // Could not any version of nUnit
                Console.WriteLine("****");
                Console.WriteLine("NUnit executable was not found, check it is installed.");
                Console.WriteLine("****");
                Console.ReadLine();
                return;
            }

            // Great we found nUnit now lets start it
            ProcessStartInfo psi = new ProcessStartInfo(executable)
                                       {
                                           Arguments = String.Format(CultureInfo.InvariantCulture, Arguments,
                                                                     Assembly.GetEntryAssembly().Location)
                                       };

            using (Process proc = new Process())
            {
                proc.StartInfo = psi;
                proc.Start();
            }
        }

        /// <summary>
        /// Gets the full path of the NUnit executable.
        /// </summary>
        /// <returns></returns>
        private static string GetNUnitExecutable()
        {
            IEnumerable<PathVersion> versions = GetAllNUnitInstalls();

            if (!versions.Any())
                return String.Empty;

            Version verMax = versions.Select(x => x.Version).Max();

            string newestInstall = versions.First(x => x.Version == verMax).Path;
            return Path.Combine(newestInstall, BinFolder, NUnitExecutableName);
        }

        /// <summary>
        /// Gets a list of all installs of NUnit.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<PathVersion> GetAllNUnitInstalls()
        {
            return (s_programsPath.Where(Directory.Exists).Select(
                path => Directory.GetDirectories(path, SearchPattern)).Where(
                    matchingFolders => matchingFolders.Length != 0).SelectMany(
                        matchingFolders => matchingFolders,
                        (matchingFolders, matchingFolder) => new { matchingFolders, matchingFolder }).Select(
                            folder => new { folder, fileName = Path.GetFileName(folder.matchingFolder) }).Where(
                                folder => folder.fileName != null).Select(
                                    fileName => new { fileName, versionName = fileName.fileName.Remove(0, 6) }).Select(
                                        fileVersion => new { fileVersion, version = new Version(fileVersion.versionName) }).Select
                (file => new PathVersion(file.fileVersion.fileName.folder.matchingFolder, file.version)));
        }
    }
}