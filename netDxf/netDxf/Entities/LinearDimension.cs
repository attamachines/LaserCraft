namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Tables;
    using System;

    public class LinearDimension : Dimension
    {
        private Vector2 firstRefPoint;
        private Vector2 secondRefPoint;
        private double offset;
        private double rotation;

        public LinearDimension() : this(Vector2.Zero, Vector2.UnitX, 0.1, 0.0)
        {
        }

        public LinearDimension(netDxf.Entities.Line referenceLine, double offset, double rotation) : this(referenceLine, offset, rotation, Vector3.UnitZ, DimensionStyle.Default)
        {
        }

        public LinearDimension(netDxf.Entities.Line referenceLine, double offset, double rotation, DimensionStyle style) : this(referenceLine, offset, rotation, Vector3.UnitZ, style)
        {
        }

        public LinearDimension(netDxf.Entities.Line referenceLine, double offset, double rotation, Vector3 normal) : this(referenceLine, offset, rotation, normal, DimensionStyle.Default)
        {
        }

        public LinearDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, double rotation) : this(firstPoint, secondPoint, offset, rotation, DimensionStyle.Default)
        {
        }

        public LinearDimension(netDxf.Entities.Line referenceLine, double offset, double rotation, Vector3 normal, DimensionStyle style) : base(DimensionType.Linear)
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
            this.rotation = MathHelper.NormalizeAngle(rotation);
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            base.Style = style;
            base.Normal = normal;
            base.Elevation = vector.Z;
        }

        public LinearDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, double rotation, DimensionStyle style) : base(DimensionType.Linear)
        {
            this.firstRefPoint = firstPoint;
            this.secondRefPoint = secondPoint;
            this.offset = offset;
            this.rotation = MathHelper.NormalizeAngle(rotation);
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
            LinearDimension dimension = new LinearDimension {
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
                Rotation = this.rotation,
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
            Vector2 p = Vector2.MidPoint(this.firstRefPoint, this.secondRefPoint);
            double d = this.rotation * 0.017453292519943295;
            Vector2 dir = new Vector2(Math.Cos(d), Math.Sin(d));
            Vector2 u = Vector2.Normalize(this.secondRefPoint - this.firstRefPoint);
            Vector2 v = point - this.firstRefPoint;
            double num2 = Vector2.CrossProduct(u, v);
            this.offset = MathHelper.PointLineDistance(p, point, dir);
            if (num2 < 0.0)
            {
                this.offset *= -1.0;
            }
            double num3 = Vector2.AngleBetween(u, dir);
            if ((num3 > 1.5707963267948966) && (num3 <= 4.71238898038469))
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

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = MathHelper.NormalizeAngle(value));
        }

        public double Offset
        {
            get => 
                this.offset;
            set => 
                (this.offset = value);
        }

        public override double Measurement
        {
            get
            {
                double num = Vector2.Angle(this.firstRefPoint, this.secondRefPoint);
                return Math.Abs((double) (Vector2.Distance(this.firstRefPoint, this.secondRefPoint) * Math.Cos((this.rotation * 0.017453292519943295) - num)));
            }
        }
    }
}

