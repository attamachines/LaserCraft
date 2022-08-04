namespace netDxf
{
    using System;

    public abstract class DxfObject
    {
        private string codename;
        private string handle;
        private DxfObject owner;

        protected DxfObject(string codename)
        {
            this.codename = codename;
            this.handle = null;
            this.owner = null;
        }

        internal virtual long AsignHandle(long entityNumber)
        {
            this.handle = entityNumber.ToString("X");
            return (entityNumber + 1L);
        }

        public override string ToString() => 
            this.codename;

        public string CodeName
        {
            get => 
                this.codename;
            internal set => 
                (this.codename = value);
        }

        public string Handle
        {
            get => 
                this.handle;
            internal set => 
                (this.handle = value);
        }

        public DxfObject Owner
        {
            get => 
                this.owner;
            internal set => 
                (this.owner = value);
        }
    }
}

