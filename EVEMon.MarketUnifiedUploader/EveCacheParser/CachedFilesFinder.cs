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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EVEMon.MarketUnifiedUploader.EveCacheParser
{
    internal static class CachedFilesFinder
    {
        private static List<string> s_methodIncludeFilter = new List<string>();
        private static List<string> s_methodExcludeFilter = new List<string>();
        private static List<string> s_includedFolders = new List<string>();

        private const string EVESettingsPath = @"CCP\EVE";
        private const string CacheFolderPath = @"cache\MachoNet\87.237.38.200";
        private const string ServerLookupName = "*_tranquility";
        private const string DefaultFolderLookupName = "CachedMethodCalls";
        private const string BulkdataFolderName = "bulkdata";
        private const string CacheFileExtensionLookup = "*.cache*";
        private const string CacheFile2ExtensionLookup = "*.cache2";

        /// <summary>
        /// Sets the folders to look for cached files.
        /// </summary>
        /// <param name="folders">The folders.</param>
        internal static void SetCachedFilesFolders(params string[] folders)
        {
            if (folders == null)
            {
                s_includedFolders = new List<string>();
                return;
            }

            s_includedFolders.AddRange(folders.Where(x => !String.IsNullOrWhiteSpace(x)));
        }

        /// <summary>
        /// Sets the methods to includ in filter.
        /// </summary>
        /// <param name="methods">The methods.</param>
        internal static void SetIncludeMethodsFilter(params string[] methods)
        {
            if (methods == null)
            {
                s_methodIncludeFilter = new List<string>();
                return;
            }

            s_methodIncludeFilter.AddRange(methods.Where(x => !String.IsNullOrWhiteSpace(x)));
        }

        /// <summary>
        /// Sets the methods to exclude in filter.
        /// </summary>
        /// <param name="methods">The args.</param>
        internal static void SetExcludeMethodsFilter(params string[] methods)
        {
            if (methods == null)
            {
                s_methodExcludeFilter = new List<string>();
                return;
            }

            s_methodExcludeFilter.AddRange(methods.Where(x => !String.IsNullOrWhiteSpace(x)));
        }

        /// <summary>
        /// Gets the bulk data cached files.
        /// </summary>
        /// <param name="folderPath">The folder location.</param>
        /// <returns></returns>
        internal static FileInfo[] GetBulkDataCachedFiles(string folderPath)
        {
            if (String.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return null;

            if (!folderPath.Contains(BulkdataFolderName))
                folderPath = Path.Combine(folderPath, BulkdataFolderName);

            return new DirectoryInfo(folderPath).GetFiles(CacheFile2ExtensionLookup);
        }

        /// <summary>
        /// Gets the macho net cached files.
        /// </summary>
        /// <returns></returns>
        internal static FileInfo[] GetMachoNetCachedFiles(string folderPath = null)
        {
            // Construct the path to the EVE cache folder
            string eveApplicationDataDir = String.IsNullOrWhiteSpace(folderPath)
                                               ? Path.Combine(
                                                   Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                   EVESettingsPath)
                                               : folderPath;

            // Quit if folder not found
            if (!Directory.Exists(eveApplicationDataDir))
                return new FileInfo[] { };

            // Find all eve clients data folders
            DirectoryInfo directory = new DirectoryInfo(eveApplicationDataDir);
            DirectoryInfo[] foldersIn = directory.GetDirectories(ServerLookupName).OrderBy(dir => dir.CreationTimeUtc).ToArray();

            // Get the path to the cache folder of each eve client
            IEnumerable<string> cacheFoldersPath =
                !foldersIn.Any()
                    ? new List<string> { Path.Combine(eveApplicationDataDir, CacheFolderPath) }
                    : foldersIn.Select(folder => folder.Name).Select(
                        folder => Path.Combine(eveApplicationDataDir, folder, CacheFolderPath));

            // Get the latest cache folder (differs on every client patch version)
            // We take into consideration the edge case where the user has multiple clients but uses only one
            string latestFolder = cacheFoldersPath.Select(path => new DirectoryInfo(path)).Where(dir => dir.Exists).SelectMany(
                dir => dir.GetDirectories()).Select(
                    dir => int.Parse(dir.Name, CultureInfo.InvariantCulture)).Concat(new[] { 0 }).Max().ToString(
                        CultureInfo.InvariantCulture);

            // Construct the final path to the cache folders
            cacheFoldersPath = s_includedFolders.Any()
                                   ? cacheFoldersPath.SelectMany(
                                       path => s_includedFolders,
                                       (path, folder) => Path.Combine(path, latestFolder, folder)).ToList()
                                   : cacheFoldersPath.Select(path => Path.Combine(path, latestFolder, DefaultFolderLookupName));

            // Get the cached files we need to scrap
            IEnumerable<FileInfo> cachedFiles = cacheFoldersPath.Select(path => new DirectoryInfo(path)).Where(
                dir => dir.Exists).SelectMany(dir => dir.GetFiles(CacheFileExtensionLookup));

            // Finds the cached files that are legit EVE files and satisfy the methods search criteria
            return cachedFiles.Where(cachedFile => cachedFile.Exists).Select(
                cachedFile => new CachedFileReader(cachedFile, false)).Where(
                    reader => reader.Buffer.First() == (byte)StreamType.StreamStart).Where(
                        cachedFile =>
                        s_methodIncludeFilter.Any()
                            ? s_methodIncludeFilter.Any(method => Encoding.ASCII.GetString(cachedFile.Buffer).Contains(method))
                            : !s_methodExcludeFilter.Any() ||
                              s_methodExcludeFilter.All(method => !Encoding.ASCII.GetString(cachedFile.Buffer).Contains(method))
                ).Select(cachedFile => new FileInfo(cachedFile.Fullname)).ToArray();
        }
    }
}
