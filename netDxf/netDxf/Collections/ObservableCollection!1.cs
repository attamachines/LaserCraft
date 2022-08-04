namespace netDxf.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class ObservableCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private readonly List<T> innerArray;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AddItemEventHandler<T> AddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeAddItemEventHandler<T> BeforeAddItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BeforeRemoveItemEventHandler<T> BeforeRemoveItem;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event RemoveItemEventHandler<T> RemoveItem;

        public ObservableCollection()
        {
            this.innerArray = new List<T>();
        }

        public ObservableCollection(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "The collection capacity cannot be negative.");
            }
            this.innerArray = new List<T>(capacity);
        }

        public void Add(T item)
        {
            if (this.OnBeforeAddItemEvent(item))
            {
                throw new ArgumentException("The item cannot be added to the collection.", "item");
            }
            this.innerArray.Add(item);
            this.OnAddItemEvent(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            foreach (T local in collection)
            {
                this.Add(local);
            }
        }

        public void Clear()
        {
            T[] array = new T[this.innerArray.Count];
            this.innerArray.CopyTo(array, 0);
            foreach (T local in array)
            {
                this.Remove(local);
            }
        }

        public bool Contains(T item) => 
            this.innerArray.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.innerArray.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator() => 
            this.innerArray.GetEnumerator();

        public int IndexOf(T item) => 
            this.innerArray.IndexOf(item);

        public void Insert(int index, T item)
        {
            if ((index < 0) || (index >= this.innerArray.Count))
            {
                throw new ArgumentOutOfRangeException($"The parameter index {index} must be in between {0} and {this.innerArray.Count}.");
            }
            if (!this.OnBeforeRemoveItemEvent(this.innerArray[index]))
            {
                if (this.OnBeforeAddItemEvent(item))
                {
                    throw new ArgumentException("The item cannot be added to the collection.", "item");
                }
                this.OnRemoveItemEvent(this.innerArray[index]);
                this.innerArray.Insert(index, item);
                this.OnAddItemEvent(item);
            }
        }

        protected virtual void OnAddItemEvent(T item)
        {
            AddItemEventHandler<T> addItem = this.AddItem;
            if (addItem > null)
            {
                addItem((ObservableCollection<T>) this, new ObservableCollectionEventArgs<T>(item));
            }
        }

        protected virtual bool OnBeforeAddItemEvent(T item)
        {
            BeforeAddItemEventHandler<T> beforeAddItem = this.BeforeAddItem;
            if (beforeAddItem > null)
            {
                ObservableCollectionEventArgs<T> e = new ObservableCollectionEventArgs<T>(item);
                beforeAddItem((ObservableCollection<T>) this, e);
                return e.Cancel;
            }
            return false;
        }

        protected virtual bool OnBeforeRemoveItemEvent(T item)
        {
            BeforeRemoveItemEventHandler<T> beforeRemoveItem = this.BeforeRemoveItem;
            if (beforeRemoveItem > null)
            {
                ObservableCollectionEventArgs<T> e = new ObservableCollectionEventArgs<T>(item);
                beforeRemoveItem((ObservableCollection<T>) this, e);
                return e.Cancel;
            }
            return false;
        }

        protected virtual void OnRemoveItemEvent(T item)
        {
            RemoveItemEventHandler<T> removeItem = this.RemoveItem;
            if (removeItem > null)
            {
                removeItem((ObservableCollection<T>) this, new ObservableCollectionEventArgs<T>(item));
            }
        }

        public bool Remove(T item)
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

        public void Remove(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (T local in items)
            {
                if (!this.innerArray.Contains(local) || this.OnBeforeRemoveItemEvent(local))
                {
                    break;
                }
                this.innerArray.Remove(local);
                this.OnRemoveItemEvent(local);
            }
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.innerArray.Count))
            {
                throw new ArgumentOutOfRangeException($"The parameter index {index} must be in between {0} and {this.innerArray.Count}.");
            }
            T item = this.innerArray[index];
            if (!this.OnBeforeRemoveItemEvent(item))
            {
                this.innerArray.RemoveAt(index);
                this.OnRemoveItemEvent(item);
            }
        }

        public void Reverse()
        {
            this.innerArray.Reverse();
        }

        public void Sort()
        {
            this.innerArray.Sort();
        }

        public void Sort(IComparer<T> comparer)
        {
            this.innerArray.Sort(comparer);
        }

        public void Sort(Comparison<T> comparision)
        {
            this.innerArray.Sort(comparision);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            this.innerArray.Sort(index, count, comparer);
        }

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            this.Insert(index, item);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public T this[int index]
        {
            get => 
                this.innerArray[index];
            set
            {
                T item = this.innerArray[index];
                T local2 = value;
                if (!this.OnBeforeRemoveItemEvent(item) && !this.OnBeforeAddItemEvent(local2))
                {
                    this.innerArray[index] = value;
                    this.OnAddItemEvent(local2);
                    this.OnRemoveItemEvent(item);
                }
            }
        }

        public int Count =>
            this.innerArray.Count;

        public virtual bool IsReadOnly =>
            false;

        public delegate void AddItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);

        public delegate void BeforeAddItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);

        public delegate void BeforeRemoveItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);

        public delegate void RemoveItemEventHandler(ObservableCollection<T> sender, ObservableCollectionEventArgs<T> e);
    }
}

