using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	class ThenByEnumerable<TElement, TKey, TLastKey> : OrderByEnumerable<TElement, TKey>
	{
		public ThenByEnumerable (
			OrderByEnumerable<TElement, TLastKey> source,
			Func<TElement, TKey> keySelector, 
			IComparer<TKey> comparer,
			bool descending)
			: base (source, keySelector, comparer, descending)
		{
		}

		public OrderByEnumerable<TElement, TLastKey> OrderedSource { get { return (OrderByEnumerable<TElement, TLastKey>) Source; } }

		internal override int CompareElements (TElement e1, TElement e2)
		{
			// First compare elements using the preceding OrderBy operator in the chain.  (If the preceding operator is also
			// an instance of ThenByEnumerable, it will, in turn, look at its previous OrderBy operator). If we get a non-zero 
			// result back, we can ignore our own comparison logic:
			int result = OrderedSource.CompareElements (e1, e2);
			if (result != 0) return result;
			
			// All preceding OrderBy operators have decided that the two elements are in the same sorting position.
			// Now it's up to us to arbitrate!  We'll call upon our normal sorting logic - as defined in the base class.
			return base.CompareElements (e1, e2);
		}

		internal override IEnumerable<TElement> GetElementsToSort ()
		{
			// Rather than sorting the result of the previous OrderBy, we'll sort the *original* sequence, using a 
			// comparer that takes all keys into account at once:
			return OrderedSource.GetElementsToSort();
		}		
	}

}
