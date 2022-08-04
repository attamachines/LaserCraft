namespace netDxf.Collections
{
    using netDxf.Tables;
    using System;

    public class DimensionStyleOverrideDictionaryEventArgs : EventArgs
    {
        private readonly DimensionStyleOverride item;
        private bool cancel;

        public DimensionStyleOverrideDictionaryEventArgs(DimensionStyleOverride item)
        {
            this.item = item;
            this.cancel = false;
        }

        public DimensionStyleOverride Item =>
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

