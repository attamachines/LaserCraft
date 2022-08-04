namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;

    public class Point : EntityObject
    {
        private Vector3 position;
        private double thickness;
        private double rotation;

        public Point() : this(Vector3.Zero)
        {
        }

        public Point(Vector2 position) : this(new Vector3(position.X, position.Y, 0.0))
        {
        }

        public Point(Vector3 position) : base(EntityType.Point, "POINT")
        {
            this.position = position;
            this.thickness = 0.0;
            this.rotation = 0.0;
        }

        public Point(double x, double y, double z) : this(new Vector3(x, y, z))
        {
        }

        public override object Clone()
        {
            Point point = new Point {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Position = this.position,
                Rotation = this.rotation,
                Thickness = this.thickness
            };
            foreach (XData data in base.XData.Values)
            {
                point.XData.Add((XData) data.Clone());
            }
            return point;
        }

        public Vector3 Position
        {
            get => 
                this.position;
            set => 
                (this.position = value);
        }

        public double Thickness
        {
            get => 
                this.thickness;
            set => 
                (this.thickness = value);
        }

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = MathHelper.NormalizeAngle(value));
        }
    }
}

