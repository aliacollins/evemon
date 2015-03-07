# region License
/* EVECacheParser - .NET 4/C# EVE Cache File Parser Library
 * Copyright © 2012 Jimi 'Desmont McCallock' C <jimikar@gmail.com>
 *
 * Based on:
 * - reverence - Python library for processing EVE Online cache and bulkdata
 *    Copyright © 2003-2011 Jamie 'Entity' van den Berge <jamie@hlekkir.com>
 *    https://github.com/ntt/reverence
 *
 * - libevecache - C++ EVE online reverse engineered cache reading library
 *    Copyright © 2009-2010  StackFoundry LLC and Yann 'Kaladr' Ramin <yann@stackfoundry.com>
 *    http://dev.eve-central.com/libevecache/
 *    http://gitorious.org/libevecache
 *    https://github.com/theatrus/libevecache
 *
 * - EveCache.Net - A port of libevecache to C#
 *    Copyright © 2011 Jason 'Jay Wareth' Watkins <jason@blacksunsystems.net>
 *    https://github.com/jwatkins42/EveCache.Net
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
# endregion


using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace EVEMon.MarketUnifiedUploader.EveCacheParser.STypes
{
    internal sealed class SObjectType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SObjectType"/> class.
        /// </summary>
        internal SObjectType()
            : base(StreamType.ClassObject)
        {
        }

        #endregion


        #region Internal Properties

        /// <summary>
        /// Gets a value indicating whether this object is a 'DBRowDescriptor'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'DBRowDescriptor'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDBRowDescriptor
        {
            get { return Name.EndsWith(".DBRowDescriptor"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'RowList'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'RowList'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRowList
        {
            get { return Name.EndsWith(".RowList"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'RowDict'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'RowDict'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRowDict
        {
            get { return Name.EndsWith(".RowDict"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'Rowset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'Rowset'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRowset
        {
            get { return Name.EndsWith(".Rowset"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CRowset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CRowset'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCRowset
        {
            get { return Name.EndsWith(".CRowset"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CFilterRowset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CFilterRowset'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCFilterRowset
        {
            get { return Name.EndsWith(".CFilterRowset"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CIndexedRowset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CIndexedRowset'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCIndexedRowset
        {
            get { return Name.EndsWith(".CIndexedRowset"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'KeyVal'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'KeyVal'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsKeyVal
        {
            get { return Name.EndsWith(".KeyVal"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CachedMethodCallResult'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CachedMethodCallResult'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCachedMethodCallResult
        {
            get { return Name.EndsWith(".CachedMethodCallResult"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CachedObject'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CachedObject'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCachedObject
        {
            get { return Name.EndsWith(".CachedObject"); }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CachedObject'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CachedObject'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsObjectCachingCachedObject
        {
            get { return Name.EndsWith(".objectCaching.CachedObject"); }
        }

        #endregion


        #region Private Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        private string Name
        {
            get
            {
                SType current = this;
                while (current.Members.Count > 0)
                {
                    current = current.Members[0];
                }

                SStringType stringType = current as SStringType;

                return stringType != null ? stringType.Text : string.Empty;
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal override object ToObject()
        {
            if (IsRowList || IsCRowset)
            {
                return Members.Where(member => member != Members.First()).Select(
                    member => member.Members).SelectMany(obj => obj, (obj, type) => new { obj, type }).Where(
                        obj => !(obj.type is SObjectType)).Select(
                            obj => obj.type.ToObject()).ToList();
            }

            if (IsCFilterRowset || IsRowDict || IsCIndexedRowset)
                return ToDictionary(Members.Where(member => member != Members.First()).ToList());

            if (IsKeyVal || IsCachedObject || IsCachedMethodCallResult)
                return Members.Where(member => member != Members.First()).Select(member => member.ToObject()).ToList();
            
            if (IsObjectCachingCachedObject)
                return Members.Where(member => member != Members.First()).Select(member => member.ToObject()).ToList();

            if (IsDBRowDescriptor)
                return null;

            // 'Clone()' is returned for debugging purposes
            return Debugger.IsAttached ? Clone() : null;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SObjectType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "<SObjectType '{0}' [{1:X4}]>", Name, DebugID);
        }

        #endregion
    }
}
