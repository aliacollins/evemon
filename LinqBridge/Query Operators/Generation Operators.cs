using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public static partial class Enumerable
	{
		public static IEnumerable<int> Range (int start, int count)
		{
			for (int i = 0; i < count; i++) 
				yield return i + start;
		}

		public static IEnumerable<TResult> Repeat<TResult> (TResult element, int count)
		{
			for (int i = 0; i < count; i++) 
				yield return element;
		}

		public static IEnumerable<TResult> Empty<TResult> ()
		{
			yield break;
		}
	}
}
