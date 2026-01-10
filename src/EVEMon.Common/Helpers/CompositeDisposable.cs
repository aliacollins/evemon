using System;
using System.Collections.Generic;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Manages multiple IDisposable subscriptions and disposes them together.
    /// Used by ViewModels to track event subscriptions.
    /// </summary>
    public sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// Adds a disposable to be managed.
        /// </summary>
        /// <param name="disposable">The disposable to add.</param>
        public void Add(IDisposable disposable)
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));

            lock (_lock)
            {
                if (_disposed)
                {
                    disposable.Dispose();
                    return;
                }

                _disposables.Add(disposable);
            }
        }

        /// <summary>
        /// Gets the number of managed disposables.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _disposables.Count;
                }
            }
        }

        /// <summary>
        /// Disposes all managed disposables.
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed)
                    return;

                _disposed = true;

                foreach (var disposable in _disposables)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch
                    {
                        // Ignore disposal exceptions
                    }
                }

                _disposables.Clear();
            }
        }
    }
}
