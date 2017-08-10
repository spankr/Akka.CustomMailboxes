using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Akka.Dispatch.MessageQueues
{
    public class UnboundedStablePriorityMessageQueue : BlockingMessageQueue
    {
        /// <summary>
        /// The default priority comparison function.
        /// </summary>
        public static readonly Func<object, int> DefaultCompareFunction = message => 1;

        private readonly List<Envelope> _data;
        private Comparer<Envelope> _priority;

        /// <summary>
        /// Creates a new unbounded, stable priority message queue.
        /// </summary>
        /// <param name="priorityFunction">The calculator function for determining the priority of inbound messages.</param>
        /// <param name="initialCapacity">The initial capacity of the queue.</param>
        public UnboundedStablePriorityMessageQueue(Func<object, int> priorityFunction, int initialCapacity)
        {
            _data = new List<Envelope>(initialCapacity);
            _priority = Comparer<Envelope>.Create((a, b) => { return priorityFunction(a.Message).CompareTo(priorityFunction(b.Message)); });
        }

        /// <summary>
        /// Unsafe method for computing the underlying message count. 
        /// </summary>
        /// <remarks>
        /// Called from within a synchronization mechanism.
        /// </remarks>
        protected override int LockedCount
        {
            get { return _data.Count; }
        }

        /// <summary>
        /// Unsafe method for enqueuing a new message to the queue.
        /// </summary>
        /// <param name="envelope">The message to enqueue.</param>
        /// <remarks>
        /// Called from within a synchronization mechanism.
        /// </remarks>
        protected override void LockedEnqueue(Envelope envelope)
        {
            _data.Add(envelope);
        }

        /// <summary>
        /// Unsafe method for attempting to dequeue a message.
        /// </summary>
        /// <param name="envelope">The message that might be dequeued.</param>
        /// <returns><c>true</c> if a message was available to be dequeued, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// Called from within a synchronization mechanism.
        /// </remarks>
        protected override bool LockedTryDequeue(out Envelope envelope)
        {
            if (_data.Count > 0)
            {
                _data.Sort(_priority);

                envelope = _data[0];
                _data.RemoveAt(0);
                return true;
            }
            envelope = default(Envelope);
            return false;
        }
    }
}
