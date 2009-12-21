using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace EVEMon.Common
{
    public static class LocalFileSystem
    {
        private static string m_portraitFolder = null;
        private static Nullable<bool> m_removeReadOnlyAttributes;
        private static object m_removeReadOnlyAttributesLock = new object();    // Nullable<T> assignment is not atomic

        private static string GetPortraitCacheFolder()
        {
            string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string EVEApplicationData = String.Format("{1}{0}CCP{0}EVE", Path.DirectorySeparatorChar, LocalApplicationData);

            // create a pattern that matches anything "*_tranquility"
            string filePattern = "*_tranquility";

            // check folder exists
            if (!Directory.Exists(EVEApplicationData))
            {
                return null;
            }

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

        /// <summary>
        /// Overwrites a destination file with the provided source file, either copying or moving this one. 
        /// This method will take care of the readonly attributes, prompting the user to EVEMon removing them.
        /// Finally, should a <see cref="UnauthorizedAccessException"/> occurs, EVEMon will display a readable message for the end-user
        /// </summary>
        /// <param name="srcFileName">The source file name.</param>
        /// <param name="destFileName">The destination file name, it does not have to already exist</param>
        /// <param name="operation">Copy or move.</param>
        /// <returns>False if the user denied to remove the read-only attribute or if he didn't have the permissions to write the file; true otherwise.</returns>
        public static bool OverwriteOrWarnTheUser(string srcFileName, string destFileName, OverwriteOperation operation)
        {
            // While problems happen and the user ask to retry...
            while (true)
            {
                try
                {
                    if (File.Exists(destFileName))
                    {
                        // We need to make sure this file is not read-only
                        // If it is, this method will request the user the permission to automatically remove the readonly attributes
                        if (!TryMakeWritable(destFileName)) return false;

                        // Delete the file
                        if (operation == OverwriteOperation.Move) File.Delete(destFileName);
                    }

                    // Overwrite the file
                    if (operation == OverwriteOperation.Move) File.Move(srcFileName, destFileName);
                    else File.Copy(srcFileName, destFileName, true);

                    // Ensures we didn't copied a read-only attribute, no permission required since the file has been overwritten
                    var newDestFile = new FileInfo(destFileName);
                    if (newDestFile.IsReadOnly) newDestFile.IsReadOnly = false;

                    // Quit the loop
                    return true;
                }
                catch (UnauthorizedAccessException exc)
                {
                    ExceptionHandler.LogException(exc, true);

                    string message = exc.Message;
                    message += "\r\n\r\nEVEMon failed to save to a file. You probably have insufficient rights over this one. Choosing to abort will make EVEMon quit.";
                    var result = MessageBox.Show(message, "Failed to write over a file", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                    // On abort, we quit the application
                    if (result == DialogResult.Abort)
                    {
                        Application.Exit();
                        return false;
                    }

                    // The loop will begin again if the users asked to retry
                    if (result == DialogResult.Ignore) return false;
                }
            }
        }

        /// <summary>
        /// Try to make a file writeable, requesting the user the right to remove the readonly attributes the first time
        /// </summary>
        /// <param name="filename">The file to make writeable. It must exists but does not have to be read-only</param>
        /// <returns>False if the file was readonly and the user denied permission to make it writeable; true otherwrise</returns>
        public static bool TryMakeWritable(string filename)
        {
            lock (m_removeReadOnlyAttributesLock)
            {
                // When the file is read-only, prompts the user for removing it.
                var file = new FileInfo(filename);
                if (file.IsReadOnly)
                {
                    // We request the user to allow us to remove the attributes
                    if (m_removeReadOnlyAttributes == null)
                    {
                        RequestPermissionToRemoveReadOnlyAttributes();
                    }

                        // Return false if we're not allowed to remove the read-only file
                        if (!m_removeReadOnlyAttributes.Value) return false;

                    // Remove the attribute
                    file.IsReadOnly = false;
                }
            }

            return true;
        }

        /// <summary>
        /// Request from the use the permission for EVEMon to remove the read-only attributes on its own files
        /// </summary>
        /// <returns>True if the user granted the rights to make its files writable, false otherwise</returns>
        private static bool RequestPermissionToRemoveReadOnlyAttributes()
        {
            if (m_removeReadOnlyAttributes == null)
            {
                // Prepare caption and text
                string message = "EVEMon detected that some of its files are read-only, preventing it to save its datas.\r\n\r\n";
                message += "Choosing YES will allow EVEMon to remove the read-only attributes on its own files (only).\r\n";
                message += "Choosing NO will force EVEMon to continue without writing its files. This can cause unexpected behaviours.\r\n\r\n";
                message += "Note : if you restart EVEMon and it still encounters read-only files, you will be prompted again.";

                // Display the message box
                var result = MessageBox.Show(message, "Allow EVEMon to make its files writable", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                // User denied us the permission to make files writeable
                if (result == DialogResult.No) m_removeReadOnlyAttributes = false;
                else m_removeReadOnlyAttributes = true;
            }

            // Returns the permission granted by the user
            return m_removeReadOnlyAttributes.Value;
        }
    }

    public enum OverwriteOperation
    {
        /// <summary>
        /// The source file will be moved to the target file
        /// </summary>
        Move,
        /// <summary>
        /// The source file will be copied over the target file
        /// </summary>
        Copy
    }

}
