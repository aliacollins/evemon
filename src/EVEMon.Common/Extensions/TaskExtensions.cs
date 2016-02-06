using System;
using System.Threading;
using System.Threading.Tasks;

namespace EVEMon.Common.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="taskToRun">The task to run.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        public static Task Execute(this Task taskToRun,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
            => Execute<object>(taskToRun, cancellationToken, continuationOptions, scheduler);

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskToRun">The task to run.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        public static Task<T> Execute<T>(this Task<T> taskToRun,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
            => Execute<T>((Task)taskToRun, cancellationToken, continuationOptions, scheduler);

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="taskToRun">The task to run.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        private static Task<T> Execute<T>(this Task taskToRun,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
        {
            var tcs = new TaskCompletionSource<T>();

            try
            {
                taskToRun.Start();

                var genericTaskToRun = taskToRun as Task<T>;
                if (genericTaskToRun != null)
                {
                    genericTaskToRun.ContinueWith(task =>
                    {
                        if (task.IsFaulted && task.Exception != null)
                            tcs.TrySetException(task.Exception);
                        else if (task.IsCanceled)
                            tcs.TrySetCanceled(cancellationToken);
                        else if (task.IsCompleted)
                            tcs.TrySetResult(task.Result);

                    }, cancellationToken, continuationOptions, scheduler ?? TaskScheduler.Current);
                }
                else
                {
                    taskToRun.ContinueWith(task =>
                    {
                        if (task.IsFaulted && task.Exception != null)
                            tcs.TrySetException(task.Exception);
                        else if (task.IsCanceled)
                            tcs.TrySetCanceled(cancellationToken);
                        else if (task.IsCompleted)
                            tcs.TrySetResult(default(T));

                    }, cancellationToken, continuationOptions, scheduler ?? TaskScheduler.Current);
                }
            }
            catch (Exception exc)
            {
                tcs.TrySetException(exc);
            }

            return tcs.Task;
        }
    }
}
