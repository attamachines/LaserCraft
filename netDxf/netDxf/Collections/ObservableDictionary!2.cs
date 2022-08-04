namespace netDxf.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        private readonly Dictionary<TKey, TValue> innerDictionary;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AddItemEventHandler<TKey, TValue> AddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeAddItemEventHandler<TKey, TValue> BeforeAddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeRemoveItemEventHandler<TKey, TValue> BeforeRemoveItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event RemoveItemEventHandler<TKey, TValue> RemoveItem;

        public ObservableDictionary()
        {
            this.innerDictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this.innerDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(int capacity)
        {
            this.innerDictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.innerDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public void Add(TKey key, TValue value)
        {
            KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
            if (!this.BeforeAddItemEvent(item))
            {
                this.innerDictionary.Add(key, value);
                this.AddItemEvent(item);
            }
        }

        private void AddItemEvent(KeyValuePair<TKey, TValue> item)
        {
            AddItemEventHandler<TKey, TValue> addItem = this.AddItem;
            if (addItem > null)
            {
                addItem((ObservableDictionary<TKey, TValue>) this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
            }
        }

        private bool BeforeAddItemEvent(KeyValuePair<TKey, TValue> item)
        {
            BeforeAddItemEventHandler<TKey, TValue> beforeAddItem = this.BeforeAddItem;
            if (beforeAddItem > null)
            {
                ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
                beforeAddItem((ObservableDictionary<TKey, TValue>) this, e);
                return e.Cancel;
            }
            return false;
        }

        private bool BeforeRemoveItemEvent(KeyValuePair<TKey, TValue> item)
        {
            BeforeRemoveItemEventHandler<TKey, TValue> beforeRemoveItem = this.BeforeRemoveItem;
            if (beforeRemoveItem > null)
            {
                ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
                beforeRemoveItem((ObservableDictionary<TKey, TValue>) this, e);
                return e.Cancel;
            }
            return false;
        }

        public void Clear()
        {
            TKey[] array = new TKey[this.innerDictionary.Count];
            this.innerDictionary.Keys.CopyTo(array, 0);
            foreach (TKey local in array)
            {
                this.Remove(local);
            }
        }

        public bool ContainsKey(TKey key) => 
            this.innerDictionary.ContainsKey(key);

        public bool ContainsValue(TValue value) => 
            this.innerDictionary.ContainsValue(value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => 
            this.innerDictionary.GetEnumerator();

        public bool Remove(TKey key)
        {
            if (!this.innerDictionary.ContainsKey(key))
            {
                return false;
            }
            KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, this.innerDictionary[key]);
            if (this.BeforeRemoveItemEvent(item))
            {
                return false;
            }
            this.innerDictionary.Remove(key);
            this.RemoveItemEvent(item);
            return true;
        }

        private void RemoveItemEvent(KeyValuePair<TKey, TValue> item)
        {
            RemoveItemEventHandler<TKey, TValue> removeItem = this.RemoveItem;
            if (removeItem > null)
            {
                removeItem((ObservableDictionary<TKey, TValue>) this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => 
            this.innerDictionary.Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.innerDictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (item.Value != this.innerDictionary[item.Key])
            {
                return false;
            }
            return this.Remove(item.Key);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public bool TryGetValue(TKey key, out TValue value) => 
            this.innerDictionary.TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            get => 
                this.innerDictionary[key];
            set
            {
                KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, this.innerDictionary[key]);
                KeyValuePair<TKey, TValue> pair2 = new KeyValuePair<TKey, TValue>(key, value);
                if (!this.BeforeRemoveItemEvent(item) && !this.BeforeAddItemEvent(pair2))
                {
                    this.innerDictionary[key] = value;
                    this.AddItemEvent(pair2);
                    this.RemoveItemEvent(item);
                }
            }
        }

        public ICollection<TKey> Keys =>
            this.innerDictionary.Keys;

        public ICollection<TValue> Values =>
            this.innerDictionary.Values;

        public int Count =>
            this.innerDictionary.Count;

        public bool IsReadOnly =>
            false;

        public delegate void AddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void BeforeAddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void BeforeRemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void RemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);
    }
}

