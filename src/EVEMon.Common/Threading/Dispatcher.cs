using System;
using System.Windows.Threading;
using ThreadDispatcher = System.Windows.Threading.Dispatcher;

namespace EVEMon.Common.Threading
{
    public static class Dispatcher
    {
        private static ThreadDispatcher s_mainThreadDispather;
        private static DispatcherTimer s_oneSecondTimer;

        /// <summary>
        /// Starts the dispatcher on the main thread.
        /// </summary>
        /// <remarks>If the method has already been called previously, this new call will silently fail.</remarks>
        internal static void Run()
        {
            if (s_mainThreadDispather != null)
                return;

            s_mainThreadDispather = ThreadDispatcher.CurrentDispatcher;

            s_oneSecondTimer = new DispatcherTimer(TimeSpan.FromSeconds(1),
                DispatcherPriority.Background,
                OneSecondTickTimer_Tick,
                s_mainThreadDispather);
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

        /// <summary>
        /// Invoke the provided delegate on the underlying actor and wait for completion.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        public static void Invoke(Action action)
        {
            if (s_mainThreadDispather.CheckAccess())
                action.Invoke();
            else
                s_mainThreadDispather.Invoke(action);
        }

        /// <summary>
        /// Schedule an action to invoke on the actor, by specifying the time it will be executed.
        /// </summary>
        /// <param name="time">The time at which the action will be executed.</param>
        /// <param name="action">The action to execute.</param>
        public static void Schedule(TimeSpan time, Action action)
        {
            DispatcherTimer timer = new DispatcherTimer { Interval = time };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                action.Invoke();
            };
            timer.Start();
        }

        /// <summary>
        /// Occurs on every second, when the timer ticks.
        /// </summary>
        private static void OneSecondTickTimer_Tick(object sender, EventArgs e)
        {
            EveMonClient.UpdateOnOneSecondTick();
        }
    }
}
