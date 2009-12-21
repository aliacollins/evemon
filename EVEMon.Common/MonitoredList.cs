using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot("monitoredList")]
    public class MonitoredList<T> : IList<T>
        where T : class
    {
        private List<T> m_inner = new List<T>();

        public event EventHandler<ChangedEventArgs<T>> Changed;
        public event EventHandler<ClearedEventArgs<T>> Cleared;

        private void OnChanged(T item, ChangeType changeType)
        {
            if (Changed != null)
            {
                Changed(this, new ChangedEventArgs<T>(item, changeType));
            }
        }

        public void ForceUpdate(T item, ChangeType changeType)
        {
            OnChanged(item, changeType);
        }

        private void OnCleared(IEnumerable<T> items)
        {
            if (Cleared != null)
            {
                Cleared(this, new ClearedEventArgs<T>(items));
            }
            OnChanged(null, ChangeType.Cleared);
        }

        #region IList<T> Members
        public int IndexOf(T item)
        {
            return m_inner.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            m_inner.Insert(index, item);
            OnChanged(item, ChangeType.Added);
        }

        public void RemoveAt(int index)
        {
            T rItem = m_inner[index];
            m_inner.RemoveAt(index);
            OnChanged(rItem, ChangeType.Removed);
        }

        public T this[int index]
        {
            get { return m_inner[index]; }
            set
            {
                bool change = false;
                if (!m_inner[index].Equals(value))
                {
                    OnChanged(m_inner[index], ChangeType.Removed);
                    change = true;
                }
                m_inner[index] = value;
                if (change)
                {
                    OnChanged(value, ChangeType.Added);
                }
            }
        }
        #endregion

        #region ICollection<T> Members
        public void Add(T item)
        {
            m_inner.Add(item);
            OnChanged(item, ChangeType.Added);
        }

        public void Clear()
        {
            List<T> removed = new List<T>();
            foreach (T item in m_inner)
            {
                removed.Add(item);
            }
            m_inner.Clear();
            OnCleared(removed);
        }

        public bool Contains(T item)
        {
            return m_inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            bool result = m_inner.Remove(item);
            if (result)
            {
                OnChanged(item, ChangeType.Removed);
            }
            return result;
        }
        #endregion

        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator()
        {
            return m_inner.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_inner.GetEnumerator();
        }
        #endregion
    }
}
