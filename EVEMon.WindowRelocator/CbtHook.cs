using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace EVEMon.WindowRelocator
{
    internal class CbtHook : IDisposable
    {
        public CbtHook()
        {
            m_sem = new Semaphore(0, 1, "EVEMon-WindowShift-WindowCreation");
            m_waitHandle = ThreadPool.RegisterWaitForSingleObject(m_sem, new WaitOrTimerCallback(SemaphoreSignalled),
                                                                  null, Timeout.Infinite, false);

            m_lib = LoadLibrary("EVEMon.WinHook.dll");
            if (m_lib == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            UIntPtr funcPtr = GetProcAddress(m_lib, "CbtHookProc");
            if (funcPtr == UIntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Dispose();
                throw new Win32Exception(err);
            }
            m_hhk = SetWindowsHookEx(HookType.WH_CBT, funcPtr, m_lib, 0);
            if (m_hhk == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Dispose();
                throw new Win32Exception(err);
            }
        }

        ~CbtHook()
        {
            Dispose();
        }

        public event EventHandler<EventArgs> WindowCreated;

        private void OnWindowCreated()
        {
            if (WindowCreated != null)
            {
                WindowCreated(this, new EventArgs());
            }
        }

        private void SemaphoreSignalled(object state, bool timedOut)
        {
            OnWindowCreated();
        }

        private Semaphore m_sem;
        private RegisteredWaitHandle m_waitHandle;
        private IntPtr m_lib = IntPtr.Zero;
        private IntPtr m_hhk = IntPtr.Zero;

        [DllImport("kernel32", SetLastError=true)]
        private static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32")]
        private static extern bool FreeLibrary(IntPtr lib);

        [DllImport("kernel32", CharSet=CharSet.Ansi, ExactSpelling=true, SetLastError=true)]
        private static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        private enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [DllImport("user32", SetLastError=true)]
        private static extern IntPtr SetWindowsHookEx(HookType hook, UIntPtr callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32", SetLastError=true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        #region IDisposable Members
        public void Dispose()
        {
            if (m_hhk != IntPtr.Zero)
            {
                UnhookWindowsHookEx(m_hhk);
                m_hhk = IntPtr.Zero;
            }
            if (m_lib != IntPtr.Zero)
            {
                FreeLibrary(m_lib);
                m_lib = IntPtr.Zero;
            }
            if (m_waitHandle != null)
            {
                m_waitHandle.Unregister(m_sem);
                m_waitHandle = null;
            }
            if (m_sem != null)
            {
                m_sem.Close();
                m_sem = null;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}