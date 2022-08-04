namespace netDxf
{
    using System;
    using System.Collections.Generic;

    public class BoundingRectangle
    {
        private Vector2 min;
        private Vector2 max;

        public BoundingRectangle(IEnumerable<Vector2> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            double maxValue = double.MaxValue;
            double y = double.MaxValue;
            double minValue = double.MinValue;
            double num4 = double.MinValue;
            bool flag = false;
            foreach (Vector2 vector in points)
            {
                flag = true;
                if (maxValue > vector.X)
                {
                    maxValue = vector.X;
                }
                if (y > vector.Y)
                {
                    y = vector.Y;
                }
                if (minValue < vector.X)
                {
                    minValue = vector.X;
                }
                if (num4 < vector.Y)
                {
                    num4 = vector.Y;
                }
            }
            if (flag)
            {
                this.min = new Vector2(maxValue, y);
                this.max = new Vector2(minValue, num4);
            }
            else
            {
                this.min = new Vector2(double.MinValue, double.MinValue);
                this.max = new Vector2(double.MaxValue, double.MaxValue);
            }
        }

        public BoundingRectangle(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public BoundingRectangle(Vector2 center, double radius)
        {
            this.min = new Vector2(center.X - radius, center.Y - radius);
            this.max = new Vector2(center.X + radius, center.Y + radius);
        }

        public BoundingRectangle(Vector2 center, double width, double height)
        {
            this.min = new Vector2(center.X - (width * 0.5), center.Y - (height * 0.5));
            this.max = new Vector2(center.X + (width * 0.5), center.Y + (height * 0.5));
        }

        public BoundingRectangle(Vector2 center, double majorAxis, double minorAxis, double rotation)
        {
            double d = rotation * 0.017453292519943295;
            double num2 = (majorAxis * 0.5) * Math.Cos(d);
            double num3 = (minorAxis * 0.5) * Math.Sin(d);
            double num4 = (majorAxis * 0.5) * Math.Sin(d);
            double num5 = (minorAxis * 0.5) * Math.Cos(d);
            double num6 = Math.Sqrt((num2 * num2) + (num3 * num3)) * 2.0;
            double num7 = Math.Sqrt((num4 * num4) + (num5 * num5)) * 2.0;
            this.min = new Vector2(center.X - (num6 * 0.5), center.Y - (num7 * 0.5));
            this.max = new Vector2(center.X + (num6 * 0.5), center.Y + (num7 * 0.5));
        }

        public bool PointInside(Vector2 point) => 
            ((((point.X >= this.min.X) && (point.X <= this.max.X)) && (point.Y >= this.min.Y)) && (point.Y <= this.max.Y));

        public static BoundingRectangle Union(IEnumerable<BoundingRectangle> rectangles)
        {
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            BoundingRectangle rectangle = null;
            foreach (BoundingRectangle rectangle2 in rectangles)
            {
                rectangle = Union(rectangle, rectangle2);
            }
            return rectangle;
        }

        public static BoundingRectangle Union(BoundingRectangle aabr1, BoundingRectangle aabr2)
        {
            if ((aabr1 == null) && (aabr2 == null))
            {
                return null;
            }
            if (aabr1 == null)
            {
                return aabr2;
            }
            if (aabr2 == null)
            {
                return aabr1;
            }
            Vector2 min = new Vector2();
            Vector2 max = new Vector2();
            for (int i = 0; i < 2; i++)
            {
                if (aabr1.Min[i] <= aabr2.Min[i])
                {
                    min[i] = aabr1.Min[i];
                }
                else
                {
                    min[i] = aabr2.Min[i];
                }
                if (aabr1.Max[i] >= aabr2.Max[i])
                {
                    max[i] = aabr1.Max[i];
                }
                else
                {
                    max[i] = aabr2.Max[i];
                }
            }
            return new BoundingRectangle(min, max);
        }

        public Vector2 Min
        {
            get => 
                this.min;
            set => 
                (this.min = value);
        }

        public Vector2 Max
        {
            get => 
                this.max;
            set => 
                (this.max = value);
        }

        public Vector2 Center =>
            ((this.min + this.max) * 0.5);

        public double Radius =>
            (Vector2.Distance(this.min, this.max) * 0.5);

        public double Width =>
            (this.max.X - this.min.X);

        public double Height =>
            (this.max.Y - this.min.Y);
    }
}

