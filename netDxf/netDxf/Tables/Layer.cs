namespace netDxf.Tables
{
    using netDxf;
    using netDxf.Collections;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Layer : TableObject
    {
        private AciColor color;
        private bool isVisible;
        private bool isFrozen;
        private bool isLocked;
        private bool plot;
        private netDxf.Tables.Linetype linetype;
        private netDxf.Lineweight lineweight;
        private netDxf.Transparency transparency;
        public const string DefaultName = "0";

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LinetypeChangedEventHandler LinetypeChanged;

        public Layer(string name) : this(name, true)
        {
        }

        internal Layer(string name, bool checkName) : base(name, "LAYER", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The layer name should be at least one character long.");
            }
            base.IsReserved = name.Equals("0", StringComparison.OrdinalIgnoreCase);
            this.color = AciColor.Default;
            this.linetype = netDxf.Tables.Linetype.Continuous;
            this.isVisible = true;
            this.plot = true;
            this.lineweight = netDxf.Lineweight.Default;
            this.transparency = new netDxf.Transparency(0);
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new Layer(newName) { 
                Color = (AciColor) this.Color.Clone(),
                IsVisible = this.isVisible,
                IsFrozen = this.isFrozen,
                IsLocked = this.isLocked,
                Plot = this.plot,
                Linetype = (netDxf.Tables.Linetype) this.Linetype.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (netDxf.Transparency) this.Transparency.Clone()
            };

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

        public static Layer Default =>
            new Layer("0");

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
                if (value.IsByLayer || value.IsByBlock)
                {
                    throw new ArgumentException("The layer color cannot be ByLayer or ByBlock", "value");
                }
                this.color = value;
            }
        }

        public bool IsVisible
        {
            get => 
                this.isVisible;
            set => 
                (this.isVisible = value);
        }

        public bool IsFrozen
        {
            get => 
                this.isFrozen;
            set => 
                (this.isFrozen = value);
        }

        public bool IsLocked
        {
            get => 
                this.isLocked;
            set => 
                (this.isLocked = value);
        }

        public bool Plot
        {
            get => 
                this.plot;
            set => 
                (this.plot = value);
        }

        public netDxf.Lineweight Lineweight
        {
            get => 
                this.lineweight;
            set
            {
                if ((value == netDxf.Lineweight.ByLayer) || (value == netDxf.Lineweight.ByBlock))
                {
                    throw new ArgumentException("The lineweight of a layer cannot be set to ByLayer or ByBlock.", "value");
                }
                this.lineweight = value;
            }
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

        public Layers Owner
        {
            get => 
                ((Layers) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public delegate void LinetypeChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Linetype> e);
    }
}

