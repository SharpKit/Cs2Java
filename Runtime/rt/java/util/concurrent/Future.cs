//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.util.concurrent
{
    public partial interface Future<V>
    {
        bool cancel(bool prm1);
        V get(long prm1, TimeUnit prm2);
        V get();
        bool  IsCancelled { get;}
        bool  IsDone { get;}
    }
}