using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
	// We'll stick with Microsoft's definition of IOrderedEnumerable to minimize confusion.

	public interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
	{
		IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey> (
			Func<TElement, TKey> keySelector,
			IComparer<TKey> comparer,
			bool descending);
	}
}
