namespace netDxf.Entities
{
    using System;

    public class AttributeChangeEventArgs : EventArgs
    {
        private readonly netDxf.Entities.Attribute item;

        public AttributeChangeEventArgs(netDxf.Entities.Attribute item)
        {
            this.item = item;
        }

        public netDxf.Entities.Attribute Item =>
            this.item;
    }
}

