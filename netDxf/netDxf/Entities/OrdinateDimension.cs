namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Tables;
    using System;

    public class OrdinateDimension : Dimension
    {
        private Vector2 origin;
        private Vector2 referencePoint;
        private double length;
        private double rotation;
        private OrdinateDimensionAxis axis;
        private Vector3 firstPoint;
        private Vector3 secondPoint;

        public OrdinateDimension() : this(Vector2.Zero, Vector2.UnitX, 0.1, OrdinateDimensionAxis.Y, (double) 0.0)
        {
        }

        public OrdinateDimension(Vector2 origin, Vector2 referencePoint, double length, OrdinateDimensionAxis axis) : this(origin, referencePoint, length, axis, 0.0, DimensionStyle.Default)
        {
        }

        public OrdinateDimension(Vector2 origin, Vector2 referencePoint, double length, OrdinateDimensionAxis axis, DimensionStyle style) : this(origin, referencePoint, length, axis, 0.0, style)
        {
        }

        public OrdinateDimension(Vector2 origin, Vector2 referencePoint, double length, OrdinateDimensionAxis axis, double rotation) : this(origin, referencePoint, length, axis, rotation, DimensionStyle.Default)
        {
        }

        public OrdinateDimension(Vector2 origin, Vector2 referencePoint, double length, OrdinateDimensionAxis axis, double rotation, DimensionStyle style) : base(DimensionType.Ordinate)
        {
            this.origin = origin;
            this.rotation = MathHelper.NormalizeAngle(rotation);
            this.length = length;
            this.referencePoint = referencePoint;
            this.axis = axis;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            base.Style = style;
        }

        internal override Block BuildBlock(string name) => 
            DimensionBlock.Build(this, name);

        public override object Clone()
        {
            OrdinateDimension dimension = new OrdinateDimension {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Style = (DimensionStyle) base.Style.Clone(),
                AttachmentPoint = base.AttachmentPoint,
                LineSpacingStyle = base.LineSpacingStyle,
                LineSpacingFactor = base.LineSpacingFactor,
                UserText = base.UserText,
                Origin = this.origin,
                ReferencePoint = this.referencePoint,
                Rotation = this.rotation,
                Length = this.length,
                Axis = this.axis,
                Elevation = base.Elevation
            };
            foreach (XData data in base.XData.Values)
            {
                dimension.XData.Add((XData) data.Clone());
            }
            return dimension;
        }

        internal Vector3 FirstPoint
        {
            get => 
                this.firstPoint;
            set => 
                (this.firstPoint = value);
        }

        internal Vector3 SecondPoint
        {
            get => 
                this.secondPoint;
            set => 
                (this.secondPoint = value);
        }

        public Vector2 Origin
        {
            get => 
                this.origin;
            set => 
                (this.origin = value);
        }

        public Vector2 ReferencePoint
        {
            get => 
                this.referencePoint;
            set => 
                (this.referencePoint = value);
        }

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                MathHelper.NormalizeAngle(this.rotation = value);
        }

        public double Length
        {
            get => 
                this.length;
            set => 
                (this.length = value);
        }

        public OrdinateDimensionAxis Axis
        {
            get => 
                this.axis;
            set => 
                (this.axis = value);
        }

        public override double Measurement =>
            ((this.axis == OrdinateDimensionAxis.X) ? this.referencePoint.X : this.referencePoint.Y);
    }
}

