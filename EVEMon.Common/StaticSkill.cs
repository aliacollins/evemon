using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;


namespace EVEMon.Common
{
    public class StaticSkill : EveObject
    {
        private bool m_public;
        private string m_descriptionNl;
        private EveAttribute m_primaryAttribute;
        private EveAttribute m_secondaryAttribute;
        private int m_rank;
        private IEnumerable<Prereq> m_prereqs;
        private long m_cost;


        public StaticSkill(bool pub, string name, int id, string description,
                          EveAttribute a1, EveAttribute a2, int rank, long cost, IEnumerable<Prereq> prereqs)
        {
            m_public = pub;
            _name = name;
            _id = id;
            _description = description;
            m_descriptionNl = description;
            m_primaryAttribute = a1;
            m_secondaryAttribute = a2;
            m_rank = rank;
            m_prereqs = prereqs;
            m_cost = cost;
        }

        private static Dictionary<string, StaticSkill> m_skillsByName = new Dictionary<string, StaticSkill>();
        private static Dictionary<int, StaticSkill> m_skillsById = new Dictionary<int, StaticSkill>();

        public static StaticSkill GetStaticSkillByName(string name)
        {
            if (m_skillsByName.Count == 0)
            {
                LoadStaticSkills();
            }
            if (m_skillsByName.ContainsKey(name))
                return m_skillsByName[name];
            else
                return null;
        }

        public static StaticSkill GetStaticSkillById(int id)
        {
            if (m_skillsByName.Count == 0)
            {
                LoadStaticSkills();
            }
            if (m_skillsById.ContainsKey(id))
                return m_skillsById[id];
            else
                return null;
        }

        public static void ReloadStaticSkills()
        {
            m_skillsByName.Clear();
            m_skillsById.Clear();
            StaticSkillGroup.ResetSkillGroups();
            LoadStaticSkills();
        }


        public static void LoadStaticSkills()
        {
            if (m_skillsByName.Count > 0) return;

            string skillfile = String.Format(
                "{1}Resources{0}eve-skills2.xml.gz",
                Path.DirectorySeparatorChar,
                System.AppDomain.CurrentDomain.BaseDirectory);
            if (!File.Exists(skillfile))
            {
                throw new ApplicationException(skillfile + " not found!");
            }
            using (FileStream s = new FileStream(skillfile, FileMode.Open, FileAccess.Read))
            using (GZipStream zs = new GZipStream(s, CompressionMode.Decompress))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(zs);

                foreach (XmlElement sgel in xdoc.SelectNodes("/skills/c"))
                {
                    List<StaticSkill> skills = new List<StaticSkill>();
                    foreach (XmlElement sel in sgel.SelectNodes("s"))
                    {
                        string _name = sel.GetAttribute("n");

                        List<StaticSkill.Prereq> prereqs = new List<StaticSkill.Prereq>();
                        foreach (XmlElement pel in sel.SelectNodes("p"))
                        {
                            StaticSkill.Prereq p = new StaticSkill.Prereq(
                                pel.GetAttribute("n"),
                                Convert.ToInt32(pel.GetAttribute("l")));
                            prereqs.Add(p);
                        }

                        bool _pub = (sel.GetAttribute("p") != "false");
                        int _id = Convert.ToInt32(sel.GetAttribute("i"));
                        string _desc = sel.GetAttribute("d");
                        EveAttribute _primAttr =
                            (EveAttribute)Enum.Parse(typeof(EveAttribute), sel.GetAttribute("a1"), true);
                        EveAttribute _secAttr =
                            (EveAttribute)Enum.Parse(typeof(EveAttribute), sel.GetAttribute("a2"), true);
                        int _rank = 0;
                        string _srank = sel.GetAttribute("r");

                        try
                        {
                            _rank = Convert.ToInt32(_srank);
                        }
                        catch (FormatException fe)
                        {
                            throw new FormatException("Skill " + _name + ": rank string: " + _srank + " ", fe);
                        }
                        long _cost = 0;

                        // for a very very few users, this throws an exception on the skill "Salvage drone operation" - I have NO IDEA why.
                        // - Brad 9 May 2007
                        try
                        {
                            string cost = sel.GetAttribute("c");
                            _cost = Convert.ToInt64(cost);
                        }
                        catch (FormatException)
                        {
                            // Ignore the exception - cost is zero anyway.
                        }

                        StaticSkill ss = new StaticSkill(_pub, _name, _id, _desc, _primAttr, _secAttr, _rank, _cost, prereqs);
                        m_skillsById[_id] = ss;
                        m_skillsByName[_name] = ss;
                        skills.Add(ss);
                    }

                    string _group = sgel.GetAttribute("n");
                    int _number = Convert.ToInt32(sgel.GetAttribute("g"));
                    StaticSkillGroup ssg = new StaticSkillGroup(_group, _number, skills);
                }
            }

            // Fix up the prereqs
            foreach (StaticSkill s in m_skillsById.Values)
            {
                foreach (StaticSkill.Prereq pr in s.Prereqs)
                {
                    StaticSkill ss = m_skillsByName[pr.Name];
                    pr.SetSkill(ss);
                }
            }

        }

        private StaticSkillGroup m_skillGroup;

        /// <summary>
        /// Gets the skill group this skill is part of.
        /// </summary>
        public StaticSkillGroup SkillGroup
        {
            get { return m_skillGroup; }
        }

        internal void SetSkillGroup(StaticSkillGroup ssg)
        {
            if (m_skillGroup != null)
            {
                throw new InvalidOperationException("can only set skillgroup once");
            }

            m_skillGroup = ssg;
        }

        public bool Public
        {
            get { return m_public; }
        }

        public string DescriptionNl
        {
            //get { return m_descriptionNl.Replace(@".", ".\n"); }
            get { return WordWrap(m_descriptionNl, 100); }
        }

        public string WordWrap(string text, int maxLength)
        {
            text = text.Replace("\n", " ");
            text = text.Replace("\r", " ");
            text = text.Replace(".", ". ");
            text = text.Replace(">", "> ");
            text = text.Replace("\t", " ");
            text = text.Replace(",", ", ");
            text = text.Replace(";", "; ");

            string[] Words = text.Split(' ');
            int currentLineLength = 0;
            ArrayList Lines = new ArrayList(text.Length / maxLength);
            string currentLine = "";
            bool InTag = false;

            foreach (string currentWord in Words)
            {
                //ignore html
                if (currentWord.Length > 0)
                {
                    if (currentWord.Substring(0, 1) == "<")
                    {
                        InTag = true;
                    }
                    if (InTag)
                    {
                        //handle filenames inside html tags
                        if (currentLine.EndsWith("."))
                        {
                            currentLine += currentWord;
                        }
                        else
                        {
                            currentLine += " " + currentWord;
                        }
                        if (currentWord.IndexOf(">") > -1)
                        {
                            InTag = false;
                        }
                    }
                    else
                    {
                        if (currentLineLength + currentWord.Length + 1 < maxLength)
                        {
                            currentLine += " " + currentWord;
                            currentLineLength += (currentWord.Length + 1);
                        }
                        else
                        {
                            Lines.Add(currentLine);
                            currentLine = currentWord;
                            currentLineLength = currentWord.Length;
                        }
                    }
                }
            }
            if (currentLine != "")
            {
                Lines.Add(currentLine);
            }
            string[] textLinesStr = new string[Lines.Count];
            Lines.CopyTo(textLinesStr, 0);

            string strWrapped = "";
            foreach (string line in textLinesStr)
            {
                strWrapped += line + "\n";
            }

            return strWrapped;
        }

        public long Cost
        {
            get { return m_cost; }
        }

        public string FormattedCost
        {
            get
            {
                if (m_cost > 0)
                {
                    return String.Format("{0:0,0,0}", m_cost);
                }
                else
                {
                    return "0";
                }
            }
        }
        /// <summary>
        /// Gets the primary attribute of this skill.
        /// </summary>
        public EveAttribute PrimaryAttribute
        {
            get { return m_primaryAttribute; }
        }

        /// <summary>
        /// Gets the secondary attribute of this skill.
        /// </summary>
        public EveAttribute SecondaryAttribute
        {
            get { return m_secondaryAttribute; }
        }


        /// <summary>
        /// Gets the rank of this skill.
        /// </summary>
        public int Rank
        {
            get { return m_rank; }
        }

        /// <summary>
        /// Gets which attribute this skill modifies.
        /// </summary>
        public EveAttribute AttributeModified
        {
            get
            {
                if (_name == "Analytical Mind" || _name == "Logic")
                {
                    return EveAttribute.Intelligence;
                }
                else if (_name == "Empathy" || _name == "Presence")
                {
                    return EveAttribute.Charisma;
                }
                else if (_name == "Instant Recall" || _name == "Eidetic Memory")
                {
                    return EveAttribute.Memory;
                }
                else if (_name == "Iron Will" || _name == "Focus")
                {
                    return EveAttribute.Willpower;
                }
                else if (_name == "Spatial Awareness" || _name == "Clarity")
                {
                    return EveAttribute.Perception;
                }
                return EveAttribute.None;
            }
        }


        /// <summary>
        /// Calculates the points required for a level of this skill.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        public int GetPointsRequiredForLevel(int level)
        {
            if (level == 0)
            {
                return 0;
            }

            /* original formula is not accurate and required lots of tweaks. The new formula is accurate and matches other skill
               point formulae both on forums and in other skill tools */
            //int pointsForLevel = Convert.ToInt32(250*m_rank*Math.Pow(32, Convert.ToDouble(level - 1)/2));

            int pointsForLevel = Convert.ToInt32(Math.Ceiling(Math.Pow(2, (2.5 * level) - 2.5) * 250 * m_rank));
            return pointsForLevel;
        }


        public IEnumerable<Prereq> Prereqs
        {
            get
            {
                return m_prereqs;
            }
        }

        public class Prereq : EntityRequiredSkill
        {
            private StaticSkill m_pointedSkill;

            public StaticSkill Skill
            {
                get { return m_pointedSkill; }
            }

            internal void SetSkill(StaticSkill ss)
            {
                if (m_pointedSkill != null)
                {
                    throw new InvalidOperationException("pointed skill can only be set once");
                }
                m_pointedSkill = ss;
            }

            internal Prereq(string name, int requiredLevel)
            {
                _name = name;
                m_pointedSkill = null;
                _level = requiredLevel;
            }
        }

    }

}
