using System;
using System.Runtime.CompilerServices;
using System.Timers;
using EUDP.Pooling;

namespace EUDP.Chanels.Nodes.Batching{
    /// <summary>
    /// Base class for all batchers. Batchers are grouping messages in block in order to reduce ammount of data sended throught betwork.
    /// Batched messages will use same UDP(and lower protocols) header and sended as one bigger message.
    /// </summary>
    public abstract class Batcher: INode<ArraySegment<byte>, ArraySegment<byte>>, ICloseAble, IDisposable{
        private INodeReceiver<ArraySegment<byte>> batchReceiver;
        private INodeSender<ArraySegment<byte>> unBatchSender;

        protected readonly int offset;
        protected readonly int chunkSize;
        protected readonly ByteBufferPool bufferPool;

        private readonly Timer batchTimer;

        protected Batcher(int offset, int chunkSize, ByteBufferPool bufferPool, int timeout){
            this.offset = offset;
            this.chunkSize = chunkSize;
            this.bufferPool = bufferPool;

            batchTimer = new Timer(timeout);
            batchTimer.Elapsed += (x,y)=>BatchWaitTimeout();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendAllBufferedMessages(){
            if (batchTimer.Enabled){
                batchTimer.Stop();
                batchTimer.Start();
            }
            SendBufferedData();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract void SendBufferedData();

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract void BatchWaitTimeout();
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract void Batch(ArraySegment<byte> data);
        protected abstract void UnBatch(ArraySegment<byte> data);

        protected void SendBatchedData(ArraySegment<byte> data) => batchReceiver.Receive(data);
        protected void SendUnBatchedData(ArraySegment<byte> data) => unBatchSender.Receive(data);

        void INodeReceiver<ArraySegment<byte>>.Receive(ArraySegment<byte> data) => Batch(data);
        void INodeSender<ArraySegment<byte>>.Receive(ArraySegment<byte> data) => UnBatch(data);

        void INodeSender<ArraySegment<byte>>.SetReceiver(INodeReceiver<ArraySegment<byte>> receiver) => batchReceiver = receiver;
        void INodeReceiver<ArraySegment<byte>>.SetSender(INodeSender<ArraySegment<byte>> sender) => unBatchSender = sender;

        protected ArraySegment<byte> AddLengthHeaderToData(ArraySegment<byte> data){
            //TODO: endians

            var dataLn = data.Count;
            data = new ArraySegment<byte>(data.Array, data.Offset - 2, data.Count + 2);
            data.Array[data.Offset] = (byte)(dataLn | 0xff00 >> 2);
            data.Array[data.Offset + 1] = (byte)(dataLn | 0xff);
            return data;
        }

        protected int GetBlockLength(ArraySegment<byte> data, int blockStart){
            //TODO: endians
            return data.Array[blockStart] << 2 | data.Array[blockStart + 1];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Close(){
            Dispose();
        }

        public void Dispose(){
            batchTimer.Dispose();
        }
    }
}
