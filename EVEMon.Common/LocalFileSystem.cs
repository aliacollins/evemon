using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EVEMon.Common
{
    public static class LocalFileSystem
    {
        private static string m_portraitFolder = null;

        private static string GetPortraitCacheFolder()
        {
            string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string EVEApplicationData = String.Format("{1}{0}CCP{0}EVE", Path.DirectorySeparatorChar, LocalApplicationData);

            // create a pattern that matches anything "*_tranquility"
            string filePattern = "*_tranquility";

            // enumerate files in the EVE cache directory
            DirectoryInfo di = new DirectoryInfo(EVEApplicationData);
            DirectoryInfo[] filesInEveCache = di.GetDirectories(filePattern);

            if (filesInEveCache.Length > 0)
            {
                string PortraitCache = filesInEveCache[0].Name;
                return String.Format("{2}{0}{1}{0}cache{0}Pictures{0}Portraits", Path.DirectorySeparatorChar, PortraitCache, EVEApplicationData);
            }
            else
            {
                return null;
            }
        }

        static LocalFileSystem()
        {
            m_portraitFolder = GetPortraitCacheFolder();
        }

        public static string PortraitCacheFolder
        {
            get
            {
                return m_portraitFolder;
            }
        }
    }
}
