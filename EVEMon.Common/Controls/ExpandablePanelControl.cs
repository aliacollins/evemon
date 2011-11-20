using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public class ExpandablePanelControl : NoFlickerPanel
    {
        // Settings
        private PanelState m_panelState;
        private int m_animationStep;
        private int m_expandedHeight;

        // Header

        // ContextMenu
        private ContextMenuStrip m_contextMenuStrip;
        private ToolStripMenuItem m_tsmiExpandCollapse;
        private ToolStripMenuItem m_tsmiSelectAnim;
        private ToolStripSeparator m_tsmiSeparator;
        private ToolStripMenuItem m_tsmiNoAnim;
        private ToolStripMenuItem m_tsmiHighAnim;
        private ToolStripMenuItem m_tsmiMedAnim;
        private ToolStripMenuItem m_tsmiLowAnim;

        // Graphics variables
        private StringFormat m_hCenteredStringFormat;
        private Bitmap m_headerImage;
        private Bitmap m_expandImage;
        private Bitmap m_collapseImage;
        private int m_offset;
        private const int Pad = 6;


        #region Constructor

        /// <summary>
        /// Cunstructor.
        /// </summary>
        public ExpandablePanelControl()
        {
            HeaderText = "Header Text";
            ExpandDirection = Direction.Up;
            // Header
            CreateHeader();

            // ContextMenu
            CreateContextMenu();

            // Event handlers
            Header.Paint += nfpHeader_Paint;
            Header.MouseClick += expandablePanelControl_MouseClick;
            MouseClick += expandablePanelControl_MouseClick;
        }

        /// <summary>
        /// Gets true if the Panel is expanded.
        /// </summary>
        private bool IsExpanded
        {
            get { return m_panelState == PanelState.Expanded; }
        }

        /// <summary>
        /// Gets the Header of the Panel.
        /// </summary>
        [Browsable(false)]
        public NoFlickerPanel Header { get; private set; }

        /// <summary>
        /// Gets or sets the Header text.
        /// </summary>
        [Browsable(false)]
        public string HeaderText { get; set; }

        /// <summary>
        /// Gets or sets the expanded Height of the Panel.
        /// </summary>
        [Browsable(false)]
        public int ExpandedHeight
        {
            get { return m_expandedHeight; }
            set
            {
                m_expandedHeight = value;

                // If we start collapsed we don't have to update the height
                if (!IsExpanded && !ExpandedOnStartup)
                    return;

                Height = m_expandedHeight;
                Refresh();
            }
        }

        #endregion


        #region Control Creation Methods

        /// <summary>
        /// Creates the Header.
        /// </summary>
        private void CreateHeader()
        {
            Header = new NoFlickerPanel { Width = Width, Height = 30, BackColor = Color.Transparent };
            Controls.Add(Header);
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        private void CreateContextMenu()
        {
            m_contextMenuStrip = new ContextMenuStrip();
            m_tsmiExpandCollapse = new ToolStripMenuItem();
            m_tsmiSelectAnim = new ToolStripMenuItem();
            m_tsmiNoAnim = new ToolStripMenuItem();
            m_tsmiHighAnim = new ToolStripMenuItem();
            m_tsmiMedAnim = new ToolStripMenuItem();
            m_tsmiLowAnim = new ToolStripMenuItem();
            m_tsmiSeparator = new ToolStripSeparator();

            // Add menu items
            m_contextMenuStrip.Items.Add(m_tsmiExpandCollapse);
            m_contextMenuStrip.Items.Add(m_tsmiSeparator);
            m_contextMenuStrip.Items.Add(m_tsmiSelectAnim);
            m_tsmiSelectAnim.DropDownItems.AddRange(new ToolStripItem[]
                                                        {
                                                            m_tsmiNoAnim,
                                                            m_tsmiHighAnim,
                                                            m_tsmiMedAnim,
                                                            m_tsmiLowAnim
                                                        });

            // Apply properties
            m_tsmiSelectAnim.Text = "Animation Speed";
            m_tsmiNoAnim.Text = "None";
            m_tsmiHighAnim.Text = "High";
            m_tsmiMedAnim.Text = "Medium";
            m_tsmiLowAnim.Text = "Low";

            // Subscribe events
            m_tsmiExpandCollapse.Click += tsmiExpandCollapse_Click;
            foreach (ToolStripMenuItem item in m_tsmiSelectAnim.DropDownItems)
            {
                item.Click += animationSpeedSelect_Click;
            }
        }

        /// <summary>
        // Sets the check state of the appropriate AnimationSpeed menu item
        /// </summary>
        private void SetAnimationSpeedContextMenuItemCheckState()
        {
            foreach (ToolStripMenuItem item in m_tsmiSelectAnim.DropDownItems.Cast<ToolStripMenuItem>().Where(
                item => Enum.IsDefined(typeof(AnimationSpeed), item.Text)))
            {
                item.Checked = (AnimationSpeed)Enum.Parse(typeof(AnimationSpeed), item.Text) == AnimationSpeed;
            }
        }

        #endregion


        #region Graphics Methods

        /// <summary>
        /// Draws the main Panel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(SystemBrushes.ControlDark, 1))
            {
                gr.DrawLine(pen, 0, 0, 0, Height);
                gr.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1);
                gr.DrawLine(pen, Width - 1, Height - 1, Width - 1, 0);
                gr.DrawLine(pen, Width - 1, 0, 0, 0);
            }

            int height = (ExpandDirection == Direction.Up ? Height - Header.Height : 0);
            Header.Location = new Point(0, height);

            base.OnPaint(e);
        }

        /// <summary>
        /// Draws the Header.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nfpHeader_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            m_hCenteredStringFormat = new StringFormat { LineAlignment = StringAlignment.Center };

            Header.Width = Width;
            m_headerImage = (IsExpanded ? m_collapseImage : m_expandImage);

            if (m_headerImage != null)
            {
                m_offset = m_headerImage.Width + Pad;
                gr.DrawImage(m_headerImage,
                             new Rectangle(Pad, Header.Height / 2 - m_headerImage.Height / 2, m_headerImage.Width,
                                           m_headerImage.Height));
            }

            gr.DrawString(HeaderText, Font, Brushes.Black,
                          new RectangleF(Pad + m_offset, 0, Header.Width - Pad * 4, Header.Height), m_hCenteredStringFormat);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the image shown in the header when Panel is collapsed. Height must be less than HeaderHeight - 4 pixels. Null to disable it.
        /// </summary>
        [Description("The image used in the header when the Panel is collapsed.")]
        public Bitmap ImageExpand
        {
            get { return m_expandImage; }
            set
            {
                if (value != null)
                {
                    if (value.Height > Header.Height - 4)
                        throw new ArgumentException("HeaderIcon: Height must be less than HeaderHeight - 4 pixels.");
                }
                m_expandImage = value;
                Header.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the image shown in the header when Panel is expanded. Height must be less than HeaderHeight - 4 pixels. Null to disable it.
        /// </summary>
        [Description("The image used in the header when the Panel is expanded.")]
        public Bitmap ImageCollapse
        {
            get { return m_collapseImage; }
            set
            {
                if (value != null)
                {
                    if (value.Height > Header.Height - 4)
                        throw new ArgumentException("HeaderIcon: Height must be less than HeaderHeight - 4 pixels.");
                }
                m_collapseImage = value;
                Header.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the Expand/Collapse Speed.
        /// </summary>
        [Description("Set the Expand/Collapse Speed.")]
        public AnimationSpeed AnimationSpeed { get; set; }

        /// <summary>
        /// Gets or sets the Header's height of the Panel. Height must be between 30 and 50.
        /// </summary>
        [Description("Set the Header's height of the Panel. Height must be between 30 and 50.")]
        public int HeaderHeight
        {
            get { return Header.Height; }
            set
            {
                if (value < 30 || value > 50)
                    throw new ArgumentException("Height must be between 30 and 50.");

                Header.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the direction of the Panel expansion.
        /// </summary>
        [Description("Set the direction of the Panel expansion.")]
        public Direction ExpandDirection { get; set; }

        /// <summary>
        /// Gets or sets  whether the Panel should start expanded.
        /// </summary>
        [Description("Indicates whether the Panel should start expanded.")]
        public bool ExpandedOnStartup { get; set; }

        /// <summary>
        /// Gets or sets whether the contextMenu should be shown by right-clicking on the header.
        /// </summary>
        [Description("Indicates whether the contextMenu should be shown by right-clicking on the header.")]
        public bool EnableContextMenu { get; set; }

        #endregion


        #region Expand/Collapse Methods

        /// <summary>
        /// Expands the Panel.
        /// </summary>
        private void ExpandPanel()
        {
            while (AnimationSpeed != AnimationSpeed.None && Height < m_expandedHeight - m_animationStep)
            {
                Height += m_animationStep;
                Refresh();
            }

            Height = m_expandedHeight;
            m_headerImage = m_collapseImage;
            m_panelState = PanelState.Expanded;

            Refresh();
        }

        /// <summary>
        /// Collapses the Panel.
        /// </summary>
        private void CollapsePanel()
        {
            while (AnimationSpeed != AnimationSpeed.None && Height > Header.Height + m_animationStep)
            {
                Height -= m_animationStep;
                Refresh();
            }

            Height = Header.Height;
            m_headerImage = m_expandImage;
            m_panelState = PanelState.Collapsed;

            Refresh();
        }

        /// <summary>
        /// Triggers the Panel to expand or collapse.
        /// </summary>
        private void SwitchStatus()
        {
            switch (m_panelState)
            {
                case PanelState.Collapsed:
                    ExpandPanel();
                    break;
                case PanelState.Expanded:
                    CollapsePanel();
                    break;
            }

            UpdateContextMenu();
        }

        /// <summary>
        /// Updates the context menu text.
        /// </summary>
        private void UpdateContextMenu()
        {
            m_tsmiExpandCollapse.Text = (IsExpanded ? "Collapse Panel" : "Expand Panel");
        }

        /// <summary>
        /// Updates the animation speed.
        /// </summary>
        private void UpdateAnimationSpeed()
        {
            switch (AnimationSpeed)
            {
                case AnimationSpeed.None:
                    m_animationStep = ExpandedHeight;
                    break;
                case AnimationSpeed.High:
                    m_animationStep = ExpandedHeight / 4;
                    break;
                case AnimationSpeed.Medium:
                    m_animationStep = ExpandedHeight / 20;
                    break;
                case AnimationSpeed.Low:
                    m_animationStep = 1;
                    break;
            }

            // Set the check state of the appropriate AnimationSpeed menu item
            if (EnableContextMenu)
                SetAnimationSpeedContextMenuItemCheckState();
        }

        #endregion


        #region ContextMenu Events

        /// <summary>
        /// Occurs when we click on an animation choice context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void animationSpeedSelect_Click(object sender, EventArgs e)
        {
            string choice = ((ToolStripItem)sender).Text;

            switch (choice)
            {
                case "None":
                    AnimationSpeed = AnimationSpeed.None;
                    break;
                case "High":
                    AnimationSpeed = AnimationSpeed.High;
                    break;
                case "Medium":
                    AnimationSpeed = AnimationSpeed.Medium;
                    break;
                case "Low":
                    AnimationSpeed = AnimationSpeed.Low;
                    break;
            }

            UpdateAnimationSpeed();
        }

        /// <summary>
        /// Occurs when we click the Expand/Collapse context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiExpandCollapse_Click(object sender, EventArgs e)
        {
            SwitchStatus();
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Updates the controls settings.
        /// </summary>
        protected override void OnCreateControl()
        {
            // Set the expanded height of the panel according to the height set in the designer
            // It can be set to a manual height by replacing "Height" with the number of your choice
            m_expandedHeight = Height;

            if (DesignMode)
                return;

            // Set the panel status for startup
            m_animationStep = m_expandedHeight;
            m_panelState = (ExpandedOnStartup ? PanelState.Collapsed : PanelState.Expanded);
            SwitchStatus();

            // Set the animation speed
            UpdateAnimationSpeed();

            base.OnCreateControl();
        }

        /// <summary>
        /// Occurs on resizing the Panel.
        /// </summary>
        /// <remarks>Forces the Panel to redraw.</remarks>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            Invalidate();
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Occurs on a mouse click in the main Panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void expandablePanelControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!EnableContextMenu)
                    return;

                int x = e.X;
                int y = e.Y;

                if (sender != this)
                {
                    Control ctl = (Control)sender;
                    x += ctl.Bounds.X;
                    y += ctl.Bounds.Y;
                }

                m_contextMenuStrip.Enabled = EnableContextMenu;
                m_contextMenuStrip.Show(this, new Point(x, y));
                m_contextMenuStrip.BringToFront();
                return;
            }

            SwitchStatus();
        }

        #endregion
    }


    #region Enumerations

    /// <summary>
    /// Enumerator for the status of the Panel.
    /// </summary>
    public enum PanelState
    {
        Expanded,
        Collapsed
    }

    /// <summary>
    /// Enumerator for the speed of the Expand/Collapse animation.
    /// </summary>
    public enum AnimationSpeed
    {
        None,
        High,
        Medium,
        Low
    }

    /// <summary>
    /// Enumerator for the direction of the expansion.
    /// </summary>
    public enum Direction
    {
        Up,
        Down
    }

    #endregion
}