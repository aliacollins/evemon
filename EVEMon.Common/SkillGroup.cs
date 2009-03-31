using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace EVEMon.Common
{
    public class SkillGroup : IEnumerable<Skill>
    {
        private string m_name;
        private int m_ID;
        private Dictionary<string, Skill> m_skills = new Dictionary<string, Skill>();

        public SkillGroup(StaticSkillGroup sgs, IEnumerable<Skill> _skills)
        {
            m_name = sgs.Name;
            m_ID = sgs.ID;

            foreach (Skill cs in _skills)
            {
                m_skills[cs.Name] = cs;
                cs.Changed += new EventHandler(gs_Changed);
                cs.SetSkillGroup(this);
            }
        }

        private int m_cachedKnownCount = -1;
        private int m_publicCount = -1;

        private void gs_Changed(object sender, EventArgs e)
        {
            m_cachedKnownCount = -1;
        }

        public int ID
        {
            get { return m_ID; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public Skill this[string name]
        {
            get
            {
                Skill result;
                m_skills.TryGetValue(name, out result);
                return result;
            }
        }

        public int Count
        {
            get { return m_skills.Count; }
        }

        public int KnownCount
        {
            get
            {
                if (m_cachedKnownCount == -1)
                {
                    m_cachedKnownCount = 0;
                    foreach (Skill gs in m_skills.Values)
                    {
                        if (gs.Known)
                        {
                            m_cachedKnownCount++;
                        }
                    }
                }
                return m_cachedKnownCount;
            }
        }

        public int PublicCount
        {
            get
            {
                if (m_publicCount == -1)
                {
                    m_publicCount = 0;
                    foreach (Skill gs in m_skills.Values)
                    {
                        if (gs.Public)
                        {
                            m_publicCount++;
                        }
                    }
                }
                return m_publicCount;
            }
        }

        public List<string> OwnedSkills()
        {
            List<string> os = new List<string>();
            foreach (Skill s in m_skills.Values)
            {
                if (s.Owned)
                {
                    if (s.Known) s.Owned = false;
                    else os.Add(s.Name);
                }
            }
            return os;
        }

        public bool Contains(string skillName)
        {
            return m_skills.ContainsKey(skillName);
        }

        public bool Contains(Skill gs)
        {
            return m_skills.ContainsValue(gs);
        }

        public int GetTotalPoints()
        {
            int result = 0;
            foreach (Skill gs in m_skills.Values)
            {
                result += gs.CurrentSkillPoints;
            }
            return result;
        }

        public int GetSkillsAtLevel(int level)
        {
            int result = 0;
            foreach (Skill s in m_skills.Values)
            {
                if (s.Level == level) result++;
            }
            return result;
        }

        #region IEnumerable<GrandSkill> Members
        public IEnumerator<Skill> GetEnumerator()
        {
            foreach (Skill gs in m_skills.Values)
            {
                yield return gs;
            }
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        internal void InsertSkill(Skill gs)
        {
            m_skills[gs.Name] = gs;
            gs.Changed += new EventHandler(gs_Changed);
            gs.SetSkillGroup(this);

            gs_Changed(this, new EventArgs());
        }

        #region Appearance in List box
        private static Image m_collapseImage;
        private static Image m_expandImage;
        private bool m_collapsed;

        public bool isCollapsed
        {
            get { return m_collapsed; }
            set
            {
                if (m_collapsed != value)
                {
                    m_collapsed = value;
                }
            }
        }

        public static Image CollapseImage
        {
            get
            {
                if (m_collapseImage == null)
                {
                    // Do not use a "using" block because Image.FromStream requires that
                    // the stream be left open.
                    Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "EVEMon.Common.Collapse_large.png");
                    m_collapseImage = Image.FromStream(s, true, true);
                }
                return m_collapseImage;
            }
        }

        public static Image ExpandImage
        {
            get
            {
                if (m_expandImage == null)
                {
                    // Do not use a "using" block because Image.FromStream requires that
                    // the stream be left open.
                    Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "EVEMon.Common.Expand_large.png");
                    m_expandImage = Image.FromStream(s, true, true);
                }
                return m_expandImage;
            }
        }

        private const int SKILL_HEADER_HEIGHT = 21;

        private const int SG_COLLAPSER_PAD_RIGHT = 6;

        public static int Height
        {
            get { return SKILL_HEADER_HEIGHT; }
        }

        public void Draw(DrawItemEventArgs e)
        {
            Font fontr = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            Graphics g = e.Graphics;
            bool hastrainingskill = false;
            foreach (Skill gs in m_skills.Values)
            {
                if (gs.Known)
                {
                    hastrainingskill = hastrainingskill || gs.InTraining;
                }
            }

            using (Brush b = new SolidBrush(Color.FromArgb(75, 75, 75)))
            {
                g.FillRectangle(b, e.Bounds);
            }
            using (Pen p = new Pen(Color.FromArgb(100, 100, 100)))
            {
                g.DrawLine(p, e.Bounds.Left, e.Bounds.Top, e.Bounds.Right + 1, e.Bounds.Top);
            }
            using (Font boldf = new Font(fontr, FontStyle.Bold))
            {
                Size titleSizeInt =
                    TextRenderer.MeasureText(g, this.Name, boldf, new Size(0, 0),
                                             TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);
                Point titleTopLeftInt = new Point(e.Bounds.Left + 3,
                                                  e.Bounds.Top + ((e.Bounds.Height / 2) - (titleSizeInt.Height / 2)));
                Point detailTopLeftInt = new Point(titleTopLeftInt.X + titleSizeInt.Width, titleTopLeftInt.Y);

                string trainingStr = String.Empty;
                if (hastrainingskill)
                {
                    trainingStr = ", ( 1 in training )";
                }
                string detailText = String.Format(", {0} of {1} skills, {2} Points{3}",
                                                  this.KnownCount,
                                                  this.PublicCount,
                                                  this.GetTotalPoints().ToString("#,##0"),
                                                  trainingStr);
                TextRenderer.DrawText(g, this.Name, boldf, titleTopLeftInt, Color.White);
                TextRenderer.DrawText(g, detailText, fontr, detailTopLeftInt, Color.White);
            }
            Image i;
            if (isCollapsed)
            {
                i = ExpandImage;
            }
            else
            {
                i = CollapseImage;
            }
            g.DrawImageUnscaled(i, new Point(e.Bounds.Right - i.Width - SG_COLLAPSER_PAD_RIGHT,
                                             (SKILL_HEADER_HEIGHT / 2) - (i.Height / 2) + e.Bounds.Top));
        }

        public Rectangle GetButtonRectangle(Rectangle itemRect)
        {
            Image btnImage;
            if (isCollapsed)
            {
                btnImage = ExpandImage;
            }
            else
            {
                btnImage = CollapseImage;
            }
            Size btnSize = btnImage.Size;
            Point btnPoint = new Point(itemRect.Right - btnImage.Width - SG_COLLAPSER_PAD_RIGHT,
                                       (SKILL_HEADER_HEIGHT / 2) - (btnImage.Height / 2) + itemRect.Top);
            return new Rectangle(btnPoint, btnSize);
        }
        #endregion
    }
}
