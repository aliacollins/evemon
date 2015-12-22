using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;

namespace EVEMon.Common.Data
{
    public sealed class MaterialCollection : ReadonlyCollection<Material>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialCollection"/> class.
        /// </summary>
        /// <param name="materials">The materials.</param>
        internal MaterialCollection(IEnumerable<Material> materials)
            : base(materials == null ? 0 : materials.Count())
        {
            if (materials == null)
                return;

            foreach (Material material in materials)
            {
                Items.Add(material);
            }
        }
    }
}