namespace netDxf
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3 : IEquatable<Vector3>
    {
        private double x;
        private double y;
        private double z;
        public Vector3(double value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
        }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(double[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Length != 3)
            {
                throw new ArgumentOutOfRangeException("array", array.Length, "The dimension of the array must be three.");
            }
            this.x = array[0];
            this.y = array[1];
            this.z = array[2];
        }

        public static Vector3 Zero =>
            new Vector3(0.0, 0.0, 0.0);
        public static Vector3 UnitX =>
            new Vector3(1.0, 0.0, 0.0);
        public static Vector3 UnitY =>
            new Vector3(0.0, 1.0, 0.0);
        public static Vector3 UnitZ =>
            new Vector3(0.0, 0.0, 1.0);
        public static Vector3 NaN =>
            new Vector3(double.NaN, double.NaN, double.NaN);
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
        public double Z
        {
            get => 
                this.z;
            set => 
                (this.z = value);
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

                    case 2:
                        return this.z;
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

                    case 2:
                        this.z = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("index");
                }
            }
        }
        public static bool IsNaN(Vector3 u) => 
            ((double.IsNaN(u.X) || double.IsNaN(u.Y)) || double.IsNaN(u.Z));

        public static double DotProduct(Vector3 u, Vector3 v) => 
            (((u.X * v.X) + (u.Y * v.Y)) + (u.Z * v.Z));

        public static Vector3 CrossProduct(Vector3 u, Vector3 v)
        {
            double x = (u.Y * v.Z) - (u.Z * v.Y);
            double y = (u.Z * v.X) - (u.X * v.Z);
            return new Vector3(x, y, (u.X * v.Y) - (u.Y * v.X));
        }

        public static double Distance(Vector3 u, Vector3 v) => 
            Math.Sqrt((((u.X - v.X) * (u.X - v.X)) + ((u.Y - v.Y) * (u.Y - v.Y))) + ((u.Z - v.Z) * (u.Z - v.Z)));

        public static double SquareDistance(Vector3 u, Vector3 v) => 
            ((((u.X - v.X) * (u.X - v.X)) + ((u.Y - v.Y) * (u.Y - v.Y))) + ((u.Z - v.Z) * (u.Z - v.Z)));

        public static double AngleBetween(Vector3 u, Vector3 v)
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

        public static Vector3 MidPoint(Vector3 u, Vector3 v) => 
            new Vector3((v.X + u.X) * 0.5, (v.Y + u.Y) * 0.5, (v.Z + u.Z) * 0.5);

        public static bool ArePerpendicular(Vector3 u, Vector3 v) => 
            ArePerpendicular(u, v, MathHelper.Epsilon);

        public static bool ArePerpendicular(Vector3 u, Vector3 v, double threshold) => 
            MathHelper.IsZero(DotProduct(u, v), threshold);

        public static bool AreParallel(Vector3 u, Vector3 v) => 
            AreParallel(u, v, MathHelper.Epsilon);

        public static bool AreParallel(Vector3 u, Vector3 v, double threshold)
        {
            Vector3 vector = CrossProduct(u, v);
            if (!MathHelper.IsZero(vector.X, threshold))
            {
                return false;
            }
            if (!MathHelper.IsZero(vector.Y, threshold))
            {
                return false;
            }
            if (!MathHelper.IsZero(vector.Z, threshold))
            {
                return false;
            }
            return true;
        }

        public static Vector3 Round(Vector3 u, int numDigits) => 
            new Vector3(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits), Math.Round(u.Z, numDigits));

        public static Vector3 Normalize(Vector3 u)
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
            return new Vector3(u.x * num2, u.y * num2, u.z * num2);
        }

        public static bool operator ==(Vector3 u, Vector3 v) => 
            Equals(u, v);

        public static bool operator !=(Vector3 u, Vector3 v) => 
            !Equals(u, v);

        public static Vector3 operator +(Vector3 u, Vector3 v) => 
            new Vector3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);

        public static Vector3 Add(Vector3 u, Vector3 v) => 
            new Vector3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);

        public static Vector3 operator -(Vector3 u, Vector3 v) => 
            new Vector3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);

        public static Vector3 Subtract(Vector3 u, Vector3 v) => 
            new Vector3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);

        public static Vector3 operator -(Vector3 u) => 
            new Vector3(-u.X, -u.Y, -u.Z);

        public static Vector3 Negate(Vector3 u) => 
            new Vector3(-u.X, -u.Y, -u.Z);

        public static Vector3 operator *(Vector3 u, double a) => 
            new Vector3(u.X * a, u.Y * a, u.Z * a);

        public static Vector3 Multiply(Vector3 u, double a) => 
            new Vector3(u.X * a, u.Y * a, u.Z * a);

        public static Vector3 operator *(double a, Vector3 u) => 
            new Vector3(u.X * a, u.Y * a, u.Z * a);

        public static Vector3 Multiply(double a, Vector3 u) => 
            new Vector3(u.X * a, u.Y * a, u.Z * a);

        public static Vector3 operator /(Vector3 u, double a)
        {
            double num = 1.0 / a;
            return new Vector3(u.X * num, u.Y * num, u.Z * num);
        }

        public static Vector3 Divide(Vector3 u, double a)
        {
            double num = 1.0 / a;
            return new Vector3(u.X * num, u.Y * num, u.Z * num);
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
                    this.z *= num2;
                }
            }
        }

        public double Modulus() => 
            Math.Sqrt(DotProduct(this, this));

        public double[] ToArray() => 
            new double[] { this.x, this.y, this.z };

        public static bool Equals(Vector3 a, Vector3 b) => 
            a.Equals(b, MathHelper.Epsilon);

        public static bool Equals(Vector3 a, Vector3 b, double threshold) => 
            a.Equals(b, threshold);

        public bool Equals(Vector3 other) => 
            this.Equals(other, MathHelper.Epsilon);

        public bool Equals(Vector3 other, double threshold) => 
            ((MathHelper.IsEqual(other.X, this.x, threshold) && MathHelper.IsEqual(other.Y, this.y, threshold)) && MathHelper.IsEqual(other.Z, this.z, threshold));

        public override bool Equals(object other) => 
            ((other is Vector3) && this.Equals((Vector3) other));

        public override int GetHashCode() => 
            ((this.X.GetHashCode() ^ this.Y.GetHashCode()) ^ this.Z.GetHashCode());

        public override string ToString() => 
            string.Format("{0}{3} {1}{3} {2}", new object[] { this.x, this.y, this.z, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator });

        public string ToString(IFormatProvider provider) => 
            string.Format("{0}{3} {1}{3} {2}", new object[] { this.x.ToString(provider), this.y.ToString(provider), this.z.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator });
    }
}

