using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.WindowsApi
{
    public static class G15Support
    {
        public static void Start()
        {
            if (OsFeatureCheck.IsWindowsNT)
            {
                G15Handler.Init();
            }
        }
    }
}
