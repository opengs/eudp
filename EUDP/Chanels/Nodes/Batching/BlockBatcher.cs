using System;
using System.Runtime.CompilerServices;
using EUDP.Pooling;

namespace EUDP.Chanels.Nodes.Batching{
    /// <summary>
    /// Groups incomming messages in blocks(chunks) of specified size. If it is possible, <see cref="BlockBatcher"/> will forward block immediately.
    /// Uncompleted block (with smaller size then chunkSize) will be forwarded after timeout.
    /// Messages order persists.
    /// </summary>
    public class BlockBatcher : Batcher{
        ArraySegment<byte> batchBuffer;
        int bytesInBuffer;

        public BlockBatcher(int offset, int chunkSize, ByteBufferPool bufferPool, int timeout) : base(offset, chunkSize, bufferPool, timeout){
            ResetBuffer();
        }

        private void ResetBuffer(){
            batchBuffer = bufferPool.Accuire(chunkSize + offset);
            batchBuffer = new ArraySegment<byte>(batchBuffer.Array, offset, chunkSize);
            bytesInBuffer = 0;
        }

        protected override void Batch(ArraySegment<byte> data){
            //If incomming data is too big for current state of the buffer - send buffered data
            if (bytesInBuffer + data.Count + 2 > chunkSize) {
                SendAllBufferedMessages();
            }

            //If buffer is too big for this message, just send this message forward 
            if (data.Count + 2 > chunkSize){
                data = AddLengthHeaderToData(data);
                SendBatchedData(data);
                return;
            }

            //Else add length header to this mesage and add it to the buffer
            data = AddLengthHeaderToData(data);
            Array.Copy(data.Array, data.Offset, batchBuffer.Array, bytesInBuffer + batchBuffer.Offset, data.Count);
            bytesInBuffer += data.Count;
            bufferPool.Release(data);
        }


        protected override void BatchWaitTimeout(){
            SendBufferedData();
        }

        protected override void UnBatch(ArraySegment<byte> data){
            for (int i = data.Offset; i < data.Offset + data.Count; i++){
                //TODO: errors checking / protocol validation
                int blockSize = GetBlockLength(data, i);
                var block = bufferPool.Accuire(blockSize);
                Array.Copy(data.Array, i + 2, block.Array, 0, blockSize);
                i += 2 + blockSize;
                SendUnBatchedData(block);
            }
            bufferPool.Release(data);
        }

        protected override void SendBufferedData(){
            if (bytesInBuffer != 0){
                var sendBuf = new ArraySegment<byte>(batchBuffer.Array, batchBuffer.Offset, bytesInBuffer);
                SendBatchedData(sendBuf);
                ResetBuffer();
            }
        }
    }
}
