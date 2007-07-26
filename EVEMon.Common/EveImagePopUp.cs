using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.IO;

namespace EVEMon.Common
{
    /// <summary>
    /// Displays 256 x 256 image of specified EveObject in a separate window
    /// </summary>
    public partial class EveImagePopUp : EVEMonForm
    {
        private const string titleBase = "EveMon Image Viewer";
        private EveObject m_imageSource = null;

        public EveImagePopUp(EveObject imageSource)
        {
            InitializeComponent();
            m_imageSource = imageSource;
            if (m_imageSource != null)
            {
                // Set window title
                this.Text = m_imageSource.Name + " - " + titleBase;
                eveImage.EveItem = m_imageSource;
            }
            else
                this.Text = titleBase;
        }

    }
}
