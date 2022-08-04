namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Tables;
    using System;

    public class AlignedDimension : Dimension
    {
        private Vector2 firstRefPoint;
        private Vector2 secondRefPoint;
        private double offset;

        public AlignedDimension() : this(Vector2.Zero, Vector2.UnitX, 0.1)
        {
        }

        public AlignedDimension(netDxf.Entities.Line referenceLine, double offset) : this(referenceLine, offset, Vector3.UnitZ, DimensionStyle.Default)
        {
        }

        public AlignedDimension(netDxf.Entities.Line referenceLine, double offset, DimensionStyle style) : this(referenceLine, offset, Vector3.UnitZ, style)
        {
        }

        public AlignedDimension(netDxf.Entities.Line referenceLine, double offset, Vector3 normal) : this(referenceLine, offset, normal, DimensionStyle.Default)
        {
        }

        public AlignedDimension(Vector2 firstPoint, Vector2 secondPoint, double offset) : this(firstPoint, secondPoint, offset, DimensionStyle.Default)
        {
        }

        public AlignedDimension(netDxf.Entities.Line referenceLine, double offset, Vector3 normal, DimensionStyle style) : base(DimensionType.Aligned)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException("referenceLine");
            }
            Vector3 vector = MathHelper.Transform(referenceLine.StartPoint, normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.firstRefPoint = new Vector2(vector.X, vector.Y);
            vector = MathHelper.Transform(referenceLine.EndPoint, normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.secondRefPoint = new Vector2(vector.X, vector.Y);
            this.offset = offset;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            base.Style = style;
            base.Normal = normal;
            base.Elevation = vector.Z;
        }

        public AlignedDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, DimensionStyle style) : base(DimensionType.Aligned)
        {
            this.firstRefPoint = firstPoint;
            this.secondRefPoint = secondPoint;
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
            AlignedDimension dimension = new AlignedDimension {
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
                FirstReferencePoint = this.firstRefPoint,
                SecondReferencePoint = this.secondRefPoint,
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
            Vector2 u = Vector2.Normalize(this.secondRefPoint - this.firstRefPoint);
            double num = Vector2.Angle(u);
            Vector2 vector2 = point - this.firstRefPoint;
            double num2 = Vector2.CrossProduct(vector2, u);
            this.offset = Math.Sign(num2) * MathHelper.PointLineDistance(point, this.firstRefPoint, u);
            if ((num <= 1.5707963267948966) || (num > 4.71238898038469))
            {
                this.offset *= -1.0;
            }
        }

        public Vector2 FirstReferencePoint
        {
            get => 
                this.firstRefPoint;
            set => 
                (this.firstRefPoint = value);
        }

        public Vector2 SecondReferencePoint
        {
            get => 
                this.secondRefPoint;
            set => 
                (this.secondRefPoint = value);
        }

        public double Offset
        {
            get => 
                this.offset;
            set => 
                (this.offset = value);
        }

        public override double Measurement =>
            Vector2.Distance(this.firstRefPoint, this.secondRefPoint);
    }
}

