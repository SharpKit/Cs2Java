//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.io
{
    public partial class ObjectOutputStream : OutputStream, ObjectOutput, ObjectStreamConstants
    {
        public abstract partial class PutField : global::java.lang.Object
        {
            public PutField(){}
            public virtual void put(global::java.lang.String prm1, float prm2){}
            public virtual void put(global::java.lang.String prm1, long prm2){}
            public virtual void put(global::java.lang.String prm1, int prm2){}
            public virtual void put(global::java.lang.String prm1, double prm2){}
            public virtual void put(global::java.lang.String prm1, global::System.Object prm2){}
            public virtual void put(global::java.lang.String prm1, bool prm2){}
            public virtual void put(global::java.lang.String prm1, byte prm2){}
            public virtual void put(global::java.lang.String prm1, char prm2){}
            public virtual void put(global::java.lang.String prm1, short prm2){}
            public virtual void write(global::java.io.ObjectOutput prm1){}
        }
        protected virtual void annotateClass(global::java.lang.Class<global::System.Object> prm1){}
        protected virtual void annotateProxyClass(global::java.lang.Class<global::System.Object> prm1){}
        public override void close(){}
        public virtual void defaultWriteObject(){}
        protected virtual void drain(){}
        protected virtual bool enableReplaceObject(bool prm1){return default(bool);}
        public override void flush(){}
        public ObjectOutputStream(OutputStream prm1){}
        public ObjectOutputStream(){}
        public virtual ObjectOutputStream.PutField putFields(){return default(ObjectOutputStream.PutField);}
        protected virtual global::System.Object replaceObject(global::System.Object prm1){return default(global::System.Object);}
        public virtual void reset(){}
        public virtual void useProtocolVersion(int prm1){}
        public override void write(int prm1){}
        public override void write(byte[] prm1){}
        public override void write(byte[] prm1, int prm2, int prm3){}
        public virtual void writeBoolean(bool prm1){}
        public virtual void writeByte(int prm1){}
        public virtual void writeBytes(global::java.lang.String prm1){}
        public virtual void writeChar(int prm1){}
        public virtual void writeChars(global::java.lang.String prm1){}
        protected virtual void writeClassDescriptor(ObjectStreamClass prm1){}
        public virtual void writeDouble(double prm1){}
        public virtual void writeFields(){}
        public virtual void writeFloat(float prm1){}
        public virtual void writeInt(int prm1){}
        public virtual void writeLong(long prm1){}
        public void writeObject(global::System.Object prm1){}
        protected virtual void writeObjectOverride(global::System.Object prm1){}
        public virtual void writeShort(int prm1){}
        protected virtual void writeStreamHeader(){}
        public virtual void writeUnshared(global::System.Object prm1){}
        public virtual void writeUTF(global::java.lang.String prm1){}
    }
}
