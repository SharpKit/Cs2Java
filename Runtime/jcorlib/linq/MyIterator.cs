using java.lang;
using java.util;
using JSharp;

namespace system.linq
{
    abstract class MyIterator<T, R> : Iterable<R>, Iterator<R>
    {
        protected Iterable<T> Source;
        private Iterator<T> Iterator;
        public MyIterator(Iterable<T> source)
        {
            Source = source;
        }

        protected abstract MyIterator<T, R> Copy();


        #region Iterator<T> Members

        public bool hasNext()
        {
            if (IsYieldReturn)
                return true;
            if (IsYieldBreak)
                return false;
            TryGetNext();
            if (IsYieldReturn)
                return true;
            if (IsYieldBreak)
                return false;
            throw new RuntimeException();

        }

        void TryGetNext()
        {
            if (Iterator == null)
                Iterator = Source.iterator();
            while (true)
            {
                var hasNext = Iterator.hasNext();
                if (!hasNext)
                {
                    OnNoMoreNext();
                    if (IsYieldBreak)
                        break;
                    else
                        throw new RuntimeException("MyIteratorException");
                }
                var item = Iterator.next();
                Process(item);
                if (IsYieldReturn)
                    break;
                if (IsYieldBreak)
                    break;
            }
        }

        protected virtual void OnNoMoreNext()
        {
            YieldBreak();
        }

        R Current;
        public R next()
        {
            if (IsYieldReturn)
            {
                IsYieldReturn = false;
                return Current;
            }
            if (IsYieldBreak)
                throw new RuntimeException();
            TryGetNext();
            if (IsYieldBreak)
                throw new RuntimeException();
            if (IsYieldReturn)
            {
                IsYieldReturn = false;
                return Current;
            }
            throw new RuntimeException();
        }

        protected virtual void Process(T item)
        {
            throw new RuntimeException();
        }
        bool IsYieldReturn;
        bool IsYieldBreak;
        protected void YieldReturn(R item)
        {
            IsYieldReturn = true;
            Current = item;
        }
        protected void YieldBreak()
        {
            IsYieldReturn = false;
            IsYieldBreak = true;
        }
        public void remove()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Iterable<R> Members

        public Iterator<R> iterator()
        {
            return this;
        }

        #endregion

        #region IEnumerable<R> Members

        [JMethod(Export = false)]
        public System.Collections.Generic.IEnumerator<R> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        [JMethod(Export = false)]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
