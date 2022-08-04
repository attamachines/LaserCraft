namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Collections;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class EntityObject : DxfObject, ICloneable
    {
        private readonly EntityType type;
        private AciColor color;
        private netDxf.Tables.Layer layer;
        private netDxf.Tables.Linetype linetype;
        private netDxf.Lineweight lineweight;
        private netDxf.Transparency transparency;
        private double linetypeScale;
        private bool isVisible;
        private Vector3 normal;
        private readonly XDataDictionary xData;
        private readonly List<DxfObject> reactors;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LayerChangedEventHandler LayerChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LinetypeChangedEventHandler LinetypeChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event XDataAddAppRegEventHandler XDataAddAppReg;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event XDataRemoveAppRegEventHandler XDataRemoveAppReg;

        protected EntityObject(EntityType type, string dxfCode) : base(dxfCode)
        {
            this.type = type;
            this.color = AciColor.ByLayer;
            this.layer = netDxf.Tables.Layer.Default;
            this.linetype = netDxf.Tables.Linetype.ByLayer;
            this.lineweight = netDxf.Lineweight.ByLayer;
            this.transparency = netDxf.Transparency.ByLayer;
            this.linetypeScale = 1.0;
            this.isVisible = true;
            this.normal = Vector3.UnitZ;
            this.reactors = new List<DxfObject>();
            this.xData = new XDataDictionary();
            this.xData.AddAppReg += new XDataDictionary.AddAppRegEventHandler(this.XData_AddAppReg);
            this.xData.RemoveAppReg += new XDataDictionary.RemoveAppRegEventHandler(this.XData_RemoveAppReg);
        }

        internal void AddReactor(DxfObject o)
        {
            this.reactors.Add(o);
        }

        public abstract object Clone();
        protected virtual netDxf.Tables.Layer OnLayerChangedEvent(netDxf.Tables.Layer oldLayer, netDxf.Tables.Layer newLayer)
        {
            LayerChangedEventHandler layerChanged = this.LayerChanged;
            if (layerChanged > null)
            {
                TableObjectChangedEventArgs<netDxf.Tables.Layer> e = new TableObjectChangedEventArgs<netDxf.Tables.Layer>(oldLayer, newLayer);
                layerChanged(this, e);
                return e.NewValue;
            }
            return newLayer;
        }

        protected virtual netDxf.Tables.Linetype OnLinetypeChangedEvent(netDxf.Tables.Linetype oldLinetype, netDxf.Tables.Linetype newLinetype)
        {
            LinetypeChangedEventHandler linetypeChanged = this.LinetypeChanged;
            if (linetypeChanged > null)
            {
                TableObjectChangedEventArgs<netDxf.Tables.Linetype> e = new TableObjectChangedEventArgs<netDxf.Tables.Linetype>(oldLinetype, newLinetype);
                linetypeChanged(this, e);
                return e.NewValue;
            }
            return newLinetype;
        }

        protected virtual void OnXDataAddAppRegEvent(ApplicationRegistry item)
        {
            XDataAddAppRegEventHandler xDataAddAppReg = this.XDataAddAppReg;
            if (xDataAddAppReg > null)
            {
                xDataAddAppReg(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
            }
        }

        protected virtual void OnXDataRemoveAppRegEvent(ApplicationRegistry item)
        {
            XDataRemoveAppRegEventHandler xDataRemoveAppReg = this.XDataRemoveAppReg;
            if (xDataRemoveAppReg > null)
            {
                xDataRemoveAppReg(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
            }
        }

        internal bool RemoveReactor(DxfObject o) => 
            this.reactors.Remove(o);

        public override string ToString() => 
            this.type.ToString();

        private void XData_AddAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {
            this.OnXDataAddAppRegEvent(e.Item);
        }

        private void XData_RemoveAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {
            this.OnXDataRemoveAppRegEvent(e.Item);
        }

        public IReadOnlyList<DxfObject> Reactors =>
            this.reactors;

        public EntityType Type =>
            this.type;

        public AciColor Color
        {
            get => 
                this.color;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.color = value;
            }
        }

        public netDxf.Tables.Layer Layer
        {
            get => 
                this.layer;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.layer = this.OnLayerChangedEvent(this.layer, value);
            }
        }

        public netDxf.Tables.Linetype Linetype
        {
            get => 
                this.linetype;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.linetype = this.OnLinetypeChangedEvent(this.linetype, value);
            }
        }

        public netDxf.Lineweight Lineweight
        {
            get => 
                this.lineweight;
            set => 
                (this.lineweight = value);
        }

        public netDxf.Transparency Transparency
        {
            get => 
                this.transparency;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.transparency = value;
            }
        }

        public double LinetypeScale
        {
            get => 
                this.linetypeScale;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The line type scale must be greater than zero.");
                }
                this.linetypeScale = value;
            }
        }

        public bool IsVisible
        {
            get => 
                this.isVisible;
            set => 
                (this.isVisible = value);
        }

        public Vector3 Normal
        {
            get => 
                this.normal;
            set
            {
                this.normal = Vector3.Normalize(value);
                if (Vector3.IsNaN(this.normal))
                {
                    throw new ArgumentException("The normal can not be the zero vector.", "value");
                }
            }
        }

        public Block Owner
        {
            get => 
                ((Block) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public XDataDictionary XData =>
            this.xData;

        public delegate void LayerChangedEventHandler(EntityObject sender, TableObjectChangedEventArgs<Layer> e);

        public delegate void LinetypeChangedEventHandler(EntityObject sender, TableObjectChangedEventArgs<Linetype> e);

        public delegate void XDataAddAppRegEventHandler(EntityObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e);

        public delegate void XDataRemoveAppRegEventHandler(EntityObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e);
    }
}

