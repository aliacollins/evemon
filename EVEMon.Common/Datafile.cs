using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EVEMon.Common
{


    #region Datafile class

    /// <summary>
    /// Represents a datafile
    /// </summary>
    public sealed class Datafile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filename"></param>
        public Datafile(string filename)
        {
            // The file may be in local directory, %APPDATA%, etc.
            Filename = filename;

            // Compute the MD5 sum
            var fullpath = GetFullPath(filename);
            MD5 md5 = MD5.Create();
            StringBuilder builder = new StringBuilder();

            using (Stream fileStream = new FileStream(fullpath, FileMode.Open))
            {
                using (Stream bufferedStream = new BufferedStream(fileStream, 1200000))
                {
                    byte[] hash = md5.ComputeHash(bufferedStream);
                    foreach (byte b in hash)
                    {
                        builder.Append(b.ToString("x2").ToLower(CultureConstants.DefaultCulture));
                    }
                }
            }

            MD5Sum = builder.ToString();
        }

        /// <summary>
        /// Gets or sets the datafile name
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets or sets the MD5 sum
        /// </summary>
        public string MD5Sum { get; private set; }

        /// <summary>
        /// Gets the fuly-qualified path of the provided datafile name
        /// </summary>
        /// <remarks>
        /// Attempts to find a datafile  - firstly, look in the %APPDATA% folder
        /// Then look in the Application data folder (roaming users on usb devices)
        /// If not there, this could be a first run so copy from resources folder in installation directory
        /// </remarks>
        /// <exception cref="ApplicationException">The file does not exist or it cannot be copied</exception>
        internal static string GetFullPath(string filename)
        {
            string evemonDataDir = EveMonClient.EVEMonDataDir ??
                                   Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EVEMon");

            // Look in the %APPDATA% folder
            string filepath = String.Format("{0}{1}{2}", evemonDataDir, Path.DirectorySeparatorChar, filename);

            if (File.Exists(filepath))
                return filepath;

            // File isn't in the current folder, look in installation directory ("resources" subdirectory)
            string baseFile = String.Format("{1}Resources{0}{2}", Path.DirectorySeparatorChar,
                                            AppDomain.CurrentDomain.BaseDirectory, filename);

            // Does not exist also ? 
            if (!File.Exists(baseFile))
                throw new ApplicationException(baseFile + " not found!");

            // The file was in the installation directory, let's copy it to %APPDATA%
            FileHelper.OverwriteOrWarnTheUser(baseFile, filepath);

            // Return
            return baseFile;
        }
    }

    #endregion
}