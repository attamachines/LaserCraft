namespace netDxf.Collections
{
    using System;

    public class ObservableCollectionEventArgs<T> : EventArgs
    {
        private readonly T item;
        private bool cancel;

        public ObservableCollectionEventArgs(T item)
        {
            this.item = item;
            this.cancel = false;
        }

        public T Item =>
            this.item;

        public bool Cancel
        {
            get => 
                this.cancel;
            set => 
                (this.cancel = value);
        }
    }
}

