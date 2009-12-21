/****h* lglcdPinvoke
 * NAME
 *   lglcdPinvoke
 *
 * SYNOPSIS
 *   This file contains pinvoke style re-definitions for the original 
 *   lglcd.h API
 *
 * DESCRIPTION
 *   The LCD API is fairly flat, so this lends itself well to
 *   Platform Invoke style calls to unmanaged code
 *
 * MODIFICATION HISTORY
 *   10/21/2005  Added pinvoke to read button status  
 *   03/23/2006  Updated for 1.02 release, new location for lglcd.h file
 *   
 *******
 */

// include the Logitech LCD SDK header
#include <wtypes.h>
#include "../lgLCDSDK/src/lglcd.h"

using namespace Microsoft::Win32;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;

// ========== Standard Flat API Access using P/Invoke ====================

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI GetInterface(DWORD dwInterfaceVersion);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdInit(void);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdConnectW(IN OUT lgLcdConnectContextW *ctx);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdEnumerate(IN int connection, IN int index,
                            OUT lgLcdDeviceDesc *description);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdOpen(IN OUT lgLcdOpenContext *ctx);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdUpdateBitmap(IN int device,
                               IN const lgLcdBitmapHeader *bitmap,
                               IN DWORD priority);


// Reads the state of the soft buttons for the device.
[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdReadSoftButtons(IN int device, OUT DWORD *buttons);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdClose(IN int device);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdDisconnect(int connection);

[DllImport("lgLcdLibWrapper.dll", CharSet=CharSet::Unicode)]
extern "C" DWORD WINAPI lgLcdDeInit(void);


//** end of lglcd_pinvoke.h **************************
