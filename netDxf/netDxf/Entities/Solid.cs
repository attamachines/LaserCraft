namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;

    public class Solid : EntityObject
    {
        private Vector2 firstVertex;
        private Vector2 secondVertex;
        private Vector2 thirdVertex;
        private Vector2 fourthVertex;
        private double elevation;
        private double thickness;

        public Solid() : this(Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero)
        {
        }

        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex) : this(new Vector2(firstVertex.X, firstVertex.Y), new Vector2(secondVertex.X, secondVertex.Y), new Vector2(thirdVertex.X, thirdVertex.Y), new Vector2(thirdVertex.X, thirdVertex.Y))
        {
        }

        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex, Vector2 fourthVertex) : base(EntityType.Solid, "SOLID")
        {
            this.firstVertex = firstVertex;
            this.secondVertex = secondVertex;
            this.thirdVertex = thirdVertex;
            this.fourthVertex = fourthVertex;
            this.elevation = 0.0;
            this.thickness = 0.0;
        }

        public override object Clone()
        {
            Solid solid = new Solid {
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
                Thickness = this.thickness
            };
            foreach (XData data in base.XData.Values)
            {
                solid.XData.Add((XData) data.Clone());
            }
            return solid;
        }

        public Vector2 FirstVertex
        {
            get => 
                this.firstVertex;
            set => 
                (this.firstVertex = value);
        }

        public Vector2 SecondVertex
        {
            get => 
                this.secondVertex;
            set => 
                (this.secondVertex = value);
        }

        public Vector2 ThirdVertex
        {
            get => 
                this.thirdVertex;
            set => 
                (this.thirdVertex = value);
        }

        public Vector2 FourthVertex
        {
            get => 
                this.fourthVertex;
            set => 
                (this.fourthVertex = value);
        }

        public double Elevation
        {
            get => 
                this.elevation;
            set => 
                (this.elevation = value);
        }

        public double Thickness
        {
            get => 
                this.thickness;
            set => 
                (this.thickness = value);
        }
    }
}

