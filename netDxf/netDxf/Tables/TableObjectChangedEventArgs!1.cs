namespace netDxf.Tables
{
    using System;

    public class TableObjectChangedEventArgs<T> : EventArgs
    {
        private readonly T oldValue;
        private T newValue;

        public TableObjectChangedEventArgs(T oldTable, T newTable)
        {
            this.oldValue = oldTable;
            this.newValue = newTable;
        }

        public T OldValue =>
            this.oldValue;

        public T NewValue
        {
            get => 
                this.newValue;
            set => 
                (this.newValue = value);
        }
    }
}

