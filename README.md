# Unbounded Stable Priority Mailbox

A custom implementation of an [Akka prioritized mailbox][1] that honors delivery order for equivalent items. The UnboundedPriorityMailbox that is provided in the akka.net implementation does not maintain delivery order on items that have the same priority. (As it should not.)

The [UnboundedStablePriorityMailbox class][2], along with its associated queue, uses a generic List with a .sort() on dequeue to maintain proper prioritization.

If time invariance (or external factors) can be eliminated, sort on enqueue could be implemented instead.

## Use Case
Our use cases for determining priority on dequeue is _"at 4pm, all pending orders from east coast need to be expedited."_ That means our priority function for that mailbox needs to be time aware and any messages in our mailbox with ```EastCoast = true``` move to the front of our queue.  At 6pm, that rule no longer applies because our last New York delivery truck has left and they're treated as normal orders again.

When it enters the mailbox isn't as important as when it is received by the actor (between 4 and 6pm) so doing that on dequeue made the most sense.

#### Additional Disclaimers
We borrowed heavily from akka.net's [codebase][3] for these classes and inspiration. The namespace was kept the same for ease of adoption if the akka.net team ever decides to merge them into the proper akka.net solution.

[1]:https://github.com/akkadotnet/akka.net/blob/v1.3/src/core/Akka/Dispatch/Mailbox.cs
[2]:http://doc.akka.io/docs/akka/2.5.3/scala/mailboxes.html
[3]:https://github.com/akkadotnet/akka.net