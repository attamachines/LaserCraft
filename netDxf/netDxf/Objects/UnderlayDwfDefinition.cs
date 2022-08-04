namespace netDxf.Objects
{
    using netDxf.Tables;
    using System;

    public class UnderlayDwfDefinition : UnderlayDefinition
    {
        public UnderlayDwfDefinition(string fileName) : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        public UnderlayDwfDefinition(string fileName, string name) : base(fileName, name, UnderlayType.DWF)
        {
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new UnderlayDwfDefinition(base.FileName, newName);
    }
}

