//#define DEBUG_SINGLETHREAD
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.NetworkLogger;
using EVEMon.SkillPlanner;
using EVEMon.WindowRelocator;

namespace EVEMon
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
#if !DEBUG
            InstanceManager im = InstanceManager.GetInstance();
            if (!im.CreatedNew)
            {
                im.Signal();
                return;
            }
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            bool startMinimized = false;

            foreach (string targ in Environment.GetCommandLineArgs())
            {
                if (targ == "-netlog")
                {
                    StartNetlog();
                    if (m_logger == null)
                    {
                        return;
                    }
                }

                if (targ == "-startMinimized")
                {
                    startMinimized = true;
                }
            }

            Plan.PlannerWindowFactory = new PlannerWindowFactory();
            EveSession.MainThread = Thread.CurrentThread;
#if DEBUG_SINGLETHREAD
#else
            InstallerDeleter.Schedule();
#endif

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.Run(new MainWindow(Settings.GetInstance(), startMinimized));
            Settings.GetInstance().SaveImmediate();

            SetRelocatorState(false);
            if (m_logger != null)
            {
                m_logger.Dispose();
            }

            G15Handler.Shutdown();
        }

        private static MainWindow m_mainWindow;

        public static MainWindow MainWindow
        {
            get { return m_mainWindow; }
            set { m_mainWindow = value; }
        }

        private static IDisposable m_logger;

        private static void StartNetlog()
        {
            m_logger = Logger.StartLogging();
        }

        private static bool m_relocatorRunning = false;

        public static void SetRelocatorState(bool state)
        {
            if (!state && !m_relocatorRunning)
            {
                return;
            }
            InternalSetRelocatorState(state);
        }

        private static void InternalSetRelocatorState(bool state)
        {
            if (state)
            {
                m_relocatorRunning = true;
                Relocator.Start(Settings.RelocateTargetScreen);
            }
            else
            {
                m_relocatorRunning = false;
                Relocator.Stop();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Debugger.IsAttached)
            {
                ShowError(e.ExceptionObject as Exception);
                Environment.Exit(1);
            }
        }

        public static Settings Settings
        {
            get { return Settings.GetInstance(); }
        }

        private static bool m_showWindowOnError = true;

        private static void ShowError(Exception e)
        {
            if (m_showWindowOnError)
            {
                m_showWindowOnError = false;
                using (UnhandledExceptionWindow f = new UnhandledExceptionWindow(e))
                {
                    f.ShowDialog();
                }
            }
        }
    }
}
