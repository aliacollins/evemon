using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Properties;
using EVEMon.SkillPlanner;

namespace EVEMon.CharacterMonitoring
{
    public partial class CharacterSkillsList : UserControl
    {
        private const TextFormatFlags Format = TextFormatFlags.NoPadding | TextFormatFlags.NoClipping;
        private const byte SkillsSummaryTextWidth = 75;
        private const byte SkillGroupTotalSPTextWidth = 100;

        // Skills drawing - Region & text padding
        private const byte PadTop = 2;
        private const byte PadLeft = 6;
        private const byte PadRight = 7;

        // Skills drawing - Boxes
        private const byte BoxWidth = 57;
        private const byte BoxHeight = 14;
        private const byte LowerBoxHeight = 8;
        private const byte BoxHPad = 6;
        private const byte BoxVPad = 2;
        private const byte SkillDetailHeight = 31;

        // Skills drawing - Skills groups
        private const byte SkillHeaderHeight = 21;
        private const byte CollapserPadRight = 4;

        // Skills drawing - Font & brushes
        private readonly Font m_skillsFont;
        private readonly Font m_boldSkillsFont;
        private Object m_lastTooltipItem;
        private bool m_requireRefresh;
        private sbyte m_count;

        private int m_maxGroupNameWidth;

        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterSkillsList()
        {
            InitializeComponent();

            lbSkills.Visible = false;

            m_skillsFont = FontFactory.GetFont("Tahoma", 8.25F);
            m_boldSkillsFont = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold);
            noSkillsLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            m_requireRefresh = true;

            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        [Browsable(false)]
        public Character Character { get; set; }


        #region Inherited events

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
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


        #region Content Management

        /// <summary>
        /// Updates all the content.
        /// </summary>
        /// <remarks>
        /// Another high-complexity method for us to look at.
        /// </remarks>
        private void UpdateContent()
        {
            // Returns if not visible
            if (!Visible)
                return;

            // When no character, we just hide the list
            if (Character == null)
            {
                noSkillsLabel.Visible = true;
                lbSkills.Visible = false;
                return;
            }

            int scrollBarPosition = lbSkills.TopIndex;

            // Update the skills list
            lbSkills.BeginUpdate();
            try
            {
                IEnumerable<Skill> skills = GetCharacterSkills();
                IOrderedEnumerable<IGrouping<SkillGroup, Skill>> groups =
                    skills.GroupBy(x => x.Group).OrderBy(x => x.Key.Name);

                m_maxGroupNameWidth = (groups.Select(
                    group => TextRenderer.MeasureText(group.Key.Name, m_boldSkillsFont, Size.Empty, Format)).Select(
                                                 groupNameSize => groupNameSize.Width)).Concat(new[] { 0 }).Max();

                // Scroll through groups
                lbSkills.Items.Clear();
                foreach (IGrouping<SkillGroup, Skill> group in groups)
                {
                    lbSkills.Items.Add(group.Key);

                    // Add items in the group when it's not collapsed
                    if (Character.UISettings.CollapsedGroups.Contains(group.Key.Name))
                        continue;

                    foreach (Skill skill in group.OrderBy(x => x.Name))
                    {
                        lbSkills.Items.Add(skill);
                    }
                }

                // Display or hide the "no skills" label.
                noSkillsLabel.Visible = !skills.Any();
                lbSkills.Visible = skills.Any();

                // Invalidate display
                lbSkills.Invalidate();
            }
            finally
            {
                lbSkills.EndUpdate();
                lbSkills.TopIndex = scrollBarPosition;
            }
        }

        /// <summary>
        /// Gets a characters skills, filtered by MainWindow settings
        /// </summary>
        /// <returns>
        /// IEnumerable of <see cref="Skill"/> Skill.
        /// </returns>
        private IEnumerable<Skill> GetCharacterSkills()
        {
            if (Settings.UI.MainWindow.ShowPrereqMetSkills)
                return Character.Skills.Where(x => x.IsKnown || (x.ArePrerequisitesMet && x.IsPublic));

            if (Settings.UI.MainWindow.ShowNonPublicSkills)
                return Character.Skills;

            return Settings.UI.MainWindow.ShowAllPublicSkills
                       ? Character.Skills.Where(x => x.IsKnown || x.IsPublic)
                       : Character.Skills.Where(x => x.IsKnown);
        }

        #endregion


        #region Drawing

        /// <summary>
        /// Handles the DrawItem event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void lbSkills_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lbSkills.Items.Count)
                return;

            object item = lbSkills.Items[e.Index];
            SkillGroup skillGroup = item as SkillGroup;
            if (skillGroup != null)
                DrawItem(skillGroup, e);
            else
                DrawItem((Skill)item, e);
        }

        /// <summary>
        /// Handles the MeasureItem event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MeasureItemEventArgs"/> instance containing the event data.</param>
        private void lbSkills_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            e.ItemHeight = GetItemHeight(lbSkills.Items[e.Index]);
        }

        /// <summary>
        /// Gets the item's height.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private int GetItemHeight(object item)
        {
            if (item is SkillGroup)
                return SkillHeaderHeight;

            return Math.Max(m_skillsFont.Height * 2 + PadTop * 2, SkillDetailHeight);
        }

        /// <summary>
        /// Draws the list item for the given skill
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="e"></param>
        private void DrawItem(Skill skill, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw background
            DrawBackground(skill, e);

            // Measure texts
            int skillPointsToNextLevel = skill.StaticData.GetPointsRequiredForLevel(Math.Min(skill.Level + 1, 5));

            string rankText = String.Format(CultureConstants.DefaultCulture, " (Rank {0})", skill.Rank);
            string spText = String.Format(CultureConstants.DefaultCulture,
                                          "SP: {0:N0}/{1:N0}", skill.SkillPoints, skillPointsToNextLevel);
            string levelText = String.Format(CultureConstants.DefaultCulture, "Level {0}", skill.Level);
            string pctText = String.Format(CultureConstants.DefaultCulture, "{0}% Done", Math.Floor(skill.PercentCompleted));

            Size skillNameSize = TextRenderer.MeasureText(g, skill.Name, m_boldSkillsFont, Size.Empty, Format);
            Size rankTextSize = TextRenderer.MeasureText(g, rankText, m_skillsFont, Size.Empty, Format);
            Size levelTextSize = TextRenderer.MeasureText(g, levelText, m_skillsFont, Size.Empty, Format);
            Size spTextSize = TextRenderer.MeasureText(g, spText, m_skillsFont, Size.Empty, Format);
            Size pctTextSize = TextRenderer.MeasureText(g, pctText, m_skillsFont, Size.Empty, Format);


            // Draw texts
            Color highlightColor = Color.Black;
            if (!skill.IsKnown)
                highlightColor = Color.Red;

            if (!skill.IsPublic)
                highlightColor = Color.DarkRed;

            if (skill.ArePrerequisitesMet && skill.IsPublic && !skill.IsKnown)
                highlightColor = Color.SlateGray;

            if (Settings.UI.MainWindow.HighlightPartialSkills && skill.IsPartiallyTrained && !skill.IsTraining)
                highlightColor = Color.Green;

            if (Settings.UI.MainWindow.HighlightQueuedSkills && skill.IsQueued && !skill.IsTraining)
                highlightColor = Color.RoyalBlue;

            TextRenderer.DrawText(g, skill.Name, m_boldSkillsFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft,
                                      e.Bounds.Top + PadTop,
                                      skillNameSize.Width + PadLeft,
                                      skillNameSize.Height), highlightColor);

            TextRenderer.DrawText(g, rankText, m_skillsFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft + skillNameSize.Width,
                                      e.Bounds.Top + PadTop,
                                      rankTextSize.Width + PadLeft,
                                      rankTextSize.Height), highlightColor);

            TextRenderer.DrawText(g, spText, m_skillsFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft,
                                      e.Bounds.Top + PadTop + skillNameSize.Height,
                                      spTextSize.Width + PadLeft,
                                      spTextSize.Height), highlightColor);

            // Boxes
            DrawBoxes(skill, e);

            // Draw progression bar
            DrawProgressionBar(skill, e);


            // Draw level and percent texts
            TextRenderer.DrawText(g, levelText, m_skillsFont,
                                  new Rectangle(
                                      e.Bounds.Right - BoxWidth - PadRight - BoxHPad - levelTextSize.Width,
                                      e.Bounds.Top + PadTop, levelTextSize.Width + PadRight,
                                      levelTextSize.Height), Color.Black);

            TextRenderer.DrawText(g, pctText, m_skillsFont,
                                  new Rectangle(
                                      e.Bounds.Right - BoxWidth - PadRight - BoxHPad - pctTextSize.Width,
                                      e.Bounds.Top + PadTop + levelTextSize.Height,
                                      pctTextSize.Width + PadRight, pctTextSize.Height), Color.Black);
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private static void DrawBackground(Skill skill, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            if (skill.IsTraining)
            {
                // In training
                g.FillRectangle(Brushes.LightSteelBlue, e.Bounds);
            }
            else if ((e.Index % 2) == 0)
            {
                // Not in training - odd
                g.FillRectangle(Brushes.White, e.Bounds);
            }
            else
            {
                // Not in training - even
                g.FillRectangle(Brushes.LightGray, e.Bounds);
            }
        }

        /// <summary>
        /// Draws the progression bar.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private static void DrawProgressionBar(Skill skill, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(Pens.Black, new Rectangle(e.Bounds.Right - BoxWidth - PadRight,
                                                      e.Bounds.Top + PadTop + BoxHeight + BoxVPad,
                                                      BoxWidth, LowerBoxHeight));

            Rectangle pctBarRect = new Rectangle(e.Bounds.Right - BoxWidth - PadRight + 2,
                                                 e.Bounds.Top + PadTop + BoxHeight + BoxVPad + 2,
                                                 BoxWidth - 3, LowerBoxHeight - 3);

            g.FillRectangle(Brushes.DarkGray, pctBarRect);
            int fillWidth = (int)(pctBarRect.Width * skill.FractionCompleted);
            if (fillWidth <= 0)
                return;

            Rectangle fillRect = new Rectangle(pctBarRect.X, pctBarRect.Y, fillWidth, pctBarRect.Height);
            g.FillRectangle(Brushes.Black, fillRect);
        }

        /// <summary>
        /// Draws the boxes.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void DrawBoxes(Skill skill, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(Pens.Black,
                            new Rectangle(e.Bounds.Right - BoxWidth - PadRight, e.Bounds.Top + PadTop, BoxWidth, BoxHeight));

            const int LevelBoxWidth = (BoxWidth - 4 - 3) / 5;
            for (int level = 1; level <= 5; level++)
            {
                Rectangle brect = new Rectangle(
                    e.Bounds.Right - BoxWidth - PadRight + 2 + (LevelBoxWidth * (level - 1)) + (level - 1),
                    e.Bounds.Top + PadTop + 2, LevelBoxWidth, BoxHeight - 3);

                g.FillRectangle(level <= skill.Level ? Brushes.Black : Brushes.DarkGray, brect);

                // Color indicator for a queued level
                CCPCharacter ccpCharacter = Character as CCPCharacter;
                if (ccpCharacter != null)
                {
                    SkillQueue skillQueue = ccpCharacter.SkillQueue;
                    if (skillQueue.Any(qskill =>
                                       (!skill.IsTraining && skill == qskill.Skill && level == qskill.Level) ||
                                       (skill.IsTraining && skill == qskill.Skill && level == qskill.Level &&
                                        level > skill.Level + 1)))
                        g.FillRectangle(Brushes.RoyalBlue, brect);
                }

                // Blinking indicator of skill in training level
                if (!skill.IsTraining || level != skill.Level + 1)
                    continue;

                if (m_count == 0)
                    g.FillRectangle(Brushes.White, brect);

                if (m_count == 1)
                    m_count = -1;

                m_count++;
            }
        }

        /// <summary>
        /// Draws the list item for the given skill group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void DrawItem(SkillGroup group, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draws the background
            using (Brush brush = Settings.UI.SafeForWork
                                     ? new SolidBrush(Color.FromArgb(75, 75, 75))
                                     : (Brush)
                                       new LinearGradientBrush(new PointF(0F, 0F), new PointF(0F, 21F),
                                                               Color.FromArgb(75, 75, 75), Color.FromArgb(25, 25, 25)))
            {
                g.FillRectangle(brush, e.Bounds);
            }

            using (Pen pen = new Pen(Color.FromArgb(100, 100, 100)))
            {
                g.DrawLine(pen, e.Bounds.Left, e.Bounds.Top, e.Bounds.Right + 1, e.Bounds.Top);
            }

            // Measure Texts
            string skillInTrainingSuffix = String.Empty;
            string skillsInQueueSuffix = String.Empty;
            bool hasTrainingSkill = group.Any(x => x.IsTraining);
            bool hasQueuedSkill = group.Any(x => x.IsQueued && !x.IsTraining);
            if (hasTrainingSkill)
                skillInTrainingSuffix = "( 1 in training )";
            if (hasQueuedSkill)
            {
                skillsInQueueSuffix = String.Format(CultureConstants.DefaultCulture, "( {0} in queue )",
                                                    group.Count(x => x.IsQueued && !x.IsTraining));
            }

            string skillsSummaryText = String.Format(CultureConstants.DefaultCulture, "{0} of {1} skills",
                                                     group.Count(x => x.IsKnown), group.Count(x => x.IsPublic));

            string skillsTotalSPText = String.Format(CultureConstants.DefaultCulture, "{0:N0} Points", group.TotalSP);

            Rectangle skillGroupNameTextRect = new Rectangle(e.Bounds.Left + PadLeft,
                                                             e.Bounds.Top + (e.Bounds.Height / 2) - (lbSkills.ItemHeight / 2),
                                                             m_maxGroupNameWidth + (PadLeft / 2),
                                                             lbSkills.ItemHeight);

            int skillsSummaryTextWidth = (int)(SkillsSummaryTextWidth * (g.DpiX / EveMonClient.DefaultDpi));
            Rectangle skillsSummaryTextRect = new Rectangle(
                skillGroupNameTextRect.X + m_maxGroupNameWidth + (PadLeft / 2), skillGroupNameTextRect.Y,
                skillsSummaryTextWidth + (PadLeft / 2), lbSkills.ItemHeight);

            int skillGroupTotalSPTextWidth = (int)(SkillGroupTotalSPTextWidth * (g.DpiX / EveMonClient.DefaultDpi));
            Rectangle skillsTotalSPTextRect = new Rectangle(
                skillsSummaryTextRect.X + skillsSummaryTextWidth + (PadLeft / 2), skillGroupNameTextRect.Y,
                skillGroupTotalSPTextWidth + (PadLeft / 2), lbSkills.ItemHeight);

            Size skillInTrainingSuffixSize = TextRenderer.MeasureText(g, skillInTrainingSuffix, m_skillsFont, Size.Empty);
            Rectangle skillInTrainingSuffixRect = new Rectangle(
                skillsTotalSPTextRect.X + skillGroupTotalSPTextWidth + (PadLeft / 2), skillGroupNameTextRect.Y,
                skillInTrainingSuffixSize.Width, lbSkills.ItemHeight);

            Size skillQueueTextSize = TextRenderer.MeasureText(g, skillsInQueueSuffix, m_skillsFont, Size.Empty);
            Rectangle skillQueueRect = new Rectangle(
                skillInTrainingSuffixRect.X + skillInTrainingSuffixSize.Width, skillInTrainingSuffixRect.Y,
                skillQueueTextSize.Width, lbSkills.ItemHeight);

            // Draw the header
            TextRenderer.DrawText(g, group.Name, m_boldSkillsFont, skillGroupNameTextRect, Color.White, Format);
            TextRenderer.DrawText(g, skillsSummaryText, m_skillsFont, skillsSummaryTextRect, Color.White,
                                  Format | TextFormatFlags.Right);
            TextRenderer.DrawText(g, skillsTotalSPText, m_skillsFont, skillsTotalSPTextRect, Color.White,
                                  Format | TextFormatFlags.Right);
            TextRenderer.DrawText(g, skillInTrainingSuffix, m_skillsFont, skillInTrainingSuffixRect, Color.White,
                                  Format | TextFormatFlags.Right);
            TextRenderer.DrawText(g, skillsInQueueSuffix, m_skillsFont, skillQueueRect, (Settings.UI.SafeForWork
                                                                                             ? Color.White
                                                                                             : Color.Yellow),
                                  Format | TextFormatFlags.Right);

            // Draws the collapsing arrows
            bool isCollapsed = Character.UISettings.CollapsedGroups.Contains(group.Name);
            Image image = (isCollapsed ? Resources.Expand : Resources.Collapse);

            g.DrawImageUnscaled(image, new Point(e.Bounds.Right - image.Width - CollapserPadRight,
                                                 (SkillHeaderHeight / 2) - (image.Height / 2) + e.Bounds.Top));
        }

        /// <summary>
        /// Gets the preferred size from the preferred size of the skills list.
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            return lbSkills.GetPreferredSize(proposedSize);
        }

        #endregion


        #region Toggle / expand

        /// <summary>
        /// Toggles all the skill groups to collapse or open.
        /// </summary>
        public void ToggleAll()
        {
            // When at least one group collapsed, expand all
            if (Character.UISettings.CollapsedGroups.Count != 0)
                Character.UISettings.CollapsedGroups.Clear();
                // When none collapsed, collapse all
            else
            {
                foreach (SkillGroup group in Character.SkillGroups)
                {
                    Character.UISettings.CollapsedGroups.Add(group.Name);
                }
            }

            // Update the list
            UpdateContent();
        }

        /// <summary>
        /// Toggles the expansion or collapsing of a single group
        /// </summary>
        /// <param name="group">The group to expand or collapse.</param>
        private void ToggleGroupExpandCollapse(SkillGroup group)
        {
            if (Character.UISettings.CollapsedGroups.Contains(group.Name))
                ExpandSkillGroup(group);
            else
                CollapseSkillGroup(group);
        }

        /// <summary>
        /// Expand a skill group
        /// </summary>
        /// <param name="group">Skill group in lbSkills</param>
        private void ExpandSkillGroup(SkillGroup group)
        {
            Character.UISettings.CollapsedGroups.Remove(group.Name);
            UpdateContent();
        }

        /// <summary>
        /// Collapse a skill group
        /// </summary>
        /// <param name="group">Skill group in lbSkills</param>
        private void CollapseSkillGroup(SkillGroup group)
        {
            Character.UISettings.CollapsedGroups.Add(group.Name);
            UpdateContent();
        }

        #endregion


        #region Local events

        /// <summary>
        /// Handles the MouseWheel event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbSkills_MouseWheel(object sender, MouseEventArgs e)
        {
            // Update the drawing based upon the mouse wheel scrolling
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
                    if (lbSkills.TopIndex - i >= 0)
                        item = lbSkills.Items[lbSkills.TopIndex - i];
                }
                    // Going down
                else
                {
                    // Compute the height of the items from current the topindex (included)
                    int height = 0;
                    for (int j = lbSkills.TopIndex + i - 1; j < lbSkills.Items.Count; j++)
                    {
                        height += GetItemHeight(lbSkills.Items[j]);
                    }

                    // Retrieve the next bottom item
                    if (height > lbSkills.ClientSize.Height)
                        item = lbSkills.Items[lbSkills.TopIndex + i - 1];
                }

                // If found a new item as top or bottom
                if (item != null)
                    numberOfPixelsToMove[i - 1] = GetItemHeight(item) * direction;
                else
                    lines -= direction;
            }

            // Scroll 
            if (lines != 0)
                lbSkills.Invalidate();
        }

        /// <summary>
        /// Handles the MouseDown event of the lbSkills control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbSkills_MouseDown(object sender, MouseEventArgs e)
        {
            // Retrieve the item at the given point and quit if none
            int index = lbSkills.IndexFromPoint(e.X, e.Y);
            if (index < 0 || index >= lbSkills.Items.Count)
                return;

            // Beware, this last index may actually means a click in the whitespace at the bottom
            // Let's deal with this special case
            if (index == lbSkills.Items.Count - 1)
            {
                Rectangle itemRect = lbSkills.GetItemRectangle(index);
                if (!itemRect.Contains(e.Location))
                    return;
            }

            // For a skill group, we have to handle the collapse/expand mechanism and the tooltip
            Object item = lbSkills.Items[index];
            SkillGroup skillGroup = item as SkillGroup;
            if (skillGroup != null)
            {
                // Left button : expand/collapse
                if (e.Button != MouseButtons.Right)
                {
                    ToggleGroupExpandCollapse(skillGroup);
                    return;
                }

                // If right click on the button, still expand/collapse
                Rectangle itemRect = lbSkills.GetItemRectangle(lbSkills.Items.IndexOf(item));
                Rectangle buttonRect = GetButtonRectangle(skillGroup, itemRect);
                if (buttonRect.Contains(e.Location))
                {
                    ToggleGroupExpandCollapse(skillGroup);
                    return;
                }

                // Regular right click, display the tooltip
                DisplayTooltip(skillGroup);
                return;
            }

            // Right click for skills below lv5 : we display a context menu to plan higher levels
            Skill skill = (Skill)item;
            if (e.Button == MouseButtons.Right)
            {
                // Build the context menu
                BuildContextMenu(skill);

                // Display the context menu
                contextMenuStripPlanPopup.Show((Control)sender, new Point(e.X, e.Y));
                return;
            }

            // Non-right click or already lv5, display the tooltip
            DisplayTooltip(skill);
        }

        /// <summary>
        /// On mouse move, we show the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSkills_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < lbSkills.Items.Count; i++)
            {
                // Skip until we found the mouse location
                Rectangle rect = lbSkills.GetItemRectangle(i);
                if (!rect.Contains(e.Location))
                    continue;

                // Updates the tooltip
                Object item = lbSkills.Items[i];
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
                int nextLevel = Math.Min(5, skill.Level + 1);
                for (int level = nextLevel; level < 6; level++)
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
                                                      menuPlanItem.Tag = new Pair<Plan, SkillLevel>(plan,
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
        }

        /// <summary>
        /// Displays the tooltip for the given item (skill or skillgroup).
        /// </summary>
        /// <param name="item"></param>
        private void DisplayTooltip(Object item)
        {
            if (ttToolTip.Active && m_lastTooltipItem != null && m_lastTooltipItem == item)
                return;

            m_lastTooltipItem = item;

            ttToolTip.Active = false;
            SkillGroup skillGroup = item as SkillGroup;
            ttToolTip.SetToolTip(lbSkills, skillGroup != null ? GetTooltip(skillGroup) : GetTooltip(item as Skill));
            ttToolTip.Active = true;
        }

        /// <summary>
        /// Gets the tooltip text for the given skill
        /// </summary>
        /// <param name="skill"></param>
        private static string GetTooltip(Skill skill)
        {
            int sp = skill.SkillPoints;
            int nextLevel = Math.Min(5, skill.Level + 1);
            int nextLevelSP = skill.StaticData.GetPointsRequiredForLevel(nextLevel);
            int pointsLeft = skill.GetLeftPointsRequiredToLevel(nextLevel);
            string remainingTimeText = skill.GetLeftTrainingTimeToLevel(nextLevel).ToDescriptiveText(
                DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.UppercaseText);

            if (sp < skill.StaticData.GetPointsRequiredForLevel(1))
            {
                // Training hasn't got past level 1 yet
                StringBuilder untrainedToolTip = new StringBuilder();
                untrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                              "Not yet trained to Level I ({0}%)\n", Math.Floor(skill.PercentCompleted));
                untrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                              "Next level I: {0:N0} skill points remaining\n", pointsLeft);
                untrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                              "Training time remaining: {0}", remainingTimeText);
                AddSkillBoilerPlate(untrainedToolTip, skill);
                return untrainedToolTip.ToString();
            }

            // So, it's a left click on a skill, we display the tool tip
            // Partially trained skill ?
            if (skill.IsPartiallyTrained)
            {
                StringBuilder partiallyTrainedToolTip = new StringBuilder();
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                     "Partially Completed ({0}%)\n", Math.Floor(skill.PercentCompleted));
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                     "Training to level {0}: {1:N0} skill points remaining\n",
                                                     Skill.GetRomanFromInt(nextLevel), pointsLeft);
                partiallyTrainedToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                     "Training time remaining: {0}", remainingTimeText);
                AddSkillBoilerPlate(partiallyTrainedToolTip, skill);
                return partiallyTrainedToolTip.ToString();
            }

            // We've completed all the skill points for the current level
            if (!skill.IsPartiallyTrained)
            {
                if (skill.Level != 5)
                {
                    StringBuilder levelCompleteToolTip = new StringBuilder();
                    levelCompleteToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                      "Completed Level {0}: {1:N0}/{2:N0}\n",
                                                      Skill.GetRomanFromInt(skill.Level), sp, nextLevelSP);
                    levelCompleteToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                      "Next level {0}: {1:N0} skill points required\n",
                                                      Skill.GetRomanFromInt(nextLevel), pointsLeft);
                    levelCompleteToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                      "Training Time: {0}", remainingTimeText);
                    AddSkillBoilerPlate(levelCompleteToolTip, skill);
                    return levelCompleteToolTip.ToString();
                }

                // Lv 5 completed
                StringBuilder lv5ToolTip = new StringBuilder();
                lv5ToolTip.AppendFormat(CultureConstants.DefaultCulture, "Level V Complete: {0:N0}/{1:N0}\n", sp,
                                        nextLevelSP);
                lv5ToolTip.Append("No further training required\n");
                AddSkillBoilerPlate(lv5ToolTip, skill);
                return lv5ToolTip.ToString();
            }

            // Error in calculating SkillPoints
            StringBuilder calculationErrorToolTip = new StringBuilder();
            calculationErrorToolTip.AppendLine("Partially Trained (Could not cacluate all skill details)");
            calculationErrorToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                 "Next level {0}: {1:N0} skill points remaining\n", nextLevel, pointsLeft);
            calculationErrorToolTip.AppendFormat(CultureConstants.DefaultCulture,
                                                 "Training time remaining: {0}", remainingTimeText);
            AddSkillBoilerPlate(calculationErrorToolTip, skill);
            return calculationErrorToolTip.ToString();
        }

        /// <summary>
        /// Adds the skill boiler plate.
        /// </summary>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="skill">The skill.</param>
        private static void AddSkillBoilerPlate(StringBuilder toolTip, Skill skill)
        {
            toolTip.Append("\n\n");
            toolTip.AppendLine(skill.DescriptionNL);
            toolTip.AppendFormat(CultureConstants.DefaultCulture, "\nPrimary: {0}, ", skill.PrimaryAttribute);
            toolTip.AppendFormat(CultureConstants.DefaultCulture, "Secondary: {0} ", skill.SecondaryAttribute);
            toolTip.AppendFormat(CultureConstants.DefaultCulture, "({0:N0} SP/hour)", skill.SkillPointsPerHour);
        }

        /// <summary>
        /// Gets the tooltip content for the given skill group
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        private static string GetTooltip(SkillGroup group)
        {
            // Maximas are computed on public skills only
            int totalValidSP = group.Where(x => x.IsPublic).Sum(x => x.SkillPoints);
            int maxSP = group.Where(x => x.IsPublic).Sum(x => x.StaticData.GetPointsRequiredForLevel(5));
            int maxKnown = group.Count(x => x.IsPublic);

            // Current achievements are computed on every skill, including non-public
            int totalSP = group.Sum(x => x.SkillPoints);
            int known = group.Count(x => x.IsKnown);

            // If the group is not completed yet
            if (totalValidSP < maxSP)
            {
                double percentDonePoints = (1.0 * Math.Min(totalSP, maxSP)) / maxSP;
                double percentDoneSkills = (1.0 * Math.Min(known, maxKnown)) / maxKnown;

                return String.Format(CultureConstants.DefaultCulture,
                                     "Points Completed: {0:N0} of {1:N0} ({2:P1})\nSkills Known: {3} of {4} ({5:P0})",
                                     totalSP, maxSP, percentDonePoints, known, maxKnown, percentDoneSkills);
            }

            // The group has been completed !
            return String.Format(CultureConstants.DefaultCulture,
                                 "Skill Group completed: {0:N0}/{1:N0} (100%)\nSkills: {2}/{3} (100%)",
                                 totalSP, maxSP, known, maxKnown);
        }

        /// <summary>
        /// Gets the rectangle for the collapse/expand button.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="itemRect">The item rect.</param>
        /// <returns></returns>
        private Rectangle GetButtonRectangle(SkillGroup group, Rectangle itemRect)
        {
            // Checks whether this group is collapsed
            bool isCollapsed = Character.UISettings.CollapsedGroups.Contains(group.Name);

            // Get the image for this state
            Image btnImage = (isCollapsed ? Resources.Expand : Resources.Collapse);

            // Compute the top left point
            Point btnPoint = new Point(itemRect.Right - btnImage.Width - CollapserPadRight,
                                       (SkillHeaderHeight / 2) - (btnImage.Height / 2) + itemRect.Top);

            return new Rectangle(btnPoint, btnImage.Size);
        }

        /// <summary>
        /// Handler for a plan item click on the plan's context menu.
        /// Add a skill to the plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void menuPlanItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem planItem = (ToolStripMenuItem)sender;
            Pair<Plan, SkillLevel> tag = (Pair<Plan, SkillLevel>)planItem.Tag;

            IPlanOperation operation = tag.A.TryPlanTo(tag.B.Skill, tag.B.Level);
            PlanHelper.SelectPerform(operation);
        }

        /// <summary>
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
        /// On timer tick, we invalidate the training skill display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            // We trigger a refresh of the list to eliminate a designer leftover (scrollbar)
            if (m_requireRefresh)
            {
                lbSkills.Invalidate();
                m_requireRefresh = false;
            }

            if (!Character.IsTraining || !Visible)
                return;

            // Retrieves the trained skill for update but quit if the skill is null (was not in our datafiles)
            QueuedSkill training = Character.CurrentlyTrainingSkill;
            if (training.Skill == null)
                return;

            // Invalidate the skill row
            int index = lbSkills.Items.IndexOf(training.Skill);
            if (index >= 0)
                lbSkills.Invalidate(lbSkills.GetItemRectangle(index));

            // Invalidate the skill group row
            int groupIndex = lbSkills.Items.IndexOf(training.Skill.Group);
            if (groupIndex >= 0)
                lbSkills.Invalidate(lbSkills.GetItemRectangle(groupIndex));
        }

        /// <summary>
        /// When the character changed, we refresh the content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != Character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// When the settings changed, we refresh the content (show all public skills, non-public skills, etc)
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