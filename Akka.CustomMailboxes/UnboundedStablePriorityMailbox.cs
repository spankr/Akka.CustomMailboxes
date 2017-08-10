using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch.MessageQueues;

namespace Akka.Dispatch
{
    /// <summary>
    /// Priority mailbox - an unbounded mailbox that allows for prioritization of its contents.
    /// Extend this class and implement the <see cref="PriorityGenerator"/> method with your own prioritization.
    /// The value returned by the <see cref="PriorityGenerator"/> method will be used to order the message in the mailbox.
    /// Lower values will be delivered first. Messages ordered by the same number will remain in delivery order.
    /// </summary>
    public abstract class UnboundedStablePriorityMailbox : MailboxType, IProducesMessageQueue<UnboundedStablePriorityMessageQueue>
    {
        /// <summary>
        /// Function responsible for generating the priority value of a message based on its type and content.
        /// </summary>
        /// <param name="message">The message to inspect.</param>
        /// <returns>An integer. The lower the value, the higher the priority.</returns>
        protected abstract int PriorityGenerator(object message);

        /// <summary>
        /// The initial capacity of the unbounded mailbox.
        /// </summary>
        public int InitialCapacity { get; }

        /// <summary>
        /// The default capacity of an unbounded priority mailbox.
        /// </summary>
        public const int DefaultCapacity = 11;

        /// <inheritdoc cref="MailboxType"/>
        public sealed override IMessageQueue Create(IActorRef owner, ActorSystem system)
        {
            return new UnboundedStablePriorityMessageQueue(PriorityGenerator, InitialCapacity);
        }

        /// <inheritdoc cref="MailboxType"/>
        protected UnboundedStablePriorityMailbox(Settings settings, Config config) : base(settings, config)
        {
            InitialCapacity = DefaultCapacity;
        }
    }
}
