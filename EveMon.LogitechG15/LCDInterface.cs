using System;

namespace lgLcdClassLibrary
{
    public delegate int ButtonDelegate(int device, int dwButtons, IntPtr pContext);

    public delegate int ConfigureDelegate(int connection, IntPtr pContext);

    /// <summary>
    /// Logitech LCD class.  This class simply exposes the constants
    /// and functions provided by the logitech sdk.  No wrapping is done
    /// to ease any of this as the goal of this class is to simply reflect
    /// exactly what is done in the C API provided by logitech.
    /// 
    /// You will notice that the in-line documentation (comments) is pretty
    /// thorough (excessive probably.)  This is because the documentation was
    /// copied directly from the PDF documentation provided with the SDK.
    /// My hope was to keep this as thorough as possible for integrated VS.NET
    /// support, and to maybe(?) prevent you from having to have the PDF open
    /// and toggle back and forth.
    /// </summary>
    public static class LCDInterface
    {
        private static bool s_lcdAvailable;
        private static bool s_lcdInterfaceInitialized;
        private static int s_result;

        private static NativeMethods.lgLcdConnectContext s_connectContext;
        private static NativeMethods.lgLcdOpenContext s_openContext;
        private static NativeMethods.lgLcdBitmap160x43x1 s_lcdBitmap;
        private static NativeMethods.lgLcdOnConfigureCB s_configCallback;
        private static NativeMethods.lgLcdOnSoftButtonsCB s_buttonCallback;


        #region Public Methods

        /// <summary>
        /// Assigns the button delegate.
        /// </summary>
        /// <param name="bDelegate">The buttons delegate.</param>
        /// <returns></returns>
        public static bool AssignButtonDelegate(ButtonDelegate bDelegate)
        {
            if (bDelegate == null)
                return false;

            s_buttonCallback = bDelegate.Invoke;
            return true;
        }

        /// <summary>
        /// Assigns the config delegate.
        /// </summary>
        /// <param name="cDelegate">The config delegate.</param>
        /// <returns></returns>
        public static bool AssignConfigDelegate(ConfigureDelegate cDelegate)
        {
            if (cDelegate == null)
                return false;

            s_configCallback = cDelegate.Invoke;
            return true;
        }

        /// <summary>
        /// Opens the LCD.
        /// </summary>
        /// <param name="appName">Name of the app.</param>
        /// <param name="isAutostartable">if set to <c>true</c> [is autostartable].</param>
        /// <returns></returns>
        public static bool Open(string appName, bool isAutostartable)
        {
            try
            {
                if (!s_lcdAvailable)
                {
                    // Initialize interface to LCD library if needed
                    if (!s_lcdInterfaceInitialized)
                    {
                        // Initialize the library
                        s_result = NativeMethods.lgLcdInit();

                        s_connectContext.appFriendlyName = appName;
                        s_connectContext.isAutostartable = isAutostartable;
                        s_connectContext.isPersistent = true;

                        // We might have a configuration screen
                        s_connectContext.onConfigure.configCallback = s_configCallback;
                        s_connectContext.onConfigure.configContext = IntPtr.Zero;

                        // The "connection" member will be returned upon return
                        s_connectContext.connection = NativeMethods.LGLCD_INVALID_CONNECTION;

                        // Connect
                        s_result = NativeMethods.lgLcdConnect(ref s_connectContext);

                        s_lcdInterfaceInitialized = true;
                    }

                    // Is an LCD available?
                    NativeMethods.lgLcdDeviceDesc deviceDescription;
                    s_result = NativeMethods.lgLcdEnumerate(s_connectContext.connection, 0, out deviceDescription);
                    if (s_result == 0)
                    {
                        // Open it
                        s_openContext.connection = s_connectContext.connection;
                        s_openContext.index = 0;

                        // We might have softbutton notification callback
                        s_openContext.onSoftbuttonsChanged.softbuttonsChangedCallback = s_buttonCallback;
                        s_openContext.onSoftbuttonsChanged.softbuttonsChangedContext = IntPtr.Zero;

                        // The "device" member will be returned upon return
                        s_openContext.device = NativeMethods.LGLCD_INVALID_DEVICE;
                        s_result = NativeMethods.lgLcdOpen(ref s_openContext);

                        if (s_result == 0)
                            s_lcdAvailable = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (lgLcd.dll)
                Console.Write("Open Caught Exception: ");
                Console.WriteLine(ex.Message);
            }

            return s_lcdAvailable;
        }

        /// <summary>
        /// Closes the LCD.
        /// </summary>
        /// <returns></returns>
        public static bool Close()
        {
            try
            {
                // Let's close the device again
                s_result = NativeMethods.lgLcdClose(s_openContext.device);

                // Take down the connection
                s_result = NativeMethods.lgLcdDisconnect(s_connectContext.connection);

                // Shutdown the library
                s_result = NativeMethods.lgLcdDeInit();

                s_lcdAvailable = false;
                s_lcdInterfaceInitialized = false;
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (lgLcd.dll)
                Console.Write("Close Caught Exception: ");
                Console.WriteLine(ex.Message);
                s_lcdAvailable = false;
                s_lcdInterfaceInitialized = false;
            }

            return true;
        }

        /// <summary>
        /// Displays the bitmap on the LCD screen.
        /// </summary>
        /// <param name="samplebitmap">The samplebitmap.</param>
        /// <param name="priority">The priority.</param>
        /// <returns></returns>
        public static bool DisplayBitmap(ref byte[] samplebitmap, uint priority)
        {
            try
            {
                if (!s_lcdAvailable && s_lcdInterfaceInitialized)
                    Open(s_connectContext.appFriendlyName, s_connectContext.isAutostartable);

                // Display bitmap if LCD is found
                if (s_lcdAvailable)
                {
                    s_lcdBitmap.hdr.Format = NativeMethods.LGLCD_BMP_FORMAT_160x43x1;
                    s_lcdBitmap.pixels = samplebitmap;
                    s_result = NativeMethods.lgLcdUpdateBitmap(s_openContext.device, ref s_lcdBitmap, priority);

                    // Has the LCD been disconnected?
                    if (s_result != 0)
                    {
                        // Close the device
                        s_result = NativeMethods.lgLcdClose(s_openContext.device);
                        s_lcdAvailable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (lgLcd.dll)
                Console.Write("DisplayBitmap Caught Exception: ");
                Console.WriteLine(ex.Message);
                s_lcdAvailable = false;
            }

            return s_lcdAvailable;
        }

        /// <summary>
        /// Reads the soft buttons.
        /// </summary>
        /// <param name="buttons">The buttons.</param>
        /// <returns></returns>
        public static bool ReadSoftButtons(ref int buttons)
        {
            try
            {
                // Display bitmap if LCD is found
                if (s_lcdAvailable)
                {
                    s_result = NativeMethods.lgLcdReadSoftButtons(s_openContext.device, out buttons);

                    // Has the LCD been disconnected?
                    if (s_result != 0)
                    {
                        // Close the device
                        s_result = NativeMethods.lgLcdClose(s_openContext.device);
                        s_lcdAvailable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (lgLcd.dll)
                Console.Write("ReadSoftButtons Caught Exception: ");
                Console.WriteLine(ex.Message);
                s_lcdAvailable = false;
            }

            return s_lcdAvailable;
        }

        #endregion

    }
}