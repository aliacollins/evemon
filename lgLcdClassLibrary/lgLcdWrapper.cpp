/****h* lgLcdWrapper
 * NAME
 *   lgLCDWrapper
 *
 * SYNOPSIS
 *   This class is a fairly simple wrapper
 *   to the unmanaged code that accesses the LCD.
 *
 * DESCRIPTION
 *   This program has a number of files that are additional
 *   to those used in the simple.c example. These are
 *   
 *     lgLcdPinvoke.h: This file contains pinvoke style 
 *     re-definitions for the original lglcd.h API.
 *
 *     lgLcdLibWrapper.dll: This is a thin wrapper DLL that finds
 *     the location of the installed Logitech G-series software,
 *     and forwards the calls to that software. This DLL needs to
 *     be in the same directory as the SimpleCPP.exe file (i.e.
 *     side-by-side sharing).
 *
 *   PRB: Linker Warnings When You Build Managed Extensions for C++ DLL Projects
 *   http://support.microsoft.com/?id=814472
 *
 * CREATION DATE
 *   11/13/2005
 *
 * MODIFICATION HISTORY
 *   
 *******
 */

// PRB: Linker Warnings When You Build Managed Extensions for C++ DLL Projects
// http://support.microsoft.com/?id=814472

#include "stdafx.h"
#include "lgLcdWrapper.h"

/****f* lgLcdWrapper/lgLcdWrapper::DisplayBitmap
 * NAME
 *  lgLcdWrapper::DisplayBitmap
 * FUNCTION
 *  Submits a bitmap for display on LCD
 * INPUTS
 *  samplebitmap  - bitmap for display
 *  priority      - priority of display
 ******
 */

Boolean lgLcdWrapper::DisplayBitmap(unsigned char* samplebitmap, DWORD priority)
{
    try 
    {
        if ((!LCDAvailable) && LCDInterfaceInitialized)
            // initialize the library
			Open(connectContext.appFriendlyName, connectContext.isAutostartable, connectContext.onConfigure.configCallback, openContext.onSoftbuttonsChanged.softbuttonsChangedCallback);

        // display bitmap if LCD is found
        if (LCDAvailable)
        {
            bmp.hdr.Format = LGLCD_BMP_FORMAT_160x43x1;
            memcpy((void *)&bmp.pixels, (void*) samplebitmap, sizeof(bmp.pixels));
            res = ::lgLcdUpdateBitmap(openContext.device, &bmp.hdr, priority);
            // has the LCD been disconnected?
            if (res != ERROR_SUCCESS)
            {
                // close the device
                res = ::lgLcdClose(openContext.device);
                LCDAvailable = false;
            }
        }
    }
    catch (Exception* ex)
    {
        // this might happen for a number of reasons .. most likely missing DLL lgLcdLibWrapper.dll
        Console::Write(S"DisplayBitmap Caught Exception: ");
        Console::WriteLine(ex->Message);
        LCDAvailable = false;
    }
    
    __finally
    {
    }
    return LCDAvailable;

}

/****f* lgLcdWrapper/lgLcdWrapper::Close
 * NAME
 *  lgLcdWrapper::Close
 * FUNCTION
 *  Close interface to LCD
 * INPUTS
 *  none
 ******
 */

Boolean lgLcdWrapper::Close()
{
    // let's close the device again
    res = ::lgLcdClose(openContext.device);
    // and take down the connection
    res = ::lgLcdDisconnect(connectContext.connection);
    // and shut down the library
    res = ::lgLcdDeInit();

    LCDAvailable = false;
    LCDInterfaceInitialized = false;
    
    return true;
}

/****f* lgLcdWrapper/lgLcdWrapper::Open
 * NAME
 *  lgLcdWrapper::Open
 * FUNCTION
 *  Open interface to LCD
 * INPUTS
 *  szApplicationName - friendly name for application
 *  isAutostartable - determines is the application can be started by LCDMon
 ******
 */

Boolean lgLcdWrapper::Open(LPCWSTR szApplicationName, BOOL isAutostartable, IntPtr configCallback, IntPtr buttonCallback)
{
    try 
    {
        if (!LCDAvailable)
        {
                // initialize interface to LCD library if needed
                if (!LCDInterfaceInitialized)
                {
                    // initialize the library
                    res = ::lgLcdInit();
                    //static LPCWSTR szApplicationName = L"lgLcdWrapperLibrary"; 
                    connectContext.appFriendlyName = szApplicationName;
                    connectContext.isAutostartable = isAutostartable;
                    connectContext.isPersistent = TRUE;
                    
                    // we might have a configuration screen
					if (configCallback != NULL)
						connectContext.onConfigure.configCallback = reinterpret_cast <lgLcdOnConfigureCB> (configCallback.ToPointer());
					else
	                    connectContext.onConfigure.configCallback = NULL;

                    connectContext.onConfigure.configContext = NULL;

                    // the "connection" member will be returned upon return
                    connectContext.connection = LGLCD_INVALID_CONNECTION;
                    
                    // and connect
                    res = ::lgLcdConnectW(&connectContext);
                    LCDInterfaceInitialized = TRUE;
                }

                // is an LCD available?
                res = ::lgLcdEnumerate(connectContext.connection, 0, &deviceDescription);
                if (res == ERROR_SUCCESS)
                {
                    // open it
                    ZeroMemory(&openContext, sizeof(openContext));
                    openContext.connection = connectContext.connection;
                    openContext.index = 0;

                    // we might have softbutton notification callback
					if (buttonCallback != NULL)
						openContext.onSoftbuttonsChanged.softbuttonsChangedCallback = reinterpret_cast <lgLcdOnSoftButtonsCB> (buttonCallback.ToPointer());
					else 
						openContext.onSoftbuttonsChanged.softbuttonsChangedCallback = NULL;

                    openContext.onSoftbuttonsChanged.softbuttonsChangedContext = NULL;
                    // the "device" member will be returned upon return
                    openContext.device = LGLCD_INVALID_DEVICE;
                    res = ::lgLcdOpen(&openContext);
                    if (res == ERROR_SUCCESS) LCDAvailable = true;
                }
        }
    }
    catch (Exception* ex)
    {
        // this might happen for a number of reasons .. most likely missing DLL lgLcdLibWrapper.dll
        Console::Write(S"Open Caught Exception: ");
        Console::WriteLine(ex->Message);
    }
    
    __finally
    {
    }

    return LCDAvailable;
}

    // 
/****f* lgLcdWrapper/lgLcdWrapper::ReadSoftButtons
 * NAME
 *  lgLcdWrapper::ReadSoftButtons
 * FUNCTION
 *  Read the buttons on the LCD
 * INPUTS
 *  none
 ******
 */

Boolean lgLcdWrapper::ReadSoftButtons(DWORD* buttons)
{
    buttonStatus = 0;
    try 
    {
        // display bitmap if LCD is found
        if (LCDAvailable)
        {
            res = ::lgLcdReadSoftButtons(openContext.device, buttons);
            // has the LCD been disconnected?
            if (res != ERROR_SUCCESS)
            {
                // close the device
                res = ::lgLcdClose(openContext.device);
                LCDAvailable = false;
            }
            else
            {
                buttonStatus = *buttons;
            }
        }
    }
    catch (Exception* ex)
    {
        // this might happen for a number of reasons .. most likely missing DLL lgLcdLibWrapper.dll
        Console::Write(S"ReadSoftButtons Caught Exception: ");
        Console::WriteLine(ex->Message);
        LCDAvailable = false;
    }
    
    __finally
    {
    }
    return LCDAvailable;
}
