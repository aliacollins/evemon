using System;
using System.Threading;
using System.Threading.Tasks;

namespace EVEMon.Common.Helpers
{
    public static class TaskHelper
    {
        /// <summary>
        /// Runs the IO bound task asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running IO bound task using <![CDATA[TaskCompletionSource<T>()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task RunIOBoundTaskAsync(Action action,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
            => ExecuteIOBoundTaskCore<object>(new Task(action), cancellationToken, continuationOptions, scheduler);

        /// <summary>
        /// Runs the IO bound task asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="state">The state.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running IO bound task using <![CDATA[TaskCompletionSource<T>()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task RunIOBoundTaskAsync(Action<object> action, object state = null,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
            => ExecuteIOBoundTaskCore<object>(new Task(action, state), cancellationToken, continuationOptions, scheduler);

        /// <summary>
        /// Runs the IO bound task asynchronous.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running IO bound task using <![CDATA[TaskCompletionSource<T>()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task RunIOBoundTaskAsync(Func<Task> function,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
            => ExecuteIOBoundTaskCore<object>(new Task<Task>(function), cancellationToken, continuationOptions, scheduler);

        /// <summary>
        /// Runs the IO bound task asynchronous.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running IO bound task using <![CDATA[TaskCompletionSource<T>()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task<TResult> RunIOBoundTaskAsync<TResult>(Func<TResult> function,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
            => ExecuteIOBoundTaskCore<TResult>(new Task<TResult>(function), cancellationToken, continuationOptions, scheduler);

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="taskToRun">The task to run.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuationOptions">The continuation options.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running IO bound task using <![CDATA[TaskCompletionSource<T>()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        private static Task<TResult> ExecuteIOBoundTaskCore<TResult>(Task taskToRun,
            CancellationToken cancellationToken = default(CancellationToken),
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler scheduler = null)
        {
            var tcs = new TaskCompletionSource<TResult>();

            try
            {
                taskToRun.Start();

                var genericTaskToRun = taskToRun as Task<TResult>;
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
                            tcs.TrySetResult(default(TResult));

                    }, cancellationToken, continuationOptions, scheduler ?? TaskScheduler.Current);
                }
            }
            catch (Exception exc)
            {
                tcs.TrySetException(exc);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Runs the compute bound task asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running compute bound task using <![CDATA[Task.Run()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task RunCPUBoundTaskAsync(Action action,
            CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(action, cancellationToken);

        /// <summary>
        /// Runs the compute bound task asynchronous.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running compute bound task using <![CDATA[Task.Run()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task RunCPUBoundTaskAsync(Func<Task> function,
            CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(function, cancellationToken);

        /// <summary>
        /// Runs the compute bound task asynchronous.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running compute bound task using <![CDATA[Task.Run()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task<TResult> RunCPUBoundTaskAsync<TResult>(Func<TResult> function,
            CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(function, cancellationToken);

        /// <summary>
        /// Runs the compute bound task asynchronous.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods purpose is to help developers understand
        /// the concept of running compute bound task using <![CDATA[Task.Run()]]>
        /// See more at: https://msdn.microsoft.com/en-us/library/hh873177.aspx
        /// </remarks>
        public static Task<TResult> RunCPUBoundTaskAsync<TResult>(Func<Task<TResult>> function,
            CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(function, cancellationToken);
    }
}
