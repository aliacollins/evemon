using System;
using System.Threading;

namespace EVEMon.Common.Threading
{
    /// <summary>
    /// A gross implementation of a generic timer.
    /// </summary>
    public sealed class ActorTimer : IActorTimer
    {
        private readonly Timer m_timer;
        private bool m_isRunning;
        private int m_period;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback">The action to call at any tick</param>
        /// <param name="period">The period between two ticks, in milliseconds</param>
        /// <param name="start">When true, the timer will start immediately; false otherwise</param>
        internal ActorTimer(Action callback, int period, bool start)
        {
            m_period = period;
            m_isRunning = start;

            m_timer = new Timer(state => Dispatcher.BeginInvoke(callback), null, (start ? 0 : Timeout.Infinite), period);
        }

        /// <summary>
        /// Starts or resumes the timer.
        /// </summary>
        public void Start()
        {
            if (m_isRunning)
                return;

            m_isRunning = true;
            Update();
        }

        /// <summary>
        /// Pauses the timer, it will be resumable with <see cref="Start"/>.
        /// </summary>
        public void Stop()
        {
            if (!m_isRunning)
                return;

            m_isRunning = false;
            Update();
        }

        /// <summary>
        /// Gets or sets the timer's period, in milliseconds.
        /// </summary>
        public int Period
        {
            get { return m_period; }
            set
            {
                if (m_period == value)
                    return;

                m_period = value;
                Update();
            }
        }

        /// <summary>
        /// Updates the timer setting
        /// </summary>
        private void Update()
        {
            m_timer.Change((m_isRunning ? 0 : Timeout.Infinite), m_period);
        }

        /// <summary>
        /// Dispose the timer
        /// </summary>
        public void Dispose()
        {
            m_timer.Dispose();
        }
    }
}