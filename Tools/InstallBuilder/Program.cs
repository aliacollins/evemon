using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace InstallBuilder
{
    internal static class Program
    {
        private static readonly string s_sourceFilesDir = Path.GetFullPath(@"..\..\..\..\..\EVEMon\bin\x86\Release");
        private static readonly string s_installerDir = Path.GetFullPath(@"..\..\..\..\..\EVEMon\bin\x86\Installbuilder\Installer");
        private static readonly string s_binariesDir = Path.GetFullPath(@"..\..\..\..\..\EVEMon\bin\x86\Installbuilder\Binaries");
        private static readonly string s_programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        private static string s_projectDir;
        private static string s_version;
        private static string s_nsisExe;

        private static bool s_isDebug;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        [STAThread]
        public static int Main(string[] args)
        {
            CheckIsDebug();

            if (!HasVersion())
                return 0;

            if (args.Any())
            {
                if (args[0] == "-version" || args[0] == "-v")
                {
                    Console.WriteLine(s_version.Substring(0, s_version.LastIndexOf('.')));
                    return 0;
                }

                if (args[0] == "-version=tc" || args[0] == "-v=tc")
                {
                    Console.WriteLine("##teamcity[buildNumber '{0}']", s_version.Substring(0, s_version.LastIndexOf('.')));
                    return 0;
                }
            }

            CheckNsisInstalled(args);

            // Create the installer folder if it doesn't exist
            if (!Directory.Exists(s_installerDir))
                Directory.CreateDirectory(s_installerDir);

            // Create the binaries folder if it doesn't exist
            if (!Directory.Exists(s_binariesDir))
                Directory.CreateDirectory(s_binariesDir);

            try
            {
                if (!String.IsNullOrEmpty(s_nsisExe))
                {
                    // Create an installer in the installers folder
                    Console.WriteLine("Starting Installer creation.");
                    BuildInstaller();
                    Console.WriteLine("Installer creation finished.");
                    Console.WriteLine();
                }

                // Create a zip file in the binaries folder
                Console.WriteLine("Starting zip installer creation.");
                BuildZip();
                Console.WriteLine("Zip installer creation finished.");
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
        /// Checks the debug.
        /// </summary>
        [Conditional("DEBUG")]
        private static void CheckIsDebug()
        {
            s_isDebug = true;
        }

        /// <summary>
        /// Finds the 'makensis' executable.
        /// </summary>
        /// <returns></returns>
        private static string FindMakeNsisExe()
        {
            string[] locations = new string[3];

            locations[0] = String.Format(CultureInfo.InvariantCulture, "{0}\\NSIS\\makensis.exe", s_programFilesDir);
            locations[1] = @"D:\Program Files\NSIS\makensis.exe"; // Possible location in TeamCity server
            locations[2] = @"D:\Program Files (x86)\NSIS\makensis.exe"; // Possible location in TeamCity server

            foreach (string path in locations.Where(File.Exists))
            {
                return path;
            }

            return String.Empty;
        }

        /// <summary>
        /// Checks that NSIS is installed.
        /// </summary>
        /// <param name="args">The args.</param>
        private static void CheckNsisInstalled(string[] args)
        {
            s_nsisExe = FindMakeNsisExe();
            Console.WriteLine("NSIS : {0}", String.IsNullOrEmpty(s_nsisExe)
                                                ? "Not Found - Installer will not be created."
                                                : s_nsisExe);

            Console.WriteLine();

            s_projectDir = args.Length == 0 ? Path.GetFullPath(@"..\..\..") : String.Join(" ", args);
            Console.WriteLine("Project directory : {0}", s_projectDir);
            Console.WriteLine("Source directory : {0}", s_sourceFilesDir);
            Console.WriteLine("Installer directory : {0}", s_installerDir);
            Console.WriteLine("Binaries directory : {0}", s_binariesDir);
            Console.WriteLine();
        }

        /// <summary>
        /// Gets true if a release version has been compiled.
        /// </summary>
        private static bool HasVersion()
        {
            try
            {
                s_version = AssemblyName.GetAssemblyName(@"..\..\..\..\..\EVEMon\bin\x86\Release\EVEMon.exe").Version.ToString();
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
        private static void BuildZip()
        {            
            // Delete any existing binaries files
            DeleteFiles(s_binariesDir);

            string zipFileName = Path.Combine(s_binariesDir,
                                              String.Format(CultureInfo.InvariantCulture, "EVEMon-binaries-{0}.zip", s_version));

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
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// Builds the installer.
        /// </summary>
        private static void BuildInstaller()
        {
            // Delete any existing installer files
            DeleteFiles(s_installerDir);

            try
            {
                string nsisScript = Path.Combine(s_projectDir,
                                                 s_isDebug
                                                     ? "bin\\x86\\Debug\\EVEMonInstallerScript.nsi"
                                                     : "bin\\x86\\Release\\EVEMonInstallerScript.nsi");

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

                int exitCode;
                using (Process makensisProcess = new Process())
                {
                    makensisProcess.StartInfo = psi;
                    makensisProcess.Start();
                    makensisProcess.ProcessorAffinity = (IntPtr)0x3;
                    Console.WriteLine(makensisProcess.StandardOutput.ReadToEnd());
                    makensisProcess.WaitForExit();
                    exitCode = makensisProcess.ExitCode;
                }

                if (exitCode == 1)
                    MessageBox.Show("MakeNSIS exited with Errors");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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