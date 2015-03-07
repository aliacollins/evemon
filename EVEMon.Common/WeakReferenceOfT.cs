using System;
using System.Runtime.Serialization;

namespace EVEMon.Common
{
    [Serializable]
    public sealed class WeakReference<T> : WeakReference
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize the current <see cref="T:System.WeakReference"/> object.</param>
        /// <param name="context">(Reserved) Describes the source and destination of the serialized stream specified by <paramref name="info"/>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="info"/> is null. </exception>
        /// <remarks>Implemented to satisfy rule CA2229</remarks>
        private WeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public WeakReference(T target)
            : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
        public WeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

        public new T Target
        {
            get { return base.Target as T; }
        }

        /// <summary>
        /// Try to perform an action over this reference's target.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>True if the action was performed, false if the reference was no longer available.</returns>
        public bool TryDo(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            // Removes the reference at this index if it does not exist anymore
            if (!IsAlive)
                return false;

            // Try to notify the monitor
            try
            {
                // Retrieve the reference and removes it if null
                T target = Target;
                if (target == null)
                    return false;

                // Notify the monitor.
                action(target);
                return true;
            }
                // Occurs when the target has been garbage collected since the moment we performed the check
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}