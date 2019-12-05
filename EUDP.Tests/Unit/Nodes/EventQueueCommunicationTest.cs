using NUnit.Framework;
using EUDP.Chanels.Nodes.Communication;
using EUDP.Chanels.Nodes;

namespace EUDP.Tests.Unit.Nodes
{
    [TestFixture]
    public class EventQueueCommunicationTest
    {
        [Test]
        public void EventQueueCommunication_noMessages(){
            var com = new EventQueueCommunication<int>();

            int messagesReceived = 0;
            com.AddMessageHandler((message) => messagesReceived++);
            com.Operate();

            Assert.AreEqual(messagesReceived, 0);
        }

        [Test]
        public void EventQueueCommunication_queueMessages(){
            var com = new EventQueueCommunication<int>();
            ((INodeSender<int>)com).Receive(1);
            ((INodeSender<int>)com).Receive(2);
            ((INodeSender<int>)com).Receive(3);

            int messagesReceived = 0;
            com.AddMessageHandler((message) => messagesReceived++);
            com.Operate();

            Assert.AreEqual(messagesReceived, 3);
        }

        [Test]
        public void EventQueueCommunication_checkReceiveOrder(){
            var com = new EventQueueCommunication<int>();
            ((INodeSender<int>)com).Receive(0);
            ((INodeSender<int>)com).Receive(1);
            ((INodeSender<int>)com).Receive(2);
            ((INodeSender<int>)com).Receive(3);

            int messagesReceived = 0;
            com.AddMessageHandler((message) => {
                Assert.AreEqual(messagesReceived, message);
                messagesReceived++; 
            });
            com.Operate();
        }
    }
}
