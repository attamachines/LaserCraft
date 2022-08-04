namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    public class MText : EntityObject
    {
        private Vector3 position;
        private double rectangleWidth;
        private double height;
        private double rotation;
        private double lineSpacing;
        private double paragraphHeightFactor;
        private MTextLineSpacingStyle lineSpacingStyle;
        private MTextAttachmentPoint attachmentPoint;
        private TextStyle style;
        private string value;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event TextStyleChangedEventHandler TextStyleChanged;

        public MText() : this(string.Empty, Vector3.Zero, 1.0, 0.0, TextStyle.Default)
        {
        }

        public MText(Vector2 position, double height, double rectangleWidth) : this(string.Empty, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, TextStyle.Default)
        {
        }

        public MText(Vector3 position, double height, double rectangleWidth) : this(string.Empty, position, height, rectangleWidth, TextStyle.Default)
        {
        }

        public MText(Vector2 position, double height, double rectangleWidth, TextStyle style) : this(string.Empty, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, style)
        {
        }

        public MText(Vector3 position, double height, double rectangleWidth, TextStyle style) : this(string.Empty, position, height, rectangleWidth, style)
        {
        }

        public MText(string text, Vector2 position, double height, double rectangleWidth) : this(text, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, TextStyle.Default)
        {
        }

        public MText(string text, Vector3 position, double height, double rectangleWidth) : this(text, position, height, rectangleWidth, TextStyle.Default)
        {
        }

        public MText(string text, Vector2 position, double height, double rectangleWidth, TextStyle style) : this(text, new Vector3(position.X, position.Y, 0.0), height, rectangleWidth, style)
        {
        }

        public MText(string text, Vector3 position, double height, double rectangleWidth, TextStyle style) : base(EntityType.MText, "MTEXT")
        {
            this.value = text;
            this.position = position;
            this.attachmentPoint = MTextAttachmentPoint.TopLeft;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            this.style = style;
            this.rectangleWidth = rectangleWidth;
            if (height <= 0.0)
            {
                throw new ArgumentOutOfRangeException("height", this.value, "The MText height must be greater than zero.");
            }
            this.height = height;
            this.lineSpacing = 1.0;
            this.paragraphHeightFactor = 1.0;
            this.lineSpacingStyle = MTextLineSpacingStyle.AtLeast;
            this.rotation = 0.0;
        }

        public override object Clone()
        {
            MText text = new MText {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Position = this.position,
                Rotation = this.rotation,
                Height = this.height,
                LineSpacingFactor = this.lineSpacing,
                ParagraphHeightFactor = this.paragraphHeightFactor,
                LineSpacingStyle = this.lineSpacingStyle,
                RectangleWidth = this.rectangleWidth,
                AttachmentPoint = this.attachmentPoint,
                Style = (TextStyle) this.style.Clone(),
                Value = this.value
            };
            foreach (XData data in base.XData.Values)
            {
                text.XData.Add((XData) data.Clone());
            }
            return text;
        }

        public void EndParagraph()
        {
            if (!MathHelper.IsOne(this.paragraphHeightFactor))
            {
                object[] objArray1 = new object[] { this.value, @"{\H", this.paragraphHeightFactor, @"x;}\P" };
                this.value = string.Concat(objArray1);
            }
            else
            {
                this.value = this.value + @"\P";
            }
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

        public string PlainText()
        {
            if (string.IsNullOrEmpty(this.value))
            {
                return string.Empty;
            }
            string str = this.value;
            StringBuilder builder = new StringBuilder();
            CharEnumerator enumerator = str.GetEnumerator();
            while (enumerator.MoveNext())
            {
                char current = enumerator.Current;
                if (current == '\\')
                {
                    if (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                    }
                    else
                    {
                        return builder.ToString();
                    }
                    if (((current == '\\') | (current == '{')) | (current == '}'))
                    {
                        builder.Append(current);
                    }
                    else if ((((((((current == 'L') | (current == 'l')) | (current == 'O')) | (current == 'o')) | (current == 'K')) | (current == 'k')) | (current == 'P')) | (current == 'X'))
                    {
                        if (current == 'P')
                        {
                            builder.Append(Environment.NewLine);
                        }
                    }
                    else
                    {
                        bool flag7 = current == 'S';
                        while (current != ';')
                        {
                            if (enumerator.MoveNext())
                            {
                                current = enumerator.Current;
                            }
                            else
                            {
                                return builder.ToString();
                            }
                            if (flag7 && (current != ';'))
                            {
                                builder.Append(current);
                            }
                        }
                    }
                }
                else if (!((current == '{') | (current == '}')))
                {
                    builder.Append(current);
                }
            }
            return builder.ToString();
        }

        public void Write(string text)
        {
            this.Write(text, null);
        }

        public void Write(string text, MTextFormattingOptions options)
        {
            if (options == null)
            {
                this.value = this.value + text;
            }
            else
            {
                this.value = this.value + options.FormatText(text);
            }
        }

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = MathHelper.NormalizeAngle(value));
        }

        public double Height
        {
            get => 
                this.height;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The MText height must be greater than zero.");
                }
                this.height = value;
            }
        }

        public double LineSpacingFactor
        {
            get => 
                this.lineSpacing;
            set
            {
                if ((value < 0.25) || (value > 4.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The MText LineSpacingFactor valid values range from 0.25 to 4.00");
                }
                this.lineSpacing = value;
            }
        }

        public double ParagraphHeightFactor
        {
            get => 
                this.paragraphHeightFactor;
            set
            {
                if ((value < 0.25) || (value > 4.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The MText ParagraphHeightFactor valid values range from 0.25 to 4.00");
                }
                this.paragraphHeightFactor = value;
            }
        }

        public MTextLineSpacingStyle LineSpacingStyle
        {
            get => 
                this.lineSpacingStyle;
            set => 
                (this.lineSpacingStyle = value);
        }

        public double RectangleWidth
        {
            get => 
                this.rectangleWidth;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The MText rectangle width must be equals or greater than zero.");
                }
                this.rectangleWidth = value;
            }
        }

        public MTextAttachmentPoint AttachmentPoint
        {
            get => 
                this.attachmentPoint;
            set => 
                (this.attachmentPoint = value);
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

        public string Value
        {
            get => 
                this.value;
            set => 
                (this.value = value);
        }

        public delegate void TextStyleChangedEventHandler(MText sender, TableObjectChangedEventArgs<TextStyle> e);
    }
}

