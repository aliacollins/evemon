using System;
using System.Collections.Generic;

namespace EVEMon.Common
{
    /// <summary>
    /// Static container class to provide access to a singleton instance of a class.
    /// </summary>
    /// <remarks>
    /// Any class can be stored as a singleton instance, provided it is decorated with the Singleton attribute and has
    /// a paramterless constructor. Instances are constructed the first time the Type is referenced.
    /// Instances may be stored using either a strong or weak reference, dependent on the WeakReference property
    /// of the SingletonAttribute. By default, instances are stored using strong references.
    /// </remarks>
    public static class Singleton
    {
        private static readonly Dictionary<Type, object> _strongInstances = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, WeakReference<object >> _weakInstances = new Dictionary<Type, WeakReference<object>>();

        /// <summary>
        /// Returns the single current instance of the specified Type. If no instance exists, the Type is constructed.
        /// </summary>
        /// <typeparam name="T">The Type to return. The required type must have a public, parameterless constructor and be decorated with the Singleton attribute.</typeparam>
        /// <returns></returns>
        public static T Instance<T>() where T : class, new ()
        {
            Type instanceType = typeof(T);
            object[] attributes = instanceType.GetCustomAttributes(typeof(SingletonAttribute), false);
            if (attributes.Length == 0)
                throw new ArgumentException(string.Format("Type {0} is not a Singleton Class", instanceType.FullName));
            SingletonAttribute singletonAttribute = (SingletonAttribute)attributes[0];
            if (singletonAttribute.WeakReference)
                return (T)WeakInstance(instanceType);
            else
                return (T)StrongInstance(instanceType);
        }

        /// <summary>
        /// Returns the current singleton instance of the specified Type, for Types stored as a strong reference. I f no instance exists, one is created.
        /// </summary>
        /// <param name="instanceType">The Type to return.</param>
        /// <returns>An instance of the specified Type as a System.Object.</returns>
        private static object StrongInstance(Type instanceType)
        {
            object instance;
            lock (_strongInstances)
            {
                if (_strongInstances.ContainsKey(instanceType))
                    instance = _strongInstances[instanceType];
                else
                {
                    instance = Activator.CreateInstance(instanceType);
                    _strongInstances.Add(instanceType, instance);
                }
            }
            return instance;
        }

        /// <summary>
        /// Returns the current singleton instance of he specified Type, for Types stored as a Weak Reference. If no instance exists, one is created.
        /// </summary>
        /// <param name="instanceType">The Type to return.</param>
        /// <returns>An instance of the specified Type as a System.Object.</returns>
        private static object WeakInstance(Type instanceType)
        {
            object instance = null;
            lock(_weakInstances)
            {
                if (_weakInstances.ContainsKey(instanceType))
                {
                    instance = _weakInstances[instanceType].Target;
                    if (instance == null)
                    {
                        instance = Activator.CreateInstance(instanceType);
                        _weakInstances[instanceType] = new WeakReference<object>(instance);
                    }
                }
                if (instance == null)
                {
                    instance = Activator.CreateInstance(instanceType);
                    _weakInstances.Add(instanceType, new WeakReference<object>(instance));
                }
            }
            return instance;
        }
    }
}
