namespace netDxf.Tables
{
    using netDxf;
    using netDxf.Collections;
    using System;

    public class UCS : TableObject
    {
        private Vector3 origin;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        private double elevation;

        public UCS(string name) : this(name, true)
        {
        }

        internal UCS(string name, bool checkName) : base(name, "UCS", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The UCS name should be at least one character long.");
            }
            this.origin = Vector3.Zero;
            this.xAxis = Vector3.UnitX;
            this.yAxis = Vector3.UnitY;
            this.zAxis = Vector3.UnitZ;
            this.elevation = 0.0;
        }

        public UCS(string name, Vector3 origin, Vector3 xDirection, Vector3 yDirection) : this(name, origin, xDirection, yDirection, true)
        {
        }

        internal UCS(string name, Vector3 origin, Vector3 xDirection, Vector3 yDirection, bool checkName) : base(name, "UCS", checkName)
        {
            if (!Vector3.ArePerpendicular(xDirection, yDirection))
            {
                throw new ArgumentException("X-axis direction and Y-axis direction must be perpendicular.");
            }
            this.origin = origin;
            this.xAxis = xDirection;
            this.xAxis.Normalize();
            this.yAxis = yDirection;
            this.yAxis.Normalize();
            this.zAxis = Vector3.CrossProduct(this.xAxis, this.yAxis);
            this.elevation = 0.0;
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new UCS(newName) { 
                Origin = this.origin,
                xAxis = this.xAxis,
                yAxis = this.yAxis,
                zAxis = this.zAxis,
                Elevation = this.elevation
            };

        public static UCS FromNormal(string name, Vector3 origin, Vector3 normal, double rotation)
        {
            Matrix3 matrix = MathHelper.ArbitraryAxis(normal);
            Matrix3 matrix2 = Matrix3.RotationZ(rotation);
            matrix *= matrix2;
            return new UCS(name) { 
                origin = origin,
                xAxis = new Vector3(matrix.M11, matrix.M21, matrix.M31),
                yAxis = new Vector3(matrix.M12, matrix.M22, matrix.M32),
                zAxis = new Vector3(matrix.M13, matrix.M23, matrix.M33)
            };
        }

        public static UCS FromXAxisAndPointOnXYplane(string name, Vector3 origin, Vector3 xDirection, Vector3 pointOnPlaneXY)
        {
            UCS ucs = new UCS(name) {
                origin = origin,
                xAxis = xDirection
            };
            ucs.xAxis.Normalize();
            ucs.zAxis = Vector3.CrossProduct(xDirection, pointOnPlaneXY);
            ucs.zAxis.Normalize();
            ucs.yAxis = Vector3.CrossProduct(ucs.zAxis, ucs.xAxis);
            return ucs;
        }

        public void SetAxis(Vector3 xDirection, Vector3 yDirection)
        {
            if (!Vector3.ArePerpendicular(xDirection, yDirection))
            {
                throw new ArgumentException("X-axis direction and Y-axis direction must be perpendicular.");
            }
            this.xAxis = xDirection;
            this.xAxis.Normalize();
            this.yAxis = yDirection;
            this.yAxis.Normalize();
            this.zAxis = Vector3.CrossProduct(this.xAxis, this.yAxis);
        }

        public Vector3 Origin
        {
            get => 
                this.origin;
            set => 
                (this.origin = value);
        }

        public Vector3 XAxis =>
            this.xAxis;

        public Vector3 YAxis =>
            this.yAxis;

        public Vector3 ZAxis =>
            this.zAxis;

        public double Elevation
        {
            get => 
                this.elevation;
            set => 
                (this.elevation = value);
        }

        public UCSs Owner
        {
            get => 
                ((UCSs) base.Owner);
            internal set => 
                (base.Owner = value);
        }
    }
}

