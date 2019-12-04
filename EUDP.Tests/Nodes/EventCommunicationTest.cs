using NUnit.Framework;
using EUDP.Chanels.Nodes.Communication;
using EUDP.Chanels.Nodes;
using Moq;

namespace EUDP.Tests
{
    [TestFixture]
    public class EventCommunicationTest{
        [Test]
        public void EventCommunication_addHandler(){
            var comm = new EventCommunication<byte[]>();
            bool messageReceived = false;
            comm.AddMessageHandler((message) => messageReceived = true);
            ((INodeSender<byte[]>)comm).Receive(new byte[0]);

            Assert.IsTrue(messageReceived);
        }

        [Test]
        public void EventCommunication_removeHandler(){
            var comm = new EventCommunication<byte[]>();
            bool messageReceived = false;
            EventCommunication<byte[]>.MessageHandler handler = (message) => { messageReceived = true;};
            comm.AddMessageHandler(handler);
            comm.RemoveMessageHandler(handler);
            ((INodeSender<byte[]>)comm).Receive(new byte[0]);

            Assert.IsFalse(messageReceived);
        }

        [Test]
        public void EventCommunication_multipleHandlers(){
            var comm = new EventCommunication<byte[]>();
            int messagesReceived = 0;
            EventCommunication<byte[]>.MessageHandler handler = (message) => { messagesReceived++; };
            comm.AddMessageHandler(handler);
            comm.AddMessageHandler(handler);
            ((INodeSender<byte[]>)comm).Receive(new byte[0]);

            Assert.AreEqual(messagesReceived, 2);
        }

        [Test]
        public void EventCommunication_removeMultipleHandlers(){
            var comm = new EventCommunication<byte[]>();
            int messagesReceived = 0;
            EventCommunication<byte[]>.MessageHandler handler = (message) => { messagesReceived++; };
            comm.AddMessageHandler(handler);
            comm.AddMessageHandler(handler);
            comm.RemoveMessageHandler(handler);
            ((INodeSender<byte[]>)comm).Receive(new byte[0]);

            Assert.AreEqual(messagesReceived, 1);
        }

        [Test]
        public void EventCommunication_sendMessageForward(){
            var data = new byte[0];
            var received = false;

            var mock = new Mock<INodeReceiver<byte[]>>();
            mock.Setup(m => m.Receive(data)).Callback(() => received = true);


            var comm = new EventCommunication<byte[]>();
            ((INodeSender<byte[]>)comm).SetReceiver(mock.Object);
            ((INodeReceiver<byte[]>)comm).Receive(data);

            Assert.IsTrue(received);
        }
    }
}
