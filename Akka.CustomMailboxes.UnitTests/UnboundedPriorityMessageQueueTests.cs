using Akka.Actor;
using Akka.Dispatch.MessageQueues;
using Xunit;

namespace Akka.CustomMailboxes.UnitTests
{
    public class UnboundedPriorityMessageQueueTests
    {

        [Fact(DisplayName = "Queue maintains insert order on equal priority items")]
        public void MaintainsInsertOrder()
        {
            var q = new UnboundedStablePriorityMessageQueue(UnboundedStablePriorityMessageQueue.DefaultCompareFunction, 5);
            var e1 = new Envelope("A", ActorRefs.Nobody);
            var e2 = new Envelope("B", ActorRefs.Nobody);
            var e3 = new Envelope("C", ActorRefs.Nobody);
            var e4 = new Envelope("D", ActorRefs.Nobody);
            Envelope recv;

            q.Enqueue(ActorRefs.Nobody, e1);
            q.Enqueue(ActorRefs.Nobody, e2);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e1, recv);
            q.Enqueue(ActorRefs.Nobody, e3);
            q.Enqueue(ActorRefs.Nobody, e4);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e2, recv);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e3, recv);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e4, recv);

            Assert.False(q.TryDequeue(out recv));

            // try reverse order
            q.Enqueue(ActorRefs.Nobody, e4);
            q.Enqueue(ActorRefs.Nobody, e3);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e4, recv);
            q.Enqueue(ActorRefs.Nobody, e2);
            q.Enqueue(ActorRefs.Nobody, e1);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e3, recv);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e2, recv);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e1, recv);

            Assert.False(q.TryDequeue(out recv));
        }

        [Fact(DisplayName = "Queue observes priority order items")]
        public void MaintainsAlphabeticalOrder()
        {
            // priority is alphabetical order
            var q = new UnboundedStablePriorityMessageQueue((o) => { return char.Parse(o.ToString()) - 'A'; }, 5);
            var e1 = new Envelope("A", ActorRefs.Nobody);
            var e2 = new Envelope("B", ActorRefs.Nobody);
            var e3 = new Envelope("C", ActorRefs.Nobody);
            var e4 = new Envelope("D", ActorRefs.Nobody);
            Envelope recv;

            q.Enqueue(ActorRefs.Nobody, e2);
            q.Enqueue(ActorRefs.Nobody, e4);
            q.Enqueue(ActorRefs.Nobody, e1);
            q.Enqueue(ActorRefs.Nobody, e3);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e1, recv);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e2, recv);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e3, recv);
            Assert.True(q.TryDequeue(out recv));
            Assert.Equal(e4, recv);

            Assert.False(q.TryDequeue(out recv));
        }

    }
}
