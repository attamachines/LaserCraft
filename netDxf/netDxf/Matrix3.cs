namespace netDxf
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3 : IEquatable<Matrix3>
    {
        private double m11;
        private double m12;
        private double m13;
        private double m21;
        private double m22;
        private double m23;
        private double m31;
        private double m32;
        private double m33;
        public Matrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        public Matrix3(double[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Length != 9)
            {
                throw new ArgumentException("The array must contain 9 elements.");
            }
            this.m11 = array[0];
            this.m12 = array[1];
            this.m13 = array[2];
            this.m21 = array[3];
            this.m22 = array[4];
            this.m23 = array[5];
            this.m31 = array[6];
            this.m32 = array[7];
            this.m33 = array[8];
        }

        public static Matrix3 Zero =>
            new Matrix3(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
        public static Matrix3 Identity =>
            new Matrix3(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0);
        public double M11
        {
            get => 
                this.m11;
            set => 
                (this.m11 = value);
        }
        public double M12
        {
            get => 
                this.m12;
            set => 
                (this.m12 = value);
        }
        public double M13
        {
            get => 
                this.m13;
            set => 
                (this.m13 = value);
        }
        public double M21
        {
            get => 
                this.m21;
            set => 
                (this.m21 = value);
        }
        public double M22
        {
            get => 
                this.m22;
            set => 
                (this.m22 = value);
        }
        public double M23
        {
            get => 
                this.m23;
            set => 
                (this.m23 = value);
        }
        public double M31
        {
            get => 
                this.m31;
            set => 
                (this.m31 = value);
        }
        public double M32
        {
            get => 
                this.m32;
            set => 
                (this.m32 = value);
        }
        public double M33
        {
            get => 
                this.m33;
            set => 
                (this.m33 = value);
        }
        public static Matrix3 operator +(Matrix3 a, Matrix3 b) => 
            new Matrix3(a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13, a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23, a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33);

        public static Matrix3 Add(Matrix3 a, Matrix3 b) => 
            new Matrix3(a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13, a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23, a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33);

        public static Matrix3 operator -(Matrix3 a, Matrix3 b) => 
            new Matrix3(a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13, a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23, a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33);

        public static Matrix3 Subtract(Matrix3 a, Matrix3 b) => 
            new Matrix3(a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13, a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23, a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33);

        public static Matrix3 operator *(Matrix3 a, Matrix3 b) => 
            new Matrix3(((a.M11 * b.M11) + (a.M12 * b.M21)) + (a.M13 * b.M31), ((a.M11 * b.M12) + (a.M12 * b.M22)) + (a.M13 * b.M32), ((a.M11 * b.M13) + (a.M12 * b.M23)) + (a.M13 * b.M33), ((a.M21 * b.M11) + (a.M22 * b.M21)) + (a.M23 * b.M31), ((a.M21 * b.M12) + (a.M22 * b.M22)) + (a.M23 * b.M32), ((a.M21 * b.M13) + (a.M22 * b.M23)) + (a.M23 * b.M33), ((a.M31 * b.M11) + (a.M32 * b.M21)) + (a.M33 * b.M31), ((a.M31 * b.M12) + (a.M32 * b.M22)) + (a.M33 * b.M32), ((a.M31 * b.M13) + (a.M32 * b.M23)) + (a.M33 * b.M33));

        public static Matrix3 Multiply(Matrix3 a, Matrix3 b) => 
            new Matrix3(((a.M11 * b.M11) + (a.M12 * b.M21)) + (a.M13 * b.M31), ((a.M11 * b.M12) + (a.M12 * b.M22)) + (a.M13 * b.M32), ((a.M11 * b.M13) + (a.M12 * b.M23)) + (a.M13 * b.M33), ((a.M21 * b.M11) + (a.M22 * b.M21)) + (a.M23 * b.M31), ((a.M21 * b.M12) + (a.M22 * b.M22)) + (a.M23 * b.M32), ((a.M21 * b.M13) + (a.M22 * b.M23)) + (a.M23 * b.M33), ((a.M31 * b.M11) + (a.M32 * b.M21)) + (a.M33 * b.M31), ((a.M31 * b.M12) + (a.M32 * b.M22)) + (a.M33 * b.M32), ((a.M31 * b.M13) + (a.M32 * b.M23)) + (a.M33 * b.M33));

        public static Vector3 operator *(Matrix3 a, Vector3 u) => 
            new Vector3(((a.M11 * u.X) + (a.M12 * u.Y)) + (a.M13 * u.Z), ((a.M21 * u.X) + (a.M22 * u.Y)) + (a.M23 * u.Z), ((a.M31 * u.X) + (a.M32 * u.Y)) + (a.M33 * u.Z));

        public static Vector3 Multiply(Matrix3 a, Vector3 u) => 
            new Vector3(((a.M11 * u.X) + (a.M12 * u.Y)) + (a.M13 * u.Z), ((a.M21 * u.X) + (a.M22 * u.Y)) + (a.M23 * u.Z), ((a.M31 * u.X) + (a.M32 * u.Y)) + (a.M33 * u.Z));

        public static Matrix3 operator *(Matrix3 m, double a) => 
            new Matrix3(m.M11 * a, m.M12 * a, m.M13 * a, m.M21 * a, m.M22 * a, m.M23 * a, m.M31 * a, m.M32 * a, m.M33 * a);

        public static Matrix3 Multiply(Matrix3 m, double a) => 
            new Matrix3(m.M11 * a, m.M12 * a, m.M13 * a, m.M21 * a, m.M22 * a, m.M23 * a, m.M31 * a, m.M32 * a, m.M33 * a);

        public static bool operator ==(Matrix3 u, Matrix3 v) => 
            Equals(u, v);

        public static bool operator !=(Matrix3 u, Matrix3 v) => 
            !Equals(u, v);

        public double Determinant() => 
            (((((((this.m11 * this.m22) * this.m33) + ((this.m12 * this.m23) * this.m31)) + ((this.m13 * this.m21) * this.m32)) - ((this.m13 * this.m22) * this.m31)) - ((this.m11 * this.m23) * this.m32)) - ((this.m12 * this.m21) * this.m33));

        public Matrix3 Inverse()
        {
            double number = this.Determinant();
            if (MathHelper.IsZero(number))
            {
                throw new ArithmeticException("The matrix is not invertible.");
            }
            number = 1.0 / number;
            return new Matrix3(number * ((this.m22 * this.m33) - (this.m23 * this.m32)), number * ((this.m13 * this.m32) - (this.m12 * this.m33)), number * ((this.m12 * this.m23) - (this.m13 * this.m22)), number * ((this.m23 * this.m31) - (this.m21 * this.m33)), number * ((this.m11 * this.m33) - (this.m13 * this.m31)), number * ((this.m13 * this.m21) - (this.m11 * this.m23)), number * ((this.m21 * this.m32) - (this.m22 * this.m31)), number * ((this.m12 * this.m31) - (this.m11 * this.m32)), number * ((this.m11 * this.m22) - (this.m12 * this.m21)));
        }

        public Matrix3 Transpose() => 
            new Matrix3(this.m11, this.m21, this.m31, this.m12, this.m22, this.m32, this.m13, this.m23, this.m33);

        public static Matrix3 RotationX(double angle)
        {
            double num = Math.Cos(angle);
            double num2 = Math.Sin(angle);
            return new Matrix3(1.0, 0.0, 0.0, 0.0, num, -num2, 0.0, num2, num);
        }

        public static Matrix3 RotationY(double angle)
        {
            double num = Math.Cos(angle);
            double num2 = Math.Sin(angle);
            return new Matrix3(num, 0.0, num2, 0.0, 1.0, 0.0, -num2, 0.0, num);
        }

        public static Matrix3 RotationZ(double angle)
        {
            double num = Math.Cos(angle);
            double num2 = Math.Sin(angle);
            return new Matrix3(num, -num2, 0.0, num2, num, 0.0, 0.0, 0.0, 1.0);
        }

        public static Matrix3 Scale(double value) => 
            Scale(value, value, value);

        public static Matrix3 Scale(Vector3 value) => 
            Scale(value.X, value.Y, value.Z);

        public static Matrix3 Scale(double x, double y, double z) => 
            new Matrix3(x, 0.0, 0.0, 0.0, y, 0.0, 0.0, 0.0, z);

        public static bool Equals(Matrix3 a, Matrix3 b) => 
            a.Equals(b, MathHelper.Epsilon);

        public static bool Equals(Matrix3 a, Matrix3 b, double threshold) => 
            a.Equals(b, threshold);

        public bool Equals(Matrix3 other) => 
            this.Equals(other, MathHelper.Epsilon);

        public bool Equals(Matrix3 obj, double threshold) => 
            ((((MathHelper.IsEqual(obj.M11, this.M11, threshold) && MathHelper.IsEqual(obj.M12, this.M12, threshold)) && (MathHelper.IsEqual(obj.M13, this.M13, threshold) && MathHelper.IsEqual(obj.M21, this.M21, threshold))) && ((MathHelper.IsEqual(obj.M22, this.M22, threshold) && MathHelper.IsEqual(obj.M23, this.M23, threshold)) && (MathHelper.IsEqual(obj.M31, this.M31, threshold) && MathHelper.IsEqual(obj.M32, this.M32, threshold)))) && MathHelper.IsEqual(obj.M33, this.M33, threshold));

        public override bool Equals(object obj) => 
            ((obj is Matrix3) && this.Equals((Matrix3) obj));

        public override int GetHashCode() => 
            ((((((((this.M11.GetHashCode() ^ this.M12.GetHashCode()) ^ this.M13.GetHashCode()) ^ this.M21.GetHashCode()) ^ this.M22.GetHashCode()) ^ this.M23.GetHashCode()) ^ this.M31.GetHashCode()) ^ this.M32.GetHashCode()) ^ this.M33.GetHashCode());

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, new object[] { this.m11, this.m12, this.m13, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator }));
            builder.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, new object[] { this.m21, this.m22, this.m23, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator }));
            builder.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, new object[] { this.m31, this.m32, this.m33, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator }));
            return builder.ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            string listSeparator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, new object[] { this.m11.ToString(provider), this.m12.ToString(provider), this.m13.ToString(provider), listSeparator }));
            builder.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, new object[] { this.m21.ToString(provider), this.m22.ToString(provider), this.m23.ToString(provider), listSeparator }));
            builder.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, new object[] { this.m31.ToString(provider), this.m32.ToString(provider), this.m33.ToString(provider), listSeparator }));
            return builder.ToString();
        }
    }
}

