using System;
using System.IO;
using System.Threading;
using EVEMon.Common;

namespace EVEMon
{
    public static class InstallerDeleter
    {
        private static Timer m_timer;

        public static void Schedule()
        {
            if (m_timer == null)
            {
                m_timer = new Timer(new TimerCallback(DoIt));
                m_timer.Change(10000, Timeout.Infinite);
            }
        }

        private static void DoIt(object state)
        {
            m_timer = null;
            foreach (string s in Directory.GetFiles(Environment.CurrentDirectory, "EVEMon-install-*.exe", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(s);
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }
        }
    }
}