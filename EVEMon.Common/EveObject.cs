using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    /// <summary>
    /// Superclass for ships/items and in the future, skills.
    /// This facilitates simpler code in the ship & item browsers, comparison tools and skill explorer
    /// Abstract because we'll be adding more stuff later to support the browsers
    /// </summary>
    public abstract class EveObject
    {
        protected int _id = -1;

        [XmlAttribute]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        protected string _icon = String.Empty;

        [XmlAttribute]
        public string Icon
        {
            get { return _icon; }
            set { _icon = String.Intern(value); }
        }

        protected string _name = String.Empty;

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected string _description = String.Empty;

        [XmlAttribute]
        public string Description
        {
            get { return _description; }
            set { _description = String.Intern(value); }
        }

        protected List<EntityRequiredSkill> _requiredSkills = new List<EntityRequiredSkill>();

        [XmlArrayItem("skill")]
        public List<EntityRequiredSkill> RequiredSkills
        {
            get { return _requiredSkills; }
        }

        protected List<EntityProperty> _properties = new List<EntityProperty>();

        [XmlArrayItem("prop")]
        public List<EntityProperty> Properties
        {
            get { return _properties; }
        }

        public override string ToString()
        {
            return _name;
        }

        public virtual string GetCategoryPath()
        {
            return "Undefined Category Path";
        }
    }
    #region Optimize the use of these
    // common classes used by ship and items - we can probably refactor code that
    // uses these classes (e.g. ship and item browsers
    /// <summary>
    /// Describes a property of a ship/item (e.g. CPU size)
    /// </summary>
    public class EntityProperty
    {
        protected string m_name;

        [XmlAttribute]
        public string Name
        {
            get { return m_name; }
            set { m_name = String.Intern(value); }
        }

        protected string m_value;

        [XmlAttribute]
        public string Value
        {
            get { return m_value.Trim(); }
            set { m_value = String.Intern(value); }
        }


        public override string ToString()
        {
            return m_name + ": " + m_value;
        }

    }

    /// <summary>
    /// Describes a skill required to use a ship or item.
    /// </summary>
    /// <remarks>
    /// N.B The list of required skills for a ship/item should only
    /// include the top level primary requirements.
    /// </remarks>
    public class EntityRequiredSkill
    {
        protected string _name;

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { _name = String.Intern(value); }
        }

        protected int _level;

        [XmlAttribute]
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public override string ToString()
        {
            return _name;
        }

    }
    #endregion
}
