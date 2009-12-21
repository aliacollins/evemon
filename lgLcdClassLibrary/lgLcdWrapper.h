#pragma once

// Expose DLL calls, file lgLcdLibWrapper.dll should be stored in local directory (same as executable)
#include "lglcdPinvoke.h"

using namespace System;


class lgLcdWrapper
{
private:

    // same names used as simple.c to keep things easier for others to follow....
    lgLcdConnectContextW    connectContext;
    lgLcdDeviceDesc         deviceDescription;
    lgLcdOpenContext        openContext;
    lgLcdBitmap160x43x1     bmp;
    DWORD                   res;
    Boolean                 LCDAvailable;
    Boolean                 LCDInterfaceInitialized;
    long                    buttonStatus;

public:

    lgLcdWrapper::lgLcdWrapper()
    {
        LCDAvailable = FALSE;
        LCDInterfaceInitialized = FALSE;
        buttonStatus = 0;
    }

    // open LCD interface
    Boolean Open(LPCWSTR szApplicationName, BOOL isAutostartable, IntPtr configCallback, IntPtr buttonCallback);

    // display a bitmap on LCD (open if not already open)
    Boolean DisplayBitmap(unsigned char* samplebitmap, DWORD priority);
    
    // read the buttons on the LCD
    Boolean ReadSoftButtons(DWORD* buttons);
    
    // close LCD interface
    Boolean Close();
};


