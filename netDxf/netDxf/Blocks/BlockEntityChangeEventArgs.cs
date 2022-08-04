namespace netDxf.Blocks
{
    using netDxf.Entities;
    using System;

    public class BlockEntityChangeEventArgs : EventArgs
    {
        private readonly EntityObject item;

        public BlockEntityChangeEventArgs(EntityObject item)
        {
            this.item = item;
        }

        public EntityObject Item =>
            this.item;
    }
}

