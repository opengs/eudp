using System;
namespace EUDP.Pooling{
    public class ByteBufferPool : ObjectPool<ArraySegment<byte>>{
        public int SegmentsSize { get; }

        public ByteBufferPool(int segmentsSize, int maxPoolSize) : base(maxPoolSize) {
            SegmentsSize = segmentsSize;
        }

        protected override ArraySegment<byte> Clear(ArraySegment<byte> obj){
            return new ArraySegment<byte>(obj.Array, 0, SegmentsSize);
        }

        protected override ArraySegment<byte> CreateNew(){
            return new ArraySegment<byte>(new byte[SegmentsSize], 0, SegmentsSize);
        }
    }
}
