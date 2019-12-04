using System;
namespace EUDP.Pooling{
    public class GenericObjectPool<T>: ObjectPool<T>{
        public delegate T ObjectFactory();
        public delegate T ObjectCleaner(T obj);

        readonly ObjectFactory factory;
        readonly ObjectCleaner cleaner;

        public GenericObjectPool(ObjectFactory factory, ObjectCleaner cleaner, int maxSize): base(maxSize){
            this.factory = factory;
            this.cleaner = cleaner;
        }

        protected override T CreateNew() => factory();
        protected override T Clear(T obj) => cleaner(obj);
    }
}
