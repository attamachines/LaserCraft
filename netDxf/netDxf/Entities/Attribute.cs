namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Attribute : DxfObject, ICloneable
    {
        private AciColor color;
        private netDxf.Tables.Layer layer;
        private netDxf.Tables.Linetype linetype;
        private netDxf.Lineweight lineweight;
        private netDxf.Transparency transparency;
        private double linetypeScale;
        private bool isVisible;
        private Vector3 normal;
        private AttributeDefinition definition;
        private string tag;
        private object value;
        private TextStyle style;
        private Vector3 position;
        private AttributeFlags flags;
        private double height;
        private double widthFactor;
        private double obliqueAngle;
        private double rotation;
        private TextAlignment alignment;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LayerChangedEventHandler LayerChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LinetypeChangedEventHandler LinetypeChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event TextStyleChangedEventHandler TextStyleChanged;

        internal Attribute() : base("ATTRIB")
        {
        }

        public Attribute(AttributeDefinition definition) : base("ATTRIB")
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            this.color = definition.Color;
            this.layer = definition.Layer;
            this.linetype = definition.Linetype;
            this.lineweight = definition.Lineweight;
            this.linetypeScale = definition.LinetypeScale;
            this.transparency = definition.Transparency;
            this.isVisible = definition.IsVisible;
            this.normal = definition.Normal;
            this.definition = definition;
            this.tag = definition.Tag;
            this.value = definition.Value;
            this.style = definition.Style;
            this.position = definition.Position;
            this.flags = definition.Flags;
            this.height = definition.Height;
            this.widthFactor = definition.WidthFactor;
            this.obliqueAngle = definition.ObliqueAngle;
            this.rotation = definition.Rotation;
            this.alignment = definition.Alignment;
        }

        public object Clone() => 
            new netDxf.Entities.Attribute { 
                Layer = (netDxf.Tables.Layer) this.Layer.Clone(),
                Linetype = (netDxf.Tables.Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (netDxf.Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.isVisible,
                Definition = (AttributeDefinition) this.definition.Clone(),
                Tag = this.tag,
                Height = this.height,
                WidthFactor = this.widthFactor,
                ObliqueAngle = this.obliqueAngle,
                Value = this.value,
                Style = this.style,
                Position = this.position,
                Flags = this.flags,
                Rotation = this.rotation,
                Alignment = this.alignment
            };

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

        protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler textStyleChanged = this.TextStyleChanged;
            if (textStyleChanged > null)
            {
                TableObjectChangedEventArgs<TextStyle> e = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
                textStyleChanged(this, e);
                return e.NewValue;
            }
            return newTextStyle;
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

        public Insert Owner
        {
            get => 
                ((Insert) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public AttributeDefinition Definition
        {
            get => 
                this.definition;
            internal set => 
                (this.definition = value);
        }

        public string Tag
        {
            get => 
                this.tag;
            internal set => 
                (this.tag = value);
        }

        public double Height
        {
            get => 
                this.height;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The height should be greater than zero.");
                }
                this.height = value;
            }
        }

        public double WidthFactor
        {
            get => 
                this.widthFactor;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The width factor should be greater than zero.");
                }
                this.widthFactor = value;
            }
        }

        public double ObliqueAngle
        {
            get => 
                this.obliqueAngle;
            set
            {
                if ((value < -85.0) || (value > 85.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The oblique angle valid values range from -85 to 85.");
                }
                this.obliqueAngle = value;
            }
        }

        public object Value
        {
            get => 
                this.value;
            set => 
                (this.value = value);
        }

        public TextStyle Style
        {
            get => 
                this.style;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.style = this.OnTextStyleChangedEvent(this.style, value);
            }
        }

        public Vector3 Position
        {
            get => 
                this.position;
            set => 
                (this.position = value);
        }

        public AttributeFlags Flags
        {
            get => 
                this.flags;
            set => 
                (this.flags = value);
        }

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = MathHelper.NormalizeAngle(value));
        }

        public TextAlignment Alignment
        {
            get => 
                this.alignment;
            set => 
                (this.alignment = value);
        }

        public delegate void LayerChangedEventHandler(netDxf.Entities.Attribute sender, TableObjectChangedEventArgs<Layer> e);

        public delegate void LinetypeChangedEventHandler(netDxf.Entities.Attribute sender, TableObjectChangedEventArgs<Linetype> e);

        public delegate void TextStyleChangedEventHandler(netDxf.Entities.Attribute sender, TableObjectChangedEventArgs<TextStyle> e);
    }
}

