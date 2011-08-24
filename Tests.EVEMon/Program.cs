using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tests.EVEMon
{
    internal static class Program
    {
        // Some constants we are going to use
        private static readonly string[] s_programsPath = new[] { @"C:\Program Files\", @"C:\Program Files (x86)\" };
        private const string ExecutableName = "nunit-x86.exe"; // must be the x86 version of nUnit because this is a x86 Project
        private const string SearchPattern = "nunit *";
        private const string BinFolder = "bin/net-2.0";
        private const string Arguments = "/run \"{0}\"";

        [STAThread]
        public static void Main()
        {
            string executable = GetNUnitExecutable();

            if (String.IsNullOrEmpty(executable) || !File.Exists(executable))
            {
                // Could not any version of nUnit
                Console.WriteLine("****");
                Console.WriteLine("Did not find {0}, check it is installed.", ExecutableName);
                Console.ReadKey();
                return;
            }

            // Great we found nUnit now lets start it
            ProcessStartInfo psi = new ProcessStartInfo(executable)
                                       {Arguments = String.Format(Arguments, Assembly.GetEntryAssembly().Location)};
            Process proc = new Process {StartInfo = psi};
            proc.Start();
        }

        /// <summary>
        /// Gets the full path of the NUnit executable.
        /// </summary>
        /// <returns></returns>
        private static string GetNUnitExecutable()
        {
            List<PathVersion> versions = GetAllNUnitInstalls();

            if (versions.IsEmpty())
                return String.Empty;

            Version verMax = versions.Select(x => x.Version).Max();

            string newestInstall = versions.Where(x => x.Version == verMax).First().Path;

            string binPath = Path.Combine(newestInstall, BinFolder);
            return Path.Combine(binPath, ExecutableName);
        }

        /// <summary>
        /// Gets a list of all installs of NUnit.
        /// </summary>
        /// <returns></returns>
        private static List<PathVersion> GetAllNUnitInstalls()
        {
            return (s_programsPath.Where(Directory.Exists).Select(
                path => Directory.GetDirectories(path, SearchPattern)).Where(
                    matchingFolders => matchingFolders.Length != 0).SelectMany(
                        matchingFolders => matchingFolders,
                        (matchingFolders, matchingFolder) => new {matchingFolders, matchingFolder}).Select(
                            folder => new {folder, fileName = Path.GetFileName(folder.matchingFolder)}).Where(
                                folder => folder.fileName != null).Select(
                                    fileName => new {fileName, versionName = fileName.fileName.Remove(0, 6)}).Select(
                                        fileVersion => new {fileVersion, version = new Version(fileVersion.versionName)}).Select(
                                            file => new PathVersion(file.fileVersion.fileName.folder.matchingFolder, file.version)))
                .ToList();

        }
    }
}
