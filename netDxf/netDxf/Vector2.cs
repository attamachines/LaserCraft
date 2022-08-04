namespace netDxf
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2 : IEquatable<Vector2>
    {
        private double x;
        private double y;
        public Vector2(double value)
        {
            this.x = value;
            this.y = value;
        }

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(double[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Length != 2)
            {
                throw new ArgumentOutOfRangeException("array", array.Length, "The dimension of the array must be two");
            }
            this.x = array[0];
            this.y = array[1];
        }

        public static Vector2 Zero =>
            new Vector2(0.0, 0.0);
        public static Vector2 UnitX =>
            new Vector2(1.0, 0.0);
        public static Vector2 UnitY =>
            new Vector2(0.0, 1.0);
        public static Vector2 NaN =>
            new Vector2(double.NaN, double.NaN);
        public double X
        {
            get => 
                this.x;
            set => 
                (this.x = value);
        }
        public double Y
        {
            get => 
                this.y;
            set => 
                (this.y = value);
        }
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;
                }
                throw new ArgumentOutOfRangeException("index");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("index");
                }
            }
        }
        public static bool IsNaN(Vector2 u) => 
            (double.IsNaN(u.X) || double.IsNaN(u.Y));

        public static double DotProduct(Vector2 u, Vector2 v) => 
            ((u.X * v.X) + (u.Y * v.Y));

        public static double CrossProduct(Vector2 u, Vector2 v) => 
            ((u.X * v.Y) - (u.Y * v.X));

        public static Vector2 Perpendicular(Vector2 u) => 
            new Vector2(-u.Y, u.X);

        public static Vector2 Polar(Vector2 u, double distance, double angle)
        {
            Vector2 vector = new Vector2(Math.Cos(angle), Math.Sin(angle));
            return (u + (vector * distance));
        }

        public static double Distance(Vector2 u, Vector2 v) => 
            Math.Sqrt(((u.X - v.X) * (u.X - v.X)) + ((u.Y - v.Y) * (u.Y - v.Y)));

        public static double SquareDistance(Vector2 u, Vector2 v) => 
            (((u.X - v.X) * (u.X - v.X)) + ((u.Y - v.Y) * (u.Y - v.Y)));

        public static double Angle(Vector2 u)
        {
            double num = Math.Atan2(u.Y, u.X);
            if (num < 0.0)
            {
                return (6.2831853071795862 + num);
            }
            return num;
        }

        public static double Angle(Vector2 u, Vector2 v)
        {
            Vector2 vector = v - u;
            return Angle(vector);
        }

        public static double AngleBetween(Vector2 u, Vector2 v)
        {
            double d = DotProduct(u, v) / (u.Modulus() * v.Modulus());
            if (d >= 1.0)
            {
                return 0.0;
            }
            if (d <= -1.0)
            {
                return 3.1415926535897931;
            }
            return Math.Acos(d);
        }

        public static Vector2 MidPoint(Vector2 u, Vector2 v) => 
            new Vector2((v.X + u.X) * 0.5, (v.Y + u.Y) * 0.5);

        public static bool ArePerpendicular(Vector2 u, Vector2 v) => 
            ArePerpendicular(u, v, MathHelper.Epsilon);

        public static bool ArePerpendicular(Vector2 u, Vector2 v, double threshold) => 
            MathHelper.IsZero(DotProduct(u, v), threshold);

        public static bool AreParallel(Vector2 u, Vector2 v) => 
            AreParallel(u, v, MathHelper.Epsilon);

        public static bool AreParallel(Vector2 u, Vector2 v, double threshold) => 
            MathHelper.IsZero(CrossProduct(u, v), threshold);

        public static Vector2 Round(Vector2 u, int numDigits) => 
            new Vector2(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits));

        public static Vector2 Normalize(Vector2 u)
        {
            double number = u.Modulus();
            if (MathHelper.IsOne(number))
            {
                return u;
            }
            if (MathHelper.IsZero(number))
            {
                return NaN;
            }
            double num2 = 1.0 / number;
            return new Vector2(u.x * num2, u.y * num2);
        }

        public static bool operator ==(Vector2 u, Vector2 v) => 
            Equals(u, v);

        public static bool operator !=(Vector2 u, Vector2 v) => 
            !Equals(u, v);

        public static Vector2 operator +(Vector2 u, Vector2 v) => 
            new Vector2(u.X + v.X, u.Y + v.Y);

        public static Vector2 Add(Vector2 u, Vector2 v) => 
            new Vector2(u.X + v.X, u.Y + v.Y);

        public static Vector2 operator -(Vector2 u, Vector2 v) => 
            new Vector2(u.X - v.X, u.Y - v.Y);

        public static Vector2 Subtract(Vector2 u, Vector2 v) => 
            new Vector2(u.X - v.X, u.Y - v.Y);

        public static Vector2 operator -(Vector2 u) => 
            new Vector2(-u.X, -u.Y);

        public static Vector2 Negate(Vector2 u) => 
            new Vector2(-u.X, -u.Y);

        public static Vector2 operator *(Vector2 u, double a) => 
            new Vector2(u.X * a, u.Y * a);

        public static Vector2 Multiply(Vector2 u, double a) => 
            new Vector2(u.X * a, u.Y * a);

        public static Vector2 operator *(double a, Vector2 u) => 
            new Vector2(u.X * a, u.Y * a);

        public static Vector2 Multiply(double a, Vector2 u) => 
            new Vector2(u.X * a, u.Y * a);

        public static Vector2 operator /(Vector2 u, double a)
        {
            double num = 1.0 / a;
            return new Vector2(u.X * num, u.Y * num);
        }

        public static Vector2 Divide(Vector2 u, double a)
        {
            double num = 1.0 / a;
            return new Vector2(u.X * num, u.Y * num);
        }

        public void Normalize()
        {
            double number = this.Modulus();
            if (!MathHelper.IsOne(number))
            {
                if (MathHelper.IsZero(number))
                {
                    this = NaN;
                }
                else
                {
                    double num2 = 1.0 / number;
                    this.x *= num2;
                    this.y *= num2;
                }
            }
        }

        public double Modulus() => 
            Math.Sqrt(DotProduct(this, this));

        public double[] ToArray() => 
            new double[] { this.x, this.y };

        public static bool Equals(Vector2 a, Vector2 b) => 
            a.Equals(b, MathHelper.Epsilon);

        public static bool Equals(Vector2 a, Vector2 b, double threshold) => 
            a.Equals(b, threshold);

        public bool Equals(Vector2 other) => 
            this.Equals(other, MathHelper.Epsilon);

        public bool Equals(Vector2 other, double threshold) => 
            (MathHelper.IsEqual(other.X, this.x, threshold) && MathHelper.IsEqual(other.Y, this.y, threshold));

        public override bool Equals(object other) => 
            ((other is Vector2) && this.Equals((Vector2) other));

        public override int GetHashCode() => 
            (this.X.GetHashCode() ^ this.Y.GetHashCode());

        public override string ToString() => 
            string.Format("{0}{2} {1}", this.x, this.y, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);

        public string ToString(IFormatProvider provider) => 
            string.Format("{0}{2} {1}", this.x.ToString(provider), this.y.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
    }
}

