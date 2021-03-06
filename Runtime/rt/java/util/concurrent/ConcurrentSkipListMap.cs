//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.util.concurrent
{
    public partial class ConcurrentSkipListMap<K,V> : AbstractMap<K,V>, ConcurrentNavigableMap<K,V>, global::java.lang.Cloneable, global::java.io.Serializable
    {
        public virtual Map_Entry<K,V> ceilingEntry(K prm1){return default(Map_Entry<K,V>);}
        public virtual K ceilingKey(K prm1){return default(K);}
        public override void clear(){}
        public virtual ConcurrentSkipListMap<K,V> clone(){return default(ConcurrentSkipListMap<K,V>);}
        public virtual Comparator<global::System.Object> comparator(){return default(Comparator<global::System.Object>);}
        public override bool containsKey(global::System.Object prm1){return default(bool);}
        public override bool containsValue(global::System.Object prm1){return default(bool);}
        public virtual NavigableSet<K> descendingKeySet(){return default(NavigableSet<K>);}
        public virtual ConcurrentNavigableMap<K,V> descendingMap(){return default(ConcurrentNavigableMap<K,V>);}
        NavigableMap<K,V> NavigableMap<K,V>.descendingMap(){return default(NavigableMap<K,V>);}
        public override Set<Map_Entry<K,V>> entrySet(){return default(Set<Map_Entry<K,V>>);}
        public override bool equals(global::System.Object prm1){return default(bool);}
        public virtual Map_Entry<K,V> firstEntry(){return default(Map_Entry<K,V>);}
        public virtual K firstKey(){return default(K);}
        public virtual Map_Entry<K,V> floorEntry(K prm1){return default(Map_Entry<K,V>);}
        public virtual K floorKey(K prm1){return default(K);}
        public override V get(global::System.Object prm1){return default(V);}
        public virtual ConcurrentNavigableMap<K,V> headMap(K prm1){return default(ConcurrentNavigableMap<K,V>);}
        public virtual ConcurrentNavigableMap<K,V> headMap(K prm1, bool prm2){return default(ConcurrentNavigableMap<K,V>);}
        SortedMap<K, V> SortedMap<K, V>.headMap(K prm1) { return default(SortedMap<K, V>); }
        NavigableMap<K,V> NavigableMap<K,V>.headMap(K prm1, bool prm2){return default(NavigableMap<K,V>);}
        public virtual Map_Entry<K,V> higherEntry(K prm1){return default(Map_Entry<K,V>);}
        public virtual K higherKey(K prm1){return default(K);}
        public ConcurrentSkipListMap(Comparator<global::System.Object> prm1){}
        public ConcurrentSkipListMap(){}
        public ConcurrentSkipListMap(SortedMap<K,V> prm1){}
        public ConcurrentSkipListMap(Map<K,V> prm1){}
        public virtual NavigableSet<K> keySet(){return default(NavigableSet<K>);}
        public virtual Map_Entry<K,V> lastEntry(){return default(Map_Entry<K,V>);}
        public virtual K lastKey(){return default(K);}
        public virtual Map_Entry<K,V> lowerEntry(K prm1){return default(Map_Entry<K,V>);}
        public virtual K lowerKey(K prm1){return default(K);}
        public virtual NavigableSet<K> navigableKeySet(){return default(NavigableSet<K>);}
        public virtual Map_Entry<K,V> pollFirstEntry(){return default(Map_Entry<K,V>);}
        public virtual Map_Entry<K,V> pollLastEntry(){return default(Map_Entry<K,V>);}
        public override V put(K prm1, V prm2){return default(V);}
        public virtual V putIfAbsent(K prm1, V prm2){return default(V);}
        public virtual bool remove(global::System.Object prm1, global::System.Object prm2){return default(bool);}
        public override V remove(global::System.Object prm1){return default(V);}
        public virtual bool replace(K prm1, V prm2, V prm3){return default(bool);}
        public virtual V replace(K prm1, V prm2){return default(V);}
        public override int size(){return default(int);}
        public virtual ConcurrentNavigableMap<K,V> subMap(K prm1, K prm2){return default(ConcurrentNavigableMap<K,V>);}
        public virtual ConcurrentNavigableMap<K,V> subMap(K prm1, bool prm2, K prm3, bool prm4){return default(ConcurrentNavigableMap<K,V>);}
        SortedMap<K, V> SortedMap<K, V>.subMap(K prm1, K prm2) { return default(SortedMap<K, V>); }
        NavigableMap<K,V> NavigableMap<K,V>.subMap(K prm1, bool prm2, K prm3, bool prm4){return default(NavigableMap<K,V>);}
        public virtual ConcurrentNavigableMap<K,V> tailMap(K prm1){return default(ConcurrentNavigableMap<K,V>);}
        public virtual ConcurrentNavigableMap<K,V> tailMap(K prm1, bool prm2){return default(ConcurrentNavigableMap<K,V>);}
        NavigableMap<K,V> NavigableMap<K,V>.tailMap(K prm1, bool prm2){return default(NavigableMap<K,V>);}
        SortedMap<K, V> SortedMap<K, V>.tailMap(K prm1) { return default(SortedMap<K, V>); }
        public override Collection<V> values(){return default(Collection<V>);}
        public bool  IsEmpty { get; private set;}
    }
}
