namespace netDxf
{
    using System;
    using System.Collections.Generic;

    public static class MathHelper
    {
        public const double DegToRad = 0.017453292519943295;
        public const double RadToDeg = 57.295779513082323;
        public const double DegToGrad = 1.1111111111111112;
        public const double GradToDeg = 0.9;
        public const double HalfPI = 1.5707963267948966;
        public const double PI = 3.1415926535897931;
        public const double ThreeHalfPI = 4.71238898038469;
        public const double TwoPI = 6.2831853071795862;
        private static double epsilon = 1E-12;

        public static Matrix3 ArbitraryAxis(Vector3 zAxis)
        {
            Vector3 vector3;
            zAxis.Normalize();
            Vector3 unitY = Vector3.UnitY;
            Vector3 unitZ = Vector3.UnitZ;
            if ((Math.Abs(zAxis.X) < 0.015625) && (Math.Abs(zAxis.Y) < 0.015625))
            {
                vector3 = Vector3.CrossProduct(unitY, zAxis);
            }
            else
            {
                vector3 = Vector3.CrossProduct(unitZ, zAxis);
            }
            vector3.Normalize();
            Vector3 vector4 = Vector3.CrossProduct(zAxis, vector3);
            vector4.Normalize();
            return new Matrix3(vector3.X, vector4.X, zAxis.X, vector3.Y, vector4.Y, zAxis.Y, vector3.Z, vector4.Z, zAxis.Z);
        }

        public static Vector2 FindIntersection(Vector2 point0, Vector2 dir0, Vector2 point1, Vector2 dir1) => 
            FindIntersection(point0, dir0, point1, dir1, Epsilon);

        public static Vector2 FindIntersection(Vector2 point0, Vector2 dir0, Vector2 point1, Vector2 dir1, double threshold)
        {
            if (Vector2.AreParallel(dir0, dir1, threshold))
            {
                return new Vector2(double.NaN, double.NaN);
            }
            Vector2 vector = point1 - point0;
            double num = Vector2.CrossProduct(dir0, dir1);
            double num2 = ((vector.X * dir1.Y) - (vector.Y * dir1.X)) / num;
            return (point0 + (num2 * dir0));
        }

        public static bool IsEqual(double a, double b) => 
            IsEqual(a, b, Epsilon);

        public static bool IsEqual(double a, double b, double threshold) => 
            IsZero(a - b, threshold);

        public static bool IsOne(double number) => 
            IsOne(number, Epsilon);

        public static bool IsOne(double number, double threshold) => 
            IsZero(number - 1.0, threshold);

        public static bool IsZero(double number) => 
            IsZero(number, Epsilon);

        public static bool IsZero(double number, double threshold) => 
            ((number >= -threshold) && (number <= threshold));

        public static double NormalizeAngle(double angle)
        {
            double num = angle % 360.0;
            if (num < 0.0)
            {
                return (360.0 + num);
            }
            return num;
        }

        public static int PointInSegment(Vector2 p, Vector2 start, Vector2 end)
        {
            Vector2 u = end - start;
            Vector2 v = p - start;
            double num = Vector2.DotProduct(u, v);
            if (num <= 0.0)
            {
                return -1;
            }
            double num2 = Vector2.DotProduct(u, u);
            if (num >= num2)
            {
                return 1;
            }
            return 0;
        }

        public static int PointInSegment(Vector3 p, Vector3 start, Vector3 end)
        {
            Vector3 u = end - start;
            Vector3 v = p - start;
            double num = Vector3.DotProduct(u, v);
            if (num <= 0.0)
            {
                return -1;
            }
            double num2 = Vector3.DotProduct(u, u);
            if (num >= num2)
            {
                return 1;
            }
            return 0;
        }

        public static double PointLineDistance(Vector2 p, Vector2 origin, Vector2 dir)
        {
            double num = Vector2.DotProduct(dir, p - origin);
            Vector2 vector = origin + (num * dir);
            Vector2 u = p - vector;
            return Math.Sqrt(Vector2.DotProduct(u, u));
        }

        public static double PointLineDistance(Vector3 p, Vector3 origin, Vector3 dir)
        {
            double num = Vector3.DotProduct(dir, p - origin);
            Vector3 vector = origin + (num * dir);
            Vector3 u = p - vector;
            return Math.Sqrt(Vector3.DotProduct(u, u));
        }

        public static Vector3 RotateAboutAxis(Vector3 v, Vector3 axis, double angle)
        {
            Vector3 vector = new Vector3();
            double num = Math.Cos(angle);
            double num2 = Math.Sin(angle);
            vector.X += (num + (((1.0 - num) * axis.X) * axis.X)) * v.X;
            vector.X += ((((1.0 - num) * axis.X) * axis.Y) - (axis.Z * num2)) * v.Y;
            vector.X += ((((1.0 - num) * axis.X) * axis.Z) + (axis.Y * num2)) * v.Z;
            vector.Y += ((((1.0 - num) * axis.X) * axis.Y) + (axis.Z * num2)) * v.X;
            vector.Y += (num + (((1.0 - num) * axis.Y) * axis.Y)) * v.Y;
            vector.Y += ((((1.0 - num) * axis.Y) * axis.Z) - (axis.X * num2)) * v.Z;
            vector.Z += ((((1.0 - num) * axis.X) * axis.Z) - (axis.Y * num2)) * v.X;
            vector.Z += ((((1.0 - num) * axis.Y) * axis.Z) + (axis.X * num2)) * v.Y;
            vector.Z += (num + (((1.0 - num) * axis.Z) * axis.Z)) * v.Z;
            return vector;
        }

        public static double RoundToNearest(double number, double roundTo) => 
            (Convert.ToInt32((double) (number / roundTo)) * roundTo);

        public static Vector2 Transform(Vector2 point, double rotation, CoordinateSystem from, CoordinateSystem to)
        {
            if (!IsZero(rotation))
            {
                double num = Math.Sin(rotation);
                double num2 = Math.Cos(rotation);
                if ((from == CoordinateSystem.World) && (to == CoordinateSystem.Object))
                {
                    return new Vector2((point.X * num2) + (point.Y * num), (-point.X * num) + (point.Y * num2));
                }
                if ((from == CoordinateSystem.Object) && (to == CoordinateSystem.World))
                {
                    return new Vector2((point.X * num2) - (point.Y * num), (point.X * num) + (point.Y * num2));
                }
            }
            return point;
        }

        public static Vector3 Transform(Vector3 point, Vector3 zAxis, CoordinateSystem from, CoordinateSystem to)
        {
            if (!zAxis.Equals(Vector3.UnitZ))
            {
                Matrix3 matrix = ArbitraryAxis(zAxis);
                if ((from == CoordinateSystem.World) && (to == CoordinateSystem.Object))
                {
                    return (Vector3) (matrix.Transpose() * point);
                }
                if ((from == CoordinateSystem.Object) && (to == CoordinateSystem.World))
                {
                    return (Vector3) (matrix * point);
                }
            }
            return point;
        }

        public static IList<Vector2> Transform(IEnumerable<Vector2> points, double rotation, CoordinateSystem from, CoordinateSystem to)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (!IsZero(rotation))
            {
                List<Vector2> list;
                double num = Math.Sin(rotation);
                double num2 = Math.Cos(rotation);
                if ((from == CoordinateSystem.World) && (to == CoordinateSystem.Object))
                {
                    list = new List<Vector2>();
                    foreach (Vector2 vector in points)
                    {
                        list.Add(new Vector2((vector.X * num2) + (vector.Y * num), (-vector.X * num) + (vector.Y * num2)));
                    }
                    return list;
                }
                if ((from == CoordinateSystem.Object) && (to == CoordinateSystem.World))
                {
                    list = new List<Vector2>();
                    foreach (Vector2 vector2 in points)
                    {
                        list.Add(new Vector2((vector2.X * num2) - (vector2.Y * num), (vector2.X * num) + (vector2.Y * num2)));
                    }
                    return list;
                }
            }
            return new List<Vector2>(points);
        }

        public static IList<Vector3> Transform(IEnumerable<Vector3> points, Vector3 zAxis, CoordinateSystem from, CoordinateSystem to)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (!zAxis.Equals(Vector3.UnitZ))
            {
                List<Vector3> list;
                Matrix3 matrix = ArbitraryAxis(zAxis);
                if ((from == CoordinateSystem.World) && (to == CoordinateSystem.Object))
                {
                    list = new List<Vector3>();
                    matrix = matrix.Transpose();
                    foreach (Vector3 vector in points)
                    {
                        list.Add((Vector3) (matrix * vector));
                    }
                    return list;
                }
                if ((from == CoordinateSystem.Object) && (to == CoordinateSystem.World))
                {
                    list = new List<Vector3>();
                    foreach (Vector3 vector2 in points)
                    {
                        list.Add((Vector3) (matrix * vector2));
                    }
                    return list;
                }
            }
            return new List<Vector3>(points);
        }

        public static double Epsilon
        {
            get => 
                epsilon;
            set => 
                (epsilon = value);
        }
    }
}

