namespace netDxf.Objects
{
    using netDxf.Tables;
    using System;

    public class UnderlayPdfDefinition : UnderlayDefinition
    {
        private string page;

        public UnderlayPdfDefinition(string fileName) : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        public UnderlayPdfDefinition(string fileName, string name) : base(fileName, name, UnderlayType.PDF)
        {
            this.page = "1";
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new UnderlayPdfDefinition(base.FileName, newName) { Page = this.page };

        public string Page
        {
            get => 
                this.page;
            set => 
                (this.page = value);
        }
    }
}

