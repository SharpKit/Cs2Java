//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.util.concurrent
{
    public abstract partial class AbstractExecutorService : global::java.lang.Object, ExecutorService
    {
        public virtual bool awaitTermination(long prm1, TimeUnit prm2){return default(bool);}
        public virtual void execute(global::java.lang.Runnable prm1){}
        public virtual List<Future<T>> invokeAll<T>(Collection<Callable<T>> prm1, long prm2, TimeUnit prm3){return default(List<Future<T>>);}
        public virtual List<Future<T>> invokeAll<T>(Collection<Callable<T>> prm1){return default(List<Future<T>>);}
        public virtual T invokeAny<T>(Collection<Callable<T>> prm1, long prm2, TimeUnit prm3){return default(T);}
        public virtual T invokeAny<T>(Collection<Callable<T>> prm1){return default(T);}
        public AbstractExecutorService(){}
        protected virtual RunnableFuture<T> newTaskFor<T>(global::java.lang.Runnable prm1, T prm2){return default(RunnableFuture<T>);}
        protected virtual RunnableFuture<T> newTaskFor<T>(Callable<T> prm1){return default(RunnableFuture<T>);}
        public virtual void shutdown(){}
        public virtual List<global::java.lang.Runnable> shutdownNow(){return default(List<global::java.lang.Runnable>);}
        public virtual Future<T> submit<T>(global::java.lang.Runnable prm1, T prm2){return default(Future<T>);}
        public virtual Future<global::System.Object> submit(global::java.lang.Runnable prm1){return default(Future<global::System.Object>);}
        public virtual Future<T> submit<T>(Callable<T> prm1){return default(Future<T>);}
        public bool  IsShutdown { get; private set;}
        public bool  IsTerminated { get; private set;}
    }
}
