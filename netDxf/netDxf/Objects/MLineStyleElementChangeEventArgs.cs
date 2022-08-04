namespace netDxf.Objects
{
    using System;

    public class MLineStyleElementChangeEventArgs : EventArgs
    {
        private readonly MLineStyleElement item;

        public MLineStyleElementChangeEventArgs(MLineStyleElement item)
        {
            this.item = item;
        }

        public MLineStyleElement Item =>
            this.item;
    }
}

