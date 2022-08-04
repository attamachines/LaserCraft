namespace netDxf.Entities
{
    using netDxf;
    using System;

    public class LwPolylineVertex : ICloneable
    {
        private Vector2 position;
        private double startWidth;
        private double endWidth;
        private double bulge;

        public LwPolylineVertex() : this(Vector2.Zero)
        {
        }

        public LwPolylineVertex(Vector2 position) : this(position, 0.0)
        {
        }

        public LwPolylineVertex(Vector2 position, double bulge)
        {
            this.position = position;
            this.bulge = bulge;
            this.startWidth = 0.0;
            this.endWidth = 0.0;
        }

        public LwPolylineVertex(double x, double y) : this(new Vector2(x, y), 0.0)
        {
        }

        public LwPolylineVertex(double x, double y, double bulge) : this(new Vector2(x, y), bulge)
        {
        }

        public object Clone() => 
            new LwPolylineVertex { 
                Position = this.position,
                Bulge = this.bulge,
                StartWidth = this.startWidth,
                EndWidth = this.endWidth
            };

        public override string ToString() => 
            $"{"LwPolylineVertex"}: ({this.position})";

        public Vector2 Position
        {
            get => 
                this.position;
            set => 
                (this.position = value);
        }

        public double StartWidth
        {
            get => 
                this.startWidth;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The LwPolylineVertex width must be equals or greater than zero.");
                }
                this.startWidth = value;
            }
        }

        public double EndWidth
        {
            get => 
                this.endWidth;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The LwPolylineVertex width must be equals or greater than zero.");
                }
                this.endWidth = value;
            }
        }

        public double Bulge
        {
            get => 
                this.bulge;
            set => 
                (this.bulge = value);
        }
    }
}

