namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class Mesh : EntityObject
    {
        private const int MaxFaces = 0xf42400;
        private readonly IReadOnlyList<Vector3> vertexes;
        private readonly IReadOnlyList<int[]> faces;
        private readonly IReadOnlyList<MeshEdge> edges;
        private byte subdivisionLevel;

        public Mesh(IEnumerable<Vector3> vertexes, IEnumerable<int[]> faces) : this(vertexes, faces, null)
        {
        }

        public Mesh(IEnumerable<Vector3> vertexes, IEnumerable<int[]> faces, IEnumerable<MeshEdge> edges) : base(EntityType.Mesh, "MESH")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<Vector3>(vertexes);
            if (faces == null)
            {
                throw new ArgumentNullException("faces");
            }
            this.faces = new List<int[]>(faces);
            if (this.faces.Count > 0xf42400)
            {
                throw new ArgumentOutOfRangeException("faces", this.faces.Count, $"The maximum number of faces in a mesh is {0xf42400}");
            }
            this.edges = (edges == null) ? new List<MeshEdge>() : new List<MeshEdge>(edges);
            this.subdivisionLevel = 0;
        }

        public override object Clone()
        {
            List<Vector3> vertexes = new List<Vector3>(this.vertexes.Count);
            List<int[]> faces = new List<int[]>(this.faces.Count);
            List<MeshEdge> edges = null;
            vertexes.AddRange(this.vertexes);
            foreach (int[] numArray in this.faces)
            {
                int[] array = new int[numArray.Length];
                numArray.CopyTo(array, 0);
                faces.Add(array);
            }
            if (this.edges > null)
            {
                edges = new List<MeshEdge>(this.edges.Count);
                foreach (MeshEdge edge in this.edges)
                {
                    edges.Add((MeshEdge) edge.Clone());
                }
            }
            Mesh mesh = new Mesh(vertexes, faces, edges) {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                SubdivisionLevel = this.subdivisionLevel
            };
            foreach (XData data in base.XData.Values)
            {
                mesh.XData.Add((XData) data.Clone());
            }
            return mesh;
        }

        public IReadOnlyList<Vector3> Vertexes =>
            this.vertexes;

        public IReadOnlyList<int[]> Faces =>
            this.faces;

        public IReadOnlyList<MeshEdge> Edges =>
            this.edges;

        public byte SubdivisionLevel
        {
            get => 
                this.subdivisionLevel;
            set => 
                (this.subdivisionLevel = value);
        }
    }
}

