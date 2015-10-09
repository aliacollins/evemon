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
            Importer.Import(args);
        }
    }
}
