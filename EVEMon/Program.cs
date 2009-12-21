//#define DEBUG_SINGLETHREAD
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EVEMon.Common;
using Windows7.DesktopIntegration;

namespace EVEMon
{
    internal static class Program
    {
        private static bool s_exitRequested;
        private static bool s_showWindowOnError = true;
        private static MainWindow s_mainWindow;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Quits if another instance already exists
            if (!EnsureInstanceUnicity()) return;

            // Subscribe application's events (especially the unhandled exceptions management for the crash box)
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ApplicationExit += new EventHandler(ApplicationExitCallback);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Creates a trace file
            EveClient.InitializeFileSystemPaths();
            EveClient.StartTraceLogging();
            EveClient.Trace("Starting up");

            // Make our windows nice
            MakeWindowsJuicy();

            // Check arguments
            bool startMinimized = Environment.GetCommandLineArgs().Contains("-startMinimized");

            // Initialization
            EveClient.Initialize();
            Settings.InitializeFromFile();

            // Did something requested an exit before we entered Run() ?
            if (s_exitRequested) return;

            // Fires the main window
            try
            {
                EveClient.Trace("Main loop - start"); 
                Application.Run(new MainWindow(startMinimized));
                EveClient.Trace("Main loop - done");
            }
            // Save before we quit.
            finally
            {
                Settings.SaveImmediate();
                EveClient.Trace("Closed");
                EveClient.StopTraceLogging();
            }
        }

        #region Properties
        /// <summary>
        /// The main window of the application
        /// </summary>
        public static MainWindow MainWindow
        {
            get { return s_mainWindow; }
            set { s_mainWindow = value; }
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
                Windows7Taskbar.SetCurrentProcessAppId("EVEMon");
                SetDebugAppID();
            }
            catch (Exception ex)
            {
                // On some systems, a crash may occur here because of some skinning programs or others.
                ExceptionHandler.LogException(ex, true);
            }
        }

        /// <summary>
        /// Sets the process AppId to EVEMon-DEBUG
        /// </summary>
        [Conditional("DEBUG")]
        private static void SetDebugAppID()
        {
            Windows7Taskbar.SetCurrentProcessAppId("EVEMon-DEBUG");
        }

        /// <summary>
        /// Ensures that only one instance of EVEMon is ran at once
        /// </summary>
        private static bool EnsureInstanceUnicity()
        {
#if !DEBUG
            InstanceManager im = InstanceManager.GetInstance();
            if (!im.CreatedNew)
            {
                im.Signal();
                return false;
            }
#endif        
            return true;
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
        /// Occurs when an exception reach the entry point of the application. We then display our custom crash box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// Handles exceptions in 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception as Exception);
        }

        /// <summary>
        /// Handles an exception through the Unhandled Exception window
        /// </summary>
        /// <param name="ex">Exception to display</param>
        private static void HandleUnhandledException(Exception ex)
        {
            if (!Debugger.IsAttached)
            {
                if (s_showWindowOnError)
                {
                    s_showWindowOnError = false;

                    // Shutdown EveClient timer incase that was causing the crash
                    // so we don't get multiple crashes
                    EveClient.Shutdown();
                    using (UnhandledExceptionWindow f = new UnhandledExceptionWindow(ex))
                    {
                        f.ShowDialog(s_mainWindow);
                    }
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// At the beginning of the program, we may check (according to the options) the local clock against Battleclinic's clock.
        /// </summary>
        /// <param name="isSynchronised"></param>
        /// <param name="serverTime"></param>
        /// <param name="localTime"></param>
        private static void TimeCheckCallback(bool? isSynchronised, DateTime serverTime, DateTime localTime)
        {
            if (isSynchronised == false)
            {
                using (TimeCheckNotification timeDialog = new TimeCheckNotification(serverTime, localTime))
                {
                    timeDialog.ShowDialog();
                }
            }
        }
        #endregion
    }
}
