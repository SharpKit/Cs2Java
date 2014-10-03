//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.nio.channels
{
    public abstract partial class Selector : global::java.lang.Object, global::java.io.Closeable
    {
        public virtual void close(){}
        public Selector(){}
        public virtual global::java.util.Set<SelectionKey> keys(){return default(global::java.util.Set<SelectionKey>);}
        public static Selector open(){return default(Selector);}
        public virtual global::java.nio.channels.spi.SelectorProvider provider(){return default(global::java.nio.channels.spi.SelectorProvider);}
        public virtual int select(){return default(int);}
        public virtual int select(long prm1){return default(int);}
        public virtual global::java.util.Set<SelectionKey> selectedKeys(){return default(global::java.util.Set<SelectionKey>);}
        public virtual int selectNow(){return default(int);}
        public virtual Selector wakeup(){return default(Selector);}
        public bool  IsOpen { get; private set;}
    }
}
