using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.Common.Controls
{
    public class SkillQueueControl : Control
    {
        private static readonly DateTime s_paintTime = DateTime.UtcNow;

        private readonly InfiniteDisplayToolTip m_toolTip;

        private DateTime m_nextRepainting = DateTime.MinValue;
        private SkillQueue m_skillQueue;
        private Color m_firstColor = Color.LightBlue;
        private Color m_secondColor = Color.DarkBlue;
        private Color m_emptyColor = Color.DimGray;
        private Color m_borderColor = Color.Gray;
        private Point m_lastLocation = new Point(-1, -1);


        #region Constructors, disposing, global events

        /// <summary>
        /// Creates the skill queue control without an associates skill queue.
        /// </summary>
        public SkillQueueControl()
        {
            m_toolTip = new InfiniteDisplayToolTip(this);

            Disposed += OnDisposed;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            Disposed -= OnDisposed;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            m_toolTip.Dispose();
        }

        /// <summary>
        /// Every second, we checks whether we should update the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (DateTime.Now > m_nextRepainting)
                Invalidate();
        }

        /// <summary>
        /// When the settings changed, the "SafeForWork" propety which affects our color schemes may have changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// When the character changes, we invalidate the repainting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            CCPCharacter ccpCharacter = e.Character as CCPCharacter;

            // Current character isn't a CCP character, so can't have a Queue.
            if (ccpCharacter == null)
                return;

            if (m_skillQueue == null || ccpCharacter.SkillQueue != m_skillQueue)
                return;

            Invalidate();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Skill Queue to be rendered.
        /// </summary>
        [Category("Data")]
        [Description("Skill queue to render on the control canvas")]
        public SkillQueue SkillQueue
        {
            get { return m_skillQueue; }
            set
            {
                m_skillQueue = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The first of two colors to be used in the queue.
        /// </summary>
        [Category("Appearance")]
        [Description("First color of the component")]
        public Color FirstColor
        {
            get { return m_firstColor; }

            set
            {
                m_firstColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The second of two colours to be used in the queue.
        /// </summary>
        [Category("Appearance")]
        [Description("Second color of the component")]
        public Color SecondColor
        {
            get { return m_secondColor; }
            set
            {
                m_secondColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The color used for the free space at the end of the queue.
        /// </summary>
        [Category("Appearance")]
        [Description("Color used for the free space at the end of the queue when there are less than 24h of training queued.")]
        public Color EmptyColor
        {
            get { return m_emptyColor; }
            set
            {
                m_emptyColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The color used for the border of the queue.
        /// </summary>
        [Category("Appearance")]
        [Description("Color used for the border of the queue.")]
        public Color BorderColor
        {
            get { return m_borderColor; }
            set
            {
                m_borderColor = value;
                Invalidate();
            }
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Paint the skill queue to the control surface.
        /// </summary>
        /// <remarks>
        /// pe.Graphics is control suface. Width and Height are
        /// derived from the control itself not pe.ClipRectangle 
        /// which could point to part of the control
        /// </remarks>
        /// <param name="pe">Paint Event</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (pe == null)
                throw new ArgumentNullException("pe");

            base.OnPaint(pe);

            Graphics g = pe.Graphics;
            int width = Width;
            int height = Height;

            // If we are in DesignMode we just paint a dummy queue
            if (DesignMode)
                PaintDesignerQueue(g, width, height);
            else
                PaintQueue(g, width, height);

            // We need to update the painting only every (24h / width in pixels)
            m_nextRepainting = DateTime.Now.AddSeconds((double)(24 * 3600) / width);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get the first of the two alternating colours.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>First Colour property, or dark Gray if in safe for work mode</returns>
        private Color GetFirstColor()
        {
            return Settings.UI.SafeForWork ? Color.DarkGray : m_firstColor;
        }

        /// <summary>
        /// Gets the second of the two alternating colours.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>Second Colour property, or gray if in safe for work mode</returns>
        private Color GetSecondColor()
        {
            return Settings.UI.SafeForWork ? Color.Gray : m_secondColor;
        }

        /// <summary>
        /// Gets the color for the free time.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>Second Colour property, or dim gray if in safe for work mode</returns>
        private Color GetEmptyColor()
        {
            return Settings.UI.SafeForWork ? Color.DimGray : m_emptyColor;
        }

        /// <summary>
        /// Gets the border color.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>Second Colour property, or black if in safe for work mode</returns>
        private Color GetBorderColor()
        {
            return Settings.UI.SafeForWork ? Color.Black : m_borderColor;
        }

        /// <summary>
        /// Paints the point (right pointing arrow) on the canvas.
        /// </summary>
        /// <remarks>
        /// Actually paints an inverse triangle
        /// </remarks>
        /// <param name="g">Graphics canvas</param>
        /// <param name="width">Width of the canvas</param>
        /// <param name="height">Height of the canvas</param>
        private void PaintPoint(Graphics g, int width, int height)
        {
            using (Brush background = new SolidBrush(BackColor))
            {
                using (Pen pen = new Pen(GetBorderColor(), 1.0f))
                {
                    int halfHeight = (height / 2);
                    int pointWidth = (height / 2) + 1;

                    // Top triangle
                    PointF topTopLeft = new PointF(width - pointWidth, 0);
                    PointF topTopRight = new PointF(width, 0);
                    PointF topBottomRight = new PointF(width, halfHeight + 1);

                    PointF[] topTriangle = { topTopLeft, topTopRight, topBottomRight };

                    g.FillPolygon(background, topTriangle);

                    // Bottom triangle
                    PointF bottomTopRight = new PointF(width, halfHeight - 1);
                    PointF bottomBottomLeft = new PointF(width - pointWidth, height);
                    PointF bottomBottomRight = new PointF(width, height);

                    PointF[] bottomTriangle = { bottomTopRight, bottomBottomLeft, bottomBottomRight };

                    g.FillPolygon(background, bottomTriangle);

                    // Border (point)
                    g.DrawLine(pen, width - pointWidth, 0, width, halfHeight + 1);
                    g.DrawLine(pen, width, halfHeight - 1, width - pointWidth, height);

                    // Border (top, left, bottom lines)
                    g.DrawLine(pen, 0, 0, width - pointWidth, 0);
                    g.DrawLine(pen, 0, 0, 0, height);
                    g.DrawLine(pen, 0, height - 1, width - pointWidth, height - 1);
                }
            }
        }

        /// <summary>
        /// Paints the first 24 hours of the skill queue including the
        /// point if the queue has more than 24 hours contained within it.
        /// </summary>
        /// <param name="g">Graphics canvas</param>
        /// <param name="width">Width of the canvas</param>
        /// <param name="height">Height of the canvas</param>
        private void PaintQueue(Graphics g, int width, int height)
        {
            Brush[] brushes = new Brush[2];
            brushes[0] = new SolidBrush(GetFirstColor());
            brushes[1] = new SolidBrush(GetSecondColor());
            try
            {
                int brushNumber = 0;
                if (m_skillQueue == null)
                    return;

                int lastX = 0;
                foreach (Rectangle skillRect in m_skillQueue.Select(skill => GetSkillRect(skill, width, height)))
                {
                    g.FillRectangle(brushes[brushNumber], skillRect);
                    lastX = skillRect.Right;

                    // Rotate the brush
                    brushNumber = (brushNumber + 1) % brushes.Length;
                }

                // If there are more than 24 hours in the queue show the point
                if (m_skillQueue.EndTime > DateTime.UtcNow.AddHours(24))
                    PaintPoint(g, width, height);
                    // Else, draw a dark region at the end and the border
                else
                {
                    // Empty region
                    Rectangle emptyRect = new Rectangle(lastX, 0, Width - lastX, Height);
                    using (SolidBrush brush = new SolidBrush(GetEmptyColor()))
                    {
                        g.FillRectangle(brush, emptyRect);
                    }

                    // Then the border
                    using (Pen pen = new Pen(GetBorderColor(), 1.0f))
                    {
                        g.DrawRectangle(pen, 0, 0, width - 1, height - 1);
                    }
                }
            }
            finally
            {
                brushes[0].Dispose();
                brushes[1].Dispose();
            }
        }

        /// <summary>
        /// Gets the rectangle a skill rendes in within a specified rectange.
        /// </summary>
        /// <param name="skill">Skill that exists within the queue</param>
        /// <param name="width">Width of the canvas</param>
        /// <param name="height">Height of the canvas</param>
        /// <returns>
        /// Rectangle representing the area within the visual
        /// queue the skill occupies.
        /// </returns>
        public static Rectangle GetSkillRect(QueuedSkill skill, int width, int height)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            TimeSpan relativeStart;
            TimeSpan relativeFinish;

            // Character is training ? we update the timespan
            if (skill.Owner.IsTraining)
            {
                relativeStart = skill.StartTime.Subtract(DateTime.UtcNow);
                relativeFinish = skill.EndTime.Subtract(DateTime.UtcNow);
            }
                // Timespan is stable
            else
            {
                relativeStart = skill.StartTime.Subtract(s_paintTime);
                relativeFinish = skill.EndTime.Subtract(s_paintTime);
            }

            int totalSeconds = (int)TimeSpan.FromHours(24).TotalSeconds;

            double start = Math.Floor((relativeStart.TotalSeconds / totalSeconds) * width);
            double finish = Math.Floor((relativeFinish.TotalSeconds / totalSeconds) * width);

            // If the start time is before now set it to zero
            if (start < 0)
                start = 0;

            return new Rectangle((int)start, 0, (int)(finish - start), height);
        }

        /// <summary>
        /// Displays the skill tool tip.
        /// </summary>
        /// <param name="skillRect">The skill rect.</param>
        /// <param name="skill">The skill.</param>
        private void DisplaySkillToolTip(Rectangle skillRect, QueuedSkill skill)
        {
            const string Format = "{0} {1}\n  Start{2}\t{3}\n  Ends\t{4}";
            string skillName = skill.SkillName;
            string skillLevel = Skill.GetRomanFromInt(skill.Level);
            string skillStart = (skill.Owner.IsTraining
                                     ? skill.StartTime.ToLocalTime().ToAbsoluteDateTimeDescription(DateTimeKind.Local)
                                     : "Paused");
            string skillEnd = (skill.Owner.IsTraining
                                   ? skill.EndTime.ToLocalTime().ToAbsoluteDateTimeDescription(DateTimeKind.Local)
                                   : "Paused");
            string startText = (skill.StartTime < DateTime.UtcNow ? "ed" : "s");
            string text = String.Format(CultureConstants.DefaultCulture, Format, skillName, skillLevel, startText, skillStart,
                                        skillEnd);
            Size textSize = TextRenderer.MeasureText(text, Font);
            Size toolTipSize = new Size(textSize.Width + 13, textSize.Height + 11);
            Point tipPoint = new Point(((Math.Min(skillRect.Right, Width) + skillRect.Left) / 2) - toolTipSize.Width / 2, -toolTipSize.Height);
            tipPoint.Offset(0, -21);
            m_toolTip.Show(text, tipPoint);
        }

        /// <summary>
        /// Displays the free room tool tip.
        /// </summary>
        /// <param name="emptyRect">The empty rect.</param>
        private void DisplayFreeRoomToolTip(Rectangle emptyRect)
        {
            TimeSpan leftTime = (m_skillQueue.IsPaused ? s_paintTime : DateTime.UtcNow).AddHours(24) - m_skillQueue.EndTime;
            string text = String.Format(CultureConstants.DefaultCulture, "Free room:{0}",
                                        leftTime.ToDescriptiveText(DescriptiveTextOptions.SpaceBetween, false));
            Size textSize = TextRenderer.MeasureText(text, Font);
            Size toolTipSize = new Size(textSize.Width + 13, textSize.Height + 11);
            Point tipPoint = new Point(((emptyRect.Right + emptyRect.Left) / 2) - toolTipSize.Width / 2, - toolTipSize.Height);
            tipPoint.Offset(0, -21);
            m_toolTip.Show(text, tipPoint);
        }

        /// <summary>
        /// Triggers when the mouse is moved displays skill.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            base.OnMouseMove(e);

            if (m_skillQueue == null)
                return;

            // Prevent rapid triggering of event when the mouse hasn't moved
            if (e.Location == m_lastLocation)
                return;

            m_lastLocation = e.Location;

            int lastX = 0;
            foreach (QueuedSkill skill in m_skillQueue)
            {
                // Find the rectangle for the skill and paint it
                Rectangle skillRect = GetSkillRect(skill, Width, Height);
                lastX = skillRect.Right;

                if (!skillRect.Contains(e.Location))
                    continue;

                DisplaySkillToolTip(skillRect, skill);
                return;
            }

            // Are we in the empty space ?
            Rectangle emptyRect = new Rectangle(lastX, 0, Width - lastX, Height);
            if (emptyRect.Contains(e.Location))
            {
                DisplayFreeRoomToolTip(emptyRect);
                return;
            }

            m_toolTip.Hide();
        }

        /// <summary>
        /// Hide tooltip when mouse moves out of skill queue.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            m_toolTip.Hide();
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Spit out a static skill queue for the designer.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void PaintDesignerQueue(Graphics g, int width, int height)
        {
            using (Brush lightBrush = new SolidBrush(GetFirstColor()))
            {
                using (Brush darkBrush = new SolidBrush(GetSecondColor()))
                {
                    g.FillRectangle(lightBrush, new Rectangle(0, 0, (width / 5) * 2, height));
                    g.FillRectangle(darkBrush, new Rectangle((width / 5) * 2, 0, (width / 5) * 2, height));
                    g.FillRectangle(lightBrush, new Rectangle((width / 5) * 4, 0, width / 5, height));
                }
            }

            PaintPoint(g, width, height);
        }

        #endregion
    }
}