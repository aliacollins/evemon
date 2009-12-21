using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace InstallBuilder
{
    public class Program
    {
        private static string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static string programDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        private static string config;
        private static string projectDir;
        private static string ver;
        private static string binariesDir;
        private static string nsisExe;

        public static int Main(string[] args)
        {
            CheckDebug();
            
            try
            {
                PopulateEnvironment(args);

                if (!String.IsNullOrEmpty(nsisExe))
                {
                    // create an installer on the developers desktop
                    Console.WriteLine("Starting Installer creation.");
                    BuildInstaller();
                    Console.WriteLine("Installer creation finished.");
                }

                // create a zip file on the developers desktop
                Console.WriteLine("Starting zip installer creation.");
                BuildZip();
                Console.WriteLine("Zip installer creation finished.");
                Console.WriteLine("Done");
                if (Debugger.IsAttached) Console.ReadLine();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured: {0} in {1}", ex.Message, ex.Source);
                Console.WriteLine();
                Console.WriteLine(ex.StackTrace);
                if (Debugger.IsAttached) Console.ReadLine();
                return 1;
            }
        }

        [Conditional("DEBUG")]
        private static void CheckDebug()
        {
            Application.Exit();
        }

        private static string findMakeNsisExe()
        {
            string[] locations = new string[3];

            locations[0] = programDir + "/NSIS/makensis.exe";
            locations[1] = "C:/Program Files/NSIS/makensis.exe";
            locations[2] = "C:/Program Files (x86)/NSIS/makensis.exe";

            foreach (string s in locations)
            {
                if (File.Exists(s)) return s;
            }

            return String.Empty;
        }

        private static void PopulateEnvironment(string[] args)
        {
            config = "Release";
            if (args.Length == 0) projectDir = Path.GetFullPath("../../..");
            else projectDir = String.Join(" ", args);

            Console.WriteLine("Project directory : " + projectDir);

            Assembly exeAsm = Assembly.LoadFrom("../../../../../EVEMon/bin/x86/" + config + "/EVEMon.exe");
            ver = exeAsm.GetName().Version.ToString();

            binariesDir = Path.Combine(projectDir, "../../EVEMon/bin/x86/" + config);
            Console.WriteLine("Binaries directory : " + binariesDir);

            nsisExe = findMakeNsisExe();
            Console.WriteLine("NSIS : " + nsisExe);
        }

        private static void BuildZip()
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            string svnRevision = ver.Substring(ver.LastIndexOf('.') + 1, ver.Length - (ver.LastIndexOf('.') + 1));
            string zipFileName = String.Format("EVEMon-binaries-{0}.zip", ver);
            zipFileName = Path.Combine(desktopDir, zipFileName);

            string[] filenames = Directory.GetFiles(binariesDir, "*", SearchOption.AllDirectories);

            FileInfo fi = new FileInfo(zipFileName);
            if (fi.Exists)
            {
                fi.Delete();
            }

            using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipFileName)))
            {
                zipStream.SetLevel(9);
                zipStream.UseZip64 = UseZip64.Off;

                byte[] buffer = new byte[4096];

                foreach (string file in filenames)
                {
                    string entryName = "EVEMon" + file.Remove(0, binariesDir.Length);
                    Console.WriteLine("Zipping {0}", entryName);
                    ZipEntry entry = new ZipEntry(entryName);

                    entry.DateTime = DateTime.Now;
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
                zipStream.Close();
            }
        }

        private static void BuildInstaller()
        {
            try
            {
                ProcessInstallScripts(projectDir);

#if DEBUG
                string nsisScript = Path.Combine(projectDir, "bin\\x86\\Debug\\EVEMon Installer Script.nsi");
#else
                string nsisScript = Path.Combine(projectDir, "bin\\x86\\Release\\EVEMon Installer Script.nsi");
#endif

                string param =
                    "/DVERSION=" + ver + " " +
                    "\"/DOUTDIR=" + desktopDir + "\" " +
                    "\"" + nsisScript + "\"";

                Console.WriteLine("NSIS script : " + nsisScript);
                Console.WriteLine("Output directory : " + desktopDir);

                ProcessStartInfo psi = new ProcessStartInfo(nsisExe, param);
                psi.WorkingDirectory = projectDir;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                Process makensisProcess = Process.Start(psi);
                Console.WriteLine(makensisProcess.StandardOutput.ReadToEnd());
                makensisProcess.WaitForExit();
                int exitCode = makensisProcess.ExitCode;
                makensisProcess.Dispose();

                if (exitCode == 1)
                {
                    MessageBox.Show("MakeNSIS exited with Errors");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void ProcessInstallScripts(string projectDir)
        {
            List<string> filesToProcess = new List<string>();
            foreach (string s in Directory.GetFiles(projectDir + "\\bin\\x86\\Release", "*.nsi"))
            {
                filesToProcess.Add(s);
            }
            foreach (string s in Directory.GetFiles(projectDir + "\\bin\\x86\\Release", "*.nsh"))
            {
                filesToProcess.Add(s);
            }
            foreach (string s in Directory.GetFiles(projectDir + "\\bin\\x86\\Debug", "*.nsi"))
            {
                filesToProcess.Add(s);
            }
            foreach (string s in Directory.GetFiles(projectDir + "\\bin\\x86\\Debug", "*.nsh"))
            {
                filesToProcess.Add(s);
            }

            foreach (string fn in filesToProcess)
            {
                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    using (StreamReader sr = new StreamReader(fn))
                    {
                        while (!sr.EndOfStream)
                        {
                            string tLine = sr.ReadLine();
                            if (tLine == "## INSTALLBUILDER: INSERT FILES HERE ##")
                                InsertFiles(sw, projectDir, true);
                            else if (tLine == "## INSTALLBUILDER: INSERT DELETES HERE ##")
                                InsertFiles(sw, projectDir, false);
                            else if (tLine.Contains("INSTALLBUILDER"))
                                throw new ApplicationException("unknown installbuilder command: " + tLine);
                            else
                            {
                                sw.WriteLine(tLine);
                            }
                        }
                    }
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    using (StreamReader sr = new StreamReader(ms))
                    using (FileStream fs = new FileStream(fn, FileMode.Create))
                    using (StreamWriter fsw = new StreamWriter(fs))
                    {
                        while (!sr.EndOfStream)
                        {
                            fsw.WriteLine(sr.ReadLine());
                        }
                    }
                }
            }
        }

        private static void InsertFiles(StreamWriter sw, string projectDir, bool adding)
        {
            foreach (string fn in Directory.GetFiles(projectDir + "\\bin\\x86\\Release", "*.*"))
            {
                if (adding)
                {
                    sw.Write(" File \"");
                    sw.Write(fn);
                    sw.WriteLine("\"");
                }
                else
                {
                    sw.Write(" Delete \"$INSTDIR\\");
                    sw.Write(Path.GetFileName(fn));
                    sw.WriteLine("\"");
                }
            }
        }
    }
}
