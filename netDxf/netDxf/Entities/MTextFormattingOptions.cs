namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Tables;
    using System;

    public class MTextFormattingOptions
    {
        private bool bold = false;
        private bool italic = false;
        private bool overline = false;
        private bool underline = false;
        private bool strikeThrough;
        private AciColor color = null;
        private string fontName = null;
        private TextAligment aligment = TextAligment.Default;
        private double heightFactor = 1.0;
        private double obliqueAngle = 0.0;
        private double characterSpaceFactor = 1.0;
        private double widthFactor = 1.0;
        private readonly TextStyle style;

        public MTextFormattingOptions(TextStyle style)
        {
            this.style = style;
        }

        public string FormatText(string text)
        {
            string str = text;
            if (this.overline)
            {
                str = $"\O{str}\o";
            }
            if (this.underline)
            {
                str = $"\L{str}\l";
            }
            if (this.strikeThrough)
            {
                str = $"\K{str}\k";
            }
            if (this.color > null)
            {
                str = this.color.UseTrueColor ? $"\C{this.color.Index};\c{AciColor.ToTrueColor(this.color)};{str}" : $"\C{this.color.Index};{str}";
            }
            if (this.fontName > null)
            {
                if (this.bold && this.italic)
                {
                    str = $"\f{this.fontName}|b1|i1;{str}";
                }
                else if (this.bold && !this.italic)
                {
                    str = $"\f{this.fontName}|b1|i0;{str}";
                }
                else if (!this.bold && this.italic)
                {
                    str = $"\f{this.fontName}|i1|b0;{str}";
                }
                else
                {
                    str = $"\F{this.fontName};{str}";
                }
            }
            else
            {
                if (this.bold && this.italic)
                {
                    str = $"\f{this.style.FontFamilyName}|b1|i1;{str}";
                }
                if (this.bold && !this.italic)
                {
                    str = $"\f{this.style.FontFamilyName}|b1|i0;{str}";
                }
                if (!this.bold && this.italic)
                {
                    str = $"\f{this.style.FontFamilyName}|i1|b0;{str}";
                }
            }
            if (this.aligment != TextAligment.Default)
            {
                str = $"\A{(int) this.aligment};{str}";
            }
            if (!MathHelper.IsOne(this.heightFactor))
            {
                str = $"\H{this.heightFactor}x;{str}";
            }
            if (!MathHelper.IsZero(this.obliqueAngle))
            {
                str = $"\Q{this.obliqueAngle};{str}";
            }
            if (!MathHelper.IsOne(this.characterSpaceFactor))
            {
                str = $"\T{this.characterSpaceFactor};{str}";
            }
            if (!MathHelper.IsOne(this.widthFactor))
            {
                str = $"\W{this.widthFactor};{str}";
            }
            return ("{" + str + "}");
        }

        public bool Bold
        {
            get => 
                this.bold;
            set => 
                (this.bold = value);
        }

        public bool Italic
        {
            get => 
                this.italic;
            set => 
                (this.italic = value);
        }

        public bool Overline
        {
            get => 
                this.overline;
            set => 
                (this.overline = value);
        }

        public bool Underline
        {
            get => 
                this.underline;
            set => 
                (this.underline = value);
        }

        public bool StrikeThrough
        {
            get => 
                this.strikeThrough;
            set => 
                (this.strikeThrough = value);
        }

        public AciColor Color
        {
            get => 
                this.color;
            set => 
                (this.color = value);
        }

        public string FontName
        {
            get => 
                this.fontName;
            set => 
                (this.fontName = value);
        }

        public TextAligment Aligment
        {
            get => 
                this.aligment;
            set => 
                (this.aligment = value);
        }

        public double HeightFactor
        {
            get => 
                this.heightFactor;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The character percentage height must be greater than zero.");
                }
                this.heightFactor = value;
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

        public double CharacterSpaceFactor
        {
            get => 
                this.characterSpaceFactor;
            set
            {
                if ((value < 0.75) || (value > 4.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The character space valid values range from a minimum of .75 to 4");
                }
                this.characterSpaceFactor = value;
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

        public enum TextAligment
        {
            Bottom,
            Center,
            Top,
            Default
        }
    }
}

