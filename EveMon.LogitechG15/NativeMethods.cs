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
        public struct LgLcdDeviceDesc
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
        public struct LgLcdBitmap160X43X1
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
        public struct LgLcdConnectContext
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
        public struct LgLcdOpenContext
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
    }
}