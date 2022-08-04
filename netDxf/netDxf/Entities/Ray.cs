namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;

    public class Ray : EntityObject
    {
        private Vector3 origin;
        private Vector3 direction;

        public Ray() : this(Vector3.Zero, Vector3.UnitX)
        {
        }

        public Ray(Vector2 origin, Vector2 direction) : this(new Vector3(origin.X, origin.Y, 0.0), new Vector3(direction.X, direction.Y, 0.0))
        {
        }

        public Ray(Vector3 origin, Vector3 direction) : base(EntityType.Ray, "RAY")
        {
            this.origin = origin;
            this.direction = direction;
            this.direction.Normalize();
        }

        public override object Clone()
        {
            Ray ray = new Ray {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Origin = this.origin,
                Direction = this.direction
            };
            foreach (XData data in base.XData.Values)
            {
                ray.XData.Add((XData) data.Clone());
            }
            return ray;
        }

        public Vector3 Origin
        {
            get => 
                this.origin;
            set => 
                (this.origin = value);
        }

        public Vector3 Direction
        {
            get => 
                this.direction;
            set
            {
                this.direction = Vector3.Normalize(value);
                if (Vector3.IsNaN(this.direction))
                {
                    throw new ArgumentException("The direction can not be the zero vector.", "value");
                }
            }
        }
    }
}

