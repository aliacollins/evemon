using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.NotificationWindow
{
    public partial class KillReportWindow : EVEMonForm
    {
        private readonly KillLog m_killLog;

        private KillReportWindow()
        {
            InitializeComponent();
        }

        public KillReportWindow(KillLog killLog)
            : this()
        {
            RememberPositionKey = "KillReportWindow";

            m_killLog = killLog;
            killReportVictim.KillLog = m_killLog;
            killReportInvolvedParties.KillLog = m_killLog;
            killReportFittingContent.KillLog = m_killLog;
        }
    }
}
