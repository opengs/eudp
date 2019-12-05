using System;
using EUDP.Pooling;

namespace EUDP.Chanels.Nodes.Batching{
    public class BlockBatcher : Batcher{
        ArraySegment<byte> batchBuffer;
        public BlockBatcher(int chunkSize, ByteBufferPool bufferPool, int timeout) : base(chunkSize, bufferPool){
            batchBuffer = bufferPool.Accuire(chunkSize);
        }

        protected override void Batch(ArraySegment<byte> data){
            throw new NotImplementedException();
        }
    }
}
