using System.Runtime.InteropServices;

namespace lgLcdClassLibrary
{
    public sealed class LgLcdx86 : ILgLcd
    {

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdInit")]
        private static extern int lgLcdInit_ext();

        int ILgLcd.lgLcdInit()
        {
            return lgLcdInit_ext();
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdDeInit")]
        private static extern int lgLcdDeInit_ext();

        int ILgLcd.lgLcdDeInit()
        {
            return lgLcdDeInit_ext();
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdConnect")]
        private static extern int lgLcdConnect_ext(ref NativeMethods.LgLcdConnectContext ctx);

        int ILgLcd.lgLcdConnect(ref NativeMethods.LgLcdConnectContext ctx)
        {
            return lgLcdConnect_ext(ref ctx);
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdDisconnect")]
        private static extern int lgLcdDisconnect_ext(int connection);

        int ILgLcd.lgLcdDisconnect(int connection)
        {
            return lgLcdDisconnect_ext(connection);
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdEnumerate")]
        private static extern int lgLcdEnumerate_ext(int connection, int index, out NativeMethods.LgLcdDeviceDesc description);

        int ILgLcd.lgLcdEnumerate(int connection, int index, out NativeMethods.LgLcdDeviceDesc description)
        {
            return lgLcdEnumerate_ext(connection, index, out description);
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdOpen")]
        private static extern int lgLcdOpen_ext(ref NativeMethods.LgLcdOpenContext ctx);

        int ILgLcd.lgLcdOpen(ref NativeMethods.LgLcdOpenContext ctx)
        {
            return lgLcdOpen_ext(ref ctx);
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdClose")]
        private static extern int lgLcdClose_ext(int device);

        int ILgLcd.lgLcdClose(int device)
        {
            return lgLcdClose_ext(device);
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdReadSoftButtons")]
        private static extern int lgLcdReadSoftButtons_ext(int device, out int buttons);

        int ILgLcd.lgLcdReadSoftButtons(int device, out int buttons)
        {
            return lgLcdReadSoftButtons_ext(device, out buttons);
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdUpdateBitmap")]
        private static extern int lgLcdUpdateBitmap_ext(int device, ref NativeMethods.LgLcdBitmap160X43X1 bitmap, uint priority);

        int ILgLcd.lgLcdUpdateBitmap(int device, ref NativeMethods.LgLcdBitmap160X43X1 bitmap, uint priority)
        {
            return lgLcdUpdateBitmap_ext(device, ref bitmap, priority);
        }

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
        [DllImport("LgLcd.x86.dll", EntryPoint = "lgLcdSetAsLCDForegroundApp")]
        private static extern int lgLcdSetAsLCDForegroundApp_ext(int device, int foregroundYesNoFlag);

        int ILgLcd.lgLcdSetAsLCDForegroundApp(int device, int foregroundYesNoFlag)
        {
            return lgLcdSetAsLCDForegroundApp_ext(device, foregroundYesNoFlag);
        }
    }
}