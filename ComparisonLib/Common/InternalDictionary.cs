using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonLib.Common
{
    public class InternalDictionary<TKey, TValue>
    {
        public InternalDictionary()
        {
            Dic = new Dictionary<TKey, TValue>();
        }

        internal InternalDictionary(IDictionary<TKey, TValue> dic)
        {
            Dic = new Dictionary<TKey, TValue>(dic);
        }
        internal Dictionary<TKey, TValue> Dic { get; }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dic.GetEnumerator();

        public Dictionary<TKey, TValue>.KeyCollection Keys => Dic.Keys;

        public Dictionary<TKey, TValue>.ValueCollection Values => Dic.Values;

        public bool Contains(KeyValuePair<TKey, TValue> item) => Dic.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            (Dic as ICollection).CopyTo(array, arrayIndex);

        public int Count => Dic.Count;
        public bool IsReadOnly => true;
        public bool ContainsKey(TKey key) => Dic.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => Dic.TryGetValue(key, out value);

        public TValue this[TKey key] => Dic[key];

        /// <summary>Get a copy from current InternalDictionary inner content</summary>
        public static implicit operator Dictionary<TKey, TValue>(InternalDictionary<TKey, TValue> internalDic)
        {
            return new Dictionary<TKey, TValue>(internalDic.Dic);
        }
        /// <summary>Based a dictionary to create a new InternalDictionary, copy current dictionary</summary>
        public static implicit operator InternalDictionary<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            return new InternalDictionary<TKey, TValue>(dic);
        }
    }
}
