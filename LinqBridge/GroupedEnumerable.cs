//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Collections;

//namespace System.Linq
//{
//     internal class GroupedEnumerable<TSource, TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
//     {
//          IEnumerable<TSource> _source;
//          Func<TSource, TKey> _keySelector;
//          Func<TSource, TElement> _elementSelector;
//          IEqualityComparer<TKey> _comparer;

//          // Methods
//          public GroupedEnumerable (
//               IEnumerable<TSource> source,
//               Func<TSource, TKey> keySelector, 
//               Func<TSource, TElement> elementSelector, 
//               IEqualityComparer<TKey> comparer)
//          {
//               if (source == null) throw new ArgumentNullException ("source");
//               if (keySelector == null) throw new ArgumentNullException ("keySelector");
//               if (elementSelector == null) throw new ArgumentNullException ("elementSelector");

//               _source = source;
//               _keySelector = keySelector;
//               _elementSelector = elementSelector;
//               _comparer = comparer;
//          }

//          public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator ()
//          {
//               return Lookup<TKey, TElement>.Create<TSource> (_source, _keySelector, _elementSelector, _comparer).GetEnumerator ();
//          }

//          IEnumerator IEnumerable.GetEnumerator ()
//          {
//               return this.GetEnumerator ();
//          }
//     }

//}
