namespace netDxf.Objects
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Collections;
    using netDxf.Entities;
    using netDxf.Tables;
    using System;

    public class Layout : TableObject, IComparable<Layout>
    {
        private netDxf.Objects.PlotSettings plot;
        private Vector2 minLimit;
        private Vector2 maxLimit;
        private Vector3 minExtents;
        private Vector3 maxExtents;
        private Vector3 basePoint;
        private double elevation;
        private Vector3 origin;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private short tabOrder;
        private netDxf.Entities.Viewport viewport;
        private readonly bool isPaperSpace;
        private Block associatedBlock;
        public const string ModelSpaceName = "Model";

        public Layout(string name) : this(name, null, new netDxf.Objects.PlotSettings())
        {
        }

        private Layout(string name, Block associatedBlock, netDxf.Objects.PlotSettings plotSettings) : base(name, "LAYOUT", true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The layout name should be at least one character long.");
            }
            if (name.Equals("Model", StringComparison.OrdinalIgnoreCase))
            {
                base.IsReserved = true;
                this.isPaperSpace = false;
                this.viewport = null;
                plotSettings.Flags = PlotFlags.Initializing | PlotFlags.UpdatePaper | PlotFlags.ModelType | PlotFlags.DrawViewportsFirst | PlotFlags.PrintLineweights | PlotFlags.PlotPlotStyles | PlotFlags.UseStandardScale;
            }
            else
            {
                base.IsReserved = false;
                this.isPaperSpace = true;
                netDxf.Entities.Viewport viewport1 = new netDxf.Entities.Viewport(1) {
                    ViewCenter = new Vector2(50.0, 100.0)
                };
                this.viewport = viewport1;
            }
            this.tabOrder = 0;
            this.associatedBlock = associatedBlock;
            this.plot = plotSettings;
            this.minLimit = new Vector2(-20.0, -7.5);
            this.maxLimit = new Vector2(277.0, 202.5);
            this.basePoint = Vector3.Zero;
            this.minExtents = new Vector3(25.7, 19.5, 0.0);
            this.maxExtents = new Vector3(231.3, 175.5, 0.0);
            this.elevation = 0.0;
            this.origin = Vector3.Zero;
            this.xAxis = Vector3.UnitX;
            this.yAxis = Vector3.UnitY;
        }

        internal override long AsignHandle(long entityNumber)
        {
            entityNumber = this.Owner.AsignHandle(entityNumber);
            if (this.isPaperSpace)
            {
                entityNumber = this.viewport.AsignHandle(entityNumber);
            }
            return base.AsignHandle(entityNumber);
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName)
        {
            if ((base.Name == "Model") || (newName == "Model"))
            {
                throw new NotSupportedException("The Model layout cannot be cloned.");
            }
            return new Layout(newName, null, (netDxf.Objects.PlotSettings) this.plot.Clone()) { 
                TabOrder = this.tabOrder,
                MinLimit = this.minLimit,
                MaxLimit = this.maxLimit,
                BasePoint = this.basePoint,
                MinExtents = this.minExtents,
                MaxExtents = this.maxExtents,
                Elevation = this.elevation,
                UcsOrigin = this.origin,
                UcsXAxis = this.xAxis,
                UcsYAxis = this.yAxis,
                Viewport = (netDxf.Entities.Viewport) this.viewport.Clone()
            };
        }

        public int CompareTo(Layout other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return this.tabOrder.CompareTo(other.tabOrder);
        }

        public static Layout ModelSpace =>
            new Layout("Model", Block.ModelSpace, new netDxf.Objects.PlotSettings());

        public short TabOrder
        {
            get => 
                this.tabOrder;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("The tab order index must be greater than zero.", "value");
                }
                this.tabOrder = value;
            }
        }

        public netDxf.Objects.PlotSettings PlotSettings
        {
            get => 
                this.plot;
            set => 
                (this.plot = value);
        }

        public Vector2 MinLimit
        {
            get => 
                this.minLimit;
            set => 
                (this.minLimit = value);
        }

        public Vector2 MaxLimit
        {
            get => 
                this.maxLimit;
            set => 
                (this.maxLimit = value);
        }

        public Vector3 MinExtents
        {
            get => 
                this.minExtents;
            set => 
                (this.minExtents = value);
        }

        public Vector3 MaxExtents
        {
            get => 
                this.maxExtents;
            set => 
                (this.maxExtents = value);
        }

        public Vector3 BasePoint
        {
            get => 
                this.basePoint;
            set => 
                (this.basePoint = value);
        }

        public double Elevation
        {
            get => 
                this.elevation;
            set => 
                (this.elevation = value);
        }

        public Vector3 UcsOrigin
        {
            get => 
                this.origin;
            set => 
                (this.origin = value);
        }

        public Vector3 UcsXAxis
        {
            get => 
                this.xAxis;
            set => 
                (this.xAxis = value);
        }

        public Vector3 UcsYAxis
        {
            get => 
                this.yAxis;
            set => 
                (this.yAxis = value);
        }

        public bool IsPaperSpace =>
            this.isPaperSpace;

        public netDxf.Entities.Viewport Viewport
        {
            get => 
                this.viewport;
            internal set => 
                (this.viewport = value);
        }

        public Layouts Owner
        {
            get => 
                ((Layouts) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public Block AssociatedBlock
        {
            get => 
                this.associatedBlock;
            internal set => 
                (this.associatedBlock = value);
        }
    }
}

