using System;
using System.Collections.Generic;
using System.Text;
using lgLcdClassLibrary;

namespace EVEMon.LogitechG15
{
    public enum LcdisplayPriority
    {
        Alert = LCDInterface.lglcd_PRIORITY_ALERT,
        Normal = LCDInterface.lglcd_PRIORITY_NORMAL,
        Background = LCDInterface.lglcd_PRIORITY_BACKGROUND,
        Idle = LCDInterface.lglcd_PRIORITY_IDLE_NO_SHOW
    }
}
