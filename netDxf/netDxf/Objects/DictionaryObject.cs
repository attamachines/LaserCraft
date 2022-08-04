namespace netDxf.Objects
{
    using netDxf;
    using System;
    using System.Collections.Generic;

    public class DictionaryObject : DxfObject
    {
        private readonly Dictionary<string, string> entries;
        private bool isHardOwner;
        private DictionaryCloningFlags cloning;

        internal DictionaryObject(DxfObject owner) : base("DICTIONARY")
        {
            this.isHardOwner = false;
            this.cloning = DictionaryCloningFlags.KeepExisting;
            this.entries = new Dictionary<string, string>();
            base.Owner = owner;
        }

        public Dictionary<string, string> Entries =>
            this.entries;

        public bool IsHardOwner
        {
            get => 
                this.isHardOwner;
            set => 
                (this.isHardOwner = value);
        }

        public DictionaryCloningFlags Cloning
        {
            get => 
                this.cloning;
            set => 
                (this.cloning = value);
        }
    }
}

