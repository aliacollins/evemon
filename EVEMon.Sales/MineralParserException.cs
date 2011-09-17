using System;

namespace EVEMon.Sales
{
    /// <summary>
    /// Thrown when something goes wrong with the mineral parser.
    /// </summary>
    [Serializable]
    public class MineralParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MineralParserException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MineralParserException(string message)
            : base(message)
        {
        }
    }
}