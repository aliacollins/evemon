using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of the training queue.  Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPISkillQueue : ISynchronizableWithLocalClock
    {
        private readonly Collection<SerializableQueuedSkill> m_queue;

        public SerializableAPISkillQueue()
        {
            m_queue = new Collection<SerializableQueuedSkill>();
        }

        [XmlArray("queue")]
        [XmlArrayItem("skill")]
        public Collection<SerializableQueuedSkill> Queue
        {
            get { return m_queue; }
        }

        #region ISynchronizableWithLocalClock Members

        /// <summary>
        /// Synchronizes the stored times with local clock.
        /// </summary>
        /// <param name="drift"></param>
        void ISynchronizableWithLocalClock.SynchronizeWithLocalClock(TimeSpan drift)
        {
            foreach (ISynchronizableWithLocalClock synch in Queue)
            {
                synch.SynchronizeWithLocalClock(drift);
            }
        }

        #endregion
    }
}