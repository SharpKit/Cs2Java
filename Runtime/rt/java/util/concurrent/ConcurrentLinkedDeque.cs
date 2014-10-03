//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.util.concurrent
{
    public partial class ConcurrentLinkedDeque<E> : AbstractCollection<E>, Deque<E>, global::java.io.Serializable
    {
        public override bool add(E prm1){return default(bool);}
        public override bool addAll(Collection<E> prm1){return default(bool);}
        public virtual void addFirst(E prm1){}
        public virtual void addLast(E prm1){}
        public override void clear(){}
        public override bool contains(global::System.Object prm1){return default(bool);}
        public virtual Iterator<E> descendingIterator(){return default(Iterator<E>);}
        public virtual E element(){return default(E);}
        public override Iterator<E> iterator(){return default(Iterator<E>);}
        public ConcurrentLinkedDeque(){}
        public ConcurrentLinkedDeque(Collection<E> prm1){}
        public virtual bool offer(E prm1){return default(bool);}
        public virtual bool offerFirst(E prm1){return default(bool);}
        public virtual bool offerLast(E prm1){return default(bool);}
        public virtual E peek(){return default(E);}
        public virtual E peekFirst(){return default(E);}
        public virtual E peekLast(){return default(E);}
        public virtual E poll(){return default(E);}
        public virtual E pollFirst(){return default(E);}
        public virtual E pollLast(){return default(E);}
        public virtual E pop(){return default(E);}
        public virtual void push(E prm1){}
        public override bool remove(global::System.Object prm1){return default(bool);}
        public virtual E remove(){return default(E);}
        public virtual E removeFirst(){return default(E);}
        public virtual bool removeFirstOccurrence(global::System.Object prm1){return default(bool);}
        public virtual E removeLast(){return default(E);}
        public virtual bool removeLastOccurrence(global::System.Object prm1){return default(bool);}
        public override int size(){return default(int);}
        public override T[] toArray<T>(T[] prm1){return default(T[]);}
        public override global::System.Object[] toArray(){return default(global::System.Object[]);}
        public E  First { get; private set;}
        public bool  IsEmpty { get; private set;}
        public E  Last { get; private set;}
    }
}
