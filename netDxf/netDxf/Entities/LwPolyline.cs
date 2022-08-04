namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class LwPolyline : EntityObject
    {
        private readonly List<LwPolylineVertex> vertexes;
        private PolylinetypeFlags flags;
        private double elevation;
        private double thickness;

        public LwPolyline() : this(new List<LwPolylineVertex>())
        {
        }

        public LwPolyline(IEnumerable<LwPolylineVertex> vertexes) : this(vertexes, false)
        {
        }

        public LwPolyline(IEnumerable<Vector2> vertexes) : this(vertexes, false)
        {
        }

        public LwPolyline(IEnumerable<LwPolylineVertex> vertexes, bool isClosed) : base(EntityType.LightWeightPolyline, "LWPOLYLINE")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<LwPolylineVertex>(vertexes);
            this.elevation = 0.0;
            this.thickness = 0.0;
            this.flags = isClosed ? PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM : PolylinetypeFlags.OpenPolyline;
        }

        public LwPolyline(IEnumerable<Vector2> vertexes, bool isClosed) : base(EntityType.LightWeightPolyline, "LWPOLYLINE")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<LwPolylineVertex>();
            foreach (Vector2 vector in vertexes)
            {
                this.vertexes.Add(new LwPolylineVertex(vector));
            }
            this.elevation = 0.0;
            this.thickness = 0.0;
            this.flags = isClosed ? PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM : PolylinetypeFlags.OpenPolyline;
        }

        public override object Clone()
        {
            LwPolyline polyline = new LwPolyline {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Elevation = this.elevation,
                Thickness = this.thickness,
                Flags = this.flags
            };
            foreach (LwPolylineVertex vertex in this.vertexes)
            {
                polyline.Vertexes.Add((LwPolylineVertex) vertex.Clone());
            }
            foreach (XData data in base.XData.Values)
            {
                polyline.XData.Add((XData) data.Clone());
            }
            return polyline;
        }

        public List<EntityObject> Explode()
        {
            List<EntityObject> list = new List<EntityObject>();
            int num = 0;
            foreach (LwPolylineVertex vertex in this.Vertexes)
            {
                Vector2 vector;
                Vector2 vector2;
                double bulge = vertex.Bulge;
                if (num == (this.Vertexes.Count - 1))
                {
                    if (!this.IsClosed)
                    {
                        return list;
                    }
                    vector = new Vector2(vertex.Position.X, vertex.Position.Y);
                    vector2 = new Vector2(this.vertexes[0].Position.X, this.vertexes[0].Position.Y);
                }
                else
                {
                    vector = new Vector2(vertex.Position.X, vertex.Position.Y);
                    vector2 = new Vector2(this.vertexes[num + 1].Position.X, this.vertexes[num + 1].Position.Y);
                }
                if (MathHelper.IsZero(bulge))
                {
                    Vector3 vector4 = MathHelper.Transform(new Vector3(vector.X, vector.Y, this.elevation), base.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    Vector3 vector5 = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, this.elevation), base.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    netDxf.Entities.Line item = new netDxf.Entities.Line {
                        Layer = (Layer) base.Layer.Clone(),
                        Linetype = (Linetype) base.Linetype.Clone(),
                        Color = (AciColor) base.Color.Clone(),
                        Lineweight = base.Lineweight,
                        Transparency = (Transparency) base.Transparency.Clone(),
                        LinetypeScale = base.LinetypeScale,
                        Normal = base.Normal,
                        StartPoint = vector4,
                        EndPoint = vector5,
                        Thickness = this.Thickness
                    };
                    list.Add(item);
                }
                else
                {
                    double num8;
                    double num9;
                    double num3 = 4.0 * Math.Atan(Math.Abs(bulge));
                    double num5 = (Vector2.Distance(vector, vector2) / 2.0) / Math.Sin(num3 / 2.0);
                    double num6 = (3.1415926535897931 - num3) / 2.0;
                    double d = Vector2.Angle(vector, vector2) + (Math.Sign(bulge) * num6);
                    Vector2 vector6 = new Vector2(vector.X + (num5 * Math.Cos(d)), vector.Y + (num5 * Math.Sin(d)));
                    if (bulge > 0.0)
                    {
                        num8 = 57.295779513082323 * Vector2.Angle(vector - vector6);
                        num9 = num8 + (57.295779513082323 * num3);
                    }
                    else
                    {
                        num9 = 57.295779513082323 * Vector2.Angle(vector - vector6);
                        num8 = num9 - (57.295779513082323 * num3);
                    }
                    Vector3 vector7 = MathHelper.Transform(new Vector3(vector6.X, vector6.Y, this.elevation), base.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    netDxf.Entities.Arc item = new netDxf.Entities.Arc {
                        Layer = (Layer) base.Layer.Clone(),
                        Linetype = (Linetype) base.Linetype.Clone(),
                        Color = (AciColor) base.Color.Clone(),
                        Lineweight = base.Lineweight,
                        Transparency = (Transparency) base.Transparency.Clone(),
                        LinetypeScale = base.LinetypeScale,
                        Normal = base.Normal,
                        Center = vector7,
                        Radius = num5,
                        StartAngle = num8,
                        EndAngle = num9,
                        Thickness = this.Thickness
                    };
                    list.Add(item);
                }
                num++;
            }
            return list;
        }

        public List<Vector2> PoligonalVertexes(int bulgePrecision, double weldThreshold, double bulgeThreshold)
        {
            List<Vector2> list = new List<Vector2>();
            int num = 0;
            foreach (LwPolylineVertex vertex in this.Vertexes)
            {
                Vector2 vector;
                Vector2 vector2;
                double bulge = vertex.Bulge;
                if (num == (this.Vertexes.Count - 1))
                {
                    vector = new Vector2(vertex.Position.X, vertex.Position.Y);
                    vector2 = new Vector2(this.vertexes[0].Position.X, this.vertexes[0].Position.Y);
                }
                else
                {
                    vector = new Vector2(vertex.Position.X, vertex.Position.Y);
                    vector2 = new Vector2(this.vertexes[num + 1].Position.X, this.vertexes[num + 1].Position.Y);
                }
                if (!vector.Equals(vector2, weldThreshold))
                {
                    if (MathHelper.IsZero(bulge) || (bulgePrecision == 0))
                    {
                        list.Add(vector);
                    }
                    else
                    {
                        double num3 = Vector2.Distance(vector, vector2);
                        if (num3 >= bulgeThreshold)
                        {
                            double num4 = (num3 / 2.0) * Math.Abs(bulge);
                            double num5 = (((num3 / 2.0) * (num3 / 2.0)) + (num4 * num4)) / (2.0 * num4);
                            double num6 = 4.0 * Math.Atan(Math.Abs(bulge));
                            double num7 = (3.1415926535897931 - num6) / 2.0;
                            double d = Vector2.Angle(vector, vector2) + (Math.Sign(bulge) * num7);
                            Vector2 vector4 = new Vector2(vector.X + (num5 * Math.Cos(d)), vector.Y + (num5 * Math.Sin(d)));
                            Vector2 vector5 = vector - vector4;
                            double num9 = (Math.Sign(bulge) * num6) / ((double) (bulgePrecision + 1));
                            list.Add(vector);
                            for (int i = 1; i <= bulgePrecision; i++)
                            {
                                Vector2 item = new Vector2();
                                Vector2 other = new Vector2(this.vertexes[this.vertexes.Count - 1].Position.X, this.vertexes[this.vertexes.Count - 1].Position.Y);
                                item.X = (vector4.X + (Math.Cos(i * num9) * vector5.X)) - (Math.Sin(i * num9) * vector5.Y);
                                item.Y = (vector4.Y + (Math.Sin(i * num9) * vector5.X)) + (Math.Cos(i * num9) * vector5.Y);
                                if (!item.Equals(other, weldThreshold) && !item.Equals(vector2, weldThreshold))
                                {
                                    list.Add(item);
                                }
                            }
                        }
                        else
                        {
                            list.Add(vector);
                        }
                    }
                }
                num++;
            }
            return list;
        }

        public void Reverse()
        {
            this.vertexes.Reverse();
        }

        public void SetConstantWidth(double width)
        {
            foreach (LwPolylineVertex vertex in this.vertexes)
            {
                vertex.StartWidth = width;
                vertex.EndWidth = width;
            }
        }

        public List<LwPolylineVertex> Vertexes =>
            this.vertexes;

        public bool IsClosed
        {
            get => 
                this.flags.HasFlag(PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM);
            set
            {
                if (value)
                {
                    this.flags |= PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
                }
                else
                {
                    this.flags &= ~PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
                }
            }
        }

        public double Thickness
        {
            get => 
                this.thickness;
            set => 
                (this.thickness = value);
        }

        public double Elevation
        {
            get => 
                this.elevation;
            set => 
                (this.elevation = value);
        }

        public bool LinetypeGeneration
        {
            get => 
                this.flags.HasFlag(PolylinetypeFlags.ContinuousLinetypePattern);
            set
            {
                if (value)
                {
                    this.flags |= PolylinetypeFlags.ContinuousLinetypePattern;
                }
                else
                {
                    this.flags &= ~PolylinetypeFlags.ContinuousLinetypePattern;
                }
            }
        }

        internal PolylinetypeFlags Flags
        {
            get => 
                this.flags;
            set => 
                (this.flags = value);
        }
    }
}

