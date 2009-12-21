using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Linq
{
	public static partial class Enumerable
	{
		// Single

		public static TSource Single<TSource> (this IEnumerable<TSource> source)
		{
			return source.Single (x => true, false);
		}

		public static TSource Single<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Single (predicate, false);
		}

		public static TSource SingleOrDefault<TSource> (this IEnumerable<TSource> source)
		{
			return source.Single (x => true, true);
		}

		public static TSource SingleOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Single (predicate, true);
		}

		static TSource Single<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate, bool orDefault)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			TSource answer = default (TSource);
			bool match = false;

			foreach (TSource element in source)
				if (predicate (element))
				{
					if (match) throw new InvalidOperationException ("Enumerable contains more than one matching element");
					match = true;
					answer = element;
				}

			if (!match && !orDefault) ThrowNoMatches ();
			return answer;
		}
		
		// First

		public static TSource First<TSource> (this IEnumerable<TSource> source)
		{
			return source.First (x => true, false);
		}

		public static TSource First<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.First (predicate, false);
		}

		public static TSource FirstOrDefault<TSource> (this IEnumerable<TSource> source)
		{
			return source.First (x => true, true);
		}

		public static TSource FirstOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.First (predicate, true);
		}

		static TSource First<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate, bool orDefault)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			foreach (TSource element in source)
				if (predicate (element))
					return element;

			if (!orDefault) ThrowNoMatches ();
			return default (TSource);
		}

		// Last

		public static TSource Last<TSource> (this IEnumerable<TSource> source)
		{
			return source.Last (x => true, false);
		}

		public static TSource Last<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Last (predicate, false);
		}

		public static TSource LastOrDefault<TSource> (this IEnumerable<TSource> source)
		{
			return source.Last (x => true, true);
		}

		public static TSource LastOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Last (predicate, true);
		}

		static TSource Last<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate, bool orDefault)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (predicate == null) throw new ArgumentNullException ("predicate");

			TSource answer = default (TSource);
			bool match = false;

			foreach (TSource element in source)
				if (predicate (element))
				{
					match = true;
					answer = element;
				}

			if (!match && !orDefault) ThrowNoMatches ();
			return answer;
		}

		// ElementAt

		public static TSource ElementAt<TSource> (this IEnumerable<TSource> source, int index)
		{
			return source.ElementAt (index, false);
		}

		public static TSource ElementAtOrDefault<TSource> (this IEnumerable<TSource> source, int index)
		{
			return source.ElementAt (index, true);
		}

		static TSource ElementAt<TSource> (this IEnumerable<TSource> source, int index, bool orDefault)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (index < 0) throw new ArgumentOutOfRangeException ("index");

			if (source is IList<TSource>) return ((IList<TSource>)source) [index];
			if (source is IList) return (TSource) ((IList)source) [index];

			foreach (TSource element in source)
				if (index-- == 0)
					return element;

			if (!orDefault) throw new ArgumentOutOfRangeException ("index");
			return default (TSource);
		}
	
	}
}
