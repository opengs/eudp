using NUnit.Framework;
using EUDP.Pooling;

namespace EUDP.Tests.Unit.Pooling
{
    [TestFixture]
    public class GenericObjectPoolTest{
        [Test]
        public void GenericObjectPool_createNew(){
            int counter = 0;
            var pool = new GenericObjectPool<int>(()=> { return ++counter; }, (i)=> { return i; }, 10);

            Assert.AreEqual(pool.Accuire(), counter);
            Assert.AreEqual(pool.Accuire(), counter);
            Assert.AreEqual(pool.Accuire(), counter);
            Assert.AreEqual(pool.Accuire(), counter);
            Assert.AreEqual(pool.Accuire(), counter);
        }

        [Test]
        public void GenericObjectPool_release(){
            int counter = 0;
            var pool = new GenericObjectPool<int>(() => { return ++counter; }, (i) => { return i; }, 10);

            pool.Release(pool.Accuire());
            Assert.AreEqual(pool.Accuire(), 1);
        }

        [Test]
        public void GenericObjectPool_maxSizeTest(){
            int counter = 0;
            var pool = new GenericObjectPool<int>(() => { return ++counter; }, (i) => { return i; }, 1);

            pool.Accuire();
            pool.Accuire();
            pool.Release(1);

            Assert.AreEqual(pool.Accuire(), 1);
            Assert.AreEqual(pool.Accuire(), 3);
        }
    }
}
