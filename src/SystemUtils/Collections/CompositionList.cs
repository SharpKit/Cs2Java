using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemUtils.Collections
{
    public class CompositionList<T> : Collection<T>
    {
        public CompositionList(Action<T> itemAdding, Action<T> itemRemoving)
        {
            ItemAdding = itemAdding;
            ItemRemoving = itemRemoving;
        }
        public Action<T> ItemRemoving { get; private set; }
        public Action<T> ItemAdding { get; private set; }

        protected override void ClearItems()
        {
            foreach (var x in this)
                ItemRemoving(x);
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            ItemAdding(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            ItemRemoving(this[index]);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            ItemRemoving(this[index]);
            base.SetItem(index, item);
            ItemAdding(this[index]);
        }
    }
}
