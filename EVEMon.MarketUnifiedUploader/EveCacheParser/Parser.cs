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


using System.Collections.Generic;
using System.IO;
using EVEMon.MarketUnifiedUploader.EveCacheParser.STypes;

namespace EVEMon.MarketUnifiedUploader.EveCacheParser
{
    public static class Parser
    {
        #region CacheFilesFinder Methods

        /// <summary>
        /// Sets the folders to look for cached files.
        /// </summary>
        /// <param name="args">The folders.</param>
        public static void SetCachedFilesFolders(params string[] args)
        {
            CachedFilesFinder.SetCachedFilesFolders(args);
        }

        /// <summary>
        /// Sets the methods to includ in filter.
        /// </summary>
        /// <param name="args">The methods.</param>
        public static void SetIncludeMethodsFilter(params string[] args)
        {
            CachedFilesFinder.SetIncludeMethodsFilter(args);
        }

        /// <summary>
        /// Sets the methods to exclude in filter.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void SetExcludeMethodsFilter(params string[] args)
        {
            CachedFilesFinder.SetExcludeMethodsFilter(args);
        }

        /// <summary>
        /// Gets the bulk data cached files.
        /// </summary>
        /// <param name="folderPath">The folder location.</param>
        /// <returns></returns>
        public static FileInfo[] GetBulkDataCachedFiles(string folderPath)
        {
            return CachedFilesFinder.GetBulkDataCachedFiles(folderPath);
        }

        /// <summary>
        /// Gets the macho net cached files.
        /// </summary>
        /// <returns></returns>
        public static FileInfo[] GetMachoNetCachedFiles(string folderPath = null)
        {
            return CachedFilesFinder.GetMachoNetCachedFiles(folderPath);
        }

        #endregion


        #region CacheFileParser Methods

        /// <summary>
        /// Dumps the structure of the file to a file.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void DumpStructure(FileInfo file)
        {
            CachedFileParser.DumpStructure(file);
        }


        /// <summary>
        /// Reads the specified file and shows it in an ASCII format.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void ShowAsAscii(FileInfo file)
        {
            CachedFileParser.ShowAsAscii(file);
        }

        /// <summary>
        /// Parses the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static KeyValuePair<object, object> Parse(FileInfo file)
        {
            return CachedFileParser.Parse(file);
        }

        /// <summary>
        /// Gets the object of a cached object.
        /// </summary>
        /// <param name="value">The object.</param>
        /// <returns></returns>
        public static object GetObject(object value)
        {
            SCachedObjectType cachedObject = value as SCachedObjectType;
            return cachedObject == null ? null : cachedObject.GetCachedObject();
        }

        #endregion
    }
}
