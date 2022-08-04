namespace netDxf.Entities
{
    using netDxf;
    using System;
    using System.Collections.Generic;

    public class PolyfaceMeshFace : DxfObject, ICloneable
    {
        private readonly VertexTypeFlags flags;
        private readonly List<short> vertexIndexes;

        public PolyfaceMeshFace() : this(new short[4])
        {
        }

        public PolyfaceMeshFace(IEnumerable<short> vertexIndexes) : base("VERTEX")
        {
            if (vertexIndexes == null)
            {
                throw new ArgumentNullException("vertexIndexes");
            }
            this.flags = VertexTypeFlags.PolyfaceMeshVertex;
            this.vertexIndexes = new List<short>(vertexIndexes);
            if (this.vertexIndexes.Count > 4)
            {
                throw new ArgumentOutOfRangeException("vertexIndexes", this.vertexIndexes.Count, "The maximum number of vertexes per face is 4");
            }
        }

        public object Clone() => 
            new PolyfaceMeshFace(this.vertexIndexes);

        public override string ToString() => 
            "PolyfaceMeshFace";

        public List<short> VertexIndexes =>
            this.vertexIndexes;

        internal VertexTypeFlags Flags =>
            this.flags;
    }
}

