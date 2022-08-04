namespace netDxf.Collections
{
    using netDxf.Entities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class EntityCollection : IList<EntityObject>, ICollection<EntityObject>, IEnumerable<EntityObject>, IEnumerable
    {
        private readonly List<EntityObject> innerArray;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AddItemEventHandler AddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeAddItemEventHandler BeforeAddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeRemoveItemEventHandler BeforeRemoveItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event RemoveItemEventHandler RemoveItem;

        public EntityCollection()
        {
            this.innerArray = new List<EntityObject>();
        }

        public EntityCollection(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "The collection capacity cannot be negative.");
            }
            this.innerArray = new List<EntityObject>(capacity);
        }

        public void Add(EntityObject item)
        {
            if (this.OnBeforeAddItemEvent(item))
            {
                throw new ArgumentException("The entity cannot be added to the collection.", "item");
            }
            this.innerArray.Add(item);
            this.OnAddItemEvent(item);
        }

        public void AddRange(IEnumerable<EntityObject> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            foreach (EntityObject obj2 in collection)
            {
                this.Add(obj2);
            }
        }

        public void Clear()
        {
            EntityObject[] array = new EntityObject[this.innerArray.Count];
            this.innerArray.CopyTo(array, 0);
            foreach (EntityObject obj2 in array)
            {
                this.Remove(obj2);
            }
        }

        public bool Contains(EntityObject item) => 
            this.innerArray.Contains(item);

        public void CopyTo(EntityObject[] array, int arrayIndex)
        {
            this.innerArray.CopyTo(array, arrayIndex);
        }

        public IEnumerator<EntityObject> GetEnumerator() => 
            this.innerArray.GetEnumerator();

        public int IndexOf(EntityObject item) => 
            this.innerArray.IndexOf(item);

        public void Insert(int index, EntityObject item)
        {
            if ((index < 0) || (index >= this.innerArray.Count))
            {
                throw new ArgumentOutOfRangeException($"The parameter index {index} must be in between {0} and {this.innerArray.Count}.");
            }
            if (!this.OnBeforeRemoveItemEvent(this.innerArray[index]))
            {
                if (this.OnBeforeAddItemEvent(item))
                {
                    throw new ArgumentException("The entity cannot be added to the collection.", "item");
                }
                this.OnRemoveItemEvent(this.innerArray[index]);
                this.innerArray.Insert(index, item);
                this.OnAddItemEvent(item);
            }
        }

        protected virtual void OnAddItemEvent(EntityObject item)
        {
            AddItemEventHandler addItem = this.AddItem;
            if (addItem > null)
            {
                addItem(this, new EntityCollectionEventArgs(item));
            }
        }

        protected virtual bool OnBeforeAddItemEvent(EntityObject item)
        {
            BeforeAddItemEventHandler beforeAddItem = this.BeforeAddItem;
            if (beforeAddItem > null)
            {
                EntityCollectionEventArgs e = new EntityCollectionEventArgs(item);
                beforeAddItem(this, e);
                return e.Cancel;
            }
            return false;
        }

        protected virtual bool OnBeforeRemoveItemEvent(EntityObject item)
        {
            BeforeRemoveItemEventHandler beforeRemoveItem = this.BeforeRemoveItem;
            if (beforeRemoveItem > null)
            {
                EntityCollectionEventArgs e = new EntityCollectionEventArgs(item);
                beforeRemoveItem(this, e);
                return e.Cancel;
            }
            return false;
        }

        protected virtual void OnRemoveItemEvent(EntityObject item)
        {
            RemoveItemEventHandler removeItem = this.RemoveItem;
            if (removeItem > null)
            {
                removeItem(this, new EntityCollectionEventArgs(item));
            }
        }

        public bool Remove(EntityObject item)
        {
            if (!this.innerArray.Contains(item))
            {
                return false;
            }
            if (this.OnBeforeRemoveItemEvent(item))
            {
                return false;
            }
            this.innerArray.Remove(item);
            this.OnRemoveItemEvent(item);
            return true;
        }

        public void Remove(IEnumerable<EntityObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (EntityObject obj2 in items)
            {
                if (!this.innerArray.Contains(obj2) || this.OnBeforeRemoveItemEvent(obj2))
                {
                    break;
                }
                this.innerArray.Remove(obj2);
                this.OnRemoveItemEvent(obj2);
            }
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.innerArray.Count))
            {
                throw new ArgumentOutOfRangeException($"The parameter index {index} must be in between {0} and {this.innerArray.Count}.");
            }
            EntityObject item = this.innerArray[index];
            if (!this.OnBeforeRemoveItemEvent(item))
            {
                this.innerArray.RemoveAt(index);
                this.OnRemoveItemEvent(item);
            }
        }

        void ICollection<EntityObject>.Add(EntityObject item)
        {
            this.Add(item);
        }

        void IList<EntityObject>.Insert(int index, EntityObject item)
        {
            this.Insert(index, item);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public EntityObject this[int index]
        {
            get => 
                this.innerArray[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                EntityObject item = this.innerArray[index];
                if (!this.OnBeforeRemoveItemEvent(item) && !this.OnBeforeAddItemEvent(value))
                {
                    this.innerArray[index] = value;
                    this.OnAddItemEvent(value);
                    this.OnRemoveItemEvent(item);
                }
            }
        }

        public int Count =>
            this.innerArray.Count;

        public virtual bool IsReadOnly =>
            false;

        public delegate void AddItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);

        public delegate void BeforeAddItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);

        public delegate void BeforeRemoveItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);

        public delegate void RemoveItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);
    }
}

