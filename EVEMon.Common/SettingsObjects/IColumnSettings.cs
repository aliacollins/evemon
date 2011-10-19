namespace EVEMon.Common.SettingsObjects
{
    public interface IColumnSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IColumnSettings"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        int Width { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        int Key { get; }
    }
}