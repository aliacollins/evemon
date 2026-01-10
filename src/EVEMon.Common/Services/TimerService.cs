using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEMon.Common.Abstractions.Services;

namespace EVEMon.Common.Services
{
    /// <summary>
    /// Implementation of <see cref="ITimerService"/> for WinForms applications.
    /// </summary>
    public sealed class TimerService : ITimerService
    {
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of <see cref="TimerService"/>.
        /// </summary>
        public TimerService()
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        /// <inheritdoc />
        public bool IsClosed => EveMonClient.Closed;

        /// <inheritdoc />
        public bool IsDataLoaded => EveMonClient.IsDataLoaded;

        /// <inheritdoc />
        public IDisposable ScheduleDelayed(TimeSpan delay, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var cts = new CancellationTokenSource();

            Task.Delay(delay, cts.Token).ContinueWith(t =>
            {
                if (!t.IsCanceled && !IsClosed)
                {
                    RunOnUIThread(action);
                }
            }, TaskScheduler.Default);

            return new DisposableAction(() => cts.Cancel());
        }

        /// <inheritdoc />
        public IDisposable ScheduleRepeating(TimeSpan interval, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var timer = new System.Windows.Forms.Timer
            {
                Interval = (int)interval.TotalMilliseconds
            };

            timer.Tick += (s, e) =>
            {
                if (!IsClosed)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine($"TimerService: Repeating action exception: {ex.Message}");
                    }
                }
            };

            timer.Start();

            return new DisposableAction(() =>
            {
                timer.Stop();
                timer.Dispose();
            });
        }

        /// <inheritdoc />
        public void RunOnUIThread(Action action)
        {
            if (action == null)
                return;

            if (_syncContext != null && SynchronizationContext.Current != _syncContext)
            {
                _syncContext.Post(_ => action(), null);
            }
            else
            {
                action();
            }
        }

        /// <inheritdoc />
        public Task RunOnUIThreadAsync(Action action)
        {
            if (action == null)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();

            if (_syncContext != null && SynchronizationContext.Current != _syncContext)
            {
                _syncContext.Post(_ =>
                {
                    try
                    {
                        action();
                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }, null);
            }
            else
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }

            return tcs.Task;
        }

        /// <summary>
        /// Helper class for creating disposable actions.
        /// </summary>
        private sealed class DisposableAction : IDisposable
        {
            private Action _action;
            private bool _disposed;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _action?.Invoke();
                _action = null;
            }
        }
    }
}
