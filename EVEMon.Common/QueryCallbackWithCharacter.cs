using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// A delegate for query callbacks that include the character identity.
    /// </summary>
    public delegate void QueryCallbackWithCharacter<T>(APIResult<T> result, CharacterIdentity id);
}
