using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using EUDP.Pooling;

namespace EUDP.Chanels.Nodes.Converting{
    /// <summary>
    /// Uses <see cref="System.Runtime.Serialization.IFormatter"/> for converting
    /// data that flows inside this node
    /// </summary>
    public class FormatterConverter<R>: Converter<R, ArraySegment<byte>>, IDisposable, ICloseAble{
        readonly ByteBufferPool buffersPool;
        readonly int headerSize;

        readonly IFormatter formatter;

        readonly MemoryStream forwardStream;
        readonly MemoryStream backwardStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EUDP.Chanels.Nodes.Converting.FormatterConverter`1"/> class.
        /// </summary>
        /// <param name="headerSize">Offset of the ArraySegment. Space for future protocol headres.</param>
        /// <param name="buffersPool">Pool used to accuire and release segments.</param>
        public FormatterConverter(IFormatter formatter, int headerSize, ByteBufferPool buffersPool){
            this.formatter = formatter;
            forwardStream = new MemoryStream();
            backwardStream = new MemoryStream();

            this.buffersPool = buffersPool;
            this.headerSize = headerSize;
        }

        public void Close(){
            Dispose();
        }

        public void Dispose(){
            forwardStream.Dispose();
            backwardStream.Dispose();
        }

        protected override R ConvertBackward(ArraySegment<byte> data){
            backwardStream.Write(data.Array, data.Offset, data.Count);
            if(data.Array.Length == buffersPool.SegmentSize)
                buffersPool.Release(data);
            return (R)formatter.Deserialize(backwardStream);
        }

        protected override ArraySegment<byte> ConvertForward(R data){
            formatter.Serialize(forwardStream, data);
            ArraySegment<byte> serializedData;

            if(buffersPool.SegmentSize <= (int)forwardStream.Length + headerSize)
                serializedData = new ArraySegment<byte>(buffersPool.Accuire().Array, headerSize, (int)forwardStream.Length);
            else
                serializedData = new ArraySegment<byte>(new byte[headerSize + forwardStream.Length], headerSize, (int)forwardStream.Length);

            forwardStream.Read(serializedData.Array, headerSize, (int)forwardStream.Length);
            return serializedData;
        }
    }
}
