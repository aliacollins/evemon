using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{
		public static IEnumerable<TSource> Concat<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			if (first == null) throw new ArgumentException ("first");
			if (second == null) throw new ArgumentException ("second");

			foreach (TSource element in first) 
				yield return element;

			foreach (TSource element in second) 
				yield return element;
		}

		public static IEnumerable<TSource> Union<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Concat (second).Distinct ();
		}

		public static IEnumerable<TSource> Intersect<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			if (first == null) throw new ArgumentException ("first");
			if (second == null) throw new ArgumentException ("second");

			var firstDict = new Dictionary<TSource, bool>();

			foreach (TSource element in first)
				firstDict [element] = false;

			foreach (TSource element in second)
				if (firstDict.ContainsKey (element)) 
					firstDict [element] = true;
			
			foreach (KeyValuePair<TSource, bool> keyValue in firstDict)
				if (keyValue.Value)
					yield return keyValue.Key;
		}

		public static IEnumerable<TSource> Except<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			if (first == null) throw new ArgumentException ("first");
			if (second == null) throw new ArgumentException ("second");
			
			Dictionary<TSource, object> firstDict = new Dictionary<TSource, object> ();

			foreach (TSource element in first) 
				firstDict [element] = null;

			foreach (TSource element in second)
				firstDict.Remove (element);

			foreach (TSource element in firstDict.Keys)
				yield return element;
		}

	}
}
