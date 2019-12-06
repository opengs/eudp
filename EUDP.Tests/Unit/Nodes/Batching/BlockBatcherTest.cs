using NUnit.Framework;
using EUDP.Pooling;
using EUDP.Chanels.Nodes.Batching;
using EUDP.Chanels.Nodes;
using System;
using Moq;

namespace EUDP.Tests.Unit.Pooling {
    [TestFixture]
    public class BlockBatcherTest {
        BlockBatcher batcher;
        Action<ArraySegment<byte>> receiveUnBatched;
        Action<ArraySegment<byte>> receiveBatched;

        INodeSender<ArraySegment<byte>> unBatchedSender;
        INodeReceiver<ArraySegment<byte>> batchedSender;

        Action<ArraySegment<byte>> onSenderReceives;
        Action<ArraySegment<byte>> onReceiverReceives;

        [SetUp]
        public void SetUp() {
            batcher = new BlockBatcher(128, 256, new ByteBufferPool(512, 10), 100);

            receiveUnBatched = ((INodeReceiver<ArraySegment<byte>>)batcher).Receive;
            receiveBatched = ((INodeSender<ArraySegment<byte>>)batcher).Receive;

            var unBatchedSenderMock = new Mock<INodeSender<ArraySegment<byte>>>();
            var batchedSenderMock = new Mock<INodeReceiver<ArraySegment<byte>>>();

            onSenderReceives = (x) => { };
            onReceiverReceives = (x) => { };

            unBatchedSenderMock.Setup(m => m.Receive(It.IsAny<ArraySegment<byte>>())).Callback<ArraySegment<byte>>((x) => onSenderReceives(x));
            batchedSenderMock.Setup(m => m.Receive(It.IsAny<ArraySegment<byte>>())).Callback<ArraySegment<byte>>((x)=>onReceiverReceives(x));

            unBatchedSender = unBatchedSenderMock.Object;
            batchedSender = batchedSenderMock.Object;
        }

        [Test]
        public void BlockBatcher_batchExact() {
            int batchesReceived = 0;
            ArraySegment<byte> receivedBatch = new ArraySegment<byte>(new byte[0]);
            onReceiverReceives += (obj) => { batchesReceived++; receivedBatch = obj; };
            receiveUnBatched(new ArraySegment<byte>(new byte[32]));
            receiveUnBatched(new ArraySegment<byte>(new byte[128]));
            receiveUnBatched(new ArraySegment<byte>(new byte[64]));
            receiveUnBatched(new ArraySegment<byte>(new byte[24]));

            Assert.AreEqual(1, batchesReceived);
            Assert.AreEqual(256, receivedBatch.Count);
        }

        [Test]
        public void BlockBatcher_batchOverflow() {
            int batchesReceived = 0;
            ArraySegment<byte> receivedBatch = new ArraySegment<byte>(new byte[0]);
            onReceiverReceives += (obj) => { batchesReceived++; receivedBatch = obj; };
            receiveUnBatched(new ArraySegment<byte>(new byte[32]));
            receiveUnBatched(new ArraySegment<byte>(new byte[128]));
            receiveUnBatched(new ArraySegment<byte>(new byte[128]));

            Assert.AreEqual(1, batchesReceived);
            Assert.AreEqual(164, receivedBatch.Count);
        }

        [Test]
        public void BlockBatcher_batchTimeout() {
            int batchesReceived = 0;
            ArraySegment<byte> receivedBatch = new ArraySegment<byte>(new byte[0]);
            onReceiverReceives += (obj) => { batchesReceived++; receivedBatch = obj; };
            receiveUnBatched(new ArraySegment<byte>(new byte[32]));

            System.Threading.Thread.Sleep(200);

            Assert.AreEqual(1, batchesReceived);
            Assert.AreEqual(34, receivedBatch.Count);
        }
    }
}
