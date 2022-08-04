namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class Spline : EntityObject
    {
        private readonly List<Vector3> fitPoints;
        private readonly SplineCreationMethod creationMethod;
        private Vector3? startTangent;
        private Vector3? endTangent;
        private SplineKnotParameterization knotParameterization;
        private double knotTolerance;
        private double ctrlPointTolerance;
        private double fitTolerance;
        private List<SplineVertex> controlPoints;
        private List<double> knots;
        private readonly SplinetypeFlags flags;
        private readonly short degree;
        private readonly bool isClosed;
        private readonly bool isPeriodic;

        public Spline(IEnumerable<Vector3> fitPoints) : base(EntityType.Spline, "SPLINE")
        {
            this.knotTolerance = 1E-07;
            this.ctrlPointTolerance = 1E-07;
            this.fitTolerance = 1E-10;
            this.degree = 3;
            this.isPeriodic = false;
            this.controlPoints = new List<SplineVertex>();
            this.knots = new List<double>();
            if (fitPoints == null)
            {
                throw new ArgumentNullException("fitPoints");
            }
            this.fitPoints = new List<Vector3>(fitPoints);
            this.creationMethod = SplineCreationMethod.FitPoints;
            this.isClosed = this.fitPoints[0].Equals(this.fitPoints[this.fitPoints.Count - 1]);
            this.flags = this.isClosed ? (SplinetypeFlags.Rational | SplinetypeFlags.Closed) : SplinetypeFlags.Rational;
        }

        public Spline(List<SplineVertex> controlPoints) : this(controlPoints, 3, false)
        {
        }

        public Spline(List<SplineVertex> controlPoints, bool periodic) : this(controlPoints, 3, periodic)
        {
        }

        public Spline(List<SplineVertex> controlPoints, short degree) : this(controlPoints, degree, false)
        {
        }

        public Spline(List<SplineVertex> controlPoints, List<double> knots, short degree) : this(controlPoints, knots, degree, new List<Vector3>(), SplineCreationMethod.ControlPoints, false)
        {
        }

        public Spline(List<SplineVertex> controlPoints, short degree, bool periodic) : base(EntityType.Spline, "SPLINE")
        {
            this.knotTolerance = 1E-07;
            this.ctrlPointTolerance = 1E-07;
            this.fitTolerance = 1E-10;
            if ((degree < 1) || (degree > 10))
            {
                throw new ArgumentOutOfRangeException("degree", degree, "The spline degree valid values range from 1 to 10.");
            }
            if (controlPoints == null)
            {
                throw new ArgumentNullException("controlPoints");
            }
            if (controlPoints.Count < 2)
            {
                throw new ArgumentException("The number of control points must be equal or greater than 2.");
            }
            if (controlPoints.Count < (degree + 1))
            {
                throw new ArgumentException("The number of control points must be equal or greater than the spline degree + 1.");
            }
            this.fitPoints = new List<Vector3>();
            this.degree = degree;
            this.creationMethod = SplineCreationMethod.ControlPoints;
            this.isPeriodic = periodic;
            if (this.isPeriodic)
            {
                this.isClosed = true;
                this.flags = SplinetypeFlags.Rational | SplinetypeFlags.Periodic | SplinetypeFlags.Closed;
            }
            else
            {
                this.isClosed = controlPoints[0].Position.Equals(controlPoints[controlPoints.Count - 1].Position);
                this.flags = this.isClosed ? (SplinetypeFlags.Rational | SplinetypeFlags.Closed) : SplinetypeFlags.Rational;
            }
            this.Create(controlPoints);
        }

        internal Spline(List<SplineVertex> controlPoints, List<double> knots, short degree, List<Vector3> fitPoints, SplineCreationMethod method, bool isPeriodic) : base(EntityType.Spline, "SPLINE")
        {
            this.knotTolerance = 1E-07;
            this.ctrlPointTolerance = 1E-07;
            this.fitTolerance = 1E-10;
            if ((degree < 1) || (degree > 10))
            {
                throw new ArgumentOutOfRangeException("degree", degree, "The spline degree valid values range from 1 to 10.");
            }
            if (controlPoints == null)
            {
                throw new ArgumentNullException("controlPoints");
            }
            if (controlPoints.Count < 2)
            {
                throw new ArgumentException("The number of control points must be equal or greater than 2.");
            }
            if (controlPoints.Count < (degree + 1))
            {
                throw new ArgumentException("The number of control points must be equal or greater than the spline degree + 1.");
            }
            if (knots == null)
            {
                throw new ArgumentNullException("knots");
            }
            if (knots.Count != ((controlPoints.Count + degree) + 1))
            {
                throw new ArgumentException("The number of knots must be equals to the number of control points + spline degree + 1.");
            }
            this.fitPoints = fitPoints;
            this.controlPoints = controlPoints;
            this.knots = knots;
            this.degree = degree;
            this.creationMethod = method;
            this.isPeriodic = isPeriodic;
            if (this.isPeriodic)
            {
                this.isClosed = true;
                this.flags = SplinetypeFlags.Rational | SplinetypeFlags.Periodic | SplinetypeFlags.Closed;
            }
            else
            {
                this.isClosed = controlPoints[0].Position.Equals(controlPoints[controlPoints.Count - 1].Position);
                this.flags = this.isClosed ? (SplinetypeFlags.Rational | SplinetypeFlags.Closed) : SplinetypeFlags.Rational;
            }
        }

        private Vector3 C(double u)
        {
            Vector3 zero = Vector3.Zero;
            double num = 0.0;
            for (int i = 0; i < this.controlPoints.Count; i++)
            {
                double num3 = this.N(i, this.degree, u);
                num += num3 * this.controlPoints[i].Weigth;
                zero += (this.controlPoints[i].Weigth * num3) * this.controlPoints[i].Position;
            }
            if (Math.Abs(num) < double.Epsilon)
            {
                return Vector3.Zero;
            }
            return (Vector3) ((1.0 / num) * zero);
        }

        public override object Clone()
        {
            netDxf.Entities.Spline spline;
            if (this.creationMethod == SplineCreationMethod.FitPoints)
            {
                spline = new netDxf.Entities.Spline(new List<Vector3>(this.fitPoints)) {
                    Layer = (Layer) base.Layer.Clone(),
                    Linetype = (Linetype) base.Linetype.Clone(),
                    Color = (AciColor) base.Color.Clone(),
                    Lineweight = base.Lineweight,
                    Transparency = (Transparency) base.Transparency.Clone(),
                    LinetypeScale = base.LinetypeScale,
                    Normal = base.Normal,
                    IsVisible = base.IsVisible,
                    KnotParameterization = this.KnotParameterization,
                    StartTangent = this.startTangent,
                    EndTangent = this.endTangent
                };
            }
            else
            {
                List<SplineVertex> controlPoints = new List<SplineVertex>(this.controlPoints.Count);
                foreach (SplineVertex vertex in this.controlPoints)
                {
                    controlPoints.Add((SplineVertex) vertex.Clone());
                }
                List<double> knots = new List<double>(this.knots);
                spline = new netDxf.Entities.Spline(controlPoints, knots, this.degree) {
                    Layer = (Layer) base.Layer.Clone(),
                    Linetype = (Linetype) base.Linetype.Clone(),
                    Color = (AciColor) base.Color.Clone(),
                    Lineweight = base.Lineweight,
                    Transparency = (Transparency) base.Transparency.Clone(),
                    LinetypeScale = base.LinetypeScale,
                    Normal = base.Normal
                };
            }
            foreach (XData data in base.XData.Values)
            {
                spline.XData.Add((XData) data.Clone());
            }
            return spline;
        }

        private void Create(List<SplineVertex> points)
        {
            this.controlPoints = new List<SplineVertex>();
            int num = this.isPeriodic ? this.degree : 0;
            int num2 = points.Count + num;
            foreach (SplineVertex vertex in points)
            {
                SplineVertex item = new SplineVertex(vertex.Position, vertex.Weigth);
                this.controlPoints.Add(item);
            }
            for (int i = 0; i < num; i++)
            {
                SplineVertex item = new SplineVertex(points[i].Position, points[i].Weigth);
                this.controlPoints.Add(item);
            }
            int capacity = (num2 + this.degree) + 1;
            this.knots = new List<double>(capacity);
            double num4 = 1.0 / ((double) (num2 - this.degree));
            if (!this.isPeriodic)
            {
                int num6 = 0;
                while (num6 <= this.degree)
                {
                    this.knots.Add(0.0);
                    num6++;
                }
                while (num6 < num2)
                {
                    this.knots.Add((double) (num6 - this.degree));
                    num6++;
                }
                while (num6 < capacity)
                {
                    this.knots.Add((double) (num2 - this.degree));
                    num6++;
                }
            }
            else
            {
                for (int j = 0; j < capacity; j++)
                {
                    this.knots.Add((j - this.degree) * num4);
                }
            }
        }

        private double N(int i, int p, double u)
        {
            if (p <= 0)
            {
                if ((this.knots[i] <= u) && (u < this.knots[i + 1]))
                {
                    return 1.0;
                }
                return 0.0;
            }
            double num = 0.0;
            if (Math.Abs((double) (this.knots[i + p] - this.knots[i])) >= double.Epsilon)
            {
                num = (u - this.knots[i]) / (this.knots[i + p] - this.knots[i]);
            }
            double num2 = 0.0;
            if (Math.Abs((double) (this.knots[(i + p) + 1] - this.knots[i + 1])) >= double.Epsilon)
            {
                num2 = (this.knots[(i + p) + 1] - u) / (this.knots[(i + p) + 1] - this.knots[i + 1]);
            }
            return ((num * this.N(i, p - 1, u)) + (num2 * this.N(i + 1, p - 1, u)));
        }

        public List<Vector3> PolygonalVertexes(int precision)
        {
            double num;
            double num2;
            if (this.controlPoints.Count == 0)
            {
                throw new NotSupportedException("A spline entity with control points is required.");
            }
            List<Vector3> list = new List<Vector3>();
            if (!this.isClosed)
            {
                precision--;
                num = this.knots[0];
                num2 = this.knots[this.knots.Count - 1];
            }
            else if (this.isPeriodic)
            {
                num = this.knots[this.degree];
                num2 = this.knots[(this.knots.Count - this.degree) - 1];
            }
            else
            {
                num = this.knots[0];
                num2 = this.knots[this.knots.Count - 1];
            }
            double num3 = (num2 - num) / ((double) precision);
            for (int i = 0; i < precision; i++)
            {
                double u = num + (num3 * i);
                list.Add(this.C(u));
            }
            if (!this.isClosed)
            {
                list.Add(this.controlPoints[this.controlPoints.Count - 1].Position);
            }
            return list;
        }

        public void Reverse()
        {
            this.fitPoints.Reverse();
            this.controlPoints.Reverse();
            Vector3? startTangent = this.startTangent;
            Vector3? endTangent = this.endTangent;
            this.startTangent = endTangent.HasValue ? new Vector3?(-endTangent.GetValueOrDefault()) : null;
            endTangent = startTangent;
            this.endTangent = endTangent.HasValue ? new Vector3?(-endTangent.GetValueOrDefault()) : null;
        }

        public void SetUniformWeights(double weight)
        {
            foreach (SplineVertex vertex in this.controlPoints)
            {
                vertex.Weigth = weight;
            }
        }

        public netDxf.Entities.Polyline ToPolyline(int precision)
        {
            IEnumerable<Vector3> enumerable = this.PolygonalVertexes(precision);
            netDxf.Entities.Polyline polyline = new netDxf.Entities.Polyline {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsClosed = this.isClosed
            };
            foreach (Vector3 vector in enumerable)
            {
                polyline.Vertexes.Add(new PolylineVertex(vector));
            }
            return polyline;
        }

        public List<Vector3> FitPoints =>
            this.fitPoints;

        public Vector3? StartTangent
        {
            get => 
                this.startTangent;
            set => 
                (this.startTangent = value);
        }

        public Vector3? EndTangent
        {
            get => 
                this.endTangent;
            set => 
                (this.endTangent = value);
        }

        public SplineKnotParameterization KnotParameterization
        {
            get => 
                this.knotParameterization;
            set => 
                (this.knotParameterization = value);
        }

        public SplineCreationMethod CreationMethod =>
            this.creationMethod;

        public double KnotTolerance
        {
            get => 
                this.knotTolerance;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The knot tolerance must be greater than zero.");
                }
                this.knotTolerance = value;
            }
        }

        public double CtrlPointTolerance
        {
            get => 
                this.ctrlPointTolerance;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The control point tolerance must be greater than zero.");
                }
                this.ctrlPointTolerance = value;
            }
        }

        public double FitTolerance
        {
            get => 
                this.fitTolerance;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The fit tolerance must be greater than zero.");
                }
                this.fitTolerance = value;
            }
        }

        public short Degree =>
            this.degree;

        public bool IsClosed =>
            this.isClosed;

        public bool IsPeriodic =>
            this.isPeriodic;

        public IReadOnlyList<SplineVertex> ControlPoints =>
            this.controlPoints;

        public IReadOnlyList<double> Knots =>
            this.knots;

        internal SplinetypeFlags Flags =>
            this.flags;
    }
}

