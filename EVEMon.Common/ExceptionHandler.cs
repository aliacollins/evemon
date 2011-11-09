using System;
using System.Diagnostics;

namespace EVEMon.Common
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        public static void LogException(Exception e, bool handled)
        {
            LogException(e, handled ? "Handled exception" : "Unhandled exception");
        }

        /// <summary>
        /// Logs a rethrown exception.
        /// </summary>
        /// <param name="e">The e.</param>
        public static void LogRethrowException(Exception e)
        {
            LogException(e, "Exception caught and rethrown");
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="header">The header.</param>
        private static void LogException(Exception e, string header)
        {
            Trace.WriteLine(String.Empty);
            EveMonClient.Trace(header);
            Trace.Indent();
            Trace.WriteLine(e.ToString());
            Trace.WriteLine(String.Empty);
            Trace.Unindent();
        }
    }
}