using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace EVEMon.Common.Controls
{
    public partial class CharacterPortrait : UserControl
    {
        private long m_id = -1;
        private Character m_character;
        private bool m_updatingPortrait;
        private bool m_pendingUpdate;


        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterPortrait()
        {
            InitializeComponent();
            this.pictureBox.Image = this.pictureBox.InitialImage;

            this.Disposed += new EventHandler(OnDisposed);
            EveClient.CharacterPortraitChanged += new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterPortraitChanged);
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            this.Disposed -= new EventHandler(OnDisposed);
            EveClient.CharacterPortraitChanged -= new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterPortraitChanged);
        }

        /// <summary>
        /// When the control is made visible, we check for a pending update.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            // If we previously delayed an update, we do it now.
            if (this.Visible && m_pendingUpdate)
            {
                UpdateContent();
            }
            base.OnVisibleChanged(e);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the character ID. Also sets the <see cref="Character"/> property to <c>null</c>.
        /// </summary>
        public long CharacterID
        {
            get { return m_id; }
            set
            {
                if (m_id != value)
                {
                    m_id = value;
                    m_character = null;
                    UpdateContent();
                }
            }
        }

        /// <summary>
        /// Gets or sets the character. Also updates the <see cref="CharacterID"/> property.
        /// </summary>
        public Character Character
        {
            get { return m_character; }
            set
            {
                if (m_character != value)
                {
                    m_id = value.CharacterID;
                    m_character = value;
                    UpdateContent();
                }
            }
        }

        /// <summary>
        /// Gets the local portraits cache path for our character's GUID.
        /// </summary>
        /// <remarks>
        /// We're talking about the cache in %APPDATA%\cache. 
        /// This is different from the ImageService's hit cache (%APPDATA%\cache\image) or the game's portrait cache (in EVE Online folder)
        /// </remarks>
        public string CachePath
        {
            get
            {
                if (m_character == null) return "";
                var cacheDir = String.Format("{1}{0}cache{0}portraits", Path.DirectorySeparatorChar, EveClient.EVEMonDataDir);
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
                return Path.Combine(cacheDir, m_character.Guid.ToString() + ".png");
            }
        }

        /// <summary>
        /// Gets or sets true when this control or the character it is bound to is updating.
        /// </summary>
        private bool IsUpdating
        {
            get
            {
                if (m_updatingPortrait) return true;
                if (m_character != null) return m_character.IsUpdatingPortrait;
                return false;
            }
            set
            {
                m_updatingPortrait = value;
                if (m_character != null) m_character.IsUpdatingPortrait = value;
            }
        }
        #endregion


        #region Default mechanism on character id change (portraits cache, then ImageService for CCP url)
        /// <summary>
        /// When the character ID changed... 
        /// <list type="bullet">
        /// <item>We check for a cached portrait in %APPDATA%\Cache.</item>
        /// <item>It if failed, we assemble the url for a CCP download and give it to ImageService.</item>
        /// <item>ImageService will first check its cache (%APPDATA%\Cache\Images), then download the url if no image was found in cache.</item>
        /// </list>
        /// </summary>
        private void UpdateContent()
        {
            if (!this.Visible)
            {
                m_pendingUpdate = true;
                return;
            }
            m_pendingUpdate = false;

            // In safe mode, doesn't bother with the character portrait
            if (Settings.UI.SafeForWork)
            {
                pictureBox.Image = pictureBox.InitialImage;
                return;
            }

            // Try to retrieve the portrait from our portrait cache (%APPDATA%\Cache)
            var image = GetPortraitFromCache();
            if (image != null)
            {
                pictureBox.Image = image;
                return;
            }

            // The image does not exist in cache, we try to retrieve it from CCP
            // Note this method will first check the ImageService cache before to resort to download.
            pictureBox.Image = pictureBox.InitialImage;
            UpdateCharacterImageFromCCP();
        }

        /// <summary>
        /// Open the character portrait from the EVEMon cache
        /// </summary>
        /// <returns>The character portrait as an Image object</returns>
        private Image GetPortraitFromCache()
        {
            if (m_id <= 0) return null;

            string cacheFileName = this.CachePath;
            if (!File.Exists(cacheFileName)) return null;

            try
            {
                var image = Image.FromFile(cacheFileName, true);
                return image;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
                return null;
            }
        }

        /// <summary>
        /// Download the image from CCP...
        /// <list type="bullet">
        /// <item>We assemble the url for a CCP download and give it to ImageService.</item>
        /// <item>ImageService will first check its cache (%APPDATA%\Cache\Images), then download the url if no image was found in cache.</item>
        /// </list>
        /// </summary>
        private void UpdateCharacterImageFromCCP()
        {
            Cursor.Current = Cursors.WaitCursor;
            if (IsUpdating) return;
            IsUpdating = true;

            ImageService.GetCharacterImageAsync(m_id, new GetImageCallback(OnGotCharacterImageFromCCP));
        }

        /// <summary>
        /// We retrieve a portrait from the ImageService's cache or from the CCP url. We then save it to the portraits' cache (%APPDATA%\Cache).
        /// </summary>
        /// <param name="i">The retrieved image.</param>
        private void OnGotCharacterImageFromCCP(Image newImage)
        {
            // Restore cursor, then quit if download failed
            Cursor.Current = Cursors.Default;
            if (newImage == null)
            {
                IsUpdating = false;
                return;
            }

            // The image was retrieved, we save it to the cache
            SavePortraitToCache(newImage);
        }

        /// <summary>
        /// Save the specified image to the EVEMon cache as this character's portrait
        /// </summary>
        /// <param name="newImage">The new portrait image.</param>
        private void SavePortraitToCache(Image newImage)
        {
            // If this control only has a character ID, we just update the picture box right now
            if (m_character == null)
            {
                pictureBox.Image = newImage;
                this.IsUpdating = false;
                return;
            }

            // Save to the portraits cache and notify we changed this character's portrait
            try
            {
                // Save the image to a temp file
                string tempFileName = Path.GetTempFileName();
                using (FileStream fs = new FileStream(tempFileName, FileMode.Create))
                {
                    newImage.Save(fs, ImageFormat.Png);
                    fs.Flush();
                }

                // Overwrite the portrait cache file
                FileHelper.OverwriteOrWarnTheUser(tempFileName, this.CachePath, OverwriteOperation.Move);

                // Notify the other controls we updated this portrait
                EveClient.OnCharacterPortraitChanged(m_character);
            }
            catch (Exception e)
            {
                // TODO : Add a tooltip here
                ExceptionHandler.LogException(e, false);
            }
            finally
            {
                // Release the updating flag
                IsUpdating = false;
            }
        }
        #endregion


        #region Mechanisms related to the game folder
        /// <summary>
        /// Download the image from the EVE cache (in EVE Online client installation folder)
        /// </summary>
        private void UpdateCharacterFromEVECache()
        {
            this.IsUpdating = true;
            try
            {
                // If we don't have the game's portraits cache already, prompt the user
                if (String.IsNullOrEmpty(EveClient.EvePortraitCacheFolder))
                {
                    // Return if the user canceled
                    if (!ChangeEVEPortraitCache()) return;
                }

                // Now, seach in the game folder all matching files 
                // (different resolutions are available for every character)
                try
                {
                    // Retrieve all files in the EVE cache directory which matches "<characterId>*.png"
                    DirectoryInfo di = new DirectoryInfo(EveClient.EvePortraitCacheFolder);
                    FileInfo[] filesInEveCache = di.GetFiles(m_id.ToString() + "*.png");

                    // Displays an error message if none found.
                    if (filesInEveCache.Length == 0)
                    {
                        String message;

                        message = "No portraits for your character were found in the folder you selected." + Environment.NewLine + Environment.NewLine;
                        message += "Ensure that you have checked the following" + Environment.NewLine;
                        message += " - You have selected a valid EVE Folder (i.e. C:\\Program Files\\CCP\\EVE\\)" + Environment.NewLine;
                        message += " - You have logged into EVE using this folder";

                        MessageBox.Show(message, "Portrait Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    // Search for the biggest portrait
                    int bestSize = 0;
                    string bestFile = "";
                    int charIDLength = m_id.ToString().Length;
                    foreach (FileInfo file in filesInEveCache)
                    {
                        int sizeLength = (file.Name.Length - (file.Extension.Length + 1)) - charIDLength;
                        int imageSize = int.Parse(file.Name.Substring(charIDLength + 1, sizeLength));

                        if (imageSize > bestSize)
                        {
                            bestFile = file.FullName;
                            bestSize = imageSize;
                        }
                    }

                    // Open the largest image and save it
                    var image = Image.FromFile(bestFile);
                    SavePortraitToCache(image);
                }
                catch (Exception e)
                {
                    // TODO : Add a tooltip here
                    ExceptionHandler.LogException(e, false);
                }
            }
            finally
            {
                this.IsUpdating = false;
            }
        }


        /// <summary>
        /// Pops up a window for the user to select their EVE portrait cache folder.
        /// </summary>
        private static bool ChangeEVEPortraitCache()
        {
            using (EVEFolderWindow f = new EVEFolderWindow())
            {
                f.EVEFolder = EveClient.EvePortraitCacheFolder;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    EveClient.EvePortraitCacheFolder = f.EVEFolder;
                    return true;
                }
            }

            return false;
        }
        #endregion


        #region Controls and global events handler
        private void EveClient_CharacterPortraitChanged(object sender, CharacterChangedEventArgs e)
        {
            if (!this.Visible)
            {
                m_pendingUpdate = true;
                return;
            }

            var image = GetPortraitFromCache();
            if (image != null) pictureBox.Image = image;
            else pictureBox.Image = pictureBox.InitialImage;
        }

        private void miUpdatePicture_Click(object sender, EventArgs e)
        {
            UpdateCharacterImageFromCCP();
        }

        private void miUpdatePictureFromEVECache_Click(object sender, EventArgs e)
        {
            UpdateCharacterFromEVECache();
        }

        private void miSetEVEFolder_Click(object sender, EventArgs e)
        {
            ChangeEVEPortraitCache();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (this.IsUpdating) return;
            cmsPictureOptions.Show(MousePosition);
        }
        #endregion
    }
}
