namespace netDxf.Entities
{
    using System;
    using System.Threading;

    public class MeshEdge : ICloneable
    {
        private int startVertexIndex;
        private int endVertexIndex;
        private double crease;

        public MeshEdge(int startVertexIndex, int endVertexIndex) : this(startVertexIndex, endVertexIndex, 0.0)
        {
        }

        public MeshEdge(int startVertexIndex, int endVertexIndex, double crease)
        {
            if (startVertexIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startVertexIndex", startVertexIndex, "The vertex index must be positive.");
            }
            this.startVertexIndex = startVertexIndex;
            if (endVertexIndex < 0)
            {
                throw new ArgumentOutOfRangeException("endVertexIndex", endVertexIndex, "The vertex index must be positive.");
            }
            this.endVertexIndex = endVertexIndex;
            this.crease = (crease < 0.0) ? -1.0 : crease;
        }

        public object Clone() => 
            new MeshEdge(this.startVertexIndex, this.endVertexIndex, this.crease);

        public override string ToString() => 
            string.Format("{0}: ({1}{4} {2}) crease={3}", new object[] { "SplineVertex", this.startVertexIndex, this.endVertexIndex, this.crease, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator });

        public string ToString(IFormatProvider provider) => 
            string.Format("{0}: ({1}{4} {2}) crease={3}", new object[] { "SplineVertex", this.startVertexIndex.ToString(provider), this.endVertexIndex.ToString(provider), this.crease.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator });

        public int StartVertexIndex
        {
            get => 
                this.startVertexIndex;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The vertex index must be must be equals or greater than zero.");
                }
                this.startVertexIndex = value;
            }
        }

        public int EndVertexIndex
        {
            get => 
                this.endVertexIndex;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The vertex index must be must be equals or greater than zero.");
                }
                this.endVertexIndex = value;
            }
        }

        public double Crease
        {
            get => 
                this.crease;
            set => 
                (this.crease = (value < 0.0) ? -1.0 : value);
        }
    }
}

