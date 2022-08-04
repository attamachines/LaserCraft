namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class Ellipse : EntityObject
    {
        private Vector3 center;
        private double majorAxis;
        private double minorAxis;
        private double rotation;
        private double startAngle;
        private double endAngle;
        private double thickness;

        public Ellipse() : this(Vector3.Zero, 1.0, 0.5)
        {
        }

        public Ellipse(Vector2 center, double majorAxis, double minorAxis) : this(new Vector3(center.X, center.Y, 0.0), majorAxis, minorAxis)
        {
        }

        public Ellipse(Vector3 center, double majorAxis, double minorAxis) : base(EntityType.Ellipse, "ELLIPSE")
        {
            this.center = center;
            this.majorAxis = majorAxis;
            this.minorAxis = minorAxis;
            this.startAngle = 0.0;
            this.endAngle = 0.0;
            this.rotation = 0.0;
            this.thickness = 0.0;
        }

        public override object Clone()
        {
            netDxf.Entities.Ellipse ellipse = new netDxf.Entities.Ellipse {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Center = this.center,
                MajorAxis = this.majorAxis,
                MinorAxis = this.minorAxis,
                Rotation = this.rotation,
                StartAngle = this.startAngle,
                EndAngle = this.endAngle,
                Thickness = this.thickness
            };
            foreach (XData data in base.XData.Values)
            {
                ellipse.XData.Add((XData) data.Clone());
            }
            return ellipse;
        }

        public Vector2 PolarCoordinateRelativeToCenter(double angle)
        {
            double num = this.MajorAxis * 0.5;
            double num2 = this.MinorAxis * 0.5;
            double a = angle * 0.017453292519943295;
            double num4 = num * Math.Sin(a);
            double num5 = num2 * Math.Cos(a);
            double num6 = (num * num2) / Math.Sqrt((num5 * num5) + (num4 * num4));
            return new Vector2(num6 * Math.Cos(a), num6 * Math.Sin(a));
        }

        public List<Vector2> PolygonalVertexes(int precision)
        {
            List<Vector2> list = new List<Vector2>();
            double a = this.rotation * 0.017453292519943295;
            double num2 = Math.Sin(a);
            double num3 = Math.Cos(a);
            if (this.IsFullEllipse)
            {
                double num4 = 6.2831853071795862 / ((double) precision);
                for (int j = 0; j < precision; j++)
                {
                    double num6 = num4 * j;
                    double num7 = Math.Sin(num6);
                    double num8 = Math.Cos(num6);
                    double x = 0.5 * (((this.majorAxis * num8) * num3) - ((this.minorAxis * num7) * num2));
                    double y = 0.5 * (((this.majorAxis * num8) * num2) + ((this.minorAxis * num7) * num3));
                    list.Add(new Vector2(x, y));
                }
                return list;
            }
            double startAngle = this.startAngle;
            double endAngle = this.endAngle;
            if (endAngle < startAngle)
            {
                endAngle += 360.0;
            }
            double num13 = (endAngle - startAngle) / ((double) precision);
            for (int i = 0; i <= precision; i++)
            {
                Vector2 vector = this.PolarCoordinateRelativeToCenter(startAngle + (num13 * i));
                double x = (vector.X * num3) - (vector.Y * num2);
                double y = (vector.X * num2) + (vector.Y * num3);
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
                IsClosed = this.IsFullEllipse
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

        public double MajorAxis
        {
            get => 
                this.majorAxis;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The major axis value must be greater than zero.");
                }
                this.majorAxis = value;
            }
        }

        public double MinorAxis
        {
            get => 
                this.minorAxis;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The minor axis value must be greater than zero.");
                }
                this.minorAxis = value;
            }
        }

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = MathHelper.NormalizeAngle(value));
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

        public bool IsFullEllipse =>
            MathHelper.IsEqual(this.startAngle, this.endAngle);
    }
}

