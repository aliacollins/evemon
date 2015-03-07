using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace EVEMon.InstallBuilder
{
    internal static class Program
    {
        private static string s_sourceFilesDir;
        private static string s_outputPath;
        private static string s_installerDir;
        private static string s_snapshotDir;
        private static string s_binariesDir;
        private static string s_solutionDir;
        private static string s_projectDir;
        private static string s_nsisExe;
        private static bool s_isSnapshot;

        private static Version s_fullVersion;
        private static string s_version;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        [STAThread]
        private static int Main(string[] args)
        {
            CheckIsSnapshot();

            if (!HasVersion())
                return 1;

            s_version = String.Format("{0}.{1}.{2}", s_fullVersion.Major, s_fullVersion.Minor, s_fullVersion.Build);

            if (args.Any())
            {
                if (args[0] == "-version" || args[0] == "-v")
                {
                    Console.WriteLine(s_version);
                    return 0;
                }

                if (args[0] == "-version=tc" || args[0] == "-v=tc")
                {
                    Console.WriteLine("##teamcity[buildNumber '{0}']", s_version);
                    return 0;
                }
            }

            try
            {
                if (CheckNsisPresent() && !s_isSnapshot)
                {
                    // Create the appropriate folder if it doesn't exist
                    if (!Directory.Exists(GetInstallerDirectory()))
                        Directory.CreateDirectory(GetInstallerDirectory());

                    // Create an installer in the appropriate folder
                    Console.WriteLine("Starting Installer creation.");
                    if (BuildInstaller() != 0)
                        return 1;
                    Console.WriteLine("Installer creation finished.");
                    Console.WriteLine();
                }

                // Create the appropriate folder if it doesn't exist
                string directory = s_isSnapshot ? GetSnapshotDirectory() : GetBinariesDirectory(); 
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                // Create a zip file in the appropriate folder
                string description = s_isSnapshot ? "Snapshot" : "Binaries";
                Console.WriteLine("Starting {0} Zip creation.", description);
                if (BuildZip() != 0)
                    return 1;
                Console.WriteLine("{0} Zip creation finished.", description);
                Console.WriteLine("Done");

                if (Debugger.IsAttached)
                    Console.ReadLine();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: {0} in {1}", ex.Message, ex.Source);
                Console.WriteLine();
                Console.WriteLine(ex.StackTrace);
                if (Debugger.IsAttached)
                    Console.ReadLine();
                return 1;
            }
        }

        /// <summary>
        /// Gets the solution directory.
        /// </summary>
        /// <returns></returns>
        private static string GetSolutionDirectory()
        {
            if (String.IsNullOrWhiteSpace(s_solutionDir))
            {
                s_solutionDir = Regex.Match(Directory.GetCurrentDirectory(), @"[a-zA-Z]+:.*\\(?=Tools)",
                                            RegexOptions.Compiled | RegexOptions.IgnoreCase).ToString();
            }
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
        /// <returns></returns>
        private static string GetOutputPath()
        {
            if (String.IsNullOrWhiteSpace(s_outputPath))
            {
                s_outputPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Remove(0, GetProjectDirectory().Length);
            }
            return s_outputPath;
        }

        /// <summary>
        /// Gets the source files directory.
        /// </summary>
        /// <returns></returns>
        private static string GetSourceFilesDirectory()
        {
            if (String.IsNullOrWhiteSpace(s_sourceFilesDir))
            {
                s_sourceFilesDir = Path.GetFullPath(Path.Combine(GetSolutionDirectory(), @"EVEMon\", GetOutputPath()));
            }
            return s_sourceFilesDir;
        }

        /// <summary>
        /// Gets the installer directory.
        /// </summary>
        /// <returns></returns>
        private static string GetInstallerDirectory()
        {
            if (String.IsNullOrWhiteSpace(s_installerDir))
            {
                s_installerDir = GetInstallbuilderDirectory("Installer");
            }
            return s_installerDir;
        }

        /// <summary>
        /// Gets the snapshot directory.
        /// </summary>
        /// <returns></returns>
        private static string GetSnapshotDirectory()
        {
            if (String.IsNullOrWhiteSpace(s_snapshotDir))
            {
                s_snapshotDir = GetInstallbuilderDirectory("Snapshot");
            }
            return s_snapshotDir;
        }

        /// <summary>
        /// Gets the binaries directory.
        /// </summary>
        /// <returns></returns>
        private static string GetBinariesDirectory()
        {
            if (String.IsNullOrWhiteSpace(s_binariesDir))
            {
                s_binariesDir = GetInstallbuilderDirectory("Binaries");
            }
            return s_binariesDir;
        }

        /// <summary>
        /// Gets the installbuilder directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        private static string GetInstallbuilderDirectory(string directory)
        {
            return
                Path.GetFullPath(Regex.Replace(GetSourceFilesDirectory(), "Debug|Release",
                                               Path.Combine("Installbuilder", directory),
                                               RegexOptions.Compiled | RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Checks the configuration is Snapshot.
        /// </summary>
        [Conditional("SNAPSHOT")]
        private static void CheckIsSnapshot()
        {
            s_isSnapshot = true;
        }

        /// <summary>
        /// Checks that NSIS is present.
        /// </summary>
        private static bool CheckNsisPresent()
        {
            s_nsisExe = FindMakeNsisExe();
            Console.WriteLine("NSIS : {0}", String.IsNullOrEmpty(s_nsisExe)
                                                ? "Not Found - Installer will not be created."
                                                : s_nsisExe);

            Console.WriteLine();

            Console.WriteLine("Project directory : {0}", GetProjectDirectory());
            Console.WriteLine("Source directory : {0}", GetSourceFilesDirectory());
            if (s_isSnapshot)
                Console.WriteLine("Snapshot directory : {0}", GetSnapshotDirectory());
            else
            {
                Console.WriteLine("Installer directory : {0}", GetInstallerDirectory());
                Console.WriteLine("Binaries directory : {0}", GetBinariesDirectory());
            }
            Console.WriteLine();

            return !String.IsNullOrEmpty(s_nsisExe);
        }

        /// <summary>
        /// Finds the 'makensis' executable.
        /// </summary>
        /// <returns></returns>
        private static string FindMakeNsisExe()
        {
            string path = Path.GetFullPath(@"..\..\..\NSIS\makensis.exe");
            return File.Exists(path) ? path : String.Empty;
        }

        /// <summary>
        /// Gets true if a release version has been compiled.
        /// </summary>
        private static bool HasVersion()
        {
            try
            {
                s_fullVersion = AssemblyName.GetAssemblyName(Path.Combine(GetSourceFilesDirectory(), "EVEMon.exe")).Version;
            }
            catch (Exception)
            {
                Console.WriteLine("A \"Release\" has to be compiled first.");
                Console.WriteLine("Install Builder will now close.");
                Console.ReadLine();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Builds the zip.
        /// </summary>
        private static int BuildZip()
        {
            string directory = s_isSnapshot ? GetSnapshotDirectory() : GetBinariesDirectory();

            // Delete any existing files in directory
            DeleteFiles(directory);

            string filename = s_isSnapshot
                                  ? String.Format(CultureInfo.InvariantCulture, "EVEMon_{0}_{1:yyyy-MM-dd}.zip",
                                                  s_fullVersion.Revision, DateTime.Now)
                                  : String.Format(CultureInfo.InvariantCulture, "EVEMon-binaries-{0}.zip", s_version);

            string zipFileName = Path.Combine(directory, filename);

            string[] filenames = Directory.GetFiles(GetSourceFilesDirectory(), "*", SearchOption.AllDirectories);

            Stream stream = null;
            try
            {
                stream = File.Create(zipFileName);

                using (ZipOutputStream zipStream = new ZipOutputStream(stream))
                {
                    stream = null;
                    zipStream.SetLevel(9);
                    zipStream.UseZip64 = UseZip64.Off;

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames.Where(file => !file.Contains("vshost") && !file.Contains(".config")))
                    {
                        string entryName = String.Format(CultureInfo.InvariantCulture, "EVEMon\\{0}",
                                                         file.Remove(0, GetSourceFilesDirectory().Length));
                        Console.WriteLine("Zipping {0}", entryName);
                        ZipEntry entry = new ZipEntry(entryName) { DateTime = DateTime.Now };

                        zipStream.PutNextEntry(entry);

                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                zipStream.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    zipStream.Finish();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            return 0;
        }

        /// <summary>
        /// Builds the installer.
        /// </summary>
        private static int BuildInstaller()
        {
            // Delete any existing files in directory
            DeleteFiles(GetInstallerDirectory());

            int exitCode;
            try
            {
                string nsisScript = Path.Combine(GetProjectDirectory(), GetOutputPath(), "EVEMonInstallerScript.nsi");
                string resourcesDir = Path.Combine(GetSolutionDirectory(), @"EVEMon.Common\Resources");
                Assembly assembly = Assembly.GetExecutingAssembly();
                string appCopyright =
                    ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)))
                        .Copyright;
                string appDescription =
                    ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)))
                        .Description;

                string productName = String.Format(CultureInfo.InvariantCulture, "/DPRODUCTNAME=\"{0}\"", Application.ProductName);
                string companyName = String.Format(CultureInfo.InvariantCulture, "/DCOMPANYNAME=\"{0}\"", Application.CompanyName);
                string copyright = String.Format(CultureInfo.InvariantCulture, "/DCOPYRIGHT=\"{0}\"", appCopyright);
                string description = String.Format(CultureInfo.InvariantCulture, "/DDESCRIPTION=\"{0}\"", appDescription);
                string version = String.Format(CultureInfo.InvariantCulture, "/DVERSION={0}", s_version);
                string fullVersion = String.Format(CultureInfo.InvariantCulture, "/DFULLVERSION={0}", s_fullVersion);
                string installerDir = String.Format(CultureInfo.InvariantCulture, "/DOUTDIR={0}", GetInstallerDirectory());
                string sourceDir = String.Format(CultureInfo.InvariantCulture, "/DSOURCEDIR={0}", GetSourceFilesDirectory());
                string resourceDir = String.Format(CultureInfo.InvariantCulture, "/DRESOURCESDIR={0}", resourcesDir);

                string param = String.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                    productName, companyName, copyright, description, version, fullVersion, installerDir, sourceDir, resourceDir, nsisScript);

                Console.WriteLine("NSIS script : {0}", nsisScript);
                Console.WriteLine("Output directory : {0}", GetInstallerDirectory());

                ProcessStartInfo psi = new ProcessStartInfo(s_nsisExe, param)
                                       {
                                           WorkingDirectory = GetProjectDirectory(),
                                           UseShellExecute = false,
                                           RedirectStandardOutput = true
                                       };

                using (Process makensisProcess = new Process())
                {
                    makensisProcess.StartInfo = psi;
                    makensisProcess.Start();
                    makensisProcess.ProcessorAffinity = (IntPtr)0x3;
                    Console.WriteLine(makensisProcess.StandardOutput.ReadToEnd());
                    makensisProcess.WaitForExit();
                    exitCode = makensisProcess.ExitCode;
                }

                if (exitCode != 0)
                    Console.WriteLine("MakeNSIS exited with errors.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

            return exitCode;
        }

        /// <summary>
        /// Deletes the files in the specified directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        private static void DeleteFiles(string directoryPath)
        {
            Console.WriteLine("Deleting all files in {0}", directoryPath);

            foreach (string file in Directory.GetFiles(directoryPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}