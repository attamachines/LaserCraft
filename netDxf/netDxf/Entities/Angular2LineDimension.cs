namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class Angular2LineDimension : Dimension
    {
        private double offset;
        private Vector2 startFirstLine;
        private Vector2 endFirstLine;
        private Vector2 startSecondLine;
        private Vector2 endSecondLine;
        private Vector3 arcDefinitionPoint;

        public Angular2LineDimension() : this(Vector2.Zero, Vector2.UnitX, Vector2.Zero, Vector2.UnitY, 0.1)
        {
        }

        public Angular2LineDimension(netDxf.Entities.Line firstLine, netDxf.Entities.Line secondLine, double offset) : this(firstLine, secondLine, offset, Vector3.UnitZ, DimensionStyle.Default)
        {
        }

        public Angular2LineDimension(netDxf.Entities.Line firstLine, netDxf.Entities.Line secondLine, double offset, DimensionStyle style) : this(firstLine, secondLine, offset, Vector3.UnitZ, style)
        {
        }

        public Angular2LineDimension(netDxf.Entities.Line firstLine, netDxf.Entities.Line secondLine, double offset, Vector3 normal) : this(firstLine, secondLine, offset, normal, DimensionStyle.Default)
        {
        }

        public Angular2LineDimension(netDxf.Entities.Line firstLine, netDxf.Entities.Line secondLine, double offset, Vector3 normal, DimensionStyle style) : base(DimensionType.Angular)
        {
            if (firstLine == null)
            {
                throw new ArgumentNullException("firstLine");
            }
            if (secondLine == null)
            {
                throw new ArgumentNullException("secondLine");
            }
            if (Vector3.AreParallel(firstLine.Direction, secondLine.Direction))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }
            Vector3[] points = new Vector3[] { firstLine.StartPoint, firstLine.EndPoint, secondLine.StartPoint, secondLine.EndPoint };
            IList<Vector3> list = MathHelper.Transform(points, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector3 vector = list[0];
            vector = list[0];
            this.startFirstLine = new Vector2(vector.X, vector.Y);
            vector = list[1];
            vector = list[1];
            this.endFirstLine = new Vector2(vector.X, vector.Y);
            vector = list[2];
            vector = list[2];
            this.startSecondLine = new Vector2(vector.X, vector.Y);
            vector = list[3];
            vector = list[3];
            this.endSecondLine = new Vector2(vector.X, vector.Y);
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
            base.Normal = normal;
            vector = list[0];
            base.Elevation = vector.Z;
        }

        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset) : this(startFirstLine, endFirstLine, startSecondLine, endSecondLine, offset, DimensionStyle.Default)
        {
        }

        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset, DimensionStyle style) : base(DimensionType.Angular)
        {
            Vector2 u = endFirstLine - startFirstLine;
            Vector2 v = endSecondLine - startSecondLine;
            if (Vector2.AreParallel(u, v))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }
            this.startFirstLine = startFirstLine;
            this.endFirstLine = endFirstLine;
            this.startSecondLine = startSecondLine;
            this.endSecondLine = endSecondLine;
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
            Angular2LineDimension dimension = new Angular2LineDimension {
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
                StartFirstLine = this.startFirstLine,
                EndFirstLine = this.endFirstLine,
                StartSecondLine = this.startSecondLine,
                EndSecondLine = this.endSecondLine,
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
            Vector2 vector = this.endFirstLine - this.startFirstLine;
            Vector2 vector2 = this.endSecondLine - this.startSecondLine;
            Vector2 u = MathHelper.FindIntersection(this.startFirstLine, vector, this.startSecondLine, vector2);
            if (Vector2.IsNaN(u))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }
            Vector2 v = point - u;
            this.offset = Vector2.Distance(u, point);
            if (Vector2.CrossProduct(vector, vector2) < 0.0)
            {
                Vector2 startFirstLine = this.startFirstLine;
                this.startFirstLine = this.endFirstLine;
                this.endFirstLine = startFirstLine;
                startFirstLine = this.startSecondLine;
                this.startSecondLine = this.endSecondLine;
                this.endSecondLine = startFirstLine;
            }
            double num2 = Vector2.CrossProduct(vector, v);
            double num3 = Vector2.CrossProduct(vector2, v);
            if ((num2 >= 0.0) && (num3 < 0.0))
            {
            }
            if ((num2 >= 0.0) && (num3 >= 0.0))
            {
                Vector2 startFirstLine = this.startFirstLine;
                this.startFirstLine = this.endFirstLine;
                this.endFirstLine = startFirstLine;
            }
            if ((num2 < 0.0) && (num3 >= 0.0))
            {
                Vector2 startFirstLine = this.startFirstLine;
                this.startFirstLine = this.endFirstLine;
                this.endFirstLine = startFirstLine;
                startFirstLine = this.startSecondLine;
                this.startSecondLine = this.endSecondLine;
                this.endSecondLine = startFirstLine;
            }
            if ((num2 < 0.0) && (num3 < 0.0))
            {
                Vector2 startSecondLine = this.startSecondLine;
                this.startSecondLine = this.endSecondLine;
                this.endSecondLine = startSecondLine;
            }
        }

        internal Vector3 ArcDefinitionPoint
        {
            get => 
                this.arcDefinitionPoint;
            set => 
                (this.arcDefinitionPoint = value);
        }

        public Vector2 StartFirstLine
        {
            get => 
                this.startFirstLine;
            set => 
                (this.startFirstLine = value);
        }

        public Vector2 EndFirstLine
        {
            get => 
                this.endFirstLine;
            set => 
                (this.endFirstLine = value);
        }

        public Vector2 StartSecondLine
        {
            get => 
                this.startSecondLine;
            set => 
                (this.startSecondLine = value);
        }

        public Vector2 EndSecondLine
        {
            get => 
                this.endSecondLine;
            set => 
                (this.endSecondLine = value);
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
                Vector2 u = this.endFirstLine - this.startFirstLine;
                Vector2 v = this.endSecondLine - this.startSecondLine;
                return (Vector2.AngleBetween(u, v) * 57.295779513082323);
            }
        }
    }
}

