using NUnit.Framework;
using EUDP.Pooling;

namespace EUDP.Tests.Unit.Pooling
{
    [TestFixture]
    public class ByteBufferPoolTest
    {
        [Test]
        public void ByteBufferPooll_accuireMultiple(){
            var pool = new ByteBufferPool(1024, 2);

            Assert.AreNotSame(pool.Accuire().Array, pool.Accuire().Array);
        }

        [Test]
        public void ByteBufferPooll_accuireStandardLength(){
            var pool = new ByteBufferPool(1024, 1);

            var buf = pool.Accuire();
            Assert.AreEqual(pool.Accuire().Array.Length, 1024);
            Assert.AreEqual(buf.Count, 1024);
            Assert.AreEqual(buf.Offset, 0);

            ByteBufferPooll_accuireWithSpecificSize(10, 10, 10);
        }

        [Test]
        public void ByteBufferPooll_accuireSmallerLength(){
            ByteBufferPooll_accuireWithSpecificSize(100, 10, 100);
        }

        [Test]
        public void ByteBufferPooll_accuireBiggerLength(){
            ByteBufferPooll_accuireWithSpecificSize(10, 100, 100);
        }

        private void ByteBufferPooll_accuireWithSpecificSize(int poolSize, int size, int outputArrayLength){
            var pool = new ByteBufferPool(poolSize, 1);

            var buf = pool.Accuire(size);
            Assert.AreEqual(buf.Array.Length, outputArrayLength);
            Assert.AreEqual(buf.Count, size);
            Assert.AreEqual(buf.Offset, 0);
        }

        [Test]
        public void ByteBufferPool_releaseStandard(){
            var pool = new ByteBufferPool(1024, 1);
            var buf = pool.Accuire();
            pool.Release(buf);
            Assert.AreEqual(pool.Accuire(), buf);
        }

        [Test]
        public void ByteBufferPool_releaseBig(){
            var pool = new ByteBufferPool(1024, 1);
            var buf = pool.Accuire(2000);
            pool.Release(buf);
            Assert.AreNotEqual(pool.Accuire(), buf);
        }

        [Test]
        public void ByteBufferPool_maxSizeTest(){
            var pool = new ByteBufferPool(1024, 1);

            var buf1 = pool.Accuire();
            var buf2 = pool.Accuire();
            pool.Release(buf1);
            pool.Release(buf2);

            Assert.AreSame(pool.Accuire().Array, buf1.Array);
            Assert.AreNotSame(pool.Accuire().Array, buf2.Array);
        }

        [Test]
        public void ByteBufferPool_cleanTest(){
            var pool = new ByteBufferPool(1024, 1);

            pool.Release(new System.ArraySegment<byte>(new byte[1024], 100, 100));

            var buf = pool.Accuire();
            Assert.AreEqual(buf.Count, 1024);
            Assert.AreEqual(buf.Offset, 0);
        }
    }
}
