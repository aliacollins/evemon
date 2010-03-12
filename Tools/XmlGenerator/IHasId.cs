using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVEMon.XmlGenerator
{
    /// <summary>
    /// Implementors have an ID attribute
    /// </summary>
    public interface IHasID
    {
        int ID { get; }
    }
}
