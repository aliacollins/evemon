using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EVEMon.Common;
using Xunit;

namespace Tests.EVEMon.Common
{
    public static class AsyncVoidMethodsTests
    {
        /// <summary>
        /// Ensures that no method is marked asynchronous void.
        /// </summary>
        [Fact]
        public static void EnsureNoAsyncVoidMethods()
        {
            AssertNoAsyncVoidMethods(typeof(EveMonClient).Assembly);
        }


        #region Helper Methods

        /// <summary>
        /// Asserts that no method is marked asynchronous void.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private static void AssertNoAsyncVoidMethods(Assembly assembly)
        {
            IEnumerable<MethodInfo> asyncVoidMethods = assembly.GetAsyncVoidMethods();

            List<string> messages = asyncVoidMethods.Select(method =>
                $"'{method.DeclaringType?.Name}.{method.Name}' is an async void method.")
                .ToList();

            Assert.False(messages.Any(),
                "Async void methods found!" + Environment.NewLine + String.Join(Environment.NewLine, messages));
        }

        /// <summary>
        /// Gets the asynchronous void methods.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        private static IEnumerable<MethodInfo> GetAsyncVoidMethods(this Assembly assembly)
            => assembly.GetLoadableTypes()
                .SelectMany(type => type.GetMethods(
                    BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.DeclaredOnly))
                .Where(method => method.HasAttribute<AsyncStateMachineAttribute>())
                .Where(method => method.ReturnType == typeof(void));

        /// <summary>
        /// Gets the loadable types.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">assembly</exception>
        private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
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
