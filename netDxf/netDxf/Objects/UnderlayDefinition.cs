namespace netDxf.Objects
{
    using netDxf.Tables;
    using System;

    public abstract class UnderlayDefinition : TableObject
    {
        private readonly UnderlayType type;
        private readonly string fileName;

        protected UnderlayDefinition(string fileName, string name, UnderlayType type) : base(name, "UNDERLAYDEFINITION", false)
        {
            this.fileName = fileName;
            this.type = type;
            switch (type)
            {
                case UnderlayType.DGN:
                    base.CodeName = "DGNDEFINITION";
                    break;

                case UnderlayType.DWF:
                    base.CodeName = "DWFDEFINITION";
                    break;

                case UnderlayType.PDF:
                    base.CodeName = "PDFDEFINITION";
                    break;
            }
        }

        public UnderlayType Type =>
            this.type;

        public string FileName =>
            this.fileName;
    }
}

