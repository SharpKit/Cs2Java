//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.util.concurrent
{
    public partial class Semaphore : global::java.lang.Object, global::java.io.Serializable
    {
        public virtual void acquire(){}
        public virtual void acquire(int prm1){}
        public virtual void acquireUninterruptibly(){}
        public virtual void acquireUninterruptibly(int prm1){}
        public virtual int availablePermits(){return default(int);}
        public virtual int drainPermits(){return default(int);}
        public bool hasQueuedThreads(){return default(bool);}
        public Semaphore(int prm1){}
        public Semaphore(int prm1, bool prm2){}
        protected virtual void reducePermits(int prm1){}
        public virtual void release(int prm1){}
        public virtual void release(){}
        public override global::java.lang.String toString(){return default(global::java.lang.String);}
        public virtual bool tryAcquire(int prm1, long prm2, TimeUnit prm3){return default(bool);}
        public virtual bool tryAcquire(int prm1){return default(bool);}
        public virtual bool tryAcquire(long prm1, TimeUnit prm2){return default(bool);}
        public virtual bool tryAcquire(){return default(bool);}
        public bool  IsFair { get; private set;}
        public Collection<global::java.lang.Thread>  QueuedThreads { get; private set;}
        public int  QueueLength { get; private set;}
    }
}