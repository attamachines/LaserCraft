namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Tables;
    using System;

    public class RadialDimension : Dimension
    {
        private Vector2 center;
        private Vector2 refPoint;
        private double offset;

        public RadialDimension() : this(Vector2.Zero, Vector2.UnitX, 0.0, DimensionStyle.Default)
        {
        }

        public RadialDimension(netDxf.Entities.Arc arc, double rotation, double offset) : this(arc, rotation, offset, DimensionStyle.Default)
        {
        }

        public RadialDimension(Circle circle, double rotation, double offset) : this(circle, rotation, offset, DimensionStyle.Default)
        {
        }

        public RadialDimension(Vector2 centerPoint, Vector2 referencePoint, double offset) : this(centerPoint, referencePoint, offset, DimensionStyle.Default)
        {
        }

        public RadialDimension(netDxf.Entities.Arc arc, double rotation, double offset, DimensionStyle style) : base(DimensionType.Radius)
        {
            if (arc == null)
            {
                throw new ArgumentNullException("arc");
            }
            Vector3 vector = MathHelper.Transform(arc.Center, arc.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.center = new Vector2(vector.X, vector.Y);
            this.refPoint = Vector2.Polar(this.center, arc.Radius, rotation * 0.017453292519943295);
            this.offset = offset;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            base.Style = style;
            base.Normal = arc.Normal;
            base.Elevation = vector.Z;
        }

        public RadialDimension(Circle circle, double rotation, double offset, DimensionStyle style) : base(DimensionType.Radius)
        {
            if (circle == null)
            {
                throw new ArgumentNullException("circle");
            }
            Vector3 vector = MathHelper.Transform(circle.Center, circle.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.center = new Vector2(vector.X, vector.Y);
            this.refPoint = Vector2.Polar(this.center, circle.Radius, rotation * 0.017453292519943295);
            if (offset < 0.0)
            {
                throw new ArgumentOutOfRangeException("offset", "The offset value cannot be negative.");
            }
            this.offset = offset;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            base.Style = style;
            base.Normal = circle.Normal;
            base.Elevation = vector.Z;
        }

        public RadialDimension(Vector2 centerPoint, Vector2 referencePoint, double offset, DimensionStyle style) : base(DimensionType.Radius)
        {
            this.center = centerPoint;
            this.refPoint = referencePoint;
            if (offset < 0.0)
            {
                throw new ArgumentOutOfRangeException("offset", "The offset value cannot be negative.");
            }
            this.offset = offset;
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
            RadialDimension dimension = new RadialDimension {
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
                CenterPoint = this.center,
                ReferencePoint = this.refPoint,
                Offset = this.offset,
                Elevation = base.Elevation
            };
            foreach (XData data in base.XData.Values)
            {
                dimension.XData.Add((XData) data.Clone());
            }
            return dimension;
        }

        public void SetDimensionLinePosition(Vector2 point)
        {
            double distance = Vector2.Distance(this.center, this.refPoint);
            double angle = Vector2.Angle(this.center, point);
            this.refPoint = Vector2.Polar(this.center, distance, angle);
            this.offset = Vector2.Distance(this.center, point);
        }

        public Vector2 CenterPoint
        {
            get => 
                this.center;
            set => 
                (this.center = value);
        }

        public Vector2 ReferencePoint
        {
            get => 
                this.refPoint;
            set => 
                (this.refPoint = value);
        }

        public double Offset
        {
            get => 
                this.offset;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", "The offset value cannot be negative.");
                }
                this.offset = value;
            }
        }

        public override double Measurement =>
            Vector2.Distance(this.center, this.refPoint);
    }
}

