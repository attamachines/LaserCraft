namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;

    public class Face3d : EntityObject
    {
        private Vector3 firstVertex;
        private Vector3 secondVertex;
        private Vector3 thirdVertex;
        private Vector3 fourthVertex;
        private Face3dEdgeFlags edgeFlags;

        public Face3d() : this(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero)
        {
        }

        public Face3d(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex) : this(new Vector3(firstVertex.X, firstVertex.Y, 0.0), new Vector3(secondVertex.X, secondVertex.Y, 0.0), new Vector3(thirdVertex.X, thirdVertex.Y, 0.0), new Vector3(thirdVertex.X, thirdVertex.Y, 0.0))
        {
        }

        public Face3d(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex) : this(firstVertex, secondVertex, thirdVertex, thirdVertex)
        {
        }

        public Face3d(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex, Vector2 fourthVertex) : this(new Vector3(firstVertex.X, firstVertex.Y, 0.0), new Vector3(secondVertex.X, secondVertex.Y, 0.0), new Vector3(thirdVertex.X, thirdVertex.Y, 0.0), new Vector3(fourthVertex.X, fourthVertex.Y, 0.0))
        {
        }

        public Face3d(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex, Vector3 fourthVertex) : base(EntityType.Face3D, "3DFACE")
        {
            this.firstVertex = firstVertex;
            this.secondVertex = secondVertex;
            this.thirdVertex = thirdVertex;
            this.fourthVertex = fourthVertex;
            this.edgeFlags = Face3dEdgeFlags.Visibles;
        }

        public override object Clone()
        {
            Face3d faced = new Face3d {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                FirstVertex = this.firstVertex,
                SecondVertex = this.secondVertex,
                ThirdVertex = this.thirdVertex,
                FourthVertex = this.fourthVertex,
                EdgeFlags = this.edgeFlags
            };
            foreach (XData data in base.XData.Values)
            {
                faced.XData.Add((XData) data.Clone());
            }
            return faced;
        }

        public Vector3 FirstVertex
        {
            get => 
                this.firstVertex;
            set => 
                (this.firstVertex = value);
        }

        public Vector3 SecondVertex
        {
            get => 
                this.secondVertex;
            set => 
                (this.secondVertex = value);
        }

        public Vector3 ThirdVertex
        {
            get => 
                this.thirdVertex;
            set => 
                (this.thirdVertex = value);
        }

        public Vector3 FourthVertex
        {
            get => 
                this.fourthVertex;
            set => 
                (this.fourthVertex = value);
        }

        public Face3dEdgeFlags EdgeFlags
        {
            get => 
                this.edgeFlags;
            set => 
                (this.edgeFlags = value);
        }
    }
}

