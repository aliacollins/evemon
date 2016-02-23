using System;
using System.Collections.Generic;
using System.IO;
using EVEMon.Common.Helpers;

namespace EVEMon.Common.Data
{


    #region Datafile class

    /// <summary>
    /// Represents a datafile
    /// </summary>
    public sealed class Datafile
    {
        private const string DatafileExtension = ".xml.gzip";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filename"></param>
        public Datafile(string filename)
        {
            // The file may be in local directory, %APPDATA%, etc.
            Filename = filename;

            // Compute the MD5 sum
            MD5Sum = Util.CreateMD5From(GetFullPath(Filename));
        }

        /// <summary>
        /// Gets or sets the datafile name
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Gets or sets the MD5 sum
        /// </summary>
        public string MD5Sum { get; private set; }

        /// <summary>
        /// Gets the datafile extension.
        /// </summary>
        /// <value>
        /// The datafile extension.
        /// </value>
        public static string DatafilesExtension => DatafileExtension;

        /// <summary>
        /// Gets the old datafile extension.
        /// </summary>
        /// <value>
        /// The old datafile extension.
        /// </value>
        public static string OldDatafileExtension => DatafileExtension.TrimEnd("ip".ToCharArray());

        /// <summary>
        /// Gets the fully-qualified path of the provided datafile name
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <remarks>
        /// Attempts to find a datafile  - firstly, look in the %APPDATA% folder
        /// Then look in the Application data folder (roaming users on usb devices)
        /// If not there, this could be a first run so copy from resources folder in installation directory
        /// </remarks>
        internal static string GetFullPath(string filename)
        {
            string evemonDataDir = EveMonClient.EVEMonDataDir ??
                                   Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EVEMon");

            // Look in the %APPDATA% folder
            string filepath = $"{evemonDataDir}{Path.DirectorySeparatorChar}{filename}";

            if (File.Exists(filepath))
                return filepath;

            // File isn't in the current folder, look in installation directory ("resources" subdirectory)
            string baseFile = $"{AppDomain.CurrentDomain.BaseDirectory}Resources{Path.DirectorySeparatorChar}{filename}";

            // Does not exist also ? 
            if (!File.Exists(baseFile))
                throw new FileNotFoundException($"{baseFile} not found!");

            // The file was in the installation directory, let's copy it to %APPDATA%
            FileHelper.CopyOrWarnTheUser(baseFile, filepath);

            // Return
            return baseFile;
        }

        /// <summary>
        /// Gets the data files from the given directory path.
        /// </summary>
        /// <param name="dirPath">The directory path.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFilesFrom(string dirPath, string fileExtension)
            => Directory.GetFiles(dirPath, "*" + fileExtension, SearchOption.TopDirectoryOnly);
    }

    #endregion
}