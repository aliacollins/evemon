using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Serialization.BattleClinic;
using EVEMon.Common.Threading;
using EVEMon.ExceptionHandling;
using EVEMon.LogitechG15;
using EVEMon.WindowsApi;

namespace EVEMon
{
    internal static class Program
    {
        /// <summary>
        /// The main window of the application.
        /// </summary>
        private static Form s_mainWindow;

        private static bool s_exitRequested;
        private static bool s_showNotWindowOnError;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Check if we are in DEBUG mode 
            EveMonClient.CheckIsDebug();

            // Quits non-debug builds if another instance already exists
            if (!EveMonClient.IsDebugBuild && !IsInstanceUnique)
                return;

            // Subscribe application's events (especially the unhandled exceptions management for the crash box)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Application.ApplicationExit += ApplicationExitCallback;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Find our files
            EveMonClient.InitializeFileSystemPaths();

            // Creates a trace file
            EveMonClient.StartTraceLogging();
            EveMonClient.Trace("Starting up");

            // Make our windows nice
            MakeWindowsJuicy();

            // Check arguments
            bool startMinimized = Environment.GetCommandLineArgs().Contains("-startMinimized");

            // Upgrades the BattleClinic API settings
            BCAPI.UpgradeSettings();

            // Initialization
            EveMonClient.Initialize();
            Settings.Initialize();
            EveIDToName.InitializeFromFile();

            // Initialize G15
            if (OSFeatureCheck.IsWindowsNT)
                G15Handler.Initialize();

            // Did something requested an exit before we entered Run() ?
            if (s_exitRequested)
                return;

            try
            {
                // Fires the main window
                EveMonClient.Trace("Main loop - start");
                s_mainWindow = new MainWindow(startMinimized);
                Application.Run(s_mainWindow);
                EveMonClient.Trace("Main loop - done");
            }
            finally
            {
                // Save before we quit
                Settings.SaveImmediate();
                EveIDToName.SaveImmediate();
                BCAPI.UploadSettingsFile();

                // Stop the one-second timer right now
                EveMonClient.Shutdown();
                EveMonClient.Trace("Closed");
                EveMonClient.StopTraceLogging();
            }
        }


        #region Properties

        /// <summary>
        /// Ensures that only one instance of EVEMon is run at once.
        /// </summary>
        private static bool IsInstanceUnique
        {
            get
            {
                InstanceManager im = InstanceManager.Instance;
                if (!im.CreatedNew)
                {
                    im.Signal();
                    return false;
                }

                return true;
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Makes the windows nice.
        /// </summary>
        private static void MakeWindowsJuicy()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                string appId = "EVEMon";
                if (EveMonClient.IsDebugBuild)
                    appId = String.Format(CultureConstants.InvariantCulture, "{0}-DEBUG", appId);

                Windows7.SetProcessAppId(appId);
            }
            catch (InvalidOperationException ex)
            {
                // On some systems, a crash may occur here because of some skinning programs or others
                ExceptionHandler.LogException(ex, false);
            }
        }

        #endregion


        #region Callbacks

        /// <summary>
        /// If <see cref="Application.Exit()"/> is called before the <see cref="Application.Run()"/> method, then it won't occur. 
        /// So, here, we set up a boolean to prevent that.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ApplicationExitCallback(object sender, EventArgs e)
        {
            s_exitRequested = true;
        }

        /// <summary>
        /// Occurs when an exception reach the entry point of the
        /// application. We then display our custom crash box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// Handles exceptions in WinForms threads, such exceptions
        /// would never reach the entry point of the application, 
        /// generally causing a CTD or trigger WER.
        /// We display our custom crash box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        /// <summary>
        /// Handles an exception through the Unhandled Exception window.
        /// </summary>
        /// <param name="ex">Exception to display</param>
        private static void HandleUnhandledException(Exception ex)
        {
            if (Debugger.IsAttached)
                return;

            if (s_showNotWindowOnError)
                return;

            s_showNotWindowOnError = true;

            try
            {
                // Shutdown EveMonClient timer in case that was causing the crash
                // so we don't get multiple crashes
                EveMonClient.Shutdown();

                // Some exceptions may be thrown on a worker thread so we need to invoke them to the UI thread,
                // if we are already on the UI thread nothing changes
                Dispatcher.Invoke(() =>
                                      {
                                          using (UnhandledExceptionWindow form = new UnhandledExceptionWindow(ex))
                                          {
                                              form.ShowDialog(s_mainWindow);
                                          }
                                      });
            }
            catch
            {
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("An error occurred and EVEMon was unable to handle the error message gracefully");
                messageBuilder.AppendLine();
                messageBuilder.AppendFormat(CultureConstants.DefaultCulture, "The exception encountered was '{0}'.", ex.Message);
                messageBuilder.AppendLine();
                messageBuilder.AppendLine();
                messageBuilder.AppendLine("Please report this on the EVEMon forums.");
                MessageBox.Show(messageBuilder.ToString(), "EVEMon Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            Environment.Exit(1);
        }

        #endregion
    }
}