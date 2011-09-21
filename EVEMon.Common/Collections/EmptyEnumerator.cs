using System;
using System.Collections;
using System.Collections.Generic;

namespace EVEMon.Common.Collections
{


    #region EmptyEnumerator<T>

    /// <summary>
    /// Implements an enumerator on a empty set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EmptyEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// Throws an exception.
        /// </summary>
        public T Current
        {
            get { throw new InvalidOperationException("The enumerator is empty"); }
        }

        /// <summary>
        /// Dispose the class (do nothing).
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        object IEnumerator.Current
        {
            get { throw new InvalidOperationException("The enumerator is empty"); }
        }

        /// <summary>
        /// Move to the next element, always return false.
        /// </summary>
        /// <returns>false</returns>
        public bool MoveNext()
        {
            return false;
        }

        /// <summary>
        /// Resets the enumerator (do nothing).
        /// </summary>
        public void Reset()
        {
        }
    }

    #endregion


    #region EmptyEnumerable<T>

    /// <summary>
    /// Cette classe implémente une collection vide et immuable.
    /// </summary>
    public class EmptyEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Singleton implementation.
        /// </summary>
        public EmptyEnumerable<T> Instance
        {
            get { return new EmptyEnumerable<T>(); }
        }

        /// <summary>
        /// Gets an empty enumerator.
        /// </summary>
        /// <returns>An empty enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }

        /// <summary>
        /// Gets an empty enumerator.
        /// </summary>
        /// <returns>An empty enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }
    }

    #endregion
}