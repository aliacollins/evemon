using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Linq
{
	public class Lookup<TKey, TElement> : ILookup<TKey, TElement>
	{
		Dictionary<TKey, Grouping<TKey, TElement>> _groupings;

		internal static Lookup<TKey, TElement> Create <TSource> (
			IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector, 
			IEqualityComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (keySelector == null) throw new ArgumentNullException ("keySelector");
			if (elementSelector == null) throw new ArgumentNullException ("elementSelector");

			var lookup = new Lookup<TKey, TElement> (comparer ?? EqualityComparer<TKey>.Default);

			foreach (TSource element in source)
			{
				TKey key = keySelector (element);				
				Grouping<TKey, TElement> grouping;

				if (!lookup._groupings.TryGetValue (key, out grouping))
					lookup._groupings.Add (key, grouping = new Grouping<TKey, TElement> (key));

				grouping.InnerList.Add (elementSelector (element));
			}

			return lookup;
		}

		Lookup (IEqualityComparer<TKey> comparer)
		{
			_groupings = new Dictionary<TKey, Grouping<TKey, TElement>> (comparer);
		}

		public int Count { get { return _groupings.Count; } }

		public IEnumerable<TElement> this [TKey key]
		{
			get
			{
				Grouping<TKey, TElement> result;
				if (_groupings.TryGetValue (key, out result))
					return result;
				else
					return Enumerable.Empty<TElement> ();
			}
		}

		public bool Contains (TKey key)
		{
			return _groupings.ContainsKey (key);
		}

		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator ()
		{
			foreach (var grouping in _groupings.Values)
				yield return grouping;
		}

		IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator (); }
	}
}
