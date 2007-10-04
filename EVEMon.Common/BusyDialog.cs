using System;
using System.Threading;
using System.Windows.Forms;

namespace EVEMon.Common
{
    public partial class BusyDialog : EVEMonForm
    {


        private static object m_lockObj = new object();

        //for multiple thread entries, turn this back into an int.
        private static bool _onDisplay = false;
        private static Thread m_runThread;
        private static BusyDialog m_instance;

        public BusyDialog()
        {
            InitializeComponent();
        }

        /*internal void Complete()
        {
            this.Invoke(new MethodInvoker(this.Close));
        }

        public static void BeginProgress()
        {
            lock (m_lockObj)
            {
                if (!_onDisplay)
                {
                    m_runThread = new Thread(new ThreadStart(ShowProgressDialog));                        
                    m_runThread.Start();
                }
                _onDisplay = true;
            }
        }

        private static void ShowProgressDialog()
        {
            using (BusyDialog d = new BusyDialog())
            {
                m_instance = d;
                d.ShowDialog();
            }
        }

        public static void EndProgress()
        {
            lock (m_lockObj)
            {
                _onDisplay = false;
                
                    if (m_instance != null)
                    {
                        m_instance.Invoke(new MethodInvoker(m_instance.Close));
                    }
                    if (m_runThread != null)
                    {
                        m_runThread.Join();
                        m_runThread = null;
                    }                
            }
        }

        public static IDisposable GetScope()
        {
            return new BusyDialogController();
        }

        public class BusyDialogController : IDisposable
        {
            public BusyDialogController()
            {
                BeginProgress();
            }

            #region IDisposable Members
            public void Dispose()
            {
                EndProgress();
            }
            #endregion
        }*/
    }
}