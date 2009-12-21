using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace System.Linq
{
	public static partial class Enumerable
	{
		// Select
        [DebuggerStepThrough]
		public static IEnumerable<TResult> Select<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (selector == null) throw new ArgumentNullException ("selector");

			foreach (TSource element in source)
				yield return selector (element);
		}

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (selector == null) throw new ArgumentNullException ("selector");

			int i = 0;
			foreach (TSource element in source)
				yield return selector (element, i++);
		}

		// SelectMany

        [DebuggerStepThrough]
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
			this IEnumerable<TSource> source,
			Func<TSource, IEnumerable<TResult>> selector)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (selector == null) throw new ArgumentNullException ("selector");

			foreach (TSource element in source)
				foreach (TResult childElement in selector (element))
					yield return childElement;
		}

        [DebuggerStepThrough]
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
			this IEnumerable<TSource> source, 
			Func<TSource, int, IEnumerable<TResult>> selector)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (selector == null) throw new ArgumentNullException ("selector");

			int i = 0;
			foreach (TSource element in source)
			{
				foreach (TResult innerElement in selector (element, i))
					yield return innerElement;
				i++;
			}
		}

        [DebuggerStepThrough]
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
			this IEnumerable<TSource> source,
			Func<TSource, IEnumerable<TCollection>> collectionSelector,
			Func<TSource, TCollection, TResult> resultSelector)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (collectionSelector == null) throw new ArgumentNullException ("collectionSelector");
			if (resultSelector == null) throw new ArgumentNullException ("resultSelector");

			foreach (TSource element in source)
				foreach (TCollection innerElement in collectionSelector (element))
					yield return resultSelector (element, innerElement);
		}

	}
}
