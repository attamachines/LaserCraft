namespace netDxf.Objects
{
    using netDxf.Entities;
    using System;

    public class GroupEntityChangeEventArgs : EventArgs
    {
        private readonly EntityObject item;

        public GroupEntityChangeEventArgs(EntityObject item)
        {
            this.item = item;
        }

        public EntityObject Item =>
            this.item;
    }
}

