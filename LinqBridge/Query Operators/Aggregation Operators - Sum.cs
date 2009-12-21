using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{	
		// int

		public static int Sum (this IEnumerable<int> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int tot = 0;
			foreach (int element in source) checked { tot += element; };
			return tot;
		}

		// It makes no sense to me that this returns a double? rather than a double, but that's the way the standard query operators work.
		public static int? Sum (this IEnumerable<int?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			int tot = 0;
			foreach (int? element in source) checked { tot += element ?? 0; };
			return tot;
		}

		public static int Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			return source.Select (selector).Sum ();
		}

		public static int? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select (selector).Sum ();
		}

		// long

		public static long Sum (this IEnumerable<long> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			long tot = 0;
			foreach (long element in source) checked { tot += element; }
			return tot;
		}

		public static long? Sum (this IEnumerable<long?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			long tot = 0;
			foreach (long? element in source) checked { tot += element ?? 0; }
			return tot;
		}

		public static long Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			return source.Select (selector).Sum ();
		}

		public static long? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select (selector).Sum ();
		}

		// float

		public static float Sum (this IEnumerable<float> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			float tot = 0;
			foreach (float element in source) tot += element;
			return tot;
		}

		public static float? Sum (this IEnumerable<float?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			float tot = 0;
			foreach (float? element in source) tot += element ?? 0;
			return tot;
		}

		public static float Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			return source.Select (selector).Sum ();
		}

		public static float? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select (selector).Sum ();
		}

		// double

		public static double Sum (this IEnumerable<double> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			double tot = 0;
			foreach (double element in source) tot += element;
			return tot;
		}

		public static double? Sum (this IEnumerable<double?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			double tot = 0;
			foreach (double? element in source)
				tot += element ?? 0;
			return tot;
		}

		public static double Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			return source.Select (selector).Sum ();
		}

		public static double ?Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select (selector).Sum ();
		}

		// decimal

		public static decimal Sum (this IEnumerable<decimal> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			decimal tot = 0;
			foreach (decimal element in source) tot += element;
			return tot;
		}

		public static decimal? Sum (this IEnumerable<decimal?> source)
		{
			if (source == null) throw new ArgumentNullException ("source");

			decimal tot = 0;
			foreach (decimal? element in source)
				tot += element ?? 0;
			return tot;
		}

		public static decimal Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select (selector).Sum ();
		}

		public static decimal? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select (selector).Sum ();
		}
	}
}
