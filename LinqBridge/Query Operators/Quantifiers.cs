using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{
		public static bool Contains<TSource> (this IEnumerable<TSource> source, TSource value)
		{
			if (source == null) throw new ArgumentNullException ("source");

			foreach (TSource element in source)
				if (object.Equals (element, value))
					return true;

			return false;
		}

		public static bool Any<TSource> (this IEnumerable<TSource> source)
		{
			return Any (source, x => true);
		}

		public static bool Any<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			foreach (TSource element in source)
				if (predicate (element)) 
					return true;

			return false;
		}

		public static bool All<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");
			
			foreach (TSource element in source)
				if (!predicate (element)) 
					return false;

			return true;
		}	

		public static bool SequenceEqual<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			if (first == null) throw new ArgumentNullException ("first");
			if (second == null) throw new ArgumentNullException ("second");

			using (var firstRator = second.GetEnumerator ())
			{
				foreach (TSource secondElement in first)
				{
					if (!firstRator.MoveNext ()) return false;
					if (!object.Equals (firstRator.Current, secondElement)) return false;
				}
				if (firstRator.MoveNext ()) return false;
			}
			return true;
		}
	}
}
