namespace netDxf
{
    using System;
    using System.Collections.Generic;

    public class ClippingBoundary : ICloneable
    {
        private readonly ClippingBoundaryType type;
        private readonly IReadOnlyList<Vector2> vertexes;

        public ClippingBoundary(IEnumerable<Vector2> vertexes)
        {
            this.type = ClippingBoundaryType.Polygonal;
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 3)
            {
                throw new ArgumentOutOfRangeException("vertexes", this.vertexes.Count, "The number of vertexes for the polygonal clipping boundary must be equal or greater than three.");
            }
        }

        public ClippingBoundary(Vector2 firstCorner, Vector2 secondCorner)
        {
            this.type = ClippingBoundaryType.Rectangular;
            List<Vector2> list1 = new List<Vector2> {
                firstCorner,
                secondCorner
            };
            this.vertexes = list1;
        }

        public ClippingBoundary(double x, double y, double width, double height)
        {
            this.type = ClippingBoundaryType.Rectangular;
            List<Vector2> list1 = new List<Vector2> {
                new Vector2(x, y),
                new Vector2(x + width, y + height)
            };
            this.vertexes = list1;
        }

        public object Clone() => 
            ((this.type == ClippingBoundaryType.Rectangular) ? new ClippingBoundary(this.vertexes[0], this.vertexes[1]) : new ClippingBoundary(this.vertexes));

        public ClippingBoundaryType Type =>
            this.type;

        public IReadOnlyList<Vector2> Vertexes =>
            this.vertexes;
    }
}

