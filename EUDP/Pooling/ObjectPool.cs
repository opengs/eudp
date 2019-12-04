using System;
using System.Collections.Concurrent;

namespace EUDP.Pooling{
    public abstract class ObjectPool<T>{
        readonly ConcurrentBag<T> pool;
        readonly int maxSize;

        public ObjectPool(int maxSize){
            pool = new ConcurrentBag<T>();
            this.maxSize = maxSize;
        }
    


        public T Accuire() {
            T obj;
            if (pool.TryTake(out obj)) return obj;
            return CreateNew();
        }

        public void Release(T obj){
            if(pool.Count < maxSize)
                pool.Add(Clear(obj));
        }

        protected abstract T CreateNew();
        protected abstract T Clear(T obj);
    }
}
