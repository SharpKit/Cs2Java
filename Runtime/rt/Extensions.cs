namespace java.lang
{
    using java.lang.reflect;

    partial interface Iterable<T> : global::System.Collections.Generic.IEnumerable<T>
    {
    }
}
namespace java.lang
{
    using java.lang.reflect;

    partial class Class<T>
    {
        //public TypeVariable<object>[] TypeParameters { get { return null; } }
    }
}
namespace java.util
{
    partial class AbstractCollection<E>
    {
        public void Add(E item) { }
        #region IEnumerable<E> Members

        System.Collections.Generic.IEnumerator<E> System.Collections.Generic.IEnumerable<E>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

    partial class AbstractQueue<E>
    {

        #region IEnumerable<E> Members

        public System.Collections.Generic.IEnumerator<E> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }

    partial class ServiceLoader<S>
    {
        #region IEnumerable<S> Members

        public System.Collections.Generic.IEnumerator<S> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }


}
namespace java.lang.reflect
{
    partial class Constructor<T>
    {
        //Class<object> Member.DeclaringClass { get { throw new global::System.Exception(); } }

        //TypeVariable<object>[] GenericDeclaration.TypeParameters { get { throw new global::System.Exception(); } }
    }

    partial class Method
    {

        //TypeVariable<object>[] GenericDeclaration.TypeParameters { get { throw new global::System.Exception(); } }
    }

}




namespace java.util.concurrent
{


    partial class ConcurrentSkipListSet<E>
    {

        #region IEnumerable<E> Members

        public System.Collections.Generic.IEnumerator<E> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

    partial class CopyOnWriteArrayList<E>
    {

        #region IEnumerable<E> Members

        public System.Collections.Generic.IEnumerator<E> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }

}


