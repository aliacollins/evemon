using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EVEMon.Common
{
    public class Util
    {
        public static void BrowserLinkClicked(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception)
            {
//                MessageBox.Show("You do not have a default browser configured. Please configure a default browser or visit " + url + " directly in your browser", "No Default Browser", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
    }
}
