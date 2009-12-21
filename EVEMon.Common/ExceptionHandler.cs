using System;
using System.Diagnostics;

namespace EVEMon.Common
{
    public static class ExceptionHandler
    {
        [Conditional("DEBUG")]
        public static void LogException(Exception e, bool handled)
        {
            string type;
            if (handled)
            {
                type = "Handled";
            }
            else
            {
                type = "Caught";
            }
            LogException(e, type);
        }

        [Conditional("DEBUG")]
        public static void LogRethrowException(Exception e)
        {
            LogException(e, "Caught and thrown");
        }

        private static void LogException(Exception e, string type)
        {
            StackTrace trace = new StackTrace(e, true);
            StackFrame frame = trace.GetFrame(trace.FrameCount - 1);
            Debug.WriteLine("");
            Debug.WriteLine(
                string.Format("{0} {1} at {2} line {3}", type, e.GetType().Name, frame.GetFileName(),
                              frame.GetFileLineNumber()), "Exception");
            Debug.WriteLine(e.Message, "Exception Message");
            Debug.WriteLine("");
        }
    }
}