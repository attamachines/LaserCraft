namespace netDxf.Entities
{
    using netDxf;
    using System;

    public class PolyfaceMeshVertex : DxfObject, ICloneable
    {
        private readonly VertexTypeFlags flags;
        private Vector3 location;

        public PolyfaceMeshVertex() : this(Vector3.Zero)
        {
        }

        public PolyfaceMeshVertex(Vector3 location) : base("VERTEX")
        {
            this.flags = VertexTypeFlags.PolyfaceMeshVertex | VertexTypeFlags.Polygon3dMesh;
            this.location = location;
        }

        public PolyfaceMeshVertex(double x, double y, double z) : this(new Vector3(x, y, z))
        {
        }

        public object Clone() => 
            new PolyfaceMeshVertex(this.location);

        public override string ToString() => 
            $"{"PolyfaceMeshVertex"}: {this.location}";

        public Vector3 Location
        {
            get => 
                this.location;
            set => 
                (this.location = value);
        }

        internal VertexTypeFlags Flags =>
            this.flags;
    }
}

