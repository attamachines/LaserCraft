namespace netDxf.Entities
{
    using netDxf;
    using System;
    using System.Collections.Generic;

    public class MLineVertex
    {
        private Vector2 location;
        private readonly Vector2 direction;
        private readonly Vector2 miter;
        private readonly List<double>[] distances;

        internal MLineVertex(Vector2 location, Vector2 direction, Vector2 miter, List<double>[] distances)
        {
            this.location = location;
            this.direction = direction;
            this.miter = miter;
            this.distances = distances;
        }

        public object Clone()
        {
            List<double>[] distances = new List<double>[this.distances.Length];
            for (int i = 0; i < this.distances.Length; i++)
            {
                distances[i] = new List<double>();
                distances[i].AddRange(this.distances[i]);
            }
            return new MLineVertex(this.location, this.direction, this.miter, distances);
        }

        public override string ToString() => 
            $"{"MLineVertex"}: ({this.location})";

        public Vector2 Location
        {
            get => 
                this.location;
            set => 
                (this.location = value);
        }

        public Vector2 Direction =>
            this.direction;

        public Vector2 Miter =>
            this.miter;

        public List<double>[] Distances =>
            this.distances;
    }
}

