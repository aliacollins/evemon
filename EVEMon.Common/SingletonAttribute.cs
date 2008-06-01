using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.Common
{
    /// <summary>
    /// Custom Attribute to decorate classes intended to be used as Singleton classes instatiated by the Singleton class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SingletonAttribute : Attribute
    {
        private bool _weakReference = false;

        /// <summary>
        /// Returns true if the class should be stored using a WeakReference. The default is false.
        /// </summary>
        public bool WeakReference
        {
            get { return _weakReference; }
            set { _weakReference = value; }
        }
    }
}
