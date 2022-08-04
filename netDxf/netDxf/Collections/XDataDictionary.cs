namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class XDataDictionary : IDictionary<string, XData>, ICollection<KeyValuePair<string, XData>>, IEnumerable<KeyValuePair<string, XData>>, IEnumerable
    {
        private readonly Dictionary<string, XData> innerDictionary;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AddAppRegEventHandler AddAppReg;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event RemoveAppRegEventHandler RemoveAppReg;

        public XDataDictionary()
        {
            this.innerDictionary = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase);
        }

        public XDataDictionary(IEnumerable<XData> items)
        {
            this.innerDictionary = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase);
            this.AddRange(items);
        }

        public XDataDictionary(int capacity)
        {
            this.innerDictionary = new Dictionary<string, XData>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        public void Add(XData item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (this.innerDictionary.TryGetValue(item.ApplicationRegistry.Name, out XData data))
            {
                data.XDataRecord.AddRange(item.XDataRecord);
            }
            else
            {
                this.innerDictionary.Add(item.ApplicationRegistry.Name, item);
                item.ApplicationRegistry.NameChanged += new TableObject.NameChangedEventHandler(this.ApplicationRegistry_NameChanged);
                this.OnAddAppRegEvent(item.ApplicationRegistry);
            }
        }

        public void AddRange(IEnumerable<XData> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (XData data in items)
            {
                this.Add(data);
            }
        }

        private void ApplicationRegistry_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            XData data = this.innerDictionary[e.OldValue];
            this.innerDictionary.Remove(e.OldValue);
            this.innerDictionary.Add(e.NewValue, data);
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

        public bool ContainsAppId(string appId) => 
            this.innerDictionary.ContainsKey(appId);

        public bool ContainsValue(XData value) => 
            this.innerDictionary.ContainsValue(value);

        public IEnumerator<KeyValuePair<string, XData>> GetEnumerator() => 
            this.innerDictionary.GetEnumerator();

        private void OnAddAppRegEvent(ApplicationRegistry item)
        {
            AddAppRegEventHandler addAppReg = this.AddAppReg;
            if (addAppReg > null)
            {
                addAppReg(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
            }
        }

        private void OnRemoveAppRegEvent(ApplicationRegistry item)
        {
            RemoveAppRegEventHandler removeAppReg = this.RemoveAppReg;
            if (removeAppReg > null)
            {
                removeAppReg(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
            }
        }

        public bool Remove(string appId)
        {
            if (!this.innerDictionary.ContainsKey(appId))
            {
                return false;
            }
            XData data = this.innerDictionary[appId];
            data.ApplicationRegistry.NameChanged -= new TableObject.NameChangedEventHandler(this.ApplicationRegistry_NameChanged);
            this.innerDictionary.Remove(appId);
            this.OnRemoveAppRegEvent(data.ApplicationRegistry);
            return true;
        }

        void ICollection<KeyValuePair<string, XData>>.Add(KeyValuePair<string, XData> item)
        {
            this.Add(item.Value);
        }

        bool ICollection<KeyValuePair<string, XData>>.Contains(KeyValuePair<string, XData> item) => 
            this.innerDictionary.Contains(item);

        void ICollection<KeyValuePair<string, XData>>.CopyTo(KeyValuePair<string, XData>[] array, int arrayIndex)
        {
            this.innerDictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, XData>>.Remove(KeyValuePair<string, XData> item) => 
            ((item.Value == this.innerDictionary[item.Key]) && this.Remove(item.Key));

        void IDictionary<string, XData>.Add(string key, XData value)
        {
            this.Add(value);
        }

        bool IDictionary<string, XData>.ContainsKey(string tag) => 
            this.innerDictionary.ContainsKey(tag);

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public bool TryGetValue(string appId, out XData value) => 
            this.innerDictionary.TryGetValue(appId, out value);

        public XData this[string appId]
        {
            get => 
                this.innerDictionary[appId];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!string.Equals(value.ApplicationRegistry.Name, appId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"The extended data application registry name {value.ApplicationRegistry.Name} must be equal to the specified appId {appId}.");
                }
                this.innerDictionary[appId] = value;
            }
        }

        public ICollection<string> AppIds =>
            this.innerDictionary.Keys;

        public ICollection<XData> Values =>
            this.innerDictionary.Values;

        public int Count =>
            this.innerDictionary.Count;

        public bool IsReadOnly =>
            false;

        ICollection<string> IDictionary<string, XData>.Keys =>
            this.innerDictionary.Keys;

        public delegate void AddAppRegEventHandler(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e);

        public delegate void RemoveAppRegEventHandler(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e);
    }
}

