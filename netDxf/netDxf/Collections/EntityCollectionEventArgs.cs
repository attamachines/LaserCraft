namespace netDxf.Collections
{
    using netDxf.Entities;
    using System;

    public class EntityCollectionEventArgs : EventArgs
    {
        private readonly EntityObject item;
        private bool cancel;

        public EntityCollectionEventArgs(EntityObject item)
        {
            this.item = item;
            this.cancel = false;
        }

        public EntityObject Item =>
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

