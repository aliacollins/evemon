using System;
using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common.Serialization.BattleClinic
{
    /// <summary>
    /// Serializable class for a Battleclinic's API method and its path.
    /// Each BCAPIConfiguration maintains a list of Battleclinic's APIMethods.
    /// </summary>
    public class BCAPIMethod
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        public BCAPIMethod(BCAPIMethods method, string path)
        {
            Method = method;
            Path = path;
        }

        /// <summary>
        /// Returns the BCAPIMethods enumeration member for this Battleclinic's APIMethod.
        /// </summary>
        public BCAPIMethods Method { get; private set; }

        /// <summary>
        /// Returns the defined URL suffix path for this BCAPIMethod.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates a set of Battleclinic's API methods with their default urls.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BCAPIMethod> CreateDefaultSet()
        {
            return Enum.GetNames(typeof(BCAPIMethods)).Select(
                methodName =>
                    new
                    {
                        methodName,
                        methodEnum = (BCAPIMethods)Enum.Parse(typeof(BCAPIMethods), methodName)
                    }).Select(method =>
                        new
                        {
                            method,
                            methodURL = NetworkConstants.ResourceManager.GetString(
                                String.Format(CultureConstants.InvariantCulture, "BattleClinicAPI{0}", method.methodName))
                        }).Select(bcAPIMethod => new BCAPIMethod(bcAPIMethod.method.methodEnum, bcAPIMethod.methodURL));
        }
    }
}