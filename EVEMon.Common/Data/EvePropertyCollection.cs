using System;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public sealed class EvePropertyCollection : ReadonlyCollection<EvePropertyValue>
    {
        #region Constructor

        /// <summary>
        /// Deserialization consructor.
        /// </summary>
        /// <param name="src"></param>
        internal EvePropertyCollection(SerializablePropertyValue[] src)
            : base(src == null ? 0 : src.Length)
        {
            if (src == null)
                return;

            m_items.Capacity = src.Length;
            foreach (var srcProp in src)
            {
                EvePropertyValue prop = new EvePropertyValue(srcProp);
                if (prop.Property != null)
                    m_items.Add(prop);
            }
            m_items.Trim();
        }

        #endregion


        #region Indexers

        /// <summary>
        /// Gets a property from its name. If not found, return null.
        /// </summary>
        /// <param name="property">The property we're searching for.</param>
        /// <returns>The wanted property when found; null otherwise.</returns>
        public Nullable<EvePropertyValue> this[EveProperty property]
        {
            get
            {

                foreach (EvePropertyValue prop in m_items)
                {
                    if (prop.Property == null)
                        break;

                    if (prop.Property == property)
                        return prop;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a property from its name. If not found, return null.
        /// </summary>
        /// <param name="property">The property we're searching for.</param>
        /// <returns>The wanted property when found; null otherwise.</returns>
        public Nullable<EvePropertyValue> this[int id]
        {
            get
            {
                foreach (EvePropertyValue prop in m_items)
                {
                    if (prop.Property == null)
                        break;

                    if (prop.Property.ID == id)
                        return prop;
                }
                return null;
            }
        }

        #endregion
    }
}
