namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Tables;
    using System;

    public class Angular3PointDimension : Dimension
    {
        private double offset;
        private Vector2 center;
        private Vector2 start;
        private Vector2 end;

        public Angular3PointDimension() : this(Vector2.Zero, Vector2.UnitX, Vector2.UnitY, 0.1)
        {
        }

        public Angular3PointDimension(netDxf.Entities.Arc arc, double offset) : this(arc, offset, DimensionStyle.Default)
        {
        }

        public Angular3PointDimension(netDxf.Entities.Arc arc, double offset, DimensionStyle style) : base(DimensionType.Angular3Point)
        {
            if (arc == null)
            {
                throw new ArgumentNullException("arc");
            }
            Vector3 vector = MathHelper.Transform(arc.Center, arc.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.center = new Vector2(vector.X, vector.Y);
            this.start = Vector2.Polar(this.center, arc.Radius, arc.StartAngle * 0.017453292519943295);
            this.end = Vector2.Polar(this.center, arc.Radius, arc.EndAngle * 0.017453292519943295);
            if (MathHelper.IsZero(offset))
            {
                throw new ArgumentOutOfRangeException("offset", "The offset value cannot be zero.");
            }
            this.offset = offset;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            base.Style = style;
            base.Normal = arc.Normal;
            base.Elevation = vector.Z;
        }

        public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset) : this(centerPoint, startPoint, endPoint, offset, DimensionStyle.Default)
        {
        }

        public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset, DimensionStyle style) : base(DimensionType.Angular3Point)
        {
            this.center = centerPoint;
            this.start = startPoint;
            this.end = endPoint;
            if (MathHelper.IsZero(offset))
            {
                throw new ArgumentOutOfRangeException("offset", "The offset value cannot be zero.");
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
            Angular3PointDimension dimension = new Angular3PointDimension {
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
                StartPoint = this.start,
                EndPoint = this.end,
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
            double angle = (Vector2.Angle(this.center, this.end) - Vector2.Angle(this.center, this.start)) * 57.295779513082323;
            angle = MathHelper.NormalizeAngle(angle);
            double num2 = (Vector2.Angle(this.center, this.end) - Vector2.Angle(this.center, point)) * 57.295779513082323;
            num2 = MathHelper.NormalizeAngle(num2);
            this.offset = Vector2.Distance(this.center, point);
            if (num2 > angle)
            {
                this.offset *= -1.0;
            }
        }

        public Vector2 CenterPoint
        {
            get => 
                this.center;
            set => 
                (this.center = value);
        }

        public Vector2 StartPoint
        {
            get => 
                this.start;
            set => 
                (this.start = value);
        }

        public Vector2 EndPoint
        {
            get => 
                this.end;
            set => 
                (this.end = value);
        }

        public double Offset
        {
            get => 
                this.offset;
            set
            {
                if (MathHelper.IsZero(value))
                {
                    throw new ArgumentOutOfRangeException("value", "The offset value cannot be zero.");
                }
                this.offset = value;
            }
        }

        public override double Measurement
        {
            get
            {
                Vector2 start = this.start;
                Vector2 end = this.end;
                if (this.offset < 0.0)
                {
                    Vector2 vector3 = start;
                    start = end;
                    end = vector3;
                }
                double angle = (Vector2.Angle(this.center, end) - Vector2.Angle(this.center, start)) * 57.295779513082323;
                return MathHelper.NormalizeAngle(angle);
            }
        }
    }
}

