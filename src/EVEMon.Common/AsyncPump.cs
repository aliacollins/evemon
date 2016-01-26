using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EVEMon.Common
{
    public sealed class AsyncPump
    {
        public static void Run(Func<Task> func)
        {
            SynchronizationContext prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                Task task = func();
                task.ContinueWith(_ => syncCtx.Complete(), TaskScheduler.Default);

                syncCtx.RunOnCurrentThread();

                task.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        private sealed class SingleThreadSynchronizationContext : SynchronizationContext
        {
            private readonly
                BlockingCollection<KeyValuePair<SendOrPostCallback, object>>
                m_queue =
                    new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

            public override void Post(SendOrPostCallback callback, object state)
            {
                m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(callback, state));
            }

            public void RunOnCurrentThread()
            {
                KeyValuePair<SendOrPostCallback, object> workItem;
                while (m_queue.TryTake(out workItem, Timeout.Infinite))
                    workItem.Key(workItem.Value);
            }

            public void Complete()
            {
                m_queue.CompleteAdding();
            }
        }
    }
}