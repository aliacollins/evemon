using System;
using System.Threading;

namespace EVEMon.Common.Threading
{
    /// <summary>
    /// Encapsulates an action, also providing synchronization services.
    /// </summary>
    public struct Message
    {
        private readonly Action m_action;
        private readonly EventWaitHandle m_waitHandle;
        private static readonly EventWaitHandle s_eventWaitHandler = new EventWaitHandle(false, EventResetMode.AutoReset);

        /// <summary>
        /// Constructor for a message with or without waiting mechanism.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="waitForInvocation">When true, the message creator requires a waiting mechanism</param>
        public Message(Action action, bool waitForInvocation)
        {
            m_action = action;
            m_waitHandle = waitForInvocation ? s_eventWaitHandler : null;
        }

        /// <summary>
        /// Invoke the action and, when appliable, signal the creator thread.
        /// </summary>
        public void Invoke()
        {
            m_action();
            if (m_waitHandle != null)
                m_waitHandle.Set();
        }

        /// <summary>
        /// Wait for the action completion by the owner of the messages pump.
        /// </summary>
        public void Wait()
        {
            if (m_waitHandle != null)
                m_waitHandle.WaitOne();
        }

        /// <summary>
        /// Cancels the operation, just signals the waiting thread when any.
        /// </summary>
        public void Cancel()
        {
            if (m_waitHandle != null)
                m_waitHandle.Set();
        }
    }
}