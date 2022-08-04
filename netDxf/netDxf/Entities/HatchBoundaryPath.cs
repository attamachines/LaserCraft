namespace netDxf.Entities
{
    using netDxf;
    using System;
    using System.Collections.Generic;

    public class HatchBoundaryPath : ICloneable
    {
        private readonly List<EntityObject> contour;
        private readonly List<Edge> edges;
        private HatchBoundaryPathTypeFlags pathType;

        public HatchBoundaryPath(IEnumerable<EntityObject> edges)
        {
            if (edges == null)
            {
                throw new ArgumentNullException("edges");
            }
            this.edges = new List<Edge>();
            this.pathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
            this.contour = new List<EntityObject>(edges);
            this.Update();
        }

        internal HatchBoundaryPath(IEnumerable<Edge> edges)
        {
            if (edges == null)
            {
                throw new ArgumentNullException("edges");
            }
            this.pathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
            this.contour = new List<EntityObject>();
            this.edges = new List<Edge>(edges);
        }

        internal void AddContour(EntityObject entity)
        {
            this.contour.Add(entity);
        }

        internal void ClearContour()
        {
            this.contour.Clear();
        }

        public object Clone()
        {
            List<Edge> edges = new List<Edge>();
            foreach (Edge edge in this.edges)
            {
                edges.Add((Edge) edge.Clone());
            }
            return new HatchBoundaryPath(edges) { pathType = this.pathType };
        }

        internal bool RemoveContour(EntityObject entity) => 
            this.contour.Remove(entity);

        private void SetInternalInfo(IEnumerable<EntityObject> entities)
        {
            bool flag = false;
            this.edges.Clear();
            foreach (EntityObject obj2 in entities)
            {
                if (this.pathType.HasFlag(HatchBoundaryPathTypeFlags.Polyline) && (this.edges.Count >= 1))
                {
                    throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                }
                switch (obj2.Type)
                {
                    case EntityType.Line:
                    {
                        if (flag)
                        {
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        }
                        this.edges.Add(Line.ConvertFrom(obj2));
                        continue;
                    }
                    case EntityType.Spline:
                    {
                        if (flag)
                        {
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        }
                        this.edges.Add(Spline.ConvertFrom(obj2));
                        continue;
                    }
                    case EntityType.Arc:
                        if (flag)
                        {
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        }
                        break;

                    case EntityType.Circle:
                        if (flag)
                        {
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        }
                        goto Label_00ED;

                    case EntityType.Ellipse:
                        if (flag)
                        {
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        }
                        goto Label_0116;

                    case EntityType.LightWeightPolyline:
                    {
                        if (flag)
                        {
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        }
                        LwPolyline polyline = (LwPolyline) obj2;
                        if (polyline.IsClosed)
                        {
                            this.edges.Add(Polyline.ConvertFrom(obj2));
                            this.pathType |= HatchBoundaryPathTypeFlags.Polyline;
                            flag = true;
                        }
                        else
                        {
                            this.SetInternalInfo(polyline.Explode());
                        }
                        continue;
                    }
                    default:
                        throw new ArgumentException($"The entity type {obj2.Type} cannot be part of a hatch boundary.");
                }
                this.edges.Add(Arc.ConvertFrom(obj2));
                continue;
            Label_00ED:
                this.edges.Add(Arc.ConvertFrom(obj2));
                continue;
            Label_0116:
                this.edges.Add(Ellipse.ConvertFrom(obj2));
            }
        }

        public void Update()
        {
            this.SetInternalInfo(this.contour);
        }

        public IReadOnlyList<Edge> Edges =>
            this.edges;

        public HatchBoundaryPathTypeFlags PathType
        {
            get => 
                this.pathType;
            internal set => 
                (this.pathType = value);
        }

        public IReadOnlyList<EntityObject> Entities =>
            this.contour;

        public class Arc : HatchBoundaryPath.Edge
        {
            public Vector2 Center;
            public double Radius;
            public double StartAngle;
            public double EndAngle;
            public bool IsCounterclockwise;

            public Arc() : base(HatchBoundaryPath.EdgeType.Arc)
            {
            }

            public Arc(EntityObject entity) : base(HatchBoundaryPath.EdgeType.Arc)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                switch (entity.Type)
                {
                    case EntityType.Arc:
                    {
                        netDxf.Entities.Arc arc = (netDxf.Entities.Arc) entity;
                        this.Center = new Vector2(arc.Center.X, arc.Center.Y);
                        this.Radius = arc.Radius;
                        this.StartAngle = arc.StartAngle;
                        this.EndAngle = arc.EndAngle;
                        this.IsCounterclockwise = true;
                        break;
                    }
                    case EntityType.Circle:
                    {
                        Circle circle = (Circle) entity;
                        this.Center = new Vector2(circle.Center.X, circle.Center.Y);
                        this.Radius = circle.Radius;
                        this.StartAngle = 0.0;
                        this.EndAngle = 360.0;
                        this.IsCounterclockwise = true;
                        break;
                    }
                    default:
                        throw new ArgumentException("The entity is not a Circle or an Arc", "entity");
                }
            }

            public override object Clone() => 
                new HatchBoundaryPath.Arc { 
                    Center = this.Center,
                    Radius = this.Radius,
                    StartAngle = this.StartAngle,
                    EndAngle = this.EndAngle,
                    IsCounterclockwise = this.IsCounterclockwise
                };

            public static HatchBoundaryPath.Arc ConvertFrom(EntityObject entity) => 
                new HatchBoundaryPath.Arc(entity);

            public override EntityObject ConvertTo()
            {
                if (MathHelper.IsEqual(MathHelper.NormalizeAngle(this.StartAngle), MathHelper.NormalizeAngle(this.EndAngle)))
                {
                    return new Circle(this.Center, this.Radius);
                }
                if (this.IsCounterclockwise)
                {
                    return new netDxf.Entities.Arc(this.Center, this.Radius, this.StartAngle, this.EndAngle);
                }
                return new netDxf.Entities.Arc(this.Center, this.Radius, 360.0 - this.EndAngle, 360.0 - this.StartAngle);
            }
        }

        public abstract class Edge : ICloneable
        {
            public readonly HatchBoundaryPath.EdgeType Type;

            protected Edge(HatchBoundaryPath.EdgeType type)
            {
                this.Type = type;
            }

            public abstract object Clone();
            public abstract EntityObject ConvertTo();
        }

        public enum EdgeType
        {
            Polyline,
            Line,
            Arc,
            Ellipse,
            Spline
        }

        public class Ellipse : HatchBoundaryPath.Edge
        {
            public Vector2 Center;
            public Vector2 EndMajorAxis;
            public double MinorRatio;
            public double StartAngle;
            public double EndAngle;
            public bool IsCounterclockwise;

            public Ellipse() : base(HatchBoundaryPath.EdgeType.Ellipse)
            {
            }

            public Ellipse(EntityObject entity) : base(HatchBoundaryPath.EdgeType.Ellipse)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                netDxf.Entities.Ellipse ellipse = entity as netDxf.Entities.Ellipse;
                if (ellipse == null)
                {
                    throw new ArgumentException("The entity is not an Ellipse", "entity");
                }
                this.Center = new Vector2(ellipse.Center.X, ellipse.Center.Y);
                double y = (0.5 * ellipse.MajorAxis) * Math.Sin(ellipse.Rotation * 0.017453292519943295);
                double x = (0.5 * ellipse.MajorAxis) * Math.Cos(ellipse.Rotation * 0.017453292519943295);
                this.EndMajorAxis = new Vector2(x, y);
                this.MinorRatio = ellipse.MinorAxis / ellipse.MajorAxis;
                if (ellipse.IsFullEllipse)
                {
                    this.StartAngle = 0.0;
                    this.EndAngle = 360.0;
                }
                else
                {
                    this.StartAngle = ellipse.StartAngle;
                    this.EndAngle = ellipse.EndAngle;
                }
                this.IsCounterclockwise = true;
            }

            public override object Clone() => 
                new HatchBoundaryPath.Ellipse { 
                    Center = this.Center,
                    EndMajorAxis = this.EndMajorAxis,
                    MinorRatio = this.MinorRatio,
                    StartAngle = this.StartAngle,
                    EndAngle = this.EndAngle,
                    IsCounterclockwise = this.IsCounterclockwise
                };

            public static HatchBoundaryPath.Ellipse ConvertFrom(EntityObject entity) => 
                new HatchBoundaryPath.Ellipse(entity);

            public override EntityObject ConvertTo()
            {
                Vector3 vector = new Vector3(this.Center.X, this.Center.Y, 0.0);
                Vector3 point = new Vector3(this.EndMajorAxis.X, this.EndMajorAxis.Y, 0.0);
                Vector3 vector3 = MathHelper.Transform(point, Vector3.UnitZ, CoordinateSystem.World, CoordinateSystem.Object);
                double num = Vector2.Angle(new Vector2(vector3.X, vector3.Y)) * 57.295779513082323;
                double num2 = 2.0 * point.Modulus();
                return new netDxf.Entities.Ellipse { 
                    MajorAxis = num2,
                    MinorAxis = num2 * this.MinorRatio,
                    Rotation = num,
                    Center = vector,
                    StartAngle = this.IsCounterclockwise ? this.StartAngle : (360.0 - this.EndAngle),
                    EndAngle = this.IsCounterclockwise ? this.EndAngle : (360.0 - this.StartAngle)
                };
            }
        }

        public class Line : HatchBoundaryPath.Edge
        {
            public Vector2 Start;
            public Vector2 End;

            public Line() : base(HatchBoundaryPath.EdgeType.Line)
            {
            }

            public Line(EntityObject entity) : base(HatchBoundaryPath.EdgeType.Line)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                netDxf.Entities.Line line = entity as netDxf.Entities.Line;
                if (line == null)
                {
                    throw new ArgumentException("The entity is not a Line", "entity");
                }
                this.Start = new Vector2(line.StartPoint.X, line.StartPoint.Y);
                this.End = new Vector2(line.EndPoint.X, line.EndPoint.Y);
            }

            public override object Clone() => 
                new HatchBoundaryPath.Line { 
                    Start = this.Start,
                    End = this.End
                };

            public static HatchBoundaryPath.Line ConvertFrom(EntityObject entity) => 
                new HatchBoundaryPath.Line(entity);

            public override EntityObject ConvertTo() => 
                new netDxf.Entities.Line(this.Start, this.End);
        }

        public class Polyline : HatchBoundaryPath.Edge
        {
            public Vector3[] Vertexes;
            public bool IsClosed;

            public Polyline() : base(HatchBoundaryPath.EdgeType.Polyline)
            {
            }

            public Polyline(EntityObject entity) : base(HatchBoundaryPath.EdgeType.Polyline)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (entity.Type == EntityType.LightWeightPolyline)
                {
                    LwPolyline polyline = (LwPolyline) entity;
                    if (!polyline.IsClosed)
                    {
                        throw new ArgumentException("Only closed polyline are supported as hatch boundary edges.", "entity");
                    }
                    this.Vertexes = new Vector3[polyline.Vertexes.Count];
                    for (int i = 0; i < polyline.Vertexes.Count; i++)
                    {
                        this.Vertexes[i] = new Vector3(polyline.Vertexes[i].Position.X, polyline.Vertexes[i].Position.Y, polyline.Vertexes[i].Bulge);
                    }
                    this.IsClosed = true;
                }
                else
                {
                    if (entity.Type != EntityType.Polyline)
                    {
                        throw new ArgumentException("The entity is not a LwPolyline or a Polyline", "entity");
                    }
                    netDxf.Entities.Polyline polyline2 = (netDxf.Entities.Polyline) entity;
                    if (!polyline2.IsClosed)
                    {
                        throw new ArgumentException("Only closed polyline are supported as hatch boundary edges.", "entity");
                    }
                    this.Vertexes = new Vector3[polyline2.Vertexes.Count];
                    for (int i = 0; i < polyline2.Vertexes.Count; i++)
                    {
                        this.Vertexes[i] = new Vector3(polyline2.Vertexes[i].Position.X, polyline2.Vertexes[i].Position.Y, 0.0);
                    }
                    this.IsClosed = true;
                }
            }

            public override object Clone()
            {
                HatchBoundaryPath.Polyline polyline = new HatchBoundaryPath.Polyline {
                    Vertexes = new Vector3[this.Vertexes.Length],
                    IsClosed = this.IsClosed
                };
                for (int i = 0; i < this.Vertexes.Length; i++)
                {
                    polyline.Vertexes[i] = this.Vertexes[i];
                }
                return polyline;
            }

            public static HatchBoundaryPath.Polyline ConvertFrom(EntityObject entity) => 
                new HatchBoundaryPath.Polyline(entity);

            public override EntityObject ConvertTo()
            {
                List<LwPolylineVertex> vertexes = new List<LwPolylineVertex>(this.Vertexes.Length);
                foreach (Vector3 vector in this.Vertexes)
                {
                    vertexes.Add(new LwPolylineVertex(vector.X, vector.Y, vector.Z));
                }
                return new LwPolyline(vertexes, this.IsClosed);
            }
        }

        public class Spline : HatchBoundaryPath.Edge
        {
            public short Degree;
            public bool IsRational;
            public bool IsPeriodic;
            public double[] Knots;
            public Vector3[] ControlPoints;

            public Spline() : base(HatchBoundaryPath.EdgeType.Spline)
            {
            }

            public Spline(EntityObject entity) : base(HatchBoundaryPath.EdgeType.Spline)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                netDxf.Entities.Spline spline = entity as netDxf.Entities.Spline;
                if (spline == null)
                {
                    throw new ArgumentException("The entity is not an Spline", "entity");
                }
                this.Degree = spline.Degree;
                this.IsRational = spline.Flags.HasFlag(SplinetypeFlags.Rational);
                this.IsPeriodic = spline.IsPeriodic;
                if (spline.ControlPoints.Count == 0)
                {
                    throw new ArgumentException("The HatchBoundaryPath spline edge requires a spline entity with control points.", "entity");
                }
                this.ControlPoints = new Vector3[spline.ControlPoints.Count];
                for (int i = 0; i < spline.ControlPoints.Count; i++)
                {
                    this.ControlPoints[i] = new Vector3(spline.ControlPoints[i].Position.X, spline.ControlPoints[i].Position.Y, spline.ControlPoints[i].Weigth);
                }
                this.Knots = new double[spline.Knots.Count];
                for (int j = 0; j < spline.Knots.Count; j++)
                {
                    this.Knots[j] = spline.Knots[j];
                }
            }

            public override object Clone()
            {
                HatchBoundaryPath.Spline spline = new HatchBoundaryPath.Spline {
                    Degree = this.Degree,
                    IsRational = this.IsRational,
                    IsPeriodic = this.IsPeriodic,
                    Knots = new double[this.Knots.Length],
                    ControlPoints = new Vector3[this.ControlPoints.Length]
                };
                for (int i = 0; i < this.Knots.Length; i++)
                {
                    spline.Knots[i] = this.Knots[i];
                }
                for (int j = 0; j < this.ControlPoints.Length; j++)
                {
                    spline.ControlPoints[j] = this.ControlPoints[j];
                }
                return spline;
            }

            public static HatchBoundaryPath.Spline ConvertFrom(EntityObject entity) => 
                new HatchBoundaryPath.Spline(entity);

            public override EntityObject ConvertTo()
            {
                List<SplineVertex> controlPoints = new List<SplineVertex>(this.ControlPoints.Length);
                foreach (Vector3 vector in this.ControlPoints)
                {
                    controlPoints.Add(new SplineVertex(vector.X, vector.Y, vector.Z));
                }
                return new netDxf.Entities.Spline(controlPoints, new List<double>(this.Knots), this.Degree);
            }
        }
    }
}

