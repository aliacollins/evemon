using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EVEMon.Common.MarketPricer
{
    public abstract class ItemPricer
    {

        /// <summary>
        /// Occurs when item prices updated.
        /// </summary>
        public abstract event EventHandler ItemPricesUpdated;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the price by type ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual double GetPriceByTypeID(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>
        /// The providers.
        /// </value>
        public static IEnumerable<ItemPricer> Providers
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetTypes().Where(
                    type => typeof(ItemPricer).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null).Select(
                        type => Activator.CreateInstance(type) as ItemPricer).OrderBy(pricer => pricer.Name);
            }
        }
    }
}
