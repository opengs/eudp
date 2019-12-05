using System;
using EUDP.Pooling;

namespace EUDP.Chanels.Nodes.Batching{
    public abstract class Batcher: INode<ArraySegment<byte>, ArraySegment<byte>>{
        private INodeReceiver<ArraySegment<byte>> batchReceiver;
        private INodeSender<ArraySegment<byte>> unBatchSender;

        protected readonly int chunkSize;
        protected readonly ByteBufferPool bufferPool;


        protected Batcher(int chunkSize, ByteBufferPool bufferPool){
            this.chunkSize = chunkSize;
            this.bufferPool = bufferPool;
        }

        protected abstract void Batch(ArraySegment<byte> data);

        protected virtual void UnBatch(ArraySegment<byte> data){
            for(int i = data.Offset; i < data.Offset + data.Count; i++){
                //TODO: endians
                //TODO: errors checking / protocol validation
                var blockSize = BitConverter.ToInt32(data.Array, i);
                var block = bufferPool.Accuire(blockSize);
                Array.Copy(data.Array, i + 4, block.Array, 0, blockSize);
                i += 4 + blockSize;
                SendUnBatchedData(block);
            }
            bufferPool.Release(data);
        }

        protected void SendBatchedData(ArraySegment<byte> data) => batchReceiver.Receive(data);
        protected void SendUnBatchedData(ArraySegment<byte> data) => unBatchSender.Receive(data);

        void INodeReceiver<ArraySegment<byte>>.Receive(ArraySegment<byte> data) => Batch(data);
        void INodeSender<ArraySegment<byte>>.Receive(ArraySegment<byte> data) => UnBatch(data);

        void INodeSender<ArraySegment<byte>>.SetReceiver(INodeReceiver<ArraySegment<byte>> receiver) => batchReceiver = receiver;
        void INodeReceiver<ArraySegment<byte>>.SetSender(INodeSender<ArraySegment<byte>> sender) => unBatchSender = sender;
    }
}
