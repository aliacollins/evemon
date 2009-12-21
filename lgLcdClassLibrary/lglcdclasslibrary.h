/****h* lgLcdClassLibrary
 * NAME
 *   lgLCDInterface
 *
 * SYNOPSIS
 *   A managed class wrapper for lgLCDWapper. This component can be
 *   built as a .NET assembly, and easily re-used in other .NET compatible
 *   languages.
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
 *     be in the same directory as the main EXE file (i.e.
 *     side-by-side sharing).
 *
 *   For VS 2003 .NET you must follow the steps below
 *
 *   PRB: Linker Warnings When You Build Managed Extensions for C++ DLL Projects
 *   http://support.microsoft.com/?id=814472
 *
 * CREATION DATE
 *   10/21/2005
 *
 * MODIFICATION HISTORY
 *   10/21/2005  Added pinvoke to read button status  
 *   
 *******
 */

// SOME USEFUL LINKS ON DELEGATES
//
// New C++ Features in VS 2005
// http://www.dotnetheaven.com/Uploadfile/amit_agrl/CplusFeatures08112005072823AM/CplusFeatures.aspx?ArticleID=2980f6ff-6308-46d2-af62-29bba6a512f8
// Function pointers and Delegates - Closing the gap!
// http://www.codeproject.com/managedcpp/FuncPtrDelegate.asp
// http://msdn.microsoft.com/vstudio/tour/vs2005_guided_tour/VS2005pro/Framework/CPlusCLI.htm

#pragma once

#include <vcclr.h>
#include "lgLcdWrapper.h"

// enable this to create a debug version of delegates that
// will write a message to console when it is called

//#define LGLCD_DEFINE_TEST_DELEGATES		1

namespace lgLcdClassLibrary
{
	// button delegate
	// The long type: In C#, the long data type is 64 bits, while in C++, it is 32 bits.
	public __delegate int ButtonDelegate(int device, DWORD dwButtons, IntPtr pContext);
	// configure delegate
	public __delegate int ConfigureDelegate(int connection, IntPtr pContext);

	public __gc class LCDInterface
	{

		lgLcdWrapper*		LCD;

		IntPtr bfp;		// pointer to button callback
		IntPtr cfp;		// pointer to config callback

#ifdef LGLCD_DEFINE_TEST_DELEGATES
		ButtonDelegate *bDelegate;
		ConfigureDelegate *cDelegate;
#endif

	public:

        // Several useful values are defined in lglcg.h, these are
        // re-defined here in a manner more accessible to non C++ .NET languages
    
        // Bitmap sizes (from lglcd.h)
        static const int lglcd_BMP_FORMAT_160x43x1 = LGLCD_BMP_FORMAT_160x43x1;
        static const int lglcd_BMP_WIDTH = LGLCD_BMP_WIDTH;
        static const int lglcd_BMP_HEIGHT = LGLCD_BMP_HEIGHT;

        // Priorities
        static const long lglcd_PRIORITY_IDLE_NO_SHOW = LGLCD_PRIORITY_IDLE_NO_SHOW;
        static const long lglcd_PRIORITY_BACKGROUND = LGLCD_PRIORITY_BACKGROUND;
        static const long lglcd_PRIORITY_NORMAL = LGLCD_PRIORITY_NORMAL;
        static const long lglcd_PRIORITY_ALERT = LGLCD_PRIORITY_ALERT;
        static const long lglcd_SYNC_UPDATE_MASK =  0x80000000;

        // Invalid handle definitions
        static const long lglcd_INVALID_CONNECTION = LGLCD_INVALID_CONNECTION;
        static const long lglcd_INVALID_DEVICE = LGLCD_INVALID_DEVICE;

        // Soft-Button masks
        static const long lglcd_BUTTON_BUTTON0 = LGLCDBUTTON_BUTTON0;
        static const long lglcd_BUTTON_BUTTON1 = LGLCDBUTTON_BUTTON1;
        static const long lglcd_BUTTON_BUTTON2 = LGLCDBUTTON_BUTTON2;
        static const long lglcd_BUTTON_BUTTON3 = LGLCDBUTTON_BUTTON3;

		// button delegate
		int buttonDelegateImplementation(int device, IN DWORD dwButtons, IN const PVOID pContext)
		{
			Console::WriteLine("sample buttonDelegate");
			return 0;
		}
		// configure delegate
		int configureDelegateImplementation(IN int connection, IN const PVOID pContext)
		{
			Console::WriteLine("sample configureDelegate");
			return 0;
		}

		LCDInterface::LCDInterface()
		{
			LCD = NULL;
			bfp = NULL;
			cfp = NULL;

#ifdef LGLCD_DEFINE_TEST_DELEGATES
			bDelegate = NULL;
			cDelegate = NULL;
#endif

		}

		// open LCD interface
		Boolean	AssignButtonDelegate(ButtonDelegate* bDelegate)
		{
			if (bDelegate != NULL)
				bfp = Marshal::GetFunctionPointerForDelegate(bDelegate);
			return true;
		}

		// open LCD interface
		Boolean	AssignConfigDelegate(ConfigureDelegate* cDelegate)
		{
			if (cDelegate != NULL)
				cfp = Marshal::GetFunctionPointerForDelegate(cDelegate);
			return true;
		}

		// open LCD interface
		Boolean	Open(String* appFriendlyName, Boolean isAutoStartable)
		{
			if (LCD != NULL) return false; 
			
            LCD = new lgLcdWrapper;
			
            // http://support.microsoft.com/?kbid=311259
            // get a pinned pointer to the string
            const __wchar_t __pin *lpsappFriendlyName = PtrToStringChars(appFriendlyName);

#ifdef LGLCD_DEFINE_TEST_DELEGATES
			// set up delegates
			bDelegate = new ButtonDelegate(this,&LCDInterface::buttonDelegateImplementation); //instance method
			cDelegate = new ConfigureDelegate(this,&LCDInterface::configureDelegateImplementation); //instance method

			// get pointer for the test versions of button and config delegates
			bfp = Marshal::GetFunctionPointerForDelegate (bDelegate);
			cfp = Marshal::GetFunctionPointerForDelegate (cDelegate);
//#else
//			bfp = NULL;
//			cfp = NULL;
#endif

            return LCD->Open(lpsappFriendlyName, (Boolean)isAutoStartable, cfp, bfp);
		}

		// display a bitmap on LCD (open if not already open)
		Boolean	DisplayBitmap(Byte* samplebitmap, Int64 priority)
		{
			if (LCD == NULL) return false; 
			// create a pinned pointer to unmanaged data type
			unsigned char __pin *pPinned = samplebitmap;
			Boolean returnStatus = LCD->DisplayBitmap(pPinned, (DWORD)priority);
			pPinned = 0;
            return returnStatus;
		}

		// close LCD interface
		Boolean	Close()
		{
			if (LCD == NULL) return false; 
			LCD->Close();
			delete LCD;
			LCD = NULL;
			return true;
		}

        // read soft buttons
		Boolean	ReadSoftButtons(Int64& buttons)
		{
            buttons = 0;
			if (LCD == NULL) return false; 
            DWORD buttonsDWORD;
			LCD->ReadSoftButtons(&buttonsDWORD);
            buttons = (long) buttonsDWORD;
			return true;
		}

	};
}
