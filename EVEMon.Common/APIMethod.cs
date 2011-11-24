using System;
using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common
{
    /// <summary>
    /// Serializable class for an API method and its path. Each APIConfiguration maintains a list of APIMethods.
    /// </summary>
    public class APIMethod
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        private APIMethod(Enum method, string path)
        {
            Method = method;
            Path = path;
        }

        /// <summary>
        /// Returns the APIMethods enumeration member for this APIMethod.
        /// </summary>
        public Enum Method { get; private set; }

        /// <summary>
        /// Returns the defined URL suffix path for this APIMethod.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates a set of API methods with their default urls.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<APIMethod> CreateDefaultSet()
        {
            return APIMethods.Methods.Where(method => method.ToString() != "None").Select(
                methodName =>
                new
                    {
                        methodName,
                        methodURL = NetworkConstants.ResourceManager.GetString(
                            String.Format(CultureConstants.InvariantCulture, "API{0}", methodName))
                    }).Where(method => method.methodURL != null).Select(
                        method => new APIMethod(method.methodName, method.methodURL));
        }
    }
}