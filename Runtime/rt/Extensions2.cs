
using JSharp;
namespace java.lang
{
    [JType(NativeOperatorOverloads=true)]
    partial class String
    {
        public static implicit operator String(string s) { return null; }
        public static implicit operator string(String s) { return null; }
    }
    [JType(NativeOperatorOverloads = true)]
    partial class Boolean
    {
        public static implicit operator Boolean(bool s) { return null; }
        public static implicit operator bool(Boolean s) { return false; }
    }
    [JType(NativeOperatorOverloads = true)]
    partial class Integer
    {
        public static implicit operator Integer(int s) { return null; }
        public static implicit operator int(Integer s) { return 0; }
    }
    partial class Object
    {
        public static readonly Class<Q> @class;
    }
    public static class J
    {
        public static Class<T> classof<T>() { return null; }
        public static Class<Q> classof2<T>() { return null; }
    }

    public static class ObjectExtensions
    {
        public static Class<T> getClass<T>(this T obj) { return default(Class<T>); }
        [JSharp.JMethod(OmitCalls=true)]
        public static T As<T>(this object obj) { return default(T); }
        public static int hashCode(this object obj) { return default(int); }
        public static  bool equals(this object obj, Object prm1) { return default(bool); }
        //protected static virtual Object clone(this object obj) { return default(Object); }
        public static  String toString(this object obj) { return default(String); }
        public static void notify(this object obj) { }
        public static void notifyAll(this object obj) { }
        public static void wait(this object obj, long prm1) { }
        public static void wait(this object obj, long prm1, int prm2) { }
        public static void wait(this object obj) { }
        //protected virtual void finalize(this object obj) { }
    }

}
