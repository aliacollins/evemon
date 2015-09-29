using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Factories;

namespace EVEMon.Common.Threading
{
    /// <summary>
    /// This class provides the tools we need to apply the threading model in use in this assembly.
    /// It relies on an internal actor, which should be the UI actor. Most objects will have affinity with this thread and most actions will occur on this actor.
    /// <para>Besides, should the actor be null (for testing purposes), then a single-threaded mode will be enforced, all asynchronous calls being replaced by synchroneous ones.</para>
    /// <para>Finally, note that, in normal mode (actor not null), very few operations are performed on other threads. They are mainly network operations.</para>
    /// </summary>
    public static class Dispatcher
    {
        private static readonly Object s_syncLock = new Object();

        private static IActorTimer s_oneSecondTimer;
        private static readonly SortedList<DateTime, Action> s_delayedOperations = new SortedList<DateTime, Action>();

        /// <summary>
        /// Gets true whether the threading model uses many threads, false otherwise.
        /// </summary>
        public static bool IsMultiThreaded
        {
            get { return Actor != null; }
        }

        /// <summary>
        /// Gets true if the calling thread is the underlying thread; false otherwise.
        /// When false, operations on the underlying actor have to be done through <see cref="Invoke"/> or <see cref="BeginInvoke"/>.
        /// </summary>
        public static bool HasAccess
        {
            get
            {
                IActor actor = Actor;
                return actor == null || actor.HasAccess;
            }
        }

        /// <summary>
        /// Gets the underlying actor of the dispatcher.
        /// When null, the dispatcher will run in single-threaded mode.
        /// </summary>
        public static IActor Actor { get; private set; }

        /// <summary>
        /// Starts the dispatcher with the given actor.
        /// </summary>
        /// <remarks>If the method has already been called previously, this new call will silently fail.</remarks>
        /// <param name="actor">The actor to run on.</param>
        /// <exception cref="ArgumentException">The specified actor is null.</exception>
        internal static void Run(IActor actor)
        {
            Enforce.ArgumentNotNull(actor, "actor");

            // Double-check pattern (1)
            if (Actor != null)
                return;

            lock (s_syncLock)
            {
                // Double-check pattern (2)
                if (Actor != null)
                    return;

                Actor = actor;
                s_oneSecondTimer = actor.GetTimer(OnOneSecondTimerTick, 1000, true);
            }
        }

        /// <summary>
        /// Invoke the provided delegate on the underlying actor and wait for completion.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        public static void Invoke(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            IActor actor = Actor;
            if (HasAccess || (actor == null))
                action();
            else
                actor.Invoke(action);
        }

        /// <summary>
        /// Invoke the provided delegate on the underlying actor and immediately returns without waiting for the completion. 
        /// Note that, when the calling thread and the bound thread are the same, we execute the action immediately without waiting.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        public static void BeginInvoke(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            IActor actor = Actor;
            if (HasAccess || (actor == null))
                action();
            else
                actor.BeginInvoke(action);
        }

        /// <summary>
        /// Schedule an action to invoke on the actor, by specifying a delay from now on (one second resolution).
        /// </summary>
        /// <remarks>
        /// Note the scheduler only has a one second resolution.
        /// <para>If the time is already elasped, the execution will occur now.</para></remarks>
        /// <param name="delay">The delay before the action is executed.</param>
        /// <param name="action">The callback to execute.</param>
        public static void Schedule(TimeSpan delay, Action action)
        {
            DateTime dateTime = DateTime.UtcNow.Add(delay);
            Schedule(dateTime, action);
        }

        /// <summary>
        /// Schedule an action to invoke on the actor, by specifying the time it will be executed  (one second resolution).
        /// </summary>
        /// <remarks>
        /// Note the scheduler only has a one second resolution.
        /// <para>If the time is already elasped, the execution will occur now.</para></remarks>
        /// <param name="action">The callback to execute.</param>
        /// <param name="time">The time at which the delay will be executed.</param>
        private static void Schedule(DateTime time, Action action)
        {
            time = time.ToUniversalTime();

            // Is it already elapsed ?
            if (time < DateTime.UtcNow)
            {
                BeginInvoke(action);
                return;
            }

            // Plan
            lock (s_syncLock)
            {
                // Add one tick until we get a unique key
                while (s_delayedOperations.ContainsKey(time))
                {
                    time = time.AddTicks(1);
                }

                // Stores it in a sorted list
                s_delayedOperations.Add(time, action);
            }
        }

        /// <summary>
        /// Invoke the provided delegate on the thread pool and immediately returns without waiting for the completion. 
        /// Note that, when the dispatcher runs in single-threaded mode (for testing purposes), the action is actually performed on the calling thread.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        public static void BackgroundInvoke(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (IsMultiThreaded)
                action.BeginInvoke(null, null);
            else
                action.Invoke();
        }

        /// <summary>
        /// Invoke the provided delegate on the thread pool and immediately returns without waiting for the completion. 
        /// Note that, when the dispatcher runs in single-threaded mode (for testing purposes), the action is actually performed on the calling thread.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <param name="callback"></param>
        /// <param name="object"></param>
        public static void BackgroundInvoke(Action action, AsyncCallback callback, object @object)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (IsMultiThreaded)
                action.BeginInvoke(callback, @object);
            else
                action.Invoke();
        }

        /// <summary>
        /// Asserts the calling thread is this actor's underlying thread or throws an exception.
        /// </summary>
        public static void AssertAccess()
        {
            IActor actor = Actor;
            if (actor != null)
                actor.AssertAccess();
        }

        /// <summary>
        /// Occurs on every second, when the timer ticks.
        /// </summary>
        private static void OnOneSecondTimerTick()
        {
            // Updates the EVEMon client every one second
            EveMonClient.UpdateOnOneSecondTick();

            // Check for scheduled operations before now
            List<Action> actionsToInvoke = new List<Action>();
            lock (s_syncLock)
            {
                // Collect all the actions scheduled before now
                actionsToInvoke.AddRange(
                    s_delayedOperations.TakeWhile(pair => pair.Key <= DateTime.UtcNow).Select(pair => pair.Value));

                foreach (Action action in actionsToInvoke)
                {
                    // Execute the entries (we're already on the proper thread)
                    action.Invoke();

                    // Remove those actions from the list
                    s_delayedOperations.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Shutdowns the dispatcher.
        /// </summary>
        internal static void Shutdown()
        {
            if (s_oneSecondTimer == null)
                return;

            s_oneSecondTimer.Stop();
            s_oneSecondTimer = null;
        }
    }
}