using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class SkillTreeDisplay : UserControl
    {
        public SkillTreeDisplay()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void MyDispose(bool disposing)
        {
            if (disposing)
            {
                // This detaches changed handlers...
                ResetLayoutData();
            }
        }

        private Plan m_plan = null;

        public Plan Plan
        {
            get { return m_plan; }
            set { m_plan = value; }
        }

        private Skill m_rootSkill = null;

        public Skill RootSkill
        {
            get { return m_rootSkill; }
            set
            {
                if (m_rootSkill != value)
                {
                    m_rootSkill = value;
                    BuildTree();
                }
            }
        }

        private bool m_worksafeMode = false;

        public bool WorksafeMode
        {
            get { return m_worksafeMode; }
            set
            {
                if (m_worksafeMode != value)
                {
                    m_worksafeMode = value;
                    this.Invalidate();
                }
            }
        }

        private const int SKILLBOX_WIDTH = 220;
        private const int SKILLBOX_HEIGHT = 73;

        private const int SKILLBOX_MARGIN_UD = 20;
        private const int SKILLBOX_MARGIN_LR = 10;

        private class SkillInfo
        {
            private Skill m_skill;
            private int m_requiredLevel = -1;
            private SkillInfo m_parent;
            private int m_left;
            private Rectangle m_currentRectangle = Rectangle.Empty;

            public Skill Skill
            {
                get { return m_skill; }
            }

            public int RequiredLevel
            {
                get { return m_requiredLevel; }
                set { m_requiredLevel = value; }
            }

            public SkillInfo ParentSkillInfo
            {
                get { return m_parent; }
            }

            public int Left
            {
                get { return m_left; }
                set { m_left = value; }
            }

            public Rectangle CurrentRectangle
            {
                get { return m_currentRectangle; }
                set { m_currentRectangle = value; }
            }

            public SkillInfo(Skill gs, SkillInfo parent)
            {
                m_skill = gs;
                m_parent = parent;
            }

            internal void AddChild(SkillInfo si)
            {
                //throw new Exception("The method or operation is not implemented.");
            }
        }

        private List<List<SkillInfo>> m_layoutData = new List<List<SkillInfo>>();
        //private Dictionary<GrandSkill, SkillInfo> m_alreadyInLayout = null;

        private void BuildTree()
        {
            SetupTree();
            LayoutTree();
            LayoutLines();
            CalculateSize();
            this.Invalidate();
        }

        private void SetupTree()
        {
            ResetLayoutData();
            //m_alreadyInLayout = new Dictionary<GrandSkill, SkillInfo>();

            if (m_rootSkill != null)
            {
                List<SkillInfo> mainLevel = new List<SkillInfo>();
                m_layoutData.Add(mainLevel);
                SkillInfo rootSi = new SkillInfo(m_rootSkill, null);
                rootSi.Left = 0;
                mainLevel.Add(rootSi);
                //m_alreadyInLayout.Add(m_rootSkill, rootSi);

                BuildPrereqs(rootSi, 1);
            }
            AttachLayoutData();
        }

        private EventHandler m_eventChangeHandler;

        private void ResetLayoutData()
        {
            SetTrainingSkill(null);
            if (m_layoutData != null && m_eventChangeHandler != null)
            {
                foreach (List<SkillInfo> lsi in m_layoutData)
                {
                    foreach (SkillInfo si in lsi)
                    {
                        Skill gs = si.Skill;
                        gs.Changed -= m_eventChangeHandler;
                    }
                }
            }
            m_layoutData = new List<List<SkillInfo>>();
        }

        private void AttachLayoutData()
        {
            if (m_eventChangeHandler == null)
            {
                m_eventChangeHandler = new EventHandler(OnEventChanged);
            }

            foreach (List<SkillInfo> lsi in m_layoutData)
            {
                foreach (SkillInfo si in lsi)
                {
                    Skill gs = si.Skill;
                    gs.Changed += m_eventChangeHandler;
                    if (gs.InTraining)
                    {
                        SetTrainingSkill(gs);
                    }
                }
            }
        }

        private Skill m_trainingSkill = null;

        private void SetTrainingSkill(Skill gs)
        {
            if (gs != null && gs.InTraining)
            {
                if (m_trainingSkill != gs)
                {
                    m_trainingSkill = gs;
                    tmrSkillTick.Enabled = true;
                }
            }
            else if (gs == null || (!gs.InTraining && m_trainingSkill == gs))
            {
                m_trainingSkill = null;
                tmrSkillTick.Enabled = false;
            }
        }

        private bool m_inTick = false;

        private void OnEventChanged(object sender, EventArgs e)
        {
            Skill gs = sender as Skill;
            if (gs == null)
            {
                return;
            }

            this.Invoke(new MethodInvoker(delegate
                                              {
                                                  SetTrainingSkill(gs);
                                                  using (Region r = GetSkillRegion(gs))
                                                  {
                                                      if (m_inTick)
                                                      {
                                                          foreach (Skill sgs in m_skillsToUpdateOnTick)
                                                          {
                                                              using (Region sr = GetSkillRegion(sgs))
                                                              {
                                                                  r.Union(sr);
                                                              }
                                                          }
                                                      }
                                                      this.Invalidate(r);
                                                  }
                                              }));
        }

        private void tmrSkillTick_Tick(object sender, EventArgs e)
        {
            m_inTick = true;
            if (m_trainingSkill != null)
            {
                OnEventChanged(m_trainingSkill, new EventArgs());
            }
            m_inTick = false;
        }

        public Region GetSkillRegion(Skill gs)
        {
            Region r = new Region();
            r.MakeEmpty();
            try
            {
                foreach (List<SkillInfo> lsi in m_layoutData)
                {
                    foreach (SkillInfo si in lsi)
                    {
                        if (si.Skill == gs)
                        {
                            r.Union(si.CurrentRectangle);
                        }
                    }
                }
                return r;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogRethrowException(e);
                r.Dispose();
                throw;
            }
        }

        private void BuildPrereqs(SkillInfo parentSi, int level)
        {
            Skill parentSkill = parentSi.Skill;
            foreach (Skill.Prereq pp in parentSkill.Prereqs)
            {
                //if (!m_alreadyInLayout.ContainsKey(pp.Skill))
                //{
                if (m_layoutData.Count <= level)
                {
                    m_layoutData.Add(new List<SkillInfo>());
                }
                List<SkillInfo> thisLevel = m_layoutData[level];

                SkillInfo si = new SkillInfo(pp.Skill, parentSi);
                si.RequiredLevel = pp.Level;
                if (thisLevel.Count == 0)
                {
                    si.Left = -10;
                }
                else
                {
                    si.Left = thisLevel[thisLevel.Count - 1].Left + SKILLBOX_WIDTH + SKILLBOX_MARGIN_LR;
                }
                thisLevel.Add(si);
                //m_alreadyInLayout.Add(pp.Skill, si);
                parentSi.AddChild(si);
                if (si.Skill.Id != parentSi.Skill.Id)
                {
                    BuildPrereqs(si, level + 1);
                }
                //}
                //else
                //{
                //    parentSi.AddChild(m_alreadyInLayout[pp.Skill]);
                //}
            }
        }

        private void LayoutTree()
        {
            // Start at 1 because the root node is fixed
            for (int y = 1; y < m_layoutData.Count; y++)
            {
                List<SkillInfo> lsi = m_layoutData[y];
                for (int x = 0; x < lsi.Count; x++)
                {
                    SkillInfo thisSi = lsi[x];
                    SkillInfo parentSi = thisSi.ParentSkillInfo;
                    int myLeft = thisSi.Left;
                    int parentLeft = parentSi.Left;
                    int desiredMovement = parentLeft - myLeft;
                    int desiredLeft = myLeft + desiredMovement;
                    if (desiredMovement < 0)
                    {
                        // Want to move left
                        if (x > 0)
                        {
                            SkillInfo neighborBox = lsi[x - 1];
                            int neighborRight = neighborBox.Left + SKILLBOX_WIDTH + SKILLBOX_MARGIN_LR;
                            if (desiredLeft < neighborRight)
                            {
                                thisSi.Left = neighborRight;
                                List<SkillInfo> principals = new List<SkillInfo>();
                                principals.Add(neighborBox);
                                principals.Add(thisSi);
                                ResolveLayoutConflict(principals, y);
                            }
                            else
                            {
                                thisSi.Left = desiredLeft;
                            }
                        }
                        else
                        {
                            thisSi.Left = desiredLeft;
                        }
                    }
                    else if (desiredMovement > 0)
                    {
                        // Want to move right
                        if (x < lsi.Count - 1)
                        {
                            SkillInfo neighborBox = lsi[x + 1];
                            int desiredRight = desiredLeft + SKILLBOX_WIDTH;
                            int neighborLeft = neighborBox.Left - SKILLBOX_MARGIN_LR;
                            if (desiredRight > neighborLeft)
                            {
                                thisSi.Left = neighborLeft - SKILLBOX_WIDTH;
                                List<SkillInfo> principals = new List<SkillInfo>();
                                principals.Add(thisSi);
                                principals.Add(neighborBox);
                                ResolveLayoutConflict(principals, y);
                            }
                            else
                            {
                                thisSi.Left = desiredLeft;
                            }
                        }
                        else
                        {
                            thisSi.Left = desiredLeft;
                        }
                    }
                }
            }
            return;
        }

        private void ResolveLayoutConflict(List<SkillInfo> principals, int level)
        {
            // Two more more boxes are pressing against each other, find out where they
            // should end up. If they end up pressing against yet another box, we'll have to
            // recurse and resolve that conflict as well.

            int overallMovement = 0;
            foreach (SkillInfo si in principals)
            {
                SkillInfo parentSi = si.ParentSkillInfo;
                int myLeft = si.Left;
                int parentLeft = parentSi.Left;
                int desiredMovement = parentLeft - myLeft;
                overallMovement += desiredMovement;
            }
            overallMovement = overallMovement/principals.Count;

            if (overallMovement < 0)
            {
                // Moving left...
                SkillInfo leadSi = principals[0];
                int desiredLeft = leadSi.Left + overallMovement;
                SkillInfo neighborSi = null;
                for (int i = 0; i < m_layoutData[level].Count; i++)
                {
                    if (m_layoutData[level][i] == leadSi)
                    {
                        break;
                    }
                    neighborSi = m_layoutData[level][i];
                }
                if (neighborSi == null)
                {
                    foreach (SkillInfo si in principals)
                    {
                        si.Left += overallMovement;
                    }
                    return;
                }
                else
                {
                    int neighborRight = neighborSi.Left + SKILLBOX_WIDTH + SKILLBOX_MARGIN_LR;
                    if (desiredLeft < neighborRight)
                    {
                        int canMoveAmount = neighborRight - leadSi.Left;
                        foreach (SkillInfo si in principals)
                        {
                            si.Left += canMoveAmount;
                        }
                        principals.Insert(0, neighborSi);
                        ResolveLayoutConflict(principals, level);
                        return;
                    }
                    else
                    {
                        foreach (SkillInfo si in principals)
                        {
                            si.Left += overallMovement;
                        }
                        return;
                    }
                }
            }
            else if (overallMovement > 0)
            {
                // Moving right...
                SkillInfo leadSi = principals[principals.Count - 1];
                int desiredLeft = leadSi.Left + overallMovement;
                int desiredRight = desiredLeft + SKILLBOX_WIDTH + SKILLBOX_MARGIN_LR;
                SkillInfo neighborSi = null;
                bool useNext = false;
                for (int i = 0; i < m_layoutData[level].Count; i++)
                {
                    if (useNext)
                    {
                        neighborSi = m_layoutData[level][i];
                        break;
                    }
                    else if (m_layoutData[level][i] == leadSi)
                    {
                        useNext = true;
                    }
                }
                if (neighborSi == null)
                {
                    foreach (SkillInfo si in principals)
                    {
                        si.Left += overallMovement;
                    }
                    return;
                }
                else
                {
                    int neighborLeft = neighborSi.Left;
                    if (desiredRight > neighborLeft)
                    {
                        int canMoveAmount = overallMovement - (desiredRight - neighborLeft);
                        foreach (SkillInfo si in principals)
                        {
                            si.Left += canMoveAmount;
                        }
                        principals.Add(neighborSi);
                        ResolveLayoutConflict(principals, level);
                        return;
                    }
                    else
                    {
                        foreach (SkillInfo si in principals)
                        {
                            si.Left += overallMovement;
                        }
                        return;
                    }
                }
            }
        }

        private List<Rectangle> m_lines = new List<Rectangle>();

        private void LayoutLines()
        {
            m_lines.Clear();

            int level = 0;
            foreach (List<SkillInfo> lsi in m_layoutData)
            {
                foreach (SkillInfo si in lsi)
                {
                    SkillInfo parentSi = si.ParentSkillInfo;

                    if (parentSi != null)
                    {
                        Point lineFrom = new Point(si.Left + (SKILLBOX_WIDTH/2), level);
                        Point lineTo = new Point(parentSi.Left + (SKILLBOX_WIDTH/2), level - 1);
                        PlotLine(lineFrom, lineTo);
                    }
                }
                level++;
            }
        }

        private void PlotLine(Point lineFrom, Point lineTo)
        {
            //throw new Exception("The method or operation is not implemented.");
            Point actualFrom = new Point(lineFrom.X,
                                         lineFrom.Y*(SKILLBOX_HEIGHT + SKILLBOX_MARGIN_UD) + (SKILLBOX_HEIGHT/2));
            Point actualTo = new Point(lineTo.X,
                                       lineTo.Y*(SKILLBOX_HEIGHT + SKILLBOX_MARGIN_UD) + (SKILLBOX_HEIGHT/2));
            m_lines.Add(new Rectangle(actualFrom.X, actualFrom.Y,
                                      actualTo.X - actualFrom.X, actualTo.Y - actualFrom.Y));
        }

        private Rectangle m_graphBounds = new Rectangle(0, 0, 10, 10);

        private void CalculateSize()
        {
            int left = 0;
            int right = 0;
            foreach (List<SkillInfo> lsi in m_layoutData)
            {
                foreach (SkillInfo si in lsi)
                {
                    int myLeft = si.Left;
                    int myRight = si.Left + SKILLBOX_WIDTH;
                    if (myLeft < left)
                    {
                        left = myLeft;
                    }
                    if (myRight > right)
                    {
                        right = myRight;
                    }
                }
            }

            int hh = m_layoutData.Count*(SKILLBOX_HEIGHT);
            hh += (m_layoutData.Count - 1)*(SKILLBOX_MARGIN_UD);

            m_graphBounds = new Rectangle(left, 0, right - left, hh);
            this.AutoScrollMinSize = m_graphBounds.Size;
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            this.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.Invalidate();
        }

        private List<Skill> m_skillsToUpdateOnTick = new List<Skill>();

        private const DescriptiveTextOptions DTO_TIME =
            DescriptiveTextOptions.UppercaseText | DescriptiveTextOptions.IncludeCommas;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!m_worksafeMode)
            {
                using (Brush b = new LinearGradientBrush(
                    this.ClientRectangle, Color.LightBlue, Color.DarkBlue, 90.0F))
                {
                    e.Graphics.FillRectangle(b, e.ClipRectangle);
                }
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Control, e.ClipRectangle);
            }

            int ofsLeft = 0 - m_graphBounds.Left + this.AutoScrollPosition.X;
            int ofsTop = 0 - m_graphBounds.Top + this.AutoScrollPosition.Y;
            if (this.ClientSize.Width > m_graphBounds.Width)
            {
                int twHalf = this.ClientSize.Width/2;
                int gwHalf = m_graphBounds.Width/2;
                ofsLeft = (twHalf - gwHalf) - m_graphBounds.Left;
            }
            if (this.ClientSize.Height > m_graphBounds.Height)
            {
                int thHalf = this.ClientSize.Height/2;
                int ghHalf = m_graphBounds.Height/2;
                ofsTop = (thHalf - ghHalf) - m_graphBounds.Top;
            }

            Pen linePen;
            if (!m_worksafeMode)
            {
                linePen = new Pen(Color.White, 5.0F);
            }
            else
            {
                linePen = new Pen(SystemColors.ControlText, 5.0F);
            }
            try
            {
                foreach (Rectangle liner in m_lines)
                {
                    Point lfrom = new Point(liner.Left + ofsLeft, liner.Top + ofsTop);
                    Point lto = new Point(liner.Right + ofsLeft, liner.Bottom + ofsTop);
                    e.Graphics.DrawLine(linePen, lfrom, lto);
                }
            }
            finally
            {
                linePen.Dispose();
            }

            using (Font boldf = FontHelper.GetFont(this.Font, FontStyle.Bold))
            {
                int level = 0;
                m_skillsToUpdateOnTick.Clear();
                foreach (List<SkillInfo> lsi in m_layoutData)
                {
                    int ttop = (level*(SKILLBOX_HEIGHT + SKILLBOX_MARGIN_UD)) + ofsTop;
                    foreach (SkillInfo si in lsi)
                    {
                        Rectangle rect = new Rectangle(
                            si.Left + ofsLeft, ttop, SKILLBOX_WIDTH, SKILLBOX_HEIGHT);
                        si.CurrentRectangle = rect;

                        string currentLevelText = "Current Level: " + Skill.GetRomanForInt(si.Skill.Level);
                        Color stdTextColor = !m_worksafeMode ? Color.Black : SystemColors.ControlText;
                        Color reqTextColor = !m_worksafeMode ? Color.Red : SystemColors.GrayText;
                        string requiredLevel = null;
                        string thisRequiredTime = null;
                        string prereqTime = null;
                        Color prTextColor = !m_worksafeMode ? Color.Yellow : SystemColors.ControlText;
                        Brush fillBrush = null;

                        int plannedLevel = m_plan.PlannedLevel(si.Skill);
                        if (plannedLevel > 0)
                        {
                            currentLevelText += " (Planned To: " + Skill.GetRomanForInt(plannedLevel) + ")";
                        }

                        if (si.RequiredLevel > 0)
                        {
                            requiredLevel = "Required Level: " + Skill.GetRomanForInt(si.RequiredLevel);
                            if (si.RequiredLevel > si.Skill.Level)
                            {
                                TimeSpan ts = si.Skill.GetTrainingTimeToLevel(si.RequiredLevel);
                                thisRequiredTime = "This Time: " + Skill.TimeSpanToDescriptiveText(ts, DTO_TIME);
                                reqTextColor = !m_worksafeMode ? Color.Yellow : SystemColors.GrayText;
                                if (si.Skill.PrerequisitesMet)
                                {
                                    fillBrush = new LinearGradientBrush(rect, Color.LightPink, Color.DarkRed, 90.0F);
                                }
                                else
                                {
                                    fillBrush = new LinearGradientBrush(rect, Color.Red, Color.Black, 90.0F);
                                    stdTextColor = !m_worksafeMode ? Color.White : SystemColors.ControlText;
                                }
                            }
                            else
                            {
                                reqTextColor = !m_worksafeMode ? Color.Black : SystemColors.ControlText;
                                fillBrush = new LinearGradientBrush(rect, Color.LightSeaGreen, Color.DarkGreen, 90.0F);
                            }
                        }
                        else
                        {
                            if (si.Skill.PrerequisitesMet)
                            {
                                fillBrush = new LinearGradientBrush(rect, Color.LightBlue, Color.Blue, 90.0F);
                            }
                            else
                            {
                                fillBrush = new LinearGradientBrush(rect, Color.Blue, Color.Black, 90.0F);
                                stdTextColor = !m_worksafeMode ? Color.White : SystemColors.ControlText;
                            }
                        }

                        if (!si.Skill.PrerequisitesMet)
                        {
                            bool timeIncludesTraining = false;
                            TimeSpan pts = si.Skill.GetPrerequisiteTime(ref timeIncludesTraining);
                            if (pts > TimeSpan.Zero)
                            {
                                prereqTime = "Prerequisite: " +
                                             Skill.TimeSpanToDescriptiveText(pts, DTO_TIME);
                                if (timeIncludesTraining)
                                {
                                    m_skillsToUpdateOnTick.Add(si.Skill);
                                }
                            }
                        }

                        if (fillBrush == null)
                        {
                            fillBrush = new LinearGradientBrush(rect, Color.LightGray,
                                                                Color.DarkGray, 90.0F);
                        }
                        if (m_worksafeMode)
                        {
                            if (fillBrush != null)
                            {
                                fillBrush.Dispose();
                            }
                            fillBrush = new SolidBrush(SystemColors.Control);
                        }

                        e.Graphics.FillRectangle(fillBrush, rect);

                        fillBrush.Dispose();

                        Size sz;
                        Point drawPoint = new Point(rect.Left + 5, rect.Top + 5);
                        sz = MeasureAndDrawText(e.Graphics, si.Skill.Name, boldf,
                                                drawPoint, stdTextColor);
                        drawPoint.Y += sz.Height;

                        sz = MeasureAndDrawText(e.Graphics, currentLevelText, this.Font,
                                                drawPoint, stdTextColor);
                        drawPoint.Y += sz.Height;

                        if (!String.IsNullOrEmpty(requiredLevel))
                        {
                            sz = MeasureAndDrawText(e.Graphics, requiredLevel, this.Font,
                                                    drawPoint, reqTextColor);
                            drawPoint.Y += sz.Height;
                        }
                        if (!String.IsNullOrEmpty(thisRequiredTime))
                        {
                            sz = MeasureAndDrawText(e.Graphics, thisRequiredTime, this.Font,
                                                    drawPoint, reqTextColor);
                            drawPoint.Y += sz.Height;
                        }
                        if (!String.IsNullOrEmpty(prereqTime))
                        {
                            sz = MeasureAndDrawText(e.Graphics, prereqTime, this.Font,
                                                    drawPoint, prTextColor);
                            drawPoint.Y += sz.Height;
                        }


                        e.Graphics.DrawRectangle(!m_worksafeMode ? Pens.Black : SystemPens.ControlDarkDark, rect);
                    }
                    level++;
                }
            }
        }

        private Size MeasureAndDrawText(Graphics g, string text, Font f, Point p, Color c)
        {
            Size res = TextRenderer.MeasureText(g, text, f);

            // This line fails on Win2K for some reason
            //TextRenderer.DrawText(g, text, f, p, c);

            // ...but this one doesn't.  Freaky.
            TextRenderer.DrawText(g, text, f,
                                  new Rectangle(p.X, p.Y, res.Width, res.Height), c, Color.Transparent,
                                  TextFormatFlags.Default);
            return res;
        }

        public event SkillClickedHandler SkillClicked;

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            Skill clickedSkill = null;
            using (this.CreateGraphics())
            {
                foreach (List<SkillInfo> lsi in m_layoutData)
                {
                    if (clickedSkill == null)
                    {
                        foreach (SkillInfo si in lsi)
                        {
                            using (Region r = this.GetSkillRegion(si.Skill))
                            {
                                if (r.IsVisible(e.Location))
                                {
                                    clickedSkill = si.Skill;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (clickedSkill != null)
            {
                if (SkillClicked != null)
                {
                    SkillClickedEventArgs se = new SkillClickedEventArgs(clickedSkill, e.Button, e.Location);
                    SkillClicked(this, se);
                }

                NewPlannerWindow npw = m_plan.PlannerWindow.Target as NewPlannerWindow;
                npw.ShowSkillInTree(clickedSkill);
            }
        }
    }

    public delegate void SkillClickedHandler(object sender, SkillClickedEventArgs e);

    public class SkillClickedEventArgs : EventArgs
    {
        private Skill m_skill;
        private MouseButtons m_button;
        private Point m_location;

        internal SkillClickedEventArgs(Skill skill, MouseButtons button, Point location)
        {
            m_skill = skill;
            m_button = button;
            m_location = location;
        }

        public Skill Skill
        {
            get { return m_skill; }
        }

        public MouseButtons Button
        {
            get { return m_button; }
        }

        public Point Location
        {
            get { return m_location; }
        }

        public int X
        {
            get { return m_location.X; }
        }

        public int Y
        {
            get { return m_location.Y; }
        }
    }
}