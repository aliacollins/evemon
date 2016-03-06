using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.Models;

namespace EVEMon.DetailsWindow
{
    public partial class KillReportWindow : EVEMonForm
    {
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
        /// Constructor used in WindowsFactory
        /// </summary>
        /// <param name="killLog">The kill log.</param>
        public KillReportWindow(KillLog killLog)
            : this()
        {
            RememberPositionKey = "KillReportWindow";

            killReportVictim.KillLog = killLog;
            killReportInvolvedParties.KillLog = killLog;
            killReportFittingContent.KillLog = killLog;
        }
    }
}
