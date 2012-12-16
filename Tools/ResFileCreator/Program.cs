using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EVEMon.ResFileCreator
{
    internal static class Program
    {
        private static readonly string[] s_programFilesFolders = new[]
                                                                     {
                                                                         "Program Files",
                                                                         "Program Files (x86)"
                                                                     };
        private static readonly string[] s_sdkVersions = new [] { "7.1A", "7.1", "7.0A", "7.0" };
        private static readonly Dictionary<string, object> s_dictionary = new Dictionary<string, object>();
        private static string s_assemblyInfoFilePath;
        private static string s_assemblyInfoFileContent;
        private static string s_filePath;
        private static string s_rcexe;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <returns></returns>
        [STAThread]
        private static void Main()
        {
            Directory.SetCurrentDirectory(@"..\..\..\..\..");

            s_assemblyInfoFilePath = Path.GetFullPath(@"EVEMon\Properties\AssemblyInfo.cs");

            s_rcexe = FindRcExe();
            if (String.IsNullOrEmpty(s_rcexe))
            {
                Console.WriteLine("RC : Not Found - Resource file will not be created.");
                return;
            }

            ParserAssemblyInfo();

            if (!GenerateRcFile())
                return;

            CreateResFile();
            File.Delete(s_filePath);
        }

        /// <summary>
        /// Parsers the assembly info.
        /// </summary>
        private static void ParserAssemblyInfo()
        {
            s_assemblyInfoFileContent = File.ReadAllText(s_assemblyInfoFilePath);
            s_dictionary["AssemblyTitle"] = GetValueOf("AssemblyTitle");
            s_dictionary["AssemblyCompany"] = GetValueOf("AssemblyCompany");
            s_dictionary["AssemblyProduct"] = GetValueOf("AssemblyProduct");
            s_dictionary["AssemblyCopyright"] = GetValueOf("AssemblyCopyright");
            s_dictionary["AssemblyVersion"] = new Version(GetValueOf("AssemblyVersion"));
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static string GetValueOf(string key)
        {
            int index = s_assemblyInfoFileContent.IndexOf(key, StringComparison.InvariantCulture) + key.Length;
            string substring = s_assemblyInfoFileContent.Substring(index);
            int length = substring.IndexOf(")", StringComparison.InvariantCulture) - 1;
            string value = s_assemblyInfoFileContent.Substring(index, length).Replace("(\"", String.Empty).Replace("\")",
                                                                                                                   String.Empty);
            return value;
        }

        /// <summary>
        /// Generates the rc file.
        /// </summary>
        /// <returns></returns>
        private static bool GenerateRcFile()
        {
            s_filePath = Path.GetFullPath(String.Format(@"EVEMon\{0}.rc", s_dictionary["AssemblyTitle"]));

            StringBuilder sb = new StringBuilder();

            AddIcons(sb);
            AddManifest(sb);
            AddVersionInfo(sb);

            try
            {
                File.WriteAllText(s_filePath, sb.ToString(), Encoding.Default);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            Console.WriteLine("Resource Compiler file created successfully.");
            return true;
        }

        /// <summary>
        /// Adds the version info.
        /// </summary>
        /// <param name="sb">The sb.</param>
        private static void AddVersionInfo(StringBuilder sb)
        {
            Version version = (Version)s_dictionary["AssemblyVersion"];
            string commaVersion = String.Format("{0},{1},{2},{3}", version.Major, version.Minor, version.Build, version.Revision);
            string dotVersion = String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            sb.AppendLine("// Version");
            sb.AppendLine("1 VERSIONINFO");
            sb.AppendFormat(" FILEVERSION {0}", commaVersion).AppendLine();
            sb.AppendFormat(" PRODUCTVERSION {0}", commaVersion).AppendLine();
            sb.AppendLine(" FILEFLAGSMASK 0x3fL");
            sb.AppendLine("#ifdef _DEBUG");
            sb.AppendLine(" FILEFLAGS 0x1L");
            sb.AppendLine("#else");
            sb.AppendLine(" FILEFLAGS 0x0L");
            sb.AppendLine("#endif");
            sb.AppendLine(" FILEOS 0x40004L");
            sb.AppendLine(" FILETYPE 0x0L");
            sb.AppendLine(" FILESUBTYPE 0x0L");

            sb.AppendLine("BEGIN");
            sb.AppendLine("    BLOCK \"StringFileInfo\"");
            sb.AppendLine("    BEGIN");
            sb.AppendLine("        BLOCK \"000004b0\"");
            sb.AppendLine("        BEGIN");
            sb.AppendFormat("            VALUE \"CompanyName\", \"{0}\"", s_dictionary["AssemblyCompany"]).AppendLine();
            sb.AppendFormat("            VALUE \"FileDescription\", \"{0}\"", s_dictionary["AssemblyTitle"]).AppendLine();
            sb.AppendFormat("            VALUE \"FileVersion\", \"{0}\"", dotVersion).AppendLine();
            sb.AppendFormat("            VALUE \"InternalName\", \"{0}.exe\"", s_dictionary["AssemblyTitle"]).AppendLine();
            sb.AppendFormat("            VALUE \"LegalCopyright\", \"{0}\"", s_dictionary["AssemblyCopyright"]).AppendLine();
            sb.AppendFormat("            VALUE \"OriginalFilename\", \"{0}.exe\"", s_dictionary["AssemblyTitle"]).AppendLine();
            sb.AppendFormat("            VALUE \"ProductName\", \"{0}\"", s_dictionary["AssemblyProduct"]).AppendLine();
            sb.AppendFormat("            VALUE \"ProductVersion\", \"{0}\"", dotVersion).AppendLine();
            sb.AppendLine("        END");
            sb.AppendLine("    END");
            sb.AppendLine("    BLOCK \"VarFileInfo\"");
            sb.AppendLine("    BEGIN");
            sb.AppendLine("        VALUE \"Translation\", 0x000, 1200");
            sb.AppendLine("    END");
            sb.AppendLine("END");
        }

        /// <summary>
        /// Adds the manifest.
        /// </summary>
        /// <param name="sb">The sb.</param>
        private static void AddManifest(StringBuilder sb)
        {
            const string ManifestFile = @"EVEMon\\app.manifest";

            if (!File.Exists(ManifestFile))
                return;

            sb.AppendLine("// Manifest");
            sb.AppendFormat("1 24 \"{0}\"", ManifestFile);
            sb.AppendLine();
        }

        /// <summary>
        /// Adds the icons.
        /// </summary>
        /// <param name="sb">The sb.</param>
        private static void AddIcons(StringBuilder sb)
        {
            const string IconsDir = @"EVEMon.Common\Resources\Icons";
            List<string> iconFilesPath = new List<string>();

            if (Directory.Exists(IconsDir))
                iconFilesPath = Directory.GetFiles(IconsDir, "*.ico", SearchOption.AllDirectories).ToList();

            if (!iconFilesPath.Any())
                return;

            int count = 1;
            string iconEVEMon = iconFilesPath.FirstOrDefault(file => file.Contains("EVEMon.ico"));

            sb.AppendLine("// Icon");
            if (iconEVEMon != null)
            {
                sb.AppendFormat("{0} ICON \"{1}\"", count, iconEVEMon).AppendLine();
                count++;
                iconFilesPath.Remove(iconEVEMon);
            }

            foreach (string iconFilePath in iconFilesPath)
            {
                sb.AppendFormat("{0} ICON \"{1}\"", count, iconFilePath).AppendLine();
                count++;
            }

            sb.AppendLine();
        }

        /// <summary>
        /// Creates the resource file.
        /// </summary>
        private static void CreateResFile()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
                                             {
                                                 FileName = s_rcexe,
                                                 Arguments = String.Format("/r {0}", s_filePath),
                                                 UseShellExecute = false,
                                                 RedirectStandardOutput = true
                                             };
            int exitCode;
            using (Process makeResProcess = new Process())
            {
                makeResProcess.StartInfo = startInfo;
                makeResProcess.Start();
                Console.WriteLine(makeResProcess.StandardOutput.ReadToEnd());
                makeResProcess.WaitForExit();
                exitCode = makeResProcess.ExitCode;
            }

            Console.WriteLine(exitCode != 0 ? "RC exited with errors." : "Resource file compiled successfully.");
        }

        /// <summary>
        /// Finds the rc executable.
        /// </summary>
        /// <returns></returns>
        private static string FindRcExe()
        {
            // Creates all possible folder and sdk version paths
            IEnumerable<string> locations = Environment.GetLogicalDrives()
                .SelectMany(drive => s_programFilesFolders,
                            (drive, programFilesFolder) => new { drive, programFilesFolder })
                .SelectMany(programFilesFolder => s_sdkVersions,
                            (programFilesFolder, sdkVersion) =>
                            String.Format(CultureInfo.InvariantCulture,
                                          @"{0}{1}\Microsoft SDKs\Windows\v{2}\Bin\RC.exe",
                                          programFilesFolder.drive, programFilesFolder.programFilesFolder, sdkVersion)).Where(File.Exists);

            return locations.FirstOrDefault();
        }
    }
}
