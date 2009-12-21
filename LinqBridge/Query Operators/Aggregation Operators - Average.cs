using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{	
		// int

		public static double Average (this IEnumerable<int> source)
		{
			if (source == null) throw new ArgumentNullException ("source");
			return (double) source.Sum () / source.Count ();
		}

		public static double? Average (this IEnumerable<int?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int count = source.Count();
			if (count == 0) return null;
			return (double)source.Sum () / count;
		}

		public static double Average<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			return source.Select (selector).Average ();
		}

		public static double? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select (selector).Average ();
		}

		// long

		public static double Average (this IEnumerable<long> source)
		{
			if (source == null) throw new ArgumentNullException ("source");
			return (double)source.Sum () / source.Count ();
		}

		public static double? Average (this IEnumerable<long?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int count = source.Count ();
			if (count == 0) return null;
			return (double)source.Sum () / count;
		}

		public static double Average<TSource> (this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			return source.Select (selector).Average ();
		}

		public static double? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select (selector).Average ();
		}

		// float

		public static float Average (this IEnumerable<float> source)
		{
			if (source == null) throw new ArgumentNullException ("source");
			return source.Sum () / source.Count ();
		}

		public static float? Average (this IEnumerable<float?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int count = source.Count ();
			if (count == 0) return null;
			return source.Sum () / count;
		}

		public static float Average<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			return source.Select (selector).Average ();
		}

		public static float? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select (selector).Average ();
		}

		// double

		public static double Average (this IEnumerable<double> source)
		{
			if (source == null) throw new ArgumentNullException ("source");
			return source.Sum () / source.Count ();
		}

		public static double? Average (this IEnumerable<double?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int count = source.Count ();
			if (count == 0) return null;
			return source.Sum () / count;
		}

		public static double Average<TSource> (this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			return source.Select (selector).Average ();
		}

		public static double? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select (selector).Average ();
		}

		// decimal

		public static decimal Average (this IEnumerable<decimal> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int count = source.Count ();
			if (count == 0) ThrowNoElements ();		// decimal has no special "NaN" value, so we can't divide by zero.
			return source.Sum () / count;
		}

		public static decimal? Average (this IEnumerable<decimal?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int count = source.Count ();
			if (count == 0) return null;
			return source.Sum () / count;
		}

		public static decimal Average<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select (selector).Average ();
		}

		public static decimal? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select (selector).Average ();
		}
	}
}
