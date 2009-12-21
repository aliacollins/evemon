using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{
		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector)
		{
			return source.GroupBy (keySelector, x => x, null);
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			IEqualityComparer<TKey> comparer)
		{
			return source.GroupBy (keySelector, x => x, comparer);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector)
		{
			return source.GroupBy (keySelector, elementSelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector, 
			IEqualityComparer<TKey> comparer)
		{
			return Lookup<TKey, TElement>.Create<TSource> (source, keySelector, elementSelector, comparer);
		}		
	}
}
