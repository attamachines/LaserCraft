namespace netDxf.Blocks
{
    using netDxf;
    using netDxf.Collections;
    using netDxf.Objects;
    using netDxf.Units;
    using System;

    public class BlockRecord : DxfObject
    {
        private string name;
        private netDxf.Objects.Layout layout;
        private static DrawingUnits defaultUnits = DrawingUnits.Unitless;
        private DrawingUnits units;
        private bool allowExploding;
        private bool scaleUniformly;
        private readonly XDataDictionary xData;

        public BlockRecord(string name) : base("BLOCK_RECORD")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            this.name = name;
            this.layout = null;
            this.units = DefaultUnits;
            this.allowExploding = true;
            this.scaleUniformly = false;
            this.xData = new XDataDictionary();
        }

        public override string ToString() => 
            this.Name;

        public string Name
        {
            get => 
                this.name;
            internal set => 
                (this.name = value);
        }

        public netDxf.Objects.Layout Layout
        {
            get => 
                this.layout;
            internal set => 
                (this.layout = value);
        }

        public DrawingUnits Units
        {
            get => 
                this.units;
            set => 
                (this.units = value);
        }

        public static DrawingUnits DefaultUnits
        {
            get => 
                defaultUnits;
            set => 
                (defaultUnits = value);
        }

        public bool AllowExploding
        {
            get => 
                this.allowExploding;
            set => 
                (this.allowExploding = value);
        }

        public bool ScaleUniformly
        {
            get => 
                this.scaleUniformly;
            set => 
                (this.scaleUniformly = value);
        }

        public BlockRecords Owner
        {
            get => 
                ((BlockRecords) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public bool IsForInternalUseOnly =>
            this.name.StartsWith("*");

        public XDataDictionary XData =>
            this.xData;
    }
}

