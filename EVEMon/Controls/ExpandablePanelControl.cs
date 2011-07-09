using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EVEMon.Controls
{
    public class ExpandablePanelControl : NoFlickerPanel
    {
        // Settings
        protected PanelState panelState;
        protected AnimationSpeed animationSpeed;
        protected Direction expandDirection = Direction.Up;
        private int m_animationStep;
        private int m_expandedHeight;
        private bool m_beginExpanded;
        
        // Header
        protected NoFlickerPanel nfpHeader;
        private string m_headerText = "Header Text";

        // ContextMenu
        protected ContextMenuStrip contextMenuStrip;
        protected ToolStripMenuItem tsmiExpandCollapse;
        protected ToolStripMenuItem tsmiSelectAnim;
        protected ToolStripSeparator tsmiSeparator;
        protected ToolStripMenuItem tsmiNoAnim;
        protected ToolStripMenuItem tsmiHighAnim;
        protected ToolStripMenuItem tsmiMedAnim;
        protected ToolStripMenuItem tsmiLowAnim;

        private bool m_enableContextMenu;

        // Graphics variables
        private StringFormat m_hCenteredStringFormat;
        private Bitmap m_headerImage;
        private Bitmap m_expandImage;
        private Bitmap m_collapseImage;
        private int m_offset;
        private int m_pad = 6;


        #region Constructor

        /// <summary>
        /// Cunstructor.
        /// </summary>
        public ExpandablePanelControl()
            : base()
        {
            // Header
            CreateHeader();

            // ContextMenu
            CreateContextMenu();

            // Event handlers
            nfpHeader.Paint += new PaintEventHandler(nfpHeader_Paint);
            nfpHeader.MouseClick += expandablePanelControl_MouseClick;
            MouseClick += expandablePanelControl_MouseClick;
        }

        /// <summary>
        /// Gets true if the Panel is expanded.
        /// </summary>
        internal bool IsExpanded
        {
            get { return panelState == PanelState.Expanded; }
        }

        /// <summary>
        /// Gets the Header of the Panel.
        /// </summary>
        internal NoFlickerPanel Header
        {
            get { return nfpHeader; }
        }

        /// <summary>
        /// Gets or sets the Header text.
        /// </summary>
        internal string HeaderText
        {
            get { return m_headerText; }
            set { m_headerText = value; }
        }

        /// <summary>
        /// Gets or sets the expanded Height of the Panel.
        /// </summary>
        internal int ExpandedHeight
        {
            get { return m_expandedHeight; }
            set
            {
                m_expandedHeight = value;

                // If we start collapsed we don't have to update the height
                if (!IsExpanded && !m_beginExpanded)
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
            nfpHeader = new NoFlickerPanel();
            nfpHeader.Width = Width;
            nfpHeader.Height = 30;
            nfpHeader.BackColor = Color.Transparent;
            Controls.Add(nfpHeader);
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        private void CreateContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();
            tsmiExpandCollapse = new ToolStripMenuItem();
            tsmiSelectAnim = new ToolStripMenuItem();
            tsmiNoAnim = new ToolStripMenuItem();
            tsmiHighAnim = new ToolStripMenuItem();
            tsmiMedAnim = new ToolStripMenuItem();
            tsmiLowAnim = new ToolStripMenuItem();
            tsmiSeparator = new ToolStripSeparator();
            
            // Add menu items
            contextMenuStrip.Items.Add(tsmiExpandCollapse);
            contextMenuStrip.Items.Add(tsmiSeparator);
            contextMenuStrip.Items.Add(tsmiSelectAnim);
            tsmiSelectAnim.DropDownItems.AddRange(new ToolStripItem[]
            {
                tsmiNoAnim,
                tsmiHighAnim,
                tsmiMedAnim,
                tsmiLowAnim
            });
            
            // Apply properties
            tsmiSelectAnim.Text = "Animation Speed";
            tsmiNoAnim.Text = "None";
            tsmiHighAnim.Text = "High";
            tsmiMedAnim.Text = "Medium";
            tsmiLowAnim.Text = "Low";

            // Subscribe events
            tsmiExpandCollapse.Click += tsmiExpandCollapse_Click;
            foreach (ToolStripMenuItem item in tsmiSelectAnim.DropDownItems)
            {
                item.Click += animationSpeedSelect_Click;
            }
        }

        /// <summary>
        // Sets the check state of the appropriate AnimationSpeed menu item
        /// </summary>
        private void SetAnimationSpeedContextMenuItemCheckState()
        {
            foreach (ToolStripMenuItem item in tsmiSelectAnim.DropDownItems)
            {
                if (Enum.IsDefined(typeof(AnimationSpeed), item.Text))
                    item.Checked = (AnimationSpeed)Enum.Parse(typeof(AnimationSpeed), item.Text) == animationSpeed;
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

            int height = (expandDirection == Direction.Up ? Height - nfpHeader.Height : 0);
            nfpHeader.Location = new Point(0, height);
            
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

            m_hCenteredStringFormat = new StringFormat();
            m_hCenteredStringFormat.LineAlignment = StringAlignment.Center;

            nfpHeader.Width = Width;
            m_headerImage = (IsExpanded ? m_collapseImage : m_expandImage);

            if (m_headerImage != null)
            {
                m_offset = m_headerImage.Width + m_pad;
                gr.DrawImage(m_headerImage, new Rectangle(m_pad, nfpHeader.Height / 2 - m_headerImage.Height / 2, m_headerImage.Width, m_headerImage.Height));
            }

            gr.DrawString(HeaderText, Font, Brushes.Black, new RectangleF(m_pad + m_offset, 0, nfpHeader.Width - m_pad * 4, nfpHeader.Height), m_hCenteredStringFormat);
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
                    if (value.Height > nfpHeader.Height - 4)
                    {
                        throw new ArgumentException("HeaderIcon: Height must be less than HeaderHeight - 4 pixels.");
                    }
                }
                m_expandImage = value;
                nfpHeader.Refresh();
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
                    if (value.Height > nfpHeader.Height - 4)
                    {
                        throw new ArgumentException("HeaderIcon: Height must be less than HeaderHeight - 4 pixels.");
                    }
                }
                m_collapseImage = value;
                nfpHeader.Refresh();

            }
        }

        /// <summary>
        /// Gets or sets the Expand/Collapse Speed.
        /// </summary>
        [Description("Set the Expand/Collapse Speed.")]
        public AnimationSpeed AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the Header's height of the Panel. Height must be between 30 and 50.
        /// </summary>
        [Description("Set the Header's height of the Panel. Height must be between 30 and 50.")]
        public int HeaderHeight
        {
            get { return nfpHeader.Height; }
            set
            {
                if (value < 30 || value > 50)
                    throw new ArgumentException("Height must be between 30 and 50.");

                nfpHeader.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the direction of the Panel expansion.
        /// </summary>
        [Description("Set the direction of the Panel expansion.")]
        public Direction ExpandDirection
        {
            get { return expandDirection; }
            set { expandDirection = value; }
        }

        /// <summary>
        /// Gets or sets  whether the Panel should start expanded.
        /// </summary>
        [Description("Indicates whether the Panel should start expanded.")]
        public bool ExpandedOnStartup
        {
            get { return m_beginExpanded; }
            set { m_beginExpanded = value; }
        }

        /// <summary>
        /// Gets or sets whether the contextMenu should be shown by right-clicking on the header.
        /// </summary>
        [Description("Indicates whether the contextMenu should be shown by right-clicking on the header.")]
        public bool EnableContextMenu
        {
            get { return m_enableContextMenu; }
            set { m_enableContextMenu = value; }
        }

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
            panelState = PanelState.Expanded;

            Refresh();

            // Clear memory
            GC.Collect();
        }

        /// <summary>
        /// Collapses the Panel.
        /// </summary>
        private void CollapsePanel()
        {
            while (AnimationSpeed != AnimationSpeed.None && Height > nfpHeader.Height + m_animationStep)
            {
                Height -= m_animationStep;
                Refresh();
            }

            Height = nfpHeader.Height;
            m_headerImage = m_expandImage;
            panelState = PanelState.Collapsed;

            Refresh();

            // Clear memory
            GC.Collect();
        }

        /// <summary>
        /// Triggers the Panel to expand or collapse.
        /// </summary>
        private void SwitchStatus()
        {
            switch (panelState)
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
            tsmiExpandCollapse.Text = (IsExpanded ? "Collapse Panel" : "Expand Panel");
        }

        /// <summary>
        /// Updates the animation speed.
        /// </summary>
        private void UpdateAnimationSpeed()
        {
            switch (animationSpeed)
            {
                case AnimationSpeed.None:
                    m_animationStep = ExpandedHeight;
                    break;
                case AnimationSpeed.High:
                    m_animationStep = (int)(ExpandedHeight / 4);
                    break;
                case AnimationSpeed.Medium:
                    m_animationStep = (int)(ExpandedHeight / 20);
                    break;
                case AnimationSpeed.Low:
                    m_animationStep = 1;
                    break;
            }

            // Set the check state of the appropriate AnimationSpeed menu item
            if (m_enableContextMenu)
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
            panelState = (m_beginExpanded ? PanelState.Collapsed : PanelState.Expanded);
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
        internal void expandablePanelControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!m_enableContextMenu)
                    return;

                int X = e.X;
                int Y = e.Y;

                if (sender != this)
                {
                    X += ((Control)sender).Bounds.X;
                    Y += ((Control)sender).Bounds.Y;
                }

                contextMenuStrip.Enabled = m_enableContextMenu;
                contextMenuStrip.Show(this, new Point(X, Y));
                contextMenuStrip.BringToFront();
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
