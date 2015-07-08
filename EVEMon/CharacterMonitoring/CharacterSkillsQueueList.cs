using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Properties;
using EVEMon.SkillPlanner;

namespace EVEMon.CharacterMonitoring
{
    internal sealed partial class CharacterSkillsQueueList : UserControl
    {
        #region Fields

        // Skills drawing - Region & text padding
        private const int PadTop = 2;
        private const int PadLeft = 6;
        private const int PadRight = 7;

        // Skills drawing - Boxes
        private const int BoxWidth = 57;
        private const int BoxHeight = 14;
        private const int LowerBoxHeight = 8;
        private const int BoxHPad = 6;
        private const int BoxVPad = 2;
        private const int MinimumHeight = 39;

        // Skills drawing - Font & brushes
        private readonly Font m_boldSkillsQueueFont;
        private readonly Font m_skillsQueueFont;

        private int m_count;
        private Object m_lastTooltipItem;
        private DateTime m_nextRepainting = DateTime.MinValue;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CharacterSkillsQueueList()
        {
            InitializeComponent();

            lbSkillsQueue.Visible = false;

            m_skillsQueueFont = FontFactory.GetFont("Tahoma", 8.25F);
            m_boldSkillsQueueFont = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold);
            noSkillsQueueLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        internal CCPCharacter Character { get; set; }

        /// <summary>
        /// Gets the item's height.
        /// </summary>
        private int GetItemHeight
        {
            get { return Math.Max(m_skillsQueueFont.Height * 2 + PadTop * 2 + LowerBoxHeight, MinimumHeight); }
        }

        #endregion


        #region Inherited events

        /// <summary>
        /// On load subscribe the events.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            EveMonClient.CharacterSkillQueueUpdated += EveMonClient_CharacterSkillQueueUpdated;
            EveMonClient.QueuedSkillsCompleted += EveMonClient_QueuedSkillsCompleted;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.CharacterSkillQueueUpdated -= EveMonClient_CharacterSkillQueueUpdated;
            EveMonClient.QueuedSkillsCompleted -= EveMonClient_QueuedSkillsCompleted;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// When the control becomes visible again, we update the content.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                UpdateContent();
        }

        #endregion


        #region Display update

        /// <summary>
        /// Updates all the content.
        /// </summary>
        private void UpdateContent()
        {
            // Returns if not visible
            if (!Visible)
                return;

            // When no character, we just hide the list
            if (Character == null)
            {
                noSkillsQueueLabel.Visible = true;
                lbSkillsQueue.Visible = false;
                return;
            }

            int scrollBarPosition = lbSkillsQueue.TopIndex;

            // Update the skills queue list
            lbSkillsQueue.BeginUpdate();
            try
            {
                // Add items in the list
                lbSkillsQueue.Items.Clear();
                foreach (QueuedSkill skill in Character.SkillQueue)
                {
                    lbSkillsQueue.Items.Add(skill);
                }

                // Display or hide the "no queue skills" label.
                noSkillsQueueLabel.Visible = !Character.SkillQueue.Any();
                lbSkillsQueue.Visible = !noSkillsQueueLabel.Visible;

                // Invalidate display
                lbSkillsQueue.Invalidate();
            }
            finally
            {
                lbSkillsQueue.EndUpdate();
                lbSkillsQueue.TopIndex = scrollBarPosition;
            }
        }

        #endregion


        #region Drawing

        /// <summary>
        /// Handles the DrawItem event of the lbSkillsQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void lbSkillsQueue_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lbSkillsQueue.Items.Count)
                return;

            QueuedSkill item = lbSkillsQueue.Items[e.Index] as QueuedSkill;

            if (item == null)
                return;

            DrawItem(item, e);
        }

        /// <summary>
        /// Handles the MeasureItem event of the lbSkillsQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MeasureItemEventArgs"/> instance containing the event data.</param>
        private void lbSkillsQueue_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.ItemHeight = GetItemHeight;
        }

        /// <summary>
        /// Draws the list item for the given skill
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="e"></param>
        private void DrawItem(QueuedSkill skill, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw background
            g.FillRectangle(e.Index % 2 == 0 ? Brushes.LightGray : Brushes.White, e.Bounds);


            // Measure texts
            const TextFormatFlags Format = TextFormatFlags.NoPadding | TextFormatFlags.NoClipping;

            Int64 skillPoints = (skill.Skill == null || skill.Skill == Skill.UnknownSkill
                ? skill.StartSP
                : skill.Skill.SkillPoints);
            Int64 skillPointsToNextLevel = (skill.Skill == null || skill.Skill == Skill.UnknownSkill
                ? skill.EndSP
                : skill.Skill.StaticData.GetPointsRequiredForLevel(
                    Math.Min(skill.Level, 5)));

            double percentCompleted = e.Index == 0 || skill.Skill != Skill.UnknownSkill
                ? skill.PercentCompleted
                : 0d;

            if (skill.Skill != null && skill.Level > skill.Skill.Level + 1)
                skillPoints = skill.CurrentSP;

            string indexText = String.Format(CultureConstants.DefaultCulture, "{0}. ", e.Index + 1);
            string rankText = String.Format(CultureConstants.DefaultCulture, " (Rank {0})",
                (skill.Skill == null ? 0 : skill.Rank));
            string spText = String.Format(CultureConstants.DefaultCulture, "SP: {0:N0}/{1:N0}", skillPoints,
                skillPointsToNextLevel);
            string levelText = String.Format(CultureConstants.DefaultCulture, "Level {0}", skill.Level);
            string pctText = String.Format(CultureConstants.DefaultCulture, "{0}% Done", Math.Floor(percentCompleted));

            Size indexTextSize = TextRenderer.MeasureText(g, indexText, m_boldSkillsQueueFont, Size.Empty, Format);
            Size skillNameSize = TextRenderer.MeasureText(g, skill.SkillName, m_boldSkillsQueueFont, Size.Empty, Format);
            Size rankTextSize = TextRenderer.MeasureText(g, rankText, m_skillsQueueFont, Size.Empty, Format);
            Size levelTextSize = TextRenderer.MeasureText(g, levelText, m_skillsQueueFont, Size.Empty, Format);
            Size spTextSize = TextRenderer.MeasureText(g, spText, m_skillsQueueFont, Size.Empty, Format);
            Size pctTextSize = TextRenderer.MeasureText(g, pctText, m_skillsQueueFont, Size.Empty, Format);


            // Draw texts
            Color highlightColor = Color.Black;

            TextRenderer.DrawText(g, indexText, m_boldSkillsQueueFont,
                new Rectangle(e.Bounds.Left + PadLeft, e.Bounds.Top + PadTop,
                    indexTextSize.Width + PadLeft, indexTextSize.Height), highlightColor);

            TextRenderer.DrawText(g, skill.SkillName, m_boldSkillsQueueFont,
                new Rectangle(e.Bounds.Left + PadLeft + indexTextSize.Width, e.Bounds.Top + PadTop,
                    skillNameSize.Width + PadLeft, skillNameSize.Height), highlightColor);
            TextRenderer.DrawText(g, rankText, m_skillsQueueFont,
                new Rectangle(e.Bounds.Left + PadLeft + indexTextSize.Width + skillNameSize.Width, e.Bounds.Top + PadTop,
                    rankTextSize.Width + PadLeft, rankTextSize.Height), highlightColor);
            TextRenderer.DrawText(g, spText, m_skillsQueueFont,
                new Rectangle(e.Bounds.Left + PadLeft + indexTextSize.Width,
                    e.Bounds.Top + PadTop + skillNameSize.Height,
                    spTextSize.Width + PadLeft, spTextSize.Height), highlightColor);


            // Boxes
            DrawBoxes(percentCompleted, skill, e);

            // Draw progression bar
            DrawProgressionBar(percentCompleted, e);


            // Draw level and percent texts
            TextRenderer.DrawText(g, levelText, m_skillsQueueFont,
                new Rectangle(
                    e.Bounds.Right - BoxWidth - PadRight - BoxHPad -
                    levelTextSize.Width,
                    e.Bounds.Top + PadTop,
                    levelTextSize.Width + PadRight,
                    levelTextSize.Height), Color.Black);

            TextRenderer.DrawText(g, pctText, m_skillsQueueFont,
                new Rectangle(
                    e.Bounds.Right - BoxWidth - PadRight - BoxHPad -
                    pctTextSize.Width,
                    e.Bounds.Top + PadTop + levelTextSize.Height,
                    pctTextSize.Width + PadRight,
                    pctTextSize.Height), Color.Black);

            // Draw the queue color bar
            DrawQueueColorBar(skill, e);
        }

        /// <summary>
        /// Draws the progression bar.
        /// </summary>
        /// <param name="percentCompleted">The percent completed.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private static void DrawProgressionBar(double percentCompleted, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(Pens.Black,
                new Rectangle(e.Bounds.Right - BoxWidth - PadRight,
                    e.Bounds.Top + PadTop + BoxHeight + BoxVPad, BoxWidth, LowerBoxHeight));

            Rectangle pctBarRect = new Rectangle(e.Bounds.Right - BoxWidth - PadRight + 2,
                e.Bounds.Top + PadTop + BoxHeight + BoxVPad + 2,
                BoxWidth - 3, LowerBoxHeight - 3);

            g.FillRectangle(Brushes.DarkGray, pctBarRect);
            int fillWidth = (int)(pctBarRect.Width * (percentCompleted / 100));
            if (fillWidth <= 0)
                return;

            Rectangle fillRect = new Rectangle(pctBarRect.X, pctBarRect.Y, fillWidth, pctBarRect.Height);
            g.FillRectangle(Brushes.Black, fillRect);
        }

        /// <summary>
        /// Draws the boxes.
        /// </summary>
        /// <param name="percentCompleted">The percent completed.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void DrawBoxes(double percentCompleted, QueuedSkill skill, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(Pens.Black,
                new Rectangle(e.Bounds.Right - BoxWidth - PadRight, e.Bounds.Top + PadTop, BoxWidth,
                    BoxHeight));

            const int LevelBoxWidth = (BoxWidth - 4 - 3) / 5;
            for (int level = 1; level <= 5; level++)
            {
                Rectangle brect =
                    new Rectangle(e.Bounds.Right - BoxWidth - PadRight + 2 + (LevelBoxWidth * (level - 1)) + (level - 1),
                        e.Bounds.Top + PadTop + 2, LevelBoxWidth, BoxHeight - 3);

                // Box color
                g.FillRectangle(skill.Skill != null && level < skill.Level ? Brushes.Black : Brushes.DarkGray, brect);

                // Color indicator for a queued level
                if (skill.Skill == null)
                    continue;

                Brush brush = (Settings.UI.SafeForWork ? Brushes.Gray : Brushes.RoyalBlue);

                foreach (QueuedSkill qskill in Character.SkillQueue)
                {
                    if ((!qskill.IsTraining && skill == qskill && level == qskill.Level)
                        || (skill == qskill && level <= qskill.Level && level > skill.Skill.Level
                            && Math.Abs(percentCompleted) < double.Epsilon))
                        g.FillRectangle(brush, brect);

                    // Blinking indicator of skill level in training
                    if (!qskill.IsTraining || skill != qskill || level != skill.Level ||
                        Math.Abs(percentCompleted) < double.Epsilon)
                        continue;

                    if (m_count == 0)
                        g.FillRectangle(Brushes.White, brect);

                    if (m_count == 1)
                        m_count = -1;

                    m_count++;
                }
            }
        }

        /// <summary>
        /// Draws the queue color bar.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void DrawQueueColorBar(QueuedSkill skill, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw skill queue color bar
            Brush brush = (Settings.UI.SafeForWork ? Brushes.DarkGray : Brushes.CornflowerBlue);
            Rectangle qBarRect = new Rectangle(e.Bounds.Left, GetItemHeight - LowerBoxHeight, e.Bounds.Width, LowerBoxHeight);
            g.FillRectangle(Brushes.DimGray, qBarRect);
            Rectangle skillRect = SkillQueueControl.GetSkillRect(skill, qBarRect.Width, LowerBoxHeight - 1);
            g.FillRectangle(brush,
                new Rectangle(skillRect.X, GetItemHeight - LowerBoxHeight, skillRect.Width, skillRect.Height));

            // If we have more than one skill level in queue, we need to redraw them only every (24h / width in pixels)
            if (e.Index == 1)
                m_nextRepainting = DateTime.Now.AddSeconds(Convert.ToDouble(86400 / Width));
        }

        #endregion


        #region Local events

        /// <summary>
        /// Handles the MouseWheel event of the lbSkillsQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbSkillsQueue_MouseWheel(object sender, MouseEventArgs e)
        {
            // Update the drawing based upon the mouse wheel scrolling.
            int numberOfItemLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            int lines = numberOfItemLinesToMove;
            if (lines == 0)
                return;

            // Compute the number of lines to move
            int direction = lines / Math.Abs(lines);
            int[] numberOfPixelsToMove = new int[lines * direction];
            for (int i = 1; i <= Math.Abs(lines); i++)
            {
                object item = null;

                // Going up
                if (direction == Math.Abs(direction))
                {
                    // Retrieve the next top item
                    if (lbSkillsQueue.TopIndex - i >= 0)
                        item = lbSkillsQueue.Items[lbSkillsQueue.TopIndex - i];
                }
                    // Going down
                else
                {
                    // Compute the height of the items from current the topindex (included)
                    int height = 0;
                    for (int j = lbSkillsQueue.TopIndex + i - 1; j < lbSkillsQueue.Items.Count; j++)
                    {
                        height += GetItemHeight;
                    }

                    // Retrieve the next bottom item
                    if (height > lbSkillsQueue.ClientSize.Height)
                        item = lbSkillsQueue.Items[lbSkillsQueue.TopIndex + i - 1];
                }

                // If found a new item as top or bottom
                if (item != null)
                    numberOfPixelsToMove[i - 1] = GetItemHeight * direction;
                else
                    lines -= direction;
            }

            // Scroll 
            if (lines != 0)
                lbSkillsQueue.Invalidate();
        }

        /// <summary>
        /// Handles the MouseDown event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbSkills_MouseDown(object sender, MouseEventArgs e)
        {
            // Retrieve the item at the given point and quit if none
            int index = lbSkillsQueue.IndexFromPoint(e.X, e.Y);
            if (index < 0 || index >= lbSkillsQueue.Items.Count)
                return;

            // Beware, this last index may actually means a click in the whitespace at the bottom
            // Let's deal with this special case
            if (index == lbSkillsQueue.Items.Count - 1)
            {
                Rectangle itemRect = lbSkillsQueue.GetItemRectangle(index);
                if (!itemRect.Contains(e.Location))
                    return;
            }

            // Right click for skills below lv5 : we display a context menu to plan higher levels
            QueuedSkill item = lbSkillsQueue.Items[index] as QueuedSkill;

            if (item == null)
                return;

            Skill skill = item.Skill;
            if (skill != null)
            {
                if (e.Button != MouseButtons.Right)
                    return;

                // Build the context menu
                BuildContextMenu(skill);

                // Display the context menu
                contextMenuStripPlanPopup.Show((Control)sender, new Point(e.X, e.Y));
                return;
            }

            // Non-right click or already lv5, display the tooltip
            DisplayTooltip(item);
        }

        /// <summary>
        /// On mouse move, we hide the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSkills_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < lbSkillsQueue.Items.Count; i++)
            {
                // Skip until we found the mouse location
                Rectangle rect = lbSkillsQueue.GetItemRectangle(i);
                if (!rect.Contains(e.Location))
                    continue;

                // Updates the tooltip
                QueuedSkill item = lbSkillsQueue.Items[i] as QueuedSkill;
                DisplayTooltip(item);
                return;
            }

            // If we went so far, we're not over anything
            m_lastTooltipItem = null;
            ttToolTip.Active = false;
        }

        /// <summary>
        /// Builds the context menu.
        /// </summary>
        /// <param name="skill">The skill.</param>
        private void BuildContextMenu(Skill skill)
        {
            contextMenuStripPlanPopup.Items.Clear();

            // "Show in Skill Explorer" menu item
            ToolStripMenuItem tmSkillExplorerTemp = null;
            try
            {
                tmSkillExplorerTemp = new ToolStripMenuItem("Show In Skill &Explorer...", Resources.LeadsTo);
                tmSkillExplorerTemp.Click += tmSkillExplorer_Click;
                tmSkillExplorerTemp.Tag = skill;

                ToolStripMenuItem tmSkillExplorer = tmSkillExplorerTemp;
                tmSkillExplorerTemp = null;

                // Add to the context menu
                contextMenuStripPlanPopup.Items.Add(tmSkillExplorer);
            }
            finally
            {
                if (tmSkillExplorerTemp != null)
                    tmSkillExplorerTemp.Dispose();
            }

            // Quit here if skill is fully trained
            if (skill.Level == 5)
                return;

            // Add a separator
            contextMenuStripPlanPopup.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tempMenuItem = null;
            try
            {
                // Reset the menu
                tempMenuItem = new ToolStripMenuItem(String.Format(CultureConstants.DefaultCulture, "Add {0}", skill.Name));

                // Build the level options
                Int64 nextLevel = Math.Min(5, skill.Level + 1);
                for (Int64 level = nextLevel; level < 6; level++)
                {
                    ToolStripMenuItem tempMenuLevel = null;
                    try
                    {
                        tempMenuLevel = new ToolStripMenuItem(
                            String.Format(CultureConstants.DefaultCulture, "Level {0} to", Skill.GetRomanFromInt(level)));

                        Character.Plans.AddTo(tempMenuLevel.DropDownItems,
                            (menuPlanItem, plan) =>
                            {
                                menuPlanItem.Click += menuPlanItem_Click;
                                menuPlanItem.Tag = new KeyValuePair<Plan, SkillLevel>(plan,
                                    new SkillLevel(skill, level));
                            });

                        ToolStripMenuItem menuLevel = tempMenuLevel;
                        tempMenuLevel = null;

                        tempMenuItem.DropDownItems.Add(menuLevel);
                    }
                    finally
                    {
                        if (tempMenuLevel != null)
                            tempMenuLevel.Dispose();
                    }
                }

                ToolStripMenuItem menuItem = tempMenuItem;
                tempMenuItem = null;

                // Add to the context menu
                contextMenuStripPlanPopup.Items.Add(menuItem);
            }
            finally
            {
                if (tempMenuItem != null)
                    tempMenuItem.Dispose();
            }

            // Add a separator
            contextMenuStripPlanPopup.Items.Add(new ToolStripSeparator());
            ToolStripMenuItem tempCreatePlanMenuItem = null;
            try
            {
                tempCreatePlanMenuItem = new ToolStripMenuItem("Create Plan from Skill Queue...");
                tempCreatePlanMenuItem.Click += tempCreatePlanMenuItem_Click;

                ToolStripMenuItem tmCreatePlan = tempCreatePlanMenuItem;
                tempCreatePlanMenuItem = null;

                // Add to the context menu
                contextMenuStripPlanPopup.Items.Add(tmCreatePlan);
            }
            finally
            {
                if (tempCreatePlanMenuItem != null)
                    tempCreatePlanMenuItem.Dispose();
            }
        }

        /// <summary>
        /// Displays the tooltip for the given skill.
        /// </summary>
        /// <param name="item"></param>
        private void DisplayTooltip(QueuedSkill item)
        {
            if (ttToolTip.Active && m_lastTooltipItem != null && m_lastTooltipItem == item)
                return;

            m_lastTooltipItem = item;

            ttToolTip.Active = false;
            ttToolTip.SetToolTip(lbSkillsQueue, GetTooltip(item));
            ttToolTip.Active = true;
        }

        /// <summary>
        /// Gets the tooltip text for the given skill
        /// </summary>
        /// <param name="skill"></param>
        private static string GetTooltip(QueuedSkill skill)
        {
            if (skill.Skill == null)
                return String.Empty;

            Int64 sp = skill.Skill.SkillPoints;
            int nextLevel = Math.Min(5, skill.Level);
            double percentCompleted = skill.PercentCompleted;

            if (skill.Level > skill.Skill.Level + 1)
                sp = skill.CurrentSP;

            Int64 nextLevelSP = skill.Skill == Skill.UnknownSkill
                ? skill.EndSP
                : skill.Skill.StaticData.GetPointsRequiredForLevel(nextLevel);
            Int64 pointsLeft = nextLevelSP - sp;
            TimeSpan timeSpanFromPoints = skill.Skill == Skill.UnknownSkill
                ? skill.EndTime.Subtract(DateTime.UtcNow)
                : skill.Skill.GetTimeSpanForPoints(pointsLeft);
            string remainingTimeText = timeSpanFromPoints.ToDescriptiveText(
                DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.UppercaseText);

            if (sp < skill.Skill.StaticData.GetPointsRequiredForLevel(1))
            {
                // Training hasn't got past level 1 yet
                StringBuilder untrainedToolTip = new StringBuilder();
                untrainedToolTip.AppendFormat(CultureConstants.DefaultCulture, "Not yet trained to Level I ({0}%)\n",
                                              Math.Floor(percentCompleted));
                untrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                              "Next level I: {0:N0} skill points remaining\n", pointsLeft);
                untrainedToolTip.AppendFormat(CultureConstants.DefaultCulture, "Training time remaining: {0}\n",
                                              remainingTimeText);
                AddSkillBoilerPlate(untrainedToolTip, skill);
                return untrainedToolTip.ToString();
            }

            // So, it's a left click on a skill, we display the tool tip
            // Currently training skill?
            if (skill.IsTraining && percentCompleted > 0)
            {
                StringBuilder partiallyTrainedToolTip = new StringBuilder();
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture, "Partially Completed ({0}%)\n",
                                                     Math.Floor(percentCompleted));
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                     "Training to level {0}: {1:N0} skill points remaining\n",
                                                     Skill.GetRomanFromInt(nextLevel), pointsLeft);
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture, "Training time remaining: {0}\n",
                                                     remainingTimeText);
                AddSkillBoilerPlate(partiallyTrainedToolTip, skill);
                return partiallyTrainedToolTip.ToString();
            }

            // Currently training skill but next queued level?
            if (skill.IsTraining && Math.Abs(percentCompleted) < double.Epsilon)
            {
                StringBuilder partiallyTrainedToolTip = new StringBuilder();
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                     "Previous level not yet completed\n");
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                     "Queued to level {0}: {1:N0} skill points remaining\n",
                                                     Skill.GetRomanFromInt(nextLevel), pointsLeft);
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture, "Training time to next level: {0}\n",
                                                     remainingTimeText);
                AddSkillBoilerPlate(partiallyTrainedToolTip, skill);
                return partiallyTrainedToolTip.ToString();
            }

            // Partially trained skill and not in training?
            if (skill.Skill.IsPartiallyTrained && !skill.IsTraining)
            {
                StringBuilder partiallyTrainedToolTip = new StringBuilder();
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture, "Partially Completed ({0}%)\n",
                                                     Math.Floor(percentCompleted));
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                     "Queued to level {0}: {1:N0} skill points remaining\n",
                                                     Skill.GetRomanFromInt(nextLevel), pointsLeft);
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture, "Training time remaining: {0}\n",
                                                     remainingTimeText);
                AddSkillBoilerPlate(partiallyTrainedToolTip, skill);
                return partiallyTrainedToolTip.ToString();
            }

            // We've completed all the skill points for the current level
            if (!skill.Skill.IsPartiallyTrained && skill.Level != 5)
            {
                StringBuilder levelCompleteToolTip = new StringBuilder();
                levelCompleteToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                  "Completed Level {0}: {1:N0}/{2:N0}\n",
                                                  Skill.GetRomanFromInt(skill.Level - 1), sp, nextLevelSP);
                levelCompleteToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                  "Queued level {0}: {1:N0} skill points required\n",
                                                  Skill.GetRomanFromInt(nextLevel), pointsLeft);
                levelCompleteToolTip.AppendFormat(CultureConstants.DefaultCulture, "Training time to next level: {0}\n",
                                                  remainingTimeText);
                AddSkillBoilerPlate(levelCompleteToolTip, skill);
                return levelCompleteToolTip.ToString();
            }

            // Error in calculating SkillPoints
            StringBuilder calculationErrorToolTip = new StringBuilder();
            calculationErrorToolTip.AppendLine("Partially Trained (Could not calculate all skill details)");
            calculationErrorToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                 "Next level {0}: {1:N0} skill points remaining\n", nextLevel,
                                                 pointsLeft);
            calculationErrorToolTip.AppendFormat(CultureConstants.DefaultCulture, "Training time remaining: {0}\n",
                                                 remainingTimeText);
            AddSkillBoilerPlate(calculationErrorToolTip, skill);
            return calculationErrorToolTip.ToString();
        }

        /// <summary>
        /// Adds the skill boiler plate.
        /// </summary>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="skill">The skill.</param>
        private static void AddSkillBoilerPlate(StringBuilder toolTip, QueuedSkill skill)
        {
            toolTip.AppendLine();
            toolTip.AppendLine(skill.Skill.Description.WordWrap(100));
            toolTip.AppendFormat(CultureConstants.DefaultCulture, "Primary: {0}, ", skill.Skill.PrimaryAttribute);
            toolTip.AppendFormat(CultureConstants.DefaultCulture, "Secondary: {0} ", skill.Skill.SecondaryAttribute);
            toolTip.AppendFormat(CultureConstants.DefaultCulture, "({0:N0} SP/hour)", skill.SkillPointsPerHour);
        }

        /// <summary>
        /// Handler for a context menu item click on a skill.
        /// Add the entire skill queue to a plan.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tempCreatePlanMenuItem_Click(object sender, EventArgs e)
        {
            if (Character == null)
                return;

            // Ask the user for a new name
            string planName,
                planDescription;
            using (NewPlanWindow npw = new NewPlanWindow { PlanName = "Current Skill Queue" })
            {
                DialogResult dr = npw.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                planName = npw.PlanName;
                planDescription = npw.PlanDescription;
            }

            // Create a new plan
            Plan newPlan = new Plan(Character) { Name = planName, Description = planDescription };

            if (Character.Plans.Any(x => x.Name == newPlan.Name))
            {
                MessageBox.Show("There is already a plan with the same name in the characters' Plans.",
                    "Plan Creation Failure",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Add skill queue in plan
            foreach (QueuedSkill qSkill in Character.SkillQueue)
            {
                newPlan.PlanTo(qSkill.Skill, qSkill.Level);
            }

            // Check if there is already a plan with the same skills
            if (Character.Plans.Any(plan => !newPlan.Except(plan, new PlanEntryComparer()).Any()))
            {
                MessageBox.Show("There is already a plan with the same skills in the characters' Plans.",
                    "Plan Creation Failure",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Add plan and save
            Character.Plans.Insert(0, newPlan);

            // Show the editor for this plan
            WindowsFactory.ShowByTag<PlanWindow, Plan>(newPlan);
        }

        /// <summary>
        /// Handler for a context menu item click on a skill.
        /// Add a skill to the plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void menuPlanItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem planItem = (ToolStripMenuItem)sender;
            KeyValuePair<Plan, SkillLevel> tag = (KeyValuePair<Plan, SkillLevel>)planItem.Tag;

            IPlanOperation operation = tag.Key.TryPlanTo(tag.Value.Skill, tag.Value.Level);
            PlanHelper.SelectPerform(operation);
        }

        /// <summary>
        /// Handler for a context menu item click on a skill.
        /// Shows the selected skill in Skill Explorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void tmSkillExplorer_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Skill skill = (Skill)item.Tag;

            SkillExplorerWindow window = WindowsFactory.ShowUnique<SkillExplorerWindow>();
            window.Skill = skill;
        }

        #endregion


        #region Global events

        /// <summary>
        /// On timer tick, we invalidate the training skill display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (!Visible || Character == null || !Character.IsTraining)
                return;

            // Retrieves the trained skill for update but quit if the skill is null (was not in our datafiles)
            QueuedSkill trainingSkill = Character.CurrentlyTrainingSkill;
            if (trainingSkill == null)
                return;

            // Invalidate the currently training skill level row
            int index = lbSkillsQueue.Items.IndexOf(trainingSkill);
            if (index == 0)
                lbSkillsQueue.Invalidate(lbSkillsQueue.GetItemRectangle(index));

            // When there are more than one skill level rows in queue, we invalidate them on a timer
            if (lbSkillsQueue.Items.Count > 1 && DateTime.Now > m_nextRepainting)
                lbSkillsQueue.Invalidate();
        }

        /// <summary>
        /// When the skill queue changed, we refresh the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterSkillQueueUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != Character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// When the queue changed, we refresh the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_QueuedSkillsCompleted(object sender, QueuedSkillsEventArgs e)
        {
            if (e.Character != Character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// When the settings changed, we refresh the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        #endregion
    }
}