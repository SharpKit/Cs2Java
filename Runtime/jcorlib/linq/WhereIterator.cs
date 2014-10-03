using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.lang;
using java.util;
using JSharp;

namespace system.linq
{
    class WhereIterator<T> : MyIterator<T, T>
    {
        public WhereIterator(Iterable<T> source, Func<T, bool> func)
            : base(source)
        {
            Func = func;
        }
        private Func<T, bool> Func;
        protected override MyIterator<T, T> Copy()
        {
            return new WhereIterator<T>(Source, Func);
        }

        protected override void Process(T item)
        {
            if (Func(item))
                YieldReturn(item);
        }
    }
    class SelectIterator<T, R> : MyIterator<T, R>
    {
        public SelectIterator(Iterable<T> source, Func<T, R> func)
            : base(source)
        {
            Func = func;
        }
        private Func<T, R> Func;

        protected override void Process(T item)
        {
                YieldReturn(Func(item));
        }

        protected override MyIterator<T, R> Copy()
        {
            return new SelectIterator<T, R>(Source, Func);
        }
    }


    class EnumerationIterator<T> : Iterator<T>, Iterable<T>
    {
        public EnumerationIterator(Enumeration<T> source)
        {
            Source = source;
        }

        protected virtual EnumerationIterator<T> Copy()
        {
            return new EnumerationIterator<T>(Source);
        }

        private Enumeration<T> Source;

        #region Iterator<T> Members

        public bool hasNext()
        {
            return Source.hasMoreElements();
        }

        public T next()
        {
            return Source.nextElement();
        }

        public void remove()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Iterable<T> Members

        public Iterator<T> iterator()
        {
            return this;
        }

        #endregion

        #region IEnumerable<T> Members

        [JMethod(Export=false)]
        public IEnumerator<T> GetEnumerator()
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
