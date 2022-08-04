namespace netDxf.Collections
{
    using netDxf.Entities;
    using System;

    public class AttributeDefinitionDictionaryEventArgs : EventArgs
    {
        private readonly AttributeDefinition item;
        private bool cancel;

        public AttributeDefinitionDictionaryEventArgs(AttributeDefinition item)
        {
            this.item = item;
            this.cancel = false;
        }

        public AttributeDefinition Item =>
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

