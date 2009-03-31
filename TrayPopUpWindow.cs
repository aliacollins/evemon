using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using EVEMon.Common;
using System.Text.RegularExpressions;
using System.Collections;

namespace EVEMon
{
    /// <summary>
    /// Popup form displayed when the user hovers over the tray icon
    /// </summary>
    /// <remarks>
    /// Display contents are governed by Settings.TrayPopupConfig<br/>
    /// Popup location is determined using mouse location, screen and screen bounds (see SetPosition()).<br/>
    /// </remarks>
    public partial class TrayPopUpWindow : Form
    {
        #region Fields
        private CharacterCollection m_characters;
        private Label lblTQStatus;
        private Label lblEveTime;
        private Settings m_settings;
        private TrayPopupConfig m_config;
        private int[] m_portraitSize = { 16, 24, 32, 40, 48, 56, 64 };
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public TrayPopUpWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Main constructor 
        /// </summary>
        /// <param name="characters">The list of characters to display in the popup.</param>
        public TrayPopUpWindow(List<CharacterMonitor> characters)
        {
            // Initialisation
            m_characters = new CharacterCollection();
            if (characters != null) m_characters.AddRange(characters);
            m_settings = Settings.GetInstance();
            m_config = m_settings.TrayPopupConfig;
            InitializeComponent();
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Adds the character panes to the form, gets the TQ status message and sets the popup position
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Get settings
            // Form level look and feel
            this.Font = new Font(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
            // Set background color
            mainPanel.BackColor = SystemColors.ControlLightLight;
            // Character Details
            if (m_config.HideCharNotTraining)
            {
                AddCharacters(m_characters.CharactersTraining, m_config.SortOrder1);
            }
            else
            {
                switch (m_config.GroupBy)
                {
                    case TrayPopupConfig.CharacterGroupings.None:
                        AddCharacters(m_characters, m_config.SortOrder1);
                        break;
                    case TrayPopupConfig.CharacterGroupings.Account:
                        AddAccounts(new AccountCollection(m_characters));
                        break;
                    case TrayPopupConfig.CharacterGroupings.TrainingAtTop:
                        AddCharacters(m_characters.CharactersTraining, m_config.SortOrder1);
                        AddCharacters(m_characters.CharactersNotTraining, m_config.SortOrder2);
                        break;
                    case TrayPopupConfig.CharacterGroupings.TrainingAtBottom:
                        AddCharacters(m_characters.CharactersNotTraining, m_config.SortOrder2);
                        AddCharacters(m_characters.CharactersTraining, m_config.SortOrder1);
                        break;
                }
            }

            // Not Training warning message
            if (m_config.ShowWarning)
            {
                AccountCollection accounts = new AccountCollection(m_characters);
                foreach (Account account in accounts)
                {
                    if (!account.IsTraining)
                        AddWarning(account);
                }
            }

            // TQ Server Status
            if (m_config.ShowTQStatus)
                AddTQStatus();

            // EVE Time
            if (m_config.ShowEveTime)
                AddEveTime();

            // Fix the panel widths to the largest.
            // We let the framework determine the appropriate widths, then fix them so that
            // updates to training time remaining don't cause the form to resize.
            int pnlWidth = 0;
            foreach (Control control in mainPanel.Controls)
            {
                if (control.Width > pnlWidth)
                {
                    pnlWidth = control.Width;
                }
            }
            foreach (Control control in mainPanel.Controls)
            {
                if (control is FlowLayoutPanel)
                {
                    FlowLayoutPanel flowPanel = control as FlowLayoutPanel;
                    int pnlHeight = flowPanel.Height;
                    flowPanel.AutoSize = false;
                    flowPanel.Width = pnlWidth;
                    flowPanel.Height = pnlHeight;
                }
            }
            // Position Popup
            TrayIcon.SetToolTipLocation(this);
        }

        /// <summary>
        /// Draws the rounded rectangle border and background
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw the border and background
            DrawBorder(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Equivalent to setting TopMost = true, except don't activate the window.
            NativeMethods.SetWindowPos(this.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
                NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            // Show the window without activating it.
            NativeMethods.ShowWindow(this.Handle, NativeMethods.SW_SHOWNOACTIVATE);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // Unsubscribe to server status updates
            EveServer server = EveServer.GetInstance();
            if (server != null)
            {
                server.ServerStatusUpdated -= new EventHandler<EveServerEventArgs>(ServerStatusUpdated);
            }
        }
        #endregion

        #region Event Handler Methods
        /// <summary>
        /// Updates the TQ status message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerStatusUpdated(object sender, EveServerEventArgs e)
        {
            SetTQStatusLabel(e.info);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Adds character details to the popup
        /// </summary>
        /// <param name="characters">A collection of characters to be added</param>
        /// <param name="sortOrder">The sort order by which character panes should be added</param>
        private void AddCharacters(CharacterCollection characters, TrayPopupConfig.SortOrders sortOrder)
        {
            switch (sortOrder)
            {
                case TrayPopupConfig.SortOrders.AlphaFirstAtBottom:
                    characters.Sort(new CharacterCollection.NameComparer(SortOrder.Descending));
                    break;
                case TrayPopupConfig.SortOrders.AlphaFirstAtTop:
                    characters.Sort(new CharacterCollection.NameComparer(SortOrder.Ascending));
                    break;
                case TrayPopupConfig.SortOrders.EarliestAtBottom:
                    characters.Sort(new CharacterCollection.CompletionTimeComparer(SortOrder.Descending));
                    break;
                case TrayPopupConfig.SortOrders.EarliestAtTop:
                    characters.Sort(new CharacterCollection.CompletionTimeComparer(SortOrder.Ascending));
                    break;
            }
            foreach (CharacterMonitor cm in characters)
            {
                mainPanel.Controls.Add(new TrayPopUpChar(cm));
            }
        }

        /// <summary>
        /// Adds account details to the popup
        /// </summary>
        /// <param name="accounts">A collection of account to be added</param>
        private void AddAccounts(AccountCollection accounts)
        {
            if (m_config.SortOrder1 == TrayPopupConfig.SortOrders.EarliestAtBottom)
                accounts.Sort(new AccountCollection.CompletionTimeComparer(SortOrder.Descending));
            else
                accounts.Sort(new AccountCollection.CompletionTimeComparer(SortOrder.Ascending));
            // Now create the account panels
            foreach (Account account in accounts)
            {
                AddCharacters(account.Characters.CharactersNotTraining, TrayPopupConfig.SortOrders.AlphaFirstAtTop);
                AddCharacters(account.Characters.CharactersTraining, TrayPopupConfig.SortOrders.EarliestAtBottom);
            }
        }

        /// <summary>
        /// Adds an 'Account not training' warning message to the popup
        /// </summary>
        /// <param name="account">The account for which to add the warning</param>
        private void AddWarning(Account account)
        {
            // Create a flowlayout to hold the content
            FlowLayoutPanel warningPanel = new FlowLayoutPanel();
            warningPanel.AutoSize = true;
            warningPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            warningPanel.Margin = new Padding(0, 0, 0, 2);
            if (!m_settings.WorksafeMode)
            {
                // Add a warning icon
                PictureBox pbWarning = new PictureBox();
                pbWarning.Image = SystemIcons.Warning.ToBitmap();
                pbWarning.SizeMode = PictureBoxSizeMode.StretchImage;
                pbWarning.Size = new Size(m_portraitSize[(int)m_config.PortraitSize], m_portraitSize[(int)m_config.PortraitSize]);
                pbWarning.Margin = new Padding(2);
                warningPanel.Controls.Add(pbWarning);
            }
            FlowLayoutPanel textPanel = new FlowLayoutPanel();
            textPanel.AutoSize = true;
            textPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            textPanel.FlowDirection = FlowDirection.TopDown;
            textPanel.Margin = new Padding(2);
            // Build an 'Account not training' message
            StringBuilder sb = new StringBuilder(String.Empty);
            String separator = string.Empty;
            foreach (CharacterMonitor cm in account.Characters)
            {
                sb.Append(separator);
                sb.Append(cm.CharacterName);
                separator = ", ";
            }
            Label lblNames = new Label();
            lblNames.AutoSize = true;
            lblNames.Font = new Font(lblNames.Font.Name, SystemFonts.MessageBoxFont.SizeInPoints * 11 / 9, FontStyle.Regular, GraphicsUnit.Point);
            lblNames.Text = sb.ToString();
            textPanel.Controls.Add(lblNames);
            Label lblMessage = new Label();
            lblMessage.AutoSize = true;
            lblMessage.Text = "This account has no characters in training!";
            textPanel.Controls.Add(lblMessage);
            warningPanel.Controls.Add(textPanel);
            mainPanel.Controls.Add(warningPanel);
        }

        /// <summary>
        /// Adds the tranquility status message
        /// </summary>
        private void AddTQStatus()
        {
            lblTQStatus = new Label();
            lblTQStatus.AutoSize = true;
            mainPanel.Controls.Add(lblTQStatus);
            EveServer server = EveServer.GetInstance();
            if (server != null)
            {
                SetTQStatusLabel(server.StatusText);
                server.ServerStatusUpdated += new EventHandler<EveServerEventArgs>(ServerStatusUpdated);
            }
            else
            {
                SetTQStatusLabel("Tranquility Status not available");
            }
        }

        private void AddEveTime()
        {
            lblEveTime = new Label();
            lblEveTime.AutoSize = true;
            mainPanel.Controls.Add(lblEveTime);

            DateTime now = DateTime.Now.ToUniversalTime();
            DateTimeFormatInfo fi = CultureInfo.CurrentCulture.DateTimeFormat;
            SetEveTimeLabel("Current EVE Time: " + now.ToString (fi.ShortDatePattern + " HH:mm"));
        }

        /// <summary>
        /// Displays the TQ status message
        /// </summary>
        /// <param name="statusText"></param>
        private void SetTQStatusLabel(String statusText)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    SetTQStatusLabel(statusText);
                }
                ));
            }
            else
            {
                lblTQStatus.Text = statusText;
            }
        }

        private void SetEveTimeLabel(String timeText)
        {
            if(InvokeRequired)
            {
                Invoke (new MethodInvoker (delegate 
                {
                    SetEveTimeLabel (timeText);
                }
                ));
            }
            else
            {
                lblEveTime.Text = timeText;
            }
        }

        /// <summary>
        /// Draws the rounded rectangle border
        /// </summary>
        /// <param name="e"></param>
        private void DrawBorder(PaintEventArgs e)
        {
            TrayPopupConfig config = Settings.GetInstance().TrayPopupConfig;
            // Create graphics object to work with
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // Define the size of the rectangle used for each of the 4 corner arcs.
            int radius = 4;
            Size cornerSize = new Size(radius * 2, radius * 2);
            // Construct a GraphicsPath for the outline
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            // Top left
            path.AddArc(new Rectangle(0, 0, cornerSize.Width, cornerSize.Height), 180, 90);
            // Top Right
            path.AddArc(new Rectangle(e.ClipRectangle.Width - 1 - cornerSize.Width, 0, cornerSize.Width, cornerSize.Height), 270, 90);
            // Bottom right
            path.AddArc(new Rectangle(e.ClipRectangle.Width - 1 - cornerSize.Width, e.ClipRectangle.Height - 1 - cornerSize.Height, cornerSize.Width, cornerSize.Height), 0, 90);
            // Bottom Left
            path.AddArc(new Rectangle(0, e.ClipRectangle.Height - 1 - cornerSize.Height, cornerSize.Width, cornerSize.Height), 90, 90);
            path.CloseFigure();
            // Draw the background
            Brush fillBrush = new SolidBrush(SystemColors.ControlLightLight);
            g.FillPath(fillBrush, path);
            // Now the border
            Pen borderPen = SystemPens.WindowFrame;
            g.DrawPath(borderPen, path);
        }
        #endregion

        #region Native Stuff
        internal class NativeMethods
        {
            public const Int32 HWND_TOPMOST = -1;
            public const Int32 SWP_NOACTIVATE = 0x0010;
            public const Int32 SWP_NOSIZE = 0x0001;
            public const Int32 SWP_NOMOVE = 0x0002;
            public const Int32 SW_SHOWNOACTIVATE = 4;

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, Int32 flags);
            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd,
                Int32 hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, uint uFlags);

        }
        #endregion

        private enum SortOrder { Ascending, Descending };

        #region Character classes
        /// <summary>
        /// A custom collection of CharacterMonitor.
        /// </summary>
        /// <remarks>
        /// Implements alternative IComparer classes to provide sorting by Completion Time and Name
        /// </remarks>
        private class CharacterCollection : List<CharacterMonitor>
        {
            /// <summary>
            /// Returns a CharacterCollection containing all characters training a skill
            /// </summary>
            public CharacterCollection CharactersTraining
            {
                get
                {
                    CharacterCollection selectedChars = new CharacterCollection();
                    foreach (CharacterMonitor cm in this)
                    {
                        if (cm.GrandCharacterInfo.IsTraining)
                            selectedChars.Add(cm);
                    }
                    return selectedChars;
                }
            }

            /// <summary>
            /// Returns a CharacterCollection containing all characters not training a skill
            /// </summary>
            public CharacterCollection CharactersNotTraining
            {
                get
                {
                    CharacterCollection selectedChars = new CharacterCollection();
                    foreach (CharacterMonitor cm in this)
                    {
                        if (!cm.GrandCharacterInfo.IsTraining)
                            selectedChars.Add(cm);
                    }
                    return selectedChars;
                }
            }

            /// <summary>
            /// Comparer class for sorting by Trainign Completion Time
            /// </summary>
            public class CompletionTimeComparer : IComparer<CharacterMonitor>
            {
                private SortOrder m_sortOrder;

                public CompletionTimeComparer(SortOrder sortOrder)
                {
                    m_sortOrder = sortOrder;
                }

                public int Compare(CharacterMonitor x, CharacterMonitor y)
                {
                    if (m_sortOrder == SortOrder.Descending)
                    {
                        CharacterMonitor temp = x;
                        x = y;
                        y = temp;
                    }

                    Skill skillX = x.GrandCharacterInfo.CurrentlyTrainingSkill;
                    Skill skillY = y.GrandCharacterInfo.CurrentlyTrainingSkill;
                    if (skillX == null && skillY == null)
                    {
                        return x.CharacterName.CompareTo(y.CharacterName);
                    }
                    else if (skillX == null && skillY != null)
                        return -1;
                    else if (skillX != null && skillY == null)
                        return 1;
                    else
                    {
                        if (skillX.EstimatedCompletion < skillY.EstimatedCompletion)
                            return -1;
                        else if (skillY.EstimatedCompletion == skillY.EstimatedCompletion)
                            return 0;
                        else
                            return 1;
                    }
                }
            }

            /// <summary>
            /// Comparer class for sorting by Character Name
            /// </summary>
            public class NameComparer : IComparer<CharacterMonitor>
            {
                private SortOrder m_sortOrder;

                public NameComparer(SortOrder sortOrder)
                {
                    m_sortOrder = sortOrder;
                }

                public int Compare(CharacterMonitor x, CharacterMonitor y)
                {
                    if (m_sortOrder == SortOrder.Ascending)
                        return x.CharacterName.CompareTo(y.CharacterName);
                    else
                        return y.CharacterName.CompareTo(x.CharacterName);
                }
            }
        }

        #endregion

        #region Account classes
        /// <summary>
        /// A custom collection of Account objects
        /// </summary>
        /// <remarks>
        /// Implements an alternative IComparer class for sorting by Training Completion Time
        /// </remarks>
        private class AccountCollection : List<Account>
        {
            public AccountCollection(CharacterCollection characters)
            {
                foreach (CharacterMonitor cm in characters)
                {
                    // Add to accounts list
                    Account account = GetAccount(cm.GrandCharacterInfo.UserId);
                    if (account == null)
                    {
                        account = new Account(cm.GrandCharacterInfo.UserId);
                        this.Add(account);
                    }
                    account.AddCharacter(cm);
                }

            }

            public bool Contains(int userID)
            {
                foreach (Account account in this)
                {
                    if (account.UserID == userID) { return true; }
                }
                return false;
            }

            public Account GetAccount(int userID)
            {
                foreach (Account account in this)
                {
                    if (account.UserID == userID) { return account; }
                }
                return null;
            }

            public class CompletionTimeComparer : IComparer<Account>
            {
                private SortOrder m_sortOrder;

                public CompletionTimeComparer(SortOrder sortOrder)
                {
                    m_sortOrder = sortOrder;
                }

                public int Compare(Account x, Account y)
                {
                    if (m_sortOrder == SortOrder.Descending)
                    {
                        Account temp = x;
                        x = y;
                        y = temp;
                    }
                    if (x.IsTraining && y.IsTraining)
                    {
                        if (x.CompletionTime < y.CompletionTime) { return -1; }
                        else if (x.CompletionTime == y.CompletionTime) { return 0; }
                        else { return 1; }
                    }
                    else if (!x.IsTraining && y.IsTraining) { return -1; }
                    else if (!x.IsTraining && !y.IsTraining) { return 0; }
                    else { return 1; }
                }
            }
        }

        /// <summary>
        /// A simple helper class to group characters by account and provide account level training info
        /// </summary>
        private class Account
        {
            private CharacterCollection m_characters;
            private int m_userID;

            public CharacterCollection Characters
            {
                get { return m_characters; }
            }

            public int UserID
            {
                get { return m_userID; }
            }

            public bool IsTraining
            {
                get
                {
                    bool result = false;
                    foreach (CharacterMonitor cm in m_characters)
                    {
                        if (cm.GrandCharacterInfo.IsTraining) { result = true; }
                    }
                    return result;
                }
            }

            public DateTime CompletionTime
            {
                get
                {
                    DateTime result = DateTime.MaxValue;
                    foreach (CharacterMonitor cm in m_characters)
                    {
                        Skill trainingSkill = cm.GrandCharacterInfo.CurrentlyTrainingSkill;
                        if (trainingSkill != null && trainingSkill.EstimatedCompletion < result)
                        {
                            result = trainingSkill.EstimatedCompletion;
                        }
                    }
                    if (result == DateTime.MaxValue) { return DateTime.Now; }
                    else { return result; }
                }
            }

            public Account() : this(0) { }

            public Account(int userID)
            {
                m_userID = userID;
                m_characters = new CharacterCollection();
            }

            public void AddCharacter(CharacterMonitor cm)
            {
                m_characters.Add(cm);
            }

        }
        #endregion

    }
}
