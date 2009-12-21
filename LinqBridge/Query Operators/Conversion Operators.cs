using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Linq
{
	public static partial class Enumerable
	{
		public static List<TSource> ToList<TSource> (this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException ("source");
			return new List<TSource> (source);
		}

		public static TSource [] ToArray<TSource> (this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException ("source");
			return source.ToList ().ToArray ();
		}
		
		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToDictionary<TSource, TKey, TSource> (keySelector, x => x, null);
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return source.ToDictionary<TSource, TKey, TSource> (keySelector, x => x, comparer);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector)
		{
			return source.ToDictionary (keySelector, elementSelector, null);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector, 
			IEqualityComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (keySelector == null) throw new ArgumentNullException ("keySelector");
			if (elementSelector == null) throw new ArgumentNullException ("elementSelector");

			Dictionary<TKey, TElement> d = new Dictionary<TKey, TElement> (comparer);
			
			foreach (TSource element in source) 
				d.Add (keySelector (element), elementSelector (element));

			return d;
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return Lookup<TKey, TSource>.Create (source, keySelector, x => x, null);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			IEqualityComparer<TKey> comparer)
		{
			return Lookup<TKey, TSource>.Create (source, keySelector, x => x, comparer);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector)
		{
			return Lookup<TKey, TElement>.Create (source, keySelector, elementSelector, null);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement> (
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector,
			Func<TSource, TElement> elementSelector, 
			IEqualityComparer<TKey> comparer)
		{
			return Lookup<TKey, TElement>.Create (source, keySelector, elementSelector, comparer);
		}

		public static IEnumerable<TSource> AsEnumerable<TSource> (this IEnumerable<TSource> source)
		{
			return source;
		}

		public static IEnumerable<TResult> OfType<TResult> (this IEnumerable source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			foreach (object obj in source)
				if (obj is TResult)
					yield return (TResult) obj;
		}

		public static IEnumerable<TResult> Cast<TResult> (this IEnumerable source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			foreach (object obj in source) 
				yield return (TResult) obj;
		}

	}
}
