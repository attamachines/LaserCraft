namespace netDxf.Collections
{
    using netDxf.Tables;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class DimensionStyleOverrideDictionary : IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>, ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>, IEnumerable<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>, IEnumerable
    {
        private readonly Dictionary<DimensionStyleOverrideType, DimensionStyleOverride> innerDictionary;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AddItemEventHandler AddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeAddItemEventHandler BeforeAddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeRemoveItemEventHandler BeforeRemoveItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event RemoveItemEventHandler RemoveItem;

        public DimensionStyleOverrideDictionary()
        {
            this.innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>();
        }

        public DimensionStyleOverrideDictionary(int capacity)
        {
            this.innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>(capacity);
        }

        public void Add(DimensionStyleOverride item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (this.OnBeforeAddItemEvent(item))
            {
                throw new ArgumentException($"The DimensionStyleOverride {item} cannot be added to the collection.", "item");
            }
            this.innerDictionary.Add(item.Type, item);
            this.OnAddItemEvent(item);
        }

        public void Add(DimensionStyleOverrideType type, object value)
        {
            this.Add(new DimensionStyleOverride(type, value));
        }

        public void AddRange(IEnumerable<DimensionStyleOverride> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            foreach (DimensionStyleOverride @override in collection)
            {
                this.Add(@override);
            }
        }

        public void Clear()
        {
            DimensionStyleOverrideType[] array = new DimensionStyleOverrideType[this.innerDictionary.Count];
            this.innerDictionary.Keys.CopyTo(array, 0);
            foreach (DimensionStyleOverrideType type in array)
            {
                this.Remove(type);
            }
        }

        public bool ContainsType(DimensionStyleOverrideType type) => 
            this.innerDictionary.ContainsKey(type);

        public bool ContainsValue(DimensionStyleOverride value) => 
            this.innerDictionary.ContainsValue(value);

        public IEnumerator<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>> GetEnumerator() => 
            this.innerDictionary.GetEnumerator();

        private void OnAddItemEvent(DimensionStyleOverride item)
        {
            AddItemEventHandler addItem = this.AddItem;
            if (addItem > null)
            {
                addItem(this, new DimensionStyleOverrideDictionaryEventArgs(item));
            }
        }

        private bool OnBeforeAddItemEvent(DimensionStyleOverride item)
        {
            BeforeAddItemEventHandler beforeAddItem = this.BeforeAddItem;
            if (beforeAddItem > null)
            {
                DimensionStyleOverrideDictionaryEventArgs e = new DimensionStyleOverrideDictionaryEventArgs(item);
                beforeAddItem(this, e);
                return e.Cancel;
            }
            return false;
        }

        private bool OnBeforeRemoveItemEvent(DimensionStyleOverride item)
        {
            BeforeRemoveItemEventHandler beforeRemoveItem = this.BeforeRemoveItem;
            if (beforeRemoveItem > null)
            {
                DimensionStyleOverrideDictionaryEventArgs e = new DimensionStyleOverrideDictionaryEventArgs(item);
                beforeRemoveItem(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void OnRemoveItemEvent(DimensionStyleOverride item)
        {
            RemoveItemEventHandler removeItem = this.RemoveItem;
            if (removeItem > null)
            {
                removeItem(this, new DimensionStyleOverrideDictionaryEventArgs(item));
            }
        }

        public bool Remove(DimensionStyleOverrideType type)
        {
            if (!this.innerDictionary.TryGetValue(type, out DimensionStyleOverride @override))
            {
                return false;
            }
            if (this.OnBeforeRemoveItemEvent(@override))
            {
                return false;
            }
            this.innerDictionary.Remove(type);
            this.OnRemoveItemEvent(@override);
            return true;
        }

        void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Add(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            this.Add(item.Value);
        }

        bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Contains(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item) => 
            this.innerDictionary.Contains(item);

        void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.CopyTo(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>[] array, int arrayIndex)
        {
            this.innerDictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Remove(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            if (item.Value != this.innerDictionary[item.Key])
            {
                return false;
            }
            return this.Remove(item.Key);
        }

        void IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Add(DimensionStyleOverrideType key, DimensionStyleOverride value)
        {
            this.Add(value);
        }

        bool IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.ContainsKey(DimensionStyleOverrideType tag) => 
            this.innerDictionary.ContainsKey(tag);

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public bool TryGetValue(DimensionStyleOverrideType type, out DimensionStyleOverride value) => 
            this.innerDictionary.TryGetValue(type, out value);

        public DimensionStyleOverride this[DimensionStyleOverrideType type]
        {
            get => 
                this.innerDictionary[type];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (type != value.Type)
                {
                    throw new ArgumentException($"The dictionary type: {type}, and the DimensionStyleOverride type: {value.Type}, must be the same");
                }
                DimensionStyleOverride item = this.innerDictionary[type];
                if (!this.OnBeforeRemoveItemEvent(item) && !this.OnBeforeAddItemEvent(value))
                {
                    this.innerDictionary[type] = value;
                    this.OnAddItemEvent(value);
                    this.OnRemoveItemEvent(item);
                }
            }
        }

        public ICollection<DimensionStyleOverrideType> Types =>
            this.innerDictionary.Keys;

        public ICollection<DimensionStyleOverride> Values =>
            this.innerDictionary.Values;

        public int Count =>
            this.innerDictionary.Count;

        public bool IsReadOnly =>
            false;

        ICollection<DimensionStyleOverrideType> IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Keys =>
            this.innerDictionary.Keys;

        public delegate void AddItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public delegate void BeforeAddItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public delegate void BeforeRemoveItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public delegate void RemoveItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);
    }
}

