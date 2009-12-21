using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
	public interface IGrouping<TKey, TElement> : IEnumerable<TElement>
	{
		TKey Key { get; }
	}
}
