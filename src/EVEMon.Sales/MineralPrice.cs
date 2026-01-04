namespace EVEMon.Sales
{
    public sealed class MineralPrice
    {
        /// <summary>
        /// Gets or sets the name of the mineral.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price of the mineral.
        /// </summary>
        /// <value>The price.</value>
        public decimal Price { get; set; }
    }
}