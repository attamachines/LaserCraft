namespace netDxf.Collections
{
    using netDxf.Entities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class AttributeDefinitionDictionary : IDictionary<string, AttributeDefinition>, ICollection<KeyValuePair<string, AttributeDefinition>>, IEnumerable<KeyValuePair<string, AttributeDefinition>>, IEnumerable
    {
        private readonly Dictionary<string, AttributeDefinition> innerDictionary;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AddItemEventHandler AddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeAddItemEventHandler BeforeAddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeRemoveItemEventHandler BeforeRemoveItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event RemoveItemEventHandler RemoveItem;

        public AttributeDefinitionDictionary()
        {
            this.innerDictionary = new Dictionary<string, AttributeDefinition>(StringComparer.OrdinalIgnoreCase);
        }

        public AttributeDefinitionDictionary(int capacity)
        {
            this.innerDictionary = new Dictionary<string, AttributeDefinition>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        public void Add(AttributeDefinition item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (this.OnBeforeAddItemEvent(item))
            {
                throw new ArgumentException("The attribute definition cannot be added to the collection.", "item");
            }
            this.innerDictionary.Add(item.Tag, item);
            this.OnAddItemEvent(item);
        }

        public void AddRange(IEnumerable<AttributeDefinition> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            foreach (AttributeDefinition definition in collection)
            {
                this.Add(definition);
            }
        }

        public void Clear()
        {
            string[] array = new string[this.innerDictionary.Count];
            this.innerDictionary.Keys.CopyTo(array, 0);
            foreach (string str in array)
            {
                this.Remove(str);
            }
        }

        public bool ContainsTag(string tag) => 
            this.innerDictionary.ContainsKey(tag);

        public bool ContainsValue(AttributeDefinition value) => 
            this.innerDictionary.ContainsValue(value);

        public IEnumerator<KeyValuePair<string, AttributeDefinition>> GetEnumerator() => 
            this.innerDictionary.GetEnumerator();

        private void OnAddItemEvent(AttributeDefinition item)
        {
            AddItemEventHandler addItem = this.AddItem;
            if (addItem > null)
            {
                addItem(this, new AttributeDefinitionDictionaryEventArgs(item));
            }
        }

        private bool OnBeforeAddItemEvent(AttributeDefinition item)
        {
            BeforeAddItemEventHandler beforeAddItem = this.BeforeAddItem;
            if (beforeAddItem > null)
            {
                AttributeDefinitionDictionaryEventArgs e = new AttributeDefinitionDictionaryEventArgs(item);
                beforeAddItem(this, e);
                return e.Cancel;
            }
            return false;
        }

        private bool OnBeforeRemoveItemEvent(AttributeDefinition item)
        {
            BeforeRemoveItemEventHandler beforeRemoveItem = this.BeforeRemoveItem;
            if (beforeRemoveItem > null)
            {
                AttributeDefinitionDictionaryEventArgs e = new AttributeDefinitionDictionaryEventArgs(item);
                beforeRemoveItem(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void OnRemoveItemEvent(AttributeDefinition item)
        {
            RemoveItemEventHandler removeItem = this.RemoveItem;
            if (removeItem > null)
            {
                removeItem(this, new AttributeDefinitionDictionaryEventArgs(item));
            }
        }

        public bool Remove(string tag)
        {
            if (!this.innerDictionary.TryGetValue(tag, out AttributeDefinition definition))
            {
                return false;
            }
            if (this.OnBeforeRemoveItemEvent(definition))
            {
                return false;
            }
            this.innerDictionary.Remove(tag);
            this.OnRemoveItemEvent(definition);
            return true;
        }

        void ICollection<KeyValuePair<string, AttributeDefinition>>.Add(KeyValuePair<string, AttributeDefinition> item)
        {
            this.Add(item.Value);
        }

        bool ICollection<KeyValuePair<string, AttributeDefinition>>.Contains(KeyValuePair<string, AttributeDefinition> item) => 
            this.innerDictionary.Contains(item);

        void ICollection<KeyValuePair<string, AttributeDefinition>>.CopyTo(KeyValuePair<string, AttributeDefinition>[] array, int arrayIndex)
        {
            this.innerDictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, AttributeDefinition>>.Remove(KeyValuePair<string, AttributeDefinition> item)
        {
            if (item.Value != this.innerDictionary[item.Key])
            {
                return false;
            }
            return this.Remove(item.Key);
        }

        void IDictionary<string, AttributeDefinition>.Add(string key, AttributeDefinition value)
        {
            this.Add(value);
        }

        bool IDictionary<string, AttributeDefinition>.ContainsKey(string tag) => 
            this.innerDictionary.ContainsKey(tag);

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public bool TryGetValue(string tag, out AttributeDefinition value) => 
            this.innerDictionary.TryGetValue(tag, out value);

        public AttributeDefinition this[string tag]
        {
            get => 
                this.innerDictionary[tag];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!string.Equals(tag, value.Tag, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"The dictionary tag: {tag}, and the attribute definition tag: {value.Tag}, must be the same");
                }
                if (this.innerDictionary[tag].Value != value)
                {
                    AttributeDefinition item = this.innerDictionary[tag];
                    if (!this.OnBeforeRemoveItemEvent(item) && !this.OnBeforeAddItemEvent(value))
                    {
                        this.innerDictionary[tag] = value;
                        this.OnAddItemEvent(value);
                        this.OnRemoveItemEvent(item);
                    }
                }
            }
        }

        public ICollection<string> Tags =>
            this.innerDictionary.Keys;

        public ICollection<AttributeDefinition> Values =>
            this.innerDictionary.Values;

        public int Count =>
            this.innerDictionary.Count;

        public bool IsReadOnly =>
            false;

        ICollection<string> IDictionary<string, AttributeDefinition>.Keys =>
            this.innerDictionary.Keys;

        public delegate void AddItemEventHandler(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e);

        public delegate void BeforeAddItemEventHandler(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e);

        public delegate void BeforeRemoveItemEventHandler(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e);

        public delegate void RemoveItemEventHandler(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e);
    }
}

