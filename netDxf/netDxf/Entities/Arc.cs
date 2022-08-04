namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class Arc : EntityObject
    {
        private Vector3 center;
        private double radius;
        private double startAngle;
        private double endAngle;
        private double thickness;

        public Arc() : this(Vector3.Zero, 1.0, 0.0, 180.0)
        {
        }

        public Arc(Vector2 center, double radius, double startAngle, double endAngle) : this(new Vector3(center.X, center.Y, 0.0), radius, startAngle, endAngle)
        {
        }

        public Arc(Vector3 center, double radius, double startAngle, double endAngle) : base(EntityType.Arc, "ARC")
        {
            this.center = center;
            this.radius = radius;
            this.startAngle = MathHelper.NormalizeAngle(startAngle);
            this.endAngle = MathHelper.NormalizeAngle(endAngle);
            this.thickness = 0.0;
        }

        public override object Clone()
        {
            netDxf.Entities.Arc arc = new netDxf.Entities.Arc {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Center = this.center,
                Radius = this.radius,
                StartAngle = this.startAngle,
                EndAngle = this.endAngle,
                Thickness = this.thickness
            };
            foreach (XData data in base.XData.Values)
            {
                arc.XData.Add((XData) data.Clone());
            }
            return arc;
        }

        public IEnumerable<Vector2> PolygonalVertexes(int precision)
        {
            if (precision < 2)
            {
                throw new ArgumentOutOfRangeException("precision", precision, "The arc precision must be greater or equal to three");
            }
            List<Vector2> list = new List<Vector2>();
            double num = this.startAngle * 0.017453292519943295;
            double num2 = this.endAngle * 0.017453292519943295;
            if (num2 < num)
            {
                num2 += 6.2831853071795862;
            }
            double num3 = (num2 - num) / ((double) precision);
            for (int i = 0; i <= precision; i++)
            {
                double a = num + (num3 * i);
                double y = this.radius * Math.Sin(a);
                double x = this.radius * Math.Cos(a);
                list.Add(new Vector2(x, y));
            }
            return list;
        }

        public LwPolyline ToPolyline(int precision)
        {
            IEnumerable<Vector2> enumerable = this.PolygonalVertexes(precision);
            Vector3 vector = MathHelper.Transform(this.center, base.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            LwPolyline polyline = new LwPolyline {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                Elevation = vector.Z,
                Thickness = this.Thickness,
                IsClosed = false
            };
            foreach (Vector2 vector2 in enumerable)
            {
                polyline.Vertexes.Add(new LwPolylineVertex(vector2.X + vector.X, vector2.Y + vector.Y));
            }
            return polyline;
        }

        public Vector3 Center
        {
            get => 
                this.center;
            set => 
                (this.center = value);
        }

        public double Radius
        {
            get => 
                this.radius;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The arc radius must be greater than zero.");
                }
                this.radius = value;
            }
        }

        public double StartAngle
        {
            get => 
                this.startAngle;
            set => 
                (this.startAngle = MathHelper.NormalizeAngle(value));
        }

        public double EndAngle
        {
            get => 
                this.endAngle;
            set => 
                (this.endAngle = MathHelper.NormalizeAngle(value));
        }

        public double Thickness
        {
            get => 
                this.thickness;
            set => 
                (this.thickness = value);
        }
    }
}

