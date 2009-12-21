using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    [System.Diagnostics.DebuggerNonUserCode]
	public static partial class Enumerable
	{
		static void ThrowNoElements ()
		{
			throw new InvalidOperationException ("Enumerable contains no elements");
		}

		static void ThrowNoMatches ()
		{
			throw new InvalidOperationException ("Enumerable contains no matching element");
		}
	}
}
