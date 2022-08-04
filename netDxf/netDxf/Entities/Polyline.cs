namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Collections;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class Polyline : EntityObject
    {
        private readonly netDxf.Entities.EndSequence endSequence;
        private readonly ObservableCollection<PolylineVertex> vertexes;
        private PolylinetypeFlags flags;
        private PolylineSmoothType smoothType;

        public Polyline() : this(new List<PolylineVertex>(), false)
        {
        }

        public Polyline(IEnumerable<PolylineVertex> vertexes) : this(vertexes, false)
        {
        }

        public Polyline(IEnumerable<Vector3> vertexes) : this(vertexes, false)
        {
        }

        public Polyline(IEnumerable<PolylineVertex> vertexes, bool isClosed) : base(EntityType.Polyline, "POLYLINE")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new ObservableCollection<PolylineVertex>();
            this.vertexes.BeforeAddItem += new ObservableCollection<PolylineVertex>.BeforeAddItemEventHandler(this.Vertexes_BeforeAddItem);
            this.vertexes.AddItem += new ObservableCollection<PolylineVertex>.AddItemEventHandler(this.Vertexes_AddItem);
            this.vertexes.BeforeRemoveItem += new ObservableCollection<PolylineVertex>.BeforeRemoveItemEventHandler(this.Vertexes_BeforeRemoveItem);
            this.vertexes.RemoveItem += new ObservableCollection<PolylineVertex>.RemoveItemEventHandler(this.Vertexes_RemoveItem);
            this.vertexes.AddRange(vertexes);
            this.flags = isClosed ? (PolylinetypeFlags.Polyline3D | PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM) : PolylinetypeFlags.Polyline3D;
            this.smoothType = PolylineSmoothType.NoSmooth;
            this.endSequence = new netDxf.Entities.EndSequence(this);
        }

        public Polyline(IEnumerable<Vector3> vertexes, bool isClosed) : base(EntityType.Polyline, "POLYLINE")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new ObservableCollection<PolylineVertex>();
            this.vertexes.BeforeAddItem += new ObservableCollection<PolylineVertex>.BeforeAddItemEventHandler(this.Vertexes_BeforeAddItem);
            this.vertexes.AddItem += new ObservableCollection<PolylineVertex>.AddItemEventHandler(this.Vertexes_AddItem);
            this.vertexes.BeforeRemoveItem += new ObservableCollection<PolylineVertex>.BeforeRemoveItemEventHandler(this.Vertexes_BeforeRemoveItem);
            this.vertexes.RemoveItem += new ObservableCollection<PolylineVertex>.RemoveItemEventHandler(this.Vertexes_RemoveItem);
            foreach (Vector3 vector in vertexes)
            {
                this.vertexes.Add(new PolylineVertex(vector));
            }
            this.flags = isClosed ? (PolylinetypeFlags.Polyline3D | PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM) : PolylinetypeFlags.Polyline3D;
            this.smoothType = PolylineSmoothType.NoSmooth;
            this.endSequence = new netDxf.Entities.EndSequence(this);
        }

        internal override long AsignHandle(long entityNumber)
        {
            foreach (PolylineVertex vertex in this.vertexes)
            {
                entityNumber = vertex.AsignHandle(entityNumber);
            }
            entityNumber = this.endSequence.AsignHandle(entityNumber);
            return base.AsignHandle(entityNumber);
        }

        public override object Clone()
        {
            netDxf.Entities.Polyline polyline = new netDxf.Entities.Polyline {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Flags = this.flags
            };
            foreach (PolylineVertex vertex in this.vertexes)
            {
                polyline.Vertexes.Add((PolylineVertex) vertex.Clone());
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
            foreach (PolylineVertex vertex in this.Vertexes)
            {
                Vector3 position;
                Vector3 position;
                if (num == (this.Vertexes.Count - 1))
                {
                    if (!this.IsClosed)
                    {
                        return list;
                    }
                    position = vertex.Position;
                    position = this.vertexes[0].Position;
                }
                else
                {
                    position = vertex.Position;
                    position = this.vertexes[num + 1].Position;
                }
                netDxf.Entities.Line item = new netDxf.Entities.Line {
                    Layer = (Layer) base.Layer.Clone(),
                    Linetype = (Linetype) base.Linetype.Clone(),
                    Color = (AciColor) base.Color.Clone(),
                    Lineweight = base.Lineweight,
                    Transparency = (Transparency) base.Transparency.Clone(),
                    LinetypeScale = base.LinetypeScale,
                    Normal = base.Normal,
                    StartPoint = position,
                    EndPoint = position
                };
                list.Add(item);
                num++;
            }
            return list;
        }

        public void Reverse()
        {
            this.vertexes.Reverse();
        }

        private void Vertexes_AddItem(ObservableCollection<PolylineVertex> sender, ObservableCollectionEventArgs<PolylineVertex> e)
        {
            if (base.Owner > null)
            {
                DxfDocument owner = base.Owner.Record.Owner.Owner;
                owner.NumHandles = e.Item.AsignHandle(owner.NumHandles);
            }
            e.Item.Owner = this;
        }

        private void Vertexes_BeforeAddItem(ObservableCollection<PolylineVertex> sender, ObservableCollectionEventArgs<PolylineVertex> e)
        {
            if (e.Item == null)
            {
                e.Cancel = true;
            }
            else if (e.Item.Owner > null)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Vertexes_BeforeRemoveItem(ObservableCollection<PolylineVertex> sender, ObservableCollectionEventArgs<PolylineVertex> e)
        {
        }

        private void Vertexes_RemoveItem(ObservableCollection<PolylineVertex> sender, ObservableCollectionEventArgs<PolylineVertex> e)
        {
            e.Item.Handle = null;
            e.Item.Owner = null;
        }

        public ObservableCollection<PolylineVertex> Vertexes =>
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

        internal PolylineSmoothType SmoothType
        {
            get => 
                this.smoothType;
            set => 
                (this.smoothType = value);
        }

        internal PolylinetypeFlags Flags
        {
            get => 
                this.flags;
            set => 
                (this.flags = value);
        }

        internal netDxf.Entities.EndSequence EndSequence =>
            this.endSequence;
    }
}

