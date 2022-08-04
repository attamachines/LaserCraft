namespace netDxf.Objects
{
    using netDxf.Tables;
    using System;

    public class UnderlayDgnDefinition : UnderlayDefinition
    {
        private string layout;

        public UnderlayDgnDefinition(string fileName) : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        public UnderlayDgnDefinition(string fileName, string name) : base(fileName, name, UnderlayType.DGN)
        {
            this.layout = "Model";
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new UnderlayDgnDefinition(base.FileName, newName) { Layout = this.layout };

        public string Layout
        {
            get => 
                this.layout;
            set => 
                (this.layout = value);
        }
    }
}

