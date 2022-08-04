namespace netDxf.Collections
{
    using netDxf;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public abstract class TableObjects<T> : DxfObject, IEnumerable<T>, IEnumerable where T: TableObject
    {
        private int maxCapacity;
        protected readonly Dictionary<string, T> list;
        protected readonly Dictionary<string, List<DxfObject>> references;

        protected TableObjects(DxfDocument document, Dictionary<string, T> list, Dictionary<string, List<DxfObject>> references, string codeName, string handle) : base(codeName)
        {
            this.maxCapacity = 0x7fffffff;
            this.list = list;
            this.references = references;
            this.Owner = document;
            if (string.IsNullOrEmpty(handle))
            {
                this.Owner.NumHandles = base.AsignHandle(this.Owner.NumHandles);
            }
            else
            {
                base.Handle = handle;
            }
            this.Owner.AddedObjects.Add(base.Handle, this);
        }

        public T Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return this.Add(item, true);
        }

        internal abstract T Add(T item, bool assignHandle);
        public void Clear()
        {
            string[] array = new string[this.list.Count];
            this.list.Keys.CopyTo(array, 0);
            foreach (string str in array)
            {
                this.Remove(str);
            }
        }

        public bool Contains(string name) => 
            this.list.ContainsKey(name);

        public bool Contains(T item) => 
            this.list.ContainsValue(item);

        public IEnumerator<T> GetEnumerator() => 
            ((IEnumerator<T>) this.list.Values.GetEnumerator());

        public List<DxfObject> GetReferences(string name)
        {
            if (!this.Contains(name))
            {
                return new List<DxfObject>();
            }
            return new List<DxfObject>(this.references[name]);
        }

        public List<DxfObject> GetReferences(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return this.GetReferences(item.Name);
        }

        public abstract bool Remove(string name);
        public abstract bool Remove(T item);
        IEnumerator IEnumerable.GetEnumerator() => 
            this.list.Values.GetEnumerator();

        public bool TryGetValue(string name, out T item) => 
            this.list.TryGetValue(name, out item);

        public T this[string name] =>
            (this.list.TryGetValue(name, out T local) ? local : default(T));

        public ICollection<T> Items =>
            ((ICollection<T>) this.list.Values);

        public ICollection<string> Names =>
            this.list.Keys;

        public int Count =>
            this.list.Count;

        public int MaxCapacity
        {
            get => 
                this.maxCapacity;
            internal set => 
                (this.maxCapacity = value);
        }

        public DxfDocument Owner
        {
            get => 
                ((DxfDocument) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        internal Dictionary<string, List<DxfObject>> References =>
            this.references;
    }
}

