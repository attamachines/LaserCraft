namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class PolyfaceMesh : EntityObject
    {
        private readonly List<PolyfaceMeshFace> faces;
        private readonly List<PolyfaceMeshVertex> vertexes;
        private readonly PolylinetypeFlags flags;
        private readonly netDxf.Entities.EndSequence endSequence;

        public PolyfaceMesh(IEnumerable<PolyfaceMeshVertex> vertexes, IEnumerable<PolyfaceMeshFace> faces) : base(EntityType.PolyfaceMesh, "POLYLINE")
        {
            this.flags = PolylinetypeFlags.PolyfaceMesh;
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<PolyfaceMeshVertex>(vertexes);
            if (this.vertexes.Count < 3)
            {
                throw new ArgumentOutOfRangeException("vertexes", this.vertexes.Count, "The polyface mesh faces list requires at least three points.");
            }
            if (faces == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.faces = new List<PolyfaceMeshFace>(faces);
            if (this.faces.Count < 1)
            {
                throw new ArgumentOutOfRangeException("vertexes", this.faces.Count, "The polyface mesh faces list requires at least one face.");
            }
            this.endSequence = new netDxf.Entities.EndSequence(this);
        }

        internal override long AsignHandle(long entityNumber)
        {
            entityNumber = this.endSequence.AsignHandle(entityNumber);
            foreach (PolyfaceMeshVertex vertex in this.vertexes)
            {
                entityNumber = vertex.AsignHandle(entityNumber);
            }
            foreach (PolyfaceMeshFace face in this.faces)
            {
                entityNumber = face.AsignHandle(entityNumber);
            }
            return base.AsignHandle(entityNumber);
        }

        public override object Clone()
        {
            List<PolyfaceMeshVertex> vertexes = new List<PolyfaceMeshVertex>();
            foreach (PolyfaceMeshVertex vertex in this.vertexes)
            {
                vertexes.Add((PolyfaceMeshVertex) vertex.Clone());
            }
            List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>();
            foreach (PolyfaceMeshFace face in this.faces)
            {
                faces.Add((PolyfaceMeshFace) face.Clone());
            }
            PolyfaceMesh mesh = new PolyfaceMesh(vertexes, faces) {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible
            };
            foreach (XData data in base.XData.Values)
            {
                mesh.XData.Add((XData) data.Clone());
            }
            return mesh;
        }

        public List<EntityObject> Explode()
        {
            List<EntityObject> list = new List<EntityObject>();
            foreach (PolyfaceMeshFace face in this.Faces)
            {
                if (face.VertexIndexes.Count == 1)
                {
                    Point item = new Point {
                        Layer = (Layer) base.Layer.Clone(),
                        Linetype = (Linetype) base.Linetype.Clone(),
                        Color = (AciColor) base.Color.Clone(),
                        Lineweight = base.Lineweight,
                        Transparency = (Transparency) base.Transparency.Clone(),
                        LinetypeScale = base.LinetypeScale,
                        Normal = base.Normal,
                        Position = this.Vertexes[Math.Abs(face.VertexIndexes[0]) - 1].Location
                    };
                    list.Add(item);
                }
                else if (face.VertexIndexes.Count == 2)
                {
                    netDxf.Entities.Line item = new netDxf.Entities.Line {
                        Layer = (Layer) base.Layer.Clone(),
                        Linetype = (Linetype) base.Linetype.Clone(),
                        Color = (AciColor) base.Color.Clone(),
                        Lineweight = base.Lineweight,
                        Transparency = (Transparency) base.Transparency.Clone(),
                        LinetypeScale = base.LinetypeScale,
                        Normal = base.Normal,
                        StartPoint = this.Vertexes[Math.Abs(face.VertexIndexes[0]) - 1].Location,
                        EndPoint = this.Vertexes[Math.Abs(face.VertexIndexes[1]) - 1].Location
                    };
                    list.Add(item);
                }
                else
                {
                    Face3dEdgeFlags visibles = Face3dEdgeFlags.Visibles;
                    short num = face.VertexIndexes[0];
                    short num2 = face.VertexIndexes[1];
                    short num3 = face.VertexIndexes[2];
                    int num4 = (face.VertexIndexes.Count == 3) ? ((int) face.VertexIndexes[2]) : ((int) face.VertexIndexes[3]);
                    if (num < 0)
                    {
                        visibles |= Face3dEdgeFlags.First;
                    }
                    if (num2 < 0)
                    {
                        visibles |= Face3dEdgeFlags.Second;
                    }
                    if (num3 < 0)
                    {
                        visibles |= Face3dEdgeFlags.Third;
                    }
                    if (num4 < 0)
                    {
                        visibles |= Face3dEdgeFlags.Fourth;
                    }
                    Vector3 location = this.Vertexes[Math.Abs(num) - 1].Location;
                    Vector3 vector2 = this.Vertexes[Math.Abs(num2) - 1].Location;
                    Vector3 vector3 = this.Vertexes[Math.Abs(num3) - 1].Location;
                    Vector3 vector4 = this.Vertexes[Math.Abs(num4) - 1].Location;
                    Face3d item = new Face3d {
                        Layer = (Layer) base.Layer.Clone(),
                        Linetype = (Linetype) base.Linetype.Clone(),
                        Color = (AciColor) base.Color.Clone(),
                        Lineweight = base.Lineweight,
                        Transparency = (Transparency) base.Transparency.Clone(),
                        LinetypeScale = base.LinetypeScale,
                        Normal = base.Normal,
                        FirstVertex = location,
                        SecondVertex = vector2,
                        ThirdVertex = vector3,
                        FourthVertex = vector4,
                        EdgeFlags = visibles
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public IReadOnlyList<PolyfaceMeshVertex> Vertexes =>
            this.vertexes;

        public IReadOnlyList<PolyfaceMeshFace> Faces =>
            this.faces;

        internal PolylinetypeFlags Flags =>
            this.flags;

        internal netDxf.Entities.EndSequence EndSequence =>
            this.endSequence;
    }
}

