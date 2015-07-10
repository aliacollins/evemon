using System;
using LogitechLcd.NET;

namespace EVEMon.LogitechG15
{
    internal static class LcdInterface
    {
        private static bool s_lcdAvailable;
        private static bool s_lcdInterfaceInitialized;
        private static bool s_result;

        private static readonly ILogitechLcd s_lgLcd;

        #region Static Constructor

        /// <summary>
        /// Initializes the <see cref="LcdInterface"/> class.
        /// </summary>
        static LcdInterface()
        {
            s_lgLcd = LogitechLcd.NET.LogitechLcd.Instance;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Opens the LCD.
        /// </summary>
        /// <param name="appName">Name of the app.</param>
        /// <returns></returns>
        internal static void Open(string appName)
        {
            try
            {
                if (s_lcdAvailable)
                    return;

                // Initialize interface to LCD library if needed
                if (!s_lcdInterfaceInitialized)
                {
                    // Initialize the library
                    s_result = s_lgLcd.Init(appName, (int)(LogitechLcdConstants.LogiLcdType.LogiLCDTypeMono));

                    // Is an LCD available?
                    if (!s_result)
                        return;

                    s_lcdInterfaceInitialized = true;
                }

                s_result = s_lgLcd.IsConnected((int)(LogitechLcdConstants.LogiLcdType.LogiLCDTypeMono));

                if (s_result)
                    s_lcdAvailable = true;
                else
                    s_lgLcd.Shutdown();
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (LogitechLcd.x##.dll)
                Console.Write("Open Caught Exception: ");
                Console.WriteLine(ex.Message);
                s_lgLcd.Shutdown();
            }
        }

        /// <summary>
        /// Closes the LCD.
        /// </summary>
        /// <returns></returns>
        internal static void Close()
        {
            try
            {
                s_lcdAvailable = false;
                s_lcdInterfaceInitialized = false;
                s_lgLcd.Shutdown();
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (LogitechLcd.x##.dll)
                Console.Write("Close Caught Exception: ");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Displays the bitmap on the LCD screen.
        /// </summary>
        /// <param name="sampleBitmap">The sampleBitmap.</param>
        /// <returns></returns>
        internal static void DisplayBitmap(ref byte[] sampleBitmap)
        {
            try
            {
                if (s_lcdInterfaceInitialized && !s_lcdAvailable)
                    Open("EVEMon");

                // Is an LCD available?
                if (!s_lcdAvailable)
                {
                    Close();
                    return;
                }

                // Display bitmap if LCD is found
               s_lgLcd.MonoSetBackground(sampleBitmap);
               s_lgLcd.Update();
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (LogitechLcd.x##.dll)
                Console.Write("DisplayBitmap Caught Exception: ");
                Console.WriteLine(ex.Message);
                Close();
            }
        }

        /// <summary>
        /// Reads the soft button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        internal static bool ReadSoftButton(int button)
        {
            try
            {
                // Has the LCD been disconnected?
                if (!s_lcdAvailable)
                    return false;

                s_result = s_lgLcd.IsButtonPressed(button);
            }
            catch (Exception ex)
            {
                // This might happen for a number of reasons .. most likely missing the lgLcd library (LogitechLcd.x##.dll)
                Console.Write("ReadSoftButton Caught Exception: ");
                Console.WriteLine(ex.Message);
                Close();
            }
            return s_result;
        }

        #endregion

    }
}