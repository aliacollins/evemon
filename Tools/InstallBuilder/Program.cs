using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace EVEMon.InstallBuilder
{
    internal static class Program
    {
        private static readonly string s_sourceFilesDir = Path.GetFullPath(@"..\..\..\..\..\EVEMon\bin\x86\Release");
        private static readonly string s_installerDir = Path.GetFullPath(@"..\..\..\..\..\EVEMon\bin\x86\Installbuilder\Installer");
        private static readonly string s_snapshotDir = Path.GetFullPath(@"..\..\..\..\..\EVEMon\bin\x86\Installbuilder\Snapshot");
        private static readonly string s_binariesDir = Path.GetFullPath(@"..\..\..\..\..\EVEMon\bin\x86\Installbuilder\Binaries");
        private static readonly string s_programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        private static readonly string s_programFilesX86Dir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        private static string s_projectDir;
        private static Version s_version;
        private static string s_nsisExe;
        
        private static bool s_isSnapshot;
        private static bool s_isDebug;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        [STAThread]
        private static int Main(string[] args)
        {
            CheckIsDebug();
            CheckIsSnapshot();

            if (!HasVersion())
                return 1;

            if (args.Any())
            {
                string version = String.Format("{0}.{1}.{2}", s_version.Major, s_version.Minor, s_version.Build);

                if (args[0] == "-version" || args[0] == "-v")
                {
                    Console.WriteLine(version);
                    return 0;
                }

                if (args[0] == "-version=tc" || args[0] == "-v=tc")
                {
                    Console.WriteLine("##teamcity[buildNumber '{0}']", version);
                    return 0;
                }
            }

            try
            {
                if (CheckNsisPresent(args) && !s_isSnapshot)
                {
                    // Create the appropriate folder if it doesn't exist
                    if (!Directory.Exists(s_installerDir))
                        Directory.CreateDirectory(s_installerDir);

                    // Create an installer in the appropriate folder
                    Console.WriteLine("Starting Installer creation.");
                    if (BuildInstaller() != 0)
                        return 1;
                    Console.WriteLine("Installer creation finished.");
                    Console.WriteLine();
                }

                // Create the appropriate folder if it doesn't exist
                string directory = s_isSnapshot ? s_snapshotDir : s_binariesDir;
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
        /// Checks the configuration is Debug.
        /// </summary>
        [Conditional("DEBUG")]
        private static void CheckIsDebug()
        {
            s_isDebug = true;
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
        /// <param name="args">The args.</param>
        private static bool CheckNsisPresent(string[] args)
        {
            s_nsisExe = FindMakeNsisExe();
            Console.WriteLine("NSIS : {0}", String.IsNullOrEmpty(s_nsisExe)
                                                ? "Not Found - Installer will not be created."
                                                : s_nsisExe);

            Console.WriteLine();

            s_projectDir = args.Length == 0 ? Path.GetFullPath(@"..\..\..") : String.Join(" ", args);
            Console.WriteLine("Project directory : {0}", s_projectDir);
            Console.WriteLine("Source directory : {0}", s_sourceFilesDir);
            if (s_isSnapshot)
                Console.WriteLine("Snapshot directory : {0}", s_snapshotDir);
            else
            {
                Console.WriteLine("Installer directory : {0}", s_installerDir);
                Console.WriteLine("Binaries directory : {0}", s_binariesDir);
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
                s_version = AssemblyName.GetAssemblyName(@"..\..\..\..\..\EVEMon\bin\x86\Release\EVEMon.exe").Version;
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
            string directory = s_isSnapshot ? s_snapshotDir : s_binariesDir;

            // Delete any existing files in directory
            DeleteFiles(directory);

            string filename = s_isSnapshot
                                  ? String.Format(CultureInfo.InvariantCulture, "EVEMon_{0}_{1:yyyy-MM-dd}.zip",
                                                  s_version.Revision, DateTime.Now)
                                  : String.Format(CultureInfo.InvariantCulture, "EVEMon-binaries-{0}.zip", s_version);

            string zipFileName = Path.Combine(directory, filename);

            string[] filenames = Directory.GetFiles(s_sourceFilesDir, "*", SearchOption.AllDirectories);

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
                        string entryName = String.Format(CultureInfo.InvariantCulture, "EVEMon{0}",
                                                         file.Remove(0, s_sourceFilesDir.Length));
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
            DeleteFiles(s_installerDir);

            int exitCode;
            try
            {
                string nsisScript = Path.Combine(s_projectDir, String.Format(@"bin\x86\{0}\EVEMonInstallerScript.nsi", 
                                                                             s_isDebug? "Debug": "Release"));

                string param = String.Format(CultureInfo.InvariantCulture, "/DVERSION={0} \"/DOUTDIR={1}\" \"{2}\"",
                                             s_version, s_installerDir, nsisScript);

                Console.WriteLine("NSIS script : {0}", nsisScript);
                Console.WriteLine("Output directory : {0}", s_installerDir);

                ProcessStartInfo psi = new ProcessStartInfo(s_nsisExe, param)
                                           {
                                               WorkingDirectory = s_projectDir,
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