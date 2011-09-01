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
        public APIMethod(APIMethods method, string path)
        {
            Method = method;
            Path = path;
        }

        /// <summary>
        /// Returns the APIMethods enumeration member for this APIMethod.
        /// </summary>
        public APIMethods Method { get; private set; }

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
            return Enum.GetNames(typeof(APIMethods)).Select(
                methodName => new { methodName, methodEnum = (APIMethods)Enum.Parse(typeof(APIMethods), methodName) }).Select(
                    methodName => new
                                      {
                                          methodName,
                                          methodURL = NetworkConstants.ResourceManager.GetString(
                                              String.Format("API{0}", methodName.methodName))
                                      }).Select(
                                          methodName => new APIMethod(methodName.methodName.methodEnum, methodName.methodURL));
        }
    }
}