using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{
		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>
			(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner,
			Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector,
			Func<TOuter, TInner, TResult> resultSelector)
		{
			if (outer == null) throw new ArgumentNullException ("outer");
			if (inner == null) throw new ArgumentNullException ("inner");
			if (outerKeySelector == null) throw new ArgumentNullException ("outerKeySelector");
			if (innerKeySelector == null) throw new ArgumentNullException ("innerKeySelector");
			if (resultSelector == null) throw new ArgumentNullException ("resultSelector");

			ILookup<TKey, TInner> lookup = inner.ToLookup (innerKeySelector);

			// We can use LINQ to write LINQ!  A SelectMany-style query over a lookup is the easiest way to  
			// implement a Join (see page 344, C# 3.0 in a Nutshell).
			return
				from outerItem in outer
				from innerItem in lookup [outerKeySelector (outerItem)]
				select resultSelector (outerItem, innerItem);
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult> (
			this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner,
			Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector,
			Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			if (outer == null) throw new ArgumentNullException ("outer");
			if (inner == null) throw new ArgumentNullException ("inner");
			if (outerKeySelector == null) throw new ArgumentNullException ("outerKeySelector");
			if (innerKeySelector == null) throw new ArgumentNullException ("innerKeySelector");
			if (resultSelector == null) throw new ArgumentNullException ("resultSelector");

			ILookup<TKey, TInner> lookup = inner.ToLookup (innerKeySelector);

			// We won't make this harder than it needs to be - a GroupJoin is just a projection over a lookup!
			return
				from outerItem in outer
				select resultSelector (outerItem, lookup [outerKeySelector (outerItem)]);
		}
	}
}
