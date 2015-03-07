using System;
using System.Runtime.InteropServices;

namespace lgLcdClassLibrary
{
    public static class NativeMethods
    {
        ///<summary>
        /// Invalid connection constant
        /// </summary>
        public const int LGLcdInvalidConnection = -1;

        /// <summary>
        /// Invalid device constant
        /// </summary>
        public const int LGLcdInvalidDevice = -1;

        /// <summary>
        /// Button mask for button 0 (first from left)
        /// </summary>
        public const uint LGLcdButton0 = 0x00000001;

        /// <summary>
        /// Button mask for button 1 (second from left)
        /// </summary>
        public const uint LGLcdButton1 = 0x00000002;

        /// <summary>
        /// Button mask for button 2 (third from left)
        /// </summary>
        public const uint LGLcdButton2 = 0x00000004;

        /// <summary>
        /// Button mask for button 3 (fourth from left)
        /// </summary>
        public const uint LGLcdButton3 = 0x00000008;

        /// <summary>
        /// Constant for G15 display resolution (160x43x1)
        /// </summary>
        public const uint LGLcdBmpFormat160X43X1 = 0x00000001;

        /// <summary>
        /// Constant for G15 display width
        /// </summary>
        public const uint LGLcdBmpWidth = 160;

        /// <summary>
        /// Constant for G15 display height
        /// </summary>
        public const uint LGLcdBmpHeight = 43;

        /// <summary>
        /// Lowest priority, disable displaying. Use this priority when you don’t have 
        /// anything to show.
        /// </summary>
        public const uint LGLcdPriorityIdleNoShow = 0;

        /// <summary>
        /// Priority used for low priority items.
        /// </summary>
        public const uint LGLcdPriorityBackground = 64;

        /// <summary>
        /// Normal priority, to be used by most applications most of the time.
        /// </summary>
        public const uint LGLcdPriorityNormal = 128;

        /// <summary>
        /// Highest priority. To be used only for critical screens, such as “your CPU 
        /// temperature is too high” 
        /// </summary>
        public const uint LGLcdPriorityAlert = 255;

        /// <summary>
        /// Function that should be called when the user wants to configure your 
        /// application. If no configuration panel is provided or needed, 
        /// leave this parameter NULL.
        /// </summary>
        /// <param name="connection">Current connection</param>
        /// <param name="pContext">Current context</param>
        /// <returns></returns>
        internal delegate int LgLcdOnConfigureCB(int connection, IntPtr pContext);

        /// <summary>
        /// Function that should be called when the state of the soft buttons changes. 
        /// If no notification is needed, leave this parameter NULL.
        /// </summary>
        /// <param name="device">Device sending buttons</param>
        /// <param name="dwButtons">Mask showing which buttons were pressed</param>
        /// <param name="pContext">Current context</param>
        /// <returns></returns>
        internal delegate int LgLcdOnSoftButtonsCB(int device, int dwButtons, IntPtr pContext);

        /// <summary>
        /// The lgLcdDeviceDesc structure describes the properties of an attached device. 
        /// This information is returned through a call to lgLcdEnumerate().
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct LgLcdDeviceDesc
        {
            /// <summary>
            /// Specifies the width of the display in pixels.
            /// </summary>
            internal readonly int Width;

            /// <summary>
            /// Specifies the height of the display in pixels.
            /// </summary>
            internal readonly int Height;

            /// <summary>
            /// Specifies the depth of the bitmap in bits per pixel.
            /// </summary>
            internal readonly int Bpp;

            /// <summary>
            /// Specifies the number of soft buttons that the device has.
            /// </summary>
            internal readonly int NumSoftButtons;
        }

        /// <summary>
        /// The lgLcdBitmapHeader exists at the beginning of any bitmap structure 
        /// defined in lgLcd. Following the header is the actual bitmap as an array 
        /// of bytes, as illustrated by lgLcdBitmap160x43x1.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct LgLcdBitmapHeader
        {
            /// <summary>
            /// Specifies the format of the structure following the header. 
            /// Currently, only LGLCD_BMP_FORMAT_160x43x1 is supported
            /// </summary>
            internal uint Format;
        }

        /// <summary>
        /// 160x43x1 bitmap.  This includes a header and an array
        /// of bytes (1 for each pixel.)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct LgLcdBitmap160X43X1
        {
            /// <summary>
            /// Header information telling what kind of bitmap this structure
            /// represents (currently only one format exists, see lgLcdBitmapHeader.)
            /// </summary>
            internal LgLcdBitmapHeader hdr;

            /// <summary>
            /// Contains the display bitmap with 160x43 pixels. Every byte represents
            /// one pixel, with &gt;=128 being “on” and &lt;128 being “off”.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6880)]
            internal byte[] pixels;
        }

        /// <summary>
        /// The lgLcdConfigureContext is part of the lgLcdConnectContext and 
        /// is used to give the library enough information to allow the user 
        /// to configure your application. The registered callback is called when the user 
        /// clicks the “Configure…” button in the LCDMon list of applications.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct LgLcdConfigureContext
        {
            /// <summary>
            /// Specifies a pointer to a function that should be called when the 
            /// user wants to configure your application. If no configuration panel 
            /// is provided or needed, leave this parameter NULL.
            /// </summary>
            internal LgLcdOnConfigureCB configCallback;

            /// <summary>
            /// Specifies an arbitrary context value of the application that is passed
            /// back to the client in the event that the registered configCallback 
            /// function is invoked.
            /// </summary>
            internal IntPtr configContext;
        }

        /// <summary>
        /// The lgLcdConnectContext contains all the information that is needed to 
        /// connect your application to LCDMon through lgLcdConnect(). Upon successful connection, 
        /// it also contains the connection handle that has to be used in subsequent calls to 
        /// lgLcdEnumerate() and lgLcdOpen().
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct LgLcdConnectContext
        {
            /// <summary>
            /// Specifies a string that contains the “friendly name” of your application. 
            /// This name is presented to the user whenever a list of applications is shown.
            /// </summary>
            internal string appFriendlyName;

            /// <summary>
            /// Specifies whether your connection is temporary (.isPersistent = FALSE) or 
            /// whether it is a regular connection that should be added to the list 
            /// (.isPersistent = TRUE).
            /// </summary>
            internal bool isPersistent;

            /// <summary>
            /// Specifies whether your application can be started by LCDMon or not.
            /// </summary>
            internal bool isAutostartable;

            /// <summary>
            /// Specifies context that is necessary to call back for configuration of 
            /// your application. See lgLcdConfigureContext for more details.
            /// </summary>
            internal LgLcdConfigureContext onConfigure;

            /// <summary>
            /// Upon successful connection, this member holds the “connection handle” 
            /// which is used in subsequent calls to lgLcdEnumerate() and lgLcdOpen(). 
            /// A value of LGLCD_INVALID_CONNECTION denotes an invalid connection.
            /// </summary>
            internal int connection;
        }

        /// <summary>
        /// The lgLcdSoftbuttonsChangedContext is part of the lgLcdOpenContext and 
        /// is used to give the library enough information to allow changes in the 
        /// state of the soft buttons to be signaled into the calling application 
        /// through a callback.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct LgLcdSoftbuttonsChangedContext
        {
            /// <summary>
            /// Specifies a pointer to a function that should be called when the 
            /// state of the soft buttons changes. If no notification is needed, 
            /// leave this parameter NULL.
            /// </summary>
            internal LgLcdOnSoftButtonsCB softbuttonsChangedCallback;

            /// <summary>
            /// Specifies an arbitrary context value of the application that is 
            /// passed back to the client in the event that soft buttons are being 
            /// pressed or released. The new value of the soft buttons is reported 
            /// in the dwButtons parameter of the callback function.
            /// </summary>
            internal IntPtr softbuttonsChangedContext;
        }

        /// <summary>
        /// The lgLcdOpenContext contains all the information that is needed to open 
        /// a specified LCD display through lgLcdOpen(). Upon successful completion 
        /// of the open it contains the device handle that has to be used in subsequent 
        /// calls to lgLcdReadSoftButtons(), lgLcdUpdateBitmap() and lgLcdClose().
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct LgLcdOpenContext
        {
            /// <summary>
            /// Specifies the connection (previously opened through lgLcdConnect()) which 
            /// this lgLcdOpen() call is for.
            /// </summary>
            internal int connection;

            /// <summary>
            /// Specifies the index of the device to open (see lgLcdEnumerate() for details).
            /// </summary>
            internal int index;

            /// <summary>
            /// Specifies the details for the callback function that should be invoked when
            /// device has changes in its soft button status, i.e. the user has pressed or
            /// a soft button. For details, see lgLcdSoftbuttonsChangedContext.
            /// </summary>
            internal LgLcdSoftbuttonsChangedContext onSoftbuttonsChanged;

            /// <summary>
            /// Upon successful opening, this member holds the device handle which is used 
            /// in subsequent calls to lgLcdReadSoftButtons(), lgLcdUpdateBitmap() and 
            /// lgLcdClose(). A value of LGLCD_INVALID_DEVICE denotes an invalid device.
            /// </summary>
            internal int device;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static uint LGLcdSyncUpdate(uint priority)
        {
            return 0x80000000 | priority;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static uint LGLcdAsyncUpdate(uint priority)
        {
            return priority;
        }

        /// <summary>
        /// The lgLcdInit() function initializes the Logitech LCD library. You must call this 
        /// function prior to any other function of the library.
        /// </summary>
        /// <remarks>
        /// No other function in the library can be called before lgLcdInit() is executed. 
        /// For result codes RPC_S_SERVER_UNAVAILABLE, ERROR_OLD_WIN_VERSION, and 
        /// ERROR_NO_SYSTEM_RESOURCES, the calling application can safely assume 
        /// that the machine it is running on is not set up to do LCD output and therefore 
        /// disable its LCD-related functionality.
        /// </remarks>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// RPC_S_SERVER_UNAVAILABLE
        ///     The Logitech LCD subsystem is not available (this is the case for systems that
        ///     don’t have the software installed)
        /// ERROR_OLD_WIN_VERSION
        ///     Attempted to initialize for Windows 9x. The library only works on Windows 2000 
        ///     and above.
        /// ERROR_NO_SYSTEM_RESOURCES
        ///     Not enough system resources.
        /// ERROR_ALREADY_INITIALIZED
        ///     lgLcdInit() has been called before.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdInit();

        /// <summary>
        /// Use lgLcdDeInit() after you are done using the library in order to release all resources 
        /// that were allocated during lgLcdInit().
        /// </summary>
        /// <remarks>
        /// All resources that were allocated during use of the library will be released when 
        /// this function is called. After this function has been called, no further calls to
        /// the library are permitted with the exception of lgLcdInit().
        /// </remarks>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// This function does not fail.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdDeInit();

        /// <summary>
        /// Use lgLcdConnect() to establish a connection to the LCD monitor process. This 
        /// connection is required for any other function to find, open and communicate with LCDs.
        /// </summary>
        /// <remarks>
        /// A connection needs to be established for an application to start using LCD 
        /// devices. lgLcdConnect() attempts to establish that connection. If the LCD 
        /// Monitor process is not running (either because it has not been started or not 
        /// installed (the user is using a different keyboard)), the connection attempt 
        /// will not succeed. In that case, your application should consider running without 
        /// any LCD support.
        /// 
        /// Since a string is part of the connection context, this function exists in an ANSI
        /// and a UNICODE version. The header file picks the appropriate version depending on
        /// whether the symbol UNICODE is present or not.
        /// </remarks>
        /// <param name="ctx">
        /// Pointer to a structure which holds all the relevant information about the connection 
        /// which you wish to establish. Upon calling, all fields except the “connection” member 
        /// need to be filled in; on return from the function, the “connection” member will be set. 
        /// See lgLcdConnectContext for details.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_SERVICE_NOT_ACTIVE
        ///     lgLcdInit() has not been called yet.
        /// ERROR_INVALID_PARAMETER
        ///     Either ctx or ctx->appFriendlyName are NULL.
        /// ERROR_FILE_NOT_FOUND
        ///     LCDMon is not running on the system.
        /// ERROR_ALREADY_EXISTS
        ///     The same client is already connected.
        /// RPC_X_WRONG_PIPE_VERSION
        ///     LCDMon does not understand the protocol.
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdConnect(ref LgLcdConnectContext ctx);

        /// <summary>
        /// Use lgLcdDisconnect() to close an existing connection to the LCD monitor process.
        /// </summary>
        /// <remarks>
        /// Closing a connection invalidates all devices that have been opened using that connection. 
        /// These invalid handles, if used after closing the connection, will cause errors to be 
        /// returned by calls to lgLcdUpdateBitmap(), lgLcdReadSoftButtons(), and lgLcdClose().
        /// 
        /// Additionally, if a callback for configuration had been registered in the call to 
        /// lgLcdConnect(), it will not be called anymore.
        /// </remarks>
        /// <param name="connection">
        /// Specifies the connection handle that was returned from a previous successful call 
        /// to lgLcdConnect()
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_SERVICE_NOT_ACTIVE
        ///     lgLcdInit() has not been called yet.
        /// ERROR_INVALID_PARAMETER
        ///     Specified connection handle does not exist.
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdDisconnect(int connection);

        /// <summary>
        /// The lgLcdEnumerate() function is used to retrieve information about all the 
        /// currently attached and supported Logitech LCD devices.
        /// </summary>
        /// <remarks>
        /// The connection parameter is returned by a call to lgLcdConnect().
        /// 
        /// To enumerate the attached devices, you should call lgLcdEnumerate() and 
        /// pass in 0 as index parameter. On subsequent calls, increase the index 
        /// parameter by 1 until the function returns ERROR_NO_MORE_ITEMS.
        /// </remarks>
        /// <param name="connection">Specifies the connection that this enumeration refers to.</param>
        /// <param name="index">Specifies which device information is requested. See Remarks.</param>
        /// <param name="description">Points to an lgLcdDeviceDesc structure which will be filled with information about the device.</param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_SERVICE_NOT_ACTIVE
        ///     lgLcdInit() has not been called yet.
        /// ERROR_NO_MORE_ITEMS
        ///     There are no more devices to be enumerated. If this error is returned on the first 
        ///     call, then there are no devices attached.
        /// ERROR_INVALID_PARAMETER
        ///     The description pointer is NULL.
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdEnumerate(int connection, int index, out LgLcdDeviceDesc description);

        /// <summary>
        /// The lgLcdOpen() function starts the communication with an attached device. You have 
        /// to call this function before retrieving button information or updating LCD bitmaps.
        /// </summary>
        /// <remarks>
        /// A handle retrieved through this function stays valid until either of the following conditions occurs:
        /// <list type="unordered">
        /// <item>
        ///     The device has been unplugged. Any operation with the given handle will result in an 
        ///     error code of ERROR_DEVICE_NOT_CONNECTED.
        /// </item>
        /// <item>
        ///     The handle has been closed using lgLcdClose().
        /// </item>
        /// </list>
        /// Part of the opening context is a callback that can be pointed to a function that is to 
        /// be called when soft button changes take place on the LCD. This callback is issued when 
        /// the LCD’s soft buttons change while your application owns the LCD space. See the 
        /// definition of lgLcdOpenContext and lgLcdSoftbuttonsChangedContext for details.
        /// </remarks>
        /// <param name="ctx">
        /// Specifies a pointer to a structure with all the information that is needed to open 
        /// the device. See lgLcdOpenContext for details. Before calling lgLcdOpen(), all fields 
        /// must be set, except the “device” member. Upon successful return, the “device” member 
        /// contains the device handle that can be used in subsequent calls to lgLcdUpdateBitmap(), 
        /// lgLcdReadSoftButtons(), and lgLcdClose().
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_SERVICE_NOT_ACTIVE
        ///     lgLcdInit() has not been called yet.
        /// ERROR_INVALID_PARAMETER
        ///     Either ctx is NULL, or ctx->connection is not valid, or ctx->index does not hold a valid device.
        /// ERROR_ALREADY_EXISTS
        ///     The specified device has already been opened in the given connection.
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdOpen(ref LgLcdOpenContext ctx);

        /// <summary>
        /// The lgLcdClose() function stops the communication with the previously opened device.
        /// </summary>
        /// <remarks>
        /// After calling lgLcdClose, the soft button callback specified in the call to lgLcdOpen() 
        /// will not be called anymore.
        /// </remarks>
        /// <param name="device">
        /// Specifies the device handle retrieved in the lgLcdOpenContext by a previous call to lgLcdOpen().
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_SERVICE_NOT_ACTIVE
        ///     lgLcdInit() has not been called yet.
        /// ERROR_INVALID_PARAMETER
        ///     The specified device handle is invalid.
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdClose(int device);

        /// <summary>
        /// The lgLcdReadSoftButtons() function reads the current state of the soft buttons 
        /// for the specified device.
        /// </summary>
        /// <remarks>
        /// The resulting DWORD contains the current state of the soft buttons, 1 bit per 
        /// button. You can use the mask definitions LGLCDBUTTON_BUTTON0 through 
        /// LGLCDBUTTON_BUTTON3 to check for any particular button in the DWORD.
        /// 
        /// If your application is not being currently displayed, you will receive a 
        /// resulting “buttons” DWORD of 0, even if some soft buttons are being pressed. 
        /// This is in order to avoid users inadvertently interacting with an application that 
        /// does not presently show on the display.
        /// </remarks>
        /// <param name="device">Specifies the device handle for which to read the soft button state.</param>
        /// <param name="buttons">
        /// Specifies a pointer to a DWORD that receives the state of the soft buttons at the 
        /// time of the call. See comments for details.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_SERVICE_NOT_ACTIVE
        ///     lgLcdInit() has not been called yet.
        /// ERROR_INVALID_PARAMETER
        ///     The specified device handle or the result pointer is invalid.
        /// ERROR_DEVICE_NOT_CONNECTED
        ///     The specified device has been disconnected.
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdReadSoftButtons(int device, out int buttons);

        /// <summary>
        /// The lgLcdUpdateBitmap() function updates the bitmap of the device.
        /// </summary>
        /// <remarks>
        /// The bitmap header parameter should point to an actual bitmap. The current revision of the 
        /// library defines a structure called lgLcdBitmap160x43x1 which holds as a first member a 
        /// bitmap header. You would typically instantiate once of these structures, set the hdr.Format 
        /// to LGLCD_BMP_FORMAT_160x43x1, then fill in the bitmap to be displayed in the pixels[] member. 
        /// Finally, you call lgLcdUpdateBitmap(… &yourBitmap.hdr …) to issue the bitmap update. Future 
        /// versions of the SDK could have additional bitmap types declared, but all of them will have 
        /// the same header at the beginning.
        /// 
        /// At any given time there may be multiple applications attempting to display a bitmap on the LCD.
        /// The priority parameter is a hint for LCDMon’s display scheduling algorithm. In a scenario 
        /// where there is contention for screen display time, LCDMon needs to determine which application’s 
        /// bitmap to display. In order to aid this scheduling, it can (but depending on user settings
        /// might not) take into account the hints that an application gives through the priority parameter. 
        /// It is highly advisable that your application gives the appropriate priority for any given screen 
        /// update to improve the user experience. A well-behaved LCD-enabled application should not use 
        /// high priorities except for alerts.
        /// 
        /// The difference between asynchronous and synchronous updates is as follows: an asynchronous 
        /// update will place the bitmap to be displayed into LCDMon and return immediately, before the 
        /// bitmap is actually sent out to the device. For synchronous updates, the call to 
        /// lgLcdUpdateBitmap() will only return after the bitmap has been sent out to the device,
        /// which takes 30 milliseconds or more. In case the application currently does not show on 
        /// the LCD because another application is displayed, the synchronous update returns after a 
        /// time that is similar to an update when the application is visible. If the macro 
        /// LGLCD_SYNC_COMPLETE_WITHIN_FRAME() is used, an error is returned to the calling 
        /// application when this condition arises.
        /// 
        /// The benefit of using the synchronous update is that your application will run “locked” with 
        /// the LCD updates. It will be suspended for the entire duration of writing to the screen, 
        /// and only get to run when the display is ready to accept a new screen. A “mini-game” on the 
        /// LCD would profit from this behavior in order to get the highest possible frame rates while 
        /// minimizing CPU usage.
        /// 
        /// The asynchronous updates are beneficial to applications that don’t care about the exact 
        /// sequence and timing of screen updates and have many other things to do. They just deposit 
        /// a bitmap to be sent to the device every once in a while and don’t worry about it actually 
        /// going out and being in sync with this event.
        /// </remarks>
        /// <param name="device">Specifies the device handle for which to update the display.</param>
        /// <param name="bitmap">Specifies a pointer to a bitmap header structure. See comments for details.</param>
        /// <param name="priority">
        /// Specifies a priority hint for this screen update, as well as whether the update should 
        /// take place synchronously or asynchronously. See comments for details.
        /// The following priorities are defined:
        /// 
        /// LGLCD_PRIORITY_IDLE_NO_SHOW
        ///     Lowest priority, disable displaying. Use this priority when you don’t have 
        ///     anything to show.
        /// LGLCD_PRIORITY_BACKGROUND
        ///     Priority used for low priority items.
        /// LGLCD_PRIORITY_NORMAL
        ///     Normal priority, to be used by most applications most of the time.
        /// LGLCD_PRIORITY_ALERT
        ///     Highest priority. To be used only for critical screens, such as “your CPU 
        ///     temperature is too high”
        /// 
        /// In addition, there are three macros that can be used to indicate whether the screen 
        /// should be updated synchronously (LGLCD_SYNC_UPDATE()) or asynchronously (LGLCD_ASYNC_UPDATE()). 
        /// When using synchronous update the LCD library can notify the calling application of whether 
        /// the bitmap was displayed or not on the LCD, using the macro (LGLCD_SYNC_COMPLETE_WITHIN_FRAME()). 
        /// Use these macros to indicate the behavior of the library.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_SERVICE_NOT_ACTIVE
        ///     lgLcdInit() has not been called yet.
        /// ERROR_INVALID_PARAMETER
        ///     The specified device handle, the bitmap header
        ///     pointer or the type of bitmap is invalid.
        /// ERROR_DEVICE_NOT_CONNECTED
        ///     The specified device has been disconnected.
        /// ERROR_ACCESS_DENIED
        ///     Synchronous operation was not displayed on the LCD within the frame interval 
        ///     (30 ms). This error code is only returned when the priority field of the 
        ///     lgLCDUpdateBitmap uses the macro LGLCD_SYNC_COMPLETE_WITHIN_FRAME().
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdUpdateBitmap(int device, ref LgLcdBitmap160X43X1 bitmap, uint priority);

        /// <summary>
        /// The lgLcdSetAsLCDForegroundApp() allows an application to become the one shown on 
        /// the LCD and prevents the LCD library from switching to other applications, when the 
        /// foregroundYesNoFlag parameter is set to LGLCD_LCD_FOREGROUND_APP_YES. When the calling 
        /// application calls this function with foregroundYesNoFlag parameter set to 
        /// LGLCD_LCD_FOREGROUND_APP_NO, the LCD library resumes its switching algorithm that 
        /// the user had chosen.
        /// </summary>
        /// <remarks>
        /// An application such as a game that wants to be shown on the LCD and does not want to be 
        /// swaped out by other applications, can use this call to become the frontmost application
        /// shown on the LCD. The LCD library will not swap out the application, except for other 
        /// applications that call this function at a later date. The frontmost application’s bitmaps 
        /// supplied using the lgLcdUpdateBitmap() call will all be displayed on the LCD.
        /// </remarks>
        /// <param name="device">Specifies the device handle for which the command is intented for.</param>
        /// <param name="foregroundYesNoFlag">
        /// Specifies whether the calling application is interested in becoming the frontmost 
        /// application shown on the LCD or it is trying to remove itself from being the frontmost. 
        /// See comments for details.
        /// 
        /// The following foregroundYesNoFlag values are defined:
        /// 
        /// LGLCD_LCD_FOREGROUND_APP_NO
        ///     Calling application does not want to be the only application shown on the LCD.
        /// LGLCD_LCD_FOREGROUND_APP_YES
        ///     Calling application wants to be the only application shown on the LCD.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is ERROR_SUCCESS.
        /// If the function fails, the return value can be one of the following:
        /// 
        /// ERROR_LOCK_FAILED
        ///     The operation could not be completed.
        /// Xxx
        ///     Other (system) error with appropriate error code.
        /// </returns>
        [DllImport("LgLcd.dll")]
        internal static extern int lgLcdSetAsLCDForegroundApp(int device, int foregroundYesNoFlag);
    }
}