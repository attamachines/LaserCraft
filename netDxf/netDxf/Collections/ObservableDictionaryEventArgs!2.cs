namespace netDxf.Collections
{
    using System;
    using System.Collections.Generic;

    public class ObservableDictionaryEventArgs<TKey, TValue> : EventArgs
    {
        private readonly KeyValuePair<TKey, TValue> item;
        private bool cancel;

        public ObservableDictionaryEventArgs(KeyValuePair<TKey, TValue> item)
        {
            this.item = item;
            this.cancel = false;
        }

        public KeyValuePair<TKey, TValue> Item =>
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

