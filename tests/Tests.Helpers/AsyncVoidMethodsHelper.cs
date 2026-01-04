using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tests.Helpers
{
    public static class AsyncVoidMethodsHelper
    {
        #region Helper Methods

        /// <summary>
        /// Gets the asynchronous void methods.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetAsyncVoidMethods(this Assembly assembly)
            => assembly.GetLoadableTypes()
                .SelectMany(type => type.GetMethods(
                    BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.DeclaredOnly))
                .Where(method => method.HasAttribute<AsyncStateMachineAttribute>())
                .Where(method =>
                    !method.GetParameters()
                        .Select(param => param.ParameterType)
                        .SequenceEqual(new[] { typeof(object), typeof(EventArgs) }) &&
                    !method.GetParameters()
                        .Select(param => param.ParameterType)
                        .SequenceEqual(new[] { typeof(EventArgs) }))
                .Where(method =>
                    !method.GetParameters()
                        .Any(param => param.ParameterType.IsSubclassOf(typeof(EventArgs))))
                .Where(method => method.ReturnType == typeof(void));

        /// <summary>
        /// Gets the loadable types.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        /// <summary>
        /// Determines whether this instance has attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        private static bool HasAttribute<TAttribute>(this MethodInfo method)
            where TAttribute : Attribute
            => method.GetCustomAttributes(typeof(TAttribute), false).Any();

        #endregion

    }
}
