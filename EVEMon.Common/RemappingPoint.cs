using System;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a remapping operation attached to a plan entry
    /// </summary>
    public sealed class RemappingPoint
    {
        private readonly int[] m_attributes = new int[5];
        private string m_description = String.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RemappingPoint()
        {
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="serial"></param>
        public RemappingPoint(SerializableRemappingPoint serial)
        {
            if (serial == null)
                throw new ArgumentNullException("serial");

            Guid = Guid.NewGuid();
            m_attributes[(int)EveAttribute.Intelligence] = serial.Intelligence;
            m_attributes[(int)EveAttribute.Perception] = serial.Perception;
            m_attributes[(int)EveAttribute.Willpower] = serial.Willpower;
            m_attributes[(int)EveAttribute.Charisma] = serial.Charisma;
            m_attributes[(int)EveAttribute.Memory] = serial.Memory;
            m_description = serial.Description;
            Status = serial.Status;
        }

        /// <summary>
        /// Gets a global identified of this remapping point.
        /// </summary>
        [XmlIgnore]
        public Guid Guid { get; private set; }

        /// <summary>
        /// Gets the point's status (whether is has been initialized/computed or not).
        /// </summary>
        [XmlIgnore]
        public RemappingPointStatus Status { get; private set; }

        /// <summary>
        /// Gets the new base value for the given attribute.
        /// </summary>
        /// <param name="attrib"></param>
        /// <returns></returns>
        public int this[EveAttribute attrib]
        {
            get { return m_attributes[(int)attrib]; }
        }

        /// <summary>
        /// Gets a short string representation of the point ("i5 p7 c8 w9 m5").
        /// </summary>
        /// <returns></returns>
        private string ToShortString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("i").Append(m_attributes[(int)EveAttribute.Intelligence].ToString()).
                Append(" p").Append(m_attributes[(int)EveAttribute.Perception].ToString()).
                Append(" c").Append(m_attributes[(int)EveAttribute.Charisma].ToString()).
                Append(" w").Append(m_attributes[(int)EveAttribute.Willpower].ToString()).
                Append(" m").Append(m_attributes[(int)EveAttribute.Memory].ToString());

            return builder.ToString();
        }

        /// <summary>
        /// Gets a long string representation of this point. Two possible formats : 
        /// <list type="">
        /// <item>"Remapping (not computed, use the attributes optimizer)"</item>
        /// <item>"Remapping (active) : &lt;description&gt;</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public string ToLongString()
        {
            switch (Status)
            {
                case RemappingPointStatus.NotComputed:
                    return "Remapping (not computed, use the attributes optimizer)";
                case RemappingPointStatus.UpToDate:
                    return String.Format("Remapping : {0}", m_description);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a string representation of this point. Two possible formats : 
        /// <list type="">
        /// <item>"Remapping (not computed, use the attributes optimizer)"</item>
        /// <item>"Remapping (active) : i5 p7 c8 w9 m5"</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Status)
            {
                case RemappingPointStatus.NotComputed:
                    return "Remapping (not computed, use the attributes optimizer)";
                case RemappingPointStatus.UpToDate:
                    return String.Format("Remapping (active) : {0}", ToShortString());
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Will set the provided character scratchpad's base attributes as the target values to remap.
        /// </summary>
        /// <param name="newScratchpad">The scratchpad with the target base values to assign to this point</param>
        /// <param name="oldScratchpad">The scratchpad before we remapped</param>
        internal void SetBaseAttributes(CharacterScratchpad newScratchpad, CharacterScratchpad oldScratchpad)
        {
            // Update the status
            Status = RemappingPointStatus.UpToDate;

            // Initialize the string
            StringBuilder builder = new StringBuilder();

            // Scroll through attributes
            for (int i = 0; i < 5; i++)
            {
                // Compute the new base attribute
                EveAttribute attrib = (EveAttribute)i;
                m_attributes[i] = newScratchpad[attrib].Base;

                // Update description
                builder.AppendLine().Append(GetStringForAttribute(attrib, oldScratchpad, newScratchpad));
            }

            // Return the final string
            m_description = builder.ToString();
        }

        /// <summary>
        /// Gets a string representation of the attribute.
        /// </summary>
        /// <param name="attrib"></param>
        /// <param name="oldScratchpad"></param>
        /// <param name="newScratchpad"></param>
        /// <returns></returns>
        public static string GetStringForAttribute(EveAttribute attrib, CharacterScratchpad oldScratchpad,
                                                   CharacterScratchpad newScratchpad)
        {
            if (oldScratchpad == null)
                throw new ArgumentNullException("oldScratchpad");

            if (newScratchpad == null)
                throw new ArgumentNullException("newScratchpad");

            int bonusDifference = newScratchpad[attrib].Base - oldScratchpad[attrib].Base;

            if (bonusDifference == 0)
                return newScratchpad[attrib].ToString("%N (0) = %e = (%B + %r + %i)");

            return newScratchpad[attrib].ToString(bonusDifference > 0
                                                      ? String.Format("%N (+{0}) = %e = (%B + %r + %i)", bonusDifference)
                                                      : String.Format("%N ({0}) = %e = (%B + %r + %i)", bonusDifference));
        }

        /// <summary>
        /// Gets a hash code from the GUID.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        /// <summary>
        /// Clones the remapping point.
        /// </summary>
        /// <returns></returns>
        public RemappingPoint Clone()
        {
            RemappingPoint clone = new RemappingPoint();
            Array.Copy(m_attributes, clone.m_attributes, 5);
            clone.Status = Status;
            clone.Guid = Guid;
            return clone;
        }

        /// <summary>
        /// Creates a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableRemappingPoint Export()
        {
            return new SerializableRemappingPoint
                       {
                           Intelligence = m_attributes[(int)EveAttribute.Intelligence],
                           Perception = m_attributes[(int)EveAttribute.Perception],
                           Willpower = m_attributes[(int)EveAttribute.Willpower],
                           Charisma = m_attributes[(int)EveAttribute.Charisma],
                           Memory = m_attributes[(int)EveAttribute.Memory],
                           Description = m_description,
                           Status = Status
                       };
        }
    }
}