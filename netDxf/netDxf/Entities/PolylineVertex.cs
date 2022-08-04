namespace netDxf.Entities
{
    using netDxf;
    using System;

    public class PolylineVertex : DxfObject, ICloneable
    {
        private VertexTypeFlags flags;
        private Vector3 position;

        public PolylineVertex() : this(Vector3.Zero)
        {
        }

        public PolylineVertex(Vector3 position) : base("VERTEX")
        {
            this.flags = VertexTypeFlags.Polyline3dVertex;
            this.position = position;
        }

        public PolylineVertex(double x, double y, double z) : this(new Vector3(x, y, z))
        {
        }

        public object Clone() => 
            new PolylineVertex(this.position);

        public override string ToString() => 
            $"{"PolylineVertex"}: ({this.position})";

        public Vector3 Position
        {
            get => 
                this.position;
            set => 
                (this.position = value);
        }

        internal VertexTypeFlags Flags
        {
            get => 
                this.flags;
            set => 
                (this.flags = value);
        }
    }
}

