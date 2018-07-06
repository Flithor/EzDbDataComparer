using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonLib.Common
{
    public class ActCallBackList<T> : IList<T>
    {
        public ActCallBackList(IEnumerable<T> innerList)
        {
            this._innerList = innerList.ToList();
        }

        private readonly List<T> _innerList;
        public IEnumerator<T> GetEnumerator() => _innerList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public delegate void AddHandle(object newItem);

        public event AddHandle AddEvent;

        protected virtual void OnAddEvent(object newitem)
        {
            AddEvent?.Invoke(newitem);
        }
        public void Add(T item)
        {
            _innerList.Add(item);
            OnAddEvent(item);
        }

        public delegate void ClearHandle(CancelEventArgs cancel);

        public event ClearHandle ClearEvent;

        protected virtual void OnClearEvent(CancelEventArgs cancel)
        {
            ClearEvent?.Invoke(cancel);
        }
        public void Clear()
        {
            var cancel = new CancelEventArgs();
            OnClearEvent(cancel);
            if (cancel.Cancel) return;
            _innerList.Clear();
        }

        public bool Contains(T item) => _innerList.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _innerList.CopyTo(array, arrayIndex);


        public delegate void RemoveHandle(T removeItem);

        public event RemoveHandle RemoveEvent;

        protected virtual void OnRemoveEvent(T removeItem)
        {
            RemoveEvent?.Invoke(removeItem);
        }
        public bool Remove(T item)
        {
            OnRemoveEvent(item);
            return _innerList.Remove(item);
        }

        public int Count => _innerList.Count;
        public bool IsReadOnly { get; } = false;
        public int IndexOf(T item) => _innerList.IndexOf(item);

        public delegate void InsertHandle(T insertItem);

        public event InsertHandle InsertEvent;

        protected virtual void OnInsertEvent(T insertItem)
        {
            InsertEvent?.Invoke(insertItem);
        }
        public void Insert(int index, T item)
        {
            OnInsertEvent(item);
            _innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            OnRemoveEvent(_innerList[index]);
            _innerList.RemoveAt(index);
        }

        public delegate void RemoveAllHandle(List<T> removedItems);

        public event RemoveAllHandle RemoveAllEvent;

        protected virtual void OnRemoveAllEvent(List<T> removedItems)
        {
            RemoveAllEvent?.Invoke(removedItems);
        }
        public void RemoveAll(Predicate<T> match)
        {
            OnRemoveAllEvent(_innerList.Where(i => match(i)).ToList());
            _innerList.RemoveAll(match);
        }

        public delegate void ValueChangedHandle(T oldValue, T newValue);

        public event ValueChangedHandle ValueChangedEvent;

        protected virtual void OnValueChangedEvent(T oldValue, T newValue)
        {
            ValueChangedEvent?.Invoke(oldValue, newValue);
        }
        public T this[int index]
        {
            get => _innerList[index];
            set
            {
                var oldValue = _innerList[index];
                _innerList[index] = value;
                OnValueChangedEvent(oldValue, value);
            }
        }

    }
}
