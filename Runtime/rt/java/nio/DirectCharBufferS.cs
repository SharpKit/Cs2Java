//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.nio
{
    internal partial class DirectCharBufferS : CharBuffer, global::sun.nio.ch.DirectBuffer
    {
        public static bool unaligned;
        public static global::sun.misc.Unsafe @unsafe;
        public DirectCharBufferS(){}
        public virtual long address(){return default(long);}
        public override CharBuffer asReadOnlyBuffer(){return default(CharBuffer);}
        public virtual global::System.Object attachment(){return default(global::System.Object);}
        public virtual global::sun.misc.Cleaner cleaner(){return default(global::sun.misc.Cleaner);}
        public override CharBuffer compact(){return default(CharBuffer);}
        public override CharBuffer duplicate(){return default(CharBuffer);}
        public override char get(int prm1){return default(char);}
        public override CharBuffer get(char[] prm1, int prm2, int prm3){return default(CharBuffer);}
        public override char get(){return default(char);}
        public override ByteOrder order(){return default(ByteOrder);}
        public override CharBuffer put(int prm1, char prm2){return default(CharBuffer);}
        public override CharBuffer put(char[] prm1, int prm2, int prm3){return default(CharBuffer);}
        public override CharBuffer put(CharBuffer prm1){return default(CharBuffer);}
        public override CharBuffer put(char prm1){return default(CharBuffer);}
        public override CharBuffer slice(){return default(CharBuffer);}
        public override CharBuffer subSequence(int prm1, int prm2){return default(CharBuffer);}
        public virtual global::java.lang.String toString(int prm1, int prm2){return default(global::java.lang.String);}
        public bool  IsDirect { get; private set;}
        public bool  IsReadOnly { get; private set;}
    }
}
