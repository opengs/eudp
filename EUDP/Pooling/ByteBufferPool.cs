using System;
namespace EUDP.Pooling{
    public class ByteBufferPool : ObjectPool<ArraySegment<byte>>{
        public int SegmentSize { get; }

        public ByteBufferPool(int segmentSize, int maxPoolSize) : base(maxPoolSize) {
            SegmentSize = segmentSize;
        }

        public override void Release(ArraySegment<byte> obj){
            if (obj.Array.Length == SegmentSize)
                base.Release(obj);
        }

        public ArraySegment<byte> Accuire(int size){
            if (size <= SegmentSize) return new ArraySegment<byte>(Accuire().Array, 0, size);
            return new ArraySegment<byte>(new byte[size], 0, size);
        }

        protected override ArraySegment<byte> Clear(ArraySegment<byte> obj){
            return new ArraySegment<byte>(obj.Array, 0, SegmentSize);
        }

        protected override ArraySegment<byte> CreateNew(){
            return new ArraySegment<byte>(new byte[SegmentSize], 0, SegmentSize);
        }
    }
}
