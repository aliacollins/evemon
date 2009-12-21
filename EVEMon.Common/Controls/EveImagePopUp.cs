using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.IO;
using EVEMon.Common.Data;

namespace EVEMon.Common.Controls
{
    /// <summary>
    /// Displays 256 x 256 image of specified EveObject in a separate window
    /// </summary>
    public partial class EveImagePopUp : EVEMonForm
    {
        private const string titleBase = "EVEMon Image Viewer";
        private Item m_imageSource = null;

        public EveImagePopUp(Item imageSource)
        {
            InitializeComponent();
            m_imageSource = imageSource;
            if (m_imageSource != null)
            {
                // Set window title
                this.Text = String.Format("{0} - {1}", m_imageSource.Name, titleBase);
                eveImage.EveItem = m_imageSource;
            }
            else
                this.Text = titleBase;
        }

    }
}
