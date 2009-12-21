using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{
		// OrderBy:

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey> (
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			return OrderBy (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey> (
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, 
			IComparer<TKey> comparer)
		{
			return new OrderByEnumerable<TSource, TKey> (source, keySelector, null, false);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector)
		{
			return OrderByDescending (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector,
			IComparer<TKey> comparer)
		{
			return new OrderByEnumerable<TSource, TKey> (source, keySelector, null, true);
		}

		// ThenBy:

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey> (
			this IOrderedEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			return ThenBy (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey> (
			this IOrderedEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			IComparer<TKey> comparer)
		{
			return source.CreateOrderedEnumerable<TKey> (keySelector, comparer, false);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey> (
			this IOrderedEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector)
		{
			return ThenByDescending (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey> (
			this IOrderedEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, 
			IComparer<TKey> comparer)
		{
			return source.CreateOrderedEnumerable<TKey> (keySelector, comparer, true);
		}
	}
}
