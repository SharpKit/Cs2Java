//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.io
{
    public partial class ObjectStreamField : global::java.lang.Object, global::java.lang.Comparable<global::System.Object>
    {
        public virtual int compareTo(global::System.Object prm1){return default(int);}
        public ObjectStreamField(global::java.lang.String prm1, global::java.lang.Class<global::System.Object> prm2, bool prm3){}
        public ObjectStreamField(global::java.lang.String prm1, global::java.lang.Class<global::System.Object> prm2){}
        protected virtual void setOffset(int prm1){}
        public override global::java.lang.String toString(){return default(global::java.lang.String);}
        public bool  IsPrimitive { get; private set;}
        public bool  IsUnshared { get; private set;}
        public global::java.lang.String  Name { get; private set;}
        public int  Offset { get; set;}
        public global::java.lang.Class<global::System.Object>  Type { get; private set;}
        public char  TypeCode { get; private set;}
        public global::java.lang.String  TypeString { get; private set;}
    }
}
