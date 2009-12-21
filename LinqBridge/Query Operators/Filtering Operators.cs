using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{
		public static IEnumerable<TSource> Distinct<TSource> (this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			// We can't use a HashSet<T> here, because we don't have access to FW3.5!
			var visitedElements = new Dictionary<TSource, object> ();

			foreach (TSource element in source)
				if (!visitedElements.ContainsKey (element))
				{
					visitedElements.Add (element, null);
					yield return element;
				}
		}

		public static IEnumerable<TSource> Skip<TSource> (this IEnumerable<TSource> source, int count)
		{
			if (source == null) throw new ArgumentNullException ("source");

			foreach (TSource element in source)
				if (count-- <= 0)
					yield return element;
		}

		public static IEnumerable<TSource> SkipWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			bool unsatisfied = true;
			foreach (TSource element in source)
			{
				if (unsatisfied) unsatisfied = predicate (element);
				if (!unsatisfied) yield return element;
			}
		}

		public static IEnumerable<TSource> SkipWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			bool unsatisfied = true;
			int i = 0;
			foreach (TSource element in source)
			{
				if (unsatisfied) unsatisfied = predicate (element, i++);
				if (!unsatisfied) yield return element;
			}
		}

		public static IEnumerable<TSource> Take<TSource> (this IEnumerable<TSource> source, int count)
		{
			if (source == null) throw new ArgumentNullException ("source");

			if (count <= 0) yield break;
			foreach (TSource element in source)
				if (count-- == 0)
					break;
				else
					yield return element;
		}

		public static IEnumerable<TSource> TakeWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			foreach (TSource element in source)
				if (predicate (element))
					yield return element;
				else
					break;
		}

		public static IEnumerable<TSource> TakeWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			int i = 0;
			foreach (TSource element in source)
				if (predicate (element, i++))
					yield return element;
				else
					break;
		}

		public static IEnumerable<TSource> Where<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			foreach (TSource element in source)
				if (predicate (element))
					yield return element;
		}

		public static IEnumerable<TSource> Where<TSource> (this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			int i = 0;
			foreach (TSource element in source)
				if (predicate (element, i++))
					yield return element;
		}		
	}
}
