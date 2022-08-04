namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class Circle : EntityObject
    {
        private Vector3 center;
        private double radius;
        private double thickness;

        public Circle() : this(Vector3.Zero, 1.0)
        {
        }

        public Circle(Vector2 center, double radius) : this(new Vector3(center.X, center.Y, 0.0), radius)
        {
        }

        public Circle(Vector3 center, double radius) : base(EntityType.Circle, "CIRCLE")
        {
            this.center = center;
            this.radius = radius;
            this.thickness = 0.0;
        }

        public override object Clone()
        {
            Circle circle = new Circle {
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
                Thickness = this.thickness
            };
            foreach (XData data in base.XData.Values)
            {
                circle.XData.Add((XData) data.Clone());
            }
            return circle;
        }

        public List<Vector2> PolygonalVertexes(int precision)
        {
            if (precision < 3)
            {
                throw new ArgumentOutOfRangeException("precision", precision, "The circle precision must be greater or equal to three");
            }
            List<Vector2> list = new List<Vector2>();
            double num = 6.2831853071795862 / ((double) precision);
            for (int i = 0; i < precision; i++)
            {
                double a = num * i;
                double y = this.radius * Math.Sin(a);
                double x = this.radius * Math.Cos(a);
                list.Add(new Vector2(x, y));
            }
            return list;
        }

        public LwPolyline ToPolyline(int precision)
        {
            IEnumerable<Vector2> enumerable = this.PolygonalVertexes(precision);
            Vector3 vector = MathHelper.Transform(this.Center, base.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            LwPolyline polyline = new LwPolyline {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                Elevation = vector.Z,
                Thickness = this.thickness,
                IsClosed = true
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
                    throw new ArgumentOutOfRangeException("value", value, "The circle radius must be greater than zero.");
                }
                this.radius = value;
            }
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

