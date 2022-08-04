namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;

    public class Line : EntityObject
    {
        private Vector3 start;
        private Vector3 end;
        private double thickness;

        public Line() : this(Vector3.Zero, Vector3.Zero)
        {
        }

        public Line(Vector2 startPoint, Vector2 endPoint) : this(new Vector3(startPoint.X, startPoint.Y, 0.0), new Vector3(endPoint.X, endPoint.Y, 0.0))
        {
        }

        public Line(Vector3 startPoint, Vector3 endPoint) : base(EntityType.Line, "LINE")
        {
            this.start = startPoint;
            this.end = endPoint;
            this.thickness = 0.0;
        }

        public override object Clone()
        {
            netDxf.Entities.Line line = new netDxf.Entities.Line {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                StartPoint = this.start,
                EndPoint = this.end,
                Thickness = this.thickness
            };
            foreach (XData data in base.XData.Values)
            {
                line.XData.Add((XData) data.Clone());
            }
            return line;
        }

        public void Reverse()
        {
            Vector3 start = this.start;
            this.start = this.end;
            this.end = start;
        }

        public Vector3 StartPoint
        {
            get => 
                this.start;
            set => 
                (this.start = value);
        }

        public Vector3 EndPoint
        {
            get => 
                this.end;
            set => 
                (this.end = value);
        }

        public Vector3 Direction =>
            (this.end - this.start);

        public double Thickness
        {
            get => 
                this.thickness;
            set => 
                (this.thickness = value);
        }
    }
}

