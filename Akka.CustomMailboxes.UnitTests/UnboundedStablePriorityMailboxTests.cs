using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.TestKit.Xunit2;
using Xunit;

namespace CustomMailboxes.UnitTests
{
    public class UnboundedStablePriorityMailboxTests : TestKit
    {
        public UnboundedStablePriorityMailboxTests() : base(@"priority-mailbox.mailbox-type=""CustomMailboxes.UnitTests.TestMailbox, Akka.CustomMailboxes.UnitTests""") { }

        [Fact(DisplayName = "Testing Priority Mailbox delivers important message first")]
        public void SendPriorityMessage_PriorityIsHandledFirst()
        {
            var actor = Sys.ActorOf(MyPrioritizedActor.Props(), "prioritizedActor");

            //Do 5 messages
            actor.Tell(new MyPrioritizedActor.NormalMessage("1"));
            actor.Tell(new MyPrioritizedActor.ImportantMessage("xx"));

            ExpectMsg<MyPrioritizedActor.ImportantMessage>();
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "1");
        }

        [Fact(DisplayName = "Testing Priority Mailbox maintains delivery order")]
        public void SendLotsOfMessages_SendPriorityMessage_MaintainDeliveryOrder()
        {
            var actor = Sys.ActorOf(MyPrioritizedActor.Props(), "prioritizedActor");

            // Do 5 messages
            actor.Tell(new MyPrioritizedActor.NormalMessage("1"));
            actor.Tell(new MyPrioritizedActor.NormalMessage("2"));
            actor.Tell(new MyPrioritizedActor.NormalMessage("3"));
            actor.Tell(new MyPrioritizedActor.ImportantMessage("xx"));
            actor.Tell(new MyPrioritizedActor.NormalMessage("4"));
            actor.Tell(new MyPrioritizedActor.NormalMessage("5"));

            ExpectMsg<MyPrioritizedActor.ImportantMessage>();
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "1");
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "2");
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "3");
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "4");
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "5");
        }
        [Fact(DisplayName = "Testing Priority Mailbox maintains delivery order on important messages")]
        public void SendMultiplePriorityMessage_PriorityIsHandledFirst()
        {
            var actor = Sys.ActorOf(MyPrioritizedActor.Props(), "prioritizedActor");

            //Do 5 messages
            actor.Tell(new MyPrioritizedActor.NormalMessage("1"));
            actor.Tell(new MyPrioritizedActor.ImportantMessage("xx"));
            actor.Tell(new MyPrioritizedActor.NormalMessage("2"));
            actor.Tell(new MyPrioritizedActor.ImportantMessage("yy"));

            ExpectMsg<MyPrioritizedActor.ImportantMessage>(m => m.Message == "xx");
            ExpectMsg<MyPrioritizedActor.ImportantMessage>(m => m.Message == "yy");
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "1");
            ExpectMsg<MyPrioritizedActor.NormalMessage>(m => m.Message == "2");
        }

    }

    public class TestMailbox : UnboundedStablePriorityMailbox
    {
        public TestMailbox(Settings settings, Config config) : base(settings, config)
        {
        }

        protected override int PriorityGenerator(object message)
        {
            return message is MyPrioritizedActor.ImportantMessage ? 0 : 99;
        }
    }

    public class MyPrioritizedActor : ReceiveActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new MyPrioritizedActor()).WithMailbox("priority-mailbox");
        }

        public MyPrioritizedActor()
        {
            ReceiveAny(m =>
            {
                Thread.Sleep(10); // simulate processing time
                Sender.Tell(m);
            });
        }

        #region Messages
        public class NormalMessage
        {
            public string Message { get; private set; }

            public NormalMessage(string msg)
            {
                Message = msg;
            }
        }

        public class ImportantMessage
        {
            public string Message { get; private set; }

            public ImportantMessage(string msg)
            {
                Message = msg;
            }
        }
        #endregion

    }

}
