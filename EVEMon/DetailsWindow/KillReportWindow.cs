using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.DetailsWindow
{
    public partial class KillReportWindow : EVEMonForm
    {
        private readonly KillLog m_killLog;

        /// <summary>
        /// Prevents a default instance of the <see cref="KillReportWindow"/> class from being created.
        /// </summary>
        private KillReportWindow()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KillReportWindow"/> class.
        /// </summary>
        /// <param name="killLog">The kill log.</param>
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
