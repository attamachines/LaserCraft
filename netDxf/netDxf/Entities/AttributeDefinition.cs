namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class AttributeDefinition : EntityObject
    {
        private readonly string tag;
        private string prompt;
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
        public event TextStyleChangedEventHandler TextStyleChange;

        public AttributeDefinition(string tag) : this(tag, TextStyle.Default)
        {
        }

        public AttributeDefinition(string tag, TextStyle style) : base(EntityType.AttributeDefinition, "ATTDEF")
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }
            this.tag = tag;
            this.flags = AttributeFlags.Visible;
            this.prompt = string.Empty;
            this.value = null;
            this.position = Vector3.Zero;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            this.style = style;
            this.height = MathHelper.IsZero(style.Height) ? 1.0 : style.Height;
            this.widthFactor = style.WidthFactor;
            this.obliqueAngle = style.ObliqueAngle;
            this.rotation = 0.0;
            this.alignment = TextAlignment.BaselineLeft;
        }

        public AttributeDefinition(string tag, double textHeight, TextStyle style) : base(EntityType.AttributeDefinition, "ATTDEF")
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }
            if (tag.Contains(" "))
            {
                throw new ArgumentException("The tag string cannot contain spaces.", "tag");
            }
            this.tag = tag;
            this.flags = AttributeFlags.Visible;
            this.prompt = string.Empty;
            this.value = null;
            this.position = Vector3.Zero;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            this.style = style;
            if (textHeight <= 0.0)
            {
                throw new ArgumentOutOfRangeException("textHeight", this.value, "The attribute definition text height must be greater than zero.");
            }
            this.height = textHeight;
            this.widthFactor = style.WidthFactor;
            this.obliqueAngle = style.ObliqueAngle;
            this.rotation = 0.0;
            this.alignment = TextAlignment.BaselineLeft;
        }

        public override object Clone()
        {
            AttributeDefinition definition = new AttributeDefinition(this.tag) {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Prompt = this.prompt,
                Value = this.value,
                Height = this.height,
                WidthFactor = this.widthFactor,
                ObliqueAngle = this.obliqueAngle,
                Style = this.style,
                Position = this.position,
                Flags = this.flags,
                Rotation = this.rotation,
                Alignment = this.alignment
            };
            foreach (XData data in base.XData.Values)
            {
                definition.XData.Add((XData) data.Clone());
            }
            return definition;
        }

        protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler textStyleChange = this.TextStyleChange;
            if (textStyleChange > null)
            {
                TableObjectChangedEventArgs<TextStyle> e = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
                textStyleChange(this, e);
                return e.NewValue;
            }
            return newTextStyle;
        }

        public string Tag =>
            this.tag;

        public string Prompt
        {
            get => 
                this.prompt;
            set => 
                (this.prompt = value);
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

        public delegate void TextStyleChangedEventHandler(AttributeDefinition sender, TableObjectChangedEventArgs<TextStyle> e);
    }
}

