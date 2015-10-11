using System.Diagnostics;
using System.IO;

namespace EVEMon.SDEToSQL
{
    internal class Program
    {
        /// <summary>
        /// The entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            using (StreamWriter traceStream = File.CreateText("trace.txt"))
            {
                TextWriterTraceListener traceListener = new TextWriterTraceListener(traceStream);
                Trace.Listeners.Add(traceListener);
                Trace.AutoFlush = true;

                Trace.WriteLine("SDEToSql.Started");
                Importer.Start(args);
                Trace.WriteLine("SDEToSql.Closed");
            }
        }
    }
}
