using System;
using System.Threading.Tasks;

namespace EVEMon.Common.Abstractions.Services
{
    /// <summary>
    /// Service interface for timer operations.
    /// Provides access to application timers and tick events without coupling to EveMonClient.
    /// </summary>
    public interface ITimerService
    {
        /// <summary>
        /// Gets whether the application is closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Gets whether data has been loaded.
        /// </summary>
        bool IsDataLoaded { get; }

        /// <summary>
        /// Schedules an action to be executed after a delay.
        /// </summary>
        /// <param name="delay">The delay before execution.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>A disposable that cancels the scheduled action when disposed.</returns>
        IDisposable ScheduleDelayed(TimeSpan delay, Action action);

        /// <summary>
        /// Schedules an action to be executed repeatedly at an interval.
        /// </summary>
        /// <param name="interval">The interval between executions.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>A disposable that stops the repeated action when disposed.</returns>
        IDisposable ScheduleRepeating(TimeSpan interval, Action action);

        /// <summary>
        /// Executes an action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        void RunOnUIThread(Action action);

        /// <summary>
        /// Executes an action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>A task that completes when the action has been executed.</returns>
        Task RunOnUIThreadAsync(Action action);
    }
}
