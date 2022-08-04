namespace netDxf.Tables
{
    using System;

    public class DimensionStyleOverrideChangeEventArgs : EventArgs
    {
        private readonly DimensionStyleOverride item;

        public DimensionStyleOverrideChangeEventArgs(DimensionStyleOverride item)
        {
            this.item = item;
        }

        public DimensionStyleOverride Item =>
            this.item;
    }
}

