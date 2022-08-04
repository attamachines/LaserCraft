namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Collections;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Hatch : EntityObject
    {
        private readonly ObservableCollection<HatchBoundaryPath> boundaryPaths;
        private HatchPattern pattern;
        private double elevation;
        private bool associative;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event HatchBoundaryPathAddedEventHandler HatchBoundaryPathAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event HatchBoundaryPathRemovedEventHandler HatchBoundaryPathRemoved;

        public Hatch(HatchPattern pattern, bool associative) : base(EntityType.Hatch, "HATCH")
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            this.pattern = pattern;
            this.boundaryPaths = new ObservableCollection<HatchBoundaryPath>();
            this.boundaryPaths.BeforeAddItem += new ObservableCollection<HatchBoundaryPath>.BeforeAddItemEventHandler(this.BoundaryPaths_BeforeAddItem);
            this.boundaryPaths.AddItem += new ObservableCollection<HatchBoundaryPath>.AddItemEventHandler(this.BoundaryPaths_AddItem);
            this.boundaryPaths.BeforeRemoveItem += new ObservableCollection<HatchBoundaryPath>.BeforeRemoveItemEventHandler(this.BoundaryPaths_BeforeRemoveItem);
            this.boundaryPaths.RemoveItem += new ObservableCollection<HatchBoundaryPath>.RemoveItemEventHandler(this.BoundaryPaths_RemoveItem);
            this.associative = associative;
        }

        public Hatch(HatchPattern pattern, IEnumerable<HatchBoundaryPath> paths, bool associative) : base(EntityType.Hatch, "HATCH")
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            this.pattern = pattern;
            this.boundaryPaths = new ObservableCollection<HatchBoundaryPath>();
            this.boundaryPaths.BeforeAddItem += new ObservableCollection<HatchBoundaryPath>.BeforeAddItemEventHandler(this.BoundaryPaths_BeforeAddItem);
            this.boundaryPaths.AddItem += new ObservableCollection<HatchBoundaryPath>.AddItemEventHandler(this.BoundaryPaths_AddItem);
            this.boundaryPaths.BeforeRemoveItem += new ObservableCollection<HatchBoundaryPath>.BeforeRemoveItemEventHandler(this.BoundaryPaths_BeforeRemoveItem);
            this.boundaryPaths.RemoveItem += new ObservableCollection<HatchBoundaryPath>.RemoveItemEventHandler(this.BoundaryPaths_RemoveItem);
            this.associative = associative;
            foreach (HatchBoundaryPath path in paths)
            {
                if (!this.associative)
                {
                    path.ClearContour();
                }
                this.boundaryPaths.Add(path);
            }
        }

        private void BoundaryPaths_AddItem(ObservableCollection<HatchBoundaryPath> sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
        {
            if (this.associative)
            {
                foreach (EntityObject obj2 in e.Item.Entities)
                {
                    obj2.AddReactor(this);
                }
            }
            else
            {
                e.Item.ClearContour();
            }
            this.OnHatchBoundaryPathAddedEvent(e.Item);
        }

        private void BoundaryPaths_BeforeAddItem(ObservableCollection<HatchBoundaryPath> sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
        {
            if (e.Item == null)
            {
                e.Cancel = true;
            }
            else if (this.boundaryPaths.Contains(e.Item))
            {
                e.Cancel = true;
            }
            e.Cancel = false;
        }

        private void BoundaryPaths_BeforeRemoveItem(ObservableCollection<HatchBoundaryPath> sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
        {
        }

        private void BoundaryPaths_RemoveItem(ObservableCollection<HatchBoundaryPath> sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
        {
            if (this.associative)
            {
                foreach (EntityObject obj2 in e.Item.Entities)
                {
                    obj2.RemoveReactor(this);
                }
            }
            this.OnHatchBoundaryPathRemovedEvent(e.Item);
        }

        public override object Clone()
        {
            Hatch hatch = new Hatch((HatchPattern) this.pattern.Clone(), false) {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Elevation = this.elevation
            };
            foreach (HatchBoundaryPath path in this.boundaryPaths)
            {
                hatch.boundaryPaths.Add((HatchBoundaryPath) path.Clone());
            }
            foreach (XData data in base.XData.Values)
            {
                hatch.XData.Add((XData) data.Clone());
            }
            return hatch;
        }

        public List<EntityObject> CreateBoundary(bool linkBoundary)
        {
            if (this.associative)
            {
                this.UnLinkBoundary();
            }
            this.associative = linkBoundary;
            List<EntityObject> list = new List<EntityObject>();
            Matrix3 trans = MathHelper.ArbitraryAxis(base.Normal);
            Vector3 pos = (Vector3) (trans * new Vector3(0.0, 0.0, this.elevation));
            foreach (HatchBoundaryPath path in this.boundaryPaths)
            {
                foreach (HatchBoundaryPath.Edge edge in path.Edges)
                {
                    EntityObject entity = edge.ConvertTo();
                    switch (entity.Type)
                    {
                        case EntityType.Line:
                            list.Add(ProcessLine((netDxf.Entities.Line) entity, trans, pos));
                            break;

                        case EntityType.Spline:
                            list.Add(ProcessSpline((netDxf.Entities.Spline) entity, trans, pos));
                            break;

                        case EntityType.Arc:
                            list.Add(ProcessArc((netDxf.Entities.Arc) entity, trans, pos));
                            break;

                        case EntityType.Circle:
                            list.Add(ProcessCircle((Circle) entity, trans, pos));
                            break;

                        case EntityType.Ellipse:
                            list.Add(ProcessEllipse((netDxf.Entities.Ellipse) entity, trans, pos));
                            break;

                        case EntityType.LightWeightPolyline:
                            list.Add(ProcessLwPolyline((LwPolyline) entity, base.Normal, this.elevation));
                            break;
                    }
                    if (this.associative)
                    {
                        path.AddContour(entity);
                        entity.AddReactor(this);
                        this.OnHatchBoundaryPathAddedEvent(path);
                    }
                }
            }
            return list;
        }

        protected virtual void OnHatchBoundaryPathAddedEvent(HatchBoundaryPath item)
        {
            HatchBoundaryPathAddedEventHandler hatchBoundaryPathAdded = this.HatchBoundaryPathAdded;
            if (hatchBoundaryPathAdded > null)
            {
                hatchBoundaryPathAdded(this, new ObservableCollectionEventArgs<HatchBoundaryPath>(item));
            }
        }

        protected virtual void OnHatchBoundaryPathRemovedEvent(HatchBoundaryPath item)
        {
            HatchBoundaryPathRemovedEventHandler hatchBoundaryPathRemoved = this.HatchBoundaryPathRemoved;
            if (hatchBoundaryPathRemoved > null)
            {
                hatchBoundaryPathRemoved(this, new ObservableCollectionEventArgs<HatchBoundaryPath>(item));
            }
        }

        private static EntityObject ProcessArc(netDxf.Entities.Arc arc, Matrix3 trans, Vector3 pos)
        {
            arc.Center = ((Vector3) (trans * arc.Center)) + pos;
            arc.Normal = (Vector3) (trans * arc.Normal);
            return arc;
        }

        private static EntityObject ProcessCircle(Circle circle, Matrix3 trans, Vector3 pos)
        {
            circle.Center = ((Vector3) (trans * circle.Center)) + pos;
            circle.Normal = (Vector3) (trans * circle.Normal);
            return circle;
        }

        private static netDxf.Entities.Ellipse ProcessEllipse(netDxf.Entities.Ellipse ellipse, Matrix3 trans, Vector3 pos)
        {
            ellipse.Center = ((Vector3) (trans * ellipse.Center)) + pos;
            ellipse.Normal = (Vector3) (trans * ellipse.Normal);
            return ellipse;
        }

        private static netDxf.Entities.Line ProcessLine(netDxf.Entities.Line line, Matrix3 trans, Vector3 pos)
        {
            line.StartPoint = ((Vector3) (trans * line.StartPoint)) + pos;
            line.EndPoint = ((Vector3) (trans * line.EndPoint)) + pos;
            line.Normal = (Vector3) (trans * line.Normal);
            return line;
        }

        private static LwPolyline ProcessLwPolyline(LwPolyline polyline, Vector3 normal, double elevation)
        {
            polyline.Elevation = elevation;
            polyline.Normal = normal;
            return polyline;
        }

        private static netDxf.Entities.Spline ProcessSpline(netDxf.Entities.Spline spline, Matrix3 trans, Vector3 pos)
        {
            foreach (SplineVertex vertex in spline.ControlPoints)
            {
                vertex.Position = ((Vector3) (trans * vertex.Position)) + pos;
            }
            spline.Normal = (Vector3) (trans * spline.Normal);
            return spline;
        }

        public List<EntityObject> UnLinkBoundary()
        {
            List<EntityObject> list = new List<EntityObject>();
            this.associative = false;
            foreach (HatchBoundaryPath path in this.boundaryPaths)
            {
                foreach (EntityObject obj2 in path.Entities)
                {
                    obj2.RemoveReactor(this);
                    list.Add(obj2);
                }
                path.ClearContour();
            }
            return list;
        }

        public HatchPattern Pattern
        {
            get => 
                this.pattern;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.pattern = value;
            }
        }

        public ObservableCollection<HatchBoundaryPath> BoundaryPaths =>
            this.boundaryPaths;

        public bool Associative =>
            this.associative;

        public double Elevation
        {
            get => 
                this.elevation;
            set => 
                (this.elevation = value);
        }

        public delegate void HatchBoundaryPathAddedEventHandler(Hatch sender, ObservableCollectionEventArgs<HatchBoundaryPath> e);

        public delegate void HatchBoundaryPathRemovedEventHandler(Hatch sender, ObservableCollectionEventArgs<HatchBoundaryPath> e);
    }
}

